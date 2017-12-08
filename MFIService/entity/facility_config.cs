using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFIService.entity
{
    class facility_config
    {
        public long intf_facility_config_id { get; set; }
        public String facility_code { get; set; }
        public String system_code { get; set; }
        public int is_active { get; set; }
        public int is_deleted { get; set; }
        public DateTime add_date { get; set; }
        public int add_by { get; set; }
        public DateTime last_edit_date { get; set; }
        public int last_edit_by { get; set; }
               
        }
}
