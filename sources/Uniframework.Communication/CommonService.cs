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
    /// ���������������ӿͻ������������
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
        /// ���÷���˵���Ӧ����
        /// </summary>
        /// <param name="data">�ͻ��˷������ĵ��ð�</param>
        /// <returns>���л����������</returns>
        /// # ���XmlSerializer����ֱ�ӽ����෴���л�Ϊ���������������޸ģ��������BinaryFormatter�򲻴��ڴ����⡣
        /// # ���ڲ���Xml���л����ܽ��ͻ�����Ӧ�÷����������������û�bf 2007-11-07 by Jacky
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
                logger.Error("���÷���������", ex);

                throw ExceptionHelper.WrapException(ex);
            }
        }
    }
}
