using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lephone.Data;
using Lephone.Data.Definition;

namespace Uniframework.Entities
{
    /// <summary>
    /// 系统公共文档类用于存储一切文档（二进制的、文本的）
    /// </summary>
    [DbTable("COM_Document")]
    [Cacheable, Serializable]
    public class Document : DbObjectModel<Document>
    {
        [Index("IX_Document", ASC = true)]
        [Length(1, 128)]
        public string Name { get; set; }
        [AllowNull, Length(255)]
        public string DocumentUri { get; set; }
        [Length(10)]
        public string Extension { get; set; }
        //[AllowNull]
        //public int Revision { get; set; }
        [Index("IX_Summary")]
        [AllowNull, Length(128)]
        public string Summary { get; set; }
        [AllowNull, LazyLoad]
        public string Data { get; set; }
        [SpecialName]
        public DateTime CreatedOn { get; set; }
        [SpecialName]
        public DateTime? UpdatedOn { get; set; }
    }
}
