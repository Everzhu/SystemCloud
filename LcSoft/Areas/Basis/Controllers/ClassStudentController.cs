using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class ClassStudentController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassStudent.List();
                vm.ClassGroupList = ClassGroupController.SelectList(vm.ClassId);
                vm.ClassName = db.Set<Basis.Entity.tbClass>().Find(vm.ClassId).ClassName;

                var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                         where p.tbClass.Id == vm.ClassId
                            && p.tbStudent.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }
                if (vm.ClassGroupId > 0)
                {
                    tb = tb.Where(d => d.tbClassGroup.Id == vm.ClassGroupId);
                }

                vm.ClassStudentList = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Dto.ClassStudent.List
                                       {
                                           Id = p.Id,
                                           StudentId = p.tbStudent.Id,
                                           ClassId = p.tbClass.Id,
                                           ClassName = p.tbClass.ClassName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                           No = p.No,
                                           ClassGroupName = p.tbClassGroup.ClassGroupName,
                                           StudentName = p.tbStudent.StudentName
                                       }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ClassStudent.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                classId = vm.ClassId,
                ClassGroupId = vm.ClassGroupId,
                searchText = vm.SearchText
            }));
        }

        public ActionResult ClassStudentList()
        {
            var vm = new Models.ClassStudent.ClassStudentList();

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.ClassTypeList = ClassTypeController.SelectList();
                vm.GradeList = GradeController.SelectList();
                vm.YearList = YearController.SelectList(Code.EnumHelper.YearType.Year);
                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                vm.ClassList = ClassController.SelectList(vm.YearId ?? 0, vm.GradeId);

                var tb = db.Table<Basis.Entity.tbClassStudent>();
                if (vm.ClassTypeId > 0)
                {
                    tb = tb.Where(d => d.tbClass.tbClassType.Id == vm.ClassTypeId);
                }

                if (vm.GradeId > 0)
                {
                    tb = tb.Where(d => d.tbClass.tbGrade.Id == vm.GradeId);
                }

                if (vm.YearId > 0)
                {
                    tb = tb.Where(d => d.tbClass.tbYear.Id == vm.YearId);
                }

                if (vm.ClassId != 0)
                {
                    tb = tb.Where(d => d.tbClass.Id == vm.ClassId);
                }

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }

                vm.List = (from p in tb
                           orderby p.tbClass.No, p.tbStudent.StudentCode
                           select new Dto.ClassStudent.ClassStudentList()
                           {
                               ClassId = p.tbClass.Id,
                               ClassName = p.tbClass.ClassName,
                               GradeName = p.tbClass.tbGrade.GradeName,
                               Id = p.Id,
                               No = p.No,
                               SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                               StudentCode = p.tbStudent.StudentCode,
                               StudentId = p.tbStudent.Id,
                               StudentName = p.tbStudent.StudentName
                           }).ToPageList(vm.Page);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassStudentList(Models.ClassStudent.ClassStudentList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassStudentList", new
            {
                searchText = vm.SearchText,
                classTypeId = vm.ClassTypeId,
                gradeId = vm.GradeId,
                yearId = vm.YearId,
                classId = vm.ClassId,
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
                var tb = (from p in db.Table<Basis.Entity.tbClassStudent>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了行政班学生");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassStudent.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbClassStudent>()
                              where p.Id == id
                              select new Dto.ClassStudent.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ClassStudentEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ClassStudent.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbClassStudent>()
                              where p.Id == vm.ClassStudentEdit.Id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.No = vm.ClassStudentEdit.No;

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了行政班学生");
                        }
                    }
                    else
                    {
                        error.AddError(Resources.LocalizedText.MsgNotFound);
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
                var error = new List<string>();

                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                        where p.tbClass.Id == classId
                                        select p.tbStudent.Id).ToList();

                var yearId = (from p in db.Table<Basis.Entity.tbClass>()
                              where p.Id == classId
                              select p.tbYear.Id).FirstOrDefault();
                var check = (from p in db.Table<Basis.Entity.tbClassStudent>()
                             where p.tbClass.tbYear.Id == yearId
                                && p.tbClass.IsDeleted == false
                                && p.tbClass.Id != classId
                                && ids.Contains(p.tbStudent.Id)
                             select new
                             {
                                 p.tbStudent.StudentCode,
                                 p.tbStudent.StudentName,
                                 p.tbClass.tbGrade.GradeName,
                                 p.tbClass.ClassName
                             }).ToList();
                if (check.Count > 0)
                {
                    error.AddError(string.Join("\r\n", check.Select(d => d.StudentCode + "(" + d.StudentName + ")已在" + d.GradeName + d.ClassName).ToList()));
                }
                else
                {
                    var maxNo = db.Table<Basis.Entity.tbClassStudent>().Where(d => d.tbClass.Id == classId).Select(d => d.No).DefaultIfEmpty(0).Max();
                    var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                       where ids.Contains(p.Id) && classStudentList.Contains(p.Id) == false
                                       select p).ToList();
                    foreach (var student in studentList)
                    {
                        maxNo = maxNo + 1;

                        var tb = new Basis.Entity.tbClassStudent();
                        tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(classId);
                        tb.tbStudent = student;
                        tb.No = maxNo;
                        db.Set<Basis.Entity.tbClassStudent>().Add(tb);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了行政班学生");
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult ChangeClass(int id)
        {
            var vm = new Basis.Models.ClassStudent.ChangeClass();
            vm.ClassList = ClassController.SelectList();
            vm.Id = id;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeClass(Models.ClassStudent.ChangeClass vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    //todo:添加学生调班记录

                    var tb = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                .Include(d => d.tbClass).Include(d => d.tbStudent)
                              where p.Id == vm.Id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(vm.ClassId);
                        tb.No = vm.No;

                        var studentChange = new Student.Entity.tbStudentChange()
                        {
                            InputDate = DateTime.Now,
                            No = db.Table<Student.Entity.tbStudentChange>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1,
                            tbStudent = db.Set<Student.Entity.tbStudent>().Find(tb.tbStudent.Id),
                            tbStudentChangeType = db.Table<Student.Entity.tbStudentChangeType>().Where(d => d.StudentChangeTypeName == "调班").FirstOrDefault(),
                            tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId)
                        };
                        db.Set<Student.Entity.tbStudentChange>().Add(studentChange);

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生调动");
                        }
                    }
                }
            }

            return Code.MvcHelper.Post(error);
        }


        public ActionResult LeaveSchool(int id)
        {
            var vm = new Basis.Models.ClassStudent.LeaveSchool();
            vm.leaveSchool.StudentId = id;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LeaveSchool(Basis.Models.ClassStudent.LeaveSchool vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var student = (from p in db.Table<Student.Entity.tbStudent>()
                                       .Include(d => d.tbSysUser)
                                   where p.Id == vm.leaveSchool.StudentId
                                   select p).FirstOrDefault();

                    new Student.Controllers.StudentController().Delete(new List<int>() { vm.leaveSchool.StudentId });
                    new Sys.Controllers.SysUserController().Approval(new List<int>() { student.tbSysUser.Id });

                    var tb = (from p in db.Table<Basis.Entity.tbClassStudent>()
                              .Include(d => d.tbClass)
                              where p.tbStudent.Id == vm.leaveSchool.StudentId
                              select p).FirstOrDefault();

                    tb.IsDeleted = true;
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生调动");
                    }
                }
            }

            return Code.MvcHelper.Post(error);
        }

        public ActionResult Import()
        {
            var vm = new Models.ClassStudent.Import();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.ClassStudent.Import vm)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

                using (var db = new XkSystem.Models.DbContext())
                {
                    if (Code.Common.GetFileType(file.FileName) != Code.FileType.Excel)
                    {
                        ModelState.AddModelError("", "上传的文件不是正确的EXCLE文件!");
                        return View(vm);
                    }
                    var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), string.Empty);
                    if (dt == null)
                    {
                        ModelState.AddModelError("", "无法读取上传的文件，请检查文件格式是否正确!");
                        return View(vm);
                    }
                    var tbList = new List<string>() { "学生学号", "学生姓名", "行政班名称", "小组名称" };

                    var Text = string.Empty;
                    foreach (var a in tbList)
                    {
                        if (!dt.Columns.Contains(a.ToString()))
                        {
                            Text += a + ",";
                        }
                    }

                    if (!string.IsNullOrEmpty(Text))
                    {
                        ModelState.AddModelError("", "上传的EXCEL内容与预期不一致!错误详细:" + Text);
                        return View(vm);
                    }

                    //将DataTable转为List
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dto = new Dto.ClassStudent.Import()
                        {
                            ClassName = dr["行政班名称"].ConvertToString(),
                            StudentCode = dr["学生学号"].ConvertToString(),
                            StudentName = dr["学生姓名"].ConvertToString(),
                            ClassGroupName = dr["小组名称"].ConvertToString()
                        };
                        vm.ImportEdit.Add(dto);
                    }

                    vm.ImportEdit.RemoveAll(d =>
                        string.IsNullOrEmpty(d.ClassName)
                        && string.IsNullOrEmpty(d.StudentCode)
                        && string.IsNullOrEmpty(d.StudentName));

                    if (vm.ImportEdit.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }

                    var year = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDisable == true
                                select p).FirstOrDefault();
                    var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                         //where p.tbYear.Id == year.Id
                                     select p).ToList();
                    var studentList = db.Table<Student.Entity.tbStudent>().ToList();
                    var classStudentList = db.Table<Basis.Entity.tbClassStudent>()
                        .Include(d => d.tbClass).Include(d => d.tbStudent).ToList();
                    var classGroupList = db.Table<Basis.Entity.tbClassGroup>().ToList();

                    foreach (var item in vm.ImportEdit)
                    {
                        if (string.IsNullOrEmpty(item.ClassName))
                        {
                            item.Error = item.Error + "行政班名称不能为空!";
                        }

                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error = item.Error + "学生学号不能为空!";
                        }

                        if (string.IsNullOrEmpty(item.StudentName))
                        {

                            item.Error = item.Error + "学生姓名不能为空!";
                        }
                        if (studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Count() == 0)
                        {
                            item.Error = item.Error + "学生姓名和学生学号不匹配!";
                        }

                        if (classList.Where(d => d.ClassName == item.ClassName).Count() == 0)
                        {
                            item.Error = item.Error + "行政班不存在!";
                        }
                    }

                    if (vm.ImportEdit.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportEdit.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    //数据导入，判断关键字，存在的数据做更新，不存在的做新增
                    foreach (var item in vm.ImportEdit)
                    {
                        if (classStudentList.Where(d => d.tbClass.ClassName == item.ClassName && d.tbStudent.StudentCode == item.StudentCode).Count() > 0)
                        {
                            item.Error = "数据已存在!";
                            vm.ImportEdit.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                            return View(vm);
                        }
                        else
                        {
                            var tb = new Basis.Entity.tbClassStudent();
                            tb.tbClass = classList.Where(d => d.ClassName == item.ClassName).FirstOrDefault();
                            tb.tbStudent = studentList.Where(d => d.StudentName == item.StudentName && d.StudentCode == item.StudentCode).FirstOrDefault();
                            db.Set<Basis.Entity.tbClassStudent>().Add(tb);
                        }
                    }

                    if (db.SaveChanges() > 0)
                    {
                        vm.ImportEdit.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了行政班学生");
                        vm.Status = true;
                    }
                }
            }
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Basis/Views/ClassStudent/ClassStudentTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult Export()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassStudent.List();
                var file = System.IO.Path.GetTempFileName();

                var studentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                   where p.tbClass.Id == vm.ClassId
                                   select new
                                   {
                                       ClassNo = p.tbClass.No,
                                       p.tbClass.ClassName,
                                       p.tbClass.tbGrade.GradeName,
                                       p.tbStudent.StudentCode,
                                       p.tbStudent.StudentName,
                                       p.tbStudent.tbSysUser.tbSex.SexName,
                                       p.No
                                   }).ToList();
                var dtStudent = new System.Data.DataTable();
                dtStudent.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("年级"),
                        new System.Data.DataColumn("班序"),
                        new System.Data.DataColumn("班级"),
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("座位号")
                    });
                foreach (var a in studentList)
                {
                    var dr = dtStudent.NewRow();
                    dr["年级"] = a.GradeName;
                    dr["班序"] = a.ClassNo;
                    dr["班级"] = a.ClassName;
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["性别"] = a.SexName;
                    dr["座位号"] = a.No;
                    dtStudent.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dtStudent);

                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
                }
                else
                {
                    return View();
                }
            }
        }

        public ActionResult ListByTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassStudent.ListByTeacher();
                vm.ClassList = ClassTeacherController.GetClassByClassTeacher();
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }
                else
                {
                    //return RedirectToAction(Url.Action("ListByTeacher", "OrgStudent", new { area = "Course" }));
                    return Redirect(Url.Action("ListByTeacher", "OrgStudent", new { area = "Course", IsClassTeacher = 2 }));
                }

                if (vm.ClassId != 0)
                {
                    var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                             where p.tbClass.Id == vm.ClassId
                                && p.tbStudent.IsDeleted == false
                             select p;

                    if (string.IsNullOrEmpty(vm.SearchText) == false)
                    {
                        tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                    }

                    vm.ClassStudentList = (from p in tb
                                           orderby p.No, p.tbStudent.StudentCode
                                           select new Dto.ClassStudent.List
                                           {
                                               Id = p.Id,
                                               StudentId = p.tbStudent.Id,
                                               ClassId = p.tbClass.Id,
                                               ClassName = p.tbClass.ClassName,
                                               StudentCode = p.tbStudent.StudentCode,
                                               SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                               No = p.No,
                                               ClassGroupName = p.tbClassGroup.ClassGroupName,
                                               StudentName = p.tbStudent.StudentName
                                           }).ToList();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListByTeacher(Models.ClassStudent.ListByTeacher vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ListByTeacher", new { classId = vm.ClassId, searchText = vm.SearchText }));
        }

        public ActionResult SelectClassStudent()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassStudent.SelectClassStudent();
                return View(vm);
            }
        }

        public ActionResult GetClassStudentTree()
        {
            using (var db = new XkSystem.Models.DbContext())
            {

                var tbClass = (from p in db.Table<Basis.Entity.tbClass>()
                               orderby p.No ascending
                               select p).Distinct().ToList();

                var tbClassIds = (from p in db.Table<Basis.Entity.tbClass>()
                                  orderby p.No ascending
                                  select p.Id).Distinct().ToList();

                var tbCalssStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                      join m in db.Table<Student.Entity.tbStudent>() on p.tbStudent.Id equals m.Id
                                      where tbClassIds.Contains(p.tbClass.Id)
                                      select new Dto.ClassStudent.SelectClassStudent
                                      {
                                          ClassId = p.tbClass.Id,
                                          StudentId = m.Id,
                                          StudentCode = m.StudentCode,
                                          StudentName = m.StudentName,
                                          SysUserId = m.tbSysUser.Id
                                      }).Distinct().ToList();

                List<Code.TreeHelper> result = new List<Code.TreeHelper>() {
                    new Code.TreeHelper() {
                        Id=-1,
                        name="行政班",
                        open=true,
                        children=new List<Code.TreeHelper>()
                    }
                };
                result = GetCourseTree(result, tbClass, tbCalssStudent);
                return Json(result);
            }
        }

        public List<Code.TreeHelper> GetCourseTree(List<Code.TreeHelper> result, List<Basis.Entity.tbClass> tbClass, List<Dto.ClassStudent.SelectClassStudent> tbClassStudent)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                foreach (var item in tbClass)
                {
                    var node = new Code.TreeHelper();
                    node.name = item.ClassName;
                    node.Id = item.Id;
                    node.open = false;
                    node.isChecked = false;
                    node.chkDisabled = false;
                    var classStudent = (from p in tbClassStudent.Where(d => d.ClassId == item.Id)
                                        select p).ToList();
                    node.children = GetCourseTreeSub(classStudent);
                    result[0].children.Add(node);
                }
                return result;
            }
        }

        private List<Code.TreeHelper> GetCourseTreeSub(List<Dto.ClassStudent.SelectClassStudent> courseList)
        {
            var result = new List<Code.TreeHelper>();
            foreach (var item in courseList)
            {
                var node = new Code.TreeHelper();
                node.name = item.StudentName;
                node.Id = item.SysUserId;
                node.open = true;
                node.isChecked = false;
                node.children = null;
                result.Add(node);
            }
            return result;
        }

        [NonAction]
        public static IQueryable<Student.Entity.tbStudent> GetClassStudent(XkSystem.Models.DbContext db, int classId)
        {
            return from p in db.Table<Basis.Entity.tbClassStudent>()
                   where p.Id == classId
                    && p.tbStudent.IsDeleted == false
                   select p.tbStudent;
        }
    }
}