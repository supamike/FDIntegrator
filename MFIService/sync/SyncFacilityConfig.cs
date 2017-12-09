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
    class SyncFacilityConfig
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords= new Sync().NewRecordsCount("intf_facility_config");
            double RecordsBatchFactor = TotalRecords/DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            facility_config FacilityConfig = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_facility_config WHERE 1=1";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        FacilityConfig = new facility_config();
                        this.SetFacilityConfig(FacilityConfig, dr);
                        if (this.InsertFacilityConfig(FacilityConfig) == 1)
                        {
                            //update sync status
                           SyncPass = SyncPass + 1;
                            //new Sync().UpdateLocalSyncStatus("intf_facility_config", "intf_facility_config_id", 1, FacilityConfig.intf_facility_config_id);
                        }
                        FacilityConfig = null;
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

        public void SetFacilityConfig(facility_config FacilityConfig, SqlDataReader dr)
        {
            try
            {
                FacilityConfig.intf_facility_config_id = Convert.ToInt64(dr["intf_facility_config_id"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityConfig.intf_facility_config_id = 0;
            }
            try
            {
                FacilityConfig.facility_code = Convert.ToString(dr["facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityConfig.facility_code = "";
            }
            try
            {
                FacilityConfig.system_code = Convert.ToString(dr["system_code"]);
            }
            catch (InvalidCastException ice)
            {
                FacilityConfig.system_code = "";
            }
            
        }

        public int InsertFacilityConfig(facility_config FacilityConfig)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO facility_option" +
                                "(" +
                                "health_facility_code," +
                                "system_detail_id," +
                                ") " +
                                " VALUES" +
                                "(" +
                                FacilityConfig.facility_code + "','" +
                                FacilityConfig.system_code + "','" +
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
