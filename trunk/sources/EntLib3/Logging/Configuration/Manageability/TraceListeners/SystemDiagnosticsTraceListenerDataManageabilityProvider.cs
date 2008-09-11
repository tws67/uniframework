//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;

using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.TraceListeners
{
	internal class SystemDiagnosticsTraceListenerDataManageabilityProvider
		: BasicCustomTraceListenerDataManageabilityProvider<SystemDiagnosticsTraceListenerData>
	{
		public SystemDiagnosticsTraceListenerDataManageabilityProvider()
		{ }

		protected sealed override NamedConfigurationSetting CreateSetting(SystemDiagnosticsTraceListenerData data, String[] attributes)
		{
			return new CustomTraceListenerSetting(data.Name, 
				data.Type.AssemblyQualifiedName, 
				data.InitData, 
				attributes, 
				data.TraceOutputOptions.ToString(),
				null);
		}
	}
}
