using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace nsCtSysLog {
    class DirMon : FileMon{
       public DirMon(String sSetFilename) : base(sSetFilename, false) { ; }
       public DirMon(String sSetFilename, bool bAddAll) : base(sSetFilename, bAddAll) { ; }
    }
}
