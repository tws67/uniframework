//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Security Application Block
//===============================================================================
// Copyright  Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Security.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Security.Cache.CachingStore.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Security.Cache.CachingStore.Configuration;
using System.Management.Instrumentation;

[assembly: Instrumented(@"root\EnterpriseLibrary")]
[assembly: InternalsVisibleTo("EnterpriseLibrary.Security.Cache.CachingStore.Configuration.Manageability.Tests")]

[assembly: ConfigurationElementManageabilityProvider(typeof(CachingStoreProviderDataManageabilityProvider), typeof(CachingStoreProviderData), typeof(SecuritySettingsManageabilityProvider))]