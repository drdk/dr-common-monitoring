using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using DR.Common.Monitoring.Models;

namespace DR.Common.Monitoring.Web.Models
{
    // Extracted from DR.MediaUniverse.RestApi.Shared.Models
    [XmlRoot(Namespace = "www.dr.dk/status", ElementName = "status")]
    public class Monitoring
    {
        [XmlElement(ElementName = "applicationstatus")]
        public string ApplicationStatus { get; set; }

        [XmlElement(ElementName = "applicationname")]
        public string ApplicationName { get; set; }

        [XmlElement(ElementName = "server-ip")]
        public string ServerIp { get; set; }

        [XmlElement(ElementName = "timestamp")]
        public DateTime TimeStamp { get; set; }

        [XmlElement(ElementName = "check")]
        public List<Check> Checks { get; set; }

        public class Check
        {
            [XmlElement(ElementName = "name")]
            public string Name { get; set; }

            [XmlElement(ElementName = "status")]
            public string Status { get; set; }
            [XmlElement(ElementName = "responseinms")]
            public double ResponseInMs { get; set; }
            [XmlElement(ElementName = "message")]
            public string Message { get; set; }

            public Check() { }
            public Check(string name, Status status)
            {
                Name = name;
                Status = (status.Passed.HasValue ? (status.Passed.Value ? "OK" : "ERROR") : "UNKNOWN");
                if (status.Duration.HasValue)
                    ResponseInMs = status.Duration.Value.TotalMilliseconds;

                if (Status == "ERROR")
                    Message = string.IsNullOrEmpty(status.Message) ? "No error message" : status.Message;
                else
                    Message = status.Message;

                if (status.Exception != null)
                {
                    Message += "\n" + status.Exception.GetType().Name + " : " + status.Exception.Message + "\n" +
                               status.Exception.StackTrace;
                }
            }
        }

        public Monitoring()
        {
            Checks = new List<Check>();
        }

        public Monitoring(IEnumerable<KeyValuePair<string, Status>> CheckNamesAndStatuses, DateTime timeStamp, string applicationName)
        {
            var status = CheckNamesAndStatuses.ToArray();
            TimeStamp = timeStamp;
            ApplicationName = applicationName;
            ApplicationStatus = (status.Any(x => !x.Value.Passed.GetValueOrDefault(true)) ? "ERROR" : "OK");
            ServerIp = Ip;
            Checks = status.Select(cs => new Check(cs.Key, cs.Value)).ToList();
        }
        public Monitoring(IEnumerable<KeyValuePair<string, Status>> CheckNamesAndStatuses, bool noFailures, DateTime timeStamp, string applicationName)
        {
            TimeStamp = timeStamp;
            ApplicationName = applicationName;
            ApplicationStatus = (noFailures ? "OK" : "ERROR");
            ServerIp = Ip;
            Checks = CheckNamesAndStatuses.Select(cs => new Check(cs.Key, cs.Value)).ToList();
        }

        private static string _ip;
        private static string Ip
        {
            get
            {
                if (!string.IsNullOrEmpty(_ip))
                    return _ip;
                var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                _ip = (from addr in hostEntry.AddressList
                       where addr.AddressFamily.ToString() == "InterNetwork"
                       select addr.ToString()).FirstOrDefault();
                return _ip;
            }
        }
    }
}
