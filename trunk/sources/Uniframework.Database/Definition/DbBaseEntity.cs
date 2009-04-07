using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lephone.Data.Definition;

namespace Uniframework.Database.Definition
{
    /// <summary>
    /// 业务实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <typeparam name="TKey">业务实体主键类型</typeparam>
    [Serializable]
    public abstract class DbBaseEntity<T, TKey> : DbObjectBase where T : DbBaseEntity<T, TKey>
    {
        [Length(128)]
        public abstract string Name { get; set; }
        [Length(30)]
        public abstract string CreatedBy { get; set; }
        [SpecialName]
        public abstract DateTime CreatedOn { get; set; }
        [AllowNull, Length(30)]
        public abstract string UpdatedBy { get; set; }
        [SpecialName]
        public abstract DateTime? UpdatedOn { get; set; }
        [SpecialName]
        public abstract int LockVersion { get; set; }
        [AllowNull, Length(255)]
        public abstract string Comment { get; set; }

        /// <summary>
        /// 以特定的名称实例化业务实体
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>已经初始化的业务实体</returns>
        public DbBaseEntity<T, TKey> Init(string name) {
            this.Name = name;
            return this;
        }

        /// <summary>
        /// 以特定的名称及创建者实例化业务实体
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="createdBy">创建者标识</param>
        /// <returns>已经初始化的业务实体</returns>
        public DbBaseEntity<T, TKey> Init(string name, string createdBy)
        {
           this.Name = name;
           this.CreatedBy = createdBy;
           this.UpdatedBy = createdBy;
           return this;
        }
    }

    /// <summary>
    /// 业务实体类
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    [Serializable]
    public abstract class DbBaseEntity<T> : DbObjectModel<T, long> where T : DbObjectModel<T, long>
    {
        [Length(128)]
        public abstract string Name { get; set; }
        [Length(30)]
        public abstract string CreatedBy { get; set; }
        [SpecialName]
        public abstract DateTime CreatedOn { get; set; }
        [AllowNull, Length(30)]
        public abstract string UpdatedBy { get; set; }
        [SpecialName]
        public abstract DateTime? UpdatedOn { get; set; }
        [SpecialName]
        public abstract int LockVersion { get; set; }
        [AllowNull, Length(255)]
        public abstract string Comment { get; set; }

        /// <summary>
        /// 以特定的名称实例化业务实体
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>已经初始化的业务实体</returns>
        public DbBaseEntity<T> Init(string name)
        {
            this.Name = name;
            return this;
        }

        /// <summary>
        /// 以特定的名称及创建者实例化业务实体
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="createdBy">创建者标识</param>
        /// <returns>已经初始化的业务实体</returns>
        public DbBaseEntity<T> Init(string name, string createdBy)
        {
            this.Name = name;
            this.CreatedBy = createdBy;
            this.UpdatedBy = createdBy;
            return this;
        }
    }
}
