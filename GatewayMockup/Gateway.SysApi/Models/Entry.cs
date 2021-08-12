using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Api.Models
{
    public class Entry
    {

        public string clientId { get; set; }
        public string certificateName { get; set; }
        public string timeStamp { get; set; }
        public string status { get; set; }
        public string requestId { get; set; }
    }
}
