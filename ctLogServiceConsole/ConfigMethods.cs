using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace nsCtSysLog {
    class ConfigMethods { // Bailout Stuff to Validate / Load the Parameters from the Config
        public static string getStringAttributeFromConfig(XmlNode nlConfig, string sSektion, string sAttrname, bool bNeedToBeSet, string sDefault) {
            try {
                return nlConfig.Attributes[sAttrname].Value;
            } catch (Exception e) {
                handleExcepion(e, "Config Failure " + sSektion + " -> " + sAttrname + " not found!!", bNeedToBeSet);
            }
            return sDefault;
        }
        public static int getIntAttributeFromConfig(XmlNode nlConfig, string sSektion, string sAttrname, bool bNeedToBeSet, int iDefault) {
            try {
                return int.Parse(nlConfig.Attributes[sAttrname].Value);
            } catch (Exception e) {
                handleExcepion(e, "Config Failure " + sSektion + " -> " + sAttrname + " not found or not a number!!" , bNeedToBeSet);
            }
            return iDefault;
        }
        public static bool getBoolAttributeFromConfig(XmlNode nlConfig, string sSektion, string sAttrname, bool bNeedToBeSet, bool bDefault) {
            try {
                return bool.Parse(nlConfig.Attributes[sAttrname].Value);
            } catch (Exception e) {
                handleExcepion(e, "Config Failure " + sSektion + " -> " + sAttrname + " not found or not a boolean!!", bNeedToBeSet);
            }
            return bDefault;
        }
        private static void handleExcepion(Exception e, string sMessage, bool bBailout) {
            if (bBailout) Console.WriteLine(e.ToString());
            Console.WriteLine(sMessage);
            if (bBailout) Environment.Exit(1);
        }
    }
}
