using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

using Uniframework.Database;
using Uniframework.Entities;

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

        public DbObjectList<Document> Documents()
        {
            return DatabaseService.ExecuteList<Document>(new Lephone.Data.SqlEntry.SqlStatement("select * from COM_Document"));
        }
    }
}
