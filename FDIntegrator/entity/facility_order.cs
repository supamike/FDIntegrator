using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDIntegrator.entity
{
    class facility_order
    {
        public long intf_facility_order_id { get; set; }
        public String facility_code { get; set; }
        public DateTime cdc_date { get; set; }
        public DateTime order_date { get; set; }
        public String product_code { get; set; }
        public DateTime edt_date { get; set; }
        public String unit_code { get; set; }
        public String batch_number { get; set; }
        public float quantity { get; set; }
        public String supplier_code { get; set; }
        public String order_number { get; set; }
        public DateTime add_date { get; set; }
        public int sync_status { get; set; }
    }
}