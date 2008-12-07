using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using Rhino.Mocks;
using Uniframework.Db4o;
using Uniframework.Tests;
using System.Threading;

namespace Uniframework.Services.Test
{
    [TestFixture]
    public class SessionServiceTest
    {
        IDb4oDatabaseService db4oService;
        IConfigurationService configService;
        ISessionService service;
        
        [SetUp]
        public void Init()
        {
            db4oService = new Db4oDatabaseService();
            configService = new XMLConfigurationService("Uniframework.config");
            MockRepository mocks = new MockRepository();
            service = new SessionService(db4oService, configService, new MockLoggerFactory(), mocks.DynamicMock<IEventDispatcher>());
            mocks.ReplayAll();

        }

        [Test]
        public void SessionItemTest()
        {

            service.Register("12345", "Jacky", "127.0.0.1", "abc");
            service.Activate("12345");

            object key1 = new object();
            service.CurrentSession["key1"] = key1;
            Assert.AreEqual(service.CurrentSession["key1"], key1);

            object key2 = new object();
            service.CurrentSession["key2"] = key2;
            Assert.AreEqual(service.CurrentSession["key2"], key2);

            service.CurrentSession.Remove("key2");
            Assert.IsNull(service.CurrentSession["key2"]);

            service.CurrentSession.RemoveAll();
            Assert.IsNull(service.CurrentSession["key1"]);

            service.UnloadSession("12345");
        }

        [Test]
        public void SessionTimeOutTest()
        {
            service.Register("testkey", "username", "23423", "asdf");
            service.Activate("testkey");
            Thread.Sleep(1000);
            try
            {
                service.GetSession("testkey");
            }
            finally
            {
            }

            service.UnloadSession("testkey");
        }
    }
}
