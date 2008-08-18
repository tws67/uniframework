using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
using Uniframework.Client;

namespace Uniframework.StartUp
{
    public class Programe
    {
        private static readonly string LOGGER_CONFIGFILE = "log4net.config";
        private static ILog logger;

        private static string sessionID;
        private static EventDetector detector;
        private static frmLogin loginForm;
        private static Mutex mutex = new Mutex();
        private static Thread ms_oThread;
        private static int PasswordTryCount = 0;

        #region Program Members

        public static log4net.ILog Logger
        {
            get
            {
                return logger;
            }
        }

        public static string SessionID
        {
            get
            {
                return sessionID;
            }
        }

        #endregion

        [STAThread]
        static void Main()
        {

        }
    }
}
