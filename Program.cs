using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Polly;
using System;
using System.Net.Http.Headers;

namespace SuperWorkerService
{
    public class Program
    {
        //If service is running in container, exit program on worker error
        private static bool exitOnError;

        public static void Main(string[] args)
        {
            //If you run in docker you may want exit program on error to restart container
            exitOnError = Environment.GetEnvironmentVariable("DOTNET_EXIT_ON_ERROR") switch
            {
                "true" => true,
                _ => false
            };

            do
            {
                CreateHostBuilder(args).Build().Run();
            } while (exitOnError == false);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

            //This add support for Ctrl+C to exit program
            .UseConsoleLifetime()
            .ConfigureLogging(loggin =>
            {
                loggin.ClearProviders();
                loggin.AddConsole(setting => setting.TimestampFormat = "[dd.MM.yyyy:hh:mm:ss.fff] ");
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTransient(_ => new MySqlConnection(hostContext.Configuration["ConnectionStrings:Main"]));
                services.AddSingleton<ExtendedMethods>();
                services.AddTransient<Tasks>();
                services.AddHostedService<Worker>();


                //
                //If you need HttpClient, uncomment this
                //requirement - Microsoft.Extensions.Http.Polly
                //

                
                services.AddHttpClient("main", c =>
                {
                    //c.BaseAddress = new Uri("https://SOME_WWW/");
                    //c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .AddTransientHttpErrorPolicy(x => x.WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context) =>
                {
                    // Add logic to be executed before each retry, such as logging
                    //Debug.WriteLine($"{exception.Result} and count {retryCount}");
                }));
                

                //
                //Structured logging to Seq
                //requirement - Seq.Extensions.Logging
                //https://github.com/datalust/seq-extensions-logging
                //
                /*
                services.AddLogging(builder => builder.AddSeq());
                */
            });
    }
}