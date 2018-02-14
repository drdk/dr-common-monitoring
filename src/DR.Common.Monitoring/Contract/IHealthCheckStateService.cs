using System.Collections.Generic;
using System.Text;

namespace DR.Common.Monitoring.Contract
{
    public interface IHealthCheckStateService<T> where T: IHealthCheckState
    {
        void Register(T payload);
        T Retrieve(string healthCheckName);
    }
}
