using System;
using DR.Common.Monitoring.Models;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace DR.Common.Monitoring.Test
{
    public class ClusterProbeTest
    {

      

        private Mock<CommonClusterProbe> _sut;

        [SetUp]
        public void Setup()
        {

            _sut = new Mock<CommonClusterProbe>("ClusterProbeCheck") { CallBase = true };
            _sut.SetupGet(x => x.NodeNames).Returns(new [] {"Node1"});
        }

        [Test]
        public void OneHealthCheckTest()
        {
            _sut.Protected().As<ICommonClusterProbe>().Setup(x => x.RunTest(It.IsAny<string>(), It.IsNotNull<StatusBuilder>())).Callback(
                (string nodeName, StatusBuilder sBld) =>
                {
                    sBld.Passed = true;
                    sBld.MessageBuilder.AppendLine("hello");
                });
            var res = _sut.Object.GetStatus("Node1", true);
            Assert.IsTrue(res.Passed.GetValueOrDefault(false));
            Assert.AreEqual($"hello{Environment.NewLine}", res.Message);
            Assert.IsNull(res.Payload);
            Assert.IsNull(res.Reactions);
            Assert.AreEqual(SeverityLevel.Error, res.CurrentLevel);
            Assert.AreEqual(SeverityLevel.Error, res.MaximumSeverityLevel);
            Assert.AreEqual("ClusterProbeCheck", res.Name);
            Assert.IsNull(res.DescriptionText);
            Assert.IsNull(res.DescriptionLink);
            Assert.IsNull(res.Exception);
            Assert.Greater(res.Duration.GetValueOrDefault(), TimeSpan.Zero);
            Assert.IsTrue(res.IncludedInScom);
            var combinedRes = _sut.Object.GetStatus(true);
            Assert.AreEqual($"Node: \"Node1\":{Environment.NewLine}hello{Environment.NewLine}", combinedRes.Message);
        }


        [Test]
        public void ExceptionTest()
        {
            _sut.Protected().As<ICommonClusterProbe>().Setup(x => x.RunTest(It.IsAny<string>(), It.IsNotNull<StatusBuilder>())).Callback(
                (string nodeName, StatusBuilder sBld) => throw new Exception("failed"));
            var res = _sut.Object.GetStatus("Node1");
            Assert.IsFalse(res.Passed.GetValueOrDefault(true));
            Assert.NotNull(res.Exception);
            Assert.AreEqual("failed", res.Exception.Message);
            var unPrivilegedRes = _sut.Object.GetStatus(false);
            Assert.IsFalse(unPrivilegedRes.Passed.GetValueOrDefault(true));
            Assert.IsNull(unPrivilegedRes.Exception);
        }
    }
}
