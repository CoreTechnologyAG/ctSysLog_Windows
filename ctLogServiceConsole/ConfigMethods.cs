using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace nsCtSysLog {
    class ConfigMethods { 
        public static string getStringAttributeFromConfig(XmlNode oConfigNode, string sSektion, string sAttrname, bool bNeedToBeSet, string sDefault) {
            try {
                return oConfigNode.Attributes[sAttrname].Value.Trim().TrimEnd(System.Environment.NewLine.ToCharArray());
            } catch (Exception e) {
                handleExcepion(e, "Config Failure " + sSektion + " -> " + sAttrname + " not found!!", bNeedToBeSet);
            }
            return sDefault;
        }
        public static int getIntAttributeFromConfig(XmlNode oConfigNode, string sSektion, string sAttrname, bool bNeedToBeSet, int iDefault) {
            try {
                return int.Parse(oConfigNode.Attributes[sAttrname].Value.Trim().TrimEnd(System.Environment.NewLine.ToCharArray()));
            } catch (Exception e) {
                handleExcepion(e, "Config Failure " + sSektion + " -> " + sAttrname + " not found or not a number!!" , bNeedToBeSet);
            }
            return iDefault;
        }
        public static bool getBoolAttributeFromConfig(XmlNode oConfigNode, string sSektion, string sAttrname, bool bNeedToBeSet, bool bDefault) {
            try {
                return bool.Parse(oConfigNode.Attributes[sAttrname].Value.Trim().TrimEnd(System.Environment.NewLine.ToCharArray()));
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
