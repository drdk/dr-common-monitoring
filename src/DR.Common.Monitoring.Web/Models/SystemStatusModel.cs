using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using DR.Common.Monitoring.Models;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace DR.Common.Monitoring.Web.Models
{
    public class SystemStatusModel
    {
        public bool NoFailures { get; set; }
        public Check[] Checks { get; set; }
        public DateTime TimeStamp { get; set; }

        public SystemStatusModel(IEnumerable<Status> statuses, DateTime? timeStamp = null)
        {
            Checks = statuses.Select(s => new Check(s)).ToArray();
            TimeStamp = timeStamp.GetValueOrDefault(DateTime.UtcNow);
            NoFailures = Checks.All(c => c.Passed.GetValueOrDefault(true));
        }
        public SystemStatusModel() { }

        public class Description
        {
            /// <summary>
            /// Extra inline documentation for a given health check
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Optional link to external documentation
            /// </summary>
            public Uri Link { get; set; }

            /// <summary>
            /// Maximum severity level of the given test if does not pass.
            /// </summary>
            public Level Level { get; set; }

            public Description(string text, Uri link, Level level)
            {
                Text = text;
                Link = link;
                Level = level;
            }

            public Description() { }
        }

        public class Check
        {
            public string Name { get; set; }

            public Description Description { get; set; }

            public bool? Passed { get; set; }

            public Level CurrentLevel { get; set; }

            public Exception Exception { get; set; }

            public IEnumerable<Reaction> Reactions { get; set; }

            public object Payload { get; set; }

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

            public Check(Status status)
            {
                Name = status.Name;
                Passed = status.Passed;
                CurrentLevel = status.CurrentLevel;
                Duration = status.Duration;
                Message = status.Message;
                Exception = status.Exception;
                Reactions = status.Reactions;
                Payload = status.Payload;
                Description = new Description(
                    status.DescriptionText,
                    status.DescriptionLink,
                    status.MaximumSeverityLevel);
            }
        }
    }
}