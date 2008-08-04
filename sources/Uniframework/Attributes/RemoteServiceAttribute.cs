using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework
{
        /// <summary>
    /// 远程系统服务属性标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple=false)]
    public class RemoteServiceAttribute : Attribute
    {
        private string description = string.Empty;
        private ServiceType serviceType = ServiceType.Unknown;
        private ServiceScope serviceScope = ServiceScope.Global;

        /// <summary>
        /// 远程系统服务标签构造函数
        /// </summary>
        /// <param name="desctiption">服务描述</param>
        /// <param name="serviceType">类型</param>
        /// <param name="serviceScope">服务发布范围</param>
        public RemoteServiceAttribute(string desctiption, ServiceType serviceType, ServiceScope serviceScope)
        {
            this.description = description;
            this.serviceType = serviceType;
            this.serviceScope = serviceScope;
        }

        /// <summary>
        /// 远程系统服务标签构造函数（重载）
        /// </summary>
        /// <param name="description">服务描述</param>
        /// <param name="serviceType">类型</param>
        public RemoteServiceAttribute(string description, ServiceType serviceType)
            : this(description, serviceType, ServiceScope.Global)
        { }

        /// <summary>
        /// 远程系统服务标签构造函数（重载）
        /// </summary>
        /// <param name="description">服务描述</param>
        /// <param name="serviceScope">服务发布范围</param>
        public RemoteServiceAttribute(string description, ServiceScope serviceScope)
            : this(description, ServiceType.Unknown, serviceScope)
        { }

        /// <summary>
        /// 远程系统服务标签构造函数（重载）
        /// </summary>
        /// <param name="description">服务描述</param>
        public RemoteServiceAttribute(string description)
            : this(description, ServiceType.Unknown, ServiceScope.Global)
        { }

        /// <summary>
        /// 远程系统服务标签构造函数（重载）
        /// </summary>
        /// <param name="serviceType">类型</param>
        public RemoteServiceAttribute(ServiceType serviceType)
            : this(string.Empty, serviceType,ServiceScope.Global)
        { }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// 服务类型
        /// </summary>
        public ServiceType ServiceType
        {
            get
            {
                return serviceType;
            }
        }

        /// <summary>
        /// 服务发布范围
        /// </summary>
        public ServiceScope ServiceScope
        { get { return serviceScope; } }
    }

    #region 系统服务类型
    /// <summary>
    /// 系统服务类型枚举
    /// </summary>
    [Serializable]
    public enum ServiceType
    {
        /// <summary>
        /// 未知系统
        /// </summary>
        Unknown,
        /// <summary>
        /// 系统服务
        /// </summary>
        System,
        /// <summary>
        /// 工作流系统
        /// </summary>
        Workflow,
        /// <summary>
        /// 业务系统
        /// </summary>
        Business,
        /// <summary>
        /// 基础设施
        /// </summary>
        Infrustructure
    }
    #endregion

    #region 系统服务范围

    /// <summary>
    /// 服务发布范围
    /// </summary>
    public enum ServiceScope
    { 
        /// <summary>
        /// 全部
        /// </summary>
        Global,
        /// <summary>
        /// 智能客户端
        /// </summary>
        SmartClient,
        /// <summary>
        /// 移动客户端
        /// </summary>
        Mobile
    }

    #endregion
}
