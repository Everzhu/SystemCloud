using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Perform.Controllers
{
    public class PerformTotalController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformTotal.List();
                var tb = from p in db.Table<Perform.Entity.tbPerformTotal>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }
                vm.PerformTotalList = (from p in tb
                                       orderby p.No
                                       select new Dto.PerformTotal.List
                                       {
                                           Id = p.Id,
                                           No = p.No,
                                           PerformName = p.tbPerform.PerformName,
                                           CourseName = p.tbCourse.CourseName,
                                           PerformId = p.tbPerform.Id,
                                           CourseId = p.tbCourse.Id,
                                           TotalScore = p.TotalScore
                                       }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.PerformTotal.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Perform.Entity.tbPerformTotal>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价总分数据");
                }
                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformTotal.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Perform.Entity.tbPerformTotal>()
                              where p.Id == id
                              select new Dto.PerformTotal.Edit
                              {
                                  Id = p.Id,
                                  CourseId = p.tbCourse.Id,
                                  TotalScore = p.TotalScore,
                                  PerformId = p.tbPerform.Id,
                                  StudentId = p.tbStudent.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.PerformTotalEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.PerformTotal.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.PerformTotalEdit.Id == 0)
                    {
                        var tb = new Perform.Entity.tbPerformTotal();
                        db.Set<Perform.Entity.tbPerformTotal>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价总分数据");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Perform.Entity.tbPerformTotal>()
                                  where p.Id == vm.PerformTotalEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价总分数据");
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
    }
}