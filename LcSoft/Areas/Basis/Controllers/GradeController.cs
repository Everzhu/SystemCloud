using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class GradeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Grade.List();
                var tb = from p in db.Table<Basis.Entity.tbGrade>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.GradeName.Contains(vm.SearchText));
                }

                vm.GradeList = (from p in tb
                                orderby p.No
                                select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Grade.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbGrade>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了年级");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Grade.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbGrade>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.GradeEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Grade.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Basis.Entity.tbGrade>().Where(d=>d.GradeName == vm.GradeEdit.GradeName && d.Id != vm.GradeEdit.Id).Any())
                    {
                        error.AddError("该年级已存在!");
                    }
                    else
                    {
                        if (vm.GradeEdit.Id == 0)
                        {
                            var tb = new Basis.Entity.tbGrade();
                            tb.No = vm.GradeEdit.No == null ? db.Table<Basis.Entity.tbGrade>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.GradeEdit.No;
                            tb.GradeName = vm.GradeEdit.GradeName;
                            db.Set<Basis.Entity.tbGrade>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了年级");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Basis.Entity.tbGrade>()
                                      where p.Id == vm.GradeEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.GradeEdit.No == null ? db.Table<Basis.Entity.tbGrade>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.GradeEdit.No;
                                tb.GradeName = vm.GradeEdit.GradeName;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了年级");
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
                var tb = (from p in db.Table<Basis.Entity.tbGrade>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.GradeName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectJuniorList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbGrade>()
                          where "初一年级,初二年级,初三年级".Contains(p.GradeName)
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.GradeName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectHighList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbGrade>()
                          where "高一年级,高二年级,高三年级".Contains(p.GradeName)
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.GradeName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}