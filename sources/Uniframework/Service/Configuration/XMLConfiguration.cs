using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统配置项的XML实现类
    /// </summary>
    public class XMLConfiguration : AbstractConfiguration
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public XMLConfiguration(string name, string value)
            : base()
        {
            this.internalName = name;
            this.internalValue = value;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="node">The node.</param>
        public XMLConfiguration(XmlNode node)
            : base()
        {
            this.internalName = node.LocalName;
            this.internalValue = node.InnerText;

            if (node.Attributes != null)
                foreach (XmlAttribute attr in node.Attributes)
                {
                    Attributes.Add(attr.LocalName, attr.InnerText);
                }

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element)
                    continue;

                Children.Add(new XMLConfiguration(child));
            }
        }
    }
}
