using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Code
{
    public class LogHelper
    {
        private static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");

        private static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");

        private static readonly log4net.ILog logdebug = log4net.LogManager.GetLogger("logdebug");

        public static void Info(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
        }

        public static void Debug(string info)
        {
            if (loginfo.IsDebugEnabled)
            {
                logdebug.Error(info);
            }
        }

        public static void Error(Exception ex)
        {
            var error = "错误标题：" + ex.Message + "<br>错误地址：" + System.Web.HttpContext.Current.Request.Url + "<br>详细信息：<pre>" + ex.StackTrace + "</pre>";

            error = DeepError(ex, error);

            logerror.Error(error);
        }

        public static string DeepError(Exception ex, string error)
        {
            if (ex.InnerException != null)
            {
                error = error + "<hr>" + "错误标题：" + ex.InnerException.Message + "<br>详细信息：<pre>" + ex.InnerException.StackTrace + "</pre>";

                DeepError(ex, error);
            }

            return error;
        }
    }
}