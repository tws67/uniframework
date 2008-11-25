using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    public class VersionInfo : IVersionInfo
    {
        private string name;
        private string description = string.Empty;
        private string version = string.Empty;
        private Dictionary<string, object> properties;

        public VersionInfo(string name)
        {
            this.name = name;
            properties = new Dictionary<string, object>();
        }

        public VersionInfo(string name, string description)
            : this(name)
        {
            this.description = description;
        }

        public VersionInfo(string name, string description, string version)
            : this(name, description)
        {
            this.version = version;
        }

        #region ICommonInfo Members

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }

        public Dictionary<string, object> Properties
        {
            get { return properties; }
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("名称 : {0}\n", name));
            sb.Append(String.Format("版本 : {0}\n", version));
            sb.Append(String.Format("描述 : {0}\n", description));
            if (properties.Count > 0)
            {
                sb.Append("扩展的版本信息 >> -------------------- \n");
                foreach(KeyValuePair<string, object> item in properties)
                {
                    sb.Append(item.Key + " : " + item.Value.ToString() + "\n");
                }
            }
            return sb.ToString();
        }
    }
}
