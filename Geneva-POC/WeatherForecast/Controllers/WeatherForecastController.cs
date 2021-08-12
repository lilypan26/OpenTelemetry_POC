// <copyright file="WeatherForecastController.cs" company="OpenTelemetry Authors">
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Examples.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using System.Reflection;

namespace Examples.AspNetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        private static readonly HttpClient HttpClient = new HttpClient();

        private static readonly ActivitySource ActivitySource = new ActivitySource("Call");

        private readonly ILogger<WeatherForecastController> logger;

        //private ILoggerFactory loggerFactory;

        GenevaLogger genevaLogger = new GenevaLogger();

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //this.loggerFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }



        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            using var scope = this.logger.BeginScope("{Id}", Guid.NewGuid().ToString("N"));

            // Making an http call here to serve as an example of
            // how dependency calls will be captured and treated
            // automatically as child of incoming request.
            Baggage.Current.SetBaggage("clientId", "123");
            Baggage.Current.SetBaggage("operationId", "abc");

            using (var activity = ActivitySource.StartActivity("GetBlobStatus"))
            {
                Thread.Sleep(100);
                var sum = 2 + 3;
            }

            var currentSpan = OpenTelemetry.Trace.Tracer.CurrentSpan;

            //var gatewayevent = new GatewayApiEvent {
            //    Customer = "XBox",
            //    ServiceName = "MS.Ess.Gateway.Api"
            //};

            //List<object> values = new List<object>();
            //var type = gatewayevent.GetType();
            //foreach (var prop in type.GetProperties())
            //{
            //    values.Add(prop.GetValue(gatewayevent));
            //}
            genevaLogger.logEvent(typeof(GatewayApiEvent), Baggage.Current.GetBaggage("clientId"), 
                                                            Baggage.Current.GetBaggage("operationId"),
                                                            "OneCert", "MS.Ess.Gateway.Api");

            genevaLogger.logEvent(typeof(GatewayGetStatusEvent), Baggage.Current.GetBaggage("clientId"),
                                                            Baggage.Current.GetBaggage("operationId"),
                                                            "In Progress", "Dublin");

            var res = HttpClient.GetStringAsync("https://localhost:5003/Matrix").Result;
            var rng = new Random();
            var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
            })
            .ToArray();
            
            foreach (var item in Baggage.Current)
            {
                Console.WriteLine(item.Key);
                Console.WriteLine(item.Value);
            }
            //Activity.Current?.AddBaggage("clientID", "123");
            //Activity.Current?.AddBaggage("operationID", "abc");
            Debug.WriteLine("hitting weatherforecast api endpoint");
            //this.logger.LogInformation("hello world bloop bloop");
            this.logger.LogInformation(
                "WeatherForecasts generated {count}: {forecasts}",
                forecast.Length,
                forecast);
            //this.logger.LogInformation("{event}", gatewayevent);

            return forecast;
        }
    }
}
