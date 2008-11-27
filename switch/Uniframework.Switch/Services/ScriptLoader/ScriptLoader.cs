using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

using csscript;
using CSScriptLibrary;

namespace Uniframework.Switch
{
    /// <summary>
    /// �ű����������
    /// </summary>
    public class ScriptLoader : IScriptLoader, IDisposable
    {
        private readonly static string DefaultMainPoint = "Run";

        private AppDomain appDomain;
        private string filename = string.Empty;
        private string mainPoint = DefaultMainPoint;
        private IChannel channel;
        private ScriptExecutor executor;
        private List<object> args = null;

        /// <summary>
        /// �ű����������캯�����ű������������������������ڵ�����������ؽű�������ͨ�������µĳ����������ؽű������Դ˱���ű�����Ĵ�����������ϵͳ������
        /// </summary>
        /// <param name="filename">�ű��ļ�</param>
        /// <param name="domainName">�ӳ���������</param>
        /// <param name="chnl">�ű����õ�ͨ��</param>
        public ScriptLoader(string filename, string domainName, IChannel chnl)
        {
            if (!File.Exists(filename))
                throw new ArgumentException("�ű��ļ� \"" + filename + "\"������!");

            this.filename = filename;
            this.channel = chnl;

            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Path.GetDirectoryName(this.filename);
            setup.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory;
            setup.ApplicationName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            setup.ShadowCopyFiles = "true";
            setup.ShadowCopyDirectories = Path.GetDirectoryName(this.filename);
            // �����ӳ�����
            string domainname = domainName == null || domainName == string.Empty ? Path.GetFileNameWithoutExtension(this.filename) : domainName;
            appDomain = AppDomain.CreateDomain(domainname, null, setup);

            executor = (ScriptExecutor)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, typeof(ScriptExecutor).ToString());
            string SearchPath = Environment.GetEnvironmentVariable("path");
            SearchPath = Path.GetDirectoryName(this.filename) + @"; Scripts\;" + AppDomain.CurrentDomain.BaseDirectory + ";" + SearchPath;
            executor.SearchDirs = SearchPath.Split(new char[] { ';' });
            executor.Channel = chnl;
        }

        /// <summary>
        /// �ű����������캯�����ذ汾
        /// </summary>
        /// <param name="filename">�ű��ļ�</param>
        /// <param name="domainName">�ӳ�����</param>
        /// <param name="chnl">�ű����õ�ͨ��</param>
        /// <param name="args">�ű�ִ��ʱ�Ĳ�����Ϣ</param>
        public ScriptLoader(string filename, string domainName, IChannel chnl, object[] args) 
            : this(filename, domainName, chnl)
        {
            this.args = new List<object>();
            foreach (object arg in args)
                this.args.Add(arg);
        }

        /// <summary>
        /// ж���ӳ�������ص���Դ
        /// </summary>
        public void Unload()
        {
            AppDomain.Unload(appDomain);
            appDomain = null;
        }

        #region Assistant function



        #endregion

        #region IScriptLoader Members

        public string FileName
        {
            get { return filename; }
        }

        public string MainPoint
        {
            get { return mainPoint; }
            set { mainPoint = value; }
        }

        public void RunScript()
        {
            executor.MainPoint = mainPoint;
            if (args == null)
            {
                executor.ExecuteAssembly(filename, null);
            }
            else
                executor.ExecuteAssembly(filename, args.ToArray());
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (appDomain != null)
                Unload();
        }

        #endregion
    }

    #region Script executor class

    class ScriptExecutor : MarshalByRefObject
    { 
        private string workingDir;
        private IChannel channel;
        private string mainPoint = string.Empty;
        private string[] searchDirs = new string[0];
    
        public ScriptExecutor(string[] searchDirs)
        {
            this.searchDirs = searchDirs;
        }

        public ScriptExecutor()
        {
        }
        
        /// <summary>
        /// AppDomain evant handler. This handler will be called if CLR cannot resolve 
        /// referenced local assemblies 
        /// </summary>
        public Assembly ResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly retval = null;
            foreach (string dir in searchDirs)
                if (null != (retval = AssemblyResolver.ResolveAssembly(args.Name, dir)))
                    break;
            return retval;
        }

        public void ExecuteAssembly(string filename, object[] args)
        {
            workingDir = Path.GetDirectoryName(filename);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveEventHandler);
            Assembly assembly = CSScript.Load(filename, null, true);
            InvokeMainPoint(assembly, args);
        }

        /// <summary>
        /// Invoke loaded assembly's main point function.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="args"></param>
        private void InvokeMainPoint(Assembly assembly, object[] args)
        {
            Type scriptType = null;
            foreach (System.Reflection.Module module in assembly.GetModules())
            {   
                // �ڵ�ǰģ���в���ʵ����ICTIScript�ӿڵ���
                foreach (Type type in module.GetTypes())
                {
                    if (type.GetInterface("ICTIScript", true) != null)
                    {
                        scriptType = type;
                        break;
                    }
                }

                if (module != null)
                {
                    break;
                }
            }

            // ͨ��������ýű���ִ�з���
            ICTIScript Script = (ICTIScript)assembly.CreateInstance(scriptType.Name, true);
            try
            {
                Script.Initialize(channel);
                if (args != null && args.Length > 0)
                    Script.Run(args);
                else
                    Script.Run();
                Script.Terminate();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Script executor members

        public string WorkingDir
        {
            get { return workingDir; }
            set { workingDir = value; }
        }

        public IChannel Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        public string MainPoint
        {
            get { return mainPoint; }
            set { mainPoint = value; }
        }

        public string[] SearchDirs
        {
            get { return searchDirs; }
            set { searchDirs = value; }
        }

        #endregion
    }

    #endregion
}
