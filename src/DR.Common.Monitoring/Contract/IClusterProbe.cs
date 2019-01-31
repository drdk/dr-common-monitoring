using System.Collections.Generic;
using DR.Common.Monitoring.Models;

namespace DR.Common.Monitoring.Contract
{
    /// <summary>
    /// Special health check for clustered system. Used when monitor a collection of nodes. 
    /// </summary>
    public interface IClusterProbe : IHealthCheck
    {
        /// <summary>
        /// List of identifier of the the registered nodes. 
        /// </summary>
        IEnumerable<string> NodeNames { get; }

        /// <summary>
        /// Status for a single node.
        /// </summary>
        /// <param name="nodeName">Node identifier to run status test on.</param>
        /// <param name="isPrivileged">If false, exceptions will be removed.</param>
        /// <returns>Result of the test run.</returns>
        /// <seealso cref="Status"/>
        Status GetStatus(string nodeName, bool isPrivileged = true);
    }
}