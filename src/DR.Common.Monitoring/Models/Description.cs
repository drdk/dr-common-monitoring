using System;

namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// Description of a given HealthCheck.
    /// </summary>
    public class Description
    {
        /// <summary>
        /// Extra inline documentation for a given health check
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Optional link to external documentaion
        /// </summary>
        public Uri Link { get; set; }

        /// <summary>
        /// Serverity level of the given test if does not pass.
        /// </summary>
        public Level Level { get; set; }
    }

    public enum Level
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5
    }
}
