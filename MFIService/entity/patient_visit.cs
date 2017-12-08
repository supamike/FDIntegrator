using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFIService.entity
{
    class patient_visit
    {
        public long intf_patient_visit_id { get; set; }
        public String facility_code { get; set; }
        public DateTime cdc_date { get; set; }
        public String patient_code { get; set; }
        public String sex { get; set; }
        public int age { get; set; }
        public DateTime visit_date { get; set; }
        public String product_code { get; set; }
        public String batch_number { get; set; }
        public String unit_code { get; set; }
        public float product_quantity { get; set; }
        public DateTime art_start_date { get; set; }
        public String regimen_code { get; set; }
        public String patient_category { get; set; }
        public DateTime add_date { get; set; }
        public int sync_status { get; set; }

        }
}
