using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 客户端更新服务
    /// </summary>
    public interface IClientUpgradeService
    {
        /// <summary>
        /// 检查客户端模块
        /// </summary>
        /// <param name="loadedModules">已经加载的客户端模块列表</param>
        /// <returns>返回检查的客户端列表</returns>
        List<ClientModuleInfo> CheckClientModule(List<ClientLoadedModuleInfo> loadedModules);
    }

    /// <summary>
    /// 客户端已经加载的SmartClient程序集信息
    /// </summary>
    [Serializable]
    public class ClientLoadedModuleInfo
    {
        private string assemblyFile;
        private string fullName;
        private Version version;
        private string updateLocation;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ClientLoadedModuleInfo()
        { }

        /// <summary>
        /// 客户端模块信息
        /// </summary>
        /// <param name="assemblyFile">程序集文件</param>
        /// <param name="fullName">程序集完全限定名</param>
        /// <param name="version">版本号</param>
        /// <param name="updateLocation">更新位置</param>
        public ClientLoadedModuleInfo(string assemblyFile, string fullName, Version version, string updateLocation)
            : this()
        {
            this.assemblyFile = assemblyFile;
            this.fullName = fullName;
            this.version = version;
            this.updateLocation = updateLocation;
        }

        #region ClientLoadedModuleInfo Members

        /// <summary>
        /// 程序集文件
        /// </summary>
        public string AssemblyFile
        {
            get { return assemblyFile; }
        }

        /// <summary>
        /// 程序集完全限定名
        /// </summary>
        public string FullName
        {
            get { return fullName; }
        }

        /// <summary>
        /// 程序集版本
        /// </summary>
        public Version Version
        {
            get { return version; }
        }

        /// <summary>
        /// 客户端程序集更新位置
        /// </summary>
        public string UpdateLocation
        {
            get { return updateLocation; }
        }

        #endregion
    }
}
