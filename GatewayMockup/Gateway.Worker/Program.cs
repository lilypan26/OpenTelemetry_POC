using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTelemetry.Logs;
using System.Diagnostics;
using OpenTelemetry;

namespace Gateway.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var listener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                ActivityStopped = activity =>
                {
                    foreach (var (key, value) in Baggage.Current)
                    {
                        activity.AddTag(key, value);
                    }
                }
            };
            ActivitySource.AddActivityListener(listener);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:5002", "https://localhost:5003");
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.ClearProviders();
                    builder.AddConsole();

                    builder.AddOpenTelemetry(options =>
                    {
                        options.AddConsoleExporter();
                        //options.AddGenevaLogExporter(options =>
                        //{
                        //    options.ConnectionString = "EtwSession=OpenTelemetry";
                        //    options.CustomFields = new List<string> { "clientId", "operationId" };
                        //    options.PrepopulatedFields = new Dictionary<string, object>
                        //    {
                        //        ["cloud.role"] = "onebox",
                        //        ["cloud.roleInstance"] = "geneva-otel"
                        //    };
                        //});

                    });


                });
    }
}
