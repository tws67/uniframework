//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright ? Microsoft Corporation. All rights reserved.
// Adapted from ACA.NET with permission from Avanade Inc.
// ACA.NET copyright ? Avanade Inc. All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System.Reflection;
using System.Security.Permissions;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Messaging;

[assembly: SecurityPermission(SecurityAction.RequestMinimum)]

[assembly: AssemblyTitle("Enterprise Library Logging Application Block")]
[assembly: AssemblyDescription("Enterprise Library Logging Application Block")]
[assembly: AssemblyVersion("3.1.0.0")]

[assembly: Instrumented(@"root\EnterpriseLibrary")]

[assembly: InternalsVisibleTo("Microsoft.Practices.EnterpriseLibrary.Logging.Tests")]
