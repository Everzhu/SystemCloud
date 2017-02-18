using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sys.Controllers
{
    public class SysUserPowerController : Controller
    {
        public ActionResult Edit()
        {
            var vm = new Sys.Models.SysUserPower.Edit();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.SysUserPower.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var power = (from p in db.Table<Sys.Entity.tbSysUserPower>()
                             where p.tbSysUser.Id == vm.UserId
                             select p).ToList();
                foreach (var a in power)
                {
                    a.IsDeleted = true;
                }

                var menuList = vm.Power.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt());

                foreach (var menu in menuList)
                {
                    var temp = new Sys.Entity.tbSysUserPower();
                    temp.tbSysMenu = db.Set<Sys.Entity.tbSysMenu>().Find(menu);
                    temp.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(vm.UserId);
                    db.Set<Sys.Entity.tbSysUserPower>().Add(temp);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加角色权限");
                    System.Web.HttpContext.Current.Cache["Power"] = SysRolePowerController.GetPower();
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult GetUserTree(int UserId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var menus = (from p in db.Table<Sys.Entity.tbSysMenu>()
                             select p).ToList();


                var tb = (from p in db.Table<Sys.Entity.tbSysUserPower>()
                                where p.tbSysUser.Id == UserId
                                select p.tbSysMenu.Id).ToList();

                var result = new List<Code.TreeHelper>();

                foreach (var menu in menus.Where(m => m.tbMenuParent == null).OrderBy(d => d.No))
                {
                    var temp = tb.Where(t => t == menu.Id).FirstOrDefault();

                    result.Add(new Code.TreeHelper()
                    {
                        Id = menu.Id,
                        name = menu.MenuName,
                        open = true,
                        isChecked = temp != 0,
                        children = AddChildrenTreeByMenuId(menus.Where(m => m.tbMenuParent != null).ToList(), menu.Id, tb)
                    });
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        private List<Code.TreeHelper> AddChildrenTreeByMenuId(List<Sys.Entity.tbSysMenu> menus, int menuParentId, List<int> containMenus)
        {
            List<Code.TreeHelper> UserTree = new List<Code.TreeHelper>();

            foreach (var menu in menus.Where(d => d.tbMenuParent.Id == menuParentId).OrderBy(d => d.No))
            {
                List<Code.TreeHelper> cn = AddChildrenTreeByMenuId(menus, menu.Id, containMenus);

                var temp = containMenus.Where(t => t == menu.Id).FirstOrDefault();

                UserTree.Add(new Code.TreeHelper()
                {
                    Id = menu.Id,
                    pId = menuParentId,
                    name = menu.MenuName,
                    open = true,
                    isChecked = temp != 0,
                    children = cn
                });
            }

            return UserTree;
        }
    }
}