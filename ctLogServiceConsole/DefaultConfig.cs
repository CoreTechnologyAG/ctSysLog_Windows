using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace nsCtSysLog {
    class DefaultConfig {
        private int iPrio;
        private int iFacility;
        private String sTag;

        // Constructor
        public DefaultConfig(XmlNode nlConfigFile) {
            iFacility = int.Parse(nlConfigFile.Attributes["facility"].Value);
            iPrio = int.Parse(nlConfigFile.Attributes["prio"].Value);
            sTag = nlConfigFile.InnerText;
        }
        // Setters and Getters
        public int getFacility() { return iFacility; }
        public int getPrio() { return iPrio; }
        public String getTag() { return sTag; }
    }
}
