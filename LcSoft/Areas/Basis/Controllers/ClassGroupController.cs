using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XkSystem.Areas.Basis.Dto.ClassGroup;

namespace XkSystem.Areas.Basis.Controllers
{
    public class ClassGroupController : Controller
    {
        public ActionResult List(int classId)
        {
            var vm = new Models.ClassGroup.List();
            vm.ClassId = classId;

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Basis.Entity.tbClassGroup>().Where(d => d.tbClass.Id == classId);
                var classStudentList = db.Table<Basis.Entity.tbClassStudent>()
                    .Include(d => d.tbClassGroup)
                    .Where(d => d.tbClass.Id == vm.ClassId).ToList();

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.ClassGroupName.Contains(vm.SearchText));
                }

                vm.ClassGroupList = (from p in tb
                                     orderby p.No
                                     select new Dto.ClassGroup.List()
                                     {
                                         Id = p.Id,
                                         No = p.No,
                                         ClassGroupName = p.ClassGroupName,
                                         TeacherCode = p.tbTeacher.TeacherCode,
                                         TeacherName = p.tbTeacher.TeacherName
                                     }).ToPageList(vm.Page);

                foreach (var v in vm.ClassGroupList)
                {
                    v.StudentCount = classStudentList.Where(d => d.tbClassGroup != null && d.tbClassGroup.Id == v.Id).Count();
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ClassGroup.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
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
                var tb = (from p in db.Table<Basis.Entity.tbClassGroup>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了行政班小组");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int classId, int id = 0)
        {
            var vm = new Models.ClassGroup.Edit();
            vm.ClassId = classId;
            if (id != 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.ClassGroupEdit = (from p in db.Table<Basis.Entity.tbClassGroup>()
                                         where p.Id == id
                                         select new Dto.ClassGroup.Edit()
                                         {
                                             Id = p.Id,
                                             No = p.No,
                                             ClassGroupName = p.ClassGroupName,
                                             TeacherId = p.tbTeacher.Id,
                                             ClassName = p.tbClass.ClassName,
                                             TeacherCode = p.tbTeacher.TeacherCode + "(" + p.tbTeacher.TeacherName + ")"
                                         }).FirstOrDefault();
                }
            }
            vm.TeacherList = Teacher.Controllers.TeacherController.SelectList1(vm.ClassGroupEdit.TeacherId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ClassGroup.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var classGroupList = db.Table<Basis.Entity.tbClassGroup>()
                        .Include(d => d.tbTeacher)
                        .Where(d => d.tbClass.Id == vm.ClassId).ToList();
                    //var teacherCode = vm.ClassGroupEdit.TeacherCode.Split(new char[] { '(', ')' })[0];
                    //var teacherName = vm.ClassGroupEdit.TeacherCode.Split(new char[] { '(', ')' })[1];
                    if (vm.ClassGroupEdit.Id > 0)
                    {
                        //判断重名
                        if (classGroupList.Where(d => d.ClassGroupName == vm.ClassGroupEdit.ClassGroupName).Count() > 1)
                        {
                            error.Add("小组名称重复！");
                            return Code.MvcHelper.Post(error);
                        }

                        var tb = classGroupList.Where(d => d.Id == vm.ClassGroupEdit.Id).FirstOrDefault();
                        tb.No = vm.ClassGroupEdit.No;
                        tb.ClassGroupName = vm.ClassGroupEdit.ClassGroupName;
                        tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.ClassGroupEdit.TeacherId);
                        //tb.tbTeacher = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.TeacherCode == teacherCode && d.TeacherName == teacherName).FirstOrDefault();
                    }
                    else
                    {
                        //判断重名
                        if (classGroupList.Where(d => d.ClassGroupName == vm.ClassGroupEdit.ClassGroupName).Count() > 0)
                        {
                            error.Add("小组名称重复！");
                            return Code.MvcHelper.Post(error);
                        }

                        var tb = new Basis.Entity.tbClassGroup()
                        {
                            ClassGroupName = vm.ClassGroupEdit.ClassGroupName,
                            No = vm.ClassGroupEdit.No,
                            tbClass = db.Set<Basis.Entity.tbClass>().Find(vm.ClassId),
                            tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.ClassGroupEdit.TeacherId)
                            //tbTeacher = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.TeacherCode == teacherCode && d.TeacherName == teacherName).FirstOrDefault()
                        };
                        db.Set<Basis.Entity.tbClassGroup>().Add(tb);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加/修改了行政班小组");
                    }
                }
            }

            return Code.MvcHelper.Post(error);
        }

        public ActionResult AddClassStudentToGroup(int classId, int classGroupId)
        {
            var vm = new Models.ClassGroup.AddClassStudentToGroup();
            vm.ClassGroupId = classGroupId;
            vm.ClassId = classId;

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Basis.Entity.tbClassStudent>()
                    .Include(d => d.tbStudent)
                    .Include(d => d.tbStudent.tbSysUser.tbSex)
                    .Include(d => d.tbClassGroup)
                    .Where(d => d.tbClass.Id == classId);

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentName.Contains(vm.SearchText) || d.tbStudent.StudentCode.Contains(vm.SearchText));
                }
                vm.ClassStudentList = (from p in tb
                                       orderby p.No
                                       select new Dto.ClassGroup.AddClassStudentToGroup()
                                       {
                                           Id = p.Id,
                                           ClassGroupName = p.tbClassGroup.ClassGroupName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName,
                                           SexName = p.tbStudent.tbSysUser.tbSex.SexName
                                       }).ToPageList(vm.Page);
            }

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddClassStudentToGroup(Models.ClassGroup.AddClassStudentToGroup vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("AddClassStudentToGroup", new
            {
                searchText = vm.SearchText,
                ClassGroupId = vm.ClassGroupId,
                ClassId = vm.ClassId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(List<int> ids, int classId, int classGroupId)
        {
            var error = new List<string>();
            if (error.Count == 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var classStudentList = db.Table<Basis.Entity.tbClassStudent>()
                        .Include(d => d.tbStudent)
                        .Include(d => d.tbClassGroup).Where(d => d.tbClass.Id == classId).ToList();

                    var errorStr = new StringBuilder();
                    foreach (var id in ids)
                    {
                        if (classStudentList.Where(d => d.Id == id && d.tbClassGroup != null).Count() > 0)
                        {
                            var classStudent = classStudentList.Where(d => d.Id == id).FirstOrDefault();
                            errorStr.Append(classStudent.tbStudent.StudentCode + "(" + classStudent.tbStudent.StudentName + ")已在小组" + classStudent.tbClassGroup.ClassGroupName + "中。\r\n");
                        }
                    }
                    if (errorStr.Length > 0)
                    {
                        error.Add(errorStr.ToString());
                        return Code.MvcHelper.Post(error);
                    }

                    var classGroup = db.Set<Basis.Entity.tbClassGroup>().Find(classGroupId);
                    foreach (var id in ids)
                    {
                        classStudentList.Where(d => d.Id == id).FirstOrDefault().tbClassGroup = classGroup;
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了行政班小组学生");
                    }
                }
            }

            return Code.MvcHelper.Post(error);
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int classId, int id = 0)
        {
            var list = new List<System.Web.Mvc.SelectListItem>();
            using (var db = new XkSystem.Models.DbContext())
            {
                list = (from p in db.Table<Basis.Entity.tbClassGroup>()
                        where p.tbClass.Id == classId
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = p.ClassGroupName,
                            Value = p.Id.ToString()
                        }).ToList();

                if (id > 0)
                {
                    list.Where(d => d.Value == id.ConvertToString()).FirstOrDefault().Selected = true;
                }
            }
            return list;
        }

        [NonAction]
        internal static List<Info> SelectList(List<int> moralClassIds)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return (from p in db.Table<Basis.Entity.tbClassGroup>()
                        where moralClassIds.Contains(p.tbClass.Id)
                        select new Dto.ClassGroup.Info()
                        {
                            Id = p.Id,
                            ClassId = p.tbClass.Id,
                            ClassName = p.tbClass.ClassName,
                            ClassGroupName = p.ClassGroupName,
                        }).ToList();
            }
        }

    }
}