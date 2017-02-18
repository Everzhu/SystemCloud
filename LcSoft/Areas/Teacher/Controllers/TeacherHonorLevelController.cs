using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Teacher.Controllers
{
    public class TeacherHonorLevelController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeacherHonorLevel.List();
                var tb = db.Table<Teacher.Entity.tbTeacherHonorLevel>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.TeacherHonorLevelName.Contains(vm.SearchText));
                }
                vm.DataList = (from p in tb
                               orderby p.No
                               select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.TeacherHonorLevel.List vm)
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
                var tb = db.Table<Teacher.Entity.tbTeacherHonorLevel>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }
                
                if (db.Table<Entity.tbTeacherHonor>().Where(d => ids.Contains(d.tbTeacherHonorLevel.Id)).Count() > 0)
                {
                    return Code.MvcHelper.Post(new List<string>() { "系统中有关联的数据，不能直接删除此数据！" });
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了教师荣誉级别");
                }
            }
            return Code.MvcHelper.Post();
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.TeacherHonorLevel.Edit();

            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.DataEdit = (from p in db.Table<Teacher.Entity.tbTeacherHonorLevel>()
                                   where p.Id == id
                                   select p).FirstOrDefault();
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.TeacherHonorLevel.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (vm.DataEdit.Id > 0)
                    {
                        var tb = db.Set<Teacher.Entity.tbTeacherHonorLevel>().Find(vm.DataEdit.Id);
                        tb.TeacherHonorLevelName = vm.DataEdit.TeacherHonorLevelName;
                        tb.No = vm.DataEdit.No == null ? db.Table<Teacher.Entity.tbTeacherHonorLevel>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No;
                    }
                    else
                    {
                        var tb = new Teacher.Entity.tbTeacherHonorLevel()
                        {
                            TeacherHonorLevelName = vm.DataEdit.TeacherHonorLevelName,
                            No = vm.DataEdit.No == null ? db.Table<Teacher.Entity.tbTeacherHonorLevel>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No
                        };
                        db.Set<Teacher.Entity.tbTeacherHonorLevel>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增/修改了教师荣誉级别");
                    }
                }
            }
            return Code.MvcHelper.Post(error);
        }

        public static List<System.Web.Mvc.SelectListItem> SelectList(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var list = (from p in db.Table<Teacher.Entity.tbTeacherHonorLevel>()
                            orderby p.No
                            select new System.Web.Mvc.SelectListItem()
                            {
                                Value = p.Id.ToString(),
                                Text = p.TeacherHonorLevelName
                            }).ToList();

                if (id > 0)
                {
                    list.Where(d => d.Value == id.ToString()).FirstOrDefault().Selected = true;
                }

                return list;
            }
        }
    }
}