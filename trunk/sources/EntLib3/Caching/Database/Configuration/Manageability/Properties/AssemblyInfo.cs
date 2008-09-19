//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Caching Application Block
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
using System.Management.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Caching.Database.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Caching.Database.Configuration.Manageability;

[assembly: Instrumented(@"root\EnterpriseLibrary")]
[assembly: InternalsVisibleTo("EnterpriseLibrary.Caching.Database.Configuration.Manageability.Tests")]

[assembly: ConfigurationElementManageabilityProvider(typeof(DataCacheStorageDataManageabilityProvider), typeof(DataCacheStorageData), typeof(CacheManagerSettingsManageabilityProvider))]