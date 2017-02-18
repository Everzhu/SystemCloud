using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Teacher.Controllers
{
    public class TeacherJobController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeacherJob.List();
                var tb = db.Table<Teacher.Entity.tbTeacherJob>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.TeacherJobName.Contains(vm.SearchText));
                }
                vm.DataList = (from p in tb
                               orderby p.No
                               select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.TeacherJob.List vm)
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
                var tb = db.Table<Teacher.Entity.tbTeacherJob>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了教师职务");
                }
            }
            return Code.MvcHelper.Post();
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.TeacherJob.Edit();

            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.DataEdit = (from p in db.Table<Teacher.Entity.tbTeacherJob>()
                                   where p.Id == id
                                   select p).FirstOrDefault();
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.TeacherJob.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (vm.DataEdit.Id > 0)
                    {
                        var tb = db.Set<Teacher.Entity.tbTeacherJob>().Find(vm.DataEdit.Id);
                        tb.TeacherJobName = vm.DataEdit.TeacherJobName;
                        tb.No = vm.DataEdit.No == null ? db.Table<Teacher.Entity.tbTeacherJob>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No;
                    }
                    else
                    {
                        var tb = new Teacher.Entity.tbTeacherJob()
                        {
                            TeacherJobName = vm.DataEdit.TeacherJobName,
                            No = vm.DataEdit.No == null ? db.Table<Teacher.Entity.tbTeacherJob>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No
                        };
                        db.Set<Teacher.Entity.tbTeacherJob>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增/修改了教师职务");
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
                list = (from p in db.Table<Teacher.Entity.tbTeacherJob>()
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem()
                        {
                            Value = p.Id.ToString(),
                            Text = p.TeacherJobName
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