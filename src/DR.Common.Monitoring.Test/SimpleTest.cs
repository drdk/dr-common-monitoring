using System;
using DR.Common.Monitoring.Contract;
using DR.Common.Monitoring.Models;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace DR.Common.Monitoring.Test
{
    public class SimpleTest
    {

        private interface ICheckImpl
        {
            void RunTest(IStatusBuilder statusBuilder);
        }

        private Mock<CommonHealthCheck> _sut;

        [SetUp]
        public void Setup()
        {

            _sut = new Mock<CommonHealthCheck>("SimpleHealthCheck") {CallBase = true};
        }

        [Test]
        public void OneHealthCheckTest()
        {
            _sut.Protected().As<ICheckImpl>().Setup(x=>x.RunTest(It.IsNotNull<IStatusBuilder>())).Callback(
                (IStatusBuilder sBld) =>
                {
                    sBld.MessageBuilder.AppendLine("hello");
                    sBld.Passed = true;
                });
            var res = _sut.Object.GetStatus(true);
            Assert.IsTrue(res.Passed.GetValueOrDefault(false));
            Assert.AreEqual("hello\r\n", res.Message);
            Assert.IsNull(res.Payload);
            Assert.IsNull(res.Reactions);
            Assert.AreEqual(Level.Error, res.CurrentLevel);
            Assert.AreEqual(Level.Error, res.MaximumSeverityLevel);
            Assert.AreEqual("SimpleHealthCheck", res.Name);
            Assert.IsNull(res.DescriptionText);
            Assert.IsNull(res.DescriptionLink);
            Assert.IsNull(res.Exception);
            Assert.Greater(res.Duration.GetValueOrDefault(), TimeSpan.Zero);
            Assert.IsTrue(res.IncludedInScom);
        }


        [Test]
        public void ExceptionTest()
        {
            _sut.Protected().As<ICheckImpl>().Setup(x => x.RunTest(It.IsNotNull<IStatusBuilder>())).Callback(
                (IStatusBuilder sBld) => throw new Exception("failed"));
            var res = _sut.Object.GetStatus(true);
            Assert.IsFalse(res.Passed.GetValueOrDefault(true));
            Assert.NotNull(res.Exception);
            Assert.AreEqual("failed", res.Exception.Message);
            var unPrivilegedRes = _sut.Object.GetStatus(false);
            Assert.IsFalse(unPrivilegedRes.Passed.GetValueOrDefault(true));
            Assert.IsNull(unPrivilegedRes.Exception);
        }


        [Test]
        public void ExceedMaximumLevelTest()
        {
            _sut.Protected().As<ICheckImpl>().Setup(x => x.RunTest(It.IsNotNull<IStatusBuilder>())).Callback(
                (IStatusBuilder sBld) =>
                {
                    sBld.MessageBuilder.AppendLine("fatal");
                    sBld.CurrentLevel = Level.Fatal;
                    sBld.Passed = false;
                });
            var res = _sut.Object.GetStatus(true);
            Assert.IsFalse(res.Passed.GetValueOrDefault(true));
            Assert.AreEqual(Level.Error, res.CurrentLevel);
            Assert.IsTrue(res.Message.Contains("Fatal"));

        }
    }
}