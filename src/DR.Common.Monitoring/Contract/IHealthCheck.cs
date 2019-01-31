using System;
using DR.Common.Monitoring.Models;

namespace DR.Common.Monitoring.Contract
{
    /// <summary> 
    /// Each area which need monitoring should implement a health check service.
    /// A health check can be test to see if a given service-layer can get valid data from a data layer.
    /// The SystemStatus class collects every registered IHealthCheck.
    /// </summary>
    public interface IHealthCheck
    {
        /// <summary>
        /// Check name, should be descriptive to help debugging. Eg. name of the tested area. 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The highest value a check can return in its Status object.
        /// </summary>
        SeverityLevel MaximumSeverityLevel { get; }

        /// <summary>
        /// Indicate whether a test is included in scom results
        /// </summary>
        bool IncludedInScom { get; }

        /// <summary>
        /// Human readable (static) check description.
        /// </summary>
        string DescriptionText { get; }

        /// <summary>
        /// Optional link to documentation for the check. 
        /// </summary>
        Uri DescriptionLink { get; }

        /// <summary>
        /// Run the test. Must always catch any exceptions, and return them in the Status object.
        /// </summary>
        /// <param name="isPrivileged">If false, exceptions will be removed.</param>
        /// <returns>Result of the test run.</returns>
        /// <seealso cref="Status"/>
        Status GetStatus(bool isPrivileged = true);
    }
}