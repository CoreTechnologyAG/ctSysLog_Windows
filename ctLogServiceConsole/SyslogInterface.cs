using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nsCtSysLog {
    interface SyslogInterface {
        bool init(String sIP, int iPort);
        bool shutdown();
        bool sendMessage(String sMessage);
    }
}
