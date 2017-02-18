using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyClassTeacherController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyClassTeacher.List();

                vm.WeekList = Basis.Controllers.WeekController.SelectList();

                var tb = from p in db.Table<Study.Entity.tbStudyClassTeacher>()
                         orderby p.tbClass.No
                         where p.tbStudy.Id == vm.StudyId
                         && p.tbClass.IsDeleted == false
                         && p.tbTeacher.IsDeleted == false
                         && p.tbStudy.IsDeleted == false
                         && p.tbWeek.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText));
                }

                vm.StudyClassTeacherList = (from p in tb
                                            orderby p.tbTeacher.TeacherName
                                            select new Dto.StudyClassTeacher.List
                                            {
                                                Id = p.Id,
                                                ClassId = p.tbClass.Id,
                                                ClassName = p.tbClass.ClassName,
                                                IsMaster = p.IsMaster,
                                                TeacherId = p.tbTeacher.Id,
                                                TeacherCode = p.tbTeacher.TeacherCode,
                                                TeacherName = p.tbTeacher.TeacherName,
                                                WeekId = p.tbWeek.Id
                                            }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudyClassTeacher.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, studyId = vm.StudyId}));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int studyId, int classId, int teacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Study.Entity.tbStudyClassTeacher>()
                          where p.tbStudy.Id == studyId && p.tbClass.Id == classId
                          && p.tbTeacher.Id == teacherId
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除晚自习教管");
                }
                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int classId = 0, int teacherId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyClassTeacher.Edit();
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.TeacherList = Teacher.Controllers.TeacherController.SelectList();
                vm.ClassList = StudyClassController.SelectList(vm.StudyId);
                if (classId != 0 && teacherId != 0 && vm.StudyId != 0)
                {
                    vm.StudyClassTeacherEdit = (from p in db.Table<Study.Entity.tbStudyClassTeacher>()
                                                where p.tbStudy.Id == vm.StudyId
                                                && p.tbClass.Id == classId
                                                && p.tbTeacher.Id == teacherId
                                                && p.tbClass.IsDeleted == false
                                                && p.tbStudy.IsDeleted == false
                                                && p.tbTeacher.IsDeleted == false
                                                select new Dto.StudyClassTeacher.Edit
                                                {
                                                    Id = p.Id,
                                                    IsMaster = p.IsMaster
                                                }).FirstOrDefault();

                    vm.WeekIdList = (from p in db.Table<Study.Entity.tbStudyClassTeacher>()
                                     where p.tbStudy.Id == vm.StudyId
                                     && p.tbClass.Id == classId
                                     && p.tbTeacher.Id == teacherId
                                     && p.tbClass.IsDeleted == false
                                     && p.tbStudy.IsDeleted == false
                                     && p.tbTeacher.IsDeleted == false
                                     select p.tbWeek.Id).Distinct().ToList();
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudyClassTeacher.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var weekIds = new List<int>();
                    if (Request["CboxWeek"] != null)
                    {
                        weekIds = Request["CboxWeek"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                    }

                    var tbStudyClassTeacherList = (from p in db.Table<Study.Entity.tbStudyClassTeacher>()
                                                   .Include(d => d.tbClass)
                                                   .Include(d => d.tbTeacher)
                                                   .Include(d => d.tbWeek)
                                                   where p.tbClass.Id == vm.ClassId
                                                   && p.tbStudy.Id == vm.StudyId
                                                   && p.tbTeacher.Id == vm.TeacherId
                                                   && p.tbClass.IsDeleted == false
                                                   && p.tbStudy.IsDeleted == false
                                                   && p.tbTeacher.IsDeleted == false
                                                   select p).ToList();

                    foreach (var a in tbStudyClassTeacherList.Where(d => weekIds.Contains(d.tbWeek.Id) == false))
                    {
                        a.IsDeleted = true;
                    }

                    foreach (var a in weekIds.Where(d => tbStudyClassTeacherList.Select(q => q.tbWeek.Id).Contains(d) == false))
                    {
                        var tb = new Study.Entity.tbStudyClassTeacher();
                        tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.TeacherId);
                        tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(vm.ClassId);
                        tb.tbWeek = db.Set<Basis.Entity.tbWeek>().Find(a);
                        tb.tbStudy = db.Set<Study.Entity.tbStudy>().Find(vm.StudyId);
                        tb.IsMaster = vm.StudyClassTeacherEdit.IsMaster;
                        db.Set<Study.Entity.tbStudyClassTeacher>().Add(tb);
                    }

                    foreach (var a in tbStudyClassTeacherList.Where(d => weekIds.Contains(d.tbWeek.Id) == true))
                    {
                        a.IsMaster = vm.StudyClassTeacherEdit.IsMaster;
                    }

                    if (db.SaveChanges() > 0)
                    {
                        if (vm.StudyClassTeacherEdit.IsMaster)
                        {
                            var tbStudyClassTeacher = (from p in db.Table<Study.Entity.tbStudyClassTeacher>()
                                                       .Include(d => d.tbClass)
                                                       where p.tbClass.Id == vm.ClassId
                                                       && p.tbStudy.Id == vm.StudyId
                                                       && p.tbTeacher.Id != vm.TeacherId
                                                       && p.tbClass.IsDeleted == false
                                                       && p.tbStudy.IsDeleted == false
                                                       && p.tbTeacher.IsDeleted == false
                                                       select p).ToList();
                            foreach (var a in tbStudyClassTeacher)
                            {
                                a.IsMaster = false;
                            }
                            db.SaveChanges();
                        }
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习教管");
                    }
                }
                return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, studyId = vm.StudyId }));
            }
        }
    }
}