using System;
using System.Collections.Generic;
using DR.Common.Monitoring.Contract;

namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// A base for health checks, includes execution timer and exception handling. 
    /// </summary>
    public abstract class CommonHealthCheck : IHealthCheck
    {
        public string Name { get; }

        public Level MaximumSeverityLevel { get; }

        public bool IncludedInScom { get; }

        public string DescriptionText { get; }

        public Uri DescriptionLink { get; }


        protected CommonHealthCheck(string name) : this(name, Level.Error, true, null, null)
        {

        }
        protected CommonHealthCheck(string name, Level maximumSeverityLevel = Level.Error, bool includeInScom = true, string descriptionText = null, Uri descriptionLink = null)
        {
            Name = name;
            MaximumSeverityLevel = maximumSeverityLevel;
            IncludedInScom = includeInScom;
            DescriptionText = descriptionText;
            DescriptionLink = descriptionLink;
        }

        /// <summary>
        /// Wraps call to protected method RunTest(). Handles exceptions and execution timer.
        /// </summary>
        /// <returns>Status object for RunTest()-call</returns>
        public Status GetStatus(bool isPrivileged = true)
        {
            var statusBuilder = new StatusBuilder(this, isPrivileged);
            try
            {
                RunTest(statusBuilder);
            }
            catch (Exception e)
            {
                statusBuilder.Passed = false;
                statusBuilder.Exception = e;
                HandleException(e, statusBuilder);
            }

            return statusBuilder.Status;

        }

        /// <summary>
        /// Optionally implemented by derived classes to handle exceptions thrown during GetStatus().
        /// </summary>
        protected virtual void HandleException(Exception ex, StatusBuilder statusBuilder) { }


        /// <summary>
        /// Must be implemented by derived classes. May throw exceptions.
        /// </summary>
        protected abstract void RunTest(StatusBuilder statusBuilder);
    }
}
