using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDIntegrator.entity
{
    class conn_status
    {
        public long intf_conn_status_id { get; set; }
        public String facility_code { get; set; }
        public DateTime cdc_date { get; set; }
        public DateTime status_date { get; set; }
        public int status_connect { get; set; }
        public int status_dispense_mode { get; set; }
        public int status_dispense_mode_data { get; set; }
        public DateTime add_date { get; set; }
        public int sync_status { get; set; }
    }
}