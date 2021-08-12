// <copyright file="Program.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;

namespace Examples.AspNetCore
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
                    //webBuilder.UseUrls("http://localhost:5000", "https://localhost:5001");
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
/*
 * .ConfigureLogging((context, builder) =>  
                {
                    builder.ClearProviders();
                    builder.AddConsole();
                    builder.AddOpenTelemetry(options =>
                        options.AddGenevaLogExporter(options => {
                            options.ConnectionString = "EtwSession=OpenTelemetry";
                            options.CustomFields = new List<string> { "foo", "bar" };
                            options.PrepopulatedFields = new Dictionary<string, object>
                            {
                                ["cloud.role"] = "onebox",
                                ["cloud.roleInstance"] = "geneva-otel",
                                ["cloud.roleVer"] = "9.0.15289.2",
                            };
                        }));
                });
 * 
 */