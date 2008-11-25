using System;
using System.Collections.Generic;
using System.Text;

using Uniframework.Services;

namespace Uniframework.Switch
{
    public interface IConfigurable
    {
        void Configuration(IConfiguration config);
    }
}
