using System.Collections.Generic;

namespace DR.Common.Monitoring.Contract
{
    public interface IHealthCheckExtra
    {
        bool? RunTestWithDetails(ref string message, ref IEnumerable<dynamic> details, bool isPrivileged = false);

    }
}
