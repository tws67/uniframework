using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

using Db4objects.Db4o;
using Db4objects.Db4o.Config;

namespace Uniframework.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class db4oHttpModule : IHttpModule, IDisposable
    {
        private static readonly int MAX_TRYTIMES = 100;

        /// <summary>
        /// Inits the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        public void Init(HttpApplication application)
        {
            Db4oFactory.Configure().UpdateDepth(Int32.MaxValue);
            Db4oFactory.Configure().OptimizeNativeQueries(true);
            Db4oFactory.Configure().DetectSchemaChanges(true); // 自动探测数据库模式的变化
            Db4oFactory.Configure().ExceptionsOnNotStorable(true);

            application.EndRequest += new EventHandler(Application_EndRequest);
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <param name="conStr">The con STR.</param>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        public static IObjectContainer GetClient(string conStr, Db4objects.Db4o.Config.IConfiguration config)
        {
            HttpContext context = HttpContext.Current;

            Dictionary<string, IObjectContainer> clients = context.Items[clientsCacheKey] as Dictionary<string, IObjectContainer>;

            if (clients == null)
            {
                clients = new Dictionary<string, IObjectContainer>();
                context.Items[clientsCacheKey] = clients;
            }
            
            if (!clients.ContainsKey(conStr))
            {
                IObjectContainer container = null;
                container = GetServer(conStr, config).OpenClient();
                if (container == null)
                {
                    int counter = 0;
                    while (container == null)
                    {
                        container = GetServer(conStr, config).OpenClient();
                        System.Threading.Thread.Sleep(1000);
                        counter++;
                        if (counter > MAX_TRYTIMES)
                            throw new Exception("已经重试超过 " + MAX_TRYTIMES.ToString() + " 次，数据库依然无法打开。");
                    }
                }
                clients.Add(conStr, container);
            }

            return clients[conStr];
        }
        
        #region Assistant functions

        /// <summary>
        /// Handles the EndRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Application_EndRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;

            Dictionary<string, IObjectContainer> clients = context.Items[clientsCacheKey] as Dictionary<string, IObjectContainer>;

            if (clients != null)
            {
                foreach (IObjectContainer client in clients.Values)
                    client.Close();

                clients.Clear();
            }
        }

        private static readonly string clientsCacheKey = "db4o-clients-in-httpcontext";

        /// <summary>
        /// Gets the client cache key.
        /// </summary>
        /// <param name="conStr">The con STR.</param>
        /// <returns></returns>
        private static string GetClientCacheKey(string conStr)
        {
            return string.Format("db4o-client-in-httpcontext-{0}", conStr);
        }

        private static Dictionary<string, IObjectServer> servers = new Dictionary<string, IObjectServer>();

        /// <summary>
        /// 打开db4o服务器
        /// </summary>
        /// <param name="conStr">连接字符串</param>
        /// <returns>返回创建的服务器</returns>
        private static IObjectServer GetServer(string conStr, Db4objects.Db4o.Config.IConfiguration config)
        {
            if (!servers.ContainsKey(conStr))
            {
                string filePath = HttpContext.Current.Server.MapPath(conStr);
                if (config == null)
                    servers.Add(conStr, Db4oFactory.OpenServer(filePath, 0));
                else
                    servers.Add(conStr, Db4oFactory.OpenServer(config, filePath, 0));
            }
            return servers[conStr];
        }

        /// <summary>
        /// Closes the servers.
        /// </summary>
        private static void CloseServers()
        {
            if (servers != null)
            {
                foreach (IObjectServer server in servers.Values)
                {
                    if (server != null)
                        server.Close();
                }
                servers.Clear();
            }
        }

        #endregion

        #region IDisposable 成员

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                CloseServers();
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

