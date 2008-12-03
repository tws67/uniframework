using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;
using System.IO;

namespace Uniframework.Tests
{
    [TestFixture]
    public class CompressorTest
    {
        private Compressor compressor;
        private BinaryFormatter bf;

        [SetUp]
        public void Init()
        {
            compressor = new Compressor();
            bf = new BinaryFormatter();
        }

        [Test]
        public void NetworkPackageTest()
        {
            NetworkInvokePackage pk1 = new NetworkInvokePackage(NetworkInvokeType.Invoke, "123");
            pk1.Context[PackageUtility.SESSION_USERNAME] = "Jacky";
            pk1.Context[PackageUtility.SESSION_PASSWORD] = "12345";

            using (MemoryStream stream = new MemoryStream()) {
                bf.Serialize(stream, pk1);
                byte[] buffer = stream.ToArray();

                Console.WriteLine("oraginal buffer length : " + buffer.Length);

                byte[] buf1 = compressor.Compress(buffer);

                Console.WriteLine("compress buffer length : " + buf1.Length);

                byte[] buf2 = compressor.Decompress(buf1);
                Assert.AreEqual(buffer.Length, buf2.Length);
                Console.WriteLine("decompress buffer length : " + buf2.Length);
            }
        }
    }
}
