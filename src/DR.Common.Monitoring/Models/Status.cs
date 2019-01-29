using System;
using DR.Common.Monitoring.Contract;

namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// IHealthCheck status object.
    /// </summary>
    public class Status
    {
        public string Name { get;  }

        public Level MaximumSeverityLevel { get; }

        public bool IncludedInScom { get; }

        public string DescriptionText { get; }

        public Uri DescriptionLink { get; }

        public Level CurrentLevel { get; }
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

        public Reaction[] Reactions { get; }

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
        public Status(IHealthCheck checkSource, bool? passed, Level currentLevel, TimeSpan? duration, string message, Exception exception, Reaction[] reactions, object payload)
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
