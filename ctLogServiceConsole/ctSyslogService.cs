using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.ServiceProcess;
using System.Xml;
using Microsoft.Win32;

namespace nsCtSysLog {
    public partial class ctSyslogService : ServiceBase {
        //private System.ComponentModel.IContainer components; TODO Remove this
        private const string sRegKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\CoreTechnology\\ctSyslog";
        ctSyslog ctService;
        public ctSyslogService() {
            this.ServiceName = "CtSyslogService";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }
        
        public void Start(string[] args) { OnStart(args); }
        
        protected override void OnStart(string[] args) {
            if (args.Length > 0) {
                ctService = new ctSyslog(args[0]);
                // Store Arg0 for later usage..
                Registry.SetValue(sRegKey, "", args[0]);
            } else {
                string sRegValue = (string) Registry.GetValue(sRegKey, "", "config.xml");
                if (sRegValue == null) sRegValue = "config.xml";
                ctService = new ctSyslog(sRegValue);
            }
        }
        
        protected override void OnStop() {
            ctService.Stop();
        }

        static void Main(params string[] args) {
            var service = new ctSyslogService();
            if (!Environment.UserInteractive) {
                var servicesToRun = new ServiceBase[] { service };
                ServiceBase.Run(servicesToRun);
                return;
            }
            Console.WriteLine("Running as Console Application");
            service.Start(args);
            // We now wait for a newline. If a New line is Received, we stop the Progi
            Console.WriteLine("Press <RETURN/ENTER> to Stop the Programm");
            string input = Console.ReadLine();
            service.Stop();

        }

        private void InitializeComponent() {

        }
    }
}
