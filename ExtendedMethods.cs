using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SuperWorkerService
{
    public class ExtendedMethods
    {
        private readonly ILogger<ExtendedMethods> log;
        private readonly IConfiguration config;

        public ExtendedMethods(ILogger<ExtendedMethods> logger, IConfiguration configuration)
        {
            log = logger;
            config = configuration;
        }

        public async Task ItIsTimeAsync()
        {
            int.TryParse(config["Schedule:ReapeteSec"], out int sec);

            if (sec != 0)
            {
                log.LogInformation($"Next launch at {sec} sec");
                await Task.Delay(TimeSpan.FromSeconds(sec)).ConfigureAwait(false);
                return;
            }

            var runAtTime = config.GetSection("Schedule:RunAtTimeUTC").Get<string[]>() ?? new string[0];

            List<DateTimeOffset> listRunAtTime = new List<DateTimeOffset>();

            foreach (var time in runAtTime)
            {
                DateTimeOffset.TryParse(time, new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal, out DateTimeOffset dateTime);

                if (DateTimeOffset.UtcNow.CompareTo(dateTime) == 1)
                    dateTime = dateTime.AddDays(1);

                listRunAtTime.Add(dateTime);
            }

            var nextExecutionTime = listRunAtTime.OrderBy(e => e.Ticks).FirstOrDefault();

            log.LogInformation($"Next launch at {nextExecutionTime} (UTC)");

            await Task.Delay(TimeSpan.FromTicks(nextExecutionTime.Ticks - DateTimeOffset.UtcNow.Ticks)).ConfigureAwait(false);

            return;
        }
    }
}