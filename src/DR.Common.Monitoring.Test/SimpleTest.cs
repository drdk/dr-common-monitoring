using System;
using System.Linq;
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
            void RunTest(StatusBuilder statusBuilder);
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
            _sut.Protected().As<ICheckImpl>().Setup(x=>x.RunTest(It.IsNotNull<StatusBuilder>())).Callback(
                (StatusBuilder sBld) =>
                {
                    sBld.MessageBuilder.AppendLine("hello");
                    sBld.Passed = true;
                });
            var res = _sut.Object.GetStatus();
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
            _sut.Protected().As<ICheckImpl>().Setup(x => x.RunTest(It.IsNotNull<StatusBuilder>())).Callback(
                (StatusBuilder sBld) => throw new Exception("failed"));
            var res = _sut.Object.GetStatus();
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
            _sut.Protected().As<ICheckImpl>().Setup(x => x.RunTest(It.IsNotNull<StatusBuilder>())).Callback(
                (StatusBuilder sBld) =>
                {
                    sBld.MessageBuilder.AppendLine("fatal");
                    sBld.CurrentLevel = Level.Fatal;
                    sBld.Passed = false;
                });
            var res = _sut.Object.GetStatus();
            Assert.IsFalse(res.Passed.GetValueOrDefault(true));
            Assert.AreEqual(Level.Error, res.CurrentLevel);
            Assert.IsTrue(res.Message.Contains("Fatal"));

        }

        [Test]
        public void ReactionsTest()
        {
            _sut.Protected().As<ICheckImpl>().Setup(x => x.RunTest(It.IsNotNull<StatusBuilder>())).Callback(
                (StatusBuilder sBld) =>
                {
                   sBld.AddReaction(new Reaction{ Method = "GET", Payload = @"{""a"" : 1}" , Url ="http://google.com", VisualDescription = "hello1"});
                   sBld.AddReaction( new []
                   {
                       new Reaction { Method = "GET", Payload = @"{""a"" : 2}", Url = "http://google.com", VisualDescription = "hello2" },
                       new Reaction { Method = "GET", Payload = @"{""a"" : 3}", Url = "http://google.com", VisualDescription = "hello3" },
                   });
                });
            var res = _sut.Object.GetStatus();
            Assert.NotNull(res.Reactions);
            Assert.AreEqual(3, res.Reactions.Length);
            Assert.That(res.Reactions.Select(r=>r.VisualDescription), Is.EquivalentTo(new [] { "hello1","hello2","hello3"}));
            Assert.AreEqual("GET", res.Reactions.First().Method);
            Assert.AreEqual(@"{""a"" : 1}", res.Reactions.First().Payload);
            Assert.AreEqual("http://google.com", res.Reactions.First().Url);
        }
    }

}