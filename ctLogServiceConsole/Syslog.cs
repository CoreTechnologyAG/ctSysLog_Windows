using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Configuration;

namespace nsCtSysLog {
    class Syslog {
        private SyslogConfig scSyslogConfig;
        private SyslogInterface si = null;
        public Syslog(SyslogConfig scSyslogConfig) {
            this.scSyslogConfig = scSyslogConfig;
            if (scSyslogConfig.getService().Equals("tcp", StringComparison.OrdinalIgnoreCase)) {    // TCP
                SyslogTCP oTCP = new SyslogTCP();
                oTCP.init(scSyslogConfig.getSyslogIP(), scSyslogConfig.getSyslogPort());
                si = oTCP;
            } else if (scSyslogConfig.getService().Equals("relp", StringComparison.OrdinalIgnoreCase)) {  // RELP ??
                // TODO FIXME NOT YET IMPLEMENTED
                Environment.Exit(99);
            } else {    // UDP as Fallback
                SyslogUDP oUDP = new SyslogUDP();
                oUDP.init(scSyslogConfig.getSyslogIP(), scSyslogConfig.getSyslogPort());
                si = oUDP;
            }
        }

        public void SendMessage(string message, string appname, string type) {
            string sMessage = "";
            foreach (string s in scSyslogConfig.getMessageFormat()) {
                switch (s) {
                    case "SP": 
                        sMessage += sMessage + " "; return;
                    case "UNIXTIMESTAMP": 
                        sMessage += sMessage + UnixTimeStampUTC(); return;
                    case "TIMESTAMP": 
                        sMessage += sMessage + FormatedTimeStamp("yyyy-MM-ddTHH:mm:ss.ffZ"); return;
                    case "SOURCE":
                        sMessage += sMessage + createAPPNAME(appname); return;
                    case "HOST":
                        sMessage += sMessage + createHOSTNAME(); return;
                    case "MSG":
                        sMessage += sMessage + createMESSAGE(message); return;
                    case "PROCID":
                        sMessage += sMessage + createPROCID(); return;
                    case "MSGID":
                        sMessage += sMessage + createMSGID(); return;
                    case "PRI":
                        sMessage += sMessage + createPRI(5,3); return;
                    case "TYPE":
                        sMessage += sMessage + type; return;
                    default: sMessage += s; return;
                }
            }
            sMessage += sMessage + createNL();
            si.sendMessage(sMessage);
        }

        public void Stop() {
            si.shutdown();
        }

        private String createPRI(int Facility, int Severity) { return "<" + (Facility * 8 + Severity).ToString() + ">"; }
        //private String createVERSION() { return "1"; }
        private string createSP() { return " "; }
        private string createTIMESTAMP() { return UnixTimeStampUTC().ToString(); }
        private string createHOSTNAME() { return scSyslogConfig.getHostname(); }
        private string createAPPNAME(String sAppname) {
            String sNewAppName = sAppname.Replace(" ", "").Replace("-", "");
            //if (sNewAppName.Length > 16) sNewAppName = sNewAppName.Substring(0, 16);
            return sNewAppName;
        }
        private string createPROCID() { return "[" + (Thread.CurrentThread.ManagedThreadId).ToString() + "]:"; } // FIXME Try to get a ID here from EventLog
        private string createMSGID() { return "0"; }  // FIXME.. something usefull here??
        private string createSTRUCTDATA() { return "-"; }  // FIXME.. This is fency... never seens something usefull here
        private string createMESSAGE(String message) { return message.Replace("\n", ""); }  // TODO Make this Thing work better
        private string createNL() { return "\n"; }

        private long UnixTimeStampUTC() {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long) timeSpan.TotalSeconds;
        }

        private String FormatedTimeStamp(string sFormat) {
            DateTime dt = new DateTime();
            dt = DateTime.Now.ToUniversalTime();
            return dt.ToString(sFormat); // "yyyy-MM-ddTHH:mm:ss.ffZ"
        }
    }
}
