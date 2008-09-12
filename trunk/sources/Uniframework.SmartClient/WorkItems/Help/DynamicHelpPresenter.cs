using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.EventBroker;

namespace Uniframework.SmartClient
{
    public class DynamicHelpPresenter : Presenter<IDynamicHelpView>
    {
        #region Dependency services

        [ServiceDependency]
        public IImageService ImageService
        {
            get;
            set;
        }

        #endregion

        private Uri homeUrl;
        public Uri HomeUrl
        {
            get { return homeUrl; }
        }

        public override void OnViewReady()
        {
            base.OnViewReady();
        }

        protected override void CloseView()
        {
            base.CloseView();
        }

        [EventSubscription(EventNames.Shell_DynamicHelpUpdated, ThreadOption.UserInterface)]
        public void OnHelpUrlUpdated(object sender, EventArgs<Uri> e)
        {
            if (homeUrl == null) {
                homeUrl = e.Data;
                View.HomeUrlChanged();
            }
            View.UpdateHelpUrl(e.Data);
        }
    }
}
