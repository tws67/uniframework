using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 动态排序工具类
    /// </summary>
    [Serializable]
    public class DynamicComparer<T> : IComparer<T>
    {
        /// <summary>
        /// 比较两个对象
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public int Compare(T x, T y)
        {
            Type type = typeof(T);

            for (int i = 0; i < sortProperties.Length; i++)
            {
                PropertyInfo pi = type.GetProperty(sortProperties[i].Name);

                object pvalx = pi.GetValue(x, null);
                object pvaly = pi.GetValue(y, null);

                if (pvalx == null && pvaly == null) return 0;
                if (pvalx == null && pvaly != null) return -1;
                if (pvalx != null && pvaly == null) return 1;

                IComparable xc = (IComparable)pvalx;
                int iResult = xc.CompareTo(pvaly);
                if (iResult != 0)
                {
                    if (sortProperties[i].Descending)
                    {
                        return -iResult;
                    }
                    else
                    {
                        return iResult;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Parses the order by.
        /// </summary>
        /// <param name="orderBy">The order by.</param>
        /// <returns></returns>
        protected SortProperty[] ParseOrderBy(string orderBy)
        {
            if (orderBy == null || orderBy.Length == 0)
                throw new ArgumentNullException("排序字符串不能为空");

            string[] props = orderBy.Split(',');
            SortProperty[] sortProps = new SortProperty[props.Length];
            string prop;
            bool descending;

            for (int i = 0; i < props.Length; i++)
            {
                descending = false;
                prop = props[i].Trim();

                if (prop.ToUpper().EndsWith(" DESC"))
                {
                    descending = true;
                    prop = prop.Substring(0, prop.ToUpper().LastIndexOf(" DESC"));
                }
                else if (prop.ToUpper().EndsWith(" ASC"))
                {
                    prop = prop.Substring(0, prop.ToUpper().LastIndexOf(" ASC"));
                }

                prop = prop.Trim();

                sortProps[i] = new SortProperty(prop, descending);
            }

            return sortProps;
        }

        /// <summary>
        /// Checks the sort properties.
        /// </summary>
        /// <param name="sortProperties">The sort properties.</param>
        private void CheckSortProperties(SortProperty[] sortProperties)
        {
            Type instanceType = typeof(T);

            if (!instanceType.IsPublic)
                throw new ArgumentException(string.Format("Type \"{0}\" is not public.", typeof(T).FullName));

            foreach (SortProperty sProp in sortProperties)
            {
                PropertyInfo pInfo = instanceType.GetProperty(sProp.Name);

                if (pInfo == null)
                    throw new ArgumentException(string.Format("No public property named \"{0}\" was found.", sProp.Name));

                if (!pInfo.CanRead)
                    throw new ArgumentException(string.Format("The property \"{0}\" is write-only.", sProp.Name));
            }
        }

        /// <summary>
        /// Initializes the specified sort properties.
        /// </summary>
        /// <param name="sortProperties">The sort properties.</param>
        private void Initialize(SortProperty[] sortProperties)
        {
            CheckSortProperties(sortProperties);
            this.sortProperties = sortProperties;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicComparer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="orderBy">The order by.</param>
        public DynamicComparer(string orderBy)
        {
            Initialize(ParseOrderBy(orderBy));
        }

        private SortProperty[] sortProperties;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct SortProperty
    {
        #region Properties

        private string name;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private bool descending;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SortProperty"/> is descending.
        /// </summary>
        /// <value><c>true</c> if descending; otherwise, <c>false</c>.</value>
        public bool Descending
        {
            get { return descending; }
            set { descending = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SortProperty"/> struct.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public SortProperty(string propertyName)
        {
            this.name = propertyName;
            this.descending = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortProperty"/> struct.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="sortDescending">if set to <c>true</c> [sort descending].</param>
        public SortProperty(string propertyName, bool sortDescending)
        {
            this.name = propertyName;
            this.descending = sortDescending;
        }

        #endregion
    }
}
