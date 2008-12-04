using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Uniframework.Client
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "WebServiceSoap", Namespace = "http://tempuri.org/")]
    public partial class WebServiceChannel : SoapHttpClientProtocol, ICommunicationChannel
    {
        private System.Threading.SendOrPostCallback WebInvokeOperationCompleted;
        private bool useDefaultCredentialsSetExplicitly;

        /// <remarks/>
        public WebServiceChannel()
        {
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        #region WebServiceChannel Members

        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true)
                            && (this.useDefaultCredentialsSetExplicitly == false))
                            && (this.IsLocalFileSystemWebService(value) == false)))
                {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        #endregion

        //protected override System.Net.WebRequest GetWebRequest(Uri uri)
        //{
        //    HttpWebRequest webRequest = (HttpWebRequest)base.GetWebRequest(uri);
        //    webRequest.KeepAlive = false;
        //    return webRequest;
        //}

        /// <remarks/>
        [SoapDocumentMethodAttribute("http://tempuri.org/WebInvoke", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElementAttribute(DataType = "base64Binary")]
        public byte[] WebInvoke([XmlElementAttribute(DataType = "base64Binary")] byte[] data)
        {
            try {
                object[] results = this.Invoke("WebInvoke", new object[] { data } );
                return ((byte[])(results[0]));
            }
            catch (SoapException ex) {
                throw ExceptionUtility.UnWrapException<Exception>(ex);
            }
        }

        /// <remarks/>
        public event WebInvokeCompletedEventHandler WebInvokeCompleted;

        /// <remarks/>
        public void WebInvokeAsync(byte[] data)
        {
            this.WebInvokeAsync(data, null);
        }

        /// <remarks/>
        public void WebInvokeAsync(byte[] data, object userState)
        {
            if ((this.WebInvokeOperationCompleted == null)) {
                this.WebInvokeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnWebInvokeOperationCompleted);
            }
            this.InvokeAsync("WebInvoke", new object[] {
                        data}, this.WebInvokeOperationCompleted, userState);
        }

        private void OnWebInvokeOperationCompleted(object arg)
        {
            if ((this.WebInvokeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.WebInvokeCompleted(this, new WebInvokeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (((url == null) || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                && (string.Compare(wsUri.Host, "localhost", System.StringComparison.OrdinalIgnoreCase) == 0)))
            {
                return true;
            }
            return false;
        }

        #region ICommunicationChannel Members

        public byte[] Invoke(byte[] data)
        {
            return this.WebInvoke(data);
        }

        #endregion
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.42")]
    public delegate void WebInvokeCompletedEventHandler(object sender, WebInvokeCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class WebInvokeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        private object[] results;

        internal WebInvokeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public byte[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
}
