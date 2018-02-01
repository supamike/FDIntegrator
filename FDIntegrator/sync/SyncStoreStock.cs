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
    class SyncStoreStock
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords= new Sync().NewRecordsCount("intf_store_stock");
            double RecordsBatchFactor = TotalRecords/DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            store_stock StoreStock = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_store_stock WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        StoreStock = new store_stock();
                        this.SetStoreStock(StoreStock, dr);
                        if (this.InsertStoreStock(StoreStock) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_store_stock", "intf_store_stock_id", 1, StoreStock.intf_store_stock_id);
                        }
                        StoreStock = null;
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

        public void SetStoreStock (store_stock StoreStock,SqlDataReader dr)
        {
            try
            {
                StoreStock.intf_store_stock_id = Convert.ToInt64(dr["intf_store_stock_id"]);
            }
            catch (InvalidCastException ice)
            {
                StoreStock.intf_store_stock_id = 0;
            }
            try
            {
                StoreStock.facility_code = Convert.ToString(dr["facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                StoreStock.facility_code = "";
            }
            try
            {
                StoreStock.product_code = Convert.ToString(dr["product_code"]);
            }
            catch (InvalidCastException ice)
            {
                StoreStock.product_code = "";
            }
            try
            {
                StoreStock.unit_code = Convert.ToString(dr["unit_code"]);
            }
            catch (InvalidCastException ice)
            {
                StoreStock.unit_code = "";
            }
            try
            {
                StoreStock.batch_number = Convert.ToString(dr["batch_number"]);
            }
            catch (InvalidCastException ice)
            {
                StoreStock.batch_number = "";
            }
            try
            {
                StoreStock.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                StoreStock.cdc_date = Convert.ToDateTime(null);
            }
            try
            {
                StoreStock.quantity = Convert.ToSingle(dr["quantity"]);
            }
            catch (InvalidCastException ice)
            {
                StoreStock.quantity = 0;
            }
            try
            {
                StoreStock.manufacture_date = Convert.ToDateTime(dr["manufacture_date"]);
            }
            catch (InvalidCastException ice)
            {
                StoreStock.manufacture_date = Convert.ToDateTime(null);
            }
            try
            {
                StoreStock.expiry_date = Convert.ToDateTime(dr["expiry_date"]);
            }
            catch (InvalidCastException ice)
            {
                StoreStock.expiry_date = Convert.ToDateTime(null);
            }
        }

        public int InsertStoreStock(store_stock StoreStock)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO intf_store_stock" +
                                "(" +
                                "intf_store_stock_id," +
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
                                StoreStock.intf_store_stock_id + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StoreStock.cdc_date) + "','" +
                                StoreStock.product_code + "','" +
                                StoreStock.facility_code + "','" +
                                StoreStock.unit_code + "','" +
                                StoreStock.batch_number + "'," +
                                StoreStock.quantity + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.Now) + "'," +
                                0 + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StoreStock.manufacture_date) + "'," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", StoreStock.expiry_date) + "'" +
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
