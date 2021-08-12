using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telemetry
{
    public class GatewayUpdateStatusEvent : GenevaEvent
    {

        public string clientId { get; set; }
        public string timeStamp { get; set; }
        public string status { get; set; }
        public string requestId { get; set; }
    }
}
