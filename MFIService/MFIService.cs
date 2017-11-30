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
            // Set up a timer to trigger every set of time; note 1 min=60s=60000.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 30000; // 60000=60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                MFIServiceEventLog.WriteEntry("In onStart:Bfr Config");
                DatabaseConnection.readConfigFile();
                MFIServiceEventLog.WriteEntry("In onStart:Afr Config:" + DatabaseConnection.getLocalConnectionString());
            }catch(Exception e)
            {
                MFIServiceEventLog.WriteEntry("In onStart:Error");
            }
        }

        protected override void OnStop()
        {
            MFIServiceEventLog.WriteEntry("In onStop");
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // SYNC
            Sync SyncJob1 = new Sync();
            MFIServiceEventLog.WriteEntry("Running-Bfr Sync", EventLogEntryType.Information, eventId++);
            SyncJob1.SyncStoreStock();
            MFIServiceEventLog.WriteEntry("Running-Afr Sync", EventLogEntryType.Information, eventId++);
        }
    }
}
