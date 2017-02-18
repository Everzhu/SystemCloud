using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Student.Controllers
{
    public class StudentSessionController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentSession.List();
                var tb = from p in db.Table<Student.Entity.tbStudentSession>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.StudentSessionName.Contains(vm.SearchText));
                }

                vm.StudentSessionList = (from p in tb
                                         orderby p.No, p.StudentSessionName
                                         select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudentSession.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Student.Entity.tbStudentSession>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                //var studentList = db.Table<Entity.tbStudent>().Where(d => ids.Contains(d.tbStudentSession.Id));
                //foreach (var v in studentList)
                //{
                //    v.tbStudentSession = null;
                //}

                if (db.Table<Entity.tbStudent>().Where(d => ids.Contains(d.tbStudentSession.Id)).Count() > 0)
                {
                    return Code.MvcHelper.Post(new List<string>() {"系统中有关联的数据，不能直接删除此数据！" });
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了学届");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentSession.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Student.Entity.tbStudentSession>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.StudentSessionEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudentSession.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Student.Entity.tbStudentSession>().Where(d=>d.StudentSessionName == vm.StudentSessionEdit.StudentSessionName && d.Id != vm.StudentSessionEdit.Id).Any())
                    {
                        error.AddError("该学届已存在!");
                    }
                    else
                    {
                        if (vm.StudentSessionEdit.Id == 0)
                        {
                            var tb = new Student.Entity.tbStudentSession();
                            tb.No = vm.StudentSessionEdit.No == null ? db.Table<Student.Entity.tbStudentSession>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudentSessionEdit.No;
                            tb.StudentSessionName = vm.StudentSessionEdit.StudentSessionName;
                            db.Set<Student.Entity.tbStudentSession>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了学届");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Student.Entity.tbStudentSession>()
                                      where p.Id == vm.StudentSessionEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.StudentSessionEdit.No == null ? db.Table<Student.Entity.tbStudentSession>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudentSessionEdit.No;
                                tb.StudentSessionName = vm.StudentSessionEdit.StudentSessionName;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了学届");
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
                list = (from p in db.Table<Student.Entity.tbStudentSession>()
                      orderby p.No descending
                      select new System.Web.Mvc.SelectListItem
                      {
                          Text = p.StudentSessionName,
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