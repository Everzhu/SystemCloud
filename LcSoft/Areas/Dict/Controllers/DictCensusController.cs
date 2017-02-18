using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class DictCensusController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictCensus.List();
                var tb = db.Table<Dict.Entity.tbDictCensus>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.DictCensusName.Contains(vm.SearchText));
                }
                vm.DataList = tb.ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DictCensus.List vm)
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
                var tb = db.Table<Dict.Entity.tbDictCensus>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了户口类型");
                }
            }
            return Code.MvcHelper.Post();
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.DictCensus.Edit();

            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.DataEdit = (from p in db.Table<Dict.Entity.tbDictCensus>()
                                   where p.Id == id
                                   select p).FirstOrDefault();
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DictCensus.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (vm.DataEdit.Id > 0)
                    {
                        var tb = db.Set<Dict.Entity.tbDictCensus>().Find(vm.DataEdit.Id);
                        tb.DictCensusName = vm.DataEdit.DictCensusName;
                        tb.No = vm.DataEdit.No == null ? db.Table<Dict.Entity.tbDictCensus>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No;
                    }
                    else
                    {
                        var tb = new Dict.Entity.tbDictCensus()
                        {
                            DictCensusName = vm.DataEdit.DictCensusName,
                            No = vm.DataEdit.No == null ? db.Table<Dict.Entity.tbDictCensus>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No
                        };
                        db.Set<Dict.Entity.tbDictCensus>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增/修改了户口类型");
                    }
                }
            }
            return Code.MvcHelper.Post(error);
        }

        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            var list = new List<System.Web.Mvc.SelectListItem>();

            using (var db = new XkSystem.Models.DbContext())
            {
                list = (from p in db.Table<Dict.Entity.tbDictCensus>()
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem()
                        {
                            Value = p.Id.ToString(),
                            Text = p.DictCensusName
                        }).ToList();
            }

            return list;
        }
    }
}