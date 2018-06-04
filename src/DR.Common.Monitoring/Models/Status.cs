using System;
using System.Collections.Generic;

namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// IHealthCheck status object.
    /// </summary>
    public class Status
    {
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
        /// Constructor for the Status object.
        /// </summary>
        /// <param name="passed">True for success, False for failure, null for unknown or undefined.</param>
        /// <param name="duration">Optional parameter defining how long the test-run took.</param>
        /// <param name="message">Optional message from the test run.</param>
        /// <param name="exception">Optional paramter defining any caught exceptions.</param>
        public Status(Description description, bool? passed = null, TimeSpan? duration = null, string message = null, Exception exception = null,
            IEnumerable<dynamic> details = null, IEnumerable<Reaction> reactions = null)
        {
            Description = description;
            Passed = passed;
            Duration = duration;
            Message = message;
            Exception = exception;
            Details = details;
            Reactions = reactions;
        }

        public IEnumerable<dynamic> Details { get; }
        
        public IEnumerable<Reaction> Reactions { get; }

        public Description Description { get; }
    }

}
