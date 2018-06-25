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
    class SyncConnStatus
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords= new Sync().NewRecordsCount("intf_conn_status");
            double RecordsBatchFactor = TotalRecords/DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            conn_status ConnStatus = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_conn_status WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        ConnStatus = new conn_status();
                        this.SetConnStatus(ConnStatus, dr);
                        if (this.InsertConnStatus(ConnStatus) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_conn_status", "intf_conn_status_id", 1, ConnStatus.intf_conn_status_id);
                        }
                        ConnStatus = null;
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

        public void SetConnStatus (conn_status ConnStatus,SqlDataReader dr)
        {
            try
            {
                ConnStatus.intf_conn_status_id = Convert.ToInt64(dr["intf_conn_status_id"]);
            }
            catch (InvalidCastException ice)
            {
                ConnStatus.intf_conn_status_id = 0;
            }
            try
            {
                ConnStatus.facility_code = Convert.ToString(dr["facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                ConnStatus.facility_code = "";
            }
            try
            {
                ConnStatus.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                ConnStatus.cdc_date = Convert.ToDateTime(null);
            }
            try
            {
                ConnStatus.status_date = Convert.ToDateTime(dr["status_date"]);
            }
            catch (InvalidCastException ice)
            {
                ConnStatus.status_date = Convert.ToDateTime(null);
            }

            try
            {
                ConnStatus.status_dispense_mode = Convert.ToInt16(dr["status_dispense_mode"]);
            }
            catch (InvalidCastException ice)
            {
                ConnStatus.status_dispense_mode = 0;
            }
            try
            {
                ConnStatus.status_dispense_mode_data = Convert.ToInt16(dr["status_dispense_mode_data"]);
            }
            catch (InvalidCastException ice)
            {
                ConnStatus.status_dispense_mode_data = 0;
            }
            try
            {
                ConnStatus.status_connect = Convert.ToInt16(dr["status_connect"]);
            }
            catch (InvalidCastException ice)
            {
                ConnStatus.status_connect = 0;
            }
        }

        public int InsertConnStatus(conn_status ConnStatus)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO intf_conn_status" +
                                "(" +
                                "intf_conn_status_id," +
                                "cdc_date," +
                                "status_date," +
                                "status_connect," +
                                "status_dispense_mode," +
                                "status_dispense_mode_data," +
                                "add_date," +
                                "load_status" +
                                ") " +
                                " VALUES" +
                                "(" +
                                ConnStatus.intf_conn_status_id + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", ConnStatus.cdc_date) + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", ConnStatus.status_date) + "','" +
                                ConnStatus.status_connect + "','" +
                                ConnStatus.status_dispense_mode + "','" +
                                ConnStatus.status_dispense_mode_data + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.Now) + "'," +
                                0 + 
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
