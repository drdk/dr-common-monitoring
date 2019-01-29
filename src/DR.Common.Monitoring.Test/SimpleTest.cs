using System;
using System.Linq.Expressions;
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
            bool? RunTest(ref string message, ref Level currentLevel, ref Reaction[] reactions, ref object payload);
        }

        private delegate bool? TestImpl(ref string message, ref Level currentLevel, ref Reaction[] reactions,
            ref object payload);

        private readonly Expression<Func<ICheckImpl, bool?>> _testExpression = x => x.RunTest(
            ref It.Ref<string>.IsAny,
            ref It.Ref<Level>.IsAny,
            ref It.Ref<Reaction[]>.IsAny,
            ref It.Ref<object>.IsAny);

        private Mock<CommonHealthCheck> _sut;

        [SetUp]
        public void Setup()
        {

            _sut = new Mock<CommonHealthCheck>("SimpleHealthCheck") {CallBase = true};
        }

        [Test]
        public void OneHealthCheckTest()
        {
            _sut.Protected().As<ICheckImpl>().Setup(_testExpression).Returns(new TestImpl(PassTest));
            var res = _sut.Object.GetStatus(true);
            Assert.IsTrue(res.Passed.GetValueOrDefault(false));
            Assert.AreEqual("hello", res.Message);
            Assert.IsNull(res.Payload);
            Assert.IsNull(res.Reactions);
            Assert.AreEqual(Level.Error, res.CurrentLevel);
            Assert.AreEqual(Level.Error, res.MaximumSeverityLevel);
            Assert.AreEqual("SimpleHealthCheck", res.Name);
            Assert.IsNull(res.DescriptionText);
            Assert.IsNull(res.DescriptionLink);
            Assert.IsNull(res.Exception);
            Assert.Greater(res.Duration.GetValueOrDefault(), TimeSpan.Zero);
        }

        private bool? PassTest(ref string msg, ref Level lvl, ref Reaction[] recs, ref object load)
        {
            msg = "hello";
            return true;
        }

        [Test]
        public void ExceptionTest()
        {
            _sut.Protected().As<ICheckImpl>().Setup(_testExpression).Returns(new TestImpl(ThrowException));
            var res = _sut.Object.GetStatus(true);
            Assert.IsFalse(res.Passed.GetValueOrDefault(true));
            Assert.NotNull(res.Exception);
            Assert.AreEqual("failed", res.Exception.Message);
        }

        private bool? ThrowException(ref string msg, ref Level lvl, ref Reaction[] recs, ref object load)
        {
            throw new Exception("failed");
        }
    }
}