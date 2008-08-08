using System;
using System.Collections.Generic;
using System.Text;

using Castle.MicroKernel.Facilities;

namespace Uniframework.Services
{
    /// <summary>
    /// Castle扩展
    /// </summary>
    public class AbstractExtend : AbstractFacility
    {
        protected string configPath = string.Empty;

        protected override void Init()
        {
            // throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 配置路径
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
        /// 加载完成方法
        /// </summary>
        /// <param name="components">组件数组</param>
        public virtual void LoadFinished(object[] components)
        {
        }

    }
}
