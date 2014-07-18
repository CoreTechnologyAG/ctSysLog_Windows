using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace nsCtSysLog {
    class SyslogTCP : SyslogInterface{
        private TcpClient oTcpClient;
        private NetworkStream oNetworkStream;
        private String sHostname;
        private int iPort;

        public bool init(String sHostname, int iPort) {
            this.sHostname = sHostname;
            this.iPort = iPort;
            return connect();
        }
        public bool sendMessage(String sMessage) {
            if (oTcpClient == null || !oTcpClient.Connected) { // Not Connected Try to Reconnect !!
                shutdown();
                connect();
            }
            if (oTcpClient != null && oTcpClient.Connected) { // Still not connected.. Fail / Bail
                try {
                    byte[] oData = Encoding.ASCII.GetBytes(sMessage);
                    oNetworkStream.Write(oData, 0, oData.Length);
                    return true;
                } catch (Exception) { return false; }
            } else {
                return false;
            }
        }

        private bool connect() {
            try {
                oTcpClient = new TcpClient(sHostname, iPort);
                oNetworkStream = oTcpClient.GetStream();
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool shutdown() {
            try { oNetworkStream.Close(); } catch (Exception) { } oNetworkStream = null;
            try { oTcpClient.Close(); } catch (Exception) { } oTcpClient = null; 
            return true;
        }
    }
}
