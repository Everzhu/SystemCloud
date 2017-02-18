using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamLevelController : Controller
    {
        public ActionResult List(string levelGroupId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamLevel.List();

                var tb = from p in db.Table<Exam.Entity.tbExamLevel>()
                         where p.tbExamLevelGroup.Id == vm.LevelGroupId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ExamLevelName.Contains(vm.SearchText));
                }

                vm.ExamLevelList = (from p in tb
                                    orderby p.No
                                    select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamLevel.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamLevel>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除考试等级");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamLevel.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamLevel>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamLevelEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamLevel.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamLevelEdit.Id == 0)
                    {
                        var tb = new Exam.Entity.tbExamLevel();
                        tb.No = vm.ExamLevelEdit.No == null ? db.Table<Exam.Entity.tbExamLevel>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamLevelEdit.No;
                        tb.ExamLevelName = vm.ExamLevelEdit.ExamLevelName;
                        tb.tbExamLevelGroup = db.Set<Exam.Entity.tbExamLevelGroup>().Find(vm.LevelGroupId);
                        tb.Rate = vm.ExamLevelEdit.Rate;
                        tb.ExamLevelValue = vm.ExamLevelEdit.ExamLevelValue;
                        tb.MaxScore = vm.ExamLevelEdit.MaxScore;
                        tb.MinScore = vm.ExamLevelEdit.MinScore;
                        db.Set<Exam.Entity.tbExamLevel>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试等级");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                  where p.Id == vm.ExamLevelEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.ExamLevelEdit.No == null ? db.Table<Exam.Entity.tbExamLevel>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamLevelEdit.No;
                            tb.ExamLevelName = vm.ExamLevelEdit.ExamLevelName;
                            tb.tbExamLevelGroup = db.Set<Exam.Entity.tbExamLevelGroup>().Find(vm.LevelGroupId);
                            tb.Rate = vm.ExamLevelEdit.Rate;
                            tb.ExamLevelValue = vm.ExamLevelEdit.ExamLevelValue;
                            tb.MaxScore = vm.ExamLevelEdit.MaxScore;
                            tb.MinScore = vm.ExamLevelEdit.MinScore;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改考试等级");
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

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int levelGroupId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamLevel>()
                          where p.tbExamLevelGroup.Id==levelGroupId
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ExamLevelName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int examId,int courseId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var levelGroupId = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                    where p.tbExam.Id == examId
                                     && p.tbCourse.Id == courseId
                                     && p.tbExamLevelGroup.IsDeleted == false
                                    select p.tbExamLevelGroup.Id).FirstOrDefault();

                var tb = (from p in db.Table<Exam.Entity.tbExamLevel>()
                          where p.tbExamLevelGroup.Id == levelGroupId
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ExamLevelName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}