using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SuperWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> log;
        private readonly IHostApplicationLifetime appLifetime;
        private readonly IConfiguration config;
        private readonly ExtendedMethods exMethods;
        private readonly Tasks tasks;

        public Worker(ILogger<Worker> logger, IHostApplicationLifetime applicationLifetime, IConfiguration configuration, ExtendedMethods extendedMethods, Tasks tasks)
        {
            log = logger;
            appLifetime = applicationLifetime;
            config = configuration;
            exMethods = extendedMethods;
            this.tasks = tasks;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Stopwatch runtime = new Stopwatch();

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await exMethods.ItIsTimeAsync().ConfigureAwait(false);

                    log.LogDebug("Worker starts");
                    runtime.Restart();

                    await tasks.Task1().ConfigureAwait(false);

                    runtime.Stop();
                    log.LogInformation("Worker completed, time of execution (s): {runetime}", runtime.Elapsed.TotalSeconds);
                }
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, "Error in main worker.");
            }
            finally
            {
                appLifetime.StopApplication();
            }
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