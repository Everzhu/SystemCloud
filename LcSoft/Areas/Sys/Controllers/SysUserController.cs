using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sys.Controllers
{
    public class SysUserController : Controller
    {
        public ActionResult List()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Teacher)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysUser.List();
                if (Request["UserType"] != null)
                {
                    Code.EnumHelper.SysUserType userType;
                    Enum.TryParse(Request["UserType"], out userType);
                    vm.UserType = userType;
                }

                var tb = from p in db.Table<Sys.Entity.tbSysUser>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.UserCode.Contains(vm.SearchText) || d.UserName.Contains(vm.SearchText) || d.IdentityNumber.Contains(vm.SearchText) || d.Mobile.Contains(vm.SearchText) || d.Email.Contains(vm.SearchText) || d.Qq.Contains(vm.SearchText));
                }

                if (vm.UserType != null)
                {
                    tb = tb.Where(d => d.UserType == vm.UserType);
                }
            
                vm.UserList = (from p in tb
                               orderby p.UserName
                               select new Dto.SysUser.List
                               {
                                   Id = p.Id,
                                   UserCode = p.UserCode,
                                   UserName = p.UserName,
                                   UserType = p.UserType,
                                   SexName = p.tbSex.SexName,
                                   Email = p.Email,
                                   IdentityNumber = p.IdentityNumber,
                                   IsDisable = p.IsDisable,
                                   IsLock = p.IsLock,
                                   Mobile = p.Mobile,
                                   Password = p.Password,
                                   Qq = p.Qq,
                               }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SysUser.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { userType = vm.UserType, searchText = vm.SearchText, pageIndex = vm.Page.PageIndex, pageSize = vm.Page.PageSize }));
        }

        /// <summary>
        /// 按用户查询其所对应的角色中的用户列表
        /// </summary>
        public ActionResult UserList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysUser.UserList();
                if (Request["UserType"] != null)
                {
                    Code.EnumHelper.SysUserType userType;
                    Enum.TryParse(Request["UserType"], out userType);
                    vm.UserType = userType;
                }

                var tb = db.Table<Sys.Entity.tbSysUser>();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.UserCode.Contains(vm.SearchText) || d.UserName.Contains(vm.SearchText) || d.IdentityNumber.Contains(vm.SearchText) || d.Mobile.Contains(vm.SearchText) || d.Email.Contains(vm.SearchText) || d.Qq.Contains(vm.SearchText));
                }
                if (vm.UserType != null)
                {
                    tb = tb.Where(d => d.UserType == vm.UserType);
                }

                vm.DataList = (from p in tb
                               where p.UserType != Code.EnumHelper.SysUserType.Administrator
                               orderby p.UserName
                               select new Dto.SysUser.UserList
                               {
                                   Id = p.Id,
                                   IsDisable = p.IsDisable,
                                   IsLock = p.IsLock,
                                   SexName = p.tbSex.SexName,
                                   UserCode = p.UserCode,
                                   UserName = p.UserName
                               }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserList(Models.SysUser.UserList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("UserList", new { userType = vm.UserType, searchText = vm.SearchText, pageIndex = vm.Page.PageIndex, pageSize = vm.Page.PageSize }));
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysUser.Edit();
                vm.SexList = Dict.Controllers.DictSexController.SelectList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                              where p.Id == id
                              select new Dto.SysUser.Edit
                              {
                                  Id = p.Id,
                                  Email = p.Email,
                                  IdentityNumber = p.IdentityNumber,
                                  IsDisable = p.IsDisable,
                                  IsLock = p.IsLock,
                                  Mobile = p.Mobile,
                                  Qq = p.Qq,
                                  SexId = p.tbSex.Id,
                                  UserCode = p.UserCode,
                                  UserName = p.UserName
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.UserEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.SysUser.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                                .Include(d => d.tbSex)
                              where p.Id == vm.UserEdit.Id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.UserCode = vm.UserEdit.UserCode;
                        tb.UserName = vm.UserEdit.UserName;
                        tb.tbSex = db.Set<Dict.Entity.tbDictSex>().Find(vm.UserEdit.SexId);
                        tb.IdentityNumber = vm.UserEdit.IdentityNumber;
                        tb.Email = vm.UserEdit.Email;
                        tb.Mobile = vm.UserEdit.Mobile;
                        tb.Qq = vm.UserEdit.Qq;
                        tb.IsDisable = vm.UserEdit.IsDisable;
                        tb.IsLock = vm.UserEdit.IsLock;
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改用户");
                        }
                    }
                    else
                    {
                        error.AddError(Resources.LocalizedText.MsgNotFound);
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult UserRoleList(int UserId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysUser.UserRoleList();

                vm.DataList = (from p in db.Table<Sys.Entity.tbSysUserRole>()
                               where p.tbSysUser.Id == UserId
                               select new Dto.SysUser.UserRoleList()
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   RoleName = p.tbSysRole.RoleName
                               }).ToList();

                return View(vm);
            }
        }

        public ActionResult ModifyUser()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysUser.Modify();
                vm.SexList = Dict.Controllers.DictSexController.SelectList();

                var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                          where p.Id == Code.Common.UserId
                          select new Dto.SysUser.Modify
                          {
                              Email = p.Email,
                              IdentityNumber = p.IdentityNumber,
                              Mobile = p.Mobile,
                              Qq = p.Qq,
                              SexId = p.tbSex.Id,
                              UserCode = p.UserCode,
                              UserName = p.UserName,
                              Photo = p.Photo
                          }).FirstOrDefault();
                if (tb != null)
                {
                    vm.UserModify = tb;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyUser(Models.SysUser.Modify vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var file = Request.Files["UserModify.Photo"];
                    if (Code.Common.GetFileType(file.FileName) != Code.FileType.Image)
                    {
                        return Content("<script >alert('图片格式必须是jpg、jpeg、png、bmp格式！');history.go(-1);</script >", "text/html");
                    }

                    var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                                .Include(d => d.tbSex)
                              where p.Id == Code.Common.UserId
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.tbSex = db.Set<Dict.Entity.tbDictSex>().Find(vm.UserModify.SexId);
                        tb.IdentityNumber = vm.UserModify.IdentityNumber;
                        tb.Email = vm.UserModify.Email;
                        tb.Mobile = vm.UserModify.Mobile;
                        tb.Qq = vm.UserModify.Qq;
                        if (file.ContentLength > 0)
                        {
                            var fileSave = Server.MapPath("~/Files/UserPhoto/");
                            Random r = new Random();
                            var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + r.Next(10000, 99999).ToString() + "." + file.FileName.Split('.').Last();
                            file.SaveAs(fileSave + fileName);
                            tb.Photo = fileName;
                        }
                        if (db.SaveChanges() > 0)
                        {
                            vm.Status = true;
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了个人用户信息");
                        }
                    }
                    else
                    {
                        error.AddError(Resources.LocalizedText.MsgNotFound);
                    }
                }

                return View(vm);
            }
        }

        public ActionResult PasswordChange()
        {
            return View(new Models.SysUser.PasswordChange());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordChange(Models.SysUser.PasswordChange vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var password = Code.Common.DESEnCode(vm.Password);
                    var passwordMd5 = Code.Common.CreateMD5Hash(vm.Password);
                    var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                              where p.Id == Code.Common.UserId
                                && (p.Password == password || p.PasswordMd5 == passwordMd5)
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        if (tb.UserCode.ToLower() == vm.PasswordNew.ToLower())
                        {
                            error.AddError("密码不能和用户名相同!");
                        }
                        else
                        {
                            tb.Password = Code.Common.DESEnCode(vm.PasswordNew);
                            tb.PasswordMd5 = Code.Common.CreateMD5Hash(vm.PasswordNew);
                            tb.NeedAlert = false;

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改用户密码");
                            }
                        }
                    }
                    else
                    {
                        error.AddError("原密码错误!");
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [NonAction]
        public static Sys.Entity.tbSysUser Info(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                          where p.Id == id
                          select p).FirstOrDefault();
                if (tb != null)
                {
                    return tb;
                }
                else
                {
                    return new Sys.Entity.tbSysUser();
                }
            }
        }



        public JsonResult SetNeedAlert()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var sysUser = db.Set<Entity.tbSysUser>().Find(Code.Common.UserId);
                if (sysUser != null)
                {
                    sysUser.NeedAlert = false;
                }
                db.SaveChanges();
                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(List<int> ids)
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Teacher)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.Password = Code.Common.DESEnCode("123456");
                    a.PasswordMd5 = Code.Common.CreateMD5Hash("123456");
                    a.NeedAlert = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("重置用户密码");
                }

                return Code.MvcHelper.Post(null, "", "密码重置成功。新密码为：123456");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPasswordById(int id)
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Teacher)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                          where p.Id == id
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.Password = Code.Common.DESEnCode("123456");
                    a.PasswordMd5 = Code.Common.CreateMD5Hash("123456");
                    a.NeedAlert = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("重置用户密码");
                }

                return Code.MvcHelper.Post(null, "", "密码重置成功。新密码为：123456");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnLock(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsLock = false;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("锁定用户");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approval(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDisable = false;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("禁用用户");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetLock(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Sys.Entity.tbSysUser>().Find(id);
                if (tb != null)
                {
                    tb.IsLock = !tb.IsLock;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("设置用户锁");
                }

                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disable(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDisable = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("解禁用户");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDisable(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Sys.Entity.tbSysUser>().Find(id);
                if (tb != null)
                {
                    tb.IsDisable = !tb.IsDisable;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("设置用户禁用");
                    }
                }

                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }

        [AllowAnonymous]
        public ActionResult GetUser(string q)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                q = q.ConvertToString();
                var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                          where (p.UserCode.Contains(q) || p.UserName.Contains(q) || p.IdentityNumber.Contains(q) || p.Mobile.Contains(q) || p.Email.Contains(q) || p.Qq.Contains(q))
                          orderby p.UserName
                          select p.UserCode + "(" + p.UserName + ")").Take(10).ToList();

                return Json(tb, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SelectUser()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysUser.SelectUser();
                var tb = from p in db.Table<Sys.Entity.tbSysUser>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.UserCode.Contains(vm.SearchText) || d.UserName.Contains(vm.SearchText));
                }

                vm.UserList = (from p in tb
                               orderby p.UserName
                               select new Dto.SysUser.SelectUser
                               {
                                   Id = p.Id,
                                   UserCode = p.UserCode,
                                   UserName = p.UserName,
                                   SexName = p.tbSex.SexName
                               }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectUser(Models.SysUser.SelectUser vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SelectUser", new { searchText = vm.SearchText, pageIndex = vm.Page.PageIndex, pageSize = vm.Page.PageSize }));
        }

        public static List<System.Web.Mvc.SelectListItem> SelectList(Code.EnumHelper.SysUserType userType, int id = 0)
        {
            var list = new List<System.Web.Mvc.SelectListItem>();

            using (var db = new XkSystem.Models.DbContext())
            {
                list = (from p in db.Table<Sys.Entity.tbSysUser>()
                        where p.UserType == userType
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem()
                        {
                            Value = p.Id.ToString(),
                            Text = p.UserName + "(" + p.UserCode + ")"
                        }).ToList();

                if (id > 0)
                {
                    list.Where(d => d.Value == id.ToString()).FirstOrDefault().Selected = true;
                }
            }

            return list;
        }
    }
}