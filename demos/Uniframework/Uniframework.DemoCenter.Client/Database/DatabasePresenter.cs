using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Uniframework.Database;
using System.Data;

namespace Uniframework.DemoCenter.Client.Database
{
    public class DatabasePresenter : Presenter<DatabaseView>
    {
        [ServiceDependency]
        public WorkItem WorkItem
        {
            get;
            set;
        }

        [ServiceDependency]
        public IDatabaseService DatabaseService
        {
            get;
            set;
        }

        public DataSet GetDocuments()
        {
            return DatabaseService.ExecuteDataset("select * from COM_Document");
        }
    }
}
