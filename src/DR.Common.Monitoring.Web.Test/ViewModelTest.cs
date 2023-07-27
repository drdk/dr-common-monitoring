using DR.Common.Monitoring.Contract;
using DR.Common.Monitoring.Models;
using DR.Common.Monitoring.Web.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScomMonitoring = DR.Common.Monitoring.Web.Models.Monitoring;

namespace DR.Common.Monitoring.Web.Test
{
    public class ViewModelTest
    {
        private Mock<IHealthCheck> _mockCheck1;
        private Mock<IHealthCheck> _mockCheck2;
        private Mock<IHealthCheck> _mockCheck3;

        private const string Name = "UnitTest";

        [SetUp]
        public void Setup()
        {
            _mockCheck1 = new Mock<IHealthCheck>();
            _mockCheck1.SetupGet(m => m.Name).Returns("HealthCheck1");
            _mockCheck1.SetupGet(m => m.MaximumSeverityLevel).Returns(SeverityLevel.Error);
            _mockCheck1.SetupGet(m => m.DescriptionText).Returns("Mock test 1");
            _mockCheck1.SetupGet(m => m.DescriptionLink).Returns(new Uri("http://google.com"));
            _mockCheck1.SetupGet(m => m.IncludedInScom).Returns(true);

            _mockCheck2 = new Mock<IHealthCheck>();
            _mockCheck2.SetupGet(m => m.Name).Returns("HealthCheck2");
            _mockCheck2.SetupGet(m => m.MaximumSeverityLevel).Returns(SeverityLevel.Debug);
            _mockCheck2.SetupGet(m => m.DescriptionText).Returns("Mock test 2");
            _mockCheck2.SetupGet(m => m.IncludedInScom).Returns(false);

            _mockCheck3 = new Mock<IHealthCheck>();
            _mockCheck3.SetupGet(m => m.Name).Returns("HealthCheck3");
            _mockCheck3.SetupGet(m => m.MaximumSeverityLevel).Returns(SeverityLevel.Warning);
            _mockCheck3.SetupGet(m => m.DescriptionText).Returns("Mock test 3");
            _mockCheck3.SetupGet(m => m.IncludedInScom).Returns(true);
        }

        private IEnumerable<Status> AllPass =>
            new[]
            {
                new Status(_mockCheck1.Object, true, SeverityLevel.Error, TimeSpan.FromMilliseconds(5), "hello", null, null, null),
                new Status(_mockCheck2.Object, true, SeverityLevel.Debug, TimeSpan.FromMilliseconds(5), "hello", null, null, null),
                new Status(_mockCheck3.Object, true, SeverityLevel.Warning, TimeSpan.FromMilliseconds(5), "hello", null, null, null)
            };

