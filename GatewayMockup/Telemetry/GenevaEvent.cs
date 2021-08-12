using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry
{

    public abstract class GenevaEvent
    {
        //public abstract string name { get; set; }

        /**
         * Returns all columns used by this event
         */
        public virtual List<string> GetAllEventColumns()
        {
            List<string> columns = new List<string>();
            Type myType = this.GetType();
            var members = myType.GetProperties();

            foreach (var member in members)
            {
                columns.Add(member.Name);
            }

            return columns;
        }

    }
}
