using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using DR.Common.Monitoring.Contract;
using DR.Common.Monitoring.Models;
using DR.Common.Monitoring.Web.Models;
using ScomMonitoring =  DR.Common.Monitoring.Web.Models.Monitoring;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using Formatting = Newtonsoft.Json.Formatting;

namespace DR.Common.Monitoring.Web.Test
{
    public class ViewModelTest
    {
        private Mock<IHealthCheck> _mockCheck1;
        private Mock<IHealthCheck> _mockCheck2;
        private Mock<IHealthCheck> _mockCheck3;

        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(),
            }
        };

        [SetUp]
        public void Setup()
        {
            _mockCheck1 = new Mock<IHealthCheck>();
            _mockCheck1.SetupGet(m => m.Name).Returns("HealthCheck1");
            _mockCheck1.SetupGet(m => m.MaximumSeverityLevel).Returns(Level.Error);
            _mockCheck1.SetupGet(m => m.DescriptionText).Returns("Mock test 1");
            _mockCheck1.SetupGet(m => m.DescriptionLink).Returns(new Uri("http://google.com"));
            _mockCheck1.SetupGet(m => m.IncludedInScom).Returns(true);

            _mockCheck2 = new Mock<IHealthCheck>();
            _mockCheck2.SetupGet(m => m.Name).Returns("HealthCheck2");
            _mockCheck2.SetupGet(m => m.MaximumSeverityLevel).Returns(Level.Debug);
            _mockCheck2.SetupGet(m => m.DescriptionText).Returns("Mock test 2");
            _mockCheck2.SetupGet(m => m.IncludedInScom).Returns(false);

            _mockCheck3 = new Mock<IHealthCheck>();
            _mockCheck3.SetupGet(m => m.Name).Returns("HealthCheck3");
            _mockCheck3.SetupGet(m => m.MaximumSeverityLevel).Returns(Level.Warning);
            _mockCheck3.SetupGet(m => m.DescriptionText).Returns("Mock test 3");
            _mockCheck3.SetupGet(m => m.IncludedInScom).Returns(true);
        }

        private IEnumerable<Status> AllPass =>
            new[]
            {
                new Status(_mockCheck1.Object, true, Level.Error, TimeSpan.FromMilliseconds(5), "hello", null, null, null),
                new Status(_mockCheck2.Object, true, Level.Debug, TimeSpan.FromMilliseconds(5), "hello", null, null, null),
                new Status(_mockCheck3.Object, true, Level.Warning, TimeSpan.FromMilliseconds(5), "hello", null, null, null)
            };

        [Test]
        public void SystemStatusModelTest()
        {
            var timestamp = new DateTime(2001, 1, 1);
            var res = new SystemStatusModel(AllPass, timestamp);
            Assert.AreEqual(timestamp,res.TimeStamp);
            Assert.IsTrue(res.NoFailures);
            Assert.AreEqual(3, res.Checks.Length);
            var json = JsonConvert.SerializeObject(res, jsonSerializerSettings);
            Console.WriteLine(json);
            Assert.AreEqual(File.ReadAllText("AllPass.json"), json);
        }

        [TestCase(null)]
        [TestCase(true)]
        [TestCase(false)]
        public void PassedTest(bool? passed)
        {
            
            var s = new Status(_mockCheck1.Object, passed, Level.Error, TimeSpan.FromMilliseconds(5), "hello", null, null,
                null);
            var c = new SystemStatusModel.Check(s);
            Assert.AreEqual(passed,c.Passed);
            var ssm = new SystemStatusModel(new []{s});
            Assert.AreEqual(passed.GetValueOrDefault(true),ssm.NoFailures);
        }

        [Test]
        public void ReactionsTest()
        {
            var s = new Status(_mockCheck1.Object, false, Level.Error, TimeSpan.FromMilliseconds(5), "hello", null,
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
            var json = JsonConvert.SerializeObject(c, jsonSerializerSettings);
            Console.WriteLine(json);
            Assert.AreEqual(File.ReadAllText("Reaction.json"), json);
        }

        [Test]
        public void PayloadTest()
        {
            var s = new Status(_mockCheck1.Object, false, Level.Error, TimeSpan.FromMilliseconds(5), "hello", null,
                null,
                new dynamic[] { new { Foo = "bar", Count = 42 }, new { Foo = "tar", Count = 43 } });
            var c = new SystemStatusModel.Check(s);
            Assert.NotNull(c.Payload);
            var json = JsonConvert.SerializeObject(c, jsonSerializerSettings);
            Console.WriteLine(json);
            Assert.AreEqual(File.ReadAllText("Payload.json"), json);
        }

        [Test]
        public void MonitoringTest()
        {

            var timestamp = new DateTime(2001, 1, 1);
            var name = "UnitTest";
            var res = new ScomMonitoring(AllPass, timestamp, name);
            var ip = res.ServerIp;
            Assert.NotNull(ip);
            Assert.AreEqual(timestamp,res.TimeStamp);
            Assert.AreEqual(name,res.ApplicationName);
            Assert.AreEqual(ScomMonitoring.ScomStatus.OK, res.ApplicationStatus);
            Assert.AreEqual(2,res.Checks.Count);
            var xs = new XmlSerializer(typeof(ScomMonitoring));
           
            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw, new XmlWriterSettings{ Indent = true}))
                {
                    xs.Serialize(xw, res);
                    var xml = sw.ToString();
                    Console.WriteLine(xml);
                    Assert.AreEqual(File.ReadAllText("AllPass.xml").Replace("{{server-ip}}", ip), xml);
                }
            }
        }
    }
}
