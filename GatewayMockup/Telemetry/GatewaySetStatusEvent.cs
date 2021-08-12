using System;
using System.Collections.Generic;
using System.Text;

namespace Telemetry
{
    public class GatewaySetStatusEvent : GenevaEvent
    {
        public string clientId { get; set; }
        public string timeStamp { get; set; }
        public string status { get; set; }
        public string requestId { get; set; }
    }
}
