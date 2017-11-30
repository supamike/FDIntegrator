using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MFIService.config;

namespace MFIService.sync
{
    class Sync
    {
        public void SyncStoreStock()
        {
            String facility_code;
            DateTime cdc_date;
            String product_code;
            String unit_code;
            String batch_number;
            float quantity;
            int i = 0;
            //TOP 100 
            String sql_from = "SELECT * FROM intf_store_stock WHERE 1=1";
            try
            {
                SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                SqlCommand cmd = new SqlCommand(sql_from, conn);
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    try
                    {
                        facility_code = Convert.ToString(dr["facility_code"]);
                    }
                    catch (InvalidCastException ice)
                    {
                        facility_code = "";
                    }
                    try
                    {
                        product_code = Convert.ToString(dr["product_code"]);
                    }
                    catch (InvalidCastException ice)
                    {
                        product_code = "";
                    }
                    try
                    {
                        unit_code = Convert.ToString(dr["unit_code"]);
                    }
                    catch (InvalidCastException ice)
                    {
                        unit_code = "";
                    }
                    try
                    {
                        batch_number = Convert.ToString(dr["batch_number"]);
                    }
                    catch (InvalidCastException ice)
                    {
                        batch_number = "";
                    }
                    try
                    {
                        cdc_date = Convert.ToDateTime(dr["cdc_date"]);
                    }
                    catch (InvalidCastException ice)
                    {
                        cdc_date = Convert.ToDateTime(null);
                    }
                    try
                    {
                        quantity = Convert.ToSingle(dr["quantity"]);
                    }
                    catch (InvalidCastException ice)
                    {
                        quantity = 0;
                    }
                    //MessageBox.Show("EXTRACTED:" + product_code);
                    String sql_to = "INSERT INTO stage_store_stock" +
                        "(" +
                        "cdc_date," +
                        "intf_product_code," +
                        "intf_facility_code," +
                        "intf_unit_code," +
                        "batch_number," +
                        "quantity," +
                        "add_date," +
                        "load_status" +
                        ") " +
                        " VALUES" +
                        "(" +
                        "'" + string.Format("{0:yyyy-MM-dd HH:mm}", cdc_date) + "','" +
                        product_code + "','" +
                        facility_code + "','" +
                        unit_code + "','" +
                        batch_number + "'," +
                        quantity + "," +
                        "'" + string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.Now) + "'," +
                        0 + "" +
                        ") ";
                    //Console.WriteLine(sql_to);
                    SqlConnection conn2 = new SqlConnection(DatabaseConnection.getRemoteConnectionString());
                    SqlCommand cmd2 = new SqlCommand(sql_to, conn2);
                    cmd2.Connection.Open();
                    cmd2.ExecuteNonQuery();
                    cmd2.Connection.Close();
                    //MessageBox.Show("LOADED:" + product_code);
                    i = i + 1;
                    //Console.WriteLine("Loaded:" + i);
                }
                dr.Close();
                //MessageBox.Show("FINISHED");
            }
            catch (SqlException me)
            {
                Console.WriteLine(me.StackTrace);
            }

        }

    }
}
