using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security;
using System.Text;
using System.Windows.Forms;

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraEditors;

using Uniframework.Client;
using Uniframework.SmartClient;

namespace Uniframework.StartUp
{
    public partial class frmLogin : DevExpress.XtraEditors.XtraForm
    {
        private int PasswordTryCount = 0;
        private readonly static string RECENT_USERS = "Shell.Property.RecentUsers";

        delegate void SetString(string text);
        delegate void SetProgress(int i);

        public frmLogin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username
        {
            get
            {
                return txtUser.Text;
            }
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get
            {
                return txtPassword.Text;
            }
        }

        /// <summary>
        /// Sets the label.
        /// </summary>
        /// <param name="label">The label.</param>
        public void SetLabel(string label)
        {
            if (this.lblProgress.InvokeRequired)
            {
                SetString setTextDel = delegate(string text)
                { lblProgress.Text = text; };
                lblProgress.Invoke(setTextDel, new object[] { label });
            }
            else
                lblProgress.Text = label;
        }

        /// <summary>
        /// Sets the user name focus.
        /// </summary>
        public void SetUserNameFocus()
        {
            this.BringToFront();
            txtUser.Focus();
        }

        /// <summary>
        /// Increaces the progress.
        /// </summary>
        /// <param name="i">The i.</param>
        public void IncreaceProgress(int i)
        {
            if (pbProgress.InvokeRequired)
            {
                SetProgress setProgress = delegate(int it)
                { pbProgress.Position += it; };
                pbProgress.Invoke(setProgress, new object[] { i });
            }
            else
                pbProgress.Position += i;
        }

        public event EventHandler Acceptted;
        public event EventHandler Cancelled;

        /// <summary>
        /// Disables the UI.
        /// </summary>
        private void DisableUI()
        {
            this.txtUser.Enabled = false;
            this.txtPassword.Enabled = false;
            this.btnLogin.Enabled = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                CommunicateProxy.SetCredential(Username, Password);
                CommunicateProxy.SessionID = Program.SessionID;
                CommunicateProxy.EncryptKey = Guid.NewGuid().ToString();
                using (WaitCursor cursor = new WaitCursor(true))
                {
                    CommunicateProxy.RegisterSession();
                }
            }
            catch (SecurityException)
            {
                string errInfo = PasswordTryCount != 3 ? "您输入的用户名或密码不正确请确认后再试。" : "非授权用户不得使用本系统，请与系统管理员联系。";
                XtraMessageBox.Show(errInfo);
                txtPassword.Focus();
                PasswordTryCount++;
                if (PasswordTryCount >= 3)
                {
                    Environment.Exit(0);
                }
                else
                    return;
            }

            DisableUI();
            this.lblProgress.Visible = true;
            this.pbProgress.Visible = true;
            if (Acceptted != null)
                Acceptted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the LinkClicked event of the linkClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void linkClose_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Cancelled != null)
                Cancelled(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the Activated event of the LoginForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LoginForm_Activated(object sender, EventArgs e)
        {
            using (PropertyService propertyService = new PropertyService()) {
                ShellLayout layout = propertyService.Get(UIExtensionSiteNames.Shell_Property_ShellLayout) as ShellLayout;
                if (layout != null)
                    UserLookAndFeel.Default.SetSkinStyle(layout.DefaultSkin);
            } 

            txtUser.Focus();
        }
    }
}