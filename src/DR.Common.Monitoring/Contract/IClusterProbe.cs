using System.Collections.Generic;
using DR.Common.Monitoring.Models;

namespace DR.Common.Monitoring.Contract
{
    public interface IClusterProbe : IHealthCheck
    {
        IEnumerable<string> NodeNames { get; }
        Status GetStatus(string nodeName);
    }
}