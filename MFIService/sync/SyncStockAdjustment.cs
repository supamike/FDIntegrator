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
    class SyncStockAdjustment
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords= new Sync().NewRecordsCount("intf_stock_adjustment");
            double RecordsBatchFactor = TotalRecords/DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            stock_adjustment StockAdjustment = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_stock_adjustmsnt WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        StockAdjustment = new stock_adjustment();
                        this.SetStockAdjustment(StockAdjustment, dr);
                        if (this.InsertStockAdjustment(StockAdjustment) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_stock_adjustment", "intf_stock_adjustment_id", 1, StockAdjustment.intf_stock_adjustment_id);
                        }
                        StockAdjustment = null;
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

        public void SetStockAdjustment (stock_adjustment StockAdjustment,SqlDataReader dr)
        {
            try
            {
                StockAdjustment.intf_stock_adjustment_id = Convert.ToInt64(dr["intf_stock_adjustment_id"]);
            }
            catch (InvalidCastException ice)
            {
                StockAdjustment.intf_stock_adjustment_id = 0;
            }
            try
            {
                StockAdjustment.facility_code = Convert.ToString(dr["facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockAdjustment.facility_code = "";
            }
            try
            {
                StockAdjustment.adjustment_type = Convert.ToString(dr["adjustment_type"]);
            }
            catch (InvalidCastException ice)
            {
                StockAdjustment.adjustment_type = "";
            }
            try
            {
                StockAdjustment.adjustment_reason = Convert.ToString(dr["adjustment_reason"]);
            }
            catch (InvalidCastException ice)
            {
                StockAdjustment.adjustment_reason = "";
            }
            try
            {
                StockAdjustment.adjustment_date = Convert.ToDateTime(dr["adjustment_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockAdjustment.adjustment_date = Convert.ToDateTime(null);
            }
            try
            {
                StockAdjustment.product_code = Convert.ToString(dr["product_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockAdjustment.product_code = "";
            }
            try
            {
                StockAdjustment.unit_code = Convert.ToString(dr["unit_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockAdjustment.unit_code = "";
            }
            try
            {
                StockAdjustment.batch_number = Convert.ToString(dr["batch_number"]);
            }
            catch (InvalidCastException ice)
            {
                StockAdjustment.batch_number = "";
            }
            try
            {
                StockAdjustment.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockAdjustment.cdc_date = Convert.ToDateTime(null);
            }
            try
            {
                StockAdjustment.quantity = Convert.ToSingle(dr["quantity"]);
            }
            catch (InvalidCastException ice)
            {
                StockAdjustment.quantity = 0;
            }
            try
            {
                StockAdjustment.manufacture_date = Convert.ToDateTime(dr["manufacture_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockAdjustment.manufacture_date = Convert.ToDateTime(null);
            }
            try
            {
                StockAdjustment.expiry_date = Convert.ToDateTime(dr["expiry_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockAdjustment.expiry_date = Convert.ToDateTime(null);
            }
        }

        public int InsertStockAdjustment(stock_adjustment StockAdjustment)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO stage_stock_adjustment" +
                                "(" +
                                "cdc_date," +
                                "intf_adjustment_date," +
                                "intf_product_code," +
                                "intf_facility_code," +
                                "intf_unit_code," +
                                "intf_adjustment_type," +
                                "intf_adjustment_reason," +
                                "batch_number," +
                                "quantity," +
                                "validation_status," +
                                "add_date," +
                                "load_status," +
                                "manufacture_date," +
                                "expiry_date" +
                                ") " +
                                " VALUES" +
                                "(" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockAdjustment.cdc_date) + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockAdjustment.adjustment_date) + "','" +
                                StockAdjustment.product_code + "','" +
                                StockAdjustment.facility_code + "','" +
                                StockAdjustment.unit_code + "','" +
                                StockAdjustment.adjustment_type + "'," +
                                StockAdjustment.adjustment_reason + "'," +
                                StockAdjustment.batch_number + "'," +
                                StockAdjustment.quantity + "," +
                                0 + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.Now) + "'," +
                                0 + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockAdjustment.manufacture_date) + "'," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockAdjustment.expiry_date) + "'" +
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
