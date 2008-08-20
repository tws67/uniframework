using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Uniframework.Services;
using System.Xml;

namespace Uniframework.Tests
{
    [TestFixture]
    public class XMLConfigurationServiceTest
    {
        XMLConfigurationService configService;

        [SetUp]
        public void Init()
        {
            configService = new XMLConfigurationService("Uniframework.config");
        }

        [Test]
        public void GetItemTest()
        {
            Assert.AreEqual(true, configService.Exists("System/Services/ObjectDatabaseService/"));

            XmlNode node = configService.GetItem("System/Services/ObjectDatabaseService/");
            Assert.IsNotNull(node);
        }

        [Test]
        public void XmlConfigurationTest()
        {
            IConfiguration config = new XMLConfiguration(configService.GetItem("System/Services/ObjectDatabaseService/"));
            Assert.AreEqual("~/App_Data", config.Attributes["dbpath"]);
        }

        [Test]
        public void GetChildrenTest()
        {
            XmlNodeList nodes = configService.GetChildren("/System/Services/");
            Assert.AreEqual(4, nodes.Count);
        }
    }
}
