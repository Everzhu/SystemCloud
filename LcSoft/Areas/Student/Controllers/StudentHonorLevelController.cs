using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Student.Controllers
{
    public class StudentHonorLevelController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentHonorLevel.List();
                var tb = db.Table<Student.Entity.tbStudentHonorLevel>();

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.StudentHonorLevelName.Contains(vm.SearchText));
                }

                vm.StudentHonorLevelList = (from p in tb
                                            orderby p.No
                                            select p).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult List(Models.StudentHonorLevel.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            var error = new List<string>();

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Student.Entity.tbStudentHonorLevel>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }

                if (db.Table<Entity.tbStudentHonor>().Where(d => ids.Contains(d.tbstudentHonorLevel.Id)).Count() > 0)
                {
                    return Code.MvcHelper.Post(new List<string>() { "系统中有关联的数据，不能直接删除此数据！" });
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除学生获奖级别");
                }
            }

            return Code.MvcHelper.Post(error);
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.StudentHonorLevel.Edit();

            if (id != 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.StudentHonorLevelEdit = db.Set<Student.Entity.tbStudentHonorLevel>().Find(id);
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudentHonorLevel.Edit vm)
        {
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                if (vm.StudentHonorLevelEdit.Id != 0)
                {
                    var tb = db.Set<Student.Entity.tbStudentHonorLevel>().Find(vm.StudentHonorLevelEdit.Id);
                    tb.No = vm.StudentHonorLevelEdit.No > 0 ? (int)vm.StudentHonorLevelEdit.No : db.Table<Student.Entity.tbStudentHonorLevel>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1;
                    tb.StudentHonorLevelName = vm.StudentHonorLevelEdit.StudentHonorLevelName;
                }
                else
                {
                    var tb = new Student.Entity.tbStudentHonorLevel()
                    {
                        No = vm.StudentHonorLevelEdit.No > 0 ? (int)vm.StudentHonorLevelEdit.No : db.Table<Student.Entity.tbStudentHonorLevel>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1,
                        StudentHonorLevelName = vm.StudentHonorLevelEdit.StudentHonorLevelName
                    };
                    db.Set<Student.Entity.tbStudentHonorLevel>().Add(tb);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加/修改学生获奖级别");
                }
            }

            return Code.MvcHelper.Post(error);
        }






        public static List<System.Web.Mvc.SelectListItem> SelectList(int id = 0)
        {
            var list = new List<System.Web.Mvc.SelectListItem>();
            using (var db = new XkSystem.Models.DbContext())
            {
                list = (from p in db.Table<Student.Entity.tbStudentHonorLevel>()
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = p.StudentHonorLevelName,
                            Value = p.Id.ToString()
                        }).ToList();

                if (id > 0)
                {
                    list.Where(d => d.Value == id.ConvertToString()).FirstOrDefault().Selected = true;
                }
            }
            return list;
        }

        [NonAction]
        public static List<Student.Entity.tbStudentHonorLevel> BuildList(XkSystem.Models.DbContext db, List<Dto.StudentHonorLevel.Edit> editList)
        {
            List<Student.Entity.tbStudentHonorLevel> list = new List<Student.Entity.tbStudentHonorLevel>();

            foreach (var v in editList)
            {
                var honorLevel = new Student.Entity.tbStudentHonorLevel()
                {
                    No = v.No > 0 ? (int)v.No : 0,
                    StudentHonorLevelName = v.StudentHonorLevelName
                };

                list.Add(honorLevel);
            }

            return list;
        }
    }
}