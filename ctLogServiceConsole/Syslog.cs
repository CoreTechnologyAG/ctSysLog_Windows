using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Configuration;

namespace nsCtSysLog {
    class Syslog {
        private SyslogConfig oSyslogConfig;
        private SyslogInterface oSyslogInterface = null;
        public Syslog(SyslogConfig oSyslogConfig) {
            this.oSyslogConfig = oSyslogConfig;
            if (oSyslogConfig.getService().Equals("tcp", StringComparison.OrdinalIgnoreCase)) {    // TCP
                SyslogTCP oTCP = new SyslogTCP();
                oTCP.init(oSyslogConfig.getSyslogIP(), oSyslogConfig.getSyslogPort());
                oSyslogInterface = oTCP;
            } else if (oSyslogConfig.getService().Equals("relp", StringComparison.OrdinalIgnoreCase)) {  // RELP ??
                // TODO FIXME NOT YET IMPLEMENTED
                Environment.Exit(99);
            } else {    // UDP as Fallback
                SyslogUDP oUDP = new SyslogUDP();
                oUDP.init(oSyslogConfig.getSyslogIP(), oSyslogConfig.getSyslogPort());
                oSyslogInterface = oUDP;
            }
        }

        public void SendMessage(string message, string appname, string type) {
            string sMessage = "";
            foreach (string sFormatPart in oSyslogConfig.getMessageFormat()) {
                switch (sFormatPart) {
                    case "SP": 
                        sMessage += " "; break;
                    case "UNIXTIMESTAMP": 
                        sMessage += UnixTimeStampUTC(); break;
                    case "TIMESTAMP": 
                        sMessage += FormatedTimeStamp(oSyslogConfig.getTimeFormat()); break;
                    case "SOURCE":
                        sMessage += createAPPNAME(appname); break;
                    case "HOST":
                        sMessage += createHOSTNAME(); break;
                    case "MSG":
                        sMessage += createMESSAGE(message); break;
                    case "PROCID":
                        sMessage += createPROCID(); break;
                    case "MSGID":
                        sMessage += createMSGID(); break;
                    case "PRI":
                        sMessage += createPRI(5,3); break;
                    case "TYPE":
                        sMessage += type; break;
                    default: sMessage += sFormatPart; break;
                }
            }
            sMessage += createNL();
            oSyslogInterface.sendMessage(sMessage);
        }

        public void Stop() {
            oSyslogInterface.shutdown();
        }

        private String createPRI(int iFacility, int iSeverity) { return "<" + (iFacility * 8 + iSeverity).ToString() + ">"; }
        private string createSP() { return " "; }
        private string createTIMESTAMP() { return UnixTimeStampUTC().ToString(); }
        private string createHOSTNAME() { return oSyslogConfig.getHostname(); }
        private string createAPPNAME(String sAppname) {
            String sNewAppName = sAppname.Replace(" ", "").Replace("-", "");
            if (sNewAppName.Length > oSyslogConfig.getMaxAppnameLength()) sNewAppName = sNewAppName.Substring(0, oSyslogConfig.getMaxAppnameLength());
            return sNewAppName;
        }
        private string createPROCID() { return "[" + (Thread.CurrentThread.ManagedThreadId).ToString() + "]:"; } // FIXME Try to get a ID here from EventLog
        private string createMSGID() { return "0"; }  // FIXME.. something usefull here??
        private string createMESSAGE(String message) { return message.Replace("\n", ""); }  // TODO Make this Thing work better
        private string createNL() { return "\n"; }

        private long UnixTimeStampUTC() {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long) timeSpan.TotalSeconds;
        }

        private String FormatedTimeStamp(string sFormat) {
            DateTime oDateTime = new DateTime();
            oDateTime = DateTime.Now.ToUniversalTime();
            return oDateTime.ToString(sFormat);  
        }
    }
}
