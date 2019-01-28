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

        public enum ScomStatus
        {
            UNKNOWN = 0,
            OK = 10,
            WARNING = 20,
            ERROR = 30
        }

        [XmlElement(ElementName = "applicationstatus", DataType = "string")]
        public ScomStatus ApplicationStatus { get; set; }

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

            [XmlElement(ElementName = "status", DataType = "string")]
            public ScomStatus Status { get; set; }
            [XmlElement(ElementName = "responseinms")]
            public double ResponseInMs { get; set; }
            [XmlElement(ElementName = "message")]
            public string Message { get; set; }

            public Check() { }
            public Check(Status status)
            {
                Name = status.Name;

                if (status.Passed.HasValue)
                {
                    if (status.Passed.Value)
                    {
                        Status = ScomStatus.OK;
                    }
                    else
                    {
                        Status = status.CurrentLevel >= Level.Error ? ScomStatus.ERROR : ScomStatus.WARNING;
                    }
                }
                else
                {
                    Status = ScomStatus.UNKNOWN;
                }

                if (status.Duration.HasValue)
                    ResponseInMs = status.Duration.Value.TotalMilliseconds;

                if (Status > ScomStatus.OK)
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

        public Monitoring(IEnumerable<Status> statuses, DateTime timeStamp, string applicationName)
        {
            var status = statuses.Where(s => s.IncludedInScom).ToArray();
            TimeStamp = timeStamp;
            ApplicationName = applicationName;

            if (status.Any(s => s.Passed.GetValueOrDefault(false) && s.CurrentLevel == Level.Warning))
            {
                ApplicationStatus = ScomStatus.WARNING;
            }
            else if (status.Any(s => s.Passed.GetValueOrDefault(false) && s.CurrentLevel > Level.Warning))
            {
                ApplicationStatus = ScomStatus.ERROR;
            }
            else
            {
                ApplicationStatus = ScomStatus.OK;
            }
            ServerIp = Ip;
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
