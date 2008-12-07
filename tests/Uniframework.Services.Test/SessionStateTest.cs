using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Uniframework.Db4o;

namespace Uniframework.Services.Test
{
    [TestFixture]
    public class SessionStateTest
    {
        private IDb4oDatabase db;

        [SetUp]
        public void Init()
        {
            string dbfile = FileUtility.ConvertToFullPath(@"..\Data\test.yap");
            if (File.Exists(dbfile))
                File.Delete(dbfile);

            db = new Db4oDatabase(@"..\Data\test.yap");
        }

        [TearDown]
        public void Clean()
        {
            db.Close();
            System.Threading.Thread.Sleep(1000);
        }

        [Test]
        public void SessionStateStoreTest()
        {
            SessionState session = new SessionState("12345", 100);
            db.Store(session);

            IList<SessionState> list = db.Load<SessionState>(delegate(SessionState ss){
                return ss.SessionId == "12345";
            });
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("12345", list[0].SessionId);
        }
    }
}
