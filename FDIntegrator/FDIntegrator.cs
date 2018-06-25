using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FDIntegrator.sync;
using FDIntegrator.config;
using System.IO;

namespace FDIntegrator
{
    public partial class FDIntegrator : ServiceBase
    {
        public FDIntegrator()
        {
            InitializeComponent();
            //FDIntegratorEventLog = new System.Diagnostics.EventLog();
            //if (!System.Diagnostics.EventLog.SourceExists("FDIntegratorSource1"))
            //{
            //    System.Diagnostics.EventLog.CreateEventSource(
            //        "FDIntegratorSource1", "FDIntegratorLog");
            //}
            //FDIntegratorEventLog.Source = "FDIntegratorSource1";
            //FDIntegratorEventLog.Log = "FDIntegratorLog";
            //
            DatabaseConnection.readConfigFile();
            // Set up a timer to trigger every set of time; note 1 min=60s=60000.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = DatabaseConnection.SYNC_INTERVAL; // 60000=60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        public FDIntegrator(int code)
        {
            DatabaseConnection.readConfigFile();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                this.WriteToFile("Service started");
            }
            catch (Exception e)
            {
                this.WriteToFile("Error on service start");
            }
        }

        protected override void OnStop()
        {
            try
            {
                this.WriteToFile("Service stopped");
            }
            catch (Exception e)
            {
                this.WriteToFile("Error on service stop");
            }
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            this.TasksAll();
        }

