using System;
using DR.Common.Monitoring.Contract;

namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// A base for health checks, includes exectution timer and exception handling. 
    /// </summary>
    public abstract class CommonHealthCheck : IHealthCheck
    {
        /// <inheritdoc />
        public abstract string Name { get; }

        internal readonly System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();
        
        /// <summary>
        /// Wraps call to protected method RunTest(). Handles exceptions and execution timer.
        /// </summary>
        /// <returns>Status object for RunTest()-call</returns>
        public Status GetStatus(bool isPrivileged = false)
        {
            lock (Stopwatch)
            {
                bool? passed = null;
                Exception exception = null;
                string message = null;
                Status result;
                Stopwatch.Restart();
                try
                {
                    passed = RunTest(ref message, isPrivileged);
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
                    result = new Status(passed: passed, duration: Stopwatch.Elapsed, message: message, exception: isPrivileged ? exception : null);
                }
                return result;
            }
        }

        /// <summary>
        /// Optionally implemented by derived classes to handle exceptions thrown during GetStatus().
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        protected virtual void HandleException(Exception ex, ref string message) { }

        /// <summary>
        /// Must be implemented by derived classes. May throw exceptions.
        /// </summary>
        /// <returns>True of success and False for failure. Should throw exceptions if possible.</returns>
        public abstract bool? RunTest(ref string message, bool isPrivileged = false);
    }
}
