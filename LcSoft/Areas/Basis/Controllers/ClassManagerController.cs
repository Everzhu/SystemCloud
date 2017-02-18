using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class ClassManagerController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassManager.List();
                var tb = db.Table<Basis.Entity.tbClassManager>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbTeacher.TeacherName.Contains(vm.SearchText));
                }
                vm.DataList = (from p in tb
                               group p by new { p.tbTeacher.Id, p.tbTeacher.No, p.tbTeacher.TeacherName } into g
                               orderby g.Key.No
                               select new Dto.ClassManager.List()
                               {
                                   Id = g.Key.Id,
                                   No = g.Key.No,
                                   TeacherId = g.Key.Id,
                                   TeacherName = g.Key.TeacherName
                               }).ToPageList(vm.Page);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ClassManager.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbClassManager>()
                          where ids.Contains(p.tbTeacher.Id)
                          select p).ToList();

                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了年级组管理");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassManager.Edit();
                var TeacherId = Request["TeacherId"].ConvertToInt();
                if (TeacherId > 0)
                {
                    vm.DataEdit.TeacherId = TeacherId;
                }
                if (id != 0)
                {
                    vm.DataEdit = (from p in db.Table<Basis.Entity.tbClassManager>()
                                   where p.Id == id
                                   select new Dto.ClassManager.Edit
                                   {
                                       Id = p.Id,
                                       TeacherId = p.tbTeacher.Id,
                                       No = p.No
                                   }).FirstOrDefault();
                }
                vm.ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                orderby p.No
                                select new Dto.ClassManager.EditClassList()
                                {
                                    Id = p.Id,
                                    No = p.No,
                                    ClassName = p.ClassName
                                }).ToList();//.ToPageList(vm.Page);
                vm.TeacherList = Teacher.Controllers.TeacherController.SelectList1(vm.DataEdit.TeacherId > 0 ? (int)vm.DataEdit.TeacherId : 0);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ClassManager.Edit vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("Edit", new
            {
                TeacherId = vm.DataEdit.TeacherId,
                //pageIndex = vm.Page.PageIndex,
                //pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult TeacherClassList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassManager.TeacherClassList();
                var tb = db.Table<Basis.Entity.tbClassManager>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbTeacher.TeacherName.Contains(vm.SearchText));
                }
                if (vm.TeacherId > 0)
                {
                    tb = tb.Where(d => d.tbTeacher.Id == vm.TeacherId);
                }
                vm.TeacherName = db.Set<Teacher.Entity.tbTeacher>().Find(vm.TeacherId).TeacherName;
                vm.DataList = (from p in tb
                               orderby p.No
                               select new Dto.ClassManager.TeacherClassList()
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   ClassId = p.tbClass.Id,
                                   ClassName = p.tbClass.ClassName
                               }).ToPageList(vm.Page);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeacherClassList(Models.ClassManager.TeacherClassList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("TeacherClassList", new
            {
                searchText = vm.SearchText,
                TeacherId = vm.TeacherId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteClass(List<int> ids, int TeacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbClassManager>()
                          where ids.Contains(p.Id) && p.tbTeacher.Id == TeacherId
                          select p).ToList();

                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了年级组管理");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult EditClass(int TeacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassManager.EditClass();
                vm.TeacherId = TeacherId;
                var hasClassIds = db.Table<Basis.Entity.tbClassManager>()
                    .Where(d => d.tbTeacher.Id == TeacherId).Select(d => d.tbClass.Id).ToList();
                vm.ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                where !hasClassIds.Contains(p.Id)
                                select new Dto.ClassManager.EditClassList()
                                {
                                    Id = p.Id,
                                    No = p.No,
                                    ClassId = p.Id,
                                    ClassName = p.ClassName
                                }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClass(Models.ClassManager.EditClass vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("EditClass", new
            {
                TeacherId = vm.DataEdit.TeacherId,
                //pageIndex = vm.Page.PageIndex,
                //pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult Insert(List<int> ids, int teacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var teacher = db.Set<Teacher.Entity.tbTeacher>().Find(teacherId);
                var hasTbs = db.Table<Basis.Entity.tbClassManager>()
                   .Include(d => d.tbClass)
                   .Where(d => d.tbTeacher.Id == teacherId).ToList();
                var classList = db.Table<Basis.Entity.tbClass>().Where(d => ids.Contains(d.Id)).ToList();
                var tbs = new List<Basis.Entity.tbClassManager>();
                foreach (var v in ids)
                {
                    if (hasTbs.Where(d => d.tbClass.Id == v).Count() == 0)
                    {
                        tbs.Add(new Basis.Entity.tbClassManager()
                        {
                            No = tbs.Select(d => d.No).DefaultIfEmpty(0).Max() + 1,
                            tbClass = classList.Where(d => d.Id == v).FirstOrDefault(),
                            tbTeacher = teacher
                        });
                    }
                }
                if (tbs.Count > 0)
                {
                    db.Set<Basis.Entity.tbClassManager>().AddRange(tbs);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加班级管理员");
                }

                return Code.MvcHelper.Post();
            }
        }
    }
}