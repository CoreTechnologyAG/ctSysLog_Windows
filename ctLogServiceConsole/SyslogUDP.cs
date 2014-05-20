using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace nsCtSysLog {
    class SyslogUDP : SyslogInterface{
        private Socket server = null;
        private IPEndPoint RemoteEndPoint;

        public bool init(String sIP, int iPort) {
            RemoteEndPoint = new IPEndPoint(IPAddress.Parse(sIP), iPort);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            return true;
        }
        public bool sendMessage(String sMessage) {
            byte[] data = Encoding.ASCII.GetBytes(sMessage);
            server.SendTo(data, data.Length, SocketFlags.None, RemoteEndPoint);
            return true;
        }

        public bool shutdown() {
            return true;
        }

       
    }
}
