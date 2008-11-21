using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.ObjectBuilder;

namespace Uniframework.DemoCenter.Client.Views
{
    public partial class SampleView : DevExpress.XtraEditors.XtraUserControl
    {
        public SampleView()
        {
            InitializeComponent();
        }

        private SamplePresenter presenter;
        [CreateNew]
        public SamplePresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        public void SetSampleEvent(string txtStr)
        {
            labSamleEvent.Text = "服务器时间 : " + txtStr;
        }

        private void btnHello_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show(Presenter.Hello(txtName.Text));
        }

        private void btnHelloOffline_Click(object sender, EventArgs e)
        {
            Presenter.HelloOffline(txtName.Text);
        }

        private void btnHelloCache_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show(Presenter.Hello4Cache(txtName.Text));
        }
    }
}
