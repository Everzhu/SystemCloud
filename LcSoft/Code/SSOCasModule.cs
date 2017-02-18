using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Net;
using System.IO;
using System.Security.Principal;
using System.Configuration;
using System.Web.SessionState;
using System.Text;
using System.Diagnostics;
using System.Data.Entity;

namespace XkSystem.CasModule
{
    /// <summary>
    /// 单点登录接口
    /// </summary>
    public class SSOCasModule : IHttpModule, IReadOnlySessionState
    {
        protected const string ReturnUrl = "XkSystem.CasModule";

        //调用配置文件参数
        public string IsStartCas = ConfigurationManager.AppSettings.Get("IsStartCas");
        public string casLogin = ConfigurationManager.AppSettings.Get("LoginUrl");
        public string casValidate = ConfigurationManager.AppSettings.Get("ValidateUrl");
        public string logoutUrl = ConfigurationManager.AppSettings.Get("LogoutUrl");
        public string ServerUrl = ConfigurationManager.AppSettings.Get("ServerUrl");

        public void Init(HttpApplication application)
        {
            //检测授权信息
            //application.BeginRequest += Application_BeginRequest;

            //检测单点是否开启:true/false
            //if (IsStartCas.ToLower() == "true")
            //{
            //    application.AcquireRequestState += (new EventHandler(this.Application_AuthenticateRequest));
            //}
        }

        /// <summary>
        /// 验证授权信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            var path = application.Request.CurrentExecutionFilePath.ToLower();
            string[] urlItems = path.ToLower().Split('/');
            string pageName = urlItems[urlItems.Length - 1];
            if (string.IsNullOrEmpty(pageName))
            {
                pageName = "/";
            }
         
            var SysConfig = application.Application["SysConfig"] == null ? "" : application.Application["SysConfig"].ToString();
            string basefilterStr = ConfigurationManager.AppSettings.Get("BaseFilterStr");
            string BackgroungUrlStr = "sys,sysindex,login,sys/,sysindex/,login/";
            if (basefilterStr.Split(',').Where(p => path.ToLower().EndsWith(p)).Count() > 0)
            {
                return;
            }

