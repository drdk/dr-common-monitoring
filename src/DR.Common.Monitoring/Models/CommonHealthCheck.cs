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


        internal readonly System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();

        protected CommonHealthCheck(string name) : this(name, Level.Error, true, null,null)
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
        public Status GetStatus(bool isPrivileged)
        {
            lock (Stopwatch)
            {
                bool? passed = null;
                Exception exception = null;
                string message = null;
                Reaction[] reactions = null;
                object payload = null;
                Level currentLevel = MaximumSeverityLevel;
                Status result;

                Stopwatch.Restart();
                try
                {
                    passed = RunTest(ref message, ref currentLevel, ref reactions, ref payload);
                }
                catch (Exception e)
                {
                    passed = false;
                    exception = e;
                    HandleException(e, ref message);
                }
                finally
                {
                    Stopwatch.Stop();
                    result = new Status(
                        checkSource: this,
                        passed: passed,
                        currentLevel: currentLevel,
                        duration: Stopwatch.Elapsed,
                        message: message,
                        exception: isPrivileged ? exception : null,
                        reactions: reactions,
                        payload: payload);
                }
                return result;
            }
        }

        /// <summary>
        /// Optionally implemented by derived classes to handle exceptions thrown during GetStatus().
        /// </summary>
        /// <param name="ex">Exception thrown in RunTest method</param>
        /// <param name="message">Status message, should append to string, if not null or empty</param>
        protected virtual void HandleException(Exception ex, ref string message) { }


        /// <summary>
        /// Must be implemented by derived classes. May throw exceptions.
        /// </summary>
        /// <param name="message">Human readable status message.</param>
        /// <param name="currentLevel">Optionally change current severity via parameter, defaults to maximum severity level.</param>
        /// <param name="reactions">Optional return reactions.</param>
        /// <param name="payload">Optional return custom data, should be serializable to json.</param>
        /// <returns>True of success and False for failure. Should throw exceptions if possible.</returns>
        protected abstract bool? RunTest(ref string message, ref Level currentLevel, ref Reaction[] reactions,
            ref object payload);
    }
}
