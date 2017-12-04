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
        int eventId=0;
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
                this.WriteToFile("Started");
            }catch(Exception e)
            {
                this.WriteToFile("Error on start");
            }
        }

        protected override void OnStop()
        {
            try
            {
                this.WriteToFile("Stopped");
            }
            catch (Exception e)
            {
                this.WriteToFile("Error on stop");
            }
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // SYNC
            Sync SyncJob1 = new Sync();
            try
            {
                this.WriteToFile("Sync store stock:started");
                SyncJob1.SyncStoreStock();
                this.WriteToFile("Sync store stock:ended");
            }
            catch(Exception e)
            {
                this.WriteToFile("Error:Sync store stock:" + e.Message);
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
