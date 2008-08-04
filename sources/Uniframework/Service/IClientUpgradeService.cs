using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// �ͻ��˸��·���
    /// </summary>
    public interface IClientUpgradeService
    {
        /// <summary>
        /// ���ͻ���ģ��
        /// </summary>
        /// <param name="loadedModules">�Ѿ����صĿͻ���ģ���б�</param>
        /// <returns>���ؼ��Ŀͻ����б�</returns>
        List<ClientModuleInfo> CheckClientModule(List<ClientLoadedModuleInfo> loadedModules);
    }

    /// <summary>
    /// �ͻ����Ѿ����ص�SmartClient������Ϣ
    /// </summary>
    [Serializable]
    public class ClientLoadedModuleInfo
    {
        private string assemblyFile;
        private string fullName;
        private Version version;
        private string updateLocation;

        /// <summary>
        /// �޲ι��캯��
        /// </summary>
        public ClientLoadedModuleInfo()
        { }

        /// <summary>
        /// �ͻ���ģ����Ϣ
        /// </summary>
        /// <param name="assemblyFile">�����ļ�</param>
        /// <param name="fullName">������ȫ�޶���</param>
        /// <param name="version">�汾��</param>
        /// <param name="updateLocation">����λ��</param>
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
        /// �����ļ�
        /// </summary>
        public string AssemblyFile
        {
            get { return assemblyFile; }
        }

        /// <summary>
        /// ������ȫ�޶���
        /// </summary>
        public string FullName
        {
            get { return fullName; }
        }

        /// <summary>
        /// ���򼯰汾
        /// </summary>
        public Version Version
        {
            get { return version; }
        }

        /// <summary>
        /// �ͻ��˳��򼯸���λ��
        /// </summary>
        public string UpdateLocation
        {
            get { return updateLocation; }
        }

        #endregion
    }
}
