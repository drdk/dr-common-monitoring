﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using DR.Common.Monitoring.Models;

#pragma warning disable 1591
// TODO: documentation 

namespace DR.Common.Monitoring.Web.Models
{
    // Extracted from DR.MediaUniverse.RestApi.Shared.Models
    /// <summary>
    /// Scom xml view model. 
    /// </summary>
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

        [XmlElement(ElementName = "applicationstatus")]
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

            [XmlElement(ElementName = "status")]
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
                        Status = status.CurrentLevel >= SeverityLevel.Error ? ScomStatus.ERROR : ScomStatus.WARNING;
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
                    var exMsg = "";
                    extractInnerExceptionInfo(status.Exception, ref exMsg);
                    Message += exMsg;
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

            if (status.Any(s => !s.Passed.GetValueOrDefault(false) && s.CurrentLevel > SeverityLevel.Warning))
            {
                ApplicationStatus = ScomStatus.ERROR;
            }
            else if (status.Any(s => !s.Passed.GetValueOrDefault(false) && s.CurrentLevel == SeverityLevel.Warning))
            {
                ApplicationStatus = ScomStatus.WARNING;
            }
            else
            {
                ApplicationStatus = ScomStatus.OK;
            }
            ServerIp = Ip;
            Checks = status.Select(s => new Check(s)).ToList();
        }

        /// <summary>
        /// Rewrites any WARNINGs to OKs
        /// </summary>
        public void HideWarnings()
        {
            if (ApplicationStatus == ScomStatus.WARNING)
                ApplicationStatus = ScomStatus.OK;

            foreach (var check in Checks)
            {
                if (check.Status == ScomStatus.WARNING)
                    check.Status = ScomStatus.OK;
            }
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

        /// <summary>
        /// Helper method used to extract exception message and stacktrace for up to 10 levels of inner exceptions.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="msg"></param>
        /// <param name="level">No more than 10 levels of inner exceptions.</param>
        private static void extractInnerExceptionInfo(Exception ex, ref string msg, int level = 0)
        {
            if (level > 10) // Don't go banananananas in recursiveness...
                return;

            if (ex != null)
            {
                if (msg == null)
                    msg = "";

                if (level > 0)
                    msg += "\n INNER EXCEPTION [" + level + "]";
                msg += "\n" + ex.GetType().Name + " : " + ex.Message + "\n" + ex.StackTrace + "\n";

                level++;
                if (ex.InnerException != null)
                    extractInnerExceptionInfo(ex.InnerException, ref msg, level);
            }
        }
    }
}
