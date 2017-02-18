using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamStatusController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamStatus.List();
                var tb = from p in db.Table<Exam.Entity.tbExamStatus>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ExamStatusName.Contains(vm.SearchText));
                }

                vm.ExamStatusList = (from p in tb
                                     orderby p.No
                                     select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamStatus.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamStatus>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除考生状态");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamStatus.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamStatus>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamStatusEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamStatus.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamStatusEdit.Id == 0)
                    {
                        var tb = new Exam.Entity.tbExamStatus();
                        tb.No = vm.ExamStatusEdit.No == null ? db.Table<Exam.Entity.tbExamStatus>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamStatusEdit.No;
                        tb.ExamStatusName = vm.ExamStatusEdit.ExamStatusName;
                        db.Set<Exam.Entity.tbExamStatus>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考生状态");
                        }

                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExamStatus>()
                                  where p.Id == vm.ExamStatusEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.ExamStatusEdit.No == null ? db.Table<Exam.Entity.tbExamStatus>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamStatusEdit.No;
                            tb.ExamStatusName = vm.ExamStatusEdit.ExamStatusName;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改考生状态");
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
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamStatus>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ExamStatusName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}