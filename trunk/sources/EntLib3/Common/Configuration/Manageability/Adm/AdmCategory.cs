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
using System.Globalization;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm
{
	/// <summary>
	/// Represents a category on an ADM template file.
	/// </summary>
	public class AdmCategory
	{
		internal const String CategoryStartTemplate = "CATEGORY \"{0}\"";
		internal const String CategoryEndTemplate = "END CATEGORY\t; \"{0}\"";

		private List<AdmCategory> categories;
		private List<AdmPolicy> policies;
		private String name;

		internal AdmCategory(String categoryName)
		{
			this.name = categoryName;

			this.categories = new List<AdmCategory>();
			this.policies = new List<AdmPolicy>();
		}

		internal void AddCategory(AdmCategory category)
		{
			this.categories.Add(category);
		}

		internal void AddPolicy(AdmPolicy policy)
		{
			this.policies.Add(policy);
		}

		internal void Write(TextWriter writer)
		{
			writer.WriteLine(String.Format(CultureInfo.InvariantCulture, CategoryStartTemplate, name));
			foreach (AdmCategory category in categories)
			{
				category.Write(writer);
			}
			foreach (AdmPolicy policy in policies)
			{
				policy.Write(writer);
			}
			writer.WriteLine(String.Format(CultureInfo.InvariantCulture, CategoryEndTemplate, name));
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public String Name
		{
			get { return name; }
		}

		/// <summary>
		/// Gest the list of sub categories.
		/// </summary>
		public IEnumerable<AdmCategory> Categories
		{
			get { return categories; }
		}

		/// <summary>
		/// Gets the list of policies in a category.
		/// </summary>
		public IEnumerable<AdmPolicy> Policies
		{
			get { return policies; }
		}
	}
}
