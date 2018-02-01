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
    class Sync
    {
        public void UpdateLocalSyncStatus(String TableName, String IdColumn,int Status,long RecordId)
        {
            try
            {
                String sql = "UPDATE " + TableName + " SET sync_status=" + Status + " WHERE " + IdColumn + "=" + RecordId;
                SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception e)
            {
                //
            }
        }

        public long NewRecordsCount(String TableName)
        {
            long n = 0;
            String sql_from = "SELECT COUNT(*) as row_count FROM " + TableName + " WHERE sync_status=0";
            try
            {
                SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                SqlCommand cmd = new SqlCommand(sql_from, conn);
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (dr.Read())
                {
                    try
                    {
                        n = Convert.ToInt64(dr["row_count"]);
                    }
                    catch (InvalidCastException ice)
                    {
                        n = 0;
                    }
                }
                dr.Close();
            }
            catch (SqlException me)
            {
                n = 0;
            }
            return n;
        }

        public void DeleteSyncedRecords(String TableName)
        {
            try
            {
                String sql = "DELETE FROM " + TableName + " WHERE sync_status=1";
                SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception e)
            {
                //
            }
        }
    }
}
