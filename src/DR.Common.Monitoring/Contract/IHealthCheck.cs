using System;
using DR.Common.Monitoring.Models;

namespace DR.Common.Monitoring.Contract
{
    /// <summary> 
    /// Each area which need monitoring should implement a health check service.
    /// A health check can be test to see if a given service-layer can get valid data from a data layer.
    /// Each implementation of IHealthCheck needs to be registered in structure map bootstrapping (in Global.asax.cs for the web-project): 
    /// For each implementation add:
    ///  a.For&lt;DR.Common.Monitoring.Contract.IHealthCheck&gt;().Add&lt;DR.MediaUniverse.Service.Foo.FooServiceHealthCheck&gt;();
    /// The SystemStatus class collects every registered IHealthCheck.
    /// </summary>
    public interface IHealthCheck
    {
        /// <summary>
        /// Check name, should be descriptive to help debugging. Eg. name of the tested area. 
        /// </summary>
        string Name { get; }
        
        Level MaximumSeverityLevel { get; }
        bool IncludedInScom { get; }
        string DescriptionText { get; }
        Uri DescriptionLink { get; }

        /// <summary>
        /// Run the test. Must always catch any exceptions, and return them in the Status object.
        /// </summary>
        /// <returns>Result of the test run.</returns>
        /// <seealso cref="Status"/>
        Status GetStatus(bool isPrivileged = true);
    }
}