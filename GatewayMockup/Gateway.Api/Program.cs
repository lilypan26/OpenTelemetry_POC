using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Api
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

            var cert = new PkitaCertificate();
            cert.ClientId = "Charles";
            cert.CertificateName = "RandomCert";

            Console.WriteLine(cert.ClientId + " " + cert.CertificateName);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    
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
