using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PodPicker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PodPickerController : ControllerBase
    {

        private readonly ILogger<PodPickerController> logger;

        private static readonly HttpClient HttpClient = new HttpClient();

        public PodPickerController(ILogger<PodPickerController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public string Get()
        {
            using var scope = this.logger.BeginScope("{Id}", Guid.NewGuid().ToString("N"));
            //var res = HttpClient.GetStringAsync("http://localhost:19999/api/request").Result;
            //var res = HttpClient.GetStringAsync("http://localhost:5006").Result;
            var rng = new Random();
            var list = new List<string> { "Lily", "Aaron", "Charles" };
            int index = rng.Next(list.Count);
            Debug.WriteLine("hitting podpicker api endpoint");
            this.logger.LogInformation(
               "Pod Picker API chose... {pod}",
               list[index]);
            return list[index];
        }
    }
}
