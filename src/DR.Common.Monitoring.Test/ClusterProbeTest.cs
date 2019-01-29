using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            bool? RunTest(string nodeName, ref string message, ref Level currentLevel, ref Reaction[] reactions, ref object payload);
        }

        private delegate bool? TestImpl(string nodeName, ref string message, ref Level currentLevel, ref Reaction[] reactions,
            ref object payload);

        private readonly Expression<Func<ICheckImpl, bool?>> _testExpression = x => x.RunTest(
            It.IsAny<string>(),
            ref It.Ref<string>.IsAny,
            ref It.Ref<Level>.IsAny,
            ref It.Ref<Reaction[]>.IsAny,
            ref It.Ref<object>.IsAny);

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
            _sut.Protected().As<ICheckImpl>().Setup(_testExpression).Returns(new TestImpl(PassTest));
            var res = _sut.Object.GetStatus("Node1", true);
            Assert.IsTrue(res.Passed.GetValueOrDefault(false));
            Assert.AreEqual("hello", res.Message);
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
            Assert.AreEqual("Node: \"Node1\" :\nhello", combinedRes.Message);
        }

        private bool? PassTest(string nodeName, ref string msg, ref Level lvl, ref Reaction[] recs, ref object load)
        {
            msg = "hello";
            return true;
        }
    }
}
