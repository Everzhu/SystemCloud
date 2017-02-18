using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class ClassAllotStudentController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassAllotStudent.List();
                vm.YearList = YearController.SelectList(Code.EnumHelper.YearType.Year);
                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }
                vm.ClassTypeList = ClassTypeController.SelectList();
                vm.SexList = Dict.Controllers.DictSexController.SelectList();

                var tb = from p in db.Table<Basis.Entity.tbClassAllotStudent>()
                         where p.tbYear.Id == vm.YearId
                         select p;
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentName.Contains(vm.SearchText) || d.tbStudent.StudentCode.Contains(vm.SearchText));
                }
                if (vm.ClassTypeId != 0)
                {
                    tb = tb.Where(d => d.tbClassType.Id == vm.ClassTypeId);
                }
                if (vm.SexId != 0)
                {
                    tb = tb.Where(d => d.tbStudent.tbSysUser.tbSex.Id == vm.SexId);
                }

                vm.ClassAllotStudentList = (from p in tb
                                            orderby p.tbStudent.StudentCode, p.tbStudent.StudentName
                                            select new Dto.ClassAllotStudent.List
                                            {
                                                Id = p.Id,
                                                Score = p.Score,
                                                ClassTypeName = p.tbClassType.ClassTypeName,
                                                GradeName = p.tbGrade.GradeName,
                                                StudentCode = p.tbStudent.StudentCode,
                                                StudentName = p.tbStudent.StudentName,
                                                Sex = p.tbStudent.tbSysUser.tbSex.SexName
                                            }).ToPageList(vm.Page);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ClassAllotStudent.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                yearId = vm.YearId,
                sexId = vm.SexId,
                classTypeId = vm.ClassTypeId,
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
                var tb = db.Table<Basis.Entity.tbClassAllotStudent>().Where(d => ids.Contains(d.Id)).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了分班班级");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassAllotStudent.Edit();
                vm.GradeList = GradeController.SelectList();
                vm.ClassTypeList = ClassTypeController.SelectList();
                vm.YearList = YearController.SelectList(Code.EnumHelper.YearType.Year);
                if (vm.ClassAllotStudentEdit.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.ClassAllotStudentEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                if (id != 0)
                {
                    vm.ClassAllotStudentEdit = (from p in db.Table<Basis.Entity.tbClassAllotStudent>()
                                                where p.Id == id
                                                select new Dto.ClassAllotStudent.Edit
                                                {
                                                    Id = p.Id,
                                                    YearId = p.tbYear.Id,
                                                    ClassTypeId = p.tbClassType.Id,
                                                    GradeId = p.tbGrade.Id,
                                                    Score = p.Score,
                                                    StudentCode = p.tbStudent.StudentCode,
                                                    StudentName = p.tbStudent.StudentName
                                                }).FirstOrDefault();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ClassAllotStudent.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ClassAllotStudentEdit.Id != 0)
                    {
                        var tb = db.Set<Basis.Entity.tbClassAllotStudent>().Find(vm.ClassAllotStudentEdit.Id);
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.ClassAllotStudentEdit.YearId);
                        tb.Score = vm.ClassAllotStudentEdit.Score;
                        tb.tbClassType = db.Set<Basis.Entity.tbClassType>().Find(vm.ClassAllotStudentEdit.ClassTypeId);
                        tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.ClassAllotStudentEdit.GradeId);
                        tb.tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == vm.ClassAllotStudentEdit.StudentCode).FirstOrDefault();
                    }
                    else
                    {
                        var student = new Student.Entity.tbStudent();
                        if (db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == vm.ClassAllotStudentEdit.StudentCode && d.StudentName == vm.ClassAllotStudentEdit.StudentName).Count() == 0)
                        {
                            var studentEdit = new Student.Dto.Student.Edit()
                            {
                                StudentCode = vm.ClassAllotStudentEdit.StudentCode,
                                StudentName = vm.ClassAllotStudentEdit.StudentName
                            };
                            var studentEditList = new List<Student.Dto.Student.Edit>();
                            studentEditList.Add(studentEdit);

                            student = Student.Controllers.StudentController.BuildList(db, studentEditList).FirstOrDefault();
                        }
                        else
                        {
                            student = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == vm.ClassAllotStudentEdit.StudentCode && d.StudentName == vm.ClassAllotStudentEdit.StudentName).FirstOrDefault();
                        }

                        var tb = new Basis.Entity.tbClassAllotStudent()
                        {
                            tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.ClassAllotStudentEdit.YearId),
                            Score = vm.ClassAllotStudentEdit.Score,
                            tbClassType = db.Set<Basis.Entity.tbClassType>().Find(vm.ClassAllotStudentEdit.ClassTypeId),
                            tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.ClassAllotStudentEdit.GradeId),
                            tbStudent = student
                        };

                        db.Set<Basis.Entity.tbClassAllotStudent>().Add(tb);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加/修改了分班学生");
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult Insert(List<int> ids, int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var year = db.Set<Basis.Entity.tbYear>().Find(yearId);
                var hasStudentIds = db.Table<Entity.tbClassAllotStudent>().Select(d => d.tbStudent.Id).ToList();
                var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                   where ids.Contains(p.Id)
                                   select p).ToList();
                var list = new List<Basis.Entity.tbClassAllotStudent>();
                foreach (var student in studentList)
                {
                    if (hasStudentIds.Contains(student.Id) == false)
                    {
                        var tb = new Basis.Entity.tbClassAllotStudent();
                        tb.tbClassType = db.Table<Basis.Entity.tbClassType>().OrderBy(d => d.No).FirstOrDefault();
                        tb.tbGrade = db.Table<Basis.Entity.tbGrade>().OrderBy(d => d.No).FirstOrDefault();
                        tb.tbStudent = student;
                        tb.tbYear = year;
                        list.Add(tb);
                    }
                }

                db.Set<Basis.Entity.tbClassAllotStudent>().AddRange(list);
                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Import()
        {
            var vm = new Models.ClassAllotStudent.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Basis/Views/ClassAllotStudent/Template.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.ClassAllotStudent.Import vm)
        {
            vm.ImportStudentList = new List<Dto.ClassAllotStudent.Import>();

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
                    var tbList = new List<string>() { "学号", "姓名", "分班成绩", "年级", "班级类型" };

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
                        ModelState.AddModelError("", "上传的EXCEL行政班内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }

                    #region 将DataTable转为List
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dto = new Dto.ClassAllotStudent.Import()
                        {
                            StudentCode = Convert.ToString(dr["学号"]),
                            StudentName = Convert.ToString(dr["姓名"]),
                            GradeName = Convert.ToString(dr["年级"]),
                            ClassTypeName = Convert.ToString(dr["班级类型"]),
                            Score = Convert.ToString(dr["分班成绩"])
                        };
                        if (vm.ImportStudentList.Where(d => d.StudentCode == dto.StudentCode
                                                        && d.StudentName == dto.StudentName
                                                        && d.GradeName == dto.GradeName
                                                        && d.ClassTypeName == dto.ClassTypeName
                                                        && d.Score == dto.Score).Count() == 0)
                        {
                            vm.ImportStudentList.Add(dto);
                        }
                    }
                    vm.ImportStudentList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.StudentCode) &&
                        string.IsNullOrEmpty(d.StudentName) &&
                        string.IsNullOrEmpty(d.GradeName) &&
                        string.IsNullOrEmpty(d.ClassTypeName) &&
                        string.IsNullOrEmpty(d.Score)
                    );
                    #endregion

                    if (vm.ImportStudentList.Count == 0 && vm.ImportStudentList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }

                    var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                       select p).ToList();
                    var classStudentList = db.Table<Basis.Entity.tbClassStudent>().Where(d => d.tbStudent.IsDeleted == false).ToList();
                    var classAllotStudentList = db.Table<Basis.Entity.tbClassAllotStudent>()
                        .Where(d => d.tbStudent.IsDeleted == false)
                        .Include(d => d.tbStudent).ToList();
                    var yearList = db.Table<Basis.Entity.tbYear>().ToList();
                    var gradeList = db.Table<Basis.Entity.tbGrade>().ToList();
                    var classTypeList = db.Table<Basis.Entity.tbClassType>().ToList();

                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportStudentList)
                    {
                        if (string.IsNullOrEmpty(item.StudentName))
                        {
                            item.Error += "姓名不能为空！";
                        }
                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error += "学号不能为空！";
                        }
                        if (!vm.IsAddStudent)
                        {
                            if (studentList.Where(d => d.StudentCode == item.StudentCode).Count() == 0)
                            {
                                item.Error += "学生不存在!";
                            }
                            if (studentList.Where(d => d.StudentCode == item.StudentCode).Count() > 0
                                && studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Count() == 0)
                            {
                                item.Error += "学号已存在，但姓名不匹配!";
                            }
                            if (classStudentList.Where(d => d.tbStudent.StudentCode == item.StudentCode).Count() > 0)
                            {
                                item.Error += "学生已存在其他行政班内！";
                            }
                            if (studentList.Where(d => d.StudentCode == item.StudentCode).Count() > 1)
                            {
                                item.Error += "学号对应多条学生记录，无法确认需要添加的记录!";
                            }
                        }
                        if (gradeList.Where(d => d.GradeName == item.GradeName).Count() == 0)
                        {
                            item.Error += "年级不存在；";
                        }
                        if (string.IsNullOrEmpty(item.Score))
                        {
                            item.Error += "分班成绩不能为空！";
                        }
                        else
                        {
                            decimal d = 0;
                            if (!decimal.TryParse(item.Score, out d))
                            {
                                item.Error += "分班成绩必须为数字！";
                            }
                        }
                    }
                    if (vm.ImportStudentList.Where(d => !string.IsNullOrEmpty(d.Error)).Count() > 0)
                    {
                        vm.ImportStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 创建外键
                    var editStudentList = new List<Student.Dto.Student.Edit>();
                    foreach (var v in vm.ImportStudentList)
                    {
                        if (studentList.Where(d => d.StudentCode == v.StudentCode && d.StudentName == v.StudentName).Count() == 0)
                        {
                            var editStudent = new Student.Dto.Student.Edit()
                            {
                                StudentCode = v.StudentCode,
                                StudentName = v.StudentName
                            };
                            if (editStudentList.Where(d => d.StudentCode == v.StudentCode && d.StudentName == v.StudentName).Count() == 0)
                            {
                                editStudentList.Add(editStudent);
                            }
                        }
                    }

                    var addStudentList = new List<Student.Entity.tbStudent>();
                    if (editStudentList.Count > 0)
                    {
                        addStudentList = Student.Controllers.StudentController.BuildList(db, editStudentList);
                    }
                    #endregion

                    #region 数据导入，判断关键字，存在的数据做更新，不存在的做新增
                    var addClassAllotStudentList = new List<Basis.Entity.tbClassAllotStudent>();
                    var year = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                    foreach (var item in vm.ImportStudentList)
                    {
                        Basis.Entity.tbClassAllotStudent tb = null;
                        var yearId = yearList.Where(e => e.IsDefault).FirstOrDefault().Id;
                        if (classAllotStudentList.Where(d => d.tbStudent.StudentCode == item.StudentCode).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                #region 修改
                                tb = classAllotStudentList.Where(d => d.tbStudent.StudentCode == item.StudentCode).FirstOrDefault();
                                tb.Score = item.Score.ConvertToDecimal();
                                tb.tbStudent = studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).FirstOrDefault();
                                tb.tbYear = year;
                                #region 添加外键
                                if (!string.IsNullOrEmpty(item.StudentCode) && !string.IsNullOrEmpty(item.StudentName))
                                {
                                    if (vm.IsAddStudent && studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Count() == 0)
                                    {
                                        tb.tbStudent = addStudentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).FirstOrDefault();
                                    }
                                    else
                                    {
                                        tb.tbStudent = studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).FirstOrDefault();
                                    }
                                }
                                if (!string.IsNullOrEmpty(item.GradeName))
                                {
                                    tb.tbGrade = gradeList.Where(d => d.GradeName == item.GradeName).FirstOrDefault();
                                }
                                #endregion

                                #endregion
                            }
                        }
                        else
                        {
                            #region 新增
                            tb = new Basis.Entity.tbClassAllotStudent();
                            tb.tbYear = year;
                            tb.Score = item.Score.ConvertToDecimal();
                            tb.tbClassType = classTypeList.Where(d => d.ClassTypeName == item.ClassTypeName).FirstOrDefault();

                            #region 添加外键
                            if (!string.IsNullOrEmpty(item.StudentCode) && !string.IsNullOrEmpty(item.StudentName))
                            {
                                if (vm.IsAddStudent && studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Count() == 0)
                                {
                                    tb.tbStudent = addStudentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).FirstOrDefault();
                                }
                                else
                                {
                                    tb.tbStudent = studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).FirstOrDefault();
                                }
                            }
                            if (!string.IsNullOrEmpty(item.GradeName))
                            {
                                tb.tbGrade = gradeList.Where(d => d.GradeName == item.GradeName).FirstOrDefault();
                            }
                            #endregion

                            addClassAllotStudentList.Add(tb);
                            #endregion
                        }
                    }
                    #endregion

                    db.Set<Basis.Entity.tbClassAllotStudent>().AddRange(addClassAllotStudentList);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了行政班");
                        vm.Status = true;
                    }
                }
            }

            vm.ImportStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public ActionResult Export(int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassAllotStudent.Export();
                var file = System.IO.Path.GetTempFileName();

                vm.exportList = (from p in db.Table<Basis.Entity.tbClassAllotStudent>()
                                 where p.tbYear.Id == yearId
                                 select new Dto.ClassAllotStudent.Export
                                 {
                                     StudentCode = p.tbStudent.StudentCode,
                                     StudentName = p.tbStudent.StudentName,
                                     Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                     Score = p.Score,
                                     GradeName = p.tbGrade.GradeName,
                                     ClassTypeName = p.tbClassType.ClassTypeName
                                 }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("分班成绩"),
                        new System.Data.DataColumn("年级"),
                        new System.Data.DataColumn("班级类型")
                    });
                foreach (var a in vm.exportList)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["性别"] = a.Sex;
                    dr["分班成绩"] = a.Score;
                    dr["年级"] = a.GradeName;
                    dr["班级类型"] = a.ClassTypeName;
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
    }
}