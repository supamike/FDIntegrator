using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFIService.entity
{
    class stock_requisition
    {
        public long intf_stock_requisition_id { get; set; }
        public String facility_code { get; set; }
        public DateTime cdc_date { get; set; }
        public String requisition_number { get; set; }
        public DateTime requisition_date { get; set; }
        public String product_code { get; set; }
        public String unit_code { get; set; }
        public float quantity { get; set; }
        public int sync_status { get; set; }
        public DateTime add_date { get; set; }

    }
}
