﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraNavBar;
using Uniframework.SmartClient;

namespace Uniframework.StartUp
{
    /// <summary>
    /// 框架外壳窗口布局属性类
    /// </summary>
    public class ShellLayout : FormLayout
    {
        public ShellLayout()
            : base()
        { }

        public NavPaneState NavPaneState
        {
            get;
            set;
        }

        public string NavPaintStyleName
        {
            get;
            set;
        }
    }
}
