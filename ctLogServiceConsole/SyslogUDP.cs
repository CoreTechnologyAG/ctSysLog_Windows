using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace nsCtSysLog {
    class SyslogUDP : SyslogInterface {
        private Socket oServer = null;
        private IPEndPoint oRemoteEndPoint;

        public bool init(String sIP, int iPort) {
            oRemoteEndPoint = new IPEndPoint(IPAddress.Parse(sIP), iPort);
            oServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            return true;
        }
        public bool sendMessage(String sMessage) {
            byte[] oaData = Encoding.ASCII.GetBytes(sMessage);
            oServer.SendTo(oaData, oaData.Length, SocketFlags.None, oRemoteEndPoint);
            return true;
        }

        public bool shutdown() {
            return true;
        }
    }
}
