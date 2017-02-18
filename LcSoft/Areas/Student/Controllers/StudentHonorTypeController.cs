using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Student.Controllers
{
    public class StudentHonorTypeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentHonorType.List();
                var tb = db.Table<Student.Entity.tbStudentHonorType>();

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.StudentHonorTypeName.Contains(vm.SearchText));
                }

                vm.StudentHonorTypeList = (from p in tb
                                           orderby p.No
                                           select p).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult List(Models.StudentHonorType.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            var error = new List<string>();

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Student.Entity.tbStudentHonorType>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }

                if (db.Table<Entity.tbStudentHonor>().Where(d => ids.Contains(d.tbStudentHonorType.Id)).Count() > 0)
                {
                    return Code.MvcHelper.Post(new List<string>() { "系统中有关联的数据，不能直接删除此数据！" });
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除学生荣誉类型");
                }
            }

            return Code.MvcHelper.Post(error);
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.StudentHonorType.Edit();

            if (id != 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.StudentHonorTypeEdit = db.Set<Student.Entity.tbStudentHonorType>().Find(id);
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudentHonorType.Edit vm)
        {
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                if (vm.StudentHonorTypeEdit.Id != 0)
                {
                    var tb = db.Set<Student.Entity.tbStudentHonorType>().Find(vm.StudentHonorTypeEdit.Id);
                    tb.No = vm.StudentHonorTypeEdit.No > 0 ? (int)vm.StudentHonorTypeEdit.No : db.Table<Student.Entity.tbStudentHonorType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1;
                    tb.StudentHonorTypeName = vm.StudentHonorTypeEdit.StudentHonorTypeName;
                }
                else
                {
                    var tb = new Student.Entity.tbStudentHonorType()
                    {
                        No = vm.StudentHonorTypeEdit.No > 0 ? (int)vm.StudentHonorTypeEdit.No : db.Table<Student.Entity.tbStudentHonorType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1,
                        StudentHonorTypeName = vm.StudentHonorTypeEdit.StudentHonorTypeName
                    };
                    db.Set<Student.Entity.tbStudentHonorType>().Add(tb);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加/修改学生荣誉类型");
                }
            }

            return Code.MvcHelper.Post(error);
        }







        public static List<System.Web.Mvc.SelectListItem> SelectList(int id = 0)
        {
            var list = new List<System.Web.Mvc.SelectListItem>();
            using (var db = new XkSystem.Models.DbContext())
            {
                list = (from p in db.Table<Student.Entity.tbStudentHonorType>()
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = p.StudentHonorTypeName,
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
        public static List<Student.Entity.tbStudentHonorType> BuildList(XkSystem.Models.DbContext db, List<Dto.StudentHonorType.Edit> editList)
        {
            List<Student.Entity.tbStudentHonorType> list = new List<Student.Entity.tbStudentHonorType>();

            foreach (var v in editList)
            {
                var honorType = new Student.Entity.tbStudentHonorType()
                {
                    No = v.No > 0 ? (int)v.No : 0,
                    StudentHonorTypeName = v.StudentHonorTypeName
                };

                list.Add(honorType);
            }

            return list;
        }
    }
}