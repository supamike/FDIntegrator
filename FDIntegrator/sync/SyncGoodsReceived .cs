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
    class SyncGoodsReceived
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords = new Sync().NewRecordsCount("intf_goods_received");
            double RecordsBatchFactor = TotalRecords / DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            goods_received GoodsReceived = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_goods_received WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    //cmd.CommandType=CommandType.Text;
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        GoodsReceived = new goods_received();
                        this.SetGoodsReceived(GoodsReceived, dr);
                        if (this.InsertGoodsReceived(GoodsReceived) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_goods_received", "intf_goods_received_id", 1, GoodsReceived.intf_goods_received_id);
                        }
                        GoodsReceived = null;
                        i = i + 1;
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Sync_goods_received:" + e.StackTrace);
                }

                loop = loop + 1;
            }
            return SyncPass + "/" + TotalRecords + " Synced" + " Loops:" + Loops;
        }

        public void SetGoodsReceived(goods_received GoodsReceived, SqlDataReader dr)
        {
            try
            {
                GoodsReceived.intf_goods_received_id = Convert.ToInt64(dr["intf_goods_received_id"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.intf_goods_received_id = 0;
            }
            try
            {
                GoodsReceived.facility_code = Convert.ToString(dr["intf_facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.facility_code = "";
            }
            try
            {
                GoodsReceived.product_code = Convert.ToString(dr["intf_product_code"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.product_code = "";
            }
            try
            {
                GoodsReceived.supplier_code = Convert.ToString(dr["supplier_code"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.supplier_code = "";
            }
            try
            {
                GoodsReceived.whs_grn_ref = Convert.ToString(dr["whs_grn_ref"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.whs_grn_ref = "";
            }
            try
            {
                GoodsReceived.order_number = Convert.ToString(dr["order_number"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.order_number = "";
            }
            try
            {
                GoodsReceived.unit_code = Convert.ToString(dr["intf_unit_code"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.unit_code = "";
            }
            try
            {
                GoodsReceived.batch_number = Convert.ToString(dr["batch_number"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.batch_number = "";
            }
            try
            {
                GoodsReceived.receipt_number = Convert.ToString(dr["receipt_number"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.receipt_number = "";
            }
            try
            {
                GoodsReceived.receipt_date = Convert.ToDateTime(dr["receipt_date"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.receipt_date = Convert.ToDateTime(null);
            }
            try
            {
                GoodsReceived.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.cdc_date = Convert.ToDateTime(null);
            }
            try
            {
                GoodsReceived.quantity = Convert.ToSingle(dr["quantity"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.quantity = 0;
            }
            try
            {
                GoodsReceived.manufacture_date = Convert.ToDateTime(dr["manufacture_date"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.manufacture_date = Convert.ToDateTime(null);
            }
            try
            {
                GoodsReceived.expiry_date = Convert.ToDateTime(dr["expiry_date"]);
            }
            catch (InvalidCastException ice)
            {
                GoodsReceived.expiry_date = Convert.ToDateTime(null);
            }
        }

        public int InsertGoodsReceived(goods_received GoodsReceived)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO intf_goods_received" +
                                "(" +
                                "intf_goods_received_id," +
                                "cdc_date," +
                                "receipt_date," +
                                "receipt_number," +
                                "intf_product_code," +
                                "supplier_code," +
                                "intf_facility_code," +
                                "intf_unit_code," +
                                "batch_number," +
                                "order_number," +
                                "whs_grn_ref," +
                                "quantity," +
                                "add_date," +
                                "load_status," +
                                "manufacture_date," +
                                "expiry_date" +
                                ") " +
                                " VALUES" +
                                "(" +
                                GoodsReceived.intf_goods_received_id + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", GoodsReceived.cdc_date) + "'," +
                                 "'" + string.Format("{0:yyyy-MM-dd HH:mm}", GoodsReceived.receipt_date) + "','" +
                                 GoodsReceived.receipt_number + "','" +
                                GoodsReceived.product_code + "','" +
                                GoodsReceived.supplier_code + "','" +
                                GoodsReceived.facility_code + "','" +
                                GoodsReceived.unit_code + "','" +
                                GoodsReceived.batch_number + "','" +
                                GoodsReceived.order_number + "','" +
                                GoodsReceived.whs_grn_ref + "'," +
                                GoodsReceived.quantity + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.Now) + "'," +
                                0 + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", GoodsReceived.manufacture_date) + "'," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", GoodsReceived.expiry_date) + "'" +
                                ") ";
                //Console.WriteLine(sql_to);
                SqlConnection conn = new SqlConnection(DatabaseConnection.getRemoteConnectionString());
                SqlCommand cmd = new SqlCommand(sql_to, conn);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                status = 1;
                cmd.Connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                status = 0;
            }
            return status;
        }

    }
}