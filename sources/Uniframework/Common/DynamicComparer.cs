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
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("A property cannot have an empty name.", "value");

                name = value.Trim();
            }
        }

        private bool descending;
        public bool Descending
        {
            get
            {
                return descending;
            }
            set
            {
                descending = value;
            }
        }

        public static bool IsComparable(Type valueType)
        {
            bool isNullable;
            return IsComparable(valueType, out isNullable);
        }

        public static bool IsComparable(Type valueType, out bool isNullable)
        {
            isNullable = valueType.IsGenericType
                    && !valueType.IsGenericTypeDefinition
                    && valueType.IsAssignableFrom(typeof(Nullable<>).MakeGenericType(valueType.GetGenericArguments()[0]));

            return (typeof(IComparable).IsAssignableFrom(valueType)
                    || typeof(IComparable<>).MakeGenericType(valueType).IsAssignableFrom(valueType)
                    || isNullable);
        }

        #region Internals
        private Type valueType;
        internal Type ValueType
        {
            get
            {
                return valueType;
            }
            private set
            {
                valueType = value;

                if (!IsComparable(value, out isNullable))
                {
                    throw new NotSupportedException("The type \""
                        + value.FullName
                        + "\" of the property \""
                        + this.Name
                        + "\" does not implement IComparable, IComparible<T> or is Nullable<T>.");
                }
            }
        }

        private MethodInfo get;
        internal MethodInfo Get
        {
            get
            {
                return get;
            }
            private set
            {
                get = value;
            }
        }

        private FieldInfo field;
        internal FieldInfo Field
        {
            get
            {
                return field;
            }
            private set
            {
                field = value;
            }
        }

        private bool isNullable;
        internal bool IsNullable
        {
            get
            {
                return isNullable;
            }
        }
        #endregion
        #endregion

        #region Static methods
        public static SortProperty[] ParseOrderBy(string orderBy)
        {
            if (orderBy == null)
                throw new ArgumentException("The orderBy clause may not be null.", "orderBy");

            string[] properties = orderBy.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            SortProperty[] sortProperties = new SortProperty[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                bool descending = false;
                string property = properties[i].Trim();
                string[] propertyElements = property.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (propertyElements.Length > 1)
                {
                    if (propertyElements[1].Equals("DESC", StringComparison.OrdinalIgnoreCase))
                    {
                        descending = true;
                    }
                    else if (propertyElements[1].Equals("ASC", StringComparison.OrdinalIgnoreCase))
                    {
                        // already set to descending = false;
                    }
                    else
                    {
                        throw new ArgumentException("Unexpected sort order type \"" + propertyElements[1] + "\" for \"" + propertyElements[0] + "\"", "orderBy");
                    }
                }

                sortProperties[i] = new SortProperty(propertyElements[0], descending);
            }

            return sortProperties;
        }
        #endregion

        #region Constructors
        public SortProperty(string propertyName, bool sortDescending)
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("A property cannot have an empty name.", "propertyName");

            name = propertyName;
            descending = sortDescending;

            // we set these when accessor validated
            valueType = null;
            get = null;
            field = null;
            isNullable = false;
        }
        #endregion

        #region Internals
        internal static void BindSortProperties(SortProperty[] sortProperties, Type instanceType)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            if (sortProperties == null)
                sortProperties = new SortProperty[0];

            if (sortProperties.Length > 0)
            {
                for (int index = 0; index < sortProperties.Length; index++)
                {
                    string propertyName = sortProperties[index].Name;
                    PropertyInfo propertyInfo = instanceType.GetProperty(propertyName, BindingFlags.GetProperty | flags);

                    if (propertyInfo != null)
                    {
                        sortProperties[index].ValueType = propertyInfo.PropertyType;
                        sortProperties[index].Get = propertyInfo.GetGetMethod(true);
                    }
                    else
                    {
                        FieldInfo fieldInfo = instanceType.GetField(propertyName, BindingFlags.GetField | flags);

                        if (fieldInfo != null)
                        {
                            sortProperties[index].ValueType = fieldInfo.FieldType;
                            sortProperties[index].Field = fieldInfo;
                        }
                        else
                        {
                            throw new ArgumentException("No public property or field named \""
                                + propertyName
                                + "\" was found in type \""
                                + instanceType.FullName
                                + "\".");
                        }
                    }
                }
            }
            else
            {
                if (!IsComparable(instanceType))
                    throw new NotSupportedException("The type \""
                        + instanceType.FullName
                        + "\" does not implement IComparable, IComparable<T> nor is a Nullable<T>.");
            }
        }
        #endregion
    }
}
