using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Uniframework.Services;

namespace Uniframework.Tests
{
    [TestFixture]
    public class XMLConfigurationTest
    {
        IConfigurationService configService;

        [SetUp]
        public void Init()
        {
            configService = new XMLConfigurationService("Uniframework.config");
        }

        [Test]
        public void XMLConfiguration_ctorTest()
        {
            Assert.AreEqual(true, configService.Exists("System/Services/SessionService"));

            XMLConfiguration config = new XMLConfiguration(configService.GetItem("System/Services/SessionService"));
            Console.WriteLine(config.ToString());

            Assert.IsNotNull(config);
        }
    }
}
