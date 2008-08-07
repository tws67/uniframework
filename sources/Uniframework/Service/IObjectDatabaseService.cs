using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// �������ݿ�ϵͳ�ӿ�
    /// </summary>
    public interface IObjectDatabaseService
    {
        /// <summary>
        /// �����ݿ����
        /// </summary>
        /// <param name="databaseName">���ݿ�����</param>
        /// <returns>���ض������ݿ�ӿ�</returns>
        IObjectDatabase OpenDatabase(string databaseName);

        /// <summary>
        /// �ر�ָ�����ݿ�
        /// </summary>
        /// <param name="databaseName">���ݿ�����</param>
        void CloseDatabase(string databaseName);

        /// <summary>
        /// ɾ��ָ�����ݿ�
        /// </summary>
        /// <param name="databaseName">���ݿ�����</param>
        void DropDatabase(string databaseName);

        /// <summary>
        /// ��ȡ�������ݿ���Ϣ
        /// </summary>
        /// <returns>�������ݿ���Ϣ����</returns>
        ObjectDatabaseInfo[] GetAllDatabaseInfo();
    }

    #region IObjectDatabase
    /// <summary>
    /// �������ݿ�ӿ�
    /// </summary>
    public interface IObjectDatabase
    {
        /// <summary>
        /// �������ʵ�������ݿ�
        /// </summary>
        /// <param name="item">Ҫ����Ķ���ʵ��</param>
        void Save(object item);

        /// <summary>
        /// �����ݿ���ɾ������ʵ��
        /// </summary>
        /// <param name="item">Ҫɾ���Ķ���ʵ��</param>
        void Delete(object item);

        /// <summary>
        /// ��ȡƥ��Ķ���ʵ������
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="match">ƥ���׼</param>
        /// <returns>ƥ��Ķ���ʵ������</returns>
        T[] Load<T>(Predicate<T> match) where T : class;

        /// <summary>
        /// ��ȡƥ��Ķ���ʵ������
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="template">��������ģ��</param>
        /// <returns>ƥ��Ķ���ʵ������</returns>
        T[] Load<T>(T template) where T : class;

        /// <summary>
        /// ��ȡ����ʵ������
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <returns></returns>
        T[] Load<T>() where T : class;

        /// <summary>
        /// ��ȡָ������ʵ���ı�ʶ
        /// </summary>
        /// <param name="obj">Ҫ�����Ķ���ʵ��</param>
        /// <returns></returns>
        string GetObjectKey(object obj);

        /// <summary>
        /// ���ݱ�ʶ��ȡ����ʵ��
        /// </summary>
        /// <param name="key">��ʶ</param>
        /// <returns></returns>
        object GetObjectByKey(string key);

        /// <summary>
        /// �������ʵ��
        /// </summary>
        /// <param name="obj">Ҫ����Ķ���ʵ��</param>
        /// <param name="depth">��Ҫ�����Ķ�����</param>
        void Activate(object obj, int depth);
    }
    #endregion

    #region ObjectDatabaseInfo
    /// <summary>
    /// �������ݿ���Ϣ��
    /// </summary>
    public class ObjectDatabaseInfo
    {
        private string name;
        private string description;
        private int size;
        private int storedClasses;
        private int objects;

        /// <summary>
        /// ��ȡ���������ݿ�����
        /// </summary>
        public string Name 
        { 
            get 
            { 
                return name; 
            } 
        }

        /// <summary>
        /// ��ȡ���������ݿ�����
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
        /// ��ȡ���ݿ��С
        /// </summary>
        public int Size 
        { 
            get 
            { 
                return size; 
            }
        }

        /// <summary>
        /// ��ȡ�����ô洢��������
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
        /// ��ȡ�����ô洢�Ķ�������
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
        /// ���췽��
        /// </summary>
        /// <param name="name">���ݿ�����</param>
        /// <param name="description">���ݿ�����</param>
        /// <param name="size">���ݿ��С</param>
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
    /// ���ڵ��Ե�IObjectDatabaseService��Mockʵ��
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
