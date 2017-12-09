using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MFIService.config;
using MFIService.entity;

namespace MFIService.sync
{
    class SyncHealthFacilityOrder
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords = new Sync().NewRecordsCount("intf_health_facility_order");
            double RecordsBatchFactor = TotalRecords / DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            health_facility_order HealthFacilityOrder = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_health_facility_order WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        HealthFacilityOrder = new health_facility_order();
                        this.SetHealthFacilityOrder(HealthFacilityOrder, dr);
                        if (this.InsertHealthFacilityOrder(HealthFacilityOrder) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_health_facility_order", "intf_health_facility_order_id", 1, HealthFacilityOrder.intf_health_facility_order_id);
                        }
                        HealthFacilityOrder = null;
                        i = i + 1;
                    }
                    dr.Close();
                }
                catch (SqlException me)
                {
                    //
                }

                loop = loop + 1;
            }
            return SyncPass + "/" + TotalRecords + " Synced" + " Loops:" + Loops;
        }

        public void SetHealthFacilityOrder(health_facility_order HealthFacilityOrder, SqlDataReader dr)
        {
            try
            {
                HealthFacilityOrder.intf_health_facility_order_id = Convert.ToInt64(dr["intf_health_facility_order_id"]);
            }
            catch (InvalidCastException ice)
            {
                HealthFacilityOrder.intf_health_facility_order_id = 0;
            }
            try
            {
                HealthFacilityOrder.facility_code = Convert.ToString(dr["facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                HealthFacilityOrder.facility_code = "";
            }
            try
            {
                HealthFacilityOrder.product_code = Convert.ToString(dr["product_code"]);
            }
            catch (InvalidCastException ice)
            {
                HealthFacilityOrder.product_code = "";
            }
            try
            {
                HealthFacilityOrder.unit_code = Convert.ToString(dr["unit_code"]);
            }
            catch (InvalidCastException ice)
            {
                HealthFacilityOrder.unit_code = "";
            }
            try
            {
                HealthFacilityOrder.order_number = Convert.ToString(dr["order_number"]);
            }
            catch (InvalidCastException ice)
            {
                HealthFacilityOrder.order_number = "";
            }
            try
            {
                HealthFacilityOrder.order_date = Convert.ToDateTime(dr["order_date"]);
            }
            catch (InvalidCastException ice)
            {
                HealthFacilityOrder.order_date = Convert.ToDateTime(null);
            }
            try
            {
                HealthFacilityOrder.batch_number = Convert.ToString(dr["batch_number"]);
            }
            catch (InvalidCastException ice)
            {
                HealthFacilityOrder.batch_number = "";
            }
            try
            {
                HealthFacilityOrder.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                HealthFacilityOrder.cdc_date = Convert.ToDateTime(null);
            }
            try
            {
                HealthFacilityOrder.edt_date = Convert.ToDateTime(dr["edt_date"]);
            }
            catch (InvalidCastException ice)
            {
                HealthFacilityOrder.edt_date = Convert.ToDateTime(null);
            }
            try
            {
                HealthFacilityOrder.quantity = Convert.ToSingle(dr["quantity"]);
            }
            catch (InvalidCastException ice)
            {
                HealthFacilityOrder.quantity = 0;
            }
           
        }

        public int InsertHealthFacilityOrder(health_facility_order HealthFacilityOrder)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO stage_health_facility_order" +
                                "(" +
                                "cdc_date," +
                                "intf_product_code," +
                                "intf_facility_code," +
                                "intf_unit_code," +
                                "intf_supplier_code," +
                                "batch_number," +
                                "quantity," +
                                "add_date," +
                                "load_status," +
                                "manufacture_date," +
                                "expiry_date" +
                                ") " +
                                " VALUES" +
                                "(" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", HealthFacilityOrder.cdc_date) + "','" +
                                 "'" + string.Format("{0:yyyy-MM-dd HH:mm}", HealthFacilityOrder.order_date) + "','" +
                                 "'" + string.Format("{0:yyyy-MM-dd HH:mm}", HealthFacilityOrder.edt_date) + "','" +
                                HealthFacilityOrder.product_code + "','" +
                                HealthFacilityOrder.facility_code + "','" +
                                HealthFacilityOrder.unit_code + "','" +
                                HealthFacilityOrder.batch_number + "'," +
                                HealthFacilityOrder.quantity + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.Now) + "'," +
                                0 + "," +
                               
                                ") ";
                SqlConnection conn = new SqlConnection(DatabaseConnection.getRemoteConnectionString());
                SqlCommand cmd = new SqlCommand(sql_to, conn);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                status = 1;
                cmd.Connection.Close();
            }
            catch (Exception e)
            {
                status = 0;
            }
            return status;
        }

    }
}