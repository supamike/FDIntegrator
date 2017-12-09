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
    class SyncStockDispensed
    {
        public object StockDispensed { get; private set; }

        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords= new Sync().NewRecordsCount("intf_stock_dispensed");
            double RecordsBatchFactor = TotalRecords/DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            stock_dispensed StockDispensed = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_stock_dispensed WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        StockDispensed = new stock_dispensed();
                        this.SetStockDispensed(StockDispensed, dr);
                        if (this.InsertStockDispensed(StockDispensed) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_store_stock", "intf_store_stock_id", 1, StockDispensed.intf_stock_dispensed_id);
                        }
                        StockDispensed = null;
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

        public void SetStockDispensed(stock_dispensed StockDispensed, SqlDataReader dr)
        {
            try
            {
                StockDispensed.intf_stock_dispensed_id = Convert.ToInt64(dr["intf_stock_dispensed_id"]);
            }
            catch (InvalidCastException ice)
            {
                StockDispensed.intf_stock_dispensed_id = 0;
            }
            try
            {
                StockDispensed.facility_code = Convert.ToString(dr["facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockDispensed.facility_code = "";
            }
            try
            {
                StockDispensed.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockDispensed.cdc_date = Convert.ToDateTime(null);
            }
            try
            {
                StockDispensed.dispense_date = Convert.ToDateTime(dr["dispense_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockDispensed.dispense_date = Convert.ToDateTime(null);
            }
            try
            {
                StockDispensed.product_code = Convert.ToString(dr["product_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockDispensed.product_code = "";
            }
            try
            {
                StockDispensed.unit_code = Convert.ToString(dr["unit_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockDispensed.unit_code = "";
            }
            try
            {
                StockDispensed.batch_number = Convert.ToString(dr["batch_number"]);
            }
            catch (InvalidCastException ice)
            {
                StockDispensed.batch_number = "";
            }
            try
            {
                StockDispensed.quantity = Convert.ToSingle(dr["quantity"]);
            }
            catch (InvalidCastException ice)
            {
                StockDispensed.quantity = 0;
            }
            try
            {
                StockDispensed.patient_code = Convert.ToString(dr["patient_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockDispensed.patient_code = "";
            }
            try
            {
                StockDispensed.manufacture_date = Convert.ToDateTime(dr["manufacture_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockDispensed.manufacture_date = Convert.ToDateTime(null);
            }
            try
            {
                StockDispensed.expiry_date = Convert.ToDateTime(dr["expiry_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockDispensed.expiry_date = Convert.ToDateTime(null);
            }
          }

        public int InsertStockDispensed(stock_dispensed StockDispensed)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO stock_dispensed" +
                                "(" +
                                "cdc_date," +
                                "dispense_date," +
                                "intf_product_code," +
                                "intf_facility_code," +
                                "intf_unit_code," +
                                "batch_number," +
                                "quantity," +
                                "patient_code," +
                                "manufacture_date," +
                                "expiry_date" +
                                "add_date," +
                                "load_status," +
                                ") " +
                                " VALUES" +
                                "(" +
                                 "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockDispensed.cdc_date) + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockDispensed.dispense_date) + "','" +
                                StockDispensed.product_code + "','" +
                                StockDispensed.facility_code + "','" +
                                StockDispensed.unit_code + "','" +
                                StockDispensed.batch_number + "'," +
                                StockDispensed.quantity + "," +
                                StockDispensed.patient_code + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockDispensed.manufacture_date) + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockDispensed.expiry_date) + "','" +
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
