using System;
using DR.Common.Monitoring.Contract;

namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// A base for health checks, includes execution timer and exception handling. 
    /// </summary>
    public abstract class CommonHealthCheck : IHealthCheck
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public SeverityLevel MaximumSeverityLevel { get; }

        /// <inheritdoc />
        public bool IncludedInScom { get; }

        /// <inheritdoc />
        public string DescriptionText { get; }

        /// <inheritdoc />
        public Uri DescriptionLink { get; }

        /// <inheritdoc />
        // ReSharper disable once RedundantOverload.Global // used by tests
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable RedundantArgumentDefaultValue
        protected CommonHealthCheck(string name) : this(name, SeverityLevel.Error, true, null, null)
        // ReSharper restore RedundantArgumentDefaultValue
        {

        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maximumSeverityLevel"></param>
        /// <param name="includeInScom"></param>
        /// <param name="descriptionText"></param>
        /// <param name="descriptionLink"></param>
        protected CommonHealthCheck(string name, SeverityLevel maximumSeverityLevel = SeverityLevel.Error, bool includeInScom = true, string descriptionText = null, Uri descriptionLink = null)
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
