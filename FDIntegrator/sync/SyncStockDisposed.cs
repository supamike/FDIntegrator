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
    class SyncStockDisposed
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords= new Sync().NewRecordsCount("intf_stock_disposed");
            double RecordsBatchFactor = TotalRecords/DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            stock_disposed StockDisposed = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_stock_disposed WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        StockDisposed = new stock_disposed();
                        this.SetStockDisposed(StockDisposed, dr);
                        if (this.InsertStockDisposed(StockDisposed) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_stock_disposed", "intf_stock_disposed_id", 1, StockDisposed.intf_stock_disposed_id);
                        }
                        StockDisposed = null;
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

        public void SetStockDisposed (stock_disposed StockDisposed,SqlDataReader dr)
        {
            try
            {
                StockDisposed.intf_stock_disposed_id = Convert.ToInt64(dr["intf_stock_disposed_id"]);
            }
            catch (InvalidCastException ice)
            {
                StockDisposed.intf_stock_disposed_id = 0;
            }
            try
            {
                StockDisposed.facility_code = Convert.ToString(dr["facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockDisposed.facility_code = "";
            }
            try
            {
                StockDisposed.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockDisposed.cdc_date = Convert.ToDateTime(null);
            }
            try
            {
                StockDisposed.cdc_date = Convert.ToDateTime(dr["dispose_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockDisposed.dispose_date = Convert.ToDateTime(null);
            }
            try
            {
                StockDisposed.dispose_reason = Convert.ToString(dr["dispose_reason"]);
            }
            catch (InvalidCastException ice)
            {
                StockDisposed.dispose_reason = "";
            }
            try
            {
                StockDisposed.product_code = Convert.ToString(dr["product_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockDisposed.product_code = "";
            }
            try
            {
                StockDisposed.unit_code = Convert.ToString(dr["unit_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockDisposed.unit_code = "";
            }
            try
            {
                StockDisposed.quantity = Convert.ToSingle(dr["quantity"]);
            }
            catch (InvalidCastException ice)
            {
                StockDisposed.quantity = 0;
            }
            try
            {
                StockDisposed.batch_number = Convert.ToString(dr["batch_number"]);
            }
            catch (InvalidCastException ice)
            {
                StockDisposed.batch_number = "";
            }
            try
            {
                StockDisposed.manufacture_date = Convert.ToDateTime(dr["manufacture_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockDisposed.manufacture_date = Convert.ToDateTime(null);
            }
            try
            {
                StockDisposed.expiry_date = Convert.ToDateTime(dr["expiry_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockDisposed.expiry_date = Convert.ToDateTime(null);
            }
        }

        public int InsertStockDisposed(stock_disposed StockDisposed)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO intf_stock_disposed" +
                                "(" +
                                "intf_stock_disposed_id," +
                                "cdc_date," +
                                "dispose_date," +
                                "intf_product_code," +
                                "intf_facility_code," +
                                "intf_unit_code," +
                                "dispose_reason," +
                                "quantity," +
                                "batch_number," +
                                "manufacture_date," +
                                "expiry_date" +
                                "add_date," +
                                "load_status," +
                                ") " +
                                " VALUES" +
                                "(" +
                                StockDisposed.intf_stock_disposed_id + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockDisposed.cdc_date) + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockDisposed.dispose_date) + "','" +
                                StockDisposed.product_code + "','" +
                                StockDisposed.facility_code + "','" +
                                StockDisposed.unit_code + "','" +
                                StockDisposed.dispose_reason + "','" +
                                StockDisposed.quantity + "," +
                                StockDisposed.batch_number + "'," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockDisposed.manufacture_date) + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockDisposed.expiry_date) + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.Now) + "'," +
                                0 + "," +
                                ") ";
                SqlConnection conn = new SqlConnection(DatabaseConnection.getRemoteConnectionString());
                SqlCommand cmd = new SqlCommand(sql_to, conn);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                status = 1;
                cmd.Connection.Close();
            }catch(Exception e)
            {
                status = 0;
            }
            return status;
        }

    }
}
