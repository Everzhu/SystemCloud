using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sys.Controllers
{
    public class SysMenuController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysMenu.List();
                var tb = from p in db.Table<Sys.Entity.tbSysMenu>()
                         where p.tbProgram.Id == Code.Common.ProgramId
                         //&& p.tbSysUserType.Id == vm.UserTypeId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.MenuName.Contains(vm.SearchText));
                }

                if (vm.ParentId == 0)
                {
                    tb = tb.Where(d => d.tbMenuParent == null);
                }
                else
                {
                    tb = tb.Where(d => d.tbMenuParent.Id == vm.ParentId);
                }

                vm.MenuList = (from p in tb
                               orderby p.No
                               select new Dto.SysMenu.List
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   IsDisable = p.IsDisable,
                                   IsShortcut = p.IsShortcut,
                                   MenuName = p.MenuName,
                                   MenuParentName = p.tbMenuParent.MenuName,
                                   Icon = p.Icon,
                                   MenuUrl = p.MenuUrl
                               }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SysMenu.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { parentId = vm.ParentId, searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysMenu>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    //级联删除相关菜单
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除菜单");
                }

                var cache = System.Web.HttpContext.Current.Cache;
                cache["Power"] = SysRolePowerController.GetPower();

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int parentId = 0, int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysMenu.Edit();
                vm.MenuEdit.MenuParentId = parentId;
                vm.ParentMenuList = Sys.Controllers.SysMenuController.SelectList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Sys.Entity.tbSysMenu>()
                              where p.Id == id
                              select new Dto.SysMenu.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  IsDisable = p.IsDisable,
                                  IsShortcut = p.IsShortcut,
                                  MenuName = p.MenuName,
                                  MenuParentId = p.tbMenuParent.Id,
                                  MenuParentName = p.tbMenuParent.MenuName,
                                  Icon = p.Icon,
                                  MenuUrl = p.MenuUrl,
                                  Remark = p.Remark
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.MenuEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.SysMenu.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.MenuEdit.Id == 0)
                    {
                        var tb = new Sys.Entity.tbSysMenu();
                        tb.No = vm.MenuEdit.No == null ? db.Table<Sys.Entity.tbSysMenu>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MenuEdit.No;
                        tb.MenuName = vm.MenuEdit.MenuName;
                        tb.tbProgram = db.Set<Admin.Entity.tbProgram>().Find(Code.Common.ProgramId);
                        tb.MenuUrl = vm.MenuEdit.MenuUrl;
                        tb.tbMenuParent = db.Set<Sys.Entity.tbSysMenu>().Find(vm.MenuEdit.MenuParentId);
                        tb.Icon = vm.MenuEdit.Icon;
                        tb.IsDisable = vm.MenuEdit.IsDisable;
                        tb.IsShortcut = vm.MenuEdit.IsShortcut;
                        tb.Remark = vm.MenuEdit.Remark;
                        db.Set<Sys.Entity.tbSysMenu>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加菜单");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Sys.Entity.tbSysMenu>()
                                  where p.Id == vm.MenuEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.MenuEdit.No == null ? db.Table<Sys.Entity.tbSysMenu>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MenuEdit.No;
                            tb.MenuName = vm.MenuEdit.MenuName;
                            tb.tbProgram = db.Set<Admin.Entity.tbProgram>().Find(Code.Common.ProgramId);
                            tb.MenuUrl = vm.MenuEdit.MenuUrl;
                            tb.tbMenuParent = db.Set<Sys.Entity.tbSysMenu>().Find(vm.MenuEdit.MenuParentId);
                            tb.Icon = vm.MenuEdit.Icon;
                            tb.IsDisable = vm.MenuEdit.IsDisable;
                            tb.IsShortcut = vm.MenuEdit.IsShortcut;
                            tb.Remark = vm.MenuEdit.Remark;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改菜单");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                var cache = System.Web.HttpContext.Current.Cache;
                cache["Power"] = SysRolePowerController.GetPower();

                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDisable(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Sys.Entity.tbSysMenu>().Find(id);
                if (tb != null)
                {
                    tb.IsDisable = !tb.IsDisable;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改菜单状态");
                    }
                }

                var cache = System.Web.HttpContext.Current.Cache;
                cache["Power"] = SysRolePowerController.GetPower();

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetShortcut(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Sys.Entity.tbSysMenu>().Find(id);
                if (tb != null)
                {
                    tb.IsShortcut = !tb.IsShortcut;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改菜单快捷");
                    }
                }

                var cache = System.Web.HttpContext.Current.Cache;
                cache["Power"] = SysRolePowerController.GetPower();

                return Code.MvcHelper.Post();
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int parentId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysMenu>()
                          where ((p.tbMenuParent.Id == parentId && parentId != 0) || (parentId == 0 && p.tbMenuParent == null))
                            && p.tbProgram.Id == Code.Common.ProgramId
                            && p.IsDisable == false
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.MenuName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        public static List<Dto.SysMenu.Info> AllMenuList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.TableRoot<Entity.tbSysMenu>()
                          where p.IsDisable == false
                          orderby p.No
                          select new Dto.SysMenu.Info
                          {
                              Id = p.Id,
                              No = (p.tbMenuParent != null ? 1000 * p.tbMenuParent.No : 1) + p.No,
                              MenuName = p.MenuName,
                              MenuUrl = p.MenuUrl,
                              IsShortcut = p.IsShortcut,
                              MenuParentId = p.tbMenuParent.Id,
                              Icon = p.Icon,
                              ProgramId = p.tbProgram.Id,
                              TenantId = p.tbTenant.Id,
                              Remark = p.Remark
                          }).ToList();
                return tb;
            }
        }

        public ActionResult GetMenuTree()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysMenu>()
                            .Include(d => d.tbMenuParent)
                            .Where(d => d.tbProgram.Id == Code.Common.ProgramId)
                          select p).ToList();

                var all = new List<Code.TreeHelper>();
                foreach (var v in tb.Where(d => d.tbMenuParent == null).OrderBy(d => d.No).ThenBy(d => d.No))
                {
                    var cn = MenuDeep(tb, v.Id);

                    all.Add(new Code.TreeHelper() { name = v.MenuName, Id = v.Id, open = true, children = cn });
                }

                //var treeList = new List<Code.TreeHelper>();
                //var root = new Code.TreeHelper();
                //root.name = "全部";
                //root.Id = 0;
                //root.open = true;
                //root.isChecked = false;
                //root.children = all;
                //treeList.Add(root);

                return Json(all, JsonRequestBehavior.AllowGet);
            }
        }

        private static List<Code.TreeHelper> MenuDeep(List<Sys.Entity.tbSysMenu> menuList, int parentId)
        {
            var pn = new List<Code.TreeHelper>();

            foreach (var v in menuList.Where(d => d.tbMenuParent != null && d.tbMenuParent.Id == parentId).OrderBy(d => d.No).ThenBy(d => d.No))
            {
                var cn = MenuDeep(menuList, v.Id);

                pn.Add(new Code.TreeHelper() { name = v.MenuName, Id = v.Id, children = cn });
            }

            return pn;
        }
    }
}