using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace nsCtSysLog {
    class DirConfig {
        private String sDirName;
        private String sFileFilter;
        private String sTagName;
        private bool bIsValid = false;
        // Constructor
        public DirConfig(XmlNode nlDirConfig) {
            sDirName = ConfigMethods.getStringAttributeFromConfig(nlDirConfig, "dir", "dirname", true, "");
            sFileFilter = ConfigMethods.getStringAttributeFromConfig(nlDirConfig, "dir", "filefilter", true, "");
            sTagName = nlDirConfig.InnerText.Trim().TrimEnd(System.Environment.NewLine.ToCharArray());
            // Check if the File Exists and also Save the Basepath for this
            DirectoryInfo diTempCheck = new DirectoryInfo(sDirName);
            if (diTempCheck.Exists) {
                bIsValid = true;
            }
        }
        public static DirConfig getDirConfig(List<DirConfig> lDirConfigs, String sFileName) {
            // Get the Dir of the File
            FileInfo oFileInfo = new FileInfo(sFileName);
            foreach (DirConfig dcTemp in lDirConfigs) {
                if (dcTemp.getDirName().Equals(oFileInfo.DirectoryName, StringComparison.OrdinalIgnoreCase)) {
                    if (doesFileNameMatchRegex(oFileInfo.Name, dcTemp.getFileFilter())) {
                        return dcTemp;
                    }      
                }
            }
            return null;
        }
        // Setters and Getters
        public bool isDirName(String sDirName) { return this.sDirName.Equals(sDirName, StringComparison.OrdinalIgnoreCase); }
        public bool isFileFilter(String sFileFilter) { return this.sFileFilter.Equals(sFileFilter, StringComparison.OrdinalIgnoreCase); }
        public String getTag() { return sTagName; }
        public bool isValid() { return bIsValid; }
        public String getFileFilter() { return sFileFilter; }
        public String getDirName() { return sDirName; }

        public static bool doesFileNameMatchRegex(String sFilename, String sRegex) {
            Match oMatch = Regex.Match(sFilename, sRegex, RegexOptions.IgnoreCase);
            return oMatch.Success;
        }
    }
}
