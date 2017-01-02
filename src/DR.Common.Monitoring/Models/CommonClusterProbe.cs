using System;
using System.Collections.Generic;
using DR.Common.Monitoring.Contract;

namespace DR.Common.Monitoring.Models
{
    /// <summary>
    /// Base class for cluster probes.
    /// </summary>
    public abstract class CommonClusterProbe : CommonHealthCheck, IClusterProbe
    {
        /// <summary>
        /// Collection of registered nodes.
        /// </summary>
        public abstract IEnumerable<string> NodeNames { get; }

        /// <summary>
        /// Wraps call to protected method RunTest(nodeName). Handles exceptions and execution timer.
        /// </summary>
        /// <returns>Status object for RunTest(nodeName)-call</returns>
        public Status GetStatus(string nodeName)
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
                    passed = RunTest(nodeName, ref message);
                }
                catch (Exception e)
                {
                    passed = false;
                    exception = e;
                    message = nodeName;
                }
                finally
                {
                    Stopwatch.Stop();
                    result = new Status(passed: passed, duration: Stopwatch.Elapsed, message: message, exception: exception);
                }
                return result;
            }
        }

        /// <inheritdoc />
        protected abstract bool? RunTest(string node, ref string message);
    }
}
