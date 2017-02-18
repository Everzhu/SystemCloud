using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Exam.List();

                var tb = from p in db.Table<Exam.Entity.tbExam>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ExamName.Contains(vm.SearchText));
                }

                vm.ExamList = (from p in tb
                               orderby p.No descending
                               select new Dto.Exam.List
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   ExamName = p.ExamName,
                                   IsPublish = p.IsPublish,
                                   ExamTypeName = p.tbExamType.ExamTypeName,
                                   YearName = p.tbYear.YearName,
                                   ExamLevelGroupName = p.tbExamLevelGroup.ExamLevelGroupName,
                                   ExamSegmentGroupName = p.tbExamSegmentGroup.ExamSegmentGroupName
                               }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Exam.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExam>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var examCourseList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                        .Include(d => d.tbExam)
                                      where ids.Contains(p.tbExam.Id)
                                      select p).ToList();

                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                        .Include(d => d.tbExamCourse.tbExam)
                                    where ids.Contains(p.tbExamCourse.tbExam.Id)
                                        && p.tbExamCourse.IsDeleted == false
                                    select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    foreach (var course in examCourseList)
                    {
                        course.IsDeleted = true;
                    }

                    foreach (var mark in examMarkList)
                    {
                        mark.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除考试");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Exam.Edit();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.ExamEdit.YearId == 0)
                {
                    vm.ExamEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                vm.ExamTypeList = Areas.Exam.Controllers.ExamTypeController.SelectList();
                vm.LevelGroupList = Areas.Exam.Controllers.ExamLevelGroupController.SelectTotalList();
                vm.SegmentGroupList = Areas.Exam.Controllers.ExamSegmentGroupController.SelectList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExam>().Include(d => d.tbExamLevelGroup).Include(d=>d.tbExamSegmentGroup)
                              where p.Id == id
                              select new Dto.Exam.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  ExamName = p.ExamName,
                                  IsPublish = p.IsPublish,
                                  YearId = p.tbYear.Id,
                                  ExamTypeId = p.tbExamType.Id,
                                  LevelGroupId = p.tbExamLevelGroup.Id,
                                  SegmentGroupId=p.tbExamSegmentGroup.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Exam.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamEdit.Id == 0)
                    {
                        var tb = new Exam.Entity.tbExam();
                        tb.No = vm.ExamEdit.No == null ? db.Table<Exam.Entity.tbExam>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamEdit.No;
                        tb.ExamName = vm.ExamEdit.ExamName;
                        tb.IsPublish = vm.ExamEdit.IsPublish;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.ExamEdit.YearId);
                        tb.tbExamType = db.Set<Exam.Entity.tbExamType>().Find(vm.ExamEdit.ExamTypeId);
                        tb.tbExamLevelGroup = db.Set<Exam.Entity.tbExamLevelGroup>().Find(vm.ExamEdit.LevelGroupId);
                        tb.tbExamSegmentGroup = db.Set<Exam.Entity.tbExamSegmentGroup>().Find(vm.ExamEdit.SegmentGroupId);
                        db.Set<Exam.Entity.tbExam>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExam>().Include(d => d.tbExamLevelGroup).Include(d=>d.tbExamSegmentGroup)
                                  where p.Id == vm.ExamEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.ExamEdit.No == null ? db.Table<Exam.Entity.tbExam>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamEdit.No;
                            tb.ExamName = vm.ExamEdit.ExamName;
                            tb.IsPublish = vm.ExamEdit.IsPublish;
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.ExamEdit.YearId);
                            tb.tbExamType = db.Set<Exam.Entity.tbExamType>().Find(vm.ExamEdit.ExamTypeId);
                            tb.tbExamLevelGroup = db.Set<Exam.Entity.tbExamLevelGroup>().Find(vm.ExamEdit.LevelGroupId);
                            tb.tbExamSegmentGroup = db.Set<Exam.Entity.tbExamSegmentGroup>().Find(vm.ExamEdit.SegmentGroupId);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改考试");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPublish(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Exam.Entity.tbExam>().Find(id);
                if (tb != null)
                {
                    tb.IsPublish = !tb.IsPublish;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExam>()
                          orderby p.No descending
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ExamName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectPublishList(bool IsPublish = true)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExam>()
                          where p.IsPublish
                          orderby p.No descending
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ExamName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}