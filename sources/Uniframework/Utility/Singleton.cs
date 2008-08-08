// ***************************************************************
//  version:  1.0   date: 11/27/2007
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  (C)2007 Midapex All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Threading;

namespace Uniframework
{
    /// <summary>
    /// Singleton Mode
    /// </summary>
    /// <typeparam name="T">Class type would be implemented singleton mode</typeparam>
    public abstract class Singleton<T>
    {
        private static object syncObj = new object();
        private static T instance;

        /// <summary>
        /// Entry for access singleton unique value
        /// </summary>
        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    Monitor.Enter(syncObj);
                    if (instance == null)
                    {
                        try
                        {
                            instance = (T)Activator.CreateInstance(typeof(T), true) ;
                        }
                        finally
                        {
                            Monitor.Exit(syncObj);
                        }
                    }
                }

                return instance;
            }
            set
            {
                Monitor.Enter(syncObj);
                try
                {
                    instance = value;
                }
                finally
                {
                    Monitor.Exit(syncObj);
                }
            }
        }
    }
}
