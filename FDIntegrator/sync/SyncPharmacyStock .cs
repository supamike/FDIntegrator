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
    class SyncPharmacyStock
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords = new Sync().NewRecordsCount("intf_pharmacy_stock");
            double RecordsBatchFactor = TotalRecords / DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            pharmacy_stock PharmacyStock = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_pharmacy_stock WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        PharmacyStock = new pharmacy_stock();
                        this.SetPharmacyStock(PharmacyStock, dr);
                        if (this.InsertPharmacyStock(PharmacyStock) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_pharmacy_stock", "intf_pharmacy_stock_id", 1, PharmacyStock.intf_pharmacy_stock_id);
                        }
                        PharmacyStock = null;
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

        public void SetPharmacyStock(pharmacy_stock PharmacyStock, SqlDataReader dr)
        {
            try
            {
                PharmacyStock.intf_pharmacy_stock_id = Convert.ToInt64(dr["intf_pharmacy_stock_id"]);
            }
            catch (InvalidCastException ice)
            {
                PharmacyStock.intf_pharmacy_stock_id = 0;
            }
            try
            {
                PharmacyStock.facility_code = Convert.ToString(dr["facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                PharmacyStock.facility_code = "";
            }
            try
            {
                PharmacyStock.product_code = Convert.ToString(dr["product_code"]);
            }
            catch (InvalidCastException ice)
            {
                PharmacyStock.product_code = "";
            }
            try
            {
                PharmacyStock.unit_code = Convert.ToString(dr["unit_code"]);
            }
            catch (InvalidCastException ice)
            {
                PharmacyStock.unit_code = "";
            }
            try
            {
                PharmacyStock.batch_number = Convert.ToString(dr["batch_number"]);
            }
            catch (InvalidCastException ice)
            {
                PharmacyStock.batch_number = "";
            }
            try
            {
                PharmacyStock.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                PharmacyStock.cdc_date = Convert.ToDateTime(null);
            }
            try
            {
                PharmacyStock.quantity = Convert.ToSingle(dr["quantity"]);
            }
            catch (InvalidCastException ice)
            {
                PharmacyStock.quantity = 0;
            }
            try
            {
                PharmacyStock.manufacture_date = Convert.ToDateTime(dr["manufacture_date"]);
            }
            catch (InvalidCastException ice)
            {
                PharmacyStock.manufacture_date = Convert.ToDateTime(null);
            }
            try
            {
                PharmacyStock.expiry_date = Convert.ToDateTime(dr["expiry_date"]);
            }
            catch (InvalidCastException ice)
            {
                PharmacyStock.expiry_date = Convert.ToDateTime(null);
            }
        }

        public int InsertPharmacyStock(pharmacy_stock PharmacyStock)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO intf_pharmacy_stock" +
                                "(" +
                                "intf_pharmacy_stock_id," +
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
                                PharmacyStock.intf_pharmacy_stock_id + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", PharmacyStock.cdc_date) + "','" +
                                PharmacyStock.product_code + "','" +
                                PharmacyStock.facility_code + "','" +
                                PharmacyStock.unit_code + "','" +
                                PharmacyStock.batch_number + "'," +
                                PharmacyStock.quantity + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.Now) + "'," +
                                0 + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", PharmacyStock.manufacture_date) + "'," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", PharmacyStock.expiry_date) + "'" +
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