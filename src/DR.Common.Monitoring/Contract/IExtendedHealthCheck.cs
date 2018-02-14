using DR.Common.Monitoring.Models;

namespace DR.Common.Monitoring.Contract
{
    public interface IExtendedHealthCheck
    {
        TestRunResult RunTest(bool isPrivileged = false);
    }
}