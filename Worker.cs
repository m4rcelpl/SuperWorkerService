using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SuperWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> log;
        private readonly IHostApplicationLifetime appLifetime;
        private readonly IConfiguration config;

        public Worker(ILogger<Worker> logger, IHostApplicationLifetime applicationLifetime, IConfiguration configuration)
        {
            log = logger;
            this.appLifetime = applicationLifetime;
            this.config = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (!await ItIsTimeAsync().ConfigureAwait(false))
                        continue;

                    log.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, "Error in main worker at {timr}", DateTime.Now);
            }
            finally
            {
                appLifetime.StopApplication();
            }
        }

        private async Task<bool> ItIsTimeAsync()
        {
            int.TryParse(config["Schedule:ReapeteSec"], out int sec);

            if (sec != 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(sec)).ConfigureAwait(false);
                return true;
            }


            var runAtTime = config.GetSection("Schedule:RunAtTime").Get<string[]>() ?? new string[0];

            List<DateTime> listRunAtTime = new List<DateTime>();

            foreach (var time in runAtTime)
            {
                DateTime.TryParse(time, out DateTime dateTime);
                listRunAtTime.Add(dateTime);
            }

            var sortedListRunAtTime = listRunAtTime.OrderBy(e => e.Ticks).ToList<DateTime>();

            //if (dateTime.Ticks != 0)
            //{
            //    TimeSpan elapsedSpan = new TimeSpan(dateTime.Ticks - DateTime.Now.Ticks);

            //    if (elapsedSpan.Seconds <= 0)
            //        elapsedSpan = elapsedSpan.Add(TimeSpan.FromDays(1));

            //    log.LogDebug($"Next worker launch in {elapsedSpan.Hours}:{elapsedSpan.Minutes}:{elapsedSpan.Seconds}");

            //    await Task.Delay(TimeSpan.FromSeconds(elapsedSpan.TotalSeconds)).ConfigureAwait(false);
            //}

            return true;
        }
    }

    ///
    /// To support database you need Dapper and some database connector like MySqlConnector
    /// Requirement: Dapper
    /// https://github.com/StackExchange/Dapper
    /// https://mysqlconnector.net/
    ///
    /*
    public static IEnumerable<T> Query<T>(this IDbConnection cnn, string sql, object param = null, SqlTransaction transaction = null, bool buffered = true);

    public static IEnumerable<dynamic> Query (this IDbConnection cnn, string sql, object param = null, SqlTransaction transaction = null, bool buffered = true);

    public static int Execute(this IDbConnection cnn, string sql, object param = null, SqlTransaction transaction = null);

    var count = connection.Execute(@"insert MyTable(colA, colB) values (@a, @b)",
    new[] { new { a=1, b=1 }, new { a=2, b=2 }, new { a=3, b=3 } }
  );
    */
}