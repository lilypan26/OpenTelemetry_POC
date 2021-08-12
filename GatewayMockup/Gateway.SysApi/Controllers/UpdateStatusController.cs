using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
using System.Net;
using System.Text;
//using Gateway.SysApi.Models;

namespace Gateway.SysApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdateStatusController : ControllerBase
    {
        private static readonly ActivitySource activitysource = new ActivitySource("SysApiController");

        private readonly ILogger<UpdateStatusController> _logger;

        public UpdateStatusController(ILogger<UpdateStatusController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IEnumerable<RequestMetadata> Post([FromBody] Entry request) //TODO: Change Entry to RequestMetadata
        {
                
            Console.WriteLine("Baggage: ");
            Console.WriteLine("");

            foreach (var val in Baggage.Current)
            {
                Console.WriteLine(val.Key + ":" + val.Value);
            }

            using (var activity = activitysource.StartActivity("UpdateStatusInFile"))
            {
                var requestID = request.requestId;
                var clientID = request.clientId;

                var fileToUpdate = clientID + "." + requestID + ".json";
                var requestStatusFolderName = @"..\StatusUpdateData";
                var requestStatusFilePath = System.IO.Path.Combine(requestStatusFolderName, fileToUpdate);


                if (System.IO.File.Exists(requestStatusFilePath))
                {
                    System.IO.File.Delete(requestStatusFilePath);
                }


                FileStream fs = new FileStream(requestStatusFilePath, FileMode.OpenOrCreate);
                StreamWriter str = new StreamWriter(fs);
                str.BaseStream.Seek(0, SeekOrigin.End);
                //string json = JsonSerializer.Serialize(requestStatusEntry);
                string json2 = JsonConvert.SerializeObject(request, Formatting.Indented);

                str.Write(json2);
                str.Flush();
                str.Close();
                fs.Close();


                yield return new RequestMetadata();
            }

        }

    }

}