using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.Security;

namespace Uniframework.Common.WorkItems.Authorization
{
    public partial class CommandView : DevExpress.XtraEditors.XtraUserControl
    {
        public CommandView()
        {
            InitializeComponent();
        }

        #region Dependency Services

        private CommandPresenter presenter;
        [CreateNew]
        public CommandPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        #endregion


        public AuthorizationNode AuthNode
        {
            get;
            set;
        }

        private void edtImage_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog() { 
                CheckFileExists = true,
                Filter = "�����ļ�(*.*)|*.*",
                Title = "ѡ��"
            };
            if (diag.ShowDialog() == DialogResult.OK) {
                edtImage.Text = diag.FileName;
                string imagefile = Path.GetFileNameWithoutExtension(diag.FileName);
                image.ContentImage = Presenter.ImageService.GetBitmap("${" + imagefile + "}", new Size(32, 32));
            }
            
            // ���ͼ��༭��Ϊ���򲻸�ֵ
            if (String.IsNullOrEmpty(edtImage.Text) || edtImage.Text.Length == 0)
                image.ContentImage = null;
        }

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (AuthNode == null) // ����ģ��ı��湤��
            {
                Presenter.AuthorizationStoreService.DeleteCommand(edtCommandUri.Text);
                AuthorizationCommand command = new AuthorizationCommand() {
                    Name = edtName.Text,
                    CommandUri = edtCommandUri.Text,
                    Category = edtCategory.Text
                };
                if (!String.IsNullOrEmpty(edtImage.Text) || edtImage.Text.Length != 0)
                    command.Image = "${" + Path.GetFileNameWithoutExtension(edtImage.Text) + "}";


                try {
                    Presenter.AuthorizationStoreService.SaveCommand(command);
                }
                catch (Exception ex) {
                    XtraMessageBox.Show("�������� \"" + edtName.Text + "\" ʱʧ�ܣ�" + ex.Message);
                    btnOK.DialogResult = DialogResult.None;
                }
            }
            else {
                AuthorizationCommand command = new AuthorizationCommand() { 
                    Name = edtName.Text,
                    CommandUri = edtCommandUri.Text,
                    Category = edtCategory.Text
                };
                if (!String.IsNullOrEmpty(edtImage.Text) || edtImage.Text.Length != 0)
                    command.Image = "${" + Path.GetFileNameWithoutExtension(edtImage.Text) + "}";

                foreach (AuthorizationCommand cmd in AuthNode.Commands) {
                    // ɾ���б���ԭ���Ĳ�����
                    if (cmd.Name == command.Name || cmd.CommandUri == command.CommandUri) {
                        AuthNode.RemoveCommand(cmd);
                        break;
                    }
                }

                // �����µĲ���
                AuthNode.AddCommand(command);
                try {
                    Presenter.AuthorizationStoreService.Save(AuthNode);
                }
                catch (Exception ex) {
                    XtraMessageBox.Show("������� \"" + edtName.Text + "\" ʱ��ʧ�ܣ�" + ex.Message);
                    btnOK.DialogResult = DialogResult.None;
                }
            }
        }

        private void edtName_Leave(object sender, EventArgs e)
        {
            labPreview.Text = edtName.Text;
        }

        /// <summary>
        /// �����б��е�ǰ����仯�¼�
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.EventArgs&lt;Uniframework.Security.AuthorizationCommand&gt;"/> instance containing the event data.</param>
        [EventSubscription(EventNames.Authorization_CurrentCommandChanged, ThreadOption.UserInterface)]
        public void OnCurrentCommandChanged(object sender, EventArgs<AuthorizationCommand> e)
        {
            edtName.Text = e.Data.Name;
            edtCommandUri.Text = e.Data.CommandUri;
            edtCategory.Text = e.Data.Category;
            edtImage.Text = e.Data.Image;

            // ��ʾͼ��
            if (!String.IsNullOrEmpty(edtImage.Text))
                image.ContentImage = Presenter.ImageService.GetBitmap(edtImage.Text);
        }

        /// <summary>
        /// Bindings the command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void BindingCommand(AuthorizationCommand command)
        {
            edtName.Text = command.Name;
            edtCommandUri.Text = command.CommandUri;
            edtCategory.Text = command.Category;
            edtImage.Text = command.Image;

            if (!String.IsNullOrEmpty(edtImage.Text))
                image.ContentImage = Presenter.ImageService.GetBitmap(edtImage.Text);
        }

        private void edtImage_Leave(object sender, EventArgs e)
        {
            edtImage_ButtonClick(this, null);
        }
    }
}
