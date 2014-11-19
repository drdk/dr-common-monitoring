using System;

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
        public bool? Passed { get; private set; }

        /// <summary>
        /// How long check took to execute. May be null, if not measured.
        /// </summary>
        public TimeSpan? Duration { get; private set; }

        /// <summary>
        /// Optional status message
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// If the check failed, this property should contains any caught exceptions.
        /// </summary>
        public Exception Exception { get; private set; }
        
        /// <summary>
        /// Constructor for the Status object.
        /// </summary>
        /// <param name="passed">True for success, False for failure, null for unknown or undefined.</param>
        /// <param name="duration">Optional parameter defining how long the test-run took.</param>
        /// <param name="exception">Optional paramter defining any caught exceptions.</param>
        public Status(bool? passed = null, TimeSpan? duration = null, string message = null, Exception exception = null)
        {
            Passed = passed;
            Duration = duration;
            Message = message;
            Exception = exception;
        }
    }
}
