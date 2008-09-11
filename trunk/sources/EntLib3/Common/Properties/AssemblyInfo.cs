//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright  Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System.Reflection;
using System.Security.Permissions;
using System.Runtime.ConstrainedExecution;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;

[assembly : SecurityPermission(SecurityAction.RequestMinimum)]
[assembly : ReliabilityContract(Consistency.WillNotCorruptState, Cer.None)]

[assembly : AssemblyTitle("Enterprise Library Shared Library")]
[assembly : AssemblyDescription("Enterprise Library Shared Library")]
[assembly: AssemblyVersion("3.1.0.0")]

[assembly: Instrumented(@"root\EnterpriseLibrary")]

[assembly: ReflectionPermission(SecurityAction.RequestMinimum)]

[assembly: InternalsVisibleTo("Microsoft.Practices.EnterpriseLibrary.Common.Tests")]
[assembly: InternalsVisibleTo("Microsoft.Practices.EnterpriseLibrary.Logging.Tests")]
[assembly: InternalsVisibleTo("Microsoft.Practices.EnterpriseLibrary.SqlConfigurationSource.Tests")]
[assembly: InternalsVisibleTo("Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Tests")]
