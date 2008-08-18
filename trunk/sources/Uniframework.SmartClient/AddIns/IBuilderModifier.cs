using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 插件单元构建修改器，用于对<seealso cref="IBuilder"/>构建的对象进行进一步的处理
    /// </summary>
    public interface IBuilderModifier
    {
        void Apply(IList items);
    }
}
