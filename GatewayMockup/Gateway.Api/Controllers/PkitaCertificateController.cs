using Gateway.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Web.Http;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Telemetry;

namespace Gateway.Api.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    public class PkitaCertificateController : ControllerBase
    {
    
        private static readonly HttpClient HttpClient = new HttpClient();

        private static readonly ActivitySource activitysource = new ActivitySource("PkitaController");

        private readonly ILogger<PkitaCertificateController> logger;

        GenevaLogger genevaLogger = new GenevaLogger();

        TestController testController = new TestController();

        public PkitaCertificateController(ILogger<PkitaCertificateController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        async public IAsyncEnumerable<PkitaCertificate> Post([Microsoft.AspNetCore.Mvc.FromBody] PkitaCertificate userData)
        {

            Guid requestId = Guid.NewGuid();
            var requestTime = DateTime.Now;
            Console.WriteLine(requestId.ToString());


            //Setting Baggage
            Baggage.Current.SetBaggage("requestID: ", requestId.ToString());
            Baggage.Current.SetBaggage("clientID: ", userData.ClientId.ToString());


            Entry requestStatusEntry = new Entry()
            {
                clientId = userData.ClientId.ToString(),
                certificateName = userData.CertificateName.ToString(),
                timeStamp = requestTime.ToString(),
                status = "InProgress",
                requestId = requestId.ToString()

            };

            //Span for storing Initial Request Status
            using (var activity = activitysource.StartActivity("GatewaySetStatusEvent"))

            {
                GatewaySetStatusEvent setStatusEvent = new GatewaySetStatusEvent()
                {
                    clientId = requestStatusEntry.clientId,
                    timeStamp = requestStatusEntry.timeStamp,
                    status = requestStatusEntry.status,
                    requestId = requestStatusEntry.requestId
                };
                //genevaLogger.logEvent(typeof(GatewaySetStatusEvent), requestStatusEntry.clientId, requestStatusEntry.timeStamp, 
                //                                                     requestStatusEntry.status, requestStatusEntry.requestId);
                genevaLogger.logEvent(setStatusEvent);

                testController.HttpTestCall();
                //genevaLogger.LogEvent(setStatusEvent);

                var requestStatusFileName = userData.ClientId + "." + requestId + ".json";
                var requestStatusFolderName = @"..\StatusUpdateData";
                var requestStatusFilePath = System.IO.Path.Combine(requestStatusFolderName, requestStatusFileName);
 

                if (System.IO.File.Exists(requestStatusFilePath))
                {
                    System.IO.File.Delete(requestStatusFilePath);
                }

                FileStream fs = new FileStream(requestStatusFilePath, FileMode.OpenOrCreate);
                StreamWriter str = new StreamWriter(fs);
                str.BaseStream.Seek(0, SeekOrigin.End);
                //string json = JsonSerializer.Serialize(requestStatusEntry);
                string json2 = JsonConvert.SerializeObject(requestStatusEntry, Formatting.Indented);

                str.Write(json2);
                str.Flush();
    
                str.Close();
                fs.Close();


                using (var activity2 = activitysource.StartActivity("SendDataTOGatewayWorker"))
                {
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromMinutes(10);

                        client.BaseAddress = new Uri("http://localhost:5002/ApiBase");

                        string json0 = JsonConvert.SerializeObject(requestStatusEntry, Formatting.Indented);
                        var content = new StringContent(json0, Encoding.UTF8, "application/json");


                        var result = await client.PostAsync(client.BaseAddress, content);
                        string resultContent = await result.Content.ReadAsStringAsync();
                    }

                }
                //Verification
                Console.WriteLine("File Created: {0}\n", requestStatusFilePath);
            }

            yield return userData;
        }

        [Microsoft.AspNetCore.Mvc.Route("Pkita")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public string Get([FromUri] string clientID, string requestID)
        {

            Console.WriteLine(clientID);
            Console.WriteLine(requestID);


            using (var activity = activitysource.StartActivity("GatewayGetStatusEvent"))
            {

                var requestTime = DateTime.Now.ToString();
                var fileToCheck = clientID + "." + requestID + ".json";
                var requestStatusFolderName = @"..\StatusUpdateData";
                var requestStatusFilePath = System.IO.Path.Combine(requestStatusFolderName, fileToCheck);
                Console.WriteLine(requestStatusFilePath);
                Console.WriteLine(requestStatusFolderName);
        

                var json = System.IO.File.ReadAllText(requestStatusFilePath);

                var entry = JsonConvert.DeserializeObject<Entry>(json);

                string status = entry.status;
                genevaLogger.logEvent(typeof(GatewayGetStatusEvent), entry.clientId, requestTime, status, entry.requestId);

                return status;


            }

        }

    }
}
