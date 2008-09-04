using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Skins;
using DevExpress.UserSkins;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Uniframework.Services;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// Uniframework 客户端执行环境
    /// </summary>
    [Service]
    public class SmartClientEnvironment
    {

        #region Smart client environment fields

        private UserInfo currentUser;
        /// <summary>
        /// Occurs when [shell status updated].
        /// </summary>
        [EventPublication(EventNames.Shell_StatusUpdated, PublicationScope.Global)]
        private event EventHandler<EventArgs<string>> ShellStatusUpdated;
        /// <summary>
        /// Occurs when [shell custom panel1 updated].
        /// </summary>
        [EventPublication(EventNames.Shell_CustomPanel1Updated, PublicationScope.Global)]
        private event EventHandler<EventArgs<string>> ShellCustomPanel1Updated;
        /// <summary>
        /// Occurs when [shell custom panel2 updated].
        /// </summary>
        [EventPublication(EventNames.Shell_CustomPanel2Updated, PublicationScope.Global)]
        private event EventHandler<EventArgs<string>> ShellCustomPanel2Updated;
        /// <summary>
        /// Occurs when [shell progress bar changed].
        /// </summary>
        [EventPublication(EventNames.Shell_ProgressBarChanged, PublicationScope.Global)]
        private event EventHandler<EventArgs<int>> ShellProgressBarChanged;

        #endregion

        public SmartClientEnvironment()
        {
        }

        #region Members

        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <value>The application path.</value>
        public string ApplicationPath {
            get {
                return FileUtility.GetParent(FileUtility.ApplicationRootPath);
            }
        }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        /// <value>The current user.</value>
        public UserInfo CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value;}
        }

        #endregion

        #region Event methods

        /// <summary>
        /// Changes the shell status text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void ChangeShellStatus(string text)
        {
            if (ShellStatusUpdated != null) { 
                ShellStatusUpdated(this, new EventArgs<string>(text));
            }
        }

        /// <summary>
        /// Changes the shell custom panel1's text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void ChangeShellCustomPanel1(string text)
        {
            if (ShellCustomPanel1Updated != null) {
                ShellCustomPanel1Updated(this, new EventArgs<string>(text));
            }
        }

        /// <summary>
        /// Changes the shell custom panel2.
        /// </summary>
        /// <param name="text">The text.</param>
        public void ChangeShellCustomPanel2(string text)
        {
            if (ShellCustomPanel2Updated != null) {
                ShellCustomPanel2Updated(this, new EventArgs<string>(text));
            }
        }

        /// <summary>
        /// Changes the shell progress bar.
        /// </summary>
        /// <param name="position">The position.</param>
        public void ChangeShellProgressBar(int position)
        {
            if (ShellProgressBarChanged != null) {
                ShellProgressBarChanged(this, new EventArgs<int>(position));
            }
        }
        #endregion
    }
}
