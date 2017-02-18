using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sys.Controllers
{
    public class SysRolePowerController : Controller
    {
        public ActionResult Edit()
        {
            var vm = new Sys.Models.SysRolePower.Edit();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.SysRolePower.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var power = (from p in db.Table<Sys.Entity.tbSysRolePower>()
                             where p.tbSysRole.Id == vm.RoleId
                             select p).ToList();
                foreach (var a in power)
                {
                    a.IsDeleted = true;
                }

                if (string.IsNullOrEmpty(vm.Power) == false)
                {
                    var menuList = vm.Power.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt());

                    foreach (var menu in menuList.Where(d => d != 0))
                    {
                        var temp = new Sys.Entity.tbSysRolePower();
                        temp.tbSysMenu = db.Set<Sys.Entity.tbSysMenu>().Find(menu);
                        temp.tbSysRole = db.Set<Sys.Entity.tbSysRole>().Find(vm.RoleId);
                        db.Set<Sys.Entity.tbSysRolePower>().Add(temp);
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加角色权限");
                    System.Web.HttpContext.Current.Cache["Power"] = SysRolePowerController.GetPower();
                }

                return Code.MvcHelper.Post(null, "", "保存成功!");
            }
        }

        public ActionResult GetRoleTree(int roleId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var menus = (from p in db.Table<Sys.Entity.tbSysMenu>()
                                .Include(d => d.tbMenuParent)
                             where p.tbProgram.Id == Code.Common.ProgramId
                                && p.IsDisable == false
                             select p).ToList();

                var rolePower = (from p in db.Table<Sys.Entity.tbSysRolePower>()
                                 where p.tbSysRole.Id == roleId
                                 select p.tbSysMenu.Id).ToList();

                var result = new List<Code.TreeHelper>();

                foreach (var menu in menus.Where(m => m.tbMenuParent == null).OrderBy(d => d.No))
                {
                    result.Add(new Code.TreeHelper()
                    {
                        Id = menu.Id,
                        name = menu.MenuName,
                        open = true,
                        isChecked = rolePower.Where(d => d == menu.Id).Any(),
                        children = AddChildrenTreeByMenuId(menus.Where(m => m.tbMenuParent != null).ToList(), menu.Id, rolePower)
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

        private List<Code.TreeHelper> AddChildrenTreeByMenuId(List<Sys.Entity.tbSysMenu> menus, int menuParentId, List<int> rolePower)
        {
            List<Code.TreeHelper> roleTree = new List<Code.TreeHelper>();

            foreach (var menu in menus.Where(d => d.tbMenuParent.Id == menuParentId).OrderBy(d => d.No))
            {
                List<Code.TreeHelper> cn = AddChildrenTreeByMenuId(menus, menu.Id, rolePower);

                roleTree.Add(new Code.TreeHelper()
                {
                    Id = menu.Id,
                    pId = menuParentId,
                    name = menu.MenuName,
                    open = true,
                    isChecked = rolePower.Where(d => d == menu.Id).Any(),
                    children = cn
                });
            }

            return roleTree;
        }

        [NonAction]
        public static List<Dto.SysMenu.Info> GetPowerByUser(int userId)
        {
            if (System.Web.HttpContext.Current.Cache["Power"] == null)
            {
                System.Web.HttpContext.Current.Cache["Power"] = GetPower();
            }

            var power = System.Web.HttpContext.Current.Cache["Power"];

            var tb = (power as List<Dto.SysMenu.Info>).Where(d => d.ProgramId == Code.Common.ProgramId && d.TenantId == Code.Common.TenantId);
            if (Code.Common.UserType == Code.EnumHelper.SysUserType.Administrator)
            {
                tb = tb.Where(d => d.UserId == 0);
            }
            else
            {
                tb = tb.Where(d => d.UserId == Code.Common.UserId);
            }

            return tb.ToList();
        }

        [NonAction]
        public static List<Dto.SysMenu.Info> GetPower()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var allMenu = SysMenuController.AllMenuList();
                 
                var userPower = (from p in db.TableRoot<Sys.Entity.tbSysUserPower>()
                                 where p.tbSysMenu.IsDeleted == false
                                    && p.tbSysMenu.IsDisable == false
                                 select new Dto.SysMenu.Info
                                 {
                                     Id = p.tbSysMenu.Id,
                                     No =  (p.tbSysMenu.tbMenuParent != null ? 1000 * p.tbSysMenu.tbMenuParent.No : 1) + p.tbSysMenu.No,
                                     MenuName = p.tbSysMenu.MenuName,
                                     MenuUrl = p.tbSysMenu.MenuUrl,
                                     MenuParentId = p.tbSysMenu.tbMenuParent.Id,
                                     Icon = p.tbSysMenu.Icon,
                                     ProgramId = p.tbSysMenu.tbProgram.Id,
                                     TenantId = p.tbTenant.Id,
                                     UserId = p.tbSysUser.Id,
                                     IsShortcut = p.tbSysMenu.IsShortcut,
                                     Remark = p.tbSysMenu.Remark
                                 }).ToList();

                var rolePower = (from p in db.TableRoot<Sys.Entity.tbSysRolePower>()
                                 join q in db.TableRoot<Sys.Entity.tbSysUserRole>()
                                 on p.tbSysRole.Id equals q.tbSysRole.Id
                                 where p.tbSysRole.IsDeleted == false
                                    && p.tbSysMenu.IsDeleted == false
                                    && p.tbSysMenu.IsDisable == false
                                 select new Dto.SysMenu.Info
                                 {
                                     Id = p.tbSysMenu.Id,
                                     No = (p.tbSysMenu.tbMenuParent != null ? 1000 * p.tbSysMenu.tbMenuParent.No : 1) + p.tbSysMenu.No,
                                     MenuName = p.tbSysMenu.MenuName,
                                     MenuUrl = p.tbSysMenu.MenuUrl,
                                     MenuParentId = p.tbSysMenu.tbMenuParent.Id,
                                     Icon = p.tbSysMenu.Icon,
                                     ProgramId = p.tbSysMenu.tbProgram.Id,
                                     TenantId = p.tbTenant.Id,
                                     UserId = q.tbSysUser.Id,
                                     IsShortcut = p.tbSysMenu.IsShortcut,
                                     Remark = p.tbSysMenu.Remark
                                 }).ToList();

                var tb = (from p in userPower.Union(rolePower).Union(allMenu)
                          group p by new { p.Id, p.No, p.MenuName, p.MenuUrl, p.MenuParentId, p.ProgramId, p.TenantId, p.UserId, p.Icon, p.IsShortcut, p.Remark } into g
                          orderby g.Key.No
                          select new Dto.SysMenu.Info
                          {
                              Id = g.Key.Id,
                              No = g.Key.No,
                              MenuName = g.Key.MenuName,
                              MenuUrl = g.Key.MenuUrl,
                              MenuParentId = g.Key.MenuParentId,
                              Icon = g.Key.Icon,
                              ProgramId = g.Key.ProgramId,
                              TenantId = g.Key.TenantId,
                              UserId = g.Key.UserId,
                              IsShortcut = g.Key.IsShortcut,
                              Remark = g.Key.Remark
                          }).ToList();

                return tb;
            }
        }
    }
}