        public void TasksAll()
        {
            // Start of sync Job after the configured time lap
            this.WriteToFile("Sync process initiated/resumed");
            //Check for connectivity to the local database server and only log if the connection has failed
            if (DatabaseConnection.IsConnectionLocal())
            {
                //Check for connectivity to the remote database server and only log if the connection has failed
                if (DatabaseConnection.IsConnectionRemote())
                {
                    Sync SyncJob = new Sync();
                    try
                    {
                        //SOHS. Stock on hand - store
                        //a. Check availability of new records
                        if (SyncJob.NewRecordsCount("intf_store_stock") > 0)
                        {
                            //b. Check availability of remote connection
                            if (DatabaseConnection.IsConnectionRemote())
                            {
                                //c. Update the log file: Before start of the entity sync
                                this.WriteToFile("Sync entity start:store stock");
                                //d. Insert new data into the remote database<<includes update of sync status.
                                //d. Update the log with the results of the insert.
                                this.WriteToFile("Sync entity end:store stock:" + new SyncStoreStock().Sync());
                            }
                        }

                        //SOHP. Stock on hand - pharmacy
                        if (SyncJob.NewRecordsCount("intf_pharmacy_stock") > 0)
                        {
                            //b. Check availability of remote connection
                            if (DatabaseConnection.IsConnectionRemote())
                            {
                                //c. Update the log file: Before start of the entity sync
                                this.WriteToFile("Sync entity start:pharmacy stock");
                                //d. Insert new data into the remote database<<includes update of sync status.
                                //d. Update the log with the results of the insert.
                                this.WriteToFile("Sync entity end:pharmacy stock:" + new SyncPharmacyStock().Sync());
                            }
                        }
                        //SDPE. Stock dispensed
                        if (SyncJob.NewRecordsCount("intf_stock_dispensed") > 0)
                        {
                            //b. Check availability of remote connection
                            if (DatabaseConnection.IsConnectionRemote())
                            {
                                //c. Update the log file: Before start of the entity sync
                                this.WriteToFile("Sync entity start:stock_dispensed");
                                //d. Insert new data into the remote database<<includes update of sync status.
                                //d. Update the log with the results of the insert.
                                this.WriteToFile("Sync entity end:stock_dispensed:" + new SyncStockDispensed().Sync());
                            }
                        }
                        //SA. Stock adjustment
                        if (SyncJob.NewRecordsCount("intf_stock_adjustment") > 0)
                        {
                            //b. Check availability of remote connection
                            if (DatabaseConnection.IsConnectionRemote())
                            {
                                //c. Update the log file: Before start of the entity sync
                                this.WriteToFile("Sync entity start:stock_adjustment");
                                //d. Insert new data into the remote database<<includes update of sync status.
                                //d. Update the log with the results of the insert.
                                this.WriteToFile("Sync entity end:stock_adjustment:" + new SyncStockAdjustment().Sync());
                            }
                        }
                        //SRQ. Stock requisition
                        if (SyncJob.NewRecordsCount("intf_stock_requisition") > 0)
                        {
                            //b. Check availability of remote connection
                            if (DatabaseConnection.IsConnectionRemote())
                            {
                                //c. Update the log file: Before start of the entity sync
                                this.WriteToFile("Sync entity start:stock_requisition");
                                //d. Insert new data into the remote database<<includes update of sync status.
                                //d. Update the log with the results of the insert.
                                this.WriteToFile("Sync entity end:stock_requisition:" + new SyncStockRequisition().Sync());
                            }
                        }
                        //SI. Stock issuing
                        if (SyncJob.NewRecordsCount("intf_stock_issue") > 0)
                        {
                            //b. Check availability of remote connection
                            if (DatabaseConnection.IsConnectionRemote())
                            {
                                //c. Update the log file: Before start of the entity sync
                                this.WriteToFile("Sync entity start:stock_issue");
                                //d. Insert new data into the remote database<<includes update of sync status.
                                //d. Update the log with the results of the insert.
                                this.WriteToFile("Sync entity end:stock_issue:" + new SyncStockIssue().Sync());
                            }
                        }
                        //SRT. Stock return
                        if (SyncJob.NewRecordsCount("intf_stock_return") > 0)
                        {
                            //b. Check availability of remote connection
                            if (DatabaseConnection.IsConnectionRemote())
                            {
                                //c. Update the log file: Before start of the entity sync
                                this.WriteToFile("Sync entity start:stock_return");
                                //d. Insert new data into the remote database<<includes update of sync status.
                                //d. Update the log with the results of the insert.
                                this.WriteToFile("Sync entity end:stock_return:" + new SyncStockReturn().Sync());
                            }
                        }
                        //HFO. Health facility orders
                        if (SyncJob.NewRecordsCount("intf_facility_order") > 0)
                        {
                            //b. Check availability of remote connection
                            if (DatabaseConnection.IsConnectionRemote())
                            {
                                //c. Update the log file: Before start of the entity sync
                                this.WriteToFile("Sync entity start:facility_order");
                                //d. Insert new data into the remote database<<includes update of sync status.
                                //d. Update the log with the results of the insert.
                                this.WriteToFile("Sync entity end:facility_order:" + new SyncFacilityOrder().Sync());
                            }
                        }
                        //GRN. Goods received
                        if (SyncJob.NewRecordsCount("intf_goods_received") > 0)
                        {
                            //b. Check availability of remote connection
                            if (DatabaseConnection.IsConnectionRemote())
                            {
                                //c. Update the log file: Before start of the entity sync
                                this.WriteToFile("Sync entity start:goods_received");
                                //d. Insert new data into the remote database<<includes update of sync status.
                                //d. Update the log with the results of the insert.
                                this.WriteToFile("Sync entity end:goods_received:" + new SyncGoodsReceived().Sync());
                            }
                        }
                        //SDPO. Stock disposal
                        if (SyncJob.NewRecordsCount("intf_stock_disposed") > 0)
                        {
                            //b. Check availability of remote connection
                            if (DatabaseConnection.IsConnectionRemote())
                            {
                                //c. Update the log file: Before start of the entity sync
                                this.WriteToFile("Sync entity start:stock_disposed");
                                //d. Insert new data into the remote database<<includes update of sync status.
                                //d. Update the log with the results of the insert.
                                this.WriteToFile("Sync entity end:stock_disposed:" + new SyncStockDisposed().Sync());
                            }
                        }
                        //PV. Patient visit
                        if (SyncJob.NewRecordsCount("intf_patient_visit") > 0)
                        {
                            //b. Check availability of remote connection
                            if (DatabaseConnection.IsConnectionRemote())
                            {
                                //c. Update the log file: Before start of the entity sync
                                this.WriteToFile("Sync entity start:patient_visit");
                                //d. Insert new data into the remote database<<includes update of sync status.
                                //d. Update the log with the results of the insert.
                                this.WriteToFile("Sync entity end:patient_visit:" + new SyncPatientVisit().Sync());
                            }
                        }
                        //conn status
                        if (SyncJob.NewRecordsCount("intf_conn_status") > 0)
                        {
                            //b. Check availability of remote connection
                            if (DatabaseConnection.IsConnectionRemote())
                            {
                                //c. Update the log file: Before start of the entity sync
                                this.WriteToFile("Sync entity start:conn_status");
                                //d. Insert new data into the remote database<<includes update of sync status.
                                //d. Update the log with the results of the insert.
                                this.WriteToFile("Sync entity end:conn_status:" + new SyncConnStatus().Sync());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this.WriteToFile("Error:Sync:" + e.Message);
                    }
                }
                else
                {
                    this.WriteToFile("Remote database connection fail");
                }
                //Delete successfully synced data from the interface local database.
                new Sync().DeleteSyncedRecords("intf_conn_status");
                new Sync().DeleteSyncedRecords("intf_facility_config");
                new Sync().DeleteSyncedRecords("intf_facility_order");
                new Sync().DeleteSyncedRecords("intf_goods_received");
                new Sync().DeleteSyncedRecords("intf_patient_visit");
                new Sync().DeleteSyncedRecords("intf_pharmacy_stock");
                new Sync().DeleteSyncedRecords("intf_stock_adjustment");
                new Sync().DeleteSyncedRecords("intf_stock_dispensed");
                new Sync().DeleteSyncedRecords("intf_stock_disposed");
                new Sync().DeleteSyncedRecords("intf_stock_issue");
                new Sync().DeleteSyncedRecords("intf_stock_requisition");
                new Sync().DeleteSyncedRecords("intf_stock_return");
                new Sync().DeleteSyncedRecords("intf_store_stock");
            }
            else
            {
                this.WriteToFile("Local database connection fail");
            }
        }

        public void WriteToFile(string text)
        {
            string path = DatabaseConnection.LOG_PATH;
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(DateTime.Now.ToString("dd/MM/yy hh:mm:ss tt") + ": " + text);
                writer.Close();
            }
        }
    }
}