        [Test]
        public void SystemStatusModelTest()
        {
            var timestamp = new DateTime(2001, 1, 1);
            var res = new SystemStatusModel(AllPass, timestamp);
            Assert.AreEqual(timestamp,res.TimeStamp);
            Assert.IsTrue(res.NoFailures);
            Assert.AreEqual(3, res.Checks.Length);
            var json = res.ToJson();
            Console.WriteLine(json);
            Assert.AreEqual(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AllPass.json")), json);
            var objFromJson = json.FromJsonTo<SystemStatusModel>();
            Assert.NotNull(objFromJson);
            Assert.AreEqual(json, objFromJson.ToJson());
        }

        [TestCase(null)]
        [TestCase(true)]
        [TestCase(false)]
        public void PassedTest(bool? passed)
        {
            
            var s = new Status(_mockCheck1.Object, passed, SeverityLevel.Error, TimeSpan.FromMilliseconds(5), "hello", null, null,
                null);
            var c = new SystemStatusModel.Check(s);
            Assert.AreEqual(passed,c.Passed);
            var ssm = new SystemStatusModel(new []{s});
            Assert.AreEqual(passed.GetValueOrDefault(true),ssm.NoFailures);
        }

        [Test]
        public void ReactionsTest()
        {
            var s = new Status(_mockCheck1.Object, false, SeverityLevel.Error, TimeSpan.FromMilliseconds(5), "hello", null,
                new[]
                {
                    new Reaction
                    {
                        Method = "GET", Payload = @"{""a"" : 1}", Url = "http://google.com",
                        VisualDescription = "hello1"
                    }
                },
                null);
            var c = new SystemStatusModel.Check(s);
            Assert.AreEqual(1, c.Reactions.Count());
            var json = c.ToJson();
            Console.WriteLine(json);
            Assert.AreEqual(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reaction.json")), json);
            var objFromJson = json.FromJsonTo<SystemStatusModel.Check>();
            Assert.NotNull(objFromJson);
            Assert.AreEqual(json,objFromJson.ToJson());
        }

        [Test]
        public void PayloadTest()
        {
            var s = new Status(_mockCheck1.Object, false, SeverityLevel.Error, TimeSpan.FromMilliseconds(5), "hello", null,
                null,
                new dynamic[] { new { Foo = "bar", Count = 42 }, new { Foo = "tar", Count = 43 } });
            var c = new SystemStatusModel.Check(s);
            Assert.NotNull(c.Payload);
            var json = c.ToJson();
            Console.WriteLine(json);
            Assert.AreEqual(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Payload.json")), json);
            var objFromJson = json.FromJsonTo<SystemStatusModel.Check>();
            Assert.NotNull(objFromJson);
            Assert.AreEqual(json, objFromJson.ToJson());
        }

        [Test]
        public void ExceptionTest()
        {
            var s = new Status(_mockCheck1.Object, false, SeverityLevel.Error, TimeSpan.FromMilliseconds(5), "hello", 
                new Exception("Failure"), 
                null, null);
            var c = new SystemStatusModel.Check(s);
            Assert.NotNull(c.Exception);
            var json = c.ToJson();
            Console.WriteLine(json);
            Assert.AreEqual(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exception.json")), json);
            var objFromJson = json.FromJsonTo<SystemStatusModel.Check>();
            Assert.NotNull(objFromJson);
            Assert.AreEqual(json, objFromJson.ToJson());
        }

        [Test]
        public void MonitoringTest()
        {
            var timestamp = new DateTime(2001, 1, 1);
            var res = new ScomMonitoring(AllPass, timestamp, Name);
            var ip = res.ServerIp;
            Assert.NotNull(ip);
            Assert.AreEqual(timestamp,res.TimeStamp);
            Assert.AreEqual(Name,res.ApplicationName);
            Assert.AreEqual(ScomMonitoring.ScomStatus.OK, res.ApplicationStatus);
            Assert.AreEqual(2,res.Checks.Count);
            var xml = res.ToXml();
            Console.WriteLine(xml);
            Assert.AreEqual(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AllPass.xml")).Replace("{{server-ip}}", ip), xml);
        }

        [Test]
        public void WarningRewriteTest()
        {
            var timestamp = new DateTime(2001, 1, 1);
            var s = new Status(_mockCheck3.Object, false, SeverityLevel.Warning, TimeSpan.FromMilliseconds(5), "hello",
                null, null, null);
            var res = new ScomMonitoring(new[] {s}, timestamp, Name);

            Assert.AreEqual(ScomMonitoring.ScomStatus.WARNING, res.ApplicationStatus);
            Assert.That(res.Checks.Select(c => c.Status), Is.All.EqualTo(ScomMonitoring.ScomStatus.WARNING));

            res.HideWarnings();

            Assert.AreEqual(ScomMonitoring.ScomStatus.OK, res.ApplicationStatus);
            Assert.That(res.Checks.Select(c => c.Status), Is.All.EqualTo(ScomMonitoring.ScomStatus.OK));


            s = new Status(_mockCheck1.Object, false, SeverityLevel.Error, TimeSpan.FromMilliseconds(5), "hello",
                null, null, null);

            res = new ScomMonitoring(new[] { s }, timestamp, Name);

            Assert.AreEqual(ScomMonitoring.ScomStatus.ERROR, res.ApplicationStatus);
            Assert.That(res.Checks.Select(c => c.Status), Is.All.EqualTo(ScomMonitoring.ScomStatus.ERROR));

            res.HideWarnings();

            Assert.AreEqual(ScomMonitoring.ScomStatus.ERROR, res.ApplicationStatus);
            Assert.That(res.Checks.Select(c => c.Status), Is.All.EqualTo(ScomMonitoring.ScomStatus.ERROR));

        }
    }
}
