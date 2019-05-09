using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace TestService
{
    public partial class MainService : ServiceBase
    {
        Timer timer;
        int cycleInterval = 60;
        LogManager log = new LogManager();

        public MainService()
        {
            InitializeComponent();
        }

        private void Cycle()
        {
            //find start timestamp in log
            DateTime lastKnownCycleTime = GetLastBroadcastTime();
            DateTime currentCycleTime = DateTime.Now;

            //REOCCURRING SERVICE ACTION HERE
            log.Debug(String.Format("Logged Debug Diagnostics at {0}", currentCycleTime));
            log.Info(String.Format("Logged Info at {0}", currentCycleTime));
            log.Warn(String.Format("Logged Warning at {0}", currentCycleTime));
            log.Error(String.Format("Logged Error at {0}", currentCycleTime));

            //write end timestamp to log
            Extensions.WriteBroadcastTimeLogFile(currentCycleTime);
        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer();
            timer.Interval = 1;
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
        }

        protected void OnTimer(object sender, ElapsedEventArgs args)
        {
            timer.Stop();
            timer.Interval = cycleInterval * 1000;
            Cycle();
            timer.Start();
        }

        protected DateTime GetLastBroadcastTime()
        {
            DateTime lastBroadcastTime = DateTime.Now;
            using (var file = File.OpenRead(Extensions.timeLogFile))
            {
                using (var reader = new StreamReader(file))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        lastBroadcastTime = Convert.ToDateTime(line);
                    }
                }
            }
            return lastBroadcastTime;
        }
    }
}
