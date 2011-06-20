using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using log4net.Config;

namespace QBClient
{
    class LogHelper
    {
        private static readonly ILog log =
LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //log.Debug("This is a DEBUG level message. The most VERBOSE level.");
            //log.Info("Extended information, with higher importance than the Debug call");
            //log.Warn("An unexpected but recoverable situation occurred");
            //log.Error("An unexpected error occurred, an exception was thrown, or is about to be thrown", ex);
            //log.Fatal("Meltdown!", ex);
        public LogHelper()
        {
            //log4net.Config.XmlConfigurator.Configure();
        }
        public static void Debug(string s)
        {
            log.Debug(s);
        }
        public static void Info(string s)
        {
            log.Info(s);
        }
        public static void Warn(string s)
        {
            log.Warn(s);
        }
        public static void Error(Exception s)
        {
            log.Error("An unexpected error occurred, an exception was thrown, or is about to be thrown", s);
        }
        public static void Fatal(Exception s)
        {
            log.Fatal("Meltdown!", s);
        }
    }
}
