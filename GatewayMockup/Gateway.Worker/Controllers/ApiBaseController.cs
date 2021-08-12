using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenTelemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Gateway.Api.Models;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Telemetry;

namespace Gateway.Worker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiBaseController : ControllerBase
    {

        private readonly ILogger<ApiBaseController> _logger;

        private static readonly ActivitySource activitysource = new ActivitySource("ApiBaseController");

        GenevaLogger genevaLogger = new GenevaLogger();

        public ApiBaseController(ILogger<ApiBaseController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        async public IAsyncEnumerable<RequestStatus> Post([FromBody] Entry request) //TODO: Change Entry to RequestMetadata
        {
            Console.WriteLine("Baggage: ");
            Console.WriteLine("");

            foreach (var val in Baggage.Current)
            {
                Console.WriteLine(val.Key + ":" + val.Value);
            }

            using (var activity = activitysource.StartActivity("GatewayUpdateStatusEvent"))
            {
                Console.WriteLine("");
                Console.WriteLine("The current reuqest Status is: " + request.status);
                Thread.Sleep(20000);
                request.status = "Pass";
                Console.WriteLine("Request status has been set to: " + request.status);

                var timestamp = DateTime.Now.ToString();
                genevaLogger.logEvent(typeof(GatewayUpdateStatusEvent), request.clientId, timestamp, request.status,
                                                                        request.requestId);

                using (var activity2 = activitysource.StartActivity("SendStatusToSysApi"))
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:5004/UpdateStatus");

                        string json0 = JsonConvert.SerializeObject(request, Formatting.Indented);
                        var content = new StringContent(json0, Encoding.UTF8, "application/json");


                        var result = await client.PostAsync(client.BaseAddress, content);
                        string resultContent = await result.Content.ReadAsStringAsync();
                    }

                    
                }
            }

            yield return new RequestStatus();
        }
    }
}
