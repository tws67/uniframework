using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI;
using Uniframework.Security;

namespace Uniframework.Common.WorkItems.Authorization
{
    public class CommandPresenter : Presenter<CommandView>
    {
        [ServiceDependency]
        public IImageService ImageService
        {
            get;
            set;
        }

        [ServiceDependency]
        public IAuthorizationStoreService AuthorizationStoreService
        {
            get;
            set;
        }
    }
}
