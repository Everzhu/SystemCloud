using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Student.Controllers
{
    public class StudentStudyTypeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentStudyType.List();
                var tb = from p in db.Table<Student.Entity.tbStudentStudyType>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.StudyTypeName.Contains(vm.SearchText));
                }

                vm.StudyTypeList = (from p in tb
                                    orderby p.No, p.StudyTypeName
                                    select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudentStudyType.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Student.Entity.tbStudentStudyType>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                //var studentList = db.Table<Entity.tbStudent>().Where(d => ids.Contains(d.tbStudentStudyType.Id));
                //foreach (var v in studentList)
                //{
                //    v.tbStudentStudyType = null;
                //}

                if (db.Table<Entity.tbStudent>().Where(d => ids.Contains(d.tbStudentStudyType.Id)).Count() > 0)
                {
                    return Code.MvcHelper.Post(new List<string>() { "系统中有关联的数据，不能直接删除此数据！" });
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了学习方式");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentStudyType.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Student.Entity.tbStudentStudyType>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.StudyTypeEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudentStudyType.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Student.Entity.tbStudentStudyType>().Where(d=>d.StudyTypeName == vm.StudyTypeEdit.StudyTypeName && d.Id != vm.StudyTypeEdit.Id).Any())
                    {
                        error.AddError("该学习方式已存在!");
                    }
                    else
                    {
                        if (vm.StudyTypeEdit.Id == 0)
                        {
                            var tb = new Student.Entity.tbStudentStudyType();
                            tb.No = vm.StudyTypeEdit.No == null ? db.Table<Student.Entity.tbStudentStudyType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudyTypeEdit.No;
                            tb.StudyTypeName = vm.StudyTypeEdit.StudyTypeName;
                            db.Set<Student.Entity.tbStudentStudyType>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了学习方式");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Student.Entity.tbStudentStudyType>()
                                      where p.Id == vm.StudyTypeEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.StudyTypeEdit.No == null ? db.Table<Student.Entity.tbStudentStudyType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudyTypeEdit.No;
                                tb.StudyTypeName = vm.StudyTypeEdit.StudyTypeName;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了学习方式");
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
                list = (from p in db.Table<Student.Entity.tbStudentStudyType>()
                        orderby p.No, p.StudyTypeName
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = p.StudyTypeName,
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