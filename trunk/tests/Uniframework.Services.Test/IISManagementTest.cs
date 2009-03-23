using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Uniframework.Services.Test
{
    [TestFixture]
    public class IISManagementTest
    {
        [Test]
        public void IISManagementCtorTest()
        {
            IISManagement iisman = new IISManagement();
        }

        [Test]
        public void CreateWebServerTest()
        {
            IISManagement iisman = new IISManagement();
            IISWebServer webServer = new IISWebServer();
            webServer.ServerComment = "test";
            IISManagement.CreateIISWebServer(webServer);
            Assert.AreEqual(true, IISManagement.ExistsIISWebServer("test"));
        }
    }
}
