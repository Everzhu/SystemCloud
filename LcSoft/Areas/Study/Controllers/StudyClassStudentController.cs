using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyClassStudentController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyClassStudent.List();

                vm.ClassList = Study.Controllers.StudyClassController.SelectList(vm.StudyId);
                vm.ClassList.Insert(0, new SelectListItem { Text = "==全部==", Value = "0" });

                var tbClassIds = (from p in db.Table<Study.Entity.tbStudyClass>()
                                  where p.tbStudy.Id == vm.StudyId
                                  && p.tbStudy.IsDeleted == false
                                  && p.tbClass.IsDeleted == false
                                  select p.tbClass.Id).Distinct().ToList();

                var tbClassStudent = from p in db.Table<Basis.Entity.tbClassStudent>()
                                     orderby p.tbClass.No, p.No
                                     where tbClassIds.Contains(p.tbClass.Id)
                                     && p.tbClass.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     select p;

                if (vm.ClassId != 0)
                {
                    tbClassStudent = tbClassStudent.Where(d => d.tbClass.Id == vm.ClassId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbClassStudent = tbClassStudent.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }

                var tbStudyClassStudentList = from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                              where p.tbStudy.Id == vm.StudyId
                                              && tbClassIds.Contains(p.tbClass.Id)
                                              && p.tbStudy.IsDeleted == false
                                              && p.tbClass.IsDeleted == false
                                              && p.tbStudent.IsDeleted == false
                                              select p;

                vm.StudyClassStudentList = (from p in tbClassStudent
                                            select new Dto.StudyClassStudent.List
                                            {
                                                Id = p.Id,
                                                StudentId = p.tbStudent.Id,
                                                StudyId = vm.StudyId,
                                                ClassId = p.tbClass.Id,
                                                ClassName = p.tbClass.ClassName,
                                                StudentCode = p.tbStudent.StudentCode,
                                                SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                                StudentName = p.tbStudent.StudentName,
                                                IsApplyStudy = tbStudyClassStudentList.Where(d => d.tbClass.Id == p.tbClass.Id && d.tbStudent.Id == p.tbStudent.Id).Count() > 0 ? true : false
                                            }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudyClassStudent.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { studyId = vm.StudyId, classId = vm.ClassId, searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Study.Entity.tbStudyClassStudent>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了晚自习班级学生");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetIsApplyStudy(int studyId, int classId, int studentId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Study.Entity.tbStudyClassStudent>().Where(d => d.tbStudy.Id == studyId && d.tbClass.Id == classId && d.tbStudent.Id == studentId).FirstOrDefault();
                if (tb != null)
                {
                    tb.IsDeleted = true;
                    tb.UpdateTime = DateTime.Now;
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习模式");
                    }
                }
                else
                {
                    var tbStudent = new Study.Entity.tbStudyClassStudent();
                    tbStudent.tbClass = db.Set<Basis.Entity.tbClass>().Find(classId);
                    tbStudent.tbStudent = db.Set<Student.Entity.tbStudent>().Find(studentId);
                    tbStudent.tbStudy = db.Set<Study.Entity.tbStudy>().Find(studyId);
                    db.Set<Study.Entity.tbStudyClassStudent>().Add(tbStudent);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习模式");
                    }
                }
            }
            return Code.MvcHelper.Post(null, Url.Action("List"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(List<int> ids, int studyId, int classId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var studyClassStudentList = (from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                             where p.tbStudy.Id == studyId
                                             && p.tbClass.Id == classId
                                             && p.tbStudy.IsDeleted == false
                                             && p.tbClass.IsDeleted == false
                                             && p.tbStudent.IsDeleted == false
                                             select p.tbStudent.Id).ToList();

                var check = (from p in db.Table<Study.Entity.tbStudyClassStudent>()
                             where p.tbClass.IsDeleted == false
                             && p.tbStudent.IsDeleted == false
                             && p.tbStudy.IsDeleted == false
                             && p.tbClass.Id != classId
                             && ids.Contains(p.tbStudent.Id)
                             select new
                             {
                                 p.tbStudent.StudentCode,
                                 p.tbStudent.StudentName,
                                 p.tbClass.ClassName,
                                 p.tbStudy.StudyName
                             }).ToList();

                if (check.Count > 0)
                {
                    error.AddError(string.Join("\r\n", check.Select(d => d.StudentCode + "(" + d.StudentName + ")已在" + d.StudyName + d.ClassName).ToList()));
                }
                else
                {
                    var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                       where ids.Contains(p.Id) && studyClassStudentList.Contains(p.Id) == false
                                       select p).ToList();
                    foreach (var student in studentList)
                    {
                        var tb = new Study.Entity.tbStudyClassStudent();
                        tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(classId);
                        tb.tbStudent = student;
                        tb.tbStudy = db.Set<Study.Entity.tbStudy>().Find(studyId);
                        db.Set<Study.Entity.tbStudyClassStudent>().Add(tb);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了晚自习班级学生");
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult Export()
        {
            var vm = new Models.StudyClassStudent.List();

            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tb = from p in db.Table<Study.Entity.tbStudyClassStudent>()
                         where p.tbStudy.Id == vm.StudyId
                         && p.tbStudy.IsDeleted == false
                         && p.tbClass.IsDeleted == false
                         && p.tbStudent.IsDeleted == false
                         select p;

                if (vm.ClassId != 0)
                {
                    tb = tb.Where(d => d.tbClass.Id == vm.ClassId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }

                vm.StudyClassStudentList = (from p in tb
                                            orderby p.tbClass.No, p.tbStudent.StudentCode
                                            select new Dto.StudyClassStudent.List
                                            {
                                                Id = p.Id,
                                                StudentId = p.tbStudent.Id,
                                                StudyId = p.tbStudy.Id,
                                                StudyName = p.tbStudy.StudyName,
                                                ClassId = p.tbClass.Id,
                                                ClassName = p.tbClass.ClassName,
                                                StudentCode = p.tbStudent.StudentCode,
                                                SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                                StudentName = p.tbStudent.StudentName
                                            }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("序号"),
                        new System.Data.DataColumn("晚自习名称"),
                        new System.Data.DataColumn("行政班"),
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("性别")
                    });
                var index = 0;
                foreach (var a in vm.StudyClassStudentList)
                {
                    index++;
                    var dr = dt.NewRow();
                    dr["序号"] = index.ToString();
                    dr["晚自习名称"] = a.StudyName;
                    dr["行政班"] = a.ClassName;
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["性别"] = a.SexName;
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);

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

        public ActionResult Import()
        {
            var vm = new Models.StudyClassStudent.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Study/Views/StudyClassStudent/StudyClassStudentTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.StudyClassStudent.Import vm)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);
                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 1、Excel模版校验
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
                    var tbList = new List<string>() { "行政班", "学号", "姓名" };

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
                        ModelState.AddModelError("", "上传的EXCEL晚自习内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }
                    #endregion

                    #region 2、Excel数据读取
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dtoStudyClassStudent = new Dto.StudyClassStudent.Import()
                        {
                            ClassName = Convert.ToString(dr["行政班"]),
                            StudentCode = Convert.ToString(dr["学号"]),
                            StudentName = Convert.ToString(dr["姓名"])
                        };
                        if (vm.ImportList.Where(d => d.ClassName == dtoStudyClassStudent.ClassName
                                                && d.StudentCode == dtoStudyClassStudent.StudentCode
                                                && d.StudentName == dtoStudyClassStudent.StudentName).Count() == 0)
                        {
                            vm.ImportList.Add(dtoStudyClassStudent);
                        }
                    }
                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.ClassName) &&
                        string.IsNullOrEmpty(d.StudentCode) &&
                        string.IsNullOrEmpty(d.StudentName)
                    );
                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "Excel未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    #region 3、Excel格式校验
                    //学生列表
                    var StudyClassStudentList = (from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                                .Include(d => d.tbStudent)
                                                .Include(d => d.tbStudy)
                                                .Include(d => d.tbClass)
                                                 select p).ToList();

                    var StudyClassList = (from p in db.Table<Study.Entity.tbStudyClass>()
                                          .Include(d => d.tbStudy)
                                          .Include(d => d.tbClass)
                                          where p.tbStudy.Id == vm.StudyId
                                          && p.tbStudy.IsDeleted == false
                                          && p.tbClass.IsDeleted == false
                                          select p).ToList();
                    //学生列表
                    var StudentList = (from p in db.Table<Student.Entity.tbStudent>() select p).ToList();

                    var StudyName = db.Set<Study.Entity.tbStudy>().Find(vm.StudyId).StudyName;

                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.ClassName))
                        {
                            item.Error = item.Error + "行政班不能为空!";
                            continue;
                        }
                        else
                        {
                            if (StudyClassList.Where(d => d.tbClass.ClassName == item.ClassName).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "行政班没有参加【" + StudyName + "】";
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error = item.Error + "学号不能为空!";
                            continue;
                        }
                        else
                        {
                            if (StudentList.Where(d => d.StudentCode == item.StudentCode).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "学生不存在数据库!";
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(item.StudentName))
                        {
                            item.Error = item.Error + "姓名不能为空!";
                            continue;
                        }
                        else
                        {
                            if (StudentList.Where(d => d.StudentName == item.StudentName).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "学生不存在数据库!";
                                continue;
                            }
                        }
                        if (StudyClassStudentList.Where(d => d.tbStudy.Id == vm.StudyId && d.tbClass.ClassName == item.ClassName && d.tbStudent.StudentCode == item.StudentCode).Count() > 0)
                        {
                            item.Error += "系统中已存在该记录!";
                            continue;
                        }
                    }
                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 4、Excel执行导入
                    var addStudyClassStudentList = new List<Study.Entity.tbStudyClassStudent>();
                    foreach (var item in vm.ImportList)
                    {
                        Study.Entity.tbStudyClassStudent tb = null;
                        if (StudyClassStudentList.Where(d => d.tbStudy.Id == vm.StudyId && d.tbClass.ClassName == item.ClassName && d.tbStudent.StudentCode == item.StudentCode).Count() == decimal.Zero)
                        {
                            tb = new Study.Entity.tbStudyClassStudent();
                            tb.tbStudy = db.Set<Study.Entity.tbStudy>().Find(vm.StudyId);
                            tb.tbClass = db.Table<Basis.Entity.tbClass>().Where(d => d.ClassName == item.ClassName).FirstOrDefault();
                            tb.tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == item.StudentCode).FirstOrDefault();
                            addStudyClassStudentList.Add(tb);
                        }
                    }
                    db.Set<Study.Entity.tbStudyClassStudent>().AddRange(addStudyClassStudentList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了晚自习行政班学生");
                        vm.Status = true;
                    }
                    #endregion
                }
            }
            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }
    }
}