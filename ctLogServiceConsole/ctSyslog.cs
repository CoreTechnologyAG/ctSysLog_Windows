using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Xml;

namespace nsCtSysLog {
    public class ctSyslog {
        public static bool bDebug = true;
        private static String sVersion = "Version 0.7";
        private static Syslog oSyslog;
        private static List<FileMon> oFileMonList = new List<FileMon>();
        private static List<FileConfig> oFileConfigList = new List<FileConfig>();
        private static List<DirConfig> oDirConfigList = new List<DirConfig>();
        private static List<EventConfig> oEventConfigList = new List<EventConfig>();
        private static SyslogConfig oSyslogConfig;
        private static DefaultConfig oDefaultConfig;
        private List<FileSystemWatcher> oDirectoryWatcherList;
        private List<FileSystemWatcher> oFileWatcherList;
        private List<EventLog> oEventLogList;
        public ctSyslog(string sConfigFile) {

            // Read all the Configs from Our own Config
            XmlDocument xmldocConfiguration = new XmlDocument();
            xmldocConfiguration.Load(sConfigFile);

            // Get the Default Config ( Needed for Naming of the Service
            XmlNodeList oFileConfigNodeList = xmldocConfiguration.GetElementsByTagName("default");
            if (oFileConfigNodeList.Count != 1) return;
            oDefaultConfig = new DefaultConfig(oFileConfigNodeList.Item(0));

            // Get the SyslogConfig
            oFileConfigNodeList = xmldocConfiguration.GetElementsByTagName("syslogserver");
            if (oFileConfigNodeList.Count != 1) return;
            oSyslogConfig = new SyslogConfig(oFileConfigNodeList.Item(0));

            // Create The SyslogInterface to Send the Stuff to our Syslog Server
            oSyslog = new Syslog(oSyslogConfig);
            oSyslog.SendMessage("ctSyslog started", "ctsyslog", oSyslogConfig.getSyslogTag());
            oSyslog.SendMessage(sVersion, "ctsyslog", oSyslogConfig.getSyslogTag());

            // Parse all the Configs
            if (!parseFileConfig(xmldocConfiguration)) {
                oSyslog.SendMessage("Error while Loading File Configs", "ctsyslog", oSyslogConfig.getSyslogTag());
                return;
            }
            if (!parseEventLogConfig(xmldocConfiguration)) {
                oSyslog.SendMessage("Error while Loading Eventlog Configs", "ctsyslog", oSyslogConfig.getSyslogTag()); 
                return;
            }
            if (!parseDirConfig(xmldocConfiguration))  {
                oSyslog.SendMessage("Error while Loading Directory Configs", "ctsyslog", oSyslogConfig.getSyslogTag());
                return;
            }

            createEventLogHandlers();
            createFileMonHandlers();
            createDirMonHandlers();
        }

        private void createEventLogHandlers() {
            // Create the EventLog Array for Storing all needed Eventlog Change Handlers
            oEventLogList = new List<EventLog>();
            // Loop all the Eventlogs found in the Config, and created the EventLogHandlers
            foreach (EventConfig oEventConfigTemp in oEventConfigList) {
                EventLog myNewLog = new EventLog(oEventConfigTemp.getEventLog(), ".");
                // Add the Handlers
                myNewLog.EntryWritten += new EntryWrittenEventHandler(OnEventEntry);
                // Enable the Handler and Store the Object
                myNewLog.EnableRaisingEvents = true;
                oEventLogList.Add(myNewLog);
            }
        }

        private void createFileMonHandlers() {
            // Create the FileSystemWatchers
            oFileWatcherList = new List<FileSystemWatcher>();
            // Loop all the FilesystemWatcher Configurations and create the corresponding Objects
            foreach (FileConfig oFileConfigTemp in oFileConfigList) {
                // If config is good 
                if (oFileConfigTemp.isValid()) {
                    // Create a Filemon                      
                    FileSystemWatcher fsw = new FileSystemWatcher(oFileConfigTemp.getBasePath(), oFileConfigTemp.getFileName());
                    fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
                    // Add the Handlers
                    fsw.Changed += new FileSystemEventHandler(OnFileChanged);
                    fsw.Created += new FileSystemEventHandler(OnFileChanged);
                    // Enable the Handler and Store the Object
                    fsw.EnableRaisingEvents = true;
                    oFileWatcherList.Add(fsw);
                    if (oFileConfigTemp.doComplete()) 
                        FileChanged(oFileConfigTemp.getFullName(), oFileConfigTemp.doComplete(), oFileConfigTemp.getTag(), oFileConfigTemp.getTag());
                }
            }
        }

        private void createDirMonHandlers() {
            // Create the FileSystemWatchers
            oDirectoryWatcherList = new List<FileSystemWatcher>();
            // Loop all the FilesystemWatcher Configurations and create the corresponding Objects
            foreach (DirConfig oDirConfigTemp in oDirConfigList) {
                // If config is good 
                if (oDirConfigTemp.isValid()) {
                    // Create a Filemon                      
                    FileSystemWatcher oFileSystemWatcher = new FileSystemWatcher(oDirConfigTemp.getDirName());
                    oFileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName;
                    // Add the Handlers
                    oFileSystemWatcher.Changed += new FileSystemEventHandler(OnDirChanged);
                    oFileSystemWatcher.Created += new FileSystemEventHandler(OnDirChanged);
                    // Enable the Handler and Store the Object
                    oFileSystemWatcher.EnableRaisingEvents = true;
                    oDirectoryWatcherList.Add(oFileSystemWatcher);
                }
            }
        }

