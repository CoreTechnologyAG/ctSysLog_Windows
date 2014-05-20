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
        private static String sVersion = "Version 0.3 Beta";
        private static Syslog slObject;
        private static List<FileMon> fma = new List<FileMon>();
        private static List<FileConfig> lFileConfigs = new List<FileConfig>();
        private static List<EventConfig> lEventConfig = new List<EventConfig>();
        private static SyslogConfig slc;
        private static DefaultConfig dc;

        public ctSyslog(string sConfigFile) {
            // Read all the Configs from Our own Config
            XmlDocument xmldocConfiguration = new XmlDocument();
            xmldocConfiguration.Load(sConfigFile);
            // Get the SyslogConfig
            XmlNodeList nlFileconfigs = xmldocConfiguration.GetElementsByTagName("syslogserver");
            if (nlFileconfigs.Count != 1) return;
            slc = new SyslogConfig(nlFileconfigs.Item(0));
            nlFileconfigs = xmldocConfiguration.GetElementsByTagName("default");
            if (nlFileconfigs.Count != 1) return;
            dc = new DefaultConfig(nlFileconfigs.Item(0));
            // Create The Socket to Send the Stuff to our Syslog Server (via UDP so far)
            slObject = new Syslog(slc);
            slObject.SendMessage("ctSyslog started", dc.getFacility(), dc.getPrio(), "ctsyslog");
            slObject.SendMessage(sVersion, dc.getFacility(), dc.getPrio(), "ctsyslog");
            // Loop all the FileConfig Nodes.
            nlFileconfigs = xmldocConfiguration.GetElementsByTagName("file");
            for (int iCounter = 0; iCounter < nlFileconfigs.Count; iCounter++) {
                FileConfig fcTemp = new FileConfig(nlFileconfigs.Item(iCounter));
                //if (debug) Console.WriteLine("Valid " + fcTemp.isValid());
                if (!fcTemp.isValid()) slObject.SendMessage("ERROR: Invalid File Configuration - " + fcTemp.getFullName(), dc.getFacility(), dc.getPrio(), "ctsyslog");
                //if (debug) Console.WriteLine(fcTemp.getFileName() + " " + fcTemp.getTag());
                lFileConfigs.Add(fcTemp);
            }
            nlFileconfigs = xmldocConfiguration.GetElementsByTagName("eventlog");
            for (int iCounter = 0; iCounter < nlFileconfigs.Count; iCounter++) {
                EventConfig ecTemp = new EventConfig(nlFileconfigs.Item(iCounter));
                if (debug) Console.WriteLine(ecTemp.getEventLog() + " " + ecTemp.getEventTag());
                lEventConfig.Add(ecTemp);
            }
            // Create the EventLog Array for Storing all needed Eventlog Change Handlers
            List<EventLog> lEventLog = new List<EventLog>();
            // Loop all the Eventlogs found in the Config, and created the EventLogHandlers
            foreach (EventConfig ecTemp in lEventConfig) {
                EventLog myNewLog = new EventLog(ecTemp.getEventLog(), ".");
                // Add the Handlers
                myNewLog.EntryWritten += new EntryWrittenEventHandler(OnEventEntry);
                // Enable the Handler and Store the Object
                myNewLog.EnableRaisingEvents = true;
                lEventLog.Add(myNewLog);
            }
            // Create the FileSystemWatchers
            List<FileSystemWatcher> lFSW = new List<FileSystemWatcher>();
            // Loop all the FilesystemWatcher Configurations and create the corresponding Objects
            foreach (FileConfig fcTemp in lFileConfigs) {
                // If config is good 
                if (fcTemp.isValid()) {
                    // Create a Filemon                      
                    FileSystemWatcher fsw = new FileSystemWatcher(fcTemp.getBasePath(), fcTemp.getFileName());
                    fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
                    // Add the Handlers
                    fsw.Changed += new FileSystemEventHandler(OnChanged);
                    fsw.Created += new FileSystemEventHandler(OnChanged);
                    // Enable the Handler and Store the Object
                    fsw.EnableRaisingEvents = true;
                    lFSW.Add(fsw);
                    if (fcTemp.doComplete()) FileChanged(fcTemp);
                }
            }
        }
        public void Stop() {
            slObject.SendMessage("ctSyslog stopping", dc.getFacility(), dc.getPrio(), "ctsyslog");
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
            slObject.SendMessage(e.Entry.Message, iFacility, iPrio, e.Entry.Source);
        }
        // Handle Events if a File Changes. We have a FileWatcher for each Logfile we track
        private static void OnChanged(object source, FileSystemEventArgs e) {
            WatcherChangeTypes wct = e.ChangeType;
            if (debug) Console.WriteLine("File {0} {1}", e.FullPath, wct.ToString());
            // Get the FileConfig / FileMon we are talking about
            FileConfig fcThis = FileConfig.getFileConfig(lFileConfigs, e.FullPath);
            FileChanged(fcThis);
        }
        private static void FileChanged(FileConfig fcThis) {
            FileMon fmThis = FileMon.getFileMon(fma, fcThis.getFullName(), fcThis.doComplete());
            // Get all the Changes happened in this File
            List<string> saChangedLines = fmThis.getNewLines();
            // For each Line create e Message for the Syslog Server
            foreach (String sStr in saChangedLines) {
                if (debug) Console.WriteLine(sStr);
                slObject.SendMessage(sStr, fcThis.getFacility(), fcThis.getPrio(), fcThis.getTag());
            }
        }
    }
}
