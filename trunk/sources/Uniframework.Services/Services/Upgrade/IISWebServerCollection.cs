using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// Web网站集合
    /// </summary>
    public class IISWebServerCollection : CollectionBase
    {
        public IISWebServer this[int Index]
        {
            get
            {
                return (IISWebServer)this.List[Index];
            }
        }

        /// <summary>
        /// Gets the <see cref="Uniframework.Services.IISWebServer"/> with the specified server comment.
        /// </summary>
        /// <value></value>
        public IISWebServer this[string ServerComment]
        {
            get
            {
                ServerComment = ServerComment.ToLower().Trim();
                IISWebServer list;
                for (int i = 0; i < this.List.Count; i++)
                {
                    list = (IISWebServer)this.List[i];
                    if (list.ServerComment.ToLower().Trim() == ServerComment)
                        return list;
                }
                return null;
            }
        }

        /// <summary>
        /// Add_s the specified web server.
        /// </summary>
        /// <param name="WebServer">The web server.</param>
        internal void AddWebServer(IISWebServer WebServer)
        {
            this.List.Add(WebServer);
        }

        /// <summary>
        /// Adds the specified web server.
        /// </summary>
        /// <param name="WebServer">The web server.</param>
        public void Add(IISWebServer WebServer)
        {
            try {
                this.List.Add(WebServer);
                IISManagement.CreateIISWebServer(WebServer);
            }
            catch {
                throw (new Exception("发生意外错误，可能是某节点将该节点的上级节点作为它自己的子级插入"));
            }
        }

        /// <summary>
        /// 是否包含指定的网站
        /// </summary>
        /// <param name="ServerComment">The server comment.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified server comment]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string ServerComment)
        {
            ServerComment = ServerComment.ToLower().Trim();
            for (int i = 0; i < this.List.Count; i++) {
                IISWebServer server = this[i];
                if (server.ServerComment.ToLower().Trim() == ServerComment)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 是否包含指定的网站
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified index]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(int index)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                IISWebServer server = this[i];
                if (server.index == index)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="WebServers">The web servers.</param>
        public void AddRange(IISWebServer[] WebServers)
        {
            for (int i = 0; i <= WebServers.GetUpperBound(0); i++)
            {
                Add(WebServers[i]);
            }
        }

        /// <summary>
        /// Removes the specified web server.
        /// </summary>
        /// <param name="WebServer">The web server.</param>
        public void Remove(IISWebServer WebServer)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                if ((IISWebServer)this.List[i] == WebServer)
                {
                    this.List.RemoveAt(i);
                    return;
                }
            }
            IISManagement.RemoveIISWebServer(WebServer.index);
        }
    }
}
