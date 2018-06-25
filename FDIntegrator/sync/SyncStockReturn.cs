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
    class SyncStockReturn
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords= new Sync().NewRecordsCount("intf_stock_return");
            double RecordsBatchFactor = TotalRecords/DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            stock_return StockReturn = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_stock_return WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        StockReturn = new stock_return();
                        this.SetStockReturn(StockReturn, dr);
                        if (this.InsertStockReturn(StockReturn) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_store_stock", "intf_stock_return_id", 1, StockReturn.intf_stock_return_id);
                        }
                        StockReturn = null;
                        i = i + 1;
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }

                loop = loop + 1;
            }
            return SyncPass + "/" + TotalRecords + " Synced" + " Loops:" + Loops;
        }

        public void SetStockReturn (stock_return StockReturn,SqlDataReader dr)
        {
            try
            {
                StockReturn.intf_stock_return_id = Convert.ToInt64(dr["intf_stock_return_id"]);
            }
            catch (InvalidCastException ice)
            {
                StockReturn.intf_stock_return_id = 0;
            }
            try
            {
                StockReturn.facility_code = Convert.ToString(dr["intf_facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockReturn.facility_code = "";
            }
            try
            {
                StockReturn.product_code = Convert.ToString(dr["intf_product_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockReturn.product_code = "";
            }
            try
            {
                StockReturn.unit_code = Convert.ToString(dr["intf_unit_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockReturn.unit_code = "";
            }
            try
            {
                StockReturn.return_number = Convert.ToString(dr["return_number"]);
            }
            catch (InvalidCastException ice)
            {
                StockReturn.return_number = "";
            }
            try
            {
                StockReturn.batch_number = Convert.ToString(dr["batch_number"]);
            }
            catch (InvalidCastException ice)
            {
                StockReturn.batch_number = "";
            }
            try
            {
                StockReturn.return_number = Convert.ToString(dr["return_number"]);
            }
            catch (InvalidCastException ice)
            {
                StockReturn.return_number = "";
            }
            try
            {
                StockReturn.return_date = Convert.ToDateTime(dr["return_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockReturn.return_date = Convert.ToDateTime(null);
            }
            try
            {
                StockReturn.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockReturn.cdc_date = Convert.ToDateTime(null);
            }
            try
            {
                StockReturn.quantity = Convert.ToSingle(dr["quantity"]);
            }
            catch (InvalidCastException ice)
            {
                StockReturn.quantity = 0;
            }
            try
            {
                StockReturn.manufacture_date = Convert.ToDateTime(dr["manufacture_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockReturn.manufacture_date = Convert.ToDateTime(null);
            }
            try
            {
                StockReturn.expiry_date = Convert.ToDateTime(dr["expiry_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockReturn.expiry_date = Convert.ToDateTime(null);
            }
        }

        public int InsertStockReturn(stock_return StockReturn)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO intf_stock_return" +
                                "(" +
                                "intf_stock_return_id," +
                                "cdc_date," +
                                "return_date," +
                                "intf_product_code," +
                                "intf_facility_code," +
                                "intf_unit_code," +
                                "batch_number," +
                                "quantity," +
                                "return_number," +
                                "add_date," +
                                "load_status," +
                                "manufacture_date," +
                                "expiry_date" +
                                ") " +
                                " VALUES" +
                                "(" +
                                StockReturn.intf_stock_return_id + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockReturn.cdc_date) + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockReturn.return_date) + "','" +
                                StockReturn.product_code + "','" +
                                StockReturn.facility_code + "','" +
                                StockReturn.unit_code + "','" +
                                StockReturn.batch_number + "'," +
                                StockReturn.quantity + "," +
                                StockReturn.return_number + "'," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.Now) + "'," +
                                0 + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockReturn.manufacture_date) + "'," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockReturn.expiry_date) + "'" +
                                ") ";
                SqlConnection conn = new SqlConnection(DatabaseConnection.getRemoteConnectionString());
                SqlCommand cmd = new SqlCommand(sql_to, conn);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                status = 1;
                cmd.Connection.Close();
            }catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
                status = 0;
            }
            return status;
        }

    }
}
