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
using MFIService.sync;
using MFIService.config;
using System.IO;

namespace MFIService
{
    public partial class MFIService : ServiceBase
    {
        public MFIService()
        {
            InitializeComponent();
            MFIServiceEventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("MFIServiceSource1"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "MFIServiceSource1", "MFIServiceLog");
            }
            MFIServiceEventLog.Source = "MFIServiceSource1";
            MFIServiceEventLog.Log = "MFIServiceLog";
            DatabaseConnection.readConfigFile();
            // Set up a timer to trigger every set of time; note 1 min=60s=60000.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = DatabaseConnection.SYNC_INTERVAL; // 60000=60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
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
                        if (SyncJob.NewRecordsCount("intf_store_stock")>0)
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
                        //SDPE. Stock dispensed
                        //SA. Stock adjustment
                        //SRQ. Stock requisition
                        //SI. Stock issuing
                        //SRT. Stock return
                        //HFO. Health facility orders
                        //GRN. Goods received
                        //SDPO. Stock disposal
                        //PV. Patient visit
                    }
                    catch (Exception e)
                    {
                        this.WriteToFile("Error:Sync store stock:" + e.Message);
                    }
                }
                else
                {
                    this.WriteToFile("Remote database connection fail");
                }
                //Delete successfully synced data from the interface local database.
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
