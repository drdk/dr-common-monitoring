using System;
using DR.Common.Monitoring.Contract;

namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// IHealthCheck status object.
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Name of the origin check. 
        /// </summary>
        public string Name { get;  }

        /// <summary>
        /// The maximum value CurrentLevel can assume. 
        /// </summary>
        public SeverityLevel MaximumSeverityLevel { get; }

        /// <summary>
        /// Indicate whether a test is included in scom results
        /// </summary>
        public bool IncludedInScom { get; }

        /// <summary>
        /// Human readable (static) check description.
        /// </summary>
        public string DescriptionText { get; }

        /// <summary>
        /// Optional link to documentation for the check. 
        /// </summary>
        public Uri DescriptionLink { get; }

        /// <summary>
        /// The current level of the status, is always less than or equal to the MaximumSeverityLevel
        /// </summary>
        public SeverityLevel CurrentLevel { get; }

        /// <summary>
        /// This property is true if the check passed. If the check can neither fail or pass this property can be null.
        /// </summary>
        public bool? Passed { get; }

        /// <summary>
        /// How long check took to execute. May be null, if not measured.
        /// </summary>
        public TimeSpan? Duration { get; }

        /// <summary>
        /// Optional status message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// If the check failed, this property should contains any caught exceptions.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Optional list of Reactions
        /// </summary>
        /// <seealso cref="Reaction"/>
        public Reaction[] Reactions { get; }

        /// <summary>
        /// Optional payload, must be serializable to and from json.
        /// </summary>
        public object Payload { get; }

        /// <summary>
        /// Constructor for the Status object.
        /// </summary>
        /// <param name="checkSource">Health check source, used to read name, max level and description.</param>
        /// <param name="passed">True for success, False for failure, null for unknown or undefined.</param>
        /// <param name="currentLevel"></param>
        /// <param name="duration">Optional parameter defining how long the test-run took.</param>
        /// <param name="message">Optional message from the test run.</param>
        /// <param name="exception">Optional parameter defining any caught exceptions.</param>
        /// <param name="reactions">Optional reactions</param>
        /// <param name="payload">Optional data payload</param>
        public Status(IHealthCheck checkSource, bool? passed, SeverityLevel currentLevel, TimeSpan? duration, string message, Exception exception, Reaction[] reactions, object payload)
        {
            Name = checkSource.Name;
            MaximumSeverityLevel = checkSource.MaximumSeverityLevel;
            IncludedInScom = checkSource.IncludedInScom;
            DescriptionText = checkSource.DescriptionText;
            DescriptionLink = checkSource.DescriptionLink;
            CurrentLevel = currentLevel;
            Passed = passed;
            Duration = duration;
            Message = message;
            Exception = exception;
            Reactions = reactions;
            Payload = payload;
        }
    }
}
