using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SuperWorkerService
{
    public class Tasks
    {
        private readonly ILogger<Tasks> log;
        private readonly IConfiguration config;

        public Tasks(ILogger<Tasks> logger, IConfiguration configuration)
        {
            log = logger;
            config = configuration;
        }

        public async Task Task1()
        {
            log.LogInformation("[{time}] Run Task1", DateTimeOffset.Now);

            Random random = new Random();

            await Task.Delay(random.Next(500, 5000));

            return;
        }
    }
}