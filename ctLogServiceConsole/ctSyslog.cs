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
        public static bool debug = true;
        private static String sVersion = "Version 0.6";
        private static Syslog slObject;
        private static List<FileMon> fma = new List<FileMon>();
        private static List<FileConfig> lFileConfigs = new List<FileConfig>();
        private static List<DirConfig> lDirConfigs = new List<DirConfig>();
        private static List<EventConfig> lEventConfig = new List<EventConfig>();
        private static SyslogConfig slc;
        private static DefaultConfig dc;
        private List<FileSystemWatcher> lDSW;
        private List<FileSystemWatcher> lFSW;
        private List<EventLog> lEventLog;
        public ctSyslog(string sConfigFile) {

            // Read all the Configs from Our own Config
            XmlDocument xmldocConfiguration = new XmlDocument();
            xmldocConfiguration.Load(sConfigFile);

            // Get the Default Config ( Needed for Naming of the Service
            XmlNodeList nlFileconfigs = xmldocConfiguration.GetElementsByTagName("default");
            if (nlFileconfigs.Count != 1) return;
            dc = new DefaultConfig(nlFileconfigs.Item(0));

            // Get the SyslogConfig
            nlFileconfigs = xmldocConfiguration.GetElementsByTagName("syslogserver");
            if (nlFileconfigs.Count != 1) return;
            slc = new SyslogConfig(nlFileconfigs.Item(0));

            // Create The SyslogInterface to Send the Stuff to our Syslog Server
            slObject = new Syslog(slc);
            slObject.SendMessage("ctSyslog started", "ctsyslog", slc.getSyslogTag());
            slObject.SendMessage(sVersion, "ctsyslog", slc.getSyslogTag());

            // Parse all the Configs
            if (!parseFileConfig(xmldocConfiguration)) {
                slObject.SendMessage("Error while Loading File Configs", "ctsyslog", slc.getSyslogTag());
                return;
            }
            if (!parseEventLogConfig(xmldocConfiguration)) {
                slObject.SendMessage("Error while Loading Eventlog Configs", "ctsyslog", slc.getSyslogTag()); 
                return;
            }
            if (!parseDirConfig(xmldocConfiguration))  {
                slObject.SendMessage("Error while Loading Directory Configs", "ctsyslog", slc.getSyslogTag());
                return;
            }

            createEventLogHandlers();
            createFileMonHandlers();
            createDirMonHandlers();
        }

        private void createEventLogHandlers() {
            // Create the EventLog Array for Storing all needed Eventlog Change Handlers
            lEventLog = new List<EventLog>();
            // Loop all the Eventlogs found in the Config, and created the EventLogHandlers
            foreach (EventConfig ecTemp in lEventConfig) {
                EventLog myNewLog = new EventLog(ecTemp.getEventLog(), ".");
                // Add the Handlers
                myNewLog.EntryWritten += new EntryWrittenEventHandler(OnEventEntry);
                // Enable the Handler and Store the Object
                myNewLog.EnableRaisingEvents = true;
                lEventLog.Add(myNewLog);
            }
        }

        private void createFileMonHandlers() {
            // Create the FileSystemWatchers
            lFSW = new List<FileSystemWatcher>();
            // Loop all the FilesystemWatcher Configurations and create the corresponding Objects
            foreach (FileConfig fcTemp in lFileConfigs) {
                // If config is good 
                if (fcTemp.isValid()) {
                    // Create a Filemon                      
                    FileSystemWatcher fsw = new FileSystemWatcher(fcTemp.getBasePath(), fcTemp.getFileName());
                    fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
                    // Add the Handlers
                    fsw.Changed += new FileSystemEventHandler(OnFileChanged);
                    fsw.Created += new FileSystemEventHandler(OnFileChanged);
                    // Enable the Handler and Store the Object
                    fsw.EnableRaisingEvents = true;
                    lFSW.Add(fsw);
                    if (fcTemp.doComplete()) FileChanged(fcTemp.getFullName(),fcTemp.doComplete(),fcTemp.getTag(),fcTemp.getTag());
                }
            }
        }

        private void createDirMonHandlers() {
            // Create the FileSystemWatchers
            lDSW = new List<FileSystemWatcher>();
            // Loop all the FilesystemWatcher Configurations and create the corresponding Objects
            foreach (DirConfig fcTemp in lDirConfigs) {
                // If config is good 
                if (fcTemp.isValid()) {
                    // Create a Filemon                      
                    FileSystemWatcher fsw = new FileSystemWatcher(fcTemp.getDirName());
                    fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName;
                    // Add the Handlers
                    fsw.Changed += new FileSystemEventHandler(OnDirChanged);
                    fsw.Created += new FileSystemEventHandler(OnDirChanged);
                    // Enable the Handler and Store the Object
                    fsw.EnableRaisingEvents = true;
                    lDSW.Add(fsw);
                }
            }
        }

        private bool parseFileConfig(XmlDocument xmldocConfiguration) {
            try {
                XmlNodeList nlFileconfigs = xmldocConfiguration.GetElementsByTagName("file");
                for (int iCounter = 0; iCounter < nlFileconfigs.Count; iCounter++) {
                    FileConfig fcTemp = new FileConfig(nlFileconfigs.Item(iCounter));
                    if (!fcTemp.isValid()) {
                        slObject.SendMessage("ERROR: Invalid File Configuration - " + fcTemp.getFullName(), "ctsyslog", slc.getSyslogTag());
                    } else {
                        lFileConfigs.Add(fcTemp);
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
                XmlNodeList nlDirConfigs = xmldocConfiguration.GetElementsByTagName("dir");
                for (int iCounter = 0; iCounter < nlDirConfigs.Count; iCounter++) {
                    DirConfig dcTemp = new DirConfig(nlDirConfigs.Item(iCounter));
                    if (!dcTemp.isValid()) {
                        slObject.SendMessage("ERROR: Invalid Dir Configuration - " + dcTemp.getDirName(), "ctsyslog", slc.getSyslogTag());
                    } else {
                        lDirConfigs.Add(dcTemp);
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
                XmlNodeList nlEventLogConfigs = xmldocConfiguration.GetElementsByTagName("eventlog");
                for (int iCounter = 0; iCounter < nlEventLogConfigs.Count; iCounter++) {
                    EventConfig ecTemp = new EventConfig(nlEventLogConfigs.Item(iCounter));
                    if (debug) Console.WriteLine(ecTemp.getEventLog() + " " + ecTemp.getEventTag());
                    lEventConfig.Add(ecTemp);
                }
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }

        public void Stop() {
            slObject.SendMessage("ctSyslog stopping", "ctsyslog", slc.getSyslogTag());
            slObject.Stop();
        }

        // Handle Events on Windows Eventlog Events
        private static void OnEventEntry(object source, EntryWrittenEventArgs e) {
            if (debug) Console.WriteLine(e.Entry.Message);
            // Get the Config for this Events
            String sSource = (string)source.GetType().GetProperty("Log").GetValue(source, null);
            EventConfig ecTemp = EventConfig.getEventConfigFor(lEventConfig, sSource);
            int iFacility = ecTemp.getFacility();
            int iPrio = ecTemp.getPrio();
            // Send the Message to the Syslog-Server
            slObject.SendMessage(e.Entry.Message, e.Entry.Source, ecTemp.getEventTag());
        }

        // Handle Events if a File Changes. We have a FileWatcher for each Logfile we track
        private static void OnFileChanged(object source, FileSystemEventArgs e) {
            WatcherChangeTypes wct = e.ChangeType;
            if (debug) Console.WriteLine("File {0} {1}", e.FullPath, wct.ToString());
            // Get the FileConfig / FileMon we are talking about
            FileConfig fcThis = FileConfig.getFileConfig(lFileConfigs, e.FullPath);
            FileChanged(fcThis.getFullName(), fcThis.doComplete(), fcThis.getTag(), fcThis.getTag());
        }

        // Handle Events if a File in a Directory Changes. We have a FileWatcher for each Logfile we track
        // Also we try to find out, if this File which has changed is in our filter.
        // If it is, check if it matches the Last File changed
        // If not start Track this File
        private static void OnDirChanged(object source, FileSystemEventArgs e) {
            WatcherChangeTypes wct = e.ChangeType;
            if (debug) Console.WriteLine("FileInDir {0} {1}", e.FullPath, wct.ToString());
            // Get the FileConfig / FileMon we are talking about
            DirConfig dcThis = DirConfig.getDirConfig(lDirConfigs, e.FullPath);
            if (dcThis != null && dcThis.isValid()) FileChanged(e.FullPath, (WatcherChangeTypes.Created.Equals(wct)), dcThis.getTag(), dcThis.getTag());
        }

        private static void FileChanged(String sFullName, bool bDoComplete, String sTag, String sType) {
            FileMon fmThis = FileMon.getFileMon(fma, sFullName, bDoComplete);
            // FileMon fmThis = FileMon.getFileMon(fma, fcThis.getFullName(), fcThis.doComplete());
            // Get all the Changes happened in this File
            List<string> saChangedLines = fmThis.getNewLines();
            // For each Line create e Message for the Syslog Server
            foreach (String sStr in saChangedLines) {
                if (debug) Console.WriteLine(sStr);
                slObject.SendMessage(sStr,sTag,sType);
            }
        }
    }
}
