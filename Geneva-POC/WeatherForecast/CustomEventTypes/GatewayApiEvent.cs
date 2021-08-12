using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Examples.AspNetCore
{
    public class GatewayApiEvent : GenevaEvent
    {
        //public override string name { set => name = "GatewayApiEvent"; get { return name; }  }
        public string ClientId { get; set; }
        public string RequestId { get; set; }
        public string Customer { get; set; }
        public string ServiceName { get; set; }

        //public string GetName() { return name; }
    }
}
