using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FDIntegrator.config;
using FDIntegrator.entity;

namespace FDIntegrator.sync
{
    class SyncFacilityOrder
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords = new Sync().NewRecordsCount("intf_facility_order");
            double RecordsBatchFactor = TotalRecords / DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            facility_order FacilityOrder = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_facility_order WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        FacilityOrder = new facility_order();
                        this.SetFacilityOrder(FacilityOrder, dr);
                        if (this.InsertFacilityOrder(FacilityOrder) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_facility_order", "intf_facility_order_id", 1, FacilityOrder.intf_facility_order_id);
                        }
                        FacilityOrder = null;
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

        public void SetFacilityOrder(facility_order FacilityOrder, SqlDataReader dr)
        {
            try
            {
                FacilityOrder.intf_facility_order_id = Convert.ToInt64(dr["intf_facility_order_id"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityOrder.intf_facility_order_id = 0;
            }
            try
            {
                FacilityOrder.facility_code = Convert.ToString(dr["facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityOrder.facility_code = "";
            }
            try
            {
                FacilityOrder.product_code = Convert.ToString(dr["product_code"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityOrder.product_code = "";
            }
            try
            {
                FacilityOrder.unit_code = Convert.ToString(dr["unit_code"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityOrder.unit_code = "";
            }
            try
            {
                FacilityOrder.order_number = Convert.ToString(dr["order_number"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityOrder.order_number = "";
            }
            try
            {
                FacilityOrder.order_date = Convert.ToDateTime(dr["order_date"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityOrder.order_date = Convert.ToDateTime(null);
            }
            try
            {
                FacilityOrder.batch_number = Convert.ToString(dr["batch_number"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityOrder.batch_number = "";
            }
            try
            {
                FacilityOrder.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityOrder.cdc_date = Convert.ToDateTime(null);
            }
            try
            {
                FacilityOrder.edt_date = Convert.ToDateTime(dr["edt_date"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityOrder.edt_date = Convert.ToDateTime(null);
            }
            try
            {
                FacilityOrder.quantity = Convert.ToSingle(dr["quantity"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityOrder.quantity = 0;
            }
           
        }

        public int InsertFacilityOrder(facility_order FacilityOrder)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO intf_facility_order" +
                                "(" +
                                "intf_facility_order_id," +
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
                                FacilityOrder.intf_facility_order_id + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", FacilityOrder.cdc_date) + "','" +
                                 "'" + string.Format("{0:yyyy-MM-dd HH:mm}", FacilityOrder.order_date) + "','" +
                                 "'" + string.Format("{0:yyyy-MM-dd HH:mm}", FacilityOrder.edt_date) + "','" +
                                FacilityOrder.product_code + "','" +
                                FacilityOrder.facility_code + "','" +
                                FacilityOrder.unit_code + "','" +
                                FacilityOrder.batch_number + "'," +
                                FacilityOrder.quantity + "," +
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