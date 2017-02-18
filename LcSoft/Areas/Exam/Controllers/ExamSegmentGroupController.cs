using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamSegmentGroupController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamSegmentGroup.List();
                var tb = from p in db.Table<Exam.Entity.tbExamSegmentGroup>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ExamSegmentGroupName.Contains(vm.SearchText));
                }

                vm.ExamSegmentGroupList = (from p in tb
                                   orderby p.No, p.ExamSegmentGroupName
                                   select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamSegmentGroup.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamSegmentGroup>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了考试分数段分组");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamSegmentGroup.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamSegmentGroup>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamSegmentGroupEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamSegmentGroup.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Exam.Entity.tbExamSegmentGroup>().Where(d=>d.ExamSegmentGroupName == vm.ExamSegmentGroupEdit.ExamSegmentGroupName && d.Id != vm.ExamSegmentGroupEdit.Id).Any())
                    {
                        error.AddError("该考试分分组已存在!");
                    }
                    else
                    {
                        if (vm.ExamSegmentGroupEdit.Id == 0)
                        {
                            var tb = new Exam.Entity.tbExamSegmentGroup();
                            tb.No = vm.ExamSegmentGroupEdit.No == null ? db.Table<Exam.Entity.tbExamSegmentGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamSegmentGroupEdit.No;
                            tb.ExamSegmentGroupName = vm.ExamSegmentGroupEdit.ExamSegmentGroupName;
                            db.Set<Exam.Entity.tbExamSegmentGroup>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了考试分数段分组");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Exam.Entity.tbExamSegmentGroup>()
                                      where p.Id == vm.ExamSegmentGroupEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.ExamSegmentGroupEdit.No == null ? db.Table<Exam.Entity.tbExamSegmentGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamSegmentGroupEdit.No;
                                tb.ExamSegmentGroupName = vm.ExamSegmentGroupEdit.ExamSegmentGroupName;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了考试分数段分组");
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
                var list = (from p in db.Table<Exam.Entity.tbExamSegmentGroup>()
                            orderby p.No, p.ExamSegmentGroupName
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.ExamSegmentGroupName,
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }
    }
}