using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using Uniframework.SmartClient;
using Microsoft.Practices.CompositeUI;
using Uniframework.SmartClient.WorkItems.Setting;

namespace Uniframework.Upgrade.Views
{
    public partial class UpgradeSettingView : DevExpress.XtraEditors.XtraUserControl, ISetting
    {
        private Property property = null;

        public UpgradeSettingView()
        {
            InitializeComponent();
        }

        private void chkCheckUpgrade_CheckedChanged(object sender, EventArgs e)
        {
            txtCheckInterval.Enabled = chkCheckUpgrade.Checked;
        }

        #region Dependendcy services
        
        
        [ServiceDependency]
        public IPropertyService PropertyService
        {
            get;
            set;
        }

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get;
            set;
        }

        #endregion

        #region ISetting Members

        public event EventHandler<EventArgs<object>> SettingChanged;

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public Property Property
        {
            get {
                if (property == null)
                    property = new Property(UIExtensionSiteNames.Shell_Property_Upgrade, new UpgradeSetting());
                return property;
            }
        }

        /// <summary>
        /// Bindings the property.
        /// </summary>
        public void BindingProperty()
        {
            UpgradeSetting setting = PropertyService.Get<UpgradeSetting>(UIExtensionSiteNames.Shell_Property_Upgrade, new UpgradeSetting());
            txtUpgradeUrl.Text = setting.UpgradeUrl;
            chkReciveMessage.Checked = setting.ReciveUpgradeMessage;
            chkCheckUpgrade.Checked = setting.CheckInterval > 0;
            txtCheckInterval.Value = setting.CheckInterval;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            UpgradeSetting setting = new UpgradeSetting { 
                CheckInterval = (int)txtCheckInterval.Value,
                ReciveUpgradeMessage = chkReciveMessage.Checked,
                UpgradeUrl = txtUpgradeUrl.Text
            };

            property.Current = setting;
            PropertyService.Set<UpgradeSetting>(UIExtensionSiteNames.Shell_Property_Upgrade, setting);
            if (SettingChanged != null)
                SettingChanged(this, new EventArgs<object>(setting)); // ´¥·¢ÊÂ¼þ
        }

        /// <summary>
        /// Loads the default.
        /// </summary>
        public void LoadDefault()
        {
            property.SetDefault();
            BindingProperty();
        }

        #endregion

        private void UpgradeSettingView_Load(object sender, EventArgs e)
        {
            BindingProperty();
        }
    }
}
