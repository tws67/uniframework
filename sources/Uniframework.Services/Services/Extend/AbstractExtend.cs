using System;
using System.Collections.Generic;
using System.Text;

using Castle.MicroKernel.Facilities;

namespace Uniframework.Services
{
    /// <summary>
    /// Castle��չ
    /// </summary>
    public class AbstractExtend : AbstractFacility
    {
        protected string configPath = string.Empty;

        protected override void Init()
        {
            // throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ����·��
        /// </summary>
        public string ConfigPath
        {
            get
            {
                return configPath;
            }

            set
            {
                configPath = value;
            }
        }

        /// <summary>
        /// ������ɷ���
        /// </summary>
        /// <param name="components">�������</param>
        public virtual void LoadFinished(object[] components)
        {
        }

    }
}
