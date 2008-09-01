using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Uniframework.SmartClient;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Uniframework.SmartClient.Tests
{
    [TestFixture]
    public class PropertyServiceTest
    {
        private PropertyService propertyService;

        [TestFixtureSetUp]
        public void Init()
        {
            if (File.Exists(@"..\Configuration\Unifrmaework.conf"))
                File.Delete(@"..\Configuration\Unifrmaework.conf");
            propertyService = new PropertyService();
        }

        [Test]
        public void GetTest()
        {
            FormLayout layout = new FormLayout { 
                Location = new Point(10, 10),
                Size = new Size(400, 400),
                WindowState = FormWindowState.Normal
            };

            propertyService.Set<FormLayout>("Test", layout);

            FormLayout lay1 = propertyService.Get("Test") as FormLayout;
            Assert.IsNotNull(lay1);
            Assert.AreEqual(new Point(10, 10), lay1.Location);
            Assert.AreEqual(new Size(400, 400), lay1.Size);
            Assert.AreEqual(FormWindowState.Normal, lay1.WindowState);

            propertyService.Set<FormLayout>("Test", lay1);
            FormLayout lay2 = propertyService.Get("Test") as FormLayout;
            Assert.IsNotNull(lay2);
            Assert.AreEqual(lay1.Location, lay2.Location);
            Assert.AreEqual(lay1.Size, lay2.Size);
            Assert.AreEqual(lay1.WindowState, lay2.WindowState);

            lay2.Location = new Point(0, 0);
            lay2.Size = new Size(300, 300);
            lay2.WindowState = FormWindowState.Maximized;

            propertyService.Set<FormLayout>("Test", lay2);
            FormLayout lay3 = propertyService.Get("Test") as FormLayout;
            Assert.IsNotNull(lay3);
            Assert.AreEqual(new Point(0, 0), lay3.Location);
            Assert.AreEqual(new Size(300, 300), lay3.Size);
            Assert.AreEqual(FormWindowState.Maximized, lay3.WindowState);
        }

        [Test]
        public void GenericGetTest()
        {
            FormLayout layout = new FormLayout
            {
                Location = new Point(10, 10),
                Size = new Size(400, 400),
                WindowState = FormWindowState.Normal
            };

            propertyService.Set<FormLayout>("Test", layout);
            FormLayout lay1 = propertyService.Get<FormLayout>("Test", new FormLayout());
            Assert.IsNotNull(lay1);
            Assert.AreEqual(new Point(10, 10), lay1.Location);
            Assert.AreEqual(new Size(400, 400), lay1.Size);
            Assert.AreEqual(FormWindowState.Normal, lay1.WindowState);

            FormLayout lay2 = propertyService.Get<FormLayout>("Test2", new FormLayout());
            Assert.IsNotNull(lay2);
            Assert.AreNotEqual(new Point(10, 10), lay2.Location);

            propertyService.Set<int>("IntProperty", 10);
            Assert.AreEqual(10, propertyService.Get<int>("IntProperty", 0));

            propertyService.Set<string>("StringProperty", "Test");
            Assert.AreEqual("Test", propertyService.Get<string>("StringProperty", ""));
        }
    }
}
