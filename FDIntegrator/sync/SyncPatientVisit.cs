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
    class SyncPatientVisit
    {
        public String Sync()
        {
            int Loops = 1;
            int loop = 1;
            double TotalRecords= new Sync().NewRecordsCount("intf_patient_visit");
            double RecordsBatchFactor = TotalRecords/DatabaseConnection.SYNC_BATCH_SIZE;
            Loops = (Int32)Math.Ceiling(RecordsBatchFactor);
            int i = 0;
            int SyncPass = 0;
            patient_visit PatientVisit = null;
            while (loop <= Loops)
            {
                String sql_from = "SELECT * FROM intf_patient_visit WHERE sync_status=0";
                try
                {
                    SqlConnection conn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
                    SqlCommand cmd = new SqlCommand(sql_from, conn);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        PatientVisit = new patient_visit();
                        this.SetPatientVisit(PatientVisit, dr);
                        if (this.InsertPatientVisit(PatientVisit) == 1)
                        {
                            //update sync status
                            SyncPass = SyncPass + 1;
                            new Sync().UpdateLocalSyncStatus("intf_patient_visit", "intf_patient_visit_id", 1, PatientVisit.intf_patient_visit_id);
                        }
                        PatientVisit = null;
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

        public void SetPatientVisit (patient_visit PatientVisit,SqlDataReader dr)
        {
            try
            {
                PatientVisit.intf_patient_visit_id = Convert.ToInt64(dr["intf_patient_visit_id"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.intf_patient_visit_id = 0;
            }
            try
            {
                PatientVisit.facility_code = Convert.ToString(dr["facility_code"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.facility_code = "";
            }
            try
            {
                PatientVisit.cdc_date = Convert.ToDateTime(dr["cdc_date"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.cdc_date = Convert.ToDateTime(null);
            }
            
            try
            {
                PatientVisit.patient_code = Convert.ToString(dr["patient_code"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.patient_code = "";
            }
            try
            {
                PatientVisit.sex = Convert.ToString(dr["sex"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.sex = "";
            }
            try
            {
                PatientVisit.age = Convert.ToInt32(dr["age"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.age =0;
            }
            try
            {
                PatientVisit.visit_date = Convert.ToDateTime(dr["visit_date"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.visit_date = Convert.ToDateTime(null);
            }
            try
            {
                PatientVisit.product_code = Convert.ToString(dr["product_code"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.product_code = "";
            }
            try
            {
                PatientVisit.batch_number = Convert.ToString(dr["batch_number"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.batch_number = "";
            }
            try
            {
                PatientVisit.unit_code = Convert.ToString(dr["unit_code"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.unit_code = "";
            }
            try
            {
                PatientVisit.product_quantity = Convert.ToSingle(dr["product_quantity"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.product_quantity = 0;
            }
            try
            {
                PatientVisit.art_start_date = Convert.ToDateTime(dr["art_start_date"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.art_start_date = Convert.ToDateTime(null);
            }
            try
            {
                PatientVisit.regimen_code = Convert.ToString(dr["regimen_code"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.regimen_code = "";
            }
            try
            {
                PatientVisit.patient_category = Convert.ToString(dr["patient_category"]);
            }
            catch (InvalidCastException ice)
            {
                PatientVisit.patient_category = "";
            }
           }

        public int InsertPatientVisit(patient_visit PatientVisit)
        {
            int status = 0;
            try
            {
                String sql_to = "INSERT INTO intf_patient_visit" +
                                "(" +
                                "intf_patient_visit_id," +
                                "cdc_date," +
                                "visit_date," +
                                "intf_product_code," +
                                "intf_facility_code," +
                                "intf_unit_code," +
                                "patient_code," +
                                "sex," +
                                "age," +
                                "batch_number," +
                                "product_quantity," +
                                "art_start_date," +
                                "intf_regimen_code," +
                                "patient_category," +
                                "add_date," +
                                "load_status," +
                                ") " +
                                " VALUES" +
                                "(" +
                                PatientVisit.intf_patient_visit_id + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", PatientVisit.cdc_date) + "','" +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", PatientVisit.visit_date) + "','" +
                                PatientVisit.product_code + "','" +
                                PatientVisit.facility_code + "','" +
                                PatientVisit.unit_code + "','" +
                                PatientVisit.patient_code + "','" +
                                PatientVisit.sex + "','" +
                                PatientVisit.age + "','" +
                                PatientVisit.batch_number + "'," +
                                PatientVisit.product_quantity + "," +
                                "'" + string.Format("{0:yyyy-MM-dd HH:mm}", PatientVisit.art_start_date) + "','" +
                                PatientVisit.regimen_code + "," +
                                PatientVisit.patient_category + "," +
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
