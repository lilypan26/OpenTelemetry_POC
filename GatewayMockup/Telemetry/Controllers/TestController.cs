using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Telemetry
{
    public class TestController
    {
        static HttpClient client = new HttpClient();
        public void HttpTestCall()
        {
            var stringTask = client.GetStringAsync("https://google.com");
        }
    }
}
