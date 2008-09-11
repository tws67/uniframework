//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm
{
	/// <summary>
	/// Represents the contents of an ADM template file.
	/// </summary>
	public class AdmContent
	{
		private List<AdmCategory> categories;

		/// <summary>
		/// Initializes a new empty instance of the <see cref="AdmContentBuilder"/> class.
		/// </summary>
		protected internal AdmContent()
		{
			this.categories = new List<AdmCategory>();
		}

		/// <summary>
		/// Writes the contents represented by the receiver to <paramref name="writer"/>.
		/// </summary>
		/// <param name="writer">The <see cref="TextWriter"/> to where the contents should be written.</param>
		public void Write(TextWriter writer)
		{
			foreach (AdmCategory category in categories)
			{
				category.Write(writer);
			}
		}

		internal void AddCategory(AdmCategory category)
		{
			categories.Add(category);
		}

		internal IEnumerable<AdmCategory> Categories
		{
			get { return categories; }
		}
	}
}
