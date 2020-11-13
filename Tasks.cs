using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SuperWorkerService
{
    public class Tasks
    {
        private readonly ILogger<Tasks> log;
        private readonly IConfiguration config;
        private readonly MySqlConnection mySqlCon;
        private readonly IHttpClientFactory httpClientFactory;

        public Tasks(ILogger<Tasks> logger, IConfiguration configuration, MySqlConnection mySqlConnection, IHttpClientFactory httpClientFactory)
        {
            log = logger;
            config = configuration;
            mySqlCon = mySqlConnection;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task Task1()
        {
            log.LogDebug("Run Task1");

            //
            // Database acces with dapper sample
            //

            try
            {
                if (mySqlCon.State == System.Data.ConnectionState.Closed)
                    await mySqlCon.OpenAsync().ConfigureAwait(false);

                var queryResult = await mySqlCon.QueryFirstOrDefaultAsync<string>("SELECT Column1 FROM tabela1 WHERE id = @id", new { id = 2 }).ConfigureAwait(false);

                log.LogInformation($"Database result: {queryResult}");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Database error");
            }
            finally
            {
                await mySqlCon.CloseAsync().ConfigureAwait(false);
            }



            //
            // Http sample
            //
            var client = httpClientFactory.CreateClient("main");

            try
            {
                var result = await client.GetAsync("http://httpstat.us/200");
                result.EnsureSuccessStatusCode();
                var vdesk = await result.Content.ReadAsStringAsync();
                log.LogInformation(vdesk);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error");
            }
           
        }
    }
}