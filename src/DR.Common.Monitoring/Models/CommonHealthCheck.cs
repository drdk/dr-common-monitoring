using System;
using DR.Common.Monitoring.Contract;

namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// A base for health checks, includes exectution timer and exception handling. 
    /// </summary>
    public abstract class CommonHealthCheck : IHealthCheck
    {
        public abstract string Name { get; }

        internal readonly System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();
        
        /// <summary>
        /// Wraps call to protected method RunTest(). Handles exceptions and execution timer.
        /// </summary>
        /// <returns>Status object for RunTest()-call</returns>
        public Status GetStatus()
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
                    passed = RunTest(ref message);
                }
                catch (Exception e)
                {
                    passed = false;
                    exception = e;
                }
                finally
                {
                    Stopwatch.Stop();
                    result = new Status(passed: passed, duration: Stopwatch.Elapsed, message: message, exception: exception);
                }
                return result;
            }
        }

        /// <summary>
        /// Must be implemented by derived classes. May throw exceptions.
        /// </summary>
        /// <returns>True of success and False for failure. Should throw exceptions if possible.</returns>
        protected abstract bool? RunTest(ref string message);
    }
}
