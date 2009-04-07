using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uniframework.Database.Definition;

using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Uniframework.Database.Tests
{
    [Serializable]
    [DbTable("COM_Role")]
    public abstract class Role : DbBaseEntity<Role>
    {
        [Length(30)]
        [Index(IndexName="IX_Role", ASC=true)]
        public abstract string Name { get; set; }
    }

    [Serializable]
    [DbTable("COM_Unit")]
    public abstract class Unit : DbBaseEntity<Unit, string>
    {
        [DbKey(IsDbGenerate = false), Length(30)]
        public string UnitId { get; set; }
        [Length(30)]
        public override string Name { get; set; }
        [SpecialName]
        public abstract int Count { get; set; }

        public Unit() { }

        public Unit(string unitId) {
            this.UnitId = unitId;
        }
    }

    [Serializable]
    public abstract class Person : DbObjectModel<Person>
    {
        [HasOne]
        public abstract PersonalComputer PC { get; set; }
        [Length(50)]
        public abstract string Name { get; set; }
    }

    [DbTable("PCs")]
    public abstract class PersonalComputer : DbObjectModel<PersonalComputer>
    {
        [Length(50)]
        public abstract string Name { get; set; }
        [BelongsTo]
        public abstract Person Owner { get; set; }
    }

    [TestFixture]
    public class CommonTest
    {
        [Test]
        public void Test1()
        {
            Role role = Role.New();
            role.Name = "Jacky";
            role.CreatedBy = "Jacky";
            role.UpdatedBy = "Jacky";
            DbEntry.Save(role);

            var r = DbEntry.GetObject<Role>(role.Id);
            Assert.AreEqual(role.Name, r.Name);

            role.Name = "Niuniu";
            DbEntry.Save(role);
            Assert.AreEqual(role.Id, r.Id);

            long id = r.Id;
            DbEntry.Delete(role);
            var r1 = DbEntry.GetObject<Role>(id);
            Assert.IsNull(r1);
        }

        [Test]
        public void Test2()
        {
            //Role r = new Role() { 
            //    Name = "Jacky",
            //    CreatedBy = "Admin",
            //    UpdatedBy = "Admin"
            //};
            Role r = Role.New();
            r.Name = "Jacky";
            r.CreatedBy = "Admin";
            r.UpdatedBy = "Admin";

            DbEntry.Save(r);

            Role r1 = DbEntry.GetObject<Role>(r.Id);
            Assert.IsNotNull(r1);
            Assert.AreEqual("Admin", r1.CreatedBy);
            r1.Name = "Modified by jacky.";
            DbEntry.Update(r1);

            Role r2 = DbEntry.GetObject<Role>(r.Id);
            Assert.AreEqual("Modified by jacky.", r2.Name);
        }

        [Test]
        public void Test3()
        {
            DbEntry.Delete<Unit>(WhereCondition.EmptyCondition);

            Unit u = DynamicObject.NewObject<Unit>("00001");
            u.Name = "吨";
            u.CreatedBy = "Jacky";
            u.UpdatedBy = "Admin";

            DbEntry.Save(u);
            var u1 = DbEntry.GetObject<Unit>(u.UnitId);
            Assert.AreEqual(u1.UnitId, u.UnitId);

            u1.Name = "M3";
            DbEntry.Update(u1);

            Unit u2 = DynamicObject.NewObject<Unit>("00002");
            u2.Init("M3", "Admin");
            Assert.AreEqual("M3", u2.Name);
            DbEntry.Save(u2);

            Assert.AreEqual(2, DbEntry.From<Unit>().Where(WhereCondition.EmptyCondition).GetCount());
        }

        [Test]
        public void Test4()
        {
            Person p = Person.New();
            p.Name = "Jacky";
            p.PC = PersonalComputer.New();
            p.PC.Name = "hp dl380 g3";
            DbEntry.Save(p);
        }
    }
}
