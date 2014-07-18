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
        private String sTagName;
        private String sBasePath;
        private bool bIsValid = false;
        private bool bDoComplete = false;
        // Constructor
        public FileConfig(XmlNode oFileConfigNode) {
            sFullName = ConfigMethods.getStringAttributeFromConfig(oFileConfigNode, "file", "filename", true, "");
            bDoComplete = ConfigMethods.getBoolAttributeFromConfig(oFileConfigNode, "file", "complete", false, false);
            sTagName = oFileConfigNode.InnerText.Trim().TrimEnd(System.Environment.NewLine.ToCharArray());
            // Check if the File Exists and also Save the Basepath for this
            FileInfo oFileInfoTempCheck = new FileInfo(sFullName);
            if (oFileInfoTempCheck.Exists) {
                bIsValid = true;
                sBasePath = oFileInfoTempCheck.DirectoryName;
                sFileName = oFileInfoTempCheck.Name;
            }
        }
        public static FileConfig getFileConfig(List<FileConfig> oFileConfigList,String sFileName) {
            foreach (FileConfig oFileConfig in oFileConfigList) {
                if (oFileConfig.getFullName().Equals(sFileName, StringComparison.OrdinalIgnoreCase)) return oFileConfig;
            }
            return null;
        }
        // Setters and Getters
        public bool isFileName(String sFilename) { return this.sFileName.Equals(sFileName, StringComparison.OrdinalIgnoreCase); }
        public bool isFullFile(String sFullName) { return this.sFullName.Equals(sFullName, StringComparison.OrdinalIgnoreCase); }
        public String getTag() { return sTagName; }
        public bool isValid() { return bIsValid; }
        public bool doComplete() { return bDoComplete; }
        public String getBasePath() { return sBasePath; }
        public String getFileName() { return sFileName; }
        public String getFullName() { return sFullName; }
    }
}
