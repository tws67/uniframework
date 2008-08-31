using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Uniframework.SmartClient.Constants;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// This interface flags an object beeing "mementocapable". This means that the
    /// state of the object could be saved to an <see cref="Properties"/> object
    /// and set from a object from the same class.
    /// This is used to save and restore the state of GUI objects.
    /// </summary>
    public interface IMementoCapable
    {
        /// <summary>
        /// Creates a new memento from the state.
        /// </summary>
        Properties CreateMemento();

        /// <summary>
        /// Sets the state to the given memento.
        /// </summary>
        void SetMemento(Properties memento);
    }

    /// <summary>
    /// 属性集
    /// </summary>
    public class Properties
    {
        /// <summary> Needed for support of late deserialization </summary>
        class SerializedValue
        {
            string content;

            public string Content
            {
                get { return content; }
            }

            public T Deserialize<T>()
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(new StringReader(content));
            }

            public SerializedValue(string content)
            {
                this.content = content;
            }
        }

        private Dictionary<string, object> properties = new Dictionary<string, object>();
        private readonly static object syncObj = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="Properties"/> class.
        /// </summary>
        public Properties()
        { }

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified property.
        /// </summary>
        /// <value></value>
        public string this[string property]
        {
            get
            {
                return Convert.ToString(Get(property));
            }
            set
            {
                Set(property, value);
            }
        }

        /// <summary>
        /// Gets the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public object Get(string property)
        {
            lock (syncObj) {
                object val;
                properties.TryGetValue(property, out val);
                return val;
            }
        }

        /// <summary>
        /// Gets the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public T Get<T>(string property, T defaultValue)
        {
            lock (syncObj)
            {
                object o;
                if (!properties.TryGetValue(property, out o))
                {
                    properties.Add(property, defaultValue);
                    return defaultValue;
                }

                if (o is string && typeof(T) != typeof(string))
                {
                    TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
                    try
                    {
                        o = c.ConvertFromInvariantString(o.ToString());
                    }
                    catch (Exception ex)
                    {
                        //MessageService.ShowWarning("Error loading property '" + property + "': " + ex.Message);
                        o = defaultValue;
                    }
                    properties[property] = o; // store for future look up
                }
                else if (o is ArrayList && typeof(T).IsArray)
                {
                    ArrayList list = (ArrayList)o;
                    Type elementType = typeof(T).GetElementType();
                    Array arr = System.Array.CreateInstance(elementType, list.Count);
                    TypeConverter c = TypeDescriptor.GetConverter(elementType);
                    try
                    {
                        for (int i = 0; i < arr.Length; ++i)
                        {
                            if (list[i] != null)
                            {
                                arr.SetValue(c.ConvertFromInvariantString(list[i].ToString()), i);
                            }
                        }
                        o = arr;
                    }
                    catch (Exception ex)
                    {
                        //MessageService.ShowWarning("Error loading property '" + property + "': " + ex.Message);
                        o = defaultValue;
                    }
                    properties[property] = o; // store for future look up
                }
                else if (!(o is string) && typeof(T) == typeof(string))
                {
                    TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
                    if (c.CanConvertTo(typeof(string)))
                    {
                        o = c.ConvertToInvariantString(o);
                    }
                    else
                    {
                        o = o.ToString();
                    }
                }
                else if (o is SerializedValue)
                {
                    try
                    {
                        o = ((SerializedValue)o).Deserialize<T>();
                    }
                    catch (Exception ex)
                    {
                        //MessageService.ShowWarning("Error loading property '" + property + "': " + ex.Message);
                        o = defaultValue;
                    }
                    properties[property] = o; // store for future look up
                }
                try
                {
                    return (T)o;
                }
                catch (NullReferenceException)
                {
                    // can happen when configuration is invalid -> o is null and a value type is expected
                    return defaultValue;
                }
            }
        }

        /// <summary>
        /// Sets the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public void Set<T>(string property, T value)
        {
            T oldValue = default(T);
            lock (syncObj)
            {
                if (!properties.ContainsKey(property))
                {
                    properties.Add(property, value);
                }
                else
                {
                    oldValue = Get<T>(property, value);
                    properties[property] = value;
                }
            }
            OnPropertyChanged(new PropertyChangedEventArgs(this, property, oldValue, value));
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get {
                lock (syncObj) {
                    return properties.Count;
                }
            }
        }

        /// <summary>
        /// Determines whether [contains] [the specified property].
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified property]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string property)
        {
            lock (syncObj) {
                return properties.ContainsKey(property);
            }
        }

        /// <summary>
        /// Removes the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        public void Remove(string property)
        {
            lock (syncObj) {
                if (properties.ContainsKey(property))
                    properties.Remove(property);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            lock (properties)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[Properties:{");
                foreach (KeyValuePair<string, object> entry in properties)
                {
                    sb.Append(entry.Key);
                    sb.Append("=");
                    sb.Append(entry.Value);
                    sb.Append(",");
                }
                sb.Append("}]");
                return sb.ToString();
            }
        }

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        [EventPublication(EventNames.Uniframework_PropertyChanged, PublicationScope.Global)]
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
