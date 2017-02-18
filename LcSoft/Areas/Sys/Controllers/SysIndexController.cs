using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sys.Controllers
{
    public class SysIndexController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "", int programId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysIndex.Login();
                vm.ProgramList = Areas.Admin.Controllers.ProgramController.InfoList();
                if (string.IsNullOrEmpty(Code.Common.Program))
                {
                    vm.ProgramList = vm.ProgramList.Take(16).ToList();
                }
                else
                {
                    if (vm.ProgramList.Where(d => d.IsDefault).Any())
                    {
                        vm.ProgramId = vm.ProgramList.Where(d => d.IsDefault).FirstOrDefault().Id;
                    }
                    else
                    {
                        if (programId != 0)
                        {
                            vm.ProgramId = programId;
                        }
                        else if (Code.Common.ProgramId != 0)
                        {
                            vm.ProgramId = Code.Common.ProgramId;
                        }
                    }
                    vm.ProgramList.Clear();
                }

                vm.CheckCodeRefer = Code.Common.CreateCheckCode();
                if (Request.Cookies[Code.Common.AppName + "AppTitle"] != null)
                {
                    Request.Cookies.Remove(Code.Common.AppName + "AppTitle");
                }

                var cookies = System.Web.HttpContext.Current.Request.Cookies[Code.Common.AppName + "XkSystem"];
                if (cookies != null && cookies.HasKeys)
                {
                    vm.SchoolName = Code.Common.DESDeCode(cookies[Code.Common.AppName + "SchoolName"]);
                    vm.UserCode = Code.Common.DESDeCode(cookies[Code.Common.AppName + "UserCode"]);
                    vm.Password = Code.Common.DESDeCode(cookies[Code.Common.AppName + "Password"]);
                    vm.CheckCode = vm.CheckCodeRefer;
                    vm.Remember = true;
                }

                if (HttpContext.Cache["TenantName"] != null)
                {
                    vm.SchoolName = db.TableRoot<Admin.Entity.tbTenant>().FirstOrDefault().TenantName;
                }
                else
                {
                    vm.IsTenant = true;
                }

                var tenantDefault = db.Set<Admin.Entity.tbTenant>().Where(d => d.IsDefault).FirstOrDefault();
                if (tenantDefault != null)
                {
                    vm.Logo = tenantDefault.Logo;
                }

                //Add,Harvey,非单点登录功能页面跳转
                vm.ReturnUrl = returnUrl;
                //Add End

                //Add,Harvey,xxzx,for demo,20161031
                //if (Code.Common.IsMobile) return View("mxxzx_Login", vm);//西乡
                //Add End

                //Add,Harvey,二十,for Java,20161129
                //if (Code.Common.IsMobile) return View("m20_Login", vm);
                //Add End

                if (Code.Common.IsMobile) return View("m_Login", vm);//光高
                if (Code.Common.Program.ToUpper() == "EAS")
                {
                    return View("Login" + Code.Common.Program, vm);
                }
                else
                {
                    return View("Login", vm);
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UserInfo()
        {
            if (Code.Common.IsMobile)
            {
                Models.SysIndex.Index vm = GetUserInfo();
                return PartialView("m_UserInfo", vm);
            }
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(Models.SysIndex.Login vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (Code.Common.UserId == 0 || string.IsNullOrEmpty(Code.Common.Program) == false)
                {
                    error = new List<string>();
                    error.AddError(SysUserLogin(vm));
                }

                if (vm.ProgramId == 0 && !Code.Common.IsMobile)
                {
                    return Code.MvcHelper.Post(error, Url.Action("Login", "SysIndex", new { area = "Sys" }));
                }

                var program = (from p in db.TableRoot<Admin.Entity.tbProgram>()
                               where p.Id == vm.ProgramId
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

                    if (string.IsNullOrEmpty(program.Startup) == false)
                    {
                        return Code.MvcHelper.Post(error, Url.Content("~/" + program.Startup));
                    }
                }
                //Add Harvey,20161201,非单点登录功能页面跳转
                if (!string.IsNullOrEmpty(vm.ReturnUrl))
                {
                    return Code.MvcHelper.Post(error, vm.ReturnUrl);
                }
                //Add End
                return Code.MvcHelper.Post(error, Url.Action("Index", "SysIndex", new { area = "Sys" }));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public static string SysUserLogin(Sys.Models.SysIndex.Login vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (string.Compare(vm.CheckCode, vm.CheckCodeRefer, true) != decimal.Zero)
                {
                    return "验证码不正确!";
                }

                var IsStartCas = ConfigurationManager.AppSettings.Get("IsStartCas");
                var password = Code.Common.DESEnCode(vm.Password);
                var passwordMd5 = Code.Common.CreateMD5Hash(vm.Password);
                var user = (from p in db.TableRoot<Sys.Entity.tbSysUser>()
                            where p.tbTenant.IsDeleted == false
                                && (p.tbTenant.TenantName == vm.SchoolName || IsStartCas == "true")
                                && (p.UserCode == vm.UserCode || p.Mobile == vm.UserCode || p.Email == vm.UserCode || p.IdentityNumber == vm.UserCode)
                                && (p.Password == password || p.PasswordMd5 == passwordMd5 || p.Password == "" || IsStartCas == "true")
                            select new
                            {
                                p.Id,
                                p.IsDisable,
                                p.IsLock,
                                p.UserName,
                                p.UserType,
                                TenantId = p.tbTenant.Id,
                                p.tbTenant.Title
                            }).FirstOrDefault();
                if (user == null)
                {
                    return "账号或密码不正确，请重新输入!";
                }
                else
                {
                    if (user.IsDisable)
                    {
                        return "帐号被禁用!";
                    }

                    if (user.IsLock)
                    {
                        return "帐号因多次密码错误被锁定，请通过【找回账号密码】功能重新激活账号!";
                    }

                    Code.Common.UserId = user.Id;
                    Code.Common.UserName = user.UserName;
                    Code.Common.UserType = user.UserType;
                    Code.Common.TenantId = user.TenantId;
                    Code.Common.AppTitle = user.Title;

                    var userRole = (from p in db.TableRoot<Sys.Entity.tbSysUserRole>().Include(p => p.tbSysRole) where p.tbSysUser.Id == user.Id select p.tbSysRole.RoleCode).ToList();

                    //是否是资产报修管理人员
                    //var assetAdminRole = (from p in db.TableRoot<Sys.Entity.tbSysUserRole>() where p.tbSysUser.Id == user.Id && p.tbSysRole.RoleCode == Code.EnumHelper.SysRoleCode.RepairManagner select p).FirstOrDefault();
                    //Code.Common.IsRepairMananger = assetAdminRole != null;

                    //是否是资产受理人员
                    //var assetApplyRole = (from p in db.TableRoot<Sys.Entity.tbSysUserRole>() where p.tbSysUser.Id == user.Id && p.tbSysRole.RoleCode == Code.EnumHelper.SysRoleCode.Repair select p).FirstOrDefault();
                    //Code.Common.IsProcessUser = assetApplyRole != null;

                    Code.Common.IsMoralMananger = userRole.Count(p => p == Code.EnumHelper.SysRoleCode.Administrator) > 0;
                    if (vm.Remember)
                    {
                        var cookie = new System.Web.HttpCookie(Code.Common.AppName + "XkSystem");
                        cookie.Values.Add(Code.Common.AppName + "UserCode", Code.Common.DESEnCode(vm.UserCode));
                        cookie.Values.Add(Code.Common.AppName + "Password", Code.Common.DESEnCode(vm.Password));
                        cookie.Values.Add(Code.Common.AppName + "SchoolName", Code.Common.DESEnCode(vm.SchoolName));
                        cookie.Expires = DateTime.Now.AddYears(1);
                        System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "XkSystem"].Expires = DateTime.Now;
                    }

                    System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "Account_Code_Ticket"].Expires = DateTime.Now;

                    SysUserLogController.Insert("登录系统!", user.Id);
                }

                return string.Empty;
            }
        }

        [AllowAnonymous]
        public ActionResult SsoLogin(int schoolId, string loginCode)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                loginCode = Code.Common.DESDeCode(loginCode);
                var user = (from p in db.TableRoot<Sys.Entity.tbSysUser>()
                            where p.tbTenant.IsDeleted == false
                                && p.tbTenant.Id == schoolId
                                && p.UserCode == loginCode
                            select new
                            {
                                p.Id,
                                p.IsDisable,
                                p.IsLock,
                                p.UserName,
                                p.UserType,
                                TenantId = p.tbTenant.Id
                            }).FirstOrDefault();
                if (user == null)
                {
                    return Content("<script>alert('账号或密码不正确，请重新输入!');</script>");
                }
                else
                {
                    if (user.IsDisable)
                    {
                        return Content("<script>alert('帐号被禁用!');</script>");
                    }

                    if (user.IsLock)
                    {
                        return Content("<script>alert('帐号因多次密码错误被锁定，请通过【找回账号密码】功能重新激活账号!');</script>");
                    }

                    Code.Common.UserId = user.Id;
                    Code.Common.UserName = user.UserName;
                    Code.Common.UserType = user.UserType;
                    Code.Common.TenantId = user.TenantId;

                    SysUserLogController.Insert("登录系统!");
                }

                var program = (from p in db.TableRoot<Admin.Entity.tbProgram>()
                               where (p.Id == Code.Common.ProgramId || Code.Common.ProgramId == 0)
                               orderby p.No
                               select new
                               {
                                   p.Id,
                                   p.IsWide,
                                   p.Startup
                               }).FirstOrDefault();
                if (program != null)
                {
                    Code.Common.ProgramId = program.Id;
                    Code.Common.IsWide = program.IsWide;

                    if (string.IsNullOrEmpty(program.Startup) == false)
                    {
                        return Code.MvcHelper.Post(null, Url.Content("~/" + program.Startup));
                    }
                }

                return Code.MvcHelper.Post(null, Url.Action("Index", "SysIndex", new { area = "Sys" }));
            }
        }

        /// <summary>
        /// 单点登录
        /// </summary>
        /// <param name="code">用户ID</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult IsLogin(string code)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["ticket"]) && string.IsNullOrEmpty(code))
            {
                return Content(Code.Common.RedirectCas(Url.Action(nameof(this.Login)), "请求参数错误！"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.TableRoot<Sys.Entity.tbSysUser>().FirstOrDefault(d => d.UserCode == code);
                if (tb == null)
                {
                    return Content(Code.Common.RedirectCas(Url.Action(nameof(this.Login)), "用户不存在！"));
                }

                Sys.Models.SysIndex.Login vm = new Sys.Models.SysIndex.Login();
                vm.CheckCode = "1234";
                vm.CheckCodeRefer = "1234";
                vm.UserCode = tb.UserCode;
                vm.Password = Code.Common.DESDeCode(tb.Password);
                vm.SchoolName = "深圳龙创软件";

                string ret = SysUserLogin(vm);
                if (!string.IsNullOrEmpty(ret))
                {
                    return Content(Code.Common.RedirectCas(Url.Action(nameof(this.Login)), ret));
                }

                var program = (from p in db.TableRoot<Admin.Entity.tbProgram>()
                                   //where (p.Id == Code.Common.ProgramId || Code.Common.ProgramId == 0)
                               where (p.Id == Code.Common.ProgramId || p.IsDefault)
                               orderby p.No
                               select new
                               {
                                   p.Id,
                                   p.ProgramName,
                                   p.IsWide,
                                   p.Startup
                               }).FirstOrDefault();
                if (program != null)
                {
                    Code.Common.ProgramId = program.Id;
                    Code.Common.ProgramName = program.ProgramName;
                    Code.Common.IsWide = program.IsWide;

                    if (string.IsNullOrEmpty(program.Startup) == false)
                    {
                        return Code.MvcHelper.Post(null, Url.Content("~/" + program.Startup));
                    }
                }
                if (Code.Common.IsMobile)
                {
                    return Content(Code.Common.Redirect(HttpContext.Request.UrlReferrer.ToString()));
                }

                
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex")));
            }
        }

        /// <summary>
        /// 单点退出
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public void ILoginOut()
        {
            if (System.Web.HttpContext.Current.Request.Cookies[Code.Common.AppName + "SysUserId"] != null)
            {
                System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "SysUserId"].Expires = DateTime.Now.AddDays(-1);
            }

            if (System.Web.HttpContext.Current.Request.Cookies[Code.Common.AppName + "UserType"] != null)
            {
                System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "UserType"].Expires = DateTime.Now.AddDays(-1);
            }

            if (System.Web.HttpContext.Current.Request.Cookies[Code.Common.AppName + "UserName"] != null)
            {
                System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "UserName"].Expires = DateTime.Now.AddDays(-1);
            }

            if (System.Web.HttpContext.Current.Request.Cookies[Code.Common.AppName + "casTicket"] != null)
            {
                System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "casTicket"].Expires = DateTime.Now.AddDays(-1);
            }

            if (System.Web.HttpContext.Current.Request.Cookies[Code.Common.AppName + "UserLevel"] != null)
            {
                System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "UserLevel"].Expires = DateTime.Now.AddDays(-1);
            }

            //System.Web.HttpContext.Current.Request.Cookies[AppName + "SysUserId"].Expires=DateTime.Now;
            //System.Web.HttpContext.Current.Request.Cookies[AppName + "UserType"].Expires = DateTime.Now;
            //System.Web.HttpContext.Current.Request.Cookies[AppName + "UserName"].Expires = DateTime.Now;
            //System.Web.HttpContext.Current.Request.Cookies[AppName + "casTicket"].Expires = DateTime.Now;

            //string IsStartCas = ConfigurationManager.AppSettings.Get("IsStartCas");
            //if (IsStartCas == "true")
            //{
            //    Response.Redirect(ConfigurationManager.AppSettings.Get("LogoutUrl"));
            //}

            //if (string.IsNullOrEmpty(Core.Helper.CookieHelper.GetCookie("UserName")) && string.IsNullOrEmpty(Core.Helper.CookieHelper.GetCookie("casTicket")) && string.IsNullOrEmpty(Core.Helper.CookieHelper.GetCookie("SysUserId")) && string.IsNullOrEmpty(Core.Helper.CookieHelper.GetCookie("UserType")))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        public ActionResult LoginOut(bool isHome = false)
        {
            if (isHome)
            {
                System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "UserId"].Expires = DateTime.Now;
                System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "UserType"].Expires = DateTime.Now;
                System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "UserName"].Expires = DateTime.Now;
                if (System.Web.HttpContext.Current.Request.Cookies[Code.Common.AppName + "casTicket"] != null)
                {
                    System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "casTicket"].Expires = DateTime.Now.AddDays(-1);
                }

                //System.Web.HttpContext.Current.Request.Cookies[AppName + "casTicket"].Expires = DateTime.Now;
                //System.Web.HttpContext.Current.Response.Cookies[AppName + "Account_Code_Ticket"].Expires = DateTime.Now;

                string IsStartCas = ConfigurationManager.AppSettings.Get("IsStartCas");
                if (IsStartCas == "true")
                {
                    if (Code.Common.IsMobile)
                    {
                        return Content(Code.Common.Redirect(Url.Content("~/Login")));
                    }
                    else
                    {
                        Response.Redirect(ConfigurationManager.AppSettings.Get("LogoutUrl") + "?service=" + ConfigurationManager.AppSettings.Get("LcconsoleUrl"));
                    }
                }
            }

            return Content(Code.Common.Redirect(Url.Content("~/Login")));
        }

        public ActionResult Index()
        {
            Models.SysIndex.Index vm = GetUserInfo();

            if (Code.Common.IsMobile)
            {
                //Add,Harvey,xxzx,for demo,20161031
                //if (System.Web.HttpContext.Current.Cache["Power"] == null)
                //{
                //    System.Web.HttpContext.Current.Cache["Power"] = SysRolePowerController.GetPower();
                //}

                //var power = System.Web.HttpContext.Current.Cache["Power"];

                //var tb = (power as List<Dto.SysMenu.Info>).Where(d => d.ProgramId == 1 && d.TenantId == Code.Common.TenantId);
                //if (Code.Common.UserType == Code.EnumHelper.SysUserType.Administrator)
                //{
                //    tb = tb.Where(d => d.UserId == 0);
                //}
                //else
                //{
                //    tb = tb.Where(d => d.UserId == Code.Common.UserId);
                //}

                //vm.SysWechatMenuList = tb.Where(m => !string.IsNullOrEmpty(m.MenuUrl)).Select(m => new Dto.SysMenu.List
                //{
                //    Id = m.Id,
                //    MenuName = m.MenuName,
                //    MenuUrl = m.MenuUrl,
                //    No = m.No
                //}).ToList();
                //return View("mxxzx_Index", vm);
                //Add End

                //return View("m20_Index", vm);//二十

                //if (XkSystem.Code.Common.IsStartCas)
                //{
                //    if (HttpContext.Request.Cookies[Code.Common.AppName + "casTicket"] == null)
                //    {
                //        string rawUrl = HttpContext.Request.RawUrl;
                //        return Redirect("~/Login?returnUrl=" + rawUrl);
                //    }
                //}
                return View("m_Index", vm);//光高

            }
            else
            {
                if (vm.MenuInfo.Count > 12 && false)
                {
                    return View(vm);
                }
                else
                {
                    return View("IndexPad", vm);
                }
            }
        }

        private static Models.SysIndex.Index GetUserInfo()
        {
            var vm = new Models.SysIndex.Index();
            vm.MenuInfo = Sys.Controllers.SysRolePowerController.GetPowerByUser(Code.Common.UserId).Where(d => d.IsShortcut).OrderBy(d => d.No).Take(12).ToList();
            vm.SysMessageList = Areas.Sys.Controllers.SysMessageController.GetLatestMessageList(5);
            vm.SysShortcutList = Areas.Sys.Controllers.SysShortcutController.SelectListByUser(Code.Common.UserId, 8);

            var userInfo = new Dto.SysUser.Info();
            userInfo.Portrait = "~/Content/Images/a5.jpg";
            userInfo.UserName = Code.Common.UserName;
            userInfo.UserTypeName = Code.Common.UserType.GetDescription();

            var sysuser = Areas.Sys.Controllers.SysUserController.Info(Code.Common.UserId);
            userInfo.NeedAlert = sysuser.NeedAlert;
            if (!String.IsNullOrWhiteSpace(sysuser.Photo))
            {
                userInfo.Portrait = "~/Files/UserPhoto/" + sysuser.Photo;
            }

            switch (Code.Common.UserType)
            {
                case Code.EnumHelper.SysUserType.Student:
                    var studentClassInfo = Areas.Basis.Controllers.ClassController.SelectInfoByStudentUserId(Code.Common.UserId);
                    if (studentClassInfo != null)
                    {
                        userInfo.ClassName = studentClassInfo.ClassName;

                        var teacherInfo = Areas.Teacher.Controllers.TeacherController.GetTeacherByClassId(studentClassInfo.Id).FirstOrDefault();
                        if (teacherInfo != null)
                        {
                            userInfo.TeacherName = teacherInfo.TeacherName;
                        }
                    }
                    break;
            }

            vm.UserInfo = userInfo;
            return vm;
        }

        public ActionResult SwitchUser()
        {
            var vm = new Models.SysIndex.SwitchUser();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchUser(Models.SysIndex.SwitchUser vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                              where (p.UserCode == vm.UserCode || p.UserName == vm.UserCode || p.UserCode + "(" + p.UserName + ")" == vm.UserCode)
                              select new
                              {
                                  p.Id,
                                  p.UserType,
                                  p.UserName
                              }).ToList();
                    if (tb.Count > 0)
                    {
                        if (tb.Count == 1)
                        {
                            Code.Common.UserId = tb.FirstOrDefault().Id;
                            Code.Common.UserName = tb.FirstOrDefault().UserName;
                            Code.Common.UserType = tb.FirstOrDefault().UserType;
                        }
                        else
                        {
                            error.Add("查询到多条重复记录，无法确定切换用户!");
                        }
                    }
                    else
                    {
                        error.Add("未查询到相关帐号信息!");
                    }
                }

                return Code.MvcHelper.Post(error, Url.Action("Index", "SysIndex"));
            }
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult Top()
        {
            var vm = new Models.SysIndex.Top();
            vm.ProgramInfo = Admin.Controllers.ProgramController.SelectList();
            vm.MenuInfo = Sys.Controllers.SysRolePowerController.GetPowerByUser(Code.Common.UserId);
            vm.PrivateMyMessageList = Sys.Controllers.SysMessageController.GetPrivateMyMessageList(Code.Common.UserId);
            //vm.AdmitUnReadPrivateMyMessageCount = Admit.Controllers.AdmitMessageController.GetPrivateMyMessageList(Code.Common.UserId).Count;
            return PartialView(vm);
        }

        [AllowAnonymous]
        public ActionResult PostLoginOut()
        {
            System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "UserId"].Expires = DateTime.Now;
            System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "UserType"].Expires = DateTime.Now;
            System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "UserName"].Expires = DateTime.Now;
            if (System.Web.HttpContext.Current.Request.Cookies[Code.Common.AppName + "casTicket"] != null)
            {
                System.Web.HttpContext.Current.Response.Cookies[Code.Common.AppName + "casTicket"].Expires = DateTime.Now.AddDays(-1);
            }
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult Left()
        {
            var vm = new Models.SysIndex.Left();
            vm.MenuInfo = Sys.Controllers.SysRolePowerController.GetPowerByUser(Code.Common.UserId);
            return PartialView(vm);
        }

        [AllowAnonymous]
        public ActionResult GetCheckCode()
        {
            return Content(Code.Common.CreateCheckCode());
        }

        [AllowAnonymous]
        public ActionResult CheckCode(string code = "")
        {
            if (string.IsNullOrEmpty(code))
            {
                code = Code.Common.CreateCheckCode();
            }

            var bytes = Code.Common.CreateCheckGraphic(code);
            return File(bytes, @"image/jpeg");
        }

        [AllowAnonymous]
        public ActionResult Forget()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysIndex.Forget();
                vm.CheckCodeRefer = Code.Common.CreateCheckCode();

                if (HttpContext.Cache["TenantName"] != null)
                {
                    vm.SchoolName = db.TableRoot<Admin.Entity.tbTenant>().FirstOrDefault().TenantName;
                }
                else
                {
                    vm.IsTenant = true;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Forget(Models.SysIndex.Forget vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (string.Compare(vm.CheckCode, vm.CheckCodeRefer, true) != decimal.Zero)
                    {
                        error.AddError("验证码不正确!");
                    }
                    else
                    {
                        var VerifyCount = 0;
                        var tb = from p in db.TableRoot<Sys.Entity.tbSysUser>()
                                    .Include(d => d.tbTenant)
                                 where p.tbTenant.TenantName == vm.SchoolName
                                 select p;
                        if (string.IsNullOrEmpty(vm.UserCode) == false)
                        {
                            VerifyCount = VerifyCount + 1;
                            tb = tb.Where(d => d.UserCode == vm.UserCode);
                        }

                        if (string.IsNullOrEmpty(vm.UserName) == false)
                        {
                            tb = tb.Where(d => d.UserName == vm.UserName);
                        }

                        if (string.IsNullOrEmpty(vm.IdentityNumber) == false)
                        {
                            VerifyCount = VerifyCount + 1;
                            tb = tb.Where(d => d.IdentityNumber == vm.IdentityNumber);
                        }

                        if (string.IsNullOrEmpty(vm.Mobile) == false)
                        {
                            VerifyCount = VerifyCount + 1;
                            tb = tb.Where(d => d.Mobile == vm.Mobile);
                        }

                        if (string.IsNullOrEmpty(vm.Email) == false)
                        {
                            VerifyCount = VerifyCount + 1;
                            tb = tb.Where(d => d.Email == vm.Email);
                        }

                        if (VerifyCount >= 3)
                        {
                            if (tb.Count() == decimal.One)
                            {
                                var user = tb.FirstOrDefault();
                                user.IsLock = false;
                                user.Password = Code.Common.DESEnCode("123456");
                                user.PasswordMd5 = Code.Common.CreateMD5Hash("123456");
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("找回帐号和密码", user.Id, user.tbTenant.Id);
                                }

                                return Code.MvcHelper.Post(error, "", "您的帐号是：" + user.UserCode + "\r\n密码已重置为：123456\r\n请登录系统后更改密码!");
                            }
                        }
                        else
                        {
                            error.AddError("请填写三项以上验证信息!");
                        }
                    }
                }

                return Code.MvcHelper.Post(error, "", "验证信息失败，未查询到相关记录!");
            }
        }

        [Code.FilterHelper.Keygen]
        [AllowAnonymous]
        public ActionResult Upgrade()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var result = "数据库更新成功";

                try
                {
                    System.Data.Entity.Database.SetInitializer(new System.Data.Entity.MigrateDatabaseToLatestVersion<XkSystem.Models.DbContext, XkSystem.Migrations.Configuration>());
                    db.Database.Initialize(true);
                }
                catch (Exception ex)
                {
                    result = "数据库更新失败!" + ex.Message + "<hr>" + ex.InnerException;
                }
                finally
                {
                    System.Data.Entity.Database.SetInitializer<XkSystem.Models.DbContext>(null);
                    this.RestartCache();
                }

                return Content(result);
            }
        }

        [AllowAnonymous]
        public ActionResult Restart()
        {
            this.RestartCache();
            return Content("系统重启成功!已重新获取最新的缓存数据!");
        }

        [AllowAnonymous]
        public ActionResult Guide()
        {
            return View();
        }

        public void RestartCache()
        {
            try
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var cache = System.Web.HttpContext.Current.Cache;

                    var tenant = (from p in db.TableRoot<Areas.Admin.Entity.tbTenant>()
                                  orderby p.IsDefault descending, p.No
                                  select new
                                  {
                                      p.TenantName,
                                      p.Title
                                  }).ToList();
                    if (tenant.Count > decimal.Zero)
                    {
                        cache["AppTitle"] = tenant.FirstOrDefault().Title;

                        if (tenant.Count == 1)
                        {
                            cache["TenantName"] = tenant.FirstOrDefault().TenantName;
                        }
                        else
                        {
                            cache.Remove("TenantName");
                        }
                    }

                    cache["Config"] = Areas.Admin.Controllers.ConfigController.GetConfig();

                    cache["Power"] = SysRolePowerController.GetPower();
                }
            }
            catch
            {

            }
        }

        public JsonResult GetPrevNextDay(DateTime date, string direction, int days)
        {
            DateTime dt = DateTime.Now;
            switch (direction)
            {
                case "PREV":
                    dt = date.AddDays(-1 * days);
                    break;
                case "NEXT":
                    dt = date.AddDays(days);
                    break;
            }

            return Json(new { Date = dt.ToString(XkSystem.Code.Common.StringToDate), DayOfWeek = XkSystem.Code.Common.GetDayOfWeek(dt) }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [Code.FilterHelper.Keygen]
        public ActionResult Keygen()
        {
            var vm = new Models.SysIndex.Keygen();
            vm.MachineCode = Code.Common.getMachineCode();

            return View(vm);
        }

        [AllowAnonymous]
        [Code.FilterHelper.Keygen]
        [HttpPost]
        public ActionResult Keygen(Models.SysIndex.Keygen vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == 0)
                {
                    var cdKey = Code.JsonHelper.FromJsonString<Admin.Dto.Config.Cdkey>(Code.Common.DESDeCode(vm.Cdkey.Replace("\r\n", string.Empty).Trim()));
                    if (cdKey.MachineCode == Code.Common.getMachineCode())
                    {
                        var tb = new Admin.Entity.tbConfig();
                        tb.ConfigName = "序列号";
                        tb.ConfigType = Code.EnumHelper.ConfigType.CdKey;
                        tb.ConfigValue = vm.Cdkey.Trim();
                        db.Set<Admin.Entity.tbConfig>().Add(tb);
                        db.SaveChanges();

                        HttpContext.Cache["Config"] = Areas.Admin.Controllers.ConfigController.GetConfig();

                        return Code.MvcHelper.Post(error, Url.Content("~/"), "注册成功!");
                    }
                    else
                    {
                        error.AddError("注册码错误!");
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }
    }
}