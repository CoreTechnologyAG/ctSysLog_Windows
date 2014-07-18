using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace nsCtSysLog {
    class EventConfig {
        private String sEventTag;
        private String sEventLog;
        private int iFacility;
        private int iPrio;
        // Constructor
        public EventConfig(XmlNode oFileConfigNode) {
            sEventLog = ConfigMethods.getStringAttributeFromConfig(oFileConfigNode, "eventlog", "eventlog", true, "");
            iPrio = ConfigMethods.getIntAttributeFromConfig(oFileConfigNode, "eventlog", "prio", false, 5);
            iFacility = ConfigMethods.getIntAttributeFromConfig(oFileConfigNode, "eventlog", "facility", false, 5);
            sEventTag = oFileConfigNode.InnerText.Trim().TrimEnd(System.Environment.NewLine.ToCharArray());
        }
        public static EventConfig getEventConfigFor(List<EventConfig> oEventConfigList,String sEvent) {
            foreach (EventConfig oEventConfigTemp in oEventConfigList) {
                if (oEventConfigTemp.getEventLog().Equals(sEvent, StringComparison.OrdinalIgnoreCase)) return oEventConfigTemp;
            }
            return null;
        }
        // Setters and Getters
        public String getEventTag() { return sEventTag; }
        public String getEventLog() { return sEventLog; }
        public int getFacility() { return iFacility; }
        public int getPrio() { return iPrio; }
    }
}
