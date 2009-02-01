using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 数据处理控制器基类
    /// </summary>
    public class DataPresenter<TView> : Presenter<TView>
        where TView : IDataView
    {
    }
}
