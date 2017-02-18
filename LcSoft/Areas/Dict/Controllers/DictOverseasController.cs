using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class DictOverseasController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictOverseas.List();
                var tb = db.Table<Dict.Entity.tbDictOverseas>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.DictOverseasName.Contains(vm.SearchText));
                }
                vm.DataList = (from p in tb
                               orderby p.No
                               select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DictOverseas.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText
            }));
        }

        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Dict.Entity.tbDictOverseas>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了港澳台侨");
                }
            }
            return Code.MvcHelper.Post();
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.DictOverseas.Edit();

            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.DataEdit = (from p in db.Table<Dict.Entity.tbDictOverseas>()
                                   where p.Id == id
                                   select p).FirstOrDefault();
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DictOverseas.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (vm.DataEdit.Id > 0)
                    {
                        var tb = db.Set<Dict.Entity.tbDictOverseas>().Find(vm.DataEdit.Id);
                        tb.DictOverseasName = vm.DataEdit.DictOverseasName;
                        tb.No = vm.DataEdit.No == null ? db.Table<Dict.Entity.tbDictOverseas>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No;
                    }
                    else
                    {
                        var tb = new Dict.Entity.tbDictOverseas()
                        {
                            DictOverseasName = vm.DataEdit.DictOverseasName,
                            No = vm.DataEdit.No == null ? db.Table<Dict.Entity.tbDictOverseas>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No
                        };
                        db.Set<Dict.Entity.tbDictOverseas>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增/修改了港澳台侨");
                    }
                }
            }
            return Code.MvcHelper.Post(error);
        }

        public static List<System.Web.Mvc.SelectListItem> SelectList(int id = 0)
        {
            var list = new List<System.Web.Mvc.SelectListItem>();

            using (var db = new XkSystem.Models.DbContext())
            {
                list = (from p in db.Table<Dict.Entity.tbDictOverseas>()
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem()
                        {
                            Value = p.Id.ToString(),
                            Text = p.DictOverseasName
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