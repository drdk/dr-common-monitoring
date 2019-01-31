using DR.Common.Monitoring.Models;

namespace DR.Common.Monitoring.Test
{
    internal interface ICommonHealthCheck
    {
        void RunTest(StatusBuilder statusBuilder);
    }
}