        private bool parseFileConfig(XmlDocument xmldocConfiguration) {
            try {
                XmlNodeList oFileConfigNodeList = xmldocConfiguration.GetElementsByTagName("file");
                for (int iCounter = 0; iCounter < oFileConfigNodeList.Count; iCounter++) {
                    FileConfig oFileConfigTemp = new FileConfig(oFileConfigNodeList.Item(iCounter));
                    if (!oFileConfigTemp.isValid()) {
                        oSyslog.SendMessage("ERROR: Invalid File Configuration - " + oFileConfigTemp.getFullName(), "ctsyslog", oSyslogConfig.getSyslogTag());
                    } else {
                        oFileConfigList.Add(oFileConfigTemp);
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }

        private bool parseDirConfig(XmlDocument xmldocConfiguration) {
            try {
                XmlNodeList oDirConfigNodeList = xmldocConfiguration.GetElementsByTagName("dir");
                for (int iCounter = 0; iCounter < oDirConfigNodeList.Count; iCounter++) {
                    DirConfig oDirConfigTemp = new DirConfig(oDirConfigNodeList.Item(iCounter));
                    if (!oDirConfigTemp.isValid()) {
                        oSyslog.SendMessage("ERROR: Invalid Dir Configuration - " + oDirConfigTemp.getDirName(), "ctsyslog", oSyslogConfig.getSyslogTag());
                    } else {
                        oDirConfigList.Add(oDirConfigTemp);
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }

        private bool parseEventLogConfig(XmlDocument xmldocConfiguration) {
            try {
                XmlNodeList oEventLogConfigNodeList = xmldocConfiguration.GetElementsByTagName("eventlog");
                for (int iCounter = 0; iCounter < oEventLogConfigNodeList.Count; iCounter++) {
                    EventConfig oEventConfigTemp = new EventConfig(oEventLogConfigNodeList.Item(iCounter));
                    if (bDebug) Console.WriteLine(oEventConfigTemp.getEventLog() + " " + oEventConfigTemp.getEventTag());
                    oEventConfigList.Add(oEventConfigTemp);
                }
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }

        public void Stop() {
            oSyslog.SendMessage("ctSyslog stopping", "ctsyslog", oSyslogConfig.getSyslogTag());
            oSyslog.Stop();
        }

        // Handle Events on Windows Eventlog Events
        private static void OnEventEntry(object oSource, EntryWrittenEventArgs oEventEntryArgs) {
            if (bDebug) Console.WriteLine(oEventEntryArgs.Entry.Message);
            // Get the Config for this Events
            String sSource = (string)oSource.GetType().GetProperty("Log").GetValue(oSource, null);
            EventConfig oEventConfigTemp = EventConfig.getEventConfigFor(oEventConfigList, sSource);
            int iFacility = oEventConfigTemp.getFacility();
            int iPrio = oEventConfigTemp.getPrio();
            // Send the Message to the Syslog-Server
            oSyslog.SendMessage(oEventEntryArgs.Entry.Message, oEventEntryArgs.Entry.Source, oEventConfigTemp.getEventTag());
        }

        // Handle Events if a File Changes. We have a FileWatcher for each Logfile we track
        private static void OnFileChanged(object oSource, FileSystemEventArgs oFileSystemEventArgs) {
            if (bDebug) Console.WriteLine("File {0} {1}", oFileSystemEventArgs.FullPath, oFileSystemEventArgs.ChangeType.ToString());
            // Get the FileConfig / FileMon we are talking about
            FileConfig oThisFileConfig = FileConfig.getFileConfig(oFileConfigList, oFileSystemEventArgs.FullPath);
            FileChanged(oThisFileConfig.getFullName(), oThisFileConfig.doComplete(), oThisFileConfig.getTag(), oThisFileConfig.getTag());
        }

        // Handle Events if a File in a Directory Changes. We have a FileWatcher for each Logfile we track
        // Also we try to find out, if this File which has changed is in our filter.
        // If it is, check if it matches the Last File changed
        // If not start Track this File
        private static void OnDirChanged(object oSource, FileSystemEventArgs oFileSystemEventArgs) {
            if (bDebug) Console.WriteLine("FileInDir {0} {1}", oFileSystemEventArgs.FullPath, oFileSystemEventArgs.ChangeType.ToString());
            // Get the FileConfig / FileMon we are talking about
            DirConfig oThisDirConfig = DirConfig.getDirConfig(oDirConfigList, oFileSystemEventArgs.FullPath);
            if (oThisDirConfig != null && oThisDirConfig.isValid()) FileChanged(oFileSystemEventArgs.FullPath, (WatcherChangeTypes.Created.Equals(oFileSystemEventArgs.ChangeType)), oThisDirConfig.getTag(), oThisDirConfig.getTag());
        }

        private static void FileChanged(String sFullName, bool bDoComplete, String sTag, String sType) {
            FileMon oThisFileMon = FileMon.getFileMon(oFileMonList, sFullName, bDoComplete);
            // Get all the Changes happened in this File
            List<string> saChangedLines = oThisFileMon.getNewLines();
            // For each Line create e Message for the Syslog Server
            foreach (String sStr in saChangedLines) {
                if (bDebug) Console.WriteLine(sStr);
                oSyslog.SendMessage(sStr,sTag,sType);
            }
        }
    }
}
