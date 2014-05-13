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
            sEventLog = nlFileConfig.Attributes["eventlog"].Value;
            iPrio = int.Parse(nlFileConfig.Attributes["prio"].Value);
            iFacility = int.Parse(nlFileConfig.Attributes["facility"].Value);
            sEventTag = nlFileConfig.InnerText;
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
