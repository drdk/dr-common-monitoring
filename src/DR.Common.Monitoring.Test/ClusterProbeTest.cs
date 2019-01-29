using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DR.Common.Monitoring.Contract;
using DR.Common.Monitoring.Models;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace DR.Common.Monitoring.Test
{
    public class ClusterProbeTest
    {

        private interface ICheckImpl
        {
            void RunTest(string nodeName, IStatusBuilder statusBuilder);
        }

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
            _sut.Protected().As<ICheckImpl>().Setup(x => x.RunTest(It.IsAny<string>(), It.IsNotNull<IStatusBuilder>())).Callback(
                (string nodeName, IStatusBuilder sBld) =>
                {
                    sBld.Passed = true;
                    sBld.MessageBuilder.AppendLine("hello");
                });
            var res = _sut.Object.GetStatus("Node1", true);
            Assert.IsTrue(res.Passed.GetValueOrDefault(false));
            Assert.AreEqual("hello\r\n", res.Message);
            Assert.IsNull(res.Payload);
            Assert.IsNull(res.Reactions);
            Assert.AreEqual(Level.Error, res.CurrentLevel);
            Assert.AreEqual(Level.Error, res.MaximumSeverityLevel);
            Assert.AreEqual("ClusterProbeCheck", res.Name);
            Assert.IsNull(res.DescriptionText);
            Assert.IsNull(res.DescriptionLink);
            Assert.IsNull(res.Exception);
            Assert.Greater(res.Duration.GetValueOrDefault(), TimeSpan.Zero);
            Assert.IsTrue(res.IncludedInScom);
            var combinedRes = _sut.Object.GetStatus(true);
            Assert.AreEqual("Node: \"Node1\":\r\nhello\r\n", combinedRes.Message);
        }
    }
}
