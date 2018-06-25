using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDIntegrator.entity
{
    class facility_config
    {
        public long intf_facility_config_id { get; set; }
        public String facility_code { get; set; }
        public String system_code { get; set; }
        public String database_type { get; set; }
        }
}
