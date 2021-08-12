using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Examples.AspNetCore
{
    public class GatewayGetStatusEvent : GenevaEvent
    {
        public string ClientId { get; set; }
        public string RequestId { get; set; }
        public string CompletedState { get; set; }
        public string Region { get; set; }

    }
}
