using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;
using Uniframework.Db4o;

namespace Uniframework.Services.Test
{
    [TestFixture]
    public class SessionServiceTest
    {
        MockRepository mocks;
        IDb4oDatabaseService db4oService;
        
        [SetUp]
        public void Init()
        {
            mocks = new MockRepository();
            db4oService = (IDb4oDatabaseService)mocks.StrictMock<IDb4oDatabaseService>(null);
            Expect.Call(db4oService.Open("Session")).Return(new Db4oDatabase("Session"));
            mocks.ReplayAll();
            mocks.VerifyAll();
        }

        [Test]
        public void GenericTest()
        {
            IList<int> list = mocks.StrictMock<IList<int>>();
            Assert.IsNotNull(list);
            Expect.Call(list.Count).Return(5);
            mocks.ReplayAll();
            Assert.AreEqual(5, list.Count);
            mocks.VerifyAll(); 
        }
    }
}
