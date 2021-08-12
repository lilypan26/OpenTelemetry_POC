using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;

namespace Matrix
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var listener = new ActivityListener
            //{
            //    ShouldListenTo = _ => true,
            //    ActivityStopped = activity =>
            //    {
            //        foreach (var (key, value) in activity.Baggage)
            //        {
            //            activity.AddTag(key, value);
            //        }
            //    }
            //};
            //ActivitySource.AddActivityListener(listener);
            //using var loggerFactory = LoggerFactory.Create(builder => builder
            //.AddOpenTelemetry(options => {
            //    options.AddGenevaLogExporter(options => {
            //        options.ConnectionString = "EtwSession=OpenTelemetry";
            //        options.CustomFields = new List<string> { "foo", "bar" };
            //        options.PrepopulatedFields = new Dictionary<string, object>
            //        {
            //            ["cloud.role"] = "BusyWorker",
            //            ["cloud.roleInstance"] = "CY1SCH030021417",
            //            ["cloud.roleVer"] = "9.0.15289.2",
            //        };
            //    });
            //}));
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
                        options.IncludeScopes = true;
                        options.ParseStateValues = true;
                        options.IncludeFormattedMessage = true;
                        options.AddConsoleExporter();

                    });


                });
    }
}
