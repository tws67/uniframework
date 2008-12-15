/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

#region Namespace imports

using System;
using System.ComponentModel;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;

#endregion

[WebServiceBindingAttribute(Name = "ListsSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
public class Lists : SoapHttpClientProtocol
{
    private SendOrPostCallback GetListOperationCompleted;
    private SendOrPostCallback GetListAndViewOperationCompleted;
    private SendOrPostCallback GetListCollectionOperationCompleted;

    public event GetListCompletedEventHandler GetListCompleted;
    public event GetListAndViewCompletedEventHandler GetListAndViewCompleted;
    public event GetListCollectionCompletedEventHandler GetListCollectionCompleted;

    public delegate void GetListCompletedEventHandler(object sender, GetListCompletedEventArgs e);
    public delegate void GetListAndViewCompletedEventHandler(object sender, GetListAndViewCompletedEventArgs e);
    public delegate void GetListCollectionCompletedEventHandler(object sender, GetListCollectionCompletedEventArgs e);

    [SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/GetList", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    public XmlNode GetList(string listName)
    {
        object[] results = this.Invoke("GetList", new object[] { listName });
        return (XmlNode)results[0];
    }

    public void GetListAsync(string listName)
    {
        this.GetListAsync(listName, null);
    }

    public void GetListAsync(string listName, object userState)
    {
        if (this.GetListOperationCompleted == null)
            this.GetListOperationCompleted = new SendOrPostCallback(this.OnGetListOperationCompleted);

        this.InvokeAsync("GetList", new object[] { listName }, this.GetListOperationCompleted, userState);
    }

    private void OnGetListOperationCompleted(object arg)
    {
        if (this.GetListCompleted != null)
        {
            InvokeCompletedEventArgs invokeArgs = (InvokeCompletedEventArgs)(arg);
            this.GetListCompleted(this, new GetListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    [SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/GetListAndView", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    public XmlNode GetListAndView(string listName, string viewName)
    {
        object[] results = this.Invoke("GetListAndView", new object[] { listName, viewName });
        return (XmlNode)results[0];
    }

    public void GetListAndViewAsync(string listName, string viewName)
    {
        this.GetListAndViewAsync(listName, viewName, null);
    }

    public void GetListAndViewAsync(string listName, string viewName, object userState)
    {
        if (this.GetListAndViewOperationCompleted == null)
            this.GetListAndViewOperationCompleted = new SendOrPostCallback(this.OnGetListAndViewOperationCompleted);

        this.InvokeAsync("GetListAndView", new object[] { listName, viewName }, this.GetListAndViewOperationCompleted, userState);
    }

    private void OnGetListAndViewOperationCompleted(object arg)
    {
        if (this.GetListAndViewCompleted != null)
        {
            InvokeCompletedEventArgs invokeArgs = (InvokeCompletedEventArgs)(arg);
            this.GetListAndViewCompleted(this, new GetListAndViewCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    [SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/GetListCollection", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    public XmlNode GetListCollection()
    {
        object[] results = this.Invoke("GetListCollection", new object[0]);
        return (XmlNode)results[0];
    }

    public void GetListCollectionAsync()
    {
        this.GetListCollectionAsync(null);
    }

    public void GetListCollectionAsync(object userState)
    {
        if (this.GetListCollectionOperationCompleted == null)
            this.GetListCollectionOperationCompleted = new SendOrPostCallback(this.OnGetListCollectionOperationCompleted);

        this.InvokeAsync("GetListCollection", new object[0], this.GetListCollectionOperationCompleted, userState);
    }

    private void OnGetListCollectionOperationCompleted(object arg)
    {
        if (this.GetListCollectionCompleted != null)
        {
            InvokeCompletedEventArgs invokeArgs = (InvokeCompletedEventArgs)(arg);
            this.GetListCollectionCompleted(this, new GetListCollectionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    public new void CancelAsync(object userState)
    {
        base.CancelAsync(userState);
    }
}

public class GetListCompletedEventArgs : AsyncCompletedEventArgs
{
    private object[] results;

    internal GetListCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
        base(exception, cancelled, userState)
    {
        this.results = results;
    }

    public XmlNode Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return (XmlNode)this.results[0];
        }
    }
}

public class GetListAndViewCompletedEventArgs : AsyncCompletedEventArgs
{
    private object[] results;

    internal GetListAndViewCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
        base(exception, cancelled, userState)
    {
        this.results = results;
    }

    public XmlNode Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return (XmlNode)this.results[0];
        }
    }
}

public class GetListCollectionCompletedEventArgs : AsyncCompletedEventArgs
{
    private object[] results;

    internal GetListCollectionCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
        base(exception, cancelled, userState)
    {
        this.results = results;
    }

    public XmlNode Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return (XmlNode)this.results[0];
        }
    }
}