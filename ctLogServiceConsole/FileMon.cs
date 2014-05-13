using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace nsCtSysLog
{
    class FileMon {
        private long lFilePosition = 0;
        private bool bAddAllOnStart = false;
        private String sFilename = null;
        private FileStream fsFileContent = null;
        public FileMon(String sSetFilename) : this(sSetFilename, false) { ; }
        public FileMon(String sSetFilename, bool bAddAll) {
            sFilename = sSetFilename;
            bAddAllOnStart = bAddAll;
            if (!bAddAllOnStart) {
                FileInfo fiFile = new FileInfo(sFilename);
                lFilePosition = fiFile.Length;
            }
        }
        private List<string> getRemainingLinesFromFileStream(FileStream fsFileContent) {
            int bReadChar = 0;
            List<string> listLines= new List<string>();
            StringBuilder sbLine = new StringBuilder();
            while (fsFileContent.Position < fsFileContent.Length) {
                bReadChar = fsFileContent.ReadByte();
                if (bReadChar != (byte)10 && bReadChar != (byte)13) {
                    sbLine.Append((char)bReadChar);
                } else if (bReadChar == (byte)13) {
                    // Ignore newLines part 2
                } else {
                    listLines.Add(sbLine.ToString());
                    sbLine.Clear();
                }
            }
            lFilePosition = fsFileContent.Position;
            return listLines;
        }
        public List<String> getNewLines() {
            fsFileContent = new FileStream(sFilename, FileMode.Open, FileAccess.Read, FileShare.Delete|FileShare.ReadWrite);
            fsFileContent.Seek(lFilePosition,SeekOrigin.Begin);
            List<String> lResult = getRemainingLinesFromFileStream(fsFileContent);
            fsFileContent.Close();
            return lResult;
        }
        public bool isFileName(String sFilename) {
            if (this.sFilename.Equals(sFilename,StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
        public static FileMon getFileMon(List<FileMon> lFileMons, String sFileName, bool bDoComplete) {
            foreach (FileMon fmTemp in lFileMons) {
                if (fmTemp.isFileName(sFileName)) return fmTemp;
            }
            FileMon fmTempRes = new FileMon(sFileName,bDoComplete);
            lFileMons.Add(fmTempRes);
            return fmTempRes;
        }
    }
}
