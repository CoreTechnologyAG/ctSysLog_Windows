using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace nsCtSysLog{
    class SyslogConfig {
        private string sSyslogIP;
        private string sSyslogTag;
        private string sHostname;
        private int iSyslogPort;
        private string sService;
        private string[] saMessageFormat;
        // Constructor
        public SyslogConfig(XmlNode nlConfigFile) {
            sSyslogIP = ConfigMethods.getStringAttributeFromConfig(nlConfigFile,"syslogconfig","ip", true,"");
            iSyslogPort = ConfigMethods.getIntAttributeFromConfig(nlConfigFile, "syslogconfig", "port", false, 514);
            sHostname = ConfigMethods.getStringAttributeFromConfig(nlConfigFile, "syslogconfig", "hostname", false, "noset"); 
            sService = ConfigMethods.getStringAttributeFromConfig(nlConfigFile, "syslogconfig", "service",false,"udp");
            saMessageFormat = ConfigMethods.getStringAttributeFromConfig(nlConfigFile, "syslogconfig", "format", false, "TIMESTAMP,SP,HOST,SP,SOURCE,SP,TYPE,SP,MSG").Split(',');
            sSyslogTag = nlConfigFile.InnerText;
        }
        // Setters and Getters
        public string getSyslogIP() { return sSyslogIP; }
        public string getSyslogTag() { return sSyslogTag; }
        public int getSyslogPort() { return iSyslogPort; }
        public string getHostname() { return sHostname;  }
        public string getService() { return sService;  }
        public string[] getMessageFormat() { return saMessageFormat; }

    }
}
