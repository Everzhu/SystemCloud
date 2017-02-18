using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Student.Controllers
{
    public class StudentChangeTypeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentChangeType.List();
                var typeList = typeof(Code.EnumHelper.StudentChangeType).ToItemList();
                var tb = from p in db.Table<Student.Entity.tbStudentChangeType>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.StudentChangeTypeName.Contains(vm.SearchText));
                }

                vm.StudentChangeTypeList = (from p in tb
                                            orderby p.No, p.StudentChangeTypeName
                                            select new Dto.StudentChangeType.List
                                            {
                                                Id = p.Id,
                                                No = p.No,
                                                StudentChangeTypeCode = (int)p.StudentChangeType,
                                                StudentChangeTypeName = p.StudentChangeTypeName
                                            }).ToList();

                foreach (var v in vm.StudentChangeTypeList)
                {
                    v.StudentChangeTypeCodeName = typeList.Where(d => d.Value == v.StudentChangeTypeCode.ToString()).FirstOrDefault().Text;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudentChangeType.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Student.Entity.tbStudentChangeType>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了学生状态");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentChangeType.Edit();
                vm.StudentChangeTypeList = typeof(Code.EnumHelper.StudentChangeType).ToItemList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Student.Entity.tbStudentChangeType>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.StudentChangeTypeEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudentChangeType.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Student.Entity.tbStudentChangeType>().Where(d => d.StudentChangeTypeName == vm.StudentChangeTypeEdit.StudentChangeTypeName && d.Id != vm.StudentChangeTypeEdit.Id).Any())
                    {
                        error.AddError("该学生状态已存在!");
                    }
                    else
                    {
                        if (vm.StudentChangeTypeEdit.Id == 0)
                        {
                            var tb = new Student.Entity.tbStudentChangeType();
                            tb.No = vm.StudentChangeTypeEdit.No == null ? db.Table<Student.Entity.tbStudentChangeType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudentChangeTypeEdit.No;
                            tb.StudentChangeTypeName = vm.StudentChangeTypeEdit.StudentChangeTypeName;
                            tb.StudentChangeType = (Code.EnumHelper.StudentChangeType)vm.StudentChangeTypeEdit.StudentChangeType;
                            db.Set<Student.Entity.tbStudentChangeType>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了学生状态");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Student.Entity.tbStudentChangeType>()
                                      where p.Id == vm.StudentChangeTypeEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.StudentChangeTypeEdit.No == null ? db.Table<Student.Entity.tbStudentChangeType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudentChangeTypeEdit.No;
                                tb.StudentChangeTypeName = vm.StudentChangeTypeEdit.StudentChangeTypeName;
                                tb.StudentChangeType = (Code.EnumHelper.StudentChangeType)vm.StudentChangeTypeEdit.StudentChangeType;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了学生状态");
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
        public static List<System.Web.Mvc.SelectListItem> SelectList(Code.EnumHelper.StudentChangeType? changeType = null)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var list = (from p in db.Table<Student.Entity.tbStudentChangeType>()
                            where p.StudentChangeType == changeType || changeType == null
                            orderby p.No, p.StudentChangeTypeName
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.StudentChangeTypeName,
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }
    }
}