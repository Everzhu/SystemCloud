using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sys.Controllers
{
    public class SysRoleController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysRole.List();
                var tb = from p in db.Table<Sys.Entity.tbSysRole>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.RoleName.Contains(vm.SearchText));
                }

                vm.RoleList = (from p in tb
                               orderby p.RoleName
                               select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SysRole.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        public ActionResult MenuList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysRole.MenuList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MenuList(Models.SysRole.MenuList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("MenuList", new { searchText = vm.SearchText }));
        }

        public ActionResult MenuRoleList(int MenuId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysRole.MenuRoleList();
                vm.MenuId = MenuId;

                vm.DataList = (from p in db.Table<Sys.Entity.tbSysRole>()
                               where p.RoleCode != Code.EnumHelper.SysRoleCode.Administrator
                               select new Dto.SysRole.MenuRoleList()
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   RoleCode = p.RoleCode,
                                   RoleName = p.RoleName,
                                   IsEnabled = false
                               }).ToList();

                var menuRoleList = db.Table<Sys.Entity.tbSysRolePower>()
                    .Where(d => d.tbSysMenu.Id == MenuId)
                    .Include(d => d.tbSysRole).ToList();

                foreach (var v in vm.DataList)
                {
                    if (menuRoleList.Where(d => d.tbSysRole.Id == v.Id).Count() > 0)
                    {
                        v.IsEnabled = true;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDisable(int RoleId, int MenuId, bool Status)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var menuIds = GetChildrenMenuId(MenuId);

                if (Status)//授权
                {
                    //如果菜单不是一级菜单，激活时同时激活它的一级菜单
                    if (db.Set<Entity.tbSysMenu>().Find(MenuId).tbMenuParent != null)
                    {
                        var id = db.Table<Entity.tbSysMenu>()
                            .Include(d => d.tbMenuParent)
                            .Where(d => d.Id == MenuId).FirstOrDefault().tbMenuParent.Id;
                        menuIds.Add(id);
                    }

                    var tb = db.Table<Sys.Entity.tbSysRolePower>()
                        .Include(d => d.tbSysMenu)
                        .Where(d => d.tbSysRole.Id == RoleId).ToList();
                    
                    var tempList = new List<Sys.Entity.tbSysRolePower>();
                    foreach (var v in menuIds)
                    {
                        if (tb.Where(d => d.tbSysMenu.Id == v).Count() == 0)
                        {
                            var temp = new Sys.Entity.tbSysRolePower()
                            {
                                tbSysMenu = db.Set<Sys.Entity.tbSysMenu>().Find(v),
                                tbSysRole = db.Set<Sys.Entity.tbSysRole>().Find(RoleId)
                            };
                            tempList.Add(temp);
                        }
                    }
                    db.Set<Sys.Entity.tbSysRolePower>().AddRange(tempList);
                }
                else//取消授权
                {
                    var tb = db.Table<Sys.Entity.tbSysRolePower>()
                    .Where(d => menuIds.Contains(d.tbSysMenu.Id) && d.tbSysRole.Id == RoleId).ToList();
                    foreach (var v in tb)
                    {
                        v.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了角色权限");
                    System.Web.HttpContext.Current.Cache["Power"] = SysRolePowerController.GetPower();
                }

                return Code.MvcHelper.Post(null, Url.Action("MenuRoleList", new { MenuId = MenuId }));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysRole>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var rolePowerList = (from p in db.Table<Sys.Entity.tbSysRolePower>()
                                        .Include(d => d.tbSysRole)
                                     where ids.Contains(p.tbSysRole.Id)
                                     select p).ToList();

                var roleUserList = (from p in db.Table<Sys.Entity.tbSysUserRole>()
                                        .Include(d => d.tbSysRole)
                                    where ids.Contains(p.tbSysRole.Id)
                                    select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    foreach (var power in rolePowerList.Where(d => d.tbSysRole.Id == a.Id))
                    {
                        power.IsDeleted = true;
                    }

                    foreach (var user in roleUserList.Where(d => d.tbSysRole.Id == a.Id))
                    {
                        user.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除角色");
                    System.Web.HttpContext.Current.Cache["Power"] = SysRolePowerController.GetPower();
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysRole.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Sys.Entity.tbSysRole>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.RoleEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.SysRole.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Sys.Entity.tbSysRole>().Where(d => d.RoleName == vm.RoleEdit.RoleName && d.Id != vm.RoleEdit.Id).Any())
                    {
                        error.AddError("该角色已存在!");
                    }
                    else
                    {
                        if (vm.RoleEdit.Id == 0)
                        {
                            var tb = new Sys.Entity.tbSysRole();
                            tb.RoleName = vm.RoleEdit.RoleName;
                            db.Set<Sys.Entity.tbSysRole>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加角色");
                                System.Web.HttpContext.Current.Cache["Power"] = SysRolePowerController.GetPower();
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Sys.Entity.tbSysRole>()
                                      where p.Id == vm.RoleEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.RoleName = vm.RoleEdit.RoleName;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改角色");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysRole>()
                          orderby p.RoleName
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.RoleName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        public ActionResult GetRoleTree()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var menus = (from p in db.Table<Sys.Entity.tbSysMenu>()
                                .Include(d => d.tbMenuParent)
                             where p.tbProgram.Id == Code.Common.ProgramId
                                && p.IsDisable == false
                             select p).ToList();

                var result = new List<Code.TreeHelper>();

                foreach (var menu in menus.Where(m => m.tbMenuParent == null).OrderBy(d => d.No))
                {
                    result.Add(new Code.TreeHelper()
                    {
                        Id = menu.Id,
                        name = menu.MenuName,
                        open = true,
                        isChecked = false,
                        children = AddChildrenTreeByMenuId(menus.Where(m => m.tbMenuParent != null).ToList(), menu.Id)
                    });
                }

                var treeList = new List<Code.TreeHelper>();
                var root = new Code.TreeHelper();
                root.name = "全部";
                root.Id = 0;
                root.open = true;
                root.isChecked = false;
                root.children = result;
                treeList.Add(root);

                return Json(treeList, JsonRequestBehavior.AllowGet);
            }
        }

        private List<Code.TreeHelper> AddChildrenTreeByMenuId(List<Sys.Entity.tbSysMenu> menus, int menuParentId)
        {
            List<Code.TreeHelper> roleTree = new List<Code.TreeHelper>();

            foreach (var menu in menus.Where(d => d.tbMenuParent.Id == menuParentId).OrderBy(d => d.No))
            {
                List<Code.TreeHelper> cn = AddChildrenTreeByMenuId(menus, menu.Id);

                roleTree.Add(new Code.TreeHelper()
                {
                    Id = menu.Id,
                    pId = menuParentId,
                    name = menu.MenuName,
                    open = true,
                    isChecked = false,//rolePower.Where(d => d == menu.Id).Any(),
                    children = cn
                });
            }

            return roleTree;
        }

        public List<int> GetChildrenMenuId(int MenuId)
        {
            var list = new List<int>();
            list.Add(MenuId);
            using (var db = new XkSystem.Models.DbContext())
            {
                var menuList = db.Table<Sys.Entity.tbSysMenu>()
                    .Where(d => d.tbMenuParent.Id == MenuId).ToList();
                if (menuList.Count > 0)
                {
                    foreach (var v in menuList)
                    {
                        var listTemp = GetChildrenMenuId(v.Id);
                        list.AddRange(listTemp);
                    }
                }
            }
            return list;
        }
    }
}