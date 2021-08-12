using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry;

namespace Matrix.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MatrixController : ControllerBase
    {

        private readonly ILogger<MatrixController> logger;

        private static readonly HttpClient HttpClient = new HttpClient();

        public MatrixController(ILogger<MatrixController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public string Get()
        {
            using var scope = this.logger.BeginScope("{Id}", Guid.NewGuid().ToString("N"));
            var res = HttpClient.GetStringAsync("https://localhost:5005/PodPicker").Result;
            //var res = HttpClient.GetStringAsync("http://localhost:5006").Result;
            var rng = new Random();
            var list = new List<string> { "red pill", "blue pill" };
            int index = rng.Next(list.Count);
            foreach (var item in Baggage.Current)
            {
                Console.WriteLine("Baggage key: " + item.Key + ", value: " + item.Value);
                //Console.WriteLine(item.Value);
            }
            Debug.WriteLine("hitting matrix api endpoint");
            this.logger.LogInformation(
               "Matrix API: You chose the... {pill}",
               list[index]);
            return list[index];
        }
    }
}
