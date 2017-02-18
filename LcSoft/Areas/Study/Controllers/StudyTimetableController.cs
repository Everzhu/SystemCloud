using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyTimetableController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyTimetable.List();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString(Code.Common.StringToDate);
                }

                if (string.IsNullOrEmpty(vm.DateSearchTo))
                {
                    vm.DateSearchTo = DateTime.Now.ToString(Code.Common.StringToDate);
                }

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);
                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                var periodMoney = (from m in db.Table<Study.Entity.tbStudyCost>()
                                   where m.tbTeacher.IsDeleted == false
                                   select new
                                   {
                                       m.Cost,
                                       TeacherId = m.tbTeacher.Id
                                   }).ToList();

                var tb = (from p in db.Table<Study.Entity.tbStudyTimetable>()
                          orderby p.tbTeacher.No, p.tbTeacher.TeacherCode
                          where p.InputDate >= fromDate && p.InputDate <= toDate
                              && p.tbClass.IsDeleted == false
                              && p.tbClass.tbGrade.Id == vm.GradeId
                              && p.tbTeacher.IsDeleted == false
                              && (!string.IsNullOrEmpty(vm.SearchText) ? (p.tbTeacher.TeacherName.Contains(vm.SearchText) || p.tbTeacher.TeacherCode.Contains(vm.SearchText)) : true)
                          select new
                          {
                              p.Id,
                              TeacherId = p.tbTeacher.Id,
                              p.tbTeacher.TeacherName,
                              p.tbClass.tbGrade.GradeName
                          }).ToList();

                vm.StudyTimetableList = (from p in tb
                                         group p by new
                                         {
                                             p.TeacherId,
                                             p.TeacherName,
                                             p.GradeName
                                         } into g
                                         select new Dto.StudyTimetable.List
                                         {
                                             TeacherName = g.Key.TeacherName,
                                             GradeName = g.Key.GradeName,
                                             ValueCount = tb.Where(d => d.TeacherId == g.Key.TeacherId).Count(),
                                             TypeName = "晚修",
                                             PeriodMoney = periodMoney.Where(d => d.TeacherId == g.Key.TeacherId).Select(d => d.Cost).FirstOrDefault().ToString(),
                                             Allowance = ((from p in periodMoney
                                                           where p.TeacherId == g.Key.TeacherId
                                                           select p.Cost).FirstOrDefault()
                                                         * tb.Where(c => c.TeacherId == g.Key.TeacherId).Count()).ToString()
                                         }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudyTimetable.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                DateSearchFrom = vm.DateSearchFrom,
                DateSearchTo = vm.DateSearchTo,
                gradeId = vm.GradeId,
            }));
        }

        public ActionResult Report()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyTimetable.List();
                vm.GradeList = Teacher.Controllers.TeacherGradeController.GetGradeByTeacher();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString(Code.Common.StringToDate);
                }

                if (string.IsNullOrEmpty(vm.DateSearchTo))
                {
                    vm.DateSearchTo = DateTime.Now.ToString(Code.Common.StringToDate);
                }

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);
                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                var periodMoney = (from m in db.Table<Study.Entity.tbStudyCost>()
                                   where m.tbTeacher.IsDeleted == false
                                   select new
                                   {
                                       m.Cost,
                                       TeacherId = m.tbTeacher.Id
                                   }).ToList();

                var tb = (from p in db.Table<Study.Entity.tbStudyTimetable>()
                          orderby p.tbTeacher.No, p.tbTeacher.TeacherCode
                          where p.InputDate >= fromDate && p.InputDate <= toDate
                              && p.tbClass.IsDeleted == false
                              && p.tbClass.tbGrade.Id == vm.GradeId
                              && p.tbTeacher.IsDeleted == false
                              && (!string.IsNullOrEmpty(vm.SearchText) ? (p.tbTeacher.TeacherName.Contains(vm.SearchText) || p.tbTeacher.TeacherCode.Contains(vm.SearchText)) : true)
                          select new
                          {
                              p.Id,
                              TeacherId = p.tbTeacher.Id,
                              p.tbTeacher.TeacherName,
                              p.tbClass.tbGrade.GradeName
                          }).ToList();

                vm.StudyTimetableList = (from p in tb
                                         group p by new
                                         {
                                             p.TeacherId,
                                             p.TeacherName,
                                             p.GradeName
                                         } into g
                                         select new Dto.StudyTimetable.List
                                         {
                                             TeacherName = g.Key.TeacherName,
                                             GradeName = g.Key.GradeName,
                                             ValueCount = tb.Where(d => d.TeacherId == g.Key.TeacherId).Count(),
                                             TypeName = "晚修",
                                             PeriodMoney = periodMoney.Where(d => d.TeacherId == g.Key.TeacherId).Select(d => d.Cost).FirstOrDefault().ToString(),
                                             Allowance = ((from p in periodMoney
                                                           where p.TeacherId == g.Key.TeacherId
                                                           select p.Cost).FirstOrDefault()
                                                         * tb.Where(c => c.TeacherId == g.Key.TeacherId).Count()).ToString()
                                         }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Report(Models.StudyTimetable.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("Report", new
            {
                searchText = vm.SearchText,
                DateSearchFrom = vm.DateSearchFrom,
                DateSearchTo = vm.DateSearchTo,
                gradeId = vm.GradeId,
            }));
        }

        public ActionResult Makeup()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyTimetable.List();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString(Code.Common.StringToDate);
                }

                if (string.IsNullOrEmpty(vm.DateSearchTo))
                {
                    vm.DateSearchTo = DateTime.Now.ToString(Code.Common.StringToDate);
                }

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);
                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                var periodMoney = (from m in db.Table<Study.Entity.tbStudyCost>()
                                   where m.tbTeacher.IsDeleted == false
                                   select new
                                   {
                                       m.Cost,
                                       TeacherId = m.tbTeacher.Id
                                   }).ToList();

                var tb = (from p in db.Table<Course.Entity.tbOrgCalendar>()
                          where p.CalendarDate >= fromDate && p.CalendarDate <= toDate
                              && p.tbOrg.OrgName.Contains("补课")
                              && p.tbOrg.IsDeleted == false
                              && p.tbTeacher.IsDeleted == false
                               && (!string.IsNullOrEmpty(vm.SearchText) ? (p.tbTeacher.TeacherName.Contains(vm.SearchText) || p.tbTeacher.TeacherCode.Contains(vm.SearchText)) : true)
                          orderby p.tbTeacher.No, p.tbTeacher.TeacherCode
                          select new
                          {
                              p.Id,
                              TeacherId = p.tbTeacher.Id,
                              p.tbTeacher.TeacherName,
                              p.tbOrg.tbGrade.GradeName
                          }).ToList();

                vm.StudyTimetableList = (from p in tb
                                         group p by new
                                         {
                                             p.TeacherId,
                                             p.TeacherName,
                                             p.GradeName
                                         } into g
                                         select new Dto.StudyTimetable.List
                                         {
                                             TeacherName = g.Key.TeacherName,
                                             GradeName = g.Key.GradeName,
                                             ValueCount = tb.Where(d => d.TeacherId == g.Key.TeacherId).Count(),
                                             TypeName = "补课",
                                             PeriodMoney = periodMoney.Where(d => d.TeacherId == g.Key.TeacherId).Select(d => d.Cost).FirstOrDefault().ToString(),
                                             Allowance = ((from p in periodMoney
                                                           where p.TeacherId == g.Key.TeacherId
                                                           select p.Cost).FirstOrDefault()
                                                         * tb.Where(c => c.TeacherId == g.Key.TeacherId).Count()).ToString()
                                         }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Makeup(Models.StudyTimetable.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("Makeup", new
            {
                searchText = vm.SearchText,
                DateSearchFrom = vm.DateSearchFrom,
                DateSearchTo = vm.DateSearchTo,
                gradeId = vm.GradeId,
            }));
        }

        public ActionResult TeacherAllowanceList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyTimetable.List();
                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = DateTime.Now.ToString("yyyy-MM");
                }
                return View(vm);
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Study.Edit();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.StudyEdit.YearId == 0)
                {
                    vm.StudyEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                if (id != 0)
                {
                    var tb = (from p in db.Table<Study.Entity.tbStudy>()
                              where p.Id == id
                              select new Dto.Study.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  IsApply = p.IsApply,
                                  IsRoom = p.IsRoom,
                                  StudyName = p.StudyName,
                                  YearId = p.tbYear.Id,
                                  ApplyFrom = p.ApplyFrom,
                                  ApplyTo = p.ApplyTo
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.StudyEdit = tb;
                        vm.ApplyFrom = tb.ApplyFrom.ToString(XkSystem.Code.Common.StringToDateTime);
                        vm.ApplyTo = tb.ApplyTo.ToString(XkSystem.Code.Common.StringToDateTime);
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Study.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.StudyEdit.Id == 0)
                    {
                        var tb = new Study.Entity.tbStudy();
                        tb.No = vm.StudyEdit.No == null ? db.Table<Study.Entity.tbStudy>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudyEdit.No;
                        tb.StudyName = vm.StudyEdit.StudyName;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.StudyEdit.YearId);
                        tb.IsApply = vm.StudyEdit.IsApply;
                        tb.IsRoom = vm.StudyEdit.IsRoom;
                        tb.ApplyFrom = DateTime.Parse(vm.ApplyFrom);
                        tb.ApplyTo = DateTime.Parse(vm.ApplyTo);
                        db.Set<Study.Entity.tbStudy>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加晚自习");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Study.Entity.tbStudy>()
                                  where p.Id == vm.StudyEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.StudyEdit.No == null ? db.Table<Study.Entity.tbStudy>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudyEdit.No;
                            tb.StudyName = vm.StudyEdit.StudyName;
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.StudyEdit.YearId);
                            tb.IsApply = vm.StudyEdit.IsApply;
                            tb.IsRoom = vm.StudyEdit.IsRoom;
                            tb.ApplyFrom = DateTime.Parse(vm.ApplyFrom);
                            tb.ApplyTo = DateTime.Parse(vm.ApplyTo);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }
                return Code.MvcHelper.Post(error, Url.Action("List"));
            }
        }

        #region 导入导出
        public ActionResult Export(int gradeId, string dateSearchFrom, string dateSearchTo, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyTimetable.List();
                var file = System.IO.Path.GetTempFileName();

                var fromDate = Convert.ToDateTime(dateSearchFrom);
                var toDate = Convert.ToDateTime(dateSearchTo);

                var periodMoney = (from m in db.Table<Study.Entity.tbStudyCost>()
                                   where m.tbTeacher.IsDeleted == false
                                   select new
                                   {
                                       m.Cost,
                                       TeacherId = m.tbTeacher.Id
                                   }).ToList();

                var tb = (from p in db.Table<Study.Entity.tbStudyTimetable>()
                          orderby p.tbTeacher.No, p.tbTeacher.TeacherCode
                          where p.InputDate >= fromDate && p.InputDate < toDate
                              && p.tbClass.IsDeleted == false
                              && p.tbClass.tbGrade.Id == vm.GradeId
                              && p.tbTeacher.IsDeleted == false
                              && (!string.IsNullOrEmpty(searchText) ? (p.tbTeacher.TeacherName.Contains(searchText) || p.tbTeacher.TeacherCode.Contains(searchText)) : true)
                          select new
                          {
                              p.Id,
                              TeacherId = p.tbTeacher.Id,
                              p.tbTeacher.TeacherName,
                              p.tbClass.tbGrade.GradeName
                          }).ToList();

                var StudyTimetableList = (from p in tb
                                          group p by new
                                          {
                                              p.TeacherId,
                                              p.TeacherName,
                                              p.GradeName
                                          } into g
                                          select new
                                          {
                                              TeacherName = g.Key.TeacherName,
                                              GradeName = g.Key.GradeName,
                                              ValueCount = tb.Where(d => d.TeacherId == g.Key.TeacherId).Count(),
                                              TypeName = "晚修",
                                              PeriodMoney = periodMoney.Where(d => d.TeacherId == g.Key.TeacherId).Select(d => d.Cost).FirstOrDefault().ToString(),
                                              Allowance = ((from p in periodMoney
                                                            where p.TeacherId == g.Key.TeacherId
                                                            select p.Cost).FirstOrDefault()
                                                          * tb.Where(c => c.TeacherId == g.Key.TeacherId).Count()).ToString()
                                          }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("教师姓名"),
                        new System.Data.DataColumn("年级"),
                        new System.Data.DataColumn("考勤次数"),
                        new System.Data.DataColumn("津贴类别"),
                        new System.Data.DataColumn("节次费用(元)"),
                        new System.Data.DataColumn("津贴(元)"),
                    });
                foreach (var a in StudyTimetableList)
                {
                    var dr = dt.NewRow();
                    dr["教师姓名"] = a.TeacherName;
                    dr["年级"] = a.GradeName;
                    dr["考勤次数"] = a.ValueCount;
                    dr["津贴类别"] = a.TypeName;
                    dr["节次费用(元)"] = a.PeriodMoney;
                    dr["津贴(元)"] = a.Allowance;
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
            var vm = new Models.StudyTimetable.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Study/Views/StudyTimetable/ShelfAttendanceImport.xls");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.StudyTimetable.Import vm)
        {
            if (ModelState.IsValid)
            {
                HSSFWorkbook hssfworkbook;
                var file = Request.Files[nameof(vm.UploadFile)];
                var filepath = System.IO.Path.GetTempFileName();
                file.SaveAs(filepath);

                using (FileStream filebook = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(filebook);
                }

                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 1、Excel模版校验
                    if (Code.Common.GetFileType(file.FileName) != Code.FileType.Excel)
                    {
                        ModelState.AddModelError("", "上传的文件不是正确的EXCLE文件!");
                        return View(vm);
                    }
                    #endregion

                    #region 2.导入
                    var yearId = (from p in db.Table<Basis.Entity.tbYear>()
                                  where p.IsDefault == true
                                  select p.Id).FirstOrDefault();

                    var classBasis = (from p in db.Table<Basis.Entity.tbClass>()
                                      where p.tbYear.Id == yearId
                                      && p.tbGrade.IsDeleted == false
                                      select p).ToList();
                    var teacherBasis = (from p in db.Table<Teacher.Entity.tbTeacher>()
                                        select p).ToList();

                    var teachershelfvalue = (from p in db.Table<Study.Entity.tbStudyTimetable>()
                                             select p).ToList();

                    NPOI.SS.UserModel.ISheet sheet = hssfworkbook.GetSheetAt(0);
                    System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

                    IRow headerRow = sheet.GetRow(2);
                    int cellCount = headerRow.LastCellNum;
                    int rowCount = sheet.LastRowNum;
                    var lstClass = new List<string>();
                    for (int i = (sheet.FirstRowNum + 3); i <= rowCount; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row != null)
                        {
                            var className = row.GetCell(0).StringCellValue;
                            if (!string.IsNullOrEmpty(className))
                            {
                                lstClass.Add(className);
                            }
                        }
                    }
                    //判断班级
                    foreach (var t in lstClass)
                    {
                        if (!string.IsNullOrEmpty(t))
                        {
                            var tt = lstClass.Where(d => d == t).Count();
                            if (tt > decimal.One)
                            {
                                var remark = string.Format("该班级{0}重复（" + lstClass.Where(d => d == t).Count() + "次)", t);
                                ModelState.AddModelError("", remark);
                                return View(vm);
                            }
                        }
                        if (!string.IsNullOrEmpty(t))
                        {
                            var ClassName = (from p in classBasis
                                             where p.ClassName == t
                                             select p).FirstOrDefault();
                            if (ClassName == null)
                            {
                                var remark = string.Format("当前学年班级{0}不存在;", t);
                                ModelState.AddModelError("", remark);
                                return View(vm);
                            }
                        }
                    }

                    //保存
                    var addStudyList = new List<Study.Entity.tbStudyTimetable>();
                    for (int i = (headerRow.FirstCellNum + 1); i < cellCount; i++)
                    {
                        var date = GetCellValue(headerRow.GetCell(i));
                        var inputDate = Convert.ToDateTime(date);
                        for (int j = (sheet.FirstRowNum + 3); j <= rowCount; j++)
                        {
                            IRow row = sheet.GetRow(j);
                            if (row != null)
                            {
                                Study.Entity.tbStudyTimetable tb = null;

                                var className = row.GetCell(0).StringCellValue;
                                var teacherName = row.GetCell(i).StringCellValue.Trim();


                                if (!string.IsNullOrEmpty(teacherName))
                                {
                                    var teacher = (from p in teacherBasis
                                                   where p.TeacherName == teacherName
                                                   select p).FirstOrDefault();
                                    if (teacher == null)
                                    {
                                        var remark = string.Format("该教师{0}不存在;", teacherName);
                                        var model = new Dto.StudyTimetable.Import()
                                        {
                                            ClassName = className,
                                            TeacherName = teacherName,
                                            Error = remark
                                        };
                                        if (vm.ImportList.Where(d => d.TeacherName == model.TeacherName
                                                && d.ClassName == model.ClassName
                                                ).Count() == 0)
                                        {
                                            vm.ImportList.Add(model);
                                        }

                                    }
                                    //修改现有的数据
                                    tb = (from p in db.Table<Study.Entity.tbStudyTimetable>()
                                          where p.tbClass.IsDeleted == false && p.tbClass.ClassName == className
                                          && p.tbTeacher.IsDeleted == false
                                          && p.InputDate == inputDate
                                          select p).FirstOrDefault();
                                    if (tb != null)
                                    {
                                        tb.InputDate = inputDate;
                                        tb.tbClass = classBasis.Where(c => c.ClassName == className).Select(c => c).FirstOrDefault();
                                        tb.tbTeacher = teacherBasis.Where(c => c.TeacherName == teacherName).Select(c => c).FirstOrDefault();
                                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                                    }
                                    else
                                    {
                                        tb = new Study.Entity.tbStudyTimetable()
                                        {
                                            tbClass = classBasis.Where(c => c.ClassName == className).Select(c => c).FirstOrDefault(),
                                            tbTeacher = teacherBasis.Where(c => c.TeacherName == teacherName).Select(c => c).FirstOrDefault(),
                                            InputDate = inputDate,
                                            tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId)
                                        };
                                        addStudyList.Add(tb);
                                    }
                                }
                            }
                        }
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }

                    db.Set<Study.Entity.tbStudyTimetable>().AddRange(addStudyList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了晚修");
                        vm.Status = true;
                    }

                    #endregion
                }
            }
            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public ActionResult ExportAllowance(string dateSearchFrom)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();
                //当前激活学年学期学段
                var activeYear = (from y in db.Table<Basis.Entity.tbYear>()
                                  where y.IsDefault == true
                                  select y).FirstOrDefault();
                #region 数据统计
                var teacher = db.Table<Teacher.Entity.tbTeacher>();
                var deptList = new List<string>() { "普通老师组", "行政老师组" };
                var ids = db.Table<Teacher.Entity.tbTeacherWithDept>()
                    .Where(d => deptList.Contains(d.tbTeacherDept.TeacherDeptName))
                    .Select(d => d.tbTeacher.Id).ToList();
                var teacherWithDeptList = db.Table<Teacher.Entity.tbTeacherWithDept>()
                  .Where(d => ids.Contains(d.tbTeacher.Id))
                  .Select(d => new { d.tbTeacher.Id, d.tbTeacherDept.TeacherDeptName }).ToList();

                var deptTeacher = (from p in db.Table<Teacher.Entity.tbTeacher>()
                                   where ids.Contains(p.Id)
                                   select new
                                   {
                                       TeacherDeptName = string.Join(",", teacherWithDeptList.Where(d => d.Id == p.Id).Select(d => d.TeacherDeptName).Distinct().ToList()),
                                       p.TeacherCode,
                                       TeacherId = p.Id
                                   }).ToList();

                //班主任
                var classTeacher = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                    where p.tbClass.IsDeleted == false
                                    && p.tbTeacher.IsDeleted == false
                                    && p.tbClass.tbYear.Id == activeYear.Id
                                    select new
                                    {
                                        TeacherId = p.tbTeacher.Id,
                                        p.tbClass.ClassName
                                    }).Distinct().ToList();
                //年级组长
                var teacherGrade = (from p in db.Table<Teacher.Entity.tbTeacherGrade>()
                                    where p.tbGrade.IsDeleted == false
                                    && p.tbTeacher.IsDeleted == false
                                    select new
                                    {
                                        TeacherId = p.tbTeacher.Id,
                                        p.tbGrade.GradeName
                                    }).Distinct().ToList();

                //备课组长
                var teacherSubject = (from p in db.Table<Teacher.Entity.tbTeacherSubject>()
                                      where p.tbGrade.IsDeleted == false
                                      && p.tbTeacher.IsDeleted == false
                                      && p.tbSubject.IsDeleted == false
                                      select new
                                      {
                                          TeacherId = p.tbTeacher.Id,
                                          p.tbSubject.SubjectName
                                      }).Distinct().ToList();

                //任课班级教师
                var orgTeacherList = (from t in db.Table<Course.Entity.tbOrgTeacher>()
                                      where t.tbOrg.IsDeleted == false
                                      && t.tbOrg.tbYear.Id == activeYear.Id
                                      && t.tbOrg.tbGrade.IsDeleted == false
                                      select new
                                      {
                                          TeacherId = t.tbTeacher.Id,
                                          t.tbTeacher.TeacherCode,
                                          t.tbTeacher.TeacherName,
                                          t.tbOrg.tbGrade.GradeName,
                                          GradeNo = t.tbOrg.tbGrade.No,
                                          OrgId = t.tbOrg.Id,
                                          t.tbOrg.OrgName,
                                          OrgNo = t.tbOrg.No,
                                          t.tbOrg.tbCourse.tbSubject.SubjectName,
                                          SubjectId = t.tbOrg.tbCourse.tbSubject.Id
                                      }).Distinct().OrderBy(d => d.GradeNo).OrderBy(d => d.TeacherCode).OrderBy(d => d.OrgNo).ToList();

                var monthDate = Convert.ToDateTime(dateSearchFrom);
                //这个月最后一天
                DateTime lastDay = monthDate.AddMonths(1).AddDays(-monthDate.AddMonths(1).Day);

                var schoolCal = (from s in db.Table<Course.Entity.tbOrgSchedule>()
                                 where s.tbWeek.IsDeleted == false
                                 && s.tbPeriod.IsDeleted == false
                                 select new
                                 {
                                     OrgId = s.tbOrg.Id,
                                     WeekId = s.tbWeek.Id,
                                     WeekName = s.tbWeek.WeekName,
                                     WeeKNo = s.tbWeek.No,
                                     PeriodId = s.tbPeriod.Id,
                                 }).ToList();

                var ts = (from p in orgTeacherList
                          join s in schoolCal on p.OrgId equals s.OrgId
                          select new
                          {
                              p.TeacherId,
                              p.SubjectId,
                              p.OrgId,
                              s.WeekId,
                              s.WeekName,
                              s.WeeKNo,
                              s.PeriodId
                          }).Distinct().ToList();

                var dayCount = (lastDay.AddDays(1) - monthDate).Days;
                var ty = (from p in ts
                          select new
                          {
                              p.TeacherId,
                              p.SubjectId
                          }).Distinct().ToList();
                var lst = new List<Dto.StudyTimetable.List>();
                foreach (var s in ty)
                {
                    var periodCount = decimal.Zero;
                    for (int i = 1; i <= dayCount; i++)
                    {
                        var myDate = monthDate.AddDays(i - 1);
                        var weekNo = decimal.Zero;
                        switch (myDate.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                weekNo = 1;
                                break;
                            case DayOfWeek.Tuesday:
                                weekNo = 2;
                                break;
                            case DayOfWeek.Wednesday:
                                weekNo = 3;
                                break;
                            case DayOfWeek.Thursday:
                                weekNo = 4;
                                break;
                            case DayOfWeek.Friday:
                                weekNo = 5;
                                break;
                            case DayOfWeek.Saturday:
                                weekNo = 6;
                                break;
                            case DayOfWeek.Sunday:
                                weekNo = 7;
                                break;
                            default:
                                break;
                        }
                        periodCount += ts.Where(d => d.TeacherId == s.TeacherId && d.SubjectId == s.SubjectId && d.WeeKNo == weekNo).Count();
                    }
                    var model = new Dto.StudyTimetable.List()
                    {
                        SubjectId = s.SubjectId,
                        TeacherId = s.TeacherId,
                        PeriodCount = periodCount
                    };
                    lst.Add(model);
                }

                lst = lst.OrderByDescending(c => c.PeriodCount).ToList();

                var tf = (from p in orgTeacherList
                          select new
                          {
                              TeacherName = p.TeacherName,
                              p.TeacherId,
                              ClassTeacherName = classTeacher.Where(d => d.TeacherId == p.TeacherId).FirstOrDefault() != null ? classTeacher.Where(d => d.TeacherId == p.TeacherId).Select(d => d.ClassName).FirstOrDefault() + "班主任" : string.Empty,
                              GradeTeacherName = teacherGrade.Where(d => d.TeacherId == p.TeacherId).FirstOrDefault() != null ? "年级长" : string.Empty,
                              TeacherSubjectName = teacherSubject.Where(d => d.TeacherId == p.TeacherId).FirstOrDefault() != null ? "备课长" : string.Empty,
                              p.GradeName,
                              OrgName = string.Join(",", orgTeacherList.Where(d => d.TeacherId == p.TeacherId && d.SubjectId == p.SubjectId).Select(d => d.OrgName).ToArray()),
                              OrgCount = orgTeacherList.Where(d => d.TeacherId == p.TeacherId && d.SubjectId == p.SubjectId).Select(d => d).Count(),
                              p.SubjectName,
                              p.SubjectId,
                              PeriodCount = decimal.Round(lst.Where(d => d.TeacherId == p.TeacherId && d.SubjectId == p.SubjectId).Select(d => d.PeriodCount).FirstOrDefault() / 4, 0, MidpointRounding.AwayFromZero),//每周上课节数  一月四周计算
                              MaxPeriodCount = lst.Where(d => d.TeacherId == p.TeacherId && d.SubjectId == p.SubjectId).FirstOrDefault() != null ?
                              decimal.Round(lst.Where(d => d.TeacherId == p.TeacherId && d.SubjectId == p.SubjectId).Select(d => d.PeriodCount).FirstOrDefault() / 4, 0, MidpointRounding.AwayFromZero) : decimal.Zero//每周上课节数  一月四周计算
                          }).ToList();
                var tg = (from p in tf
                          select new
                          {
                              p.TeacherName,
                              p.TeacherId,
                              ParJobName = p.ClassTeacherName + " " + p.GradeTeacherName + " " + p.TeacherSubjectName,
                              p.GradeName,
                              p.SubjectName,
                              p.OrgName,
                              p.PeriodCount,
                              //TempCount = orgTeacherList.Where(d => d.TeacherId == p.TeacherId && d.SubjectId == p.SubjectId).Select(d => d.SubjectName).FirstOrDefault() == "数学" ?
                              //           (p.MaxPeriodCount - 2) : (p.MaxPeriodCount - 1)
                              TempCount = Math.Ceiling(p.PeriodCount / 4)
                          }).Distinct().ToList();

                var tbTmp = (from p in tg
                             select new
                             {
                                 p.TeacherName,
                                 p.TeacherId,
                                 p.ParJobName,
                                 p.GradeName,
                                 p.SubjectName,
                                 p.OrgName,
                                 p.PeriodCount,
                                 PeriodPreCount = p.TempCount > decimal.Zero ? p.TempCount : decimal.Zero
                             }).Distinct().ToList();

                var tm = (from p in tbTmp
                          select new
                          {
                              p.TeacherName,
                              p.ParJobName,
                              p.GradeName,
                              p.SubjectName,
                              p.OrgName,
                              p.PeriodCount,
                              PeriodPreCount = p.PeriodPreCount,
                              ClassHour = (p.PeriodCount + p.PeriodPreCount) * 4,
                              DeptName = deptTeacher.Where(d => d.TeacherId == p.TeacherId).Select(d => d.TeacherDeptName).FirstOrDefault()
                          }).ToList();

                var tb = (from p in tm
                          select new
                          {
                              TeacherName = p.TeacherName,
                              GradeName = p.GradeName,
                              ParJobName = p.ParJobName,
                              SubjectName = p.SubjectName,
                              OrgName = p.OrgName,
                              PeriodCount = p.PeriodCount,
                              PeriodPreCount = p.PeriodPreCount,
                              ClassHour = p.ClassHour,
                              TotalMoney = p.DeptName == "普通老师组" ? (p.ClassHour <= 44 ? p.ClassHour * 5 : ((p.ClassHour - 44) * 20 + 44 * 5)) : p.DeptName == "行政老师组" ? (p.ClassHour <= 20 ? p.ClassHour * 5 : ((p.ClassHour - 20) * 20 + 20 * 5)) : decimal.Zero
                          }).Distinct().ToList();
                #endregion

                #region 导出
                var optionList = new List<string>() { "兼职", "班级", "上课节数/周", "备课量/周", "课间操管理折算课时数", "月课时总数", "备注1", "备注2" };
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = hssfworkbook.CreateCellStyle();//设置垂直居中格式
                cellstyle.VerticalAlignment = VerticalAlignment.Center;//垂直居中
                cellstyle.Alignment = HorizontalAlignment.Center;//居中
                cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                cellstyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                cellstyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                cellstyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                //缩小字体填充  
                cellstyle.ShrinkToFit = false;

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("Sheet1") as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;
                IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                //表头
                ICell cell = cellHeader.CreateCell(0);
                cell.SetCellValue("姓名");
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex + 1, 0, 0);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);

                cell = cellHeader.CreateCell(1);
                cell.SetCellValue("年级");
                var cellRangeAddress1 = new CellRangeAddress(rowStartIndex, rowStartIndex + 1, 1, 1);
                setRegionStyle(sheet1, cellRangeAddress1, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress1);

                cell = cellHeader.CreateCell(2);
                cell.SetCellValue("任教学科");
                var cellRangeAddress2 = new CellRangeAddress(rowStartIndex, rowStartIndex + 1, 2, 2);
                setRegionStyle(sheet1, cellRangeAddress2, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress2);

                cell = cellHeader.CreateCell(3);
                cell.SetCellValue("基本工作量");
                var cellRangeAddress3 = new CellRangeAddress(rowStartIndex, rowStartIndex, 3, 10);
                setRegionStyle(sheet1, cellRangeAddress3, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress3);

                cell = cellHeader.CreateCell(11);
                cell.SetCellValue("月课时津贴");
                var cellRangeAddress4 = new CellRangeAddress(rowStartIndex, rowStartIndex + 1, 11, 11);
                setRegionStyle(sheet1, cellRangeAddress4, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress4);
                //第二行
                IRow cellHeaderR = sheet1.CreateRow(rowStartIndex + 1);
                var sn = -1;
                foreach (var option in optionList)
                {
                    ICell cell2 = cellHeaderR.CreateCell(4 + sn);
                    cell2.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(4 + sn, 15 * 256);
                    cell2.SetCellValue(option);
                    sn++;
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                setBorder(cellRangeAddress2, sheet1, hssfworkbook);
                setBorder(cellRangeAddress3, sheet1, hssfworkbook);
                setBorder(cellRangeAddress4, sheet1, hssfworkbook);
                foreach (var o in tb)
                {
                    //数据行
                    cellHeader = sheet1.CreateRow(rowStartIndex + 2);
                    cell = cellHeader.CreateCell(0);
                    cell.SetCellValue(o.TeacherName);

                    cell = cellHeader.CreateCell(1);
                    cell.SetCellValue(o.GradeName);

                    cell = cellHeader.CreateCell(2);
                    cell.SetCellValue(o.SubjectName);
                    rowStartIndex++;

                    var InNos = -1;
                    foreach (var option in optionList)
                    {
                        cell = cellHeader.CreateCell(4 + InNos);
                        InNos++;
                        var result = string.Empty;
                        switch (InNos)
                        {
                            case 0:
                                result = o.ParJobName;
                                break;
                            case 1:
                                result = o.OrgName;
                                break;
                            case 2:
                                result = o.PeriodCount.ToString();
                                break;
                            case 3:
                                result = o.PeriodPreCount.ToString();
                                break;
                            case 4:
                                result = string.Empty;
                                break;
                            case 5:
                                result = o.ClassHour.ToString();
                                break;
                            case 6:
                                result = string.Empty;
                                break;
                            case 7:
                                result = string.Empty;
                                break;
                            default:
                                break;
                        }
                        cell.SetCellValue(result);
                    }

                    cell = cellHeader.CreateCell(4 + InNos);
                    cell.SetCellValue(o.TotalMoney.ToString());
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode(activeYear.YearName + "每月工作量统计报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }
        #endregion

        public void setRegionStyle(HSSFSheet sheet, CellRangeAddress region, ICellStyle cs)
        {
            for (int i = region.FirstRow; i <= region.LastRow; i++)
            {
                IRow row = HSSFCellUtil.GetRow(i, sheet);
                for (int j = region.FirstColumn; j <= region.LastColumn; j++)
                {
                    ICell singleCell = HSSFCellUtil.GetCell(row, (short)j);
                    singleCell.CellStyle = cs;
                }
            }
        }
        public void setBorder(CellRangeAddress cellRangeAddress, HSSFSheet sheet, HSSFWorkbook wb)
        {
            RegionUtil.SetBorderLeft(1, cellRangeAddress, sheet, wb);
            RegionUtil.SetBorderBottom(1, cellRangeAddress, sheet, wb);
            RegionUtil.SetBorderRight(1, cellRangeAddress, sheet, wb);
            RegionUtil.SetBorderTop(1, cellRangeAddress, sheet, wb);
        }

        /// <summary>
        /// 根据Excel列类型获取列的值
        /// </summary>
        /// <param name="cell">Excel列</param>
        /// <returns></returns>
        public string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                    if (DateUtil.IsValidExcelDate(cell.NumericCellValue))
                    {
                        return cell.DateCellValue.ToString();
                    }
                    else
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Unknown:
                default:
                    return cell.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }
    }
}