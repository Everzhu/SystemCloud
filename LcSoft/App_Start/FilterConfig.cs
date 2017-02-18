using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;

using System.Collections.Generic;

namespace XkSystem
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new UserAuthorize());
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        public class UserAuthorize : AuthorizeAttribute
        {
            //调用配置文件参数
            public string IsStartCas = ConfigurationManager.AppSettings.Get("IsStartCas");
            public string casLogin = ConfigurationManager.AppSettings.Get("LoginUrl");
            public string casValidate = ConfigurationManager.AppSettings.Get("ValidateUrl");
            public string ServerUrl = ConfigurationManager.AppSettings.Get("ServerUrl");

            public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
            {
                var config = HttpContext.Current.Cache["Config"] as List<Areas.Admin.Entity.tbConfig>;
                if (HttpContext.Current.Cache["License"] == null && !filterContext.ActionDescriptor.IsDefined(typeof(Code.FilterHelper.KeygenAttribute), true))
                {
                    if (config == null)
                    {
                        //再次重新读取缓存
                        new Areas.Sys.Controllers.SysIndexController().RestartCache();
                        config = HttpContext.Current.Cache["Config"] as List<Areas.Admin.Entity.tbConfig>;

                        if (config == null)
                        {
                            filterContext.HttpContext.Response.Write("读取系统配置信息失败，请确保系统与数据库版本一致!");
                            filterContext.HttpContext.Response.End();
                        }
                    }

                    if (config.Where(d => d.ConfigType == Code.EnumHelper.ConfigType.CdKey && Code.JsonHelper.FromJsonString<Areas.Admin.Dto.Config.Cdkey>(Code.Common.DESDeCode(d.ConfigValue)).MachineCode == Code.Common.getMachineCode()).Any())
                    {
                        //判断注册码是否正确
                        var cdKey = config.Where(d => d.ConfigType == Code.EnumHelper.ConfigType.CdKey)
                                    .Select(d => Code.JsonHelper.FromJsonString<Areas.Admin.Dto.Config.Cdkey>(Code.Common.DESDeCode(d.ConfigValue)))
                                    .Where(d => d.MachineCode == Code.Common.getMachineCode())
                                    .Where(d => d.Deadline.ConvertToDateTime() >= DateTime.Today)
                                    .ToList();
                        //判断是否超时
                        if (!cdKey.Any())
                        {
                            filterContext.HttpContext.Response.Write("您的系统已过试用期，请联系开放商购买正式版本!");
                            filterContext.HttpContext.Response.End();
                        }
                        else
                        {
                            HttpContext.Current.Cache["License"] = "true";
                        }
                    }
                    else
                    {
                        //系统未注册
                        filterContext.HttpContext.Response.Redirect("~/Sys/SysIndex/Keygen");
                        filterContext.HttpContext.Response.End();
                    }
                }

                if (filterContext.RequestContext.HttpContext.Request.QueryString["JavaApp"] != null)
                {
                    Code.Common.IsJavaApp = true;
                }

                //var path = filterContext.HttpContext.Request.CurrentExecutionFilePath.ToLower();
                //string[] urlItems = path.ToLower().Split('/');
                //string pageName = urlItems[urlItems.Length - 1];
                //if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
                //{
                //    string a = "1";//授权访问
                //}
                //else
                //{
                //    string b = "2";//匿名访问
                //}
                //匿名访问页面不检测单点
                if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
                {
                    //检测单点是否开启:true/false
                    if (IsStartCas.ToLower() == "true")
                    {
                        try
                        {
                            var path = filterContext.HttpContext.Request.CurrentExecutionFilePath.ToLower();
                            string[] urlItems = path.ToLower().Split('/');
                            string pageName = urlItems[urlItems.Length - 1];
                            //获取cookie缓存票据
                            var authCookie = filterContext.HttpContext.Request.Cookies[Code.Common.AppName + "casTicket"];

                            //判断平台是否已登录
                            if (authCookie != null)
                            {
                                //浏览器不关闭的情况下会存在Cookie遗留的问题
                                if (filterContext.HttpContext.Request.Cookies[Code.Common.AppName + "UserId"] == null)
                                {
                                    //清空票据cookie
                                    System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "UserId"].Expires = DateTime.Now;
                                    System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "UserType"].Expires = DateTime.Now;

                                    //登录
                                    Ilogin(filterContext);
                                }
                                else
                                {
                                    //if (Code.Common.IsLog)
                                    //{
                                    //    Code.LogHelper.Info("pageName:" + pageName + "；UserId:" + filterContext.HttpContext.Request.Cookies[AppName + "UserId"].Value + "；casTicket:" + filterContext.HttpContext.Request.Cookies[AppName + "casTicket"].Value);
                                    //}
                                    return;
                                }

                            }
                            else
                            {
                                //登录
                                Ilogin(filterContext);
                            }
                        }
                        catch (Exception ex)
                        {
                            var aa = ex.Message;
                            throw;
                        }
                    }
                    else
                    {
                        if (filterContext.HttpContext.Request["Account"] != null)
                        {
                            UserId(Code.Common.DESDeCode(filterContext.HttpContext.Request["Account"]));
                        }

                        if (filterContext.HttpContext.Request["AppId"] != null)
                        {
                            var programId = filterContext.HttpContext.Request["AppId"].ConvertToInt();
                            using (var db = new XkSystem.Models.DbContext())
                            {
                                var program = (from p in db.TableRoot<Areas.Admin.Entity.tbProgram>()
                                               where p.Id == programId
                                               orderby p.No
                                               select new
                                               {
                                                   p.Id,
                                                   p.IsWide,
                                                   p.ProgramName,
                                                   p.Startup
                                               }).FirstOrDefault();
                                if (program != null)
                                {
                                    Code.Common.ProgramId = program.Id;
                                    Code.Common.IsWide = program.IsWide;
                                    Code.Common.ProgramName = program.ProgramName;
                                }
                            }
                        }

                        if (filterContext.HttpContext.Request.Cookies[Code.Common.AppName + "UserId"] == null)
                        {
                            //Add,Harvey,20161201,不开单点自动跳转到功能页面
                            string rawUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                            //Add End

                            filterContext.Result = new RedirectResult("~/Login?returnUrl=" + rawUrl);
                        }
                    }
                }
            }

            private void Ilogin(AuthorizationContext filterContext)
            {
                try
                {
                    var path = filterContext.HttpContext.Request.CurrentExecutionFilePath.ToLower();
                    string[] urlItems = path.ToLower().Split('/');
                    string pageName = urlItems[urlItems.Length - 1];


                    string casTicket = filterContext.HttpContext.Request.QueryString["ticket"];
                    Code.LogHelper.Info("casTicket:" + casTicket);

                    //返回地址
                    string serviceUrl = filterContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Path).ToLower().Split(new string[] { "/islogin", "/login" }, StringSplitOptions.None)[0];
                    if (!string.IsNullOrEmpty(ServerUrl))
                    {
                        serviceUrl = ServerUrl + serviceUrl.Replace("http://" + filterContext.HttpContext.Request.Url.Authority, "");
                        if (Code.Common.IsMobile)
                        {
                            if (casTicket == null || casTicket.Length == 0)
                            {
                                serviceUrl += "/sys/sysindex/login";//接入微信入口单点跳转地址不对
                            }
                        }
                    }
                    if (casTicket == null || casTicket.Length == 0)
                    {
                        string redir = casLogin + "?service=" + serviceUrl;
                        if (Code.Common.IsMobile)
                        {
                            Code.LogHelper.Info("IsMobile!");
                            redir = serviceUrl;
                        }
                        filterContext.HttpContext.Response.Redirect(redir);
                        return;
                    }
                    else
                    {
                        string validateurl = casValidate + "?ticket=" + casTicket + "&" + "service=" + serviceUrl;
                        WebClient client = new WebClient();
                        StreamReader Reader = new StreamReader(client.OpenRead(validateurl));

                        string resp = Reader.ReadToEnd();

                        StackTrace st = new StackTrace(new StackFrame(true));
                        NameTable nt = new NameTable();
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
                        XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);
                        XmlTextReader reader = new XmlTextReader(resp, XmlNodeType.Element, context);

                        string netid = null;

                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {
                                string tag = reader.LocalName;
                                if (tag == "user" && !string.IsNullOrEmpty(casTicket))
                                {
                                    netid = reader.ReadString();
                                    break;
                                }
                            }
                        }
                        reader.Close();

                        if (netid == null)
                        {
                            if (XkSystem.Code.Common.UserId != 0)
                            {
                                return;
                            }
                            else
                            {
                                string redir = casLogin + "?service=" + serviceUrl;
                                filterContext.HttpContext.Response.Redirect(redir);
                                return;
                            }
                        }
                        else
                        {

                            var rootPath = filterContext.HttpContext.Request.ApplicationPath;
                            if (!rootPath.EndsWith("/"))
                                rootPath = rootPath + "/";
                            if (XkSystem.Code.Common.UserId == 0)
                            {
                                //XkSystem.Code.Common.UserId = UserId(netid);
                                UserId(netid);
                            }
                            HttpContext.Current.Response.Cookies[Code.Common.AppName + "casTicket"].Value = casTicket;

                            filterContext.HttpContext.Response.Redirect(ServerUrl + "/" + Code.Common.FolderName + "/Sys/SysIndex/IsLogin?code=" + netid);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            private void UserId(string UserCode)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var user = (from p in db.TableRoot<Areas.Sys.Entity.tbSysUser>()
                                where p.tbTenant.IsDeleted == false
                                    && p.UserCode == UserCode
                                select new { Id = p.Id, UserName = p.UserName, UserType = p.UserType, TenantId = p.tbTenant.Id, AppTitle = p.tbTenant.Title }).FirstOrDefault();
                    if (user != null)
                    {
                        Code.Common.UserId = user.Id;
                        Code.Common.UserName = user.UserName;
                        Code.Common.UserType = user.UserType;
                        Code.Common.TenantId = user.TenantId;
                        Code.Common.AppTitle = user.AppTitle;
                    }
                }
            }
        }
    }
}
