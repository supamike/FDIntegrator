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
    class SyncStockRequisition
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords= new Sync().NewRecordsCount("intf_stock_requisition");
            double RecordsBatchFactor = TotalRecords/DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            stock_requisition StockRequisition = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_stock_requisition WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        StockRequisition = new stock_requisition();
                        this.SetStockRequisition(StockRequisition, dr);
                        if (this.InsertStockRequisition(StockRequisition) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_stock_requisition", "intf_stock_requisition_id", 1, StockRequisition.intf_stock_requisition_id);
                        }
                        StockRequisition = null;
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

        public void SetStockRequisition (stock_requisition StockRequisition,SqlDataReader dr)
        {
            try
            {
                StockRequisition.intf_stock_requisition_id = Convert.ToInt64(dr["intf_stock_requisition_id"]);
            }
            catch (InvalidCastException ice)
            {
                StockRequisition.intf_stock_requisition_id = 0;
            }
            try
            {
                StockRequisition.facility_code = Convert.ToString(dr["facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockRequisition.facility_code = "";
            }
            try
            {
                StockRequisition.product_code = Convert.ToString(dr["product_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockRequisition.product_code = "";
            }
            try
            {
                StockRequisition.unit_code = Convert.ToString(dr["unit_code"]);
            }
            catch (InvalidCastException ice)
            {
                StockRequisition.unit_code = "";
            }
            try
            {
                StockRequisition.requisition_number = Convert.ToString(dr["requisition_number"]);
            }
            catch (InvalidCastException ice)
            {
                StockRequisition.requisition_number = "";
            }
            try
            {
                StockRequisition.requisition_date = Convert.ToDateTime(dr["requisition_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockRequisition.requisition_date = Convert.ToDateTime(null);
            }
            try
            {
                StockRequisition.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                StockRequisition.cdc_date = Convert.ToDateTime(null);
            }
            try
            {
                StockRequisition.quantity = Convert.ToSingle(dr["quantity"]);
            }
            catch (InvalidCastException ice)
            {
                StockRequisition.quantity = 0;
            }
        }

        public int InsertStockRequisition(stock_requisition StockRequisition)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO intf_stock_requisition" +
                                "(" +
                                "intf_stock_requisition_id," +
                                "cdc_date," +
                                "intf_product_code," +
                                "intf_facility_code," +
                                "intf_unit_code," +
                                "batch_number," +
                                "quantity," +
                                "add_date," +
                                "load_status," +
                                "manufacture_date," +
                                "expiry_date" +
                                ") " +
                                " VALUES" +
                                "(" +
                                StockRequisition.intf_stock_requisition_id + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockRequisition.cdc_date) + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StockRequisition.requisition_date) + "'," +
                                StockRequisition.product_code + "','" +
                                StockRequisition.facility_code + "','" +
                                StockRequisition.unit_code + "','" +
                                StockRequisition.quantity + "," +
                                StockRequisition.requisition_number + "'," +
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
