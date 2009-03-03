﻿//----------------------------------------------------------------------------------------
// patterns & practices - Smart Client Software Factory - Guidance Package
//
// This file was generated by this guidance package as part of the solution template
//
// The IProfileCatalogService interface abstract the implementation to retrieve the 
// profile catalog from a given Url
// 
// For more information see: 
// ms-help://MS.VSCC.v80/MS.VSIPCC.v80/ms.scsf.2006jun/SCSF/html/03-210-Creating%20a%20Smart%20Client%20Solution.htm
//
// Latest version of this Guidance Package: http://go.microsoft.com/fwlink/?LinkId=62182
//----------------------------------------------------------------------------------------

using System;
namespace Microsoft.Practices.CompositeUI.Common
{
    public interface IProfileCatalogService
    {
        string GetProfileCatalog(string[] roles);
        string Url { get; set; }
    }
}
