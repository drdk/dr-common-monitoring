using DR.Common.Monitoring.Models;

namespace DR.Common.Monitoring.Test
{
    internal interface ICommonClusterProbe
    {
        void RunTest(string nodeName, StatusBuilder statusBuilder);
    }
}