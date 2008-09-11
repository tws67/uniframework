//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Exception Handling Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.Manageability
{
	internal class CustomHandlerDataManageabilityProvider
		: CustomProviderDataManageabilityProvider<CustomHandlerData>
	{
		public new const String ProviderTypePropertyName = CustomProviderDataManageabilityProvider<CustomHandlerData>.ProviderTypePropertyName;
		public new const String AttributesPropertyName = CustomProviderDataManageabilityProvider<CustomHandlerData>.AttributesPropertyName;

		public CustomHandlerDataManageabilityProvider()
			: base("")
		{ }

		protected override void AddAdministrativeTemplateDirectives(AdmContentBuilder contentBuilder,
			CustomHandlerData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			// directives are parts of the exception type policy
			AddElementAdministrativeTemplateParts(contentBuilder,
				configurationObject,
				configurationSource,
				elementPolicyKeyName);
		}

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder,
			CustomHandlerData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			contentBuilder.AddTextPart(String.Format(CultureInfo.CurrentCulture,
													Resources.HandlerPartNameTemplate,
													configurationObject.Name));

			contentBuilder.AddEditTextPart(Resources.CustomHandlerTypePartName,
				elementPolicyKeyName,
				ProviderTypePropertyName,
				configurationObject.Type.AssemblyQualifiedName,
				1024,
				true);

			contentBuilder.AddEditTextPart(Resources.CustomHandlerAttributesPartName,
				elementPolicyKeyName,
				AttributesPropertyName,
				GenerateAttributesString(configurationObject.Attributes),
				1024,
				false);
		}

		protected override NamedConfigurationSetting CreateSetting(CustomHandlerData data,
			String[] attributes)
		{
			return new CustomHandlerSetting(data.Name, data.Type.AssemblyQualifiedName, attributes);
		}
	}
}
