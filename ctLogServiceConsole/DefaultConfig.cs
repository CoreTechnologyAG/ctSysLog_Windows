using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace nsCtSysLog {
    class DefaultConfig {
        private String sTag;
        // Constructor
        public DefaultConfig(XmlNode nlConfigFile) { sTag = nlConfigFile.InnerText.Trim().TrimEnd(System.Environment.NewLine.ToCharArray()); }
        // Setters and Getters
        public String getTag() { return sTag; }
    }
}
