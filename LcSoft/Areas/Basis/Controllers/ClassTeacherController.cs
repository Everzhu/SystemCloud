using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class ClassTeacherController : Controller
    {
        public ActionResult List(string ClassId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassTeacher.List();
                var tb = from p in db.Table<Basis.Entity.tbClassTeacher>()
                         where p.tbClass.Id == vm.ClassId
                            && p.tbTeacher.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText));
                }

                vm.ClassTeacherList = (from p in tb
                                       orderby p.tbTeacher.TeacherCode
                                       select new Dto.ClassTeacher.List
                                       {
                                           Id = p.Id,
                                           ClassName = p.tbClass.ClassName,
                                           TeacherName = p.tbTeacher.TeacherName
                                       }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ClassTeacher.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了班主任");
                }


                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassTeacher.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                              where p.Id == id
                              select new Dto.ClassTeacher.Edit
                              {
                                  Id = p.Id,
                                  ClassId = p.tbClass.Id,
                                  TeacherId = p.tbTeacher.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ClassTeacherEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ClassTeacher.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ClassTeacherEdit.Id == 0)
                    {
                        var tb = new Basis.Entity.tbClassTeacher();
                        tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(vm.ClassId);
                        tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.ClassTeacherEdit.TeacherId);
                        db.Set<Basis.Entity.tbClassTeacher>().Add(tb);

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了班主任");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                  where p.Id == vm.ClassTeacherEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了班主任");
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
        public ActionResult Insert(List<int> ids, int classId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassTeacher.Edit();

                var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                        where p.tbClass.Id == classId
                                        select p.tbTeacher.Id).ToList();
                var TeacherList = (from p in db.Table<Teacher.Entity.tbTeacher>()
                                   where ids.Contains(p.Id) && classTeacherList.Contains(p.Id) == false
                                   select p).ToList();
                foreach (var Teacher in TeacherList)
                {
                    var tb = new Basis.Entity.tbClassTeacher();
                    tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(classId);
                    tb.tbTeacher = Teacher;
                    db.Set<Basis.Entity.tbClassTeacher>().Add(tb);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了班主任");
                }

                return Json(new { Status = decimal.One, Message = "操作成功！" });
            }
        }

        [NonAction]
        public bool IsClassTeacher(int yearId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                          where p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                            && p.tbClass.IsDeleted == false
                            && p.tbClass.tbYear.Id == yearId
                          select decimal.One).Any();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetClassByClassTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var year = (from p in db.Table<Basis.Entity.tbYear>()
                            where p.YearType == Code.EnumHelper.YearType.Section
                            orderby p.IsDefault descending
                            select new
                            {
                                p.tbYearParent.tbYearParent.Id
                            }).FirstOrDefault();
                if (year != null)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                              where p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                && p.tbClass.IsDeleted == false
                                && p.tbClass.tbYear.Id == year.Id
                              select new System.Web.Mvc.SelectListItem
                              {
                                  Text = p.tbClass.ClassName,
                                  Value = p.tbClass.Id.ToString()
                              }).ToList();
                    return tb;
                }
                else
                {
                    return new List<SelectListItem>();
                }
            }
        }
    }
}