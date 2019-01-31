using System;

#pragma warning disable 1591
// TODO: Add documentation 

namespace DR.Common.Monitoring.Contract
{
    public interface IHealthCheckState
    {
        string HealthCheckName { get; set; }
        DateTimeOffset LastFlagged { get; }
        string AdditionalInfo { get; set; }
    }
}