using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Course.Controllers
{
    public class OrgManagerController : Controller
    {
        public ActionResult TeacherList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgManager.TeacherList();
                var tb = from p in db.Table<Course.Entity.tbOrgManager>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbTeacher.TeacherName.Contains(vm.SearchText));
                }

                vm.DataList = (from p in tb
                               group p by new { p.tbTeacher.Id, p.tbTeacher.TeacherName } into g
                               orderby g.Key.Id
                               select new Dto.OrgManager.TeacherList
                               {
                                   Id = g.Key.Id,
                                   TeacherId = g.Key.Id,
                                   TeacherName = g.Key.TeacherName
                               }).ToPageList(vm.Page);

                var tbOrgManagerList = (from p in db.Table<Course.Entity.tbOrgManager>()
                                        select new Dto.OrgManager.TeacherOrgList
                                        {
                                            Id = p.tbTeacher.Id,
                                            OrgName = p.tbOrg.OrgName
                                        }).ToList();

                var index = 0;
                foreach (var a in vm.DataList)
                {
                    index++;
                    a.No = index;
                    a.OrgNames = string.Join(",", tbOrgManagerList.Where(d => d.Id == a.TeacherId).Select(d => d.OrgName));
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeacherList(Models.OrgManager.TeacherList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("TeacherList", new
            {
                searchText = vm.SearchText,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrgManager>()
                          where ids.Contains(p.tbTeacher.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了科组管理");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgManager.TeacherEdit();
                var TeacherId = Request["TeacherId"].ConvertToInt();
                if (TeacherId > 0)
                {
                    vm.DataEdit.TeacherId = TeacherId;
                }
                if (id != 0)
                {
                    vm.DataEdit = (from p in db.Table<Course.Entity.tbOrgManager>()
                                   where p.Id == id
                                   select new Dto.OrgManager.TeacherEdit
                                   {
                                       Id = p.Id,
                                       TeacherId = p.tbTeacher.Id,
                                       TeacherName = p.tbTeacher.TeacherName,
                                       No = p.No
                                   }).FirstOrDefault();
                }
                vm.OrgList = (from p in db.Table<Course.Entity.tbOrg>()
                              orderby p.No
                              select new Dto.OrgManager.EditOrgList()
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  OrgName = p.OrgName
                              }).ToList();//.ToPageList(vm.Page);
                vm.TeacherList = Teacher.Controllers.TeacherController.SelectList1(vm.DataEdit.TeacherId > 0 ? (int)vm.DataEdit.TeacherId : 0);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.OrgManager.TeacherEdit vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("Edit", new
            {
                TeacherId = vm.DataEdit.TeacherId,
                //pageIndex = vm.Page.PageIndex,
                //pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult Insert(List<int> ids, int teacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var teacher = db.Set<Teacher.Entity.tbTeacher>().Find(teacherId);
                var hasTbs = db.Table<Course.Entity.tbOrgManager>()
                    .Include(d => d.tbOrg)
                    .Where(d => d.tbTeacher.Id == teacherId).ToList();
                var orgList = db.Table<Course.Entity.tbOrg>().Where(d => ids.Contains(d.Id)).ToList();
                var tbs = new List<Course.Entity.tbOrgManager>();
                foreach (var v in ids)
                {
                    if (hasTbs.Where(d => d.tbOrg.Id == v).Count() == 0)
                    {
                        tbs.Add(new Course.Entity.tbOrgManager()
                        {
                            No = tbs.Select(d => d.No).DefaultIfEmpty(0).Max() + 1,
                            tbOrg = orgList.Where(d => d.Id == v).FirstOrDefault(),
                            tbTeacher = teacher
                        });
                    }
                }
                if (tbs.Count > 0)
                {
                    db.Set<Course.Entity.tbOrgManager>().AddRange(tbs);
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加科组管理");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult TeacherOrgList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgManager.TeacherOrgList();
                var tb = db.Table<Course.Entity.tbOrgManager>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbTeacher.TeacherName.Contains(vm.SearchText));
                }
                if (vm.TeacherId > 0)
                {
                    tb = tb.Where(d => d.tbTeacher.Id == vm.TeacherId);
                }
                vm.TeacherName = db.Set<Teacher.Entity.tbTeacher>().Find(vm.TeacherId).TeacherName;
                vm.DataList = (from p in tb
                               orderby p.No
                               select new Dto.OrgManager.TeacherOrgList()
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   OrgId = p.tbOrg.Id,
                                   OrgName = p.tbOrg.OrgName
                               }).ToPageList(vm.Page);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeacherOrgList(Models.OrgManager.TeacherOrgList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("TeacherOrgList", new
            {
                searchText = vm.SearchText,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOrg(List<int> ids, int TeacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrgManager>()
                          where ids.Contains(p.Id) && p.tbTeacher.Id == TeacherId
                          select p).ToList();

                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了科组管理");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult EditOrg(int TeacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgManager.EditOrg();
                vm.TeacherId = TeacherId;
                var hasClassIds = db.Table<Course.Entity.tbOrgManager>()
                    .Where(d => d.tbTeacher.Id == TeacherId).Select(d => d.tbOrg.Id).ToList();
                vm.OrgList = (from p in db.Table<Course.Entity.tbOrg>()
                              where !hasClassIds.Contains(p.Id)
                              select new Dto.OrgManager.EditOrgList()
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  OrgName = p.OrgName
                              }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrg(Models.OrgManager.EditOrg vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("EditOrg", new
            {
                TeacherId = vm.DataEdit.TeacherId
            }));
        }

        public ActionResult InsertTeacherOrg(List<int> ids, int teacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var teacher = db.Set<Teacher.Entity.tbTeacher>().Find(teacherId);
                var tbs = new List<Course.Entity.tbOrgManager>();
                foreach (var v in ids)
                {
                    tbs.Add(new Course.Entity.tbOrgManager()
                    {
                        tbOrg = db.Set<Course.Entity.tbOrg>().Find(v),
                        tbTeacher = teacher
                    });
                }
                db.Set<Course.Entity.tbOrgManager>().AddRange(tbs);

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加科组管理");
                }

                return Code.MvcHelper.Post();
            }
        }
    }
}