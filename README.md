ctSysLog_Windows
================

Logging Facility for getting the Windows Logfiles and Events into Logstash

With this Utility coded in VS Studio 2012 you can send your Windows Events and Logfiles to a Logstash Server
You need to create a UDP or TCP Configuration to receive the Logentries from this Service

To Install this Service check INSTALL.txt under ./Setup

To the Logstash add something like this:

input {
	....
  udp {
    type => ctsyslog
    port => 6688
  }
  tcp  {
    type => ctsyslog
    port => 6689
  }
}
filter {
  if [type] == "ctsyslog" {
    grok {
      match => { "message" => "<%{POSINT:priority}>%{TIMESTAMP_ISO8601:syslog_timestamp} %{SYSLOGHOST:syslog_hostname} %{DATA:program}(?:\[%{POSINT:pid}\])?: %{DATA:type} %{GREEDYDATA:syslog_message}" }
      add_field => [ "logsource", "%{syslog_hostname}" ]
    }
    syslog_pri { }
    if !("_grokparsefailure" in [tags]) {
      mutate {
        replace => [ "message", "%{syslog_message}" ]
      }
    }
    mutate {
      remove_field => [ "syslog_hostname", "syslog_message", "syslog_timestamp" ]
    }
  }
}
output {
  ......
}

Check the config.xml to get an idea, who configuration may work.