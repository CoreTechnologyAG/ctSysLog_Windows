using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;

namespace nsCtSysLog {
    class Syslog {
        private Socket server = null;
        private IPEndPoint RemoteEndPoint;
        private SyslogConfig scSyslogConfig;
        public Syslog(SyslogConfig scSyslogConfig) {
            this.scSyslogConfig = scSyslogConfig;
            RemoteEndPoint = new IPEndPoint(IPAddress.Parse(scSyslogConfig.getSyslogIP()), scSyslogConfig.getSyslogPort());
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        
        public void SendMessage(String message, int fac, int sev, String appname) {
            string sendMessage = createPRI(fac, sev);
            //sendMessage += createVERSION();
            //sendMessage += createSP();
            sendMessage += createTIMESTAMP();
            sendMessage += createSP();
            sendMessage += createHOSTNAME();
            sendMessage += createSP();
            sendMessage += createAPPNAME(appname);
            sendMessage += ":";
            //sendMessage += createPROCID();
            //sendMessage += createMSGID();
            sendMessage += createSP();
            //sendMessage += createSTRUCTDATA();
            //sendMessage += createSP();
            sendMessage += createMESSAGE(message);
            sendMessage += createNL();
            byte[] data = Encoding.ASCII.GetBytes(sendMessage);
            server.SendTo(data, data.Length, SocketFlags.None, RemoteEndPoint);
        }

        public void Stop() {
            // TODO Gracefully Shutdown?
        }

        private String createPRI(int Facility, int Severity) { return "<" + (Facility * 8 + Severity).ToString() + ">"; }
        private String createVERSION() { return "1"; }
        private String createSP() { return " "; }
        private String createTIMESTAMP() {
            DateTime dt = new DateTime();
            dt = DateTime.Now.ToUniversalTime();
            return dt.ToString("yyyy-MM-ddTHH:mm:ss.ffZ");
        }
        private String createHOSTNAME() { return scSyslogConfig.getHostname(); }
        private String createAPPNAME(String sAppname) {
            String sNewAppName = sAppname.Replace(" ", "").Replace("-", "");
            //if (sNewAppName.Length > 16) sNewAppName = sNewAppName.Substring(0, 16);
            return sNewAppName;
        }
        private String createPROCID() { return "[" + (Thread.CurrentThread.ManagedThreadId).ToString() + "]:"; } // FIXME Try to get a ID here from EventLog
        private String createMSGID() { return "0"; }  // FIXME.. something usefull here??
        private String createSTRUCTDATA() { return "-"; }  // FIXME.. This is fency... never seens something usefull here
        private String createMESSAGE(String message) { return message.Replace("\n", ""); }  // TODO Make this Thing work better
        private String createNL() { return "\n"; }
    }
}
