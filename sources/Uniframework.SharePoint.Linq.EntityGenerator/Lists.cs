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
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Services.Description;
using System.Xml;

#endregion

/// <summary>
/// Web service proxy for the SharePoint lists web service.
/// </summary>
[WebServiceBindingAttribute(Name = "ListsSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
internal class Lists : SoapHttpClientProtocol
{
    [SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/GetList", RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/", ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
    public XmlNode GetList(string listName)
    {
        object[] results = this.Invoke("GetList", new object[] { listName });
        return (XmlNode)(results[0]);
    }
}