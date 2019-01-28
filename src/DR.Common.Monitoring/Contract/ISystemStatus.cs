using System;
using System.Collections.Generic;
using DR.Common.Monitoring.Models;

namespace DR.Common.Monitoring.Contract
{
    /// <summary>
    /// Global collection of every registered IHealthCheck. 
    /// </summary>
    public interface ISystemStatus
    {
        /// <summary>
        /// Runs every registred test.
        /// </summary>
        /// <returns>Returns a collection of pairs; the key is the check name and the value is the check status object.</returns>
        IEnumerable<Status> RunAllChecks();

        /// <summary>
        /// Run a specific check.
        /// </summary>
        /// <param name="name">Name of check to run</param>
        /// <returns>Status object the test run.</returns>
        /// <exception cref="KeyNotFoundException">If no check with called name is registred.</exception>
        Status RunCheck(string name);


        /// <summary>
        /// Run a specific cluster probe check.
        /// </summary>
        /// <param name="name">Name of check to run</param>
        /// <param name="node">Node identifier to query</param>
        /// <exception cref="KeyNotFoundException">If no check with called name is registred.</exception>
        /// <exception cref="InvalidCastException">If called check isn't a IClusterProbe.</exception>
        Status RunProbeCheck(string name, string node);

        /// <summary>
        /// List the names for registered checks.
        /// </summary>
        IEnumerable<string> Names { get; }
    }
}