            var rootPath = application.Request.ApplicationPath;
            //if (!rootPath.EndsWith("/"))
            //    rootPath = rootPath + "/";
            if (string.IsNullOrEmpty(SysConfig) && BackgroungUrlStr.Split(',').Contains(pageName.ToLower()))
            {
                application.Response.Redirect("~/Sys/SysConfig/SysConfigEdit?paramter=" + XkSystem.Code.Common.ToMD5("True_XkSystem_zyk"+DateTime.Now.ToString("yyyy-MM-dd HH:mm"), true), false);
                return;
            }
            else if (string.IsNullOrEmpty(SysConfig) && pageName.ToLower() != "sysconfigdit")
            {
                application.Response.Redirect("~/Sys/SysConfig/SysConfigEdit?paramter=" + XkSystem.Code.Common.ToMD5("False_XkSystem_zyk" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"), true), false);
                return;
            }
            else
            {
                return;
            }
        }

        private void Application_AuthenticateRequest(Object source, EventArgs e)
        {
            try
            {
                HttpApplication application = (HttpApplication)source;
                var path = application.Request.CurrentExecutionFilePath.ToLower();

                string basefilterStr = ConfigurationManager.AppSettings.Get("BaseFilterStr");
                if (basefilterStr.Split(',').Where(p => path.ToLower().EndsWith(p)).Count() > 0)
                {
                    return;
                }
                string filterStr = ConfigurationManager.AppSettings.Get("FilterStr");
                string[] urlItems = path.ToLower().Split('/');
                string pageName = urlItems[urlItems.Length - 1];
                if (string.IsNullOrEmpty(pageName))
                {
                    pageName = "/";
                }
                if (filterStr.Split(',').FirstOrDefault(d => pageName.ToLower() == d) != null)
                {

                    return;
                }

                //if (filterStr.Split(',').Where(p => path.ToLower().EndsWith(p)).Count() > 0)
                //{
                //    return;
                //}

                //获取cookie缓存票据
                string authCookie = HttpContext.Current.Request.Cookies[AppName + "casTicket"].Value;

                //判断平台是否已登录
                if (!string.IsNullOrEmpty(authCookie))
                {
                    //浏览器不关闭的情况下会存在Cookie遗留的问题
                    if (XkSystem.Code.Common.UserId == 0)
                    {
                        //清空票据cookie
                        System.Web.HttpContext.Current.Response.Cookies[AppName + "SysUserId"].Expires = DateTime.Now;
                        System.Web.HttpContext.Current.Response.Cookies[AppName + "UserType"].Expires = DateTime.Now;
                        System.Web.HttpContext.Current.Response.Cookies[AppName + "UserName"].Expires = DateTime.Now;
                        System.Web.HttpContext.Current.Response.Cookies[AppName + "casTicket"].Expires = DateTime.Now;

                        //登录
                        Ilogin(application);
                    }
                    else
                    {
                        return;
                    }

                }
                else
                {
                    //登录
                    Ilogin(application);
                }

            }
            catch
            {
                throw;
            }

        }


        /// <summary>
        /// Check CdKey
        /// </summary>
        /// <param name="Qcdkey">请求的Key</param>
        /// <param name="Rcdkey">数据库中的key</param>
        /// <returns></returns>
        private bool CheckCdKey(string Qcdkey, string Rcdkey)
        {
            if (Qcdkey != Rcdkey)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 登录
        /// </summary>
        private void Ilogin(HttpApplication application)
        {
            try
            {
                //返回地址
                string serviceUrl = application.Request.Url.GetLeftPart(UriPartial.Path).ToLower().Split(new string[] { "/islogin", "/login" }, StringSplitOptions.None)[0];
                if (!string.IsNullOrEmpty(ServerUrl))
                {
                    serviceUrl = ServerUrl + serviceUrl.Replace("http://" + application.Request.Url.Authority, "");
                }

                string casTicket = application.Request.QueryString["ticket"];
                //if (!string.IsNullOrEmpty(casTicket))
                //{
                //    XkSystem.Core.Helper.CookieHelper.SetCookie("casTicket", casTicket);
                //}
                //else
                //{
                //    casTicket = XkSystem.Core.Helper.CookieHelper.GetCookie("casTicket");
                //}

                if (casTicket == null || casTicket.Length == 0)
                {
                    string redir = casLogin + "?service=" + serviceUrl;
                    application.Response.Redirect(redir);
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

                    int netid = 0;

                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            string tag = reader.LocalName;
                            if (tag == "user" && !string.IsNullOrEmpty(casTicket))
                            {
                                netid = reader.ReadString().ConvertToInt();
                                break;
                            }
                        }
                    }
                    reader.Close();

                    if (netid == 0)
                    {
                        //application.Response.Write("获取Cas配置错误提示！");
                        if (XkSystem.Code.Common.UserId != 0)
                        {
                            ////是否是该网站用户
                            //string ret = CheckUser();
                            //if (!string.IsNullOrEmpty(ret))
                            //{
                            //    netid = ret;
                            //}
                            //if (string.IsNullOrEmpty(netid))
                            //{
                            //    string redir = casLogin + "?service=" + serviceUrl;
                            //    application.Response.Redirect(redir);
                            //    return;
                            //}

                            return;
                        }
                        else
                        {
                            string redir = casLogin + "?service=" + serviceUrl;
                            application.Response.Redirect(redir);
                            return;
                        }
                    }
                    else
                    {
                        //application.Response.Write("Bienvenue " + netid);
                        //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket("UserInformation", true, DateTime.Now.Hour + 8);
                        //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
                        //cookie.Domain = "http://" + application.Request.Url.Authority + application.Request.ApplicationPath;
                        //cookie.Value = FormsAuthentication.Encrypt(ticket);
                        //application.Response.Cookies.Add(cookie);

                        var rootPath = application.Request.ApplicationPath;
                        if (!rootPath.EndsWith("/"))
                            rootPath = rootPath + "/";

                        XkSystem.Code.Common.UserId = netid;
                        HttpContext.Current.Response.Cookies[AppName + "casTicket"].Value = casTicket;

                        application.Response.Redirect(ServerUrl + rootPath + "Sys/SysIndex/IsLogin?code=" + netid);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static int CheckUser()
        {
            //是否是该网站用户
            using (var db = new XkSystem.Models.DbContext())
            {
                var user = db.tbSysUser.Include(d => d.tbSysUserType).FirstOrDefault(d => d.IsDisable == false && d.Id == XkSystem.Code.Common.UserId);
                if (user == null)
                {
                    user = db.tbSysUser.FirstOrDefault(d => d.IsDisable == false && d.UserCode == HttpContext.Current.Request.Cookies[AppName + "casUserCode"].Value);

                    HttpContext.Current.Response.Cookies[AppName + "SysUserId"].Value = user.Id.ToString();
                    HttpContext.Current.Response.Cookies[AppName + "UserType"].Value = user.tbSysUserType.UserTypeCode.ToString();
                    HttpContext.Current.Response.Cookies[AppName + "UserName"].Value = user.UserName;

                    return user.Id;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 请求退出
        /// </summary>
        //private void ReqLoginOut(HttpApplication application)
        //{
        //    byte[] byts = new byte[application.Request.InputStream.Length];
        //    application.Request.InputStream.Read(byts, 0, byts.Length);
        //    string req = System.Text.Encoding.Default.GetString(byts);
        //    req = application.Server.UrlDecode(req);

        //    if (req != null && application.Request.InputStream.Length > 0)
        //    {
        //        //req==
        //        //logoutRequest=
        //        //<samlp:LogoutRequest xmlns:samlp="urn:oasis:names:tc:SAML:2.0:protocol" ID="LR-11-DOsrfY9fLkTMm3IsR5ebUQhE5Y15ISzIll4" Version="2.0" IssueInstant="2014-02-11T10:24:51Z">
        //        //<saml:NameID xmlns:saml="urn:oasis:names:tc:SAML:2.0:assertion">@NOT_USED@</saml:NameID>
        //        //<samlp:SessionIndex>ST-11-hoOatfYkRE1nGMMlKpkK-cas</samlp:SessionIndex>
        //        //</samlp:LogoutRequest>
        //        if (req.StartsWith("logoutRequest="))
        //        {
        //            req = req.Remove(0, "logoutRequest=".Length);

        //            NameTable nt = new NameTable();
        //            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
        //            XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);
        //            XmlTextReader reader = new XmlTextReader(req, XmlNodeType.Element, context);

        //            string casTicket = null;

        //            while (reader.Read())
        //            {
        //                if (reader.IsStartElement())
        //                {
        //                    string tag = reader.LocalName;
        //                    if (tag == "SessionIndex")
        //                    {
        //                        casTicket = reader.ReadString();
        //                        application.Context.Application[casTicket] = null;

        //                        break;
        //                    }
        //                }
        //            }
        //            reader.Close();
        //        }
        //    }
        //}


        public void Dispose()
        {
        }
    }
}
