using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Uniframework.Db4o;
using Uniframework.Security;
using Uniframework.Tests;

namespace Uniframework.Services.Test
{
    [TestFixture]
    public class AuthorizationStoreServiceTest
    {
        private IDb4oDatabaseService dbService;
        private IAuthorizationStoreService authService;

        [SetUp]
        public void Init()
        {
            MockRepository mocks = new MockRepository();
            authService = new AuthorizationStoreService(mocks.DynamicMock<Db4oDatabaseService>(), new MockLoggerFactory());
        }

        [Test]
        public void SaveCommandTest()
        {
            AuthorizationCommand cmd = new AuthorizationCommand() { 
                Name = "新建(&N)...",
                CommandUri = "/Shell/Module/Foundation/File/New",
                Category = "文件"
            };
            
            authService.SaveCommand(cmd);

            Assert.AreEqual(1, authService.GetCommands().Count);
            Assert.AreEqual("文件", authService.GetCommand("新建(&N)...", "/Shell/Module/Foundation/File/New").Category);
            authService.ClearCommand();
        }

        [Test]
        public void AuthorizationNodeTest()
        {

            AuthorizationNode node = new AuthorizationNode() { 
                Id = "Shell",
                Name = "系统权限",
                AuthorizationUri = "/Shell"
            };

            authService.Save(node);

            Assert.AreEqual(1, authService.GetAuthorizationNodes().Count);
            Assert.AreEqual("Shell", authService.GetAuthorizationNodes()[0].Name);
            authService.Clear();
        }
    }
}
