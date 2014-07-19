using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace nsCtSysLog {
    class FileMon {
        private long lFilePosition = 0;
        private bool bAddAllOnStart = false;
        private String sFilename = null;
        private FileStream oFileContentStream = null;
        public FileMon(String sSetFilename) : this(sSetFilename, false) { ; }
        public FileMon(String sSetFilename, bool bAddAll) {
            sFilename = sSetFilename;
            bAddAllOnStart = bAddAll;
            if (!bAddAllOnStart) {
                FileInfo oFileInfo = new FileInfo(sFilename);
                lFilePosition = oFileInfo.Length;
            }
        }
        private List<string> getRemainingLinesFromFileStream(FileStream oFileContentStream) {
            int iReadByteCharAsInt = 0;
            List<string> oLineList= new List<string>();
            StringBuilder sbLine = new StringBuilder();
            while (oFileContentStream.Position < oFileContentStream.Length) {
                iReadByteCharAsInt = oFileContentStream.ReadByte();
                if (iReadByteCharAsInt != (byte)10 && iReadByteCharAsInt != (byte)13) {
                    sbLine.Append((char)iReadByteCharAsInt);
                } else if (iReadByteCharAsInt == (byte)13) {
                    // Ignore newLines part 2
                } else {
                    oLineList.Add(sbLine.ToString());
                    sbLine.Clear();
                }
            }
            lFilePosition = oFileContentStream.Position;
            return oLineList;
        }
        public List<String> getNewLines() {
            oFileContentStream = new FileStream(sFilename, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite);
            oFileContentStream.Seek(lFilePosition, SeekOrigin.Begin);
            List<String> oResultList = getRemainingLinesFromFileStream(oFileContentStream);
            oFileContentStream.Close();
            return oResultList;
        }
        public bool isFileName(String sFilename) {
            if (this.sFilename.Equals(sFilename,StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
        public static FileMon getFileMon(List<FileMon> oFileMonList, String sFileName, bool bDoComplete) {
            foreach (FileMon oFileMonTemp in oFileMonList) {
                if (oFileMonTemp.isFileName(sFileName)) return oFileMonTemp;
            }
            FileMon oFileMonNew = new FileMon(sFileName, bDoComplete);
            oFileMonList.Add(oFileMonNew);
            return oFileMonNew;
        }
    }
}
