using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Linq;

namespace Uniframework.Entities
{
    /// <summary>
    /// 文档关联类
    /// </summary>
    [DbTable("COM_DocumentAssociate")]
    [Cacheable, Serializable]
    public class DocumentAssociate : LinqObjectModel<DocumentAssociate>
    {
        [Index("IX_DocumentAssociate")]
        [Length(128)]
        public string Name { get; set; }
        [BelongsTo, DbColumn("ParentId")]
        public DocumentAssociate Parent { get; set; }
        [HasMany(OrderBy = "Id")]
        public IList<DocumentAssociate> Children { get; set; }
        [Length(30)]
        public string Owner { get; set; }
        [SpecialName]
        public DateTime CreatedOn { get; set; }
        [Length(255), AllowNull]
        public string AppUri { get; set; }
        [Length(255), AllowNull]
        public string DataSource { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDefault { get; set; }
        public bool LocalCache { get; set; }
        [HasOne(OrderBy = "DocumentId"), DbColumn("DocumentId")]
        public Document Document { get; set; }
        [Length(30)]
        public string UpdatedBy { get; set; }
        [SpecialName]
        public DateTime? UpdatedOn { get; set; }
        [Length(255)]
        public string Comment { get; set; }
    }
}
