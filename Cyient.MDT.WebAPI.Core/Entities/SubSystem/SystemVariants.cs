using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyient.MDT.WebAPI.Core.Entities.SubSystem
{
    public class SystemVariants
    {
        public int SYSTEM_ID { get; set; }
        public int SYSTEM_VARIANT_ID { get; set; }
        public string SYSTEM_IMAGE { get; set; }
        public string SYSTEM_NAME { get; set; }
        public double EQUIPMENT_COST { get; set; }
        public double ELECTRICAL_COST { get; set; }
        public double MECHANICAL_COST { get; set; }
        public string COMMENTS { get; set; }
        public string REMARKS { get; set; }
    }
}
