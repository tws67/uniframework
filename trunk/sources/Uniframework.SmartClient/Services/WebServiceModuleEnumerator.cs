using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI.Configuration;
using Microsoft.Practices.CompositeUI.Services;

using Uniframework.Client;
using Uniframework.Services;

namespace Uniframework.SmartClient
{
    public class WebServiceModuleEnumerator : IModuleEnumerator
    {
        private static readonly string AddinsPath = @"\AddIns\";
        bool hasMultiMainWorkspace = false;
        List<IModuleInfo> list = new List<IModuleInfo>();

        public bool HasMultiMainWorkspace
        {
            get
            {
                return hasMultiMainWorkspace;
            }
        }

        public void Load()
        {
            ISystemService systemService = ServiceRepository.Instance.GetService<ISystemService>();
            ClientModuleInfo[] infos = systemService.GetClientModules();
            CheckMultiMainWorkspace(infos);
            foreach (ClientModuleInfo info in infos)
            {
                if (!File.Exists(info.AssemblyFile))
                {
                    string addinsPath = FileUtility.GetParent(FileUtility.ApplicationRootPath) + AddinsPath;
                    List<string> files = FileUtility.SearchDirectory(addinsPath, info.AssemblyFile);
                    if (files.Count > 0)
                        info.AssemblyFile = files[0];
                }
                // Modified by Jacky 2008-08-18,去掉了允许的角色列表
                //list.Add(new ModuleInfo(info.AllowedRoles, info.AssemblyFile, info.UpdateLocation));
                list.Add(new ModuleInfo(null, info.AssemblyFile, info.UpdateLocation));
            }
        }

        public IModuleInfo[] EnumerateModules()
        {
            return list.ToArray();
        }

        #region Assistant function

        private void CheckMultiMainWorkspace(ClientModuleInfo[] infos)
        {
            int mainModuleCount = 0;
            foreach (ClientModuleInfo info in infos)
            {
                if (info.IsMainModule)
                    mainModuleCount++;
            }
            hasMultiMainWorkspace = mainModuleCount > 1;
        }

        #endregion 

    }

    #region Entity calss

    public class ModuleInfo : IModuleInfo
    {
        private string assemblyFile;
        private string updateLocation;
        private IList<string> allowedRolesField;

        public ModuleInfo(IList<string> roles, string file, string location)
        {
            assemblyFile = file;
            updateLocation = location;
            allowedRolesField = roles;
        }

        public IList<string> AllowedRoles
        {
            get { return this.allowedRolesField; }
        }

        public string AssemblyFile
        {
            get { return assemblyFile; }
        }

        public string UpdateLocation
        {
            get { return updateLocation; }
        }
    }

    #endregion

}
