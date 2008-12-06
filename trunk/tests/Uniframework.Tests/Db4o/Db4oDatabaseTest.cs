using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using NUnit.Framework;
using Uniframework.Db4o;

namespace Uniframework.Tests.Db4o
{
    [TestFixture]
    public class Db4oDatabaseTest
    {
        private string dbfile1;
        private string dbfile2;
        IObjectContainer container;
        
        [SetUp]
        public void SetUp()
        {
            if(!Directory.Exists(FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Data\"))
                Directory.CreateDirectory(FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Data\");

            dbfile1 = FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Data\temp1.yap";
            dbfile2 = FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Data\temp2.yap";

            container = Db4oFactory.OpenFile(dbfile1);
        }

        [TearDown]
        public void TearDown()
        {
            container.Close();

            if(File.Exists(dbfile1))
                File.Delete(dbfile1);
            if (File.Exists(dbfile2))
                File.Delete(dbfile2);

            if (Directory.Exists(FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Data\"))
                Directory.Delete(FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Data\");
        }

        [Test]
        [ExpectedException(typeof(Db4objects.Db4o.Ext.DatabaseFileLockedException))]
        public void DBFileLockTest()
        {
            using (IObjectContainer container1 = Db4oFactory.OpenFile(dbfile1)) {
                using (IObjectContainer container2 = Db4oFactory.OpenFile(dbfile1)) {
                    container1.Store(new Random().Next());
                    container2.Store(new Random().Next());
                }
            }
        }

        [Test]
        public void Db4oDatabase_ctorTest()
        { 
            using (Db4oDatabase db = new Db4oDatabase(dbfile1, container)){
                Assert.AreEqual(true, File.Exists(dbfile1));
            }
        }

        [Test]
        public void StoreTest()
        {
            using (Db4oDatabase db = new Db4oDatabase(dbfile1, container)) {
                MockObject obj1 = new MockObject
                {
                    Name = "Jacky",
                    Phone = "22305779",
                    Address = new Address { 
                        Office = "ZDH",
                        Home = "BBG"
                    }
                };

                db.Store(obj1);

                Assert.IsNotNull(db.Load<MockObject>()[0]);
                Assert.IsNotNull(db.Load<Address>()[0]);

                IList<MockObject> list = db.Load<MockObject>(delegate(MockObject obj) {
                    return obj.Name == "Jacky";
                });
                Assert.AreEqual("Jacky", list[0].Name);
                Assert.AreEqual("22305779", list[0].Phone);
                Assert.IsNotNull(list[0].Address);
                Console.WriteLine(list[0]);

                MockObject obj2 = list[0];
                obj2.Phone = "22228057";
                db.Store(obj2);
                Assert.AreEqual(1, db.Load<MockObject>(delegate(MockObject obj) {
                    return obj.Name == "Jacky";
                }).Count);
                Console.WriteLine(obj2);

                MockObject obj3 = new MockObject
                {
                    Name = "Jacky",
                    Phone = "22305779",
                    Address = new Address
                    {
                        Office = "ZDH",
                        Home = "BBG"
                    }
                };
                db.Store(obj3);
                list.Clear();
                list = db.Load<MockObject>(delegate(MockObject obj) {
                    return obj.Name == "Jacky";
                });
                Assert.AreEqual(2, list.Count);
                foreach (MockObject obj in list) {
                    Console.WriteLine(obj);
                }
            }
        }

        [Test]
        public void StoreIListTest()
        {
            using (Db4oDatabase db = new Db4oDatabase(dbfile1, container)) {
                IList<Address> list = new List<Address> { 
                    new Address { Office = "ZDH", Home = "BBG" },
                    new Address { Office = "DHZ", Home = "GBB" }
                };

                db.Store(list);
                Assert.AreEqual(2, db.Load<Address>().Count);

                IList<Address> list2 = new List<Address> { 
                    new Address { Office = "ZDH1", Home = "BBG" }, 
                    new Address { Office = "ZDH2", Home = "BBG" }, 
                    new Address { Office = "ZDH3", Home = "BBG" }, 
                    new Address { Office = "ZDH4", Home = "BBG" }
                };
                db.Store(list2);
                Assert.AreEqual(6, db.Load<Address>().Count);

                IList<Address> list3 = db.Load<Address>(delegate(Address addr) {
                    return addr.Home == "BBG";
                });
                Assert.AreEqual(5, list3.Count);
            }
        }

        [Test]
        public void DeleteTest()
        {
            using (Db4oDatabase db = new Db4oDatabase(dbfile1, container)) {
                MockObject obj1 = new MockObject
                {
                    Name = "Jacky",
                    Phone = "22305779",
                    Address = new Address
                    {
                        Office = "ZDH",
                        Home = "BBG"
                    }
                };

                db.Store(obj1);
                Assert.AreEqual(1, db.Load<MockObject>(delegate(MockObject obj) {
                    return obj.Name == "Jacky";
                }).Count);

                MockObject obj2 = db.Load<MockObject>(delegate(MockObject obj) {
                    return obj.Name == "Jacky";
                })[0];
                db.Delete(obj2);
                Assert.AreEqual(0, db.Load<MockObject>(delegate(MockObject obj) {
                    return obj.Name == "Jacky";
                }).Count);

            }
        }
    }
}
