/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Diagnostics;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Represents a URL used in a SharePoint list. Stores a Uri together with a friendly name that indicates the title of the URL.
    /// </summary>
    [Serializable]
    public sealed class Url
    {
        #region Private members

        /// <summary>
        /// Url address.
        /// </summary>
        private string _address;

        /// <summary>
        /// Url description.
        /// </summary>
        private string _description;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor for factory method.
        /// </summary>
        internal Url()
        {
        }

        /// <summary>
        /// Creates a new Url instance based on a given URI and a description.
        /// </summary>
        /// <param name="address">URL to refer to.</param>
        /// <param name="description">Description for the URL.</param>
        public Url(string address, string description)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            if (!Uri.IsWellFormedUriString(address, UriKind.Absolute))
                throw RuntimeErrors.InvalidUriSpecified();

            _address = address;
            _description = description ?? "";
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Creates a new Url instance based on a given textual representation of a SharePoint URL field value.
        /// </summary>
        /// <param name="fieldValue">Internal textual representation value of a URL field in SharePoint.</param>
        public static Url Parse(string fieldValue)
        {
            Debug.Assert(fieldValue != null);

            int i;
            for (i = 0; i < fieldValue.Length; i++)
            {
                //
                // Search for commas.
                //
                if (fieldValue[i] == ',')
                {
                    //
                    // If this character is the last one, trim it.
                    //
                    if (i == fieldValue.Length - 1)
                        fieldValue = fieldValue.Substring(0, fieldValue.Length - 1);
                    else
                    {
                        //
                        // Check whether the next character is a space. This indicates the gap between url and description.
                        //
                        if (i + 1 < fieldValue.Length && fieldValue[i + 1] == ' ')
                            break;

                        //
                        // If no space was found, skip the next character (should be a double comma) and move on.
                        //
                        i++;
                    }
                }
            }

            Url result = new Url();

            //
            // If stopped before reaching the end of the input, we have a composed field with url and description.
            //
            if (i < fieldValue.Length)
            {
                result._address = fieldValue.Substring(0, i).Replace(",,", ",");
                i += 2;
                if (i < fieldValue.Length)
                    result._description = fieldValue.Substring(i, fieldValue.Length - i);
            }
            //
            // If not, url and description are defined to be the same.
            //
            else
            {
                result._address = fieldValue;
                result._description = fieldValue;
            }

            return result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Url's description.
        /// </summary>
        /// <remarks>Only a getter is supplied; when updating entities one will have to create a new Url instance in order for change tracking to work.</remarks>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Gets the Url being referred to.
        /// </summary>
        /// <remarks>Only a getter is supplied; when updating entities one will have to create a new Url instance in order for change tracking to work.</remarks>
        public string Address
        {
            get { return _address; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a hash code for the Url instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _address.GetHashCode() ^ _description.GetHashCode();
        }

        /// <summary>
        /// Checks whether the given object is equal to the Url instance.
        /// </summary>
        /// <param name="obj">Object to be compared to the current instance.</param>
        /// <returns>true if the objects are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            Url u = obj as Url;

            if (u == null)
                return false;
            else if (object.ReferenceEquals(this, u))
                return true;
            else
                return u.Description == _description && u.Address == _address;
        }

        /// <summary>
        /// Gets a friendly string representation of the URL including the friendly name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _address + ", " + _description;
        }

        #endregion
    }
}
