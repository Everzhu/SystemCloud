using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveDataController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveData.List();
                var tb = from p in db.Table<Entity.tbElectiveData>()
                         where p.tbElectiveOrg.tbElective.Id == 0
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbElectiveOrg.OrgName.Contains(vm.SearchText));
                }

                vm.ElectiveDataList = (from p in tb
                                       orderby p.InputDate descending
                                       select new Dto.ElectiveData.List
                                       {
                                           Id = p.Id,
                                           ElectiveOrgName = p.tbElectiveOrg.OrgName,
                                           StudentName = p.tbStudent.StudentName,
                                           IsFixed = p.IsFixed,
                                           IsPreElective = p.IsPreElective
                                       }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveData.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveData>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了选课数据");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveData.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbElectiveData>()
                              where p.Id == id
                              select new Dto.ElectiveData.Edit
                              {
                                  Id = p.Id,
                                  ElectiveOrgId = p.tbElectiveOrg.Id,
                                  StudentId = p.tbStudent.Id,
                                  IsFixed = p.IsFixed,
                                  IsPreElective = p.IsPreElective
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ElectiveDataEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ElectiveData.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ElectiveDataEdit.Id == 0)
                    {
                        var tb = new Entity.tbElectiveData()
                        {
                            IsFixed = vm.ElectiveDataEdit.IsFixed,
                            IsPreElective = vm.ElectiveDataEdit.IsPreElective,
                            tbElectiveOrg = db.Set<Entity.tbElectiveOrg>().Find(vm.ElectiveDataEdit.ElectiveOrgId),
                            tbStudent = db.Set<Student.Entity.tbStudent>().Find(Code.Common.UserId),
                        };
                        db.Set<Entity.tbElectiveData>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了选课数据");
                        }
                    }
                    else
                    {
                        //var tb = (from p in db.Table<Entity.tbElectiveData>()
                        //          where p.Id == vm.ElectiveDataEdit.Id
                        //          select p).FirstOrDefault();
                        //if (tb != null)
                        //{
                        //    if (db.SaveChanges() > 0)
                        //    {
                        //        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了选课数据");
                        //    }
                        //}
                        //else
                        //{
                        //    error.AddError(Resources.LocalizedText.MsgNotFound);
                        //}
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(string id)
        {
            return View();
        }

        public ActionResult Export()
        {
            return View();
        }

        [NonAction]
        public static int GetCount(XkSystem.Models.DbContext db, int orgId)
        {
            var tb = (from p in db.Table<Entity.tbElectiveData>()
                      where p.tbElectiveOrg.Id == orgId
                        && p.tbStudent.IsDeleted == false
                      select 1).Count();
            return tb;
        }
    }
}