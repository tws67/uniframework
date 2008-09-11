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
using System.Globalization;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm
{
	/// <summary>
	/// Represents a DROPDOWNLIST part on an ADM template.
	/// </summary>
	public class AdmDropDownListPart : AdmPart
	{
		internal const String DropDownListTemplate = "\t\t\tDROPDOWNLIST";
		internal const String ItemListStartTemplate = "\t\t\tITEMLIST";
		internal const String ListItemTemplate = "\t\t\t\tNAME \"{0}\" VALUE \"{1}\"";
		internal const String DefaultListItemTemplate = "\t\t\t\tNAME \"{0}\" VALUE \"{1}\" DEFAULT";
		internal const String ItemListEndTemplate = "\t\t\tEND ITEMLIST";

		private IEnumerable<AdmDropDownListItem> items;
		private String defaultValue;

		internal AdmDropDownListPart(String partName, String keyName, String valueName,
			IEnumerable<AdmDropDownListItem> items, String defaultValue)
			: base(partName, keyName, valueName)
		{
			this.items = items;
			this.defaultValue = defaultValue;
		}

		/// <summary>
		/// Writes the text representing the part to the <paramref name="writer"/>.
		/// </summary>
		/// <param name="writer">The <see cref="System.IO.TextWriter"/> to where the text for the part should be written to.</param>
		protected override void WritePart(System.IO.TextWriter writer)
		{
			base.WritePart(writer);

			writer.WriteLine(ItemListStartTemplate);
			foreach (AdmDropDownListItem item in items)
			{
				if (item.Name.Equals(defaultValue))
				{
					writer.WriteLine(String.Format(CultureInfo.InvariantCulture, DefaultListItemTemplate, item.Name, item.Value));
				}
				else
				{
					writer.WriteLine(String.Format(CultureInfo.InvariantCulture, ListItemTemplate, item.Name, item.Value));
				}
			}
			writer.WriteLine(ItemListEndTemplate);
		}

		/// <summary>
		/// Gets the default value for the part.
		/// </summary>
		public String DefaultValue
		{
			get { return defaultValue; }
		}

		/// <summary>
		/// Gets the list of name/value pairs for the part.
		/// </summary>
		public IEnumerable<AdmDropDownListItem> Items
		{
			get { return items; }
		}

		/// <summary>
		/// Gest the template representing the type of the part.
		/// </summary>
		protected override string PartTypeTemplate
		{
			get { return DropDownListTemplate; }
		}
	}
}
