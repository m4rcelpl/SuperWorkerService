using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;

namespace SuperWorkerService
{
    public class Program
    {
        //If service is running in container, exit program on worker error
        private static bool runningInContainer;

        public static void Main(string[] args)
        {
            do
            {
                CreateHostBuilder(args).Build().Run();
            } while (!runningInContainer);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

            //This add support for Ctrl+C to exit program
            .UseConsoleLifetime()
            .ConfigureAppConfiguration((hostContext, configuration) =>
            {
                configuration.AddJsonFile("appsettings.json", false, true);
            })
            .ConfigureLogging((hostContext, loggin) =>
            {
                loggin.ClearProviders();
                loggin.AddConsole();
            })
            .ConfigureServices((hostContext, services) =>
            {
                //DOTNET_RUNNING_IN_CONTAINER is a standard environment variables set to true in all docker .NET images
                runningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") switch
                {
                    "True" => true,
                    "TRUE" => true,
                    "true" => true,
                    _ => false
                };

                //
                //If you need HttpClient, uncomment this
                //requirement - Microsoft.Extensions.Http.Polly
                //
                /*
                services.AddHttpClient("RequestTracker", c =>
                {
                    c.BaseAddress = new Uri("https://github.com/");
                    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                }).AddTransientHttpErrorPolicy(x => x.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(5)));
                */


                //
                //Structured logging to Seq
                //requirement - Seq.Extensions.Logging
                //https://github.com/datalust/seq-extensions-logging
                //
                /*
                services.AddLogging(builder => builder.AddSeq());
                */

                services.AddHostedService<Worker>();
            });
    }
}