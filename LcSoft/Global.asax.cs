using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace XkSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            WebApiConfig.Register(System.Web.Http.GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            if (Code.Common.IsLog)
            {
                Code.LogHelper.Info("系统启动!");
            }

            new Areas.Sys.Controllers.SysIndexController().RestartCache();

            //允许被嵌入到Iframe中，主要是为了给云桌面调用的
            System.Web.Helpers.AntiForgeryConfig.SuppressXFrameOptionsHeader = true;


            // 在应用程序启动时运行的代码
            var myTimer = new System.Timers.Timer(0.5 * 60000);//设计时间间隔，如果一个小时执行一次就改为3600000 ，这里五分钟调用一次          
            //t.AutoReset = true;
            myTimer.Enabled = true;
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(myTimer_Elapsed);
        }

        private void myTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            (sender as System.Timers.Timer).Stop();
            //短信发送（定时器）
            //XkSystem.Areas.Sms.Controllers.SmsSendController.SmsSend();
            (sender as System.Timers.Timer).Start();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest()
        {

        }

        protected void Application_EndRequest()
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = HttpContext.Current.Server.GetLastError();
            if (Code.Common.IsLog)
            {
                Code.LogHelper.Error(ex);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
