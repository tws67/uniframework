using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Castle.Windsor;
using Uniframework.Services;

namespace Uniframework.Communication
{
    /// <summary>
    /// 公共服务用于连接客户端与服务器端
    /// </summary>
    public class CommonService
    {
        private static ILogger logger;
        private static Serializer serializer = new Serializer();

        #region Assistant function

        private static ServiceGateway Gateway
        {
            get {
                return Singleton<DefaultContainer>.Instance[typeof(ServiceGateway)] as ServiceGateway;
            }
        }

        private static bool Ping(NetworkInvokePackage ping)
        {
            if (DefaultContainer.SystemReady) {
                Gateway.ActiviteSessioin(ping.SessionId);
                return true;
            }
            else {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 调用服务端的相应方法
        /// </summary>
        /// <param name="data">客户端发过来的调用包</param>
        /// <returns>序列化后的数据流</returns>
        /// # 针对XmlSerializer不能直接将子类反序列化为基类的情况进行了修改，如果采用BinaryFormatter则不存在此问题。
        /// # 由于采用Xml序列化不能将客户端与应用服务器相连，所以用回bf 2007-11-07 by Jacky
        public static byte[] Invoke(byte[] data)
        {
            try {
                NetworkInvokePackage package = serializer.Deserialize<NetworkInvokePackage>(data);
                if (package.InvokeType == NetworkInvokeType.Ping)
                    return serializer.Serialize<bool>(Ping(package));
                else
                    return Gateway.Execute(package);
            }
            catch (Exception ex) {
                if (logger == null) 
                    logger = DefaultHttpApplication.LoggerFactory.CreateLogger<CommonService>("Framework");
                logger.Error("调用服务发生错误", ex);

                throw ExceptionHelper.WrapException(ex);
            }
        }
    }
}
