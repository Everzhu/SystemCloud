using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sys.Controllers
{
    public class SysShortcutController : Controller
    {
        public ActionResult Edit()
        {
            var vm = new Sys.Models.SysShortcut.Edit();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.SysShortcut.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var power = (from p in db.Table<Sys.Entity.tbSysShortcut>()
                             where p.tbSysUser.Id == Code.Common.UserId
                             select p).ToList();
                foreach (var a in power)
                {
                    a.IsDeleted = true;
                }

                if (!String.IsNullOrEmpty(vm.Power))
                {
                    var menuIds = vm.Power.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt());
                    var tbMenuList = (from p in db.Table<Sys.Entity.tbSysMenu>()
                                      where menuIds.Contains(p.Id)
                                        && string.IsNullOrEmpty(p.MenuUrl) == false
                                      select p).ToList();
                    if (tbMenuList.Count() > 8)
                    {
                        error.AddError("常用功能不能超过8项！");
                    }
                    else
                    {
                        foreach (var menu in tbMenuList)
                        {
                            var temp = new Sys.Entity.tbSysShortcut();
                            temp.tbSysMenu = menu;
                            temp.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            db.Set<Sys.Entity.tbSysShortcut>().Add(temp);
                        }
                    }
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加常用功能");
                }
                return Code.MvcHelper.Post(error, "", "保存成功!");
            }
        }

        public ActionResult GetRoleTree(int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var menus = Areas.Sys.Controllers.SysRolePowerController.GetPowerByUser(userId);
                var tb = (from p in db.Table<Sys.Entity.tbSysShortcut>()
                          where p.tbSysUser.Id == userId
                          select p.tbSysMenu.Id).ToList();
                var result = new List<Code.TreeHelper>();

                foreach (Dto.SysMenu.Info menu in menus.Where(m => m.MenuParentId == null).OrderBy(d => d.No))
                {
                    var temp = tb.Where(t => t == menu.Id).FirstOrDefault();

                    result.Add(new Code.TreeHelper()
                    {
                        Id = menu.Id,
                        name = menu.MenuName,
                        open = true,
                        isChecked = temp != 0,
                        children = AddChildrenTreeByMenuId(menus.Where(m => m.MenuParentId != null).ToList(), menu.Id, tb)
                    });
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        private List<Code.TreeHelper> AddChildrenTreeByMenuId(List<Dto.SysMenu.Info> menus, int menuParentId, List<int> containMenus)
        {
            List<Code.TreeHelper> roleTree = new List<Code.TreeHelper>();

            foreach (var menu in menus.Where(d => d.MenuParentId == menuParentId).OrderBy(d => d.No))
            {
                List<Code.TreeHelper> cn = AddChildrenTreeByMenuId(menus, menu.Id, containMenus);

                var temp = containMenus.Where(t => t == menu.Id).FirstOrDefault();

                roleTree.Add(new Code.TreeHelper()
                {
                    Id = menu.Id,
                    pId = menuParentId,
                    name = menu.MenuName,
                    open = true,
                    isChecked = temp != 0,
                    children = cn
                });
            }

            return roleTree;
        }

        public static List<Dto.SysMenu.Info> SelectListByUser(int userId, int records)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var menuIds = Areas.Sys.Controllers.SysRolePowerController.GetPowerByUser(userId).Select(m => m.Id);
                var tb = (from p in db.Table<Sys.Entity.tbSysShortcut>()
                          where p.tbSysUser.Id == userId
                            //&& p.tbSysMenu.tbMenuParent != null
                            && menuIds.Contains(p.tbSysMenu.Id)
                          orderby p.tbSysMenu.tbMenuParent.No, p.tbSysMenu.No
                          select new Dto.SysMenu.Info
                          {
                              Id = p.tbSysMenu.Id,
                              MenuName = p.tbSysMenu.MenuName,
                              MenuUrl = p.tbSysMenu.MenuUrl
                          }).Take(records).ToList();

                return tb;
            }
        }
    }
}