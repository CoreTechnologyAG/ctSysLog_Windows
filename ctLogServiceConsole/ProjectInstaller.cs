using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.ServiceProcess;
namespace nsCtSysLog {
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer {
        private ServiceInstaller ctServiceInstaller;
        private ServiceProcessInstaller ctProcessInstaller;
        public ProjectInstaller() {
            ctProcessInstaller = new ServiceProcessInstaller();
            ctServiceInstaller = new ServiceInstaller();

            ctProcessInstaller.Account = ServiceAccount.LocalSystem;
            ctServiceInstaller.StartType = ServiceStartMode.Automatic;
            ctServiceInstaller.ServiceName = "ctSyslogService";
            Installers.Add(ctServiceInstaller);
            Installers.Add(ctProcessInstaller);
        }
    }
}
