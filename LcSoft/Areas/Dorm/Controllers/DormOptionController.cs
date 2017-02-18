using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dorm.Controllers
{
    public class DormOptionController : Controller
    {
        public ActionResult List()
        {
            var vm = new Models.DormOption.List();

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Dorm.Entity.tbDormOption>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.DormOptionName.Contains(vm.SearchText));
                }
                vm.DormOptionList = (from p in tb
                                     orderby p.No
                                     select p).ToPageList(vm.Page);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DormOption.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                SearchText = vm.SearchText,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Dorm.Entity.tbDormOption>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了表现选项");
                }
            }
            return Code.MvcHelper.Post();
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.DormOption.Edit();

            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.DormOptionEdit = (from p in db.Table<Dorm.Entity.tbDormOption>()
                                         where p.Id == id
                                         select p).FirstOrDefault();
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DormOption.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (vm.DormOptionEdit.Id > 0)
                    {
                        var tb = db.Set<Dorm.Entity.tbDormOption>().Find(vm.DormOptionEdit.Id);
                        tb.No = vm.DormOptionEdit.No;
                        tb.DormOptionName = vm.DormOptionEdit.DormOptionName;
                        tb.DormOptionValue = vm.DormOptionEdit.DormOptionValue;
                    }
                    else
                    {
                        var tb = new Dorm.Entity.tbDormOption()
                        {
                            No = vm.DormOptionEdit.No,
                            DormOptionValue = vm.DormOptionEdit.DormOptionValue,
                            DormOptionName = vm.DormOptionEdit.DormOptionName
                        };
                        db.Set<Dorm.Entity.tbDormOption>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增/修改了表现选项");
                    }
                }
            }
            return Code.MvcHelper.Post(error);
        }





        public static List<System.Web.Mvc.SelectListItem> SelectList(int id = 0)
        {
            var tb = new List<System.Web.Mvc.SelectListItem>();

            using (var db = new XkSystem.Models.DbContext())
            {
                tb = (from p in db.Table<Dorm.Entity.tbDormOption>()
                      orderby p.No
                      select new System.Web.Mvc.SelectListItem()
                      {
                          Value = p.Id.ToString(),
                          Text = p.DormOptionName
                      }).ToList();

                if (id > 0)
                {
                    tb.Where(d => d.Value == id.ToString()).FirstOrDefault().Selected = true;
                }
            }

            return tb;
        }
    }
}