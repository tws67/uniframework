//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright � Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm
{
	/// <summary>
	/// Represents a TEXT part in an ADM template.
	/// </summary>
	public class AdmTextPart : AdmPart
	{
		internal const String TextTemplate = "\t\t\tTEXT";

		internal AdmTextPart(String partName)
			: base(partName, null, null)
		{ }

		/// <summary>
		/// Gest the template representing the type of the part.
		/// </summary>
		protected override string PartTypeTemplate
		{
			get { return TextTemplate; }
		}
	}
}
