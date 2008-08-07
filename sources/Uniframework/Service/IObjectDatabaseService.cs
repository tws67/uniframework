using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 对象数据库系统接口
    /// </summary>
    public interface IObjectDatabaseService
    {
        /// <summary>
        /// 打开数据库对象
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        /// <returns>返回对象数据库接口</returns>
        IObjectDatabase OpenDatabase(string databaseName);

        /// <summary>
        /// 关闭指定数据库
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        void CloseDatabase(string databaseName);

        /// <summary>
        /// 删除指定数据库
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        void DropDatabase(string databaseName);

        /// <summary>
        /// 获取所有数据库信息
        /// </summary>
        /// <returns>返回数据库信息数组</returns>
        ObjectDatabaseInfo[] GetAllDatabaseInfo();
    }

    #region IObjectDatabase
    /// <summary>
    /// 对象数据库接口
    /// </summary>
    public interface IObjectDatabase
    {
        /// <summary>
        /// 保存对象实例到数据库
        /// </summary>
        /// <param name="item">要保存的对象实例</param>
        void Save(object item);

        /// <summary>
        /// 从数据库中删除对象实例
        /// </summary>
        /// <param name="item">要删除的对象实例</param>
        void Delete(object item);

        /// <summary>
        /// 获取匹配的对象实例数组
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="match">匹配标准</param>
        /// <returns>匹配的对象实例数组</returns>
        T[] Load<T>(Predicate<T> match) where T : class;

        /// <summary>
        /// 获取匹配的对象实例数组
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="template">对象类型模板</param>
        /// <returns>匹配的对象实例数组</returns>
        T[] Load<T>(T template) where T : class;

        /// <summary>
        /// 获取对象实例数组
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        T[] Load<T>() where T : class;

        /// <summary>
        /// 获取指定对象实例的标识
        /// </summary>
        /// <param name="obj">要操作的对象实例</param>
        /// <returns></returns>
        string GetObjectKey(object obj);

        /// <summary>
        /// 根据标识获取对象实例
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns></returns>
        object GetObjectByKey(string key);

        /// <summary>
        /// 激活对象实例
        /// </summary>
        /// <param name="obj">要激活的对象实例</param>
        /// <param name="depth">需要操作的对象层次</param>
        void Activate(object obj, int depth);
    }
    #endregion

    #region ObjectDatabaseInfo
    /// <summary>
    /// 对象数据库信息类
    /// </summary>
    public class ObjectDatabaseInfo
    {
        private string name;
        private string description;
        private int size;
        private int storedClasses;
        private int objects;

        /// <summary>
        /// 获取或设置数据库名称
        /// </summary>
        public string Name 
        { 
            get 
            { 
                return name; 
            } 
        }

        /// <summary>
        /// 获取或设置数据库描述
        /// </summary>
        public string Description 
        { 
            get 
            { 
                return description; 
            } 
            
            set 
            { 
                description = value; 
            } 
        }

        /// <summary>
        /// 获取数据库大小
        /// </summary>
        public int Size 
        { 
            get 
            { 
                return size; 
            }
        }

        /// <summary>
        /// 获取或设置存储的类总数
        /// </summary>
        public int StoredClasses 
        { 
            get
            {
                return storedClasses; 
            } 
            
            set 
            { 
                storedClasses = value; 
            } 
        }

        /// <summary>
        /// 获取或设置存储的对象总数
        /// </summary>
        public int Objects 
        { 
            get 
            { 
                return objects; 
            } 
            
            set 
            {
                objects = value; 
            } 
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">数据库名称</param>
        /// <param name="description">数据库描述</param>
        /// <param name="size">数据库大小</param>
        public ObjectDatabaseInfo(string name, string description, int size)
        {
            this.name = name;
            this.description = description;
            this.size = size;
            storedClasses = 0;
            objects = 0;
        }
    }
    #endregion

    #if(Debug)
    /// <summary>
    /// 用于调试的IObjectDatabaseService的Mock实现
    /// </summary>
    public class MockObjectDatabaseService : IObjectDatabaseService
    {
        public IObjectDatabase db;

    #region IObjectDatabaseService Members

        public void CloseDatabase(string databaseName)
        {

        }

        public void DropDatabase(string databaseName)
        {

        }

        public ObjectDatabaseInfo[] GetAllDatabaseInfo()
        {
            return null;
        }

        public ObjectDatabaseInfo[] GetInfo()
        {
            return null;
        }

        public IObjectDatabase OpenDatabase(string databaseName)
        {
            return db;
        }

        public IObjectDatabase ObjectDatabase
        {
            get
            {
                return db;
            }
            set
            {
                db = value;
            }
        }

    #endregion
    }

    //public class MockObjectDatabase : IObjectDatabase
    //{
    //    public ArrayList al = new ArrayList();

    //    #region IObjectDatabase Members

    //    public void Delete(object item)
    //    {
    //        if (item == null) return;
    //        if(al.Contains(item)) al.Remove(item);
    //    }

    //    public T[] Load<T>() where T : class
    //    {
    //        List<T> list = new List<T>();
    //        foreach (T t in al)
    //        {
    //            list.Add(t);
    //        }
    //        return list.ToArray();
    //    }

    //    public T[] Load<T>(T template) where T : class
    //    {
    //        return new T[0];
    //    }

    //    public T[] Load<T>(Predicate<T> match) where T : class
    //    {
    //        List<T> list = new List<T>();
    //        foreach (object t in al)
    //        {
    //            if(t is T)
    //                list.Add(t as T);
    //        }
    //        return list.FindAll(match).ToArray();
    //    }

    //    public void Save(object item)
    //    {
    //        if (!al.Contains(item))
    //            al.Add(item);
    //    }

    //    #endregion
    //}
    #endif
}
