using System;
using System.Collections.Generic;

namespace DR.Common.Monitoring.Contract
{
    [Obsolete("Use IExtendedHealthCheck instead as it gets rid of reffing parameters in favour of a richer result type while being easier to extend.")]
    public interface IHealthCheckExtra
    {
        bool? RunTestWithDetails(ref string message, ref IEnumerable<dynamic> details, bool isPrivileged = false);
    }
}
