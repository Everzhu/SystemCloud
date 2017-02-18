using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamTypeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamType.List();
                var tb = from p in db.Table<Exam.Entity.tbExamType>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ExamTypeName.Contains(vm.SearchText));
                }

                vm.ExamTypeList = (from p in tb
                                   orderby p.No, p.ExamTypeName
                                   select p).ToList();
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
                var tb = (from p in db.Table<Exam.Entity.tbExamType>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了考试类型");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamType.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamType>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamTypeEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamType.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Exam.Entity.tbExamType>().Where(d=>d.ExamTypeName == vm.ExamTypeEdit.ExamTypeName && d.Id != vm.ExamTypeEdit.Id).Any())
                    {
                        error.AddError("该考试类型已存在!");
                    }
                    else
                    {
                        if (vm.ExamTypeEdit.Id == 0)
                        {
                            var tb = new Exam.Entity.tbExamType();
                            tb.No = vm.ExamTypeEdit.No == null ? db.Table<Exam.Entity.tbExamType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamTypeEdit.No;
                            tb.ExamTypeName = vm.ExamTypeEdit.ExamTypeName;
                            db.Set<Exam.Entity.tbExamType>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了考试类型");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Exam.Entity.tbExamType>()
                                      where p.Id == vm.ExamTypeEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.ExamTypeEdit.No == null ? db.Table<Exam.Entity.tbExamType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamTypeEdit.No;
                                tb.ExamTypeName = vm.ExamTypeEdit.ExamTypeName;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了考试类型");
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
                var list = (from p in db.Table<Exam.Entity.tbExamType>()
                            orderby p.No, p.ExamTypeName
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.ExamTypeName,
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }
    }
}