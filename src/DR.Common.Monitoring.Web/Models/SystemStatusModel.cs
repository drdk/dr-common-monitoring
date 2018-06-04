using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using DR.Common.Monitoring.Models;

namespace DR.Common.Monitoring.Web.Models
{
    // Extracted from DR.MediaUniverse.RestApi.Web.Models
    public class SystemStatusModel
    {
        public bool NoFailures;
        public IEnumerable<CheckWithException> Checks;
        public DateTime TimeStamp;

        public class Check
        {
            public string Name { get; set; }

            public Description Description { get; set; }

            public bool? Passed { get; set; }

            [XmlIgnore]
            public TimeSpan? Duration { get; set; }

            [Browsable(false)]
            [XmlElement(DataType = "duration", ElementName = "Duration")]
            public string DurationString
            {
                get
                {
                    return Duration.HasValue ? XmlConvert.ToString(Duration.Value) : null;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                        Duration = XmlConvert.ToTimeSpan(value);
                    else
                        Duration = null;
                }
            }

            public string Message { get; set; }

            public Check(string name, Status status)
            {
                Name = name;
                Passed = status.Passed;
                Duration = status.Duration;
                Message = status.Message;
                Description = status.Description;
            }

            public Check()
            {
            }
        }

        public class CheckWithException : Check
        {
            public Exception Exception { get; set; }

            public CheckWithException(string name, Status status)
                : base(name, status)
            {
                Exception = status.Exception;
            }
        }
    }
}