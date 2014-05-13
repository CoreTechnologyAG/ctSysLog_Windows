using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace nsCtSysLog {
    class FileConfig {
        private String sFullName;
        private String sFileName;
        private int iPrio;
        private int iFacility;
        private String sTagName;
        private String sBasePath;
        private bool bIsValid = false;
        private bool bDoComplete = false;
        // Constructor
        public FileConfig(XmlNode nlFileConfig) {
            sFullName = nlFileConfig.Attributes["filename"].Value;
            iPrio = int.Parse(nlFileConfig.Attributes["prio"].Value);
            iFacility = int.Parse(nlFileConfig.Attributes["facility"].Value);
            if (nlFileConfig.Attributes["complete"] != null) {
                bDoComplete = bool.Parse(nlFileConfig.Attributes["complete"].Value);
            }
            sTagName = nlFileConfig.InnerText;
            // Check if the File Exists and also Save the Basepath for this
            FileInfo fiTempCheck = new FileInfo(sFullName);
            if (fiTempCheck.Exists) {
                bIsValid = true;
                sBasePath = fiTempCheck.DirectoryName;
                sFileName = fiTempCheck.Name;
            }
        }
        public static FileConfig getFileConfig(List<FileConfig> lFileConfigs,String sFileName) {
            foreach (FileConfig fcTemp in lFileConfigs) {
                if (fcTemp.getFullName().Equals(sFileName, StringComparison.OrdinalIgnoreCase)) return fcTemp;
            }
            return null;
        }
        // Setters and Getters
        public bool isFileName(String sFilename) { return this.sFileName.Equals(sFileName, StringComparison.OrdinalIgnoreCase); }
        public bool isFullFile(String sFullName) { return this.sFullName.Equals(sFullName, StringComparison.OrdinalIgnoreCase); }
        public int getFacility() { return iFacility; }
        public int getPrio() { return iPrio;}
        public String getTag() { return sTagName; }
        public bool isValid() { return bIsValid; }
        public bool doComplete() { return bDoComplete; }
        public String getBasePath() { return sBasePath; }
        public String getFileName() { return sFileName; }
        public String getFullName() { return sFullName; }
    }
}
