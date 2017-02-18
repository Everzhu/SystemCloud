using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sys.Controllers
{
    public class SysUserRoleController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysUserRole.List();

                vm.RoleName = db.Set<Sys.Entity.tbSysRole>().Find(vm.RoleId).RoleName;

                var tb = from p in db.Table<Sys.Entity.tbSysUserRole>()
                         where p.tbSysRole.Id == vm.RoleId
                            && p.tbSysUser.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbSysUser.UserCode.Contains(vm.SearchText) || d.tbSysUser.UserName.Contains(vm.SearchText));
                }

                vm.SysUserRoleList = (from p in tb
                                      orderby p.tbSysUser.UserCode
                                      select new Dto.SysUserRole.List
                                      {
                                          Id = p.Id,
                                          SysUserCode = p.tbSysUser.UserCode,
                                          SysUserName = p.tbSysUser.UserName
                                      }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SysUserRole.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, RoleId = vm.RoleId, pageIndex = vm.Page.PageIndex, pageSize = vm.Page.PageSize }));
        }

        /// <summary>
        /// 按用户查询其所对应的角色中的用户列表
        /// </summary>
        public ActionResult RoleList(int UserId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysUserRole.RoleList();
                vm.UserId = UserId;
                vm.UserName = db.Set<Sys.Entity.tbSysUser>().Find(vm.UserId).UserName;
                var userRoleList = db.Table<Sys.Entity.tbSysUserRole>()
                    .Include(d => d.tbSysRole)
                    .Where(d => d.tbSysUser.Id == UserId).ToList();

                var tb = db.Set<Sys.Entity.tbSysRole>();
                vm.DataList = (from p in db.Set<Sys.Entity.tbSysRole>()
                               where p.RoleCode != Code.EnumHelper.SysRoleCode.Administrator
                               select new Dto.SysUserRole.RoleList()
                               {
                                   Id = p.Id,
                                   RoleName = p.RoleName
                               }).ToList();

                foreach (var v in vm.DataList)
                {
                    if (userRoleList.Where(d => d.tbSysRole.Id == v.Id).Count() > 0)
                    {
                        v.IsHas = true;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleList(Models.SysUserRole.RoleList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("RoleList"));
        }

        public ActionResult UserRoleList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysRole.MenuList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoleList(Models.SysRole.MenuList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("UserRoleList", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysUserRole>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除用户角色");
                    System.Web.HttpContext.Current.Cache["Power"] = SysRolePowerController.GetPower();
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysUserRole.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Sys.Entity.tbSysUserRole>()
                              where p.Id == id
                              select new Dto.SysUserRole.Edit
                              {
                                  Id = p.Id,
                                  SysRoleId = p.tbSysRole.Id,
                                  SysUserId = p.tbSysUser.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.SysUserRoleEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.SysUserRole.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.SysUserRoleEdit.Id == 0)
                    {
                        var tb = new Sys.Entity.tbSysUserRole();
                        tb.tbSysRole = db.Set<Sys.Entity.tbSysRole>().Find("");
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find("");
                        db.Set<Sys.Entity.tbSysUserRole>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加用户角色");
                            System.Web.HttpContext.Current.Cache["Power"] = SysRolePowerController.GetPower();
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Sys.Entity.tbSysUserRole>()
                                  where p.Id == vm.SysUserRoleEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.tbSysRole = db.Set<Sys.Entity.tbSysRole>().Find("");
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find("");
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("更新用户角色");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(List<int> ids, int roleId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var vm = new Models.SysUser.Edit();

                var userRoleList = (from p in db.Table<Sys.Entity.tbSysUserRole>()
                                    where p.tbSysRole.Id == roleId
                                    select p.tbSysUser.Id).ToList();

                var userList = (from p in db.Table<Sys.Entity.tbSysUser>()
                                where ids.Contains(p.Id) 
                                    && userRoleList.Contains(p.Id) == false
                                select p).ToList();
                foreach (var user in userList)
                {
                    var tb = new Sys.Entity.tbSysUserRole();
                    tb.tbSysRole = db.Set<Sys.Entity.tbSysRole>().Find(roleId);
                    tb.tbSysUser = user;
                    db.Set<Sys.Entity.tbSysUserRole>().Add(tb);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了角色成员");
                    System.Web.HttpContext.Current.Cache["Power"] = SysRolePowerController.GetPower();
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert1(List<int> ids, int UserId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                var user = db.Set<Sys.Entity.tbSysUser>().Find(UserId);
                var userRoleList = db.Table<Sys.Entity.tbSysUserRole>()
                    .Where(d => d.tbSysUser.Id == UserId).ToList();
                foreach (var v in userRoleList)
                {
                    v.IsDeleted = true;
                }
                var tbUserRoleList = new List<Sys.Entity.tbSysUserRole>();
                foreach (var v in ids)
                {
                    tbUserRoleList.Add(new Sys.Entity.tbSysUserRole()
                    {
                        tbSysRole = db.Set<Sys.Entity.tbSysRole>().Find(v),
                        tbSysUser = user
                    });
                }

                db.Set<Sys.Entity.tbSysUserRole>().AddRange(tbUserRoleList);

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了角色成员");
                    System.Web.HttpContext.Current.Cache["Power"] = SysRolePowerController.GetPower();
                }

                return Code.MvcHelper.Post(error);
            }
        }

        public static void GenerateUserRole(Code.EnumHelper.SysRoleCode roleCode)
        {
            using (var db = new XkSystem.Models.DbContext())
            {

            }
        }
    }
}