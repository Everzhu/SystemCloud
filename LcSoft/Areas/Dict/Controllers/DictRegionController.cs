using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class DictRegionController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictRegion.List();
                var tb = from p in db.Table<Dict.Entity.tbDictRegion>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.RegionName.Contains(vm.SearchText));
                }

                vm.RegionList = (from p in tb
                                 orderby p.No
                                 select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DictRegion.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictRegion>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了区域");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictRegion.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Dict.Entity.tbDictRegion>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.RegionEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DictRegion.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Dict.Entity.tbDictRegion>().Where(d=>d.RegionName == vm.RegionEdit.RegionName && d.Id != vm.RegionEdit.Id).Any())
                    {
                        error.AddError("该区域已存在!");
                    }
                    else
                    {
                        if (vm.RegionEdit.Id == 0)
                        {
                            var tb = new Dict.Entity.tbDictRegion();
                            tb.No = vm.RegionEdit.No == null ? db.Table<Dict.Entity.tbDictRegion>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.RegionEdit.No;
                            tb.RegionName = vm.RegionEdit.RegionName;
                            db.Set<Dict.Entity.tbDictRegion>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了区域");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Dict.Entity.tbDictRegion>()
                                      where p.Id == vm.RegionEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.RegionEdit.No == null ? db.Table<Dict.Entity.tbDictRegion>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.RegionEdit.No;
                                tb.RegionName = vm.RegionEdit.RegionName;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了区域");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post();
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int parentId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictRegion>()
                          where (p.tbDictRegionParent.Id == parentId || parentId == 0)
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.RegionName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList2(int id = 0)
        {
            var list = new List<System.Web.Mvc.SelectListItem>();

            using (var db = new XkSystem.Models.DbContext())
            {
                list = (from p in db.Table<Dict.Entity.tbDictRegion>()
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem()
                        {
                            Value = p.Id.ToString(),
                            Text = p.RegionName
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