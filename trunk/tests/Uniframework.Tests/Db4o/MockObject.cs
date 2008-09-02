using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.Tests.Db4o
{
    public class MockObject
    {
        public MockObject()
        { }

        public string Name
        {
            get;
            set;
        }

        public string Phone
        {
            get;
            set;
        }

        public Address Address
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name + " Phone : " + Phone);
            sb.Append(Environment.NewLine);
            sb.Append(Address.ToString());
            return sb.ToString();
        }
    }

    public class Address
    {
        public Address()
        { }

        public string Office
        {
            get;
            set;
        }

        public string Home
        {
            get;
            set;
        }

        public override string ToString()
        {
            return String.Format("  Address - Office[{0}] Home[{1}]", Office, Home);
        }
    }

}
