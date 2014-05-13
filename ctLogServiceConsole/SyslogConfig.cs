using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace nsCtSysLog {
    class SyslogConfig {
        private String sSyslogIP;
        private String sSyslogTag;
        private String sHostname;
        private int iSyslogPort;
        // Constructor
        public SyslogConfig(XmlNode nlConfigFile) {
            sSyslogIP = nlConfigFile.Attributes["ip"].Value;
            iSyslogPort = int.Parse(nlConfigFile.Attributes["port"].Value);
            sHostname = nlConfigFile.Attributes["hostname"].Value;
            sSyslogTag = nlConfigFile.InnerText;
        }
        // Setters and Getters
        public String getSyslogIP() { return sSyslogIP; }
        public String getSyslogTag() { return sSyslogTag; }
        public int getSyslogPort() { return iSyslogPort; }
        public String getHostname() { return sHostname;  }
    }
}
