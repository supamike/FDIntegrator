using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDIntegrator.entity
{
    class stock_disposed
    {
        public long intf_stock_disposed_id { get; set; }
        public String facility_code { get; set; }
        public DateTime cdc_date { get; set; }
        public DateTime dispose_date { get; set; }
        public String dispose_reason { get; set; }
        public String product_code { get; set; }
        public String unit_code { get; set; }
        public float quantity { get; set; }
        public String batch_number { get; set; }
        public DateTime manufacture_date { get; set; }
        public DateTime expiry_date { get; set; }
        public DateTime add_date { get; set; }
        public int sync_status { get; set; }

        }
}
