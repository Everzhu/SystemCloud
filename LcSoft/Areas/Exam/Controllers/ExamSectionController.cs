using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamSectionController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamSection.List();
                var tb = from p in db.Table<Exam.Entity.tbExamSection>()
                         where p.tbGrade.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ExamSectionName.Contains(vm.SearchText) || d.ExamSectionNameEn.Contains(vm.SearchText));
                }

                vm.ExamSectionList = (from p in tb
                                   orderby p.No, p.ExamSectionName,p.ExamSectionNameEn
                                   select new Dto.ExamSection.List
                                   {
                                       Id = p.Id,
                                       No = p.No,
                                       GradeName=p.tbGrade.GradeName,
                                       ExamSectionName = p.ExamSectionName,
                                       ExamSectionNameEn=p.ExamSectionNameEn
                                   }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamType.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamSection>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了学习时间");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamSection.Edit();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamSection>()
                              where p.Id == id && p.tbGrade.IsDeleted == false
                              select new Dto.ExamSection.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  GradeId= p.tbGrade.Id,
                                  ExamSectionName = p.ExamSectionName,
                                  ExamSectionNameEn=p.ExamSectionNameEn
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamSectionEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamSection.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Exam.Entity.tbExamSection>().Where(d=>d.ExamSectionName == vm.ExamSectionEdit.ExamSectionName && d.tbGrade.Id == vm.ExamSectionEdit.GradeId && d.Id != vm.ExamSectionEdit.Id).Any())
                    {
                        error.AddError("该年级学习时间已存在!");
                    }
                    else
                    {
                        if (vm.ExamSectionEdit.Id == 0)
                        {
                            var tb = new Exam.Entity.tbExamSection();
                            tb.No = vm.ExamSectionEdit.No == null ? db.Table<Exam.Entity.tbExamSection>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamSectionEdit.No;
                            tb.ExamSectionName = vm.ExamSectionEdit.ExamSectionName;
                            tb.ExamSectionNameEn = vm.ExamSectionEdit.ExamSectionNameEn;
                            tb.tbGrade= db.Set<Basis.Entity.tbGrade>().Find(vm.ExamSectionEdit.GradeId);
                            db.Set<Exam.Entity.tbExamSection>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了学习时间");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Exam.Entity.tbExamSection>()
                                      where p.Id == vm.ExamSectionEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.ExamSectionEdit.No == null ? db.Table<Exam.Entity.tbExamSection>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamSectionEdit.No;
                                tb.ExamSectionName = vm.ExamSectionEdit.ExamSectionName;
                                tb.ExamSectionNameEn = vm.ExamSectionEdit.ExamSectionNameEn;
                                tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.ExamSectionEdit.GradeId);

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了学习时间");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var list = (from p in db.Table<Exam.Entity.tbExamSection>()
                            where p.tbGrade.IsDeleted == false
                            orderby p.No, p.ExamSectionName,p.ExamSectionNameEn
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text =p.tbGrade.GradeName+p.ExamSectionName,
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }
    }
}