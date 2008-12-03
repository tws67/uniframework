using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using NUnit.Framework;

namespace Uniframework.Tests
{
    [TestFixture]
    public class SerializerTest
    {
        private Serializer serializer;

        [SetUp]
        public void Init()
        {
            serializer = new Serializer();
        }

        [Test]
        public void SampleTest()
        {
            string abc = "abc";
            byte[] buffer = serializer.Serialize<string>(abc);

            Assert.AreEqual(abc, serializer.Deserialize<string>(buffer));

            MockContact contact = new MockContact() { Name = "Jacky",
                Phone = "22305779",
                Address = "DONGGUAN"
            };

            buffer = serializer.Serialize<MockContact>(contact);
            Assert.AreEqual(true, buffer.Length > 0);

            MockContact c2 = serializer.Deserialize<MockContact>(buffer);
            Assert.IsNotNull(c2);
            Assert.AreEqual("Jacky", c2.Name);
            Assert.AreEqual("22305779", c2.Phone);
        }

        [Test]
        public void NetWorkPackageTest()
        {
            NetworkInvokePackage pk1 = new NetworkInvokePackage(NetworkInvokeType.Invoke, "123");
            pk1.Context[PackageUtility.SESSION_USERNAME] = "Jacky";
            pk1.Context[PackageUtility.SESSION_PASSWORD] = "12345";
            pk1.Context[PackageUtility.SESSION_IPADDRESS] = "127.0.0.1";

            byte[] buffer = serializer.Serialize<NetworkInvokePackage>(pk1);
            Assert.AreEqual(true, buffer.Length > 0);
            Console.WriteLine(buffer.Length);

            NetworkInvokePackage pk2 = serializer.Deserialize<NetworkInvokePackage>(buffer);
            Assert.IsNotNull(pk2);
            Assert.AreEqual("123", pk2.SessionID);
            Assert.AreEqual("Jacky", (string)pk2.Context[PackageUtility.SESSION_USERNAME]);
            Assert.AreEqual("12345", (string)pk2.Context[PackageUtility.SESSION_PASSWORD]);
        }

        
        [Serializable]
        public class MockContact
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
        }
    }
}
