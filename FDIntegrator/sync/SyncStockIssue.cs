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
    class SyncStockIssue
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords= new Sync().NewRecordsCount("intf_stock_issue");
            double RecordsBatchFactor = TotalRecords/DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            stock_issue StockIssue = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_stock_issue WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        StockIssue = new stock_issue();
                        this.SetStockIssue(StockIssue, dr);
                        if (this.InsertStockIssue(StockIssue) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_store_stock", "intf_stock_issue_id", 1, StockIssue.intf_stock_issue_id);
                        }
                        StockIssue = null;
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

        public void SetStockIssue (stock_issue StockIssue,SqlDataReader dr)
        {
            try
            {
                StockIssue.intf_stock_issue_id = Convert.ToInt64(dr["intf_stock_issue_id"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.intf_stock_issue_id = 0;
            }
            try
            {
                StockIssue.facility_code = Convert.ToString(dr["intf_facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.facility_code = "";
            }
            try
            {
                StockIssue.product_code = Convert.ToString(dr["intf_product_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.product_code = "";
            }
            try
            {
                StockIssue.unit_code = Convert.ToString(dr["intf_unit_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.unit_code = "";
            }
            try
            {
                StockIssue.issue_number = Convert.ToString(dr["issue_number"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.issue_number = "";
            }
            try
            {
                StockIssue.batch_number = Convert.ToString(dr["batch_number"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.batch_number = "";
            }
            try
            {
                StockIssue.issue_number = Convert.ToString(dr["issue_number"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.issue_number = "";
            }
            try
            {
                StockIssue.requisition_number = Convert.ToString(dr["requisition_number"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.requisition_number = "";
            }
            try
            {
                StockIssue.issue_date = Convert.ToDateTime(dr["issue_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.issue_date = Convert.ToDateTime(null);
            }
            try
            {
                StockIssue.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.cdc_date = Convert.ToDateTime(null);
            }
            try
            {
                StockIssue.quantity = Convert.ToSingle(dr["quantity"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.quantity = 0;
            }
            try
            {
                StockIssue.manufacture_date = Convert.ToDateTime(dr["manufacture_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.manufacture_date = Convert.ToDateTime(null);
            }
            try
            {
                StockIssue.expiry_date = Convert.ToDateTime(dr["expiry_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockIssue.expiry_date = Convert.ToDateTime(null);
            }
        }

        public int InsertStockIssue(stock_issue StockIssue)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO intf_stock_issue" +
                                "(" +
                                "intf_stock_issue_id," +
                                "cdc_date," +
                                "issue_date," +
                                "intf_product_code," +
                                "intf_facility_code," +
                                "intf_unit_code," +
                                "batch_number," +
                                "quantity," +
                                "issue_number," +
                                "requisition_number," +
                                "add_date," +
                                "load_status," +
                                "manufacture_date," +
                                "expiry_date" +
                                ") " +
                                " VALUES" +
                                "(" +
                                StockIssue.intf_stock_issue_id + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockIssue.cdc_date) + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockIssue.issue_date) + "','" +
                                StockIssue.product_code + "','" +
                                StockIssue.facility_code + "','" +
                                StockIssue.unit_code + "','" +
                                StockIssue.batch_number + "'," +
                                StockIssue.quantity + "," +
                                StockIssue.issue_number + "'," +
                                StockIssue.requisition_number + "'," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.Now) + "'," +
                                0 + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockIssue.manufacture_date) + "'," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockIssue.expiry_date) + "'" +
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
