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
    /// <summary>
    /// JSON view model for System Status
    /// </summary>
    public class SystemStatusModel
    {
        /// <summary>
        /// true if no child check was Passed set to false
        /// </summary>
        public bool NoFailures { get; set; }

        /// <summary>
        /// List of checks executed
        /// </summary>
        public Check[] Checks { get; set; }

        /// <summary>
        /// Time of the test execution.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="statuses">List of checks executed</param>
        /// <param name="timeStamp">Time of the test execution.</param>
        public SystemStatusModel(IEnumerable<Status> statuses, DateTime? timeStamp = null)
        {
            Checks = statuses.Select(s => new Check(s)).ToArray();
            TimeStamp = timeStamp.GetValueOrDefault(DateTime.UtcNow);
            NoFailures = Checks.All(c => c.Passed.GetValueOrDefault(true));
        }


        /// <inheritdoc />
        public SystemStatusModel() { }

        /// <summary>
        /// Static Check description
        /// </summary>
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
            public SeverityLevel Level { get; set; }

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="text"></param>
            /// <param name="link"></param>
            /// <param name="level"></param>
            public Description(string text, Uri link, SeverityLevel level)
            {
                Text = text;
                Link = link;
                Level = level;
            }

            /// <inheritdoc />
            public Description() { }
        }

        /// <summary>
        /// View model for Status
        /// </summary>
        public class Check
        {
            /// <summary>
            /// Name of the origin check. 
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Check description
            /// </summary>
            public Description Description { get; set; }

            /// <summary>
            /// This property is true if the check passed. If the check can neither fail or pass this property can be null.
            /// </summary>
            public bool? Passed { get; set; }

            /// <summary>
            /// The current level of the status, is always less than or equal to the MaximumSeverityLevel defined in Description.
            /// </summary>
            public SeverityLevel CurrentLevel { get; set; }

            /// <summary>
            /// If the check failed, this property should contains any caught exceptions.
            /// </summary>
            public Exception Exception { get; set; }

            /// <summary>
            /// Optional list of Reactions
            /// </summary>
            /// <seealso cref="Reaction"/>
            public IEnumerable<Reaction> Reactions { get; set; }

            /// <summary>
            /// Optional payload, must be serializable to and from json.
            /// </summary>
            public object Payload { get; set; }

            /// <summary>
            /// How long the check took to execute.
            /// </summary>
            [XmlIgnore]
            public TimeSpan? Duration { get; set; }


            /// <summary>
            /// XML formatted duration
            /// </summary>
            [Browsable(false)]
            [XmlElement(DataType = "duration", ElementName = "Duration")]
            public string DurationString
            {
                get => Duration.HasValue ? XmlConvert.ToString(Duration.Value) : null;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                        Duration = XmlConvert.ToTimeSpan(value);
                    else
                        Duration = null;
                }
            }

            /// <summary>
            /// Optional status message
            /// </summary>
            public string Message { get; set; }


            /// <inheritdoc />
            public Check() { }

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="status">Status to map from</param>
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