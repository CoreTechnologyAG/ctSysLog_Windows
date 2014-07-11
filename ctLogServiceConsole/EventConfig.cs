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
        public EventConfig(XmlNode nlFileConfig) {
            sEventLog = ConfigMethods.getStringAttributeFromConfig(nlFileConfig, "eventlog", "eventlog", true, "");
            iPrio = ConfigMethods.getIntAttributeFromConfig(nlFileConfig, "eventlog", "prio", false, 5);
            iFacility = ConfigMethods.getIntAttributeFromConfig(nlFileConfig, "eventlog", "facility", false, 5); 
            sEventTag = nlFileConfig.InnerText.Trim().TrimEnd(System.Environment.NewLine.ToCharArray());
        }
        public static EventConfig getEventConfigFor(List<EventConfig> lEventConfigs,String sEvent) {
            foreach (EventConfig ecTemp in lEventConfigs) { 
                if (ecTemp.getEventLog().Equals(sEvent,StringComparison.OrdinalIgnoreCase)) return ecTemp;
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
