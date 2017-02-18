using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Student.Controllers
{
    public class StudentTypeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentType.List();
                var tb = from p in db.Table<Student.Entity.tbStudentType>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.StudentTypeName.Contains(vm.SearchText));
                }

                vm.StudentTypeList = (from p in tb
                                      orderby p.No, p.StudentTypeName
                                      select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudentType.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Student.Entity.tbStudentType>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                //var studentList = db.Table<Entity.tbStudent>().Where(d => ids.Contains(d.tbStudentType.Id));
                //foreach (var v in studentList)
                //{
                //    v.tbStudentType = null;
                //}

                if (db.Table<Entity.tbStudent>().Where(d => ids.Contains(d.tbStudentType.Id)).Count() > 0)
                {
                    return Code.MvcHelper.Post(new List<string>() { "系统中有关联的数据，不能直接删除此数据！" });
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了学生类型");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentType.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Student.Entity.tbStudentType>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.StudentTypeEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudentType.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Student.Entity.tbStudentType>().Where(d=>d.StudentTypeName == vm.StudentTypeEdit.StudentTypeName && d.Id != vm.StudentTypeEdit.Id).Any())
                    {
                        error.AddError("该学生类型已存在!");
                    }
                    else
                    {
                        if (vm.StudentTypeEdit.Id == 0)
                        {
                            var tb = new Student.Entity.tbStudentType();
                            tb.No = vm.StudentTypeEdit.No == null ? db.Table<Student.Entity.tbStudentType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudentTypeEdit.No;
                            tb.StudentTypeName = vm.StudentTypeEdit.StudentTypeName;
                            db.Set<Student.Entity.tbStudentType>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了学生类型");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Student.Entity.tbStudentType>()
                                      where p.Id == vm.StudentTypeEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.StudentTypeEdit.No == null ? db.Table<Student.Entity.tbStudentType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudentTypeEdit.No;
                                tb.StudentTypeName = vm.StudentTypeEdit.StudentTypeName;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了学生类型");
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
        public static List<System.Web.Mvc.SelectListItem> SelectList(int id = 0)
        {
            var list = new List<System.Web.Mvc.SelectListItem>();
            using (var db = new XkSystem.Models.DbContext())
            {
                list = (from p in db.Table<Student.Entity.tbStudentType>()
                        orderby p.No, p.StudentTypeName
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = p.StudentTypeName,
                            Value = p.Id.ToString()
                        }).ToList();
                if (id > 0)
                {
                    list.Where(d => d.Value == id.ConvertToString()).FirstOrDefault().Selected = true;
                }
            }
            return list;
        }
    }
}