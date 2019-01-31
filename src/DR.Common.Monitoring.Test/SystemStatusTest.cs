using System;
using System.Collections.Generic;
using System.Linq;
using DR.Common.Monitoring.Contract;
using DR.Common.Monitoring.Models;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace DR.Common.Monitoring.Test
{
    public class SystemStatusTest
    {
        private SystemStatus _sut;
        private Mock<CommonHealthCheck> _healthCheckMock1;
        private Mock<CommonClusterProbe> _healthCheckMock2;

        [SetUp]
        public void Setup()
        {
            _healthCheckMock1 = new Mock<CommonHealthCheck>("SimpleHealthCheck") { CallBase = true };
            _healthCheckMock1.Protected().As<ICommonHealthCheck>()
                .Setup(x => x.RunTest(It.IsNotNull<StatusBuilder>()))
                .Callback((StatusBuilder sBld) => { });
            _healthCheckMock2 = new Mock<CommonClusterProbe>("ClusterProbeCheck") { CallBase = true };
            _healthCheckMock2.Protected().As<ICommonClusterProbe>()
                .Setup(x => x.RunTest(It.IsAny<string>(), It.IsNotNull<StatusBuilder>()))
                .Callback((string nodeName, StatusBuilder sBld) => { });
            _healthCheckMock2.SetupGet(x => x.NodeNames).Returns(new[] { "Node1" });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EmptyTest(bool isPrivileged)
        {
            Assert.DoesNotThrow(() => _sut = new SystemStatus(new IHealthCheck[] { }, isPrivileged));
            Assert.IsEmpty(_sut.Names);
            Assert.IsEmpty(_sut.RunAllChecks());
            Assert.Throws<KeyNotFoundException>(() => _sut.RunCheck("NotFoundCheck"));
            Assert.Throws<KeyNotFoundException>(() => _sut.RunProbeCheck("NotFoundCheck", "Node1"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TwoCheckTest(bool isPrivileged)
        {
            Assert.DoesNotThrow(() => 
                _sut = new SystemStatus(new IHealthCheck[] { _healthCheckMock1.Object, _healthCheckMock2.Object }, isPrivileged));
            Assert.That(_sut.Names, Is.EquivalentTo(new[] { "SimpleHealthCheck", "ClusterProbeCheck" }));
            var res = _sut.RunAllChecks();
            Assert.AreEqual(2, res.Count());
            Assert.NotNull(_sut.RunCheck("SimpleHealthCheck"));
            Assert.NotNull(_sut.RunCheck("ClusterProbeCheck"));
            Assert.NotNull(_sut.RunProbeCheck("ClusterProbeCheck", "Node1"));
            Assert.Throws<KeyNotFoundException>(() => _sut.RunCheck("NotFoundCheck"));
            Assert.Throws<KeyNotFoundException>(() => _sut.RunProbeCheck("ClusterProbeCheck", "Node2"));
            Assert.Throws<InvalidCastException>(() => _sut.RunProbeCheck("SimpleHealthCheck", "Node1"));
        }
    }
}