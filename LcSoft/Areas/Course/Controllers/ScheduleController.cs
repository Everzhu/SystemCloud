using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace XkSystem.Areas.Course.Controllers
{
    public class ScheduleController : Controller
    {
        public ActionResult Class(int Id, int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.ClassAll();

                var baseYearId = db.Table<Basis.Entity.tbYear>().Include(d => d.tbYearParent.tbYearParent).Where(d => d.Id == yearId).Select(d => d.tbYearParent.tbYearParent.Id).FirstOrDefault();

                vm.ClassList = Basis.Controllers.ClassController.SelectList(baseYearId, 0, Id);
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
                vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId ?? 0).Where(x => x.ClassId == Id).ToList();

                return View(vm);
            }
        }

        public ActionResult ClassAll()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.ClassAll();

                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                var baseYearId = db.Table<Basis.Entity.tbYear>().Include(d => d.tbYearParent.tbYearParent).Where(d => d.Id == vm.YearId)
                    .Select(d => d.tbYearParent.tbYearParent.Id).FirstOrDefault();

                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
                vm.ClassList = Basis.Controllers.ClassController.SelectList(baseYearId, 0, 0, vm.SearchText);
                vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId ?? 0);

                return View(vm);
            }
        }

        public ActionResult ClassPrint()
        {
            var vm = new Models.Schedule.ClassPrint();
            vm.WeekList = Basis.Controllers.WeekController.SelectList();
            vm.ClassList = Basis.Controllers.ClassController.SelectList(vm.YearId, 0, 0, vm.SearchText);
            vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
            vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId);

            return View(vm);
        }

        public ActionResult ClassExport()
        {
            var file = System.IO.Path.GetTempFileName();
            var classExport = new Models.Schedule.ClassExport();

            var ClassList = Basis.Controllers.ClassController.SelectList();
            var WeekList = Basis.Controllers.WeekController.SelectList();
            var PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
            var OrgScheduleList = OrgScheduleController.GetAll(Convert.ToInt32(classExport.year));

            string[] arrColumns = new string[WeekList.Count * PeriodList.Count + 1];
            for (int i = 0; i < arrColumns.Length; i++)
            {
                arrColumns[i] = (i + 1).ToString();
            }

            List<NPOI.SS.Util.CellRangeAddress> regions = new List<NPOI.SS.Util.CellRangeAddress>();
            DataTable dt = Code.Common.ArrayToDataTable(arrColumns);

            string[] weekArr = new string[WeekList.Count * PeriodList.Count + 1];
            string[] weekIdArr = new string[WeekList.Count * PeriodList.Count + 1];
            string[] periodArr = new string[WeekList.Count * PeriodList.Count + 1];
            string[] periodIdArr = new string[WeekList.Count * PeriodList.Count + 1];
            for (int i = 0; i < WeekList.Count; i++)
            {
                for (int j = 0; j < PeriodList.Count; j++)
                {
                    weekArr[i * PeriodList.Count + j + 1] = WeekList[i].Text;
                    weekIdArr[i * PeriodList.Count + j + 1] = WeekList[i].Value;
                    periodArr[i * PeriodList.Count + j + 1] = PeriodList[j].Text;
                    periodIdArr[i * PeriodList.Count + j + 1] = PeriodList[j].Value;
                }

                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, i * PeriodList.Count + 1, (i + 1) * PeriodList.Count));
            }
            dt.Rows.Add(weekIdArr);
            dt.Rows.Add(periodIdArr);
            dt.Rows.Add(weekArr);
            dt.Rows.Add(periodArr);

            for (int i = 0; i < ClassList.Count; i++)
            {
                string[] tempArr = new string[WeekList.Count * PeriodList.Count + 1];
                tempArr[0] = ClassList[i].Text;

                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    var orgSchedule = OrgScheduleList.Where(d => d.ClassId == Convert.ToInt32(ClassList[i].Value)
                          && d.WeekId == Convert.ToInt32(dt.Rows[0][j])
                          && d.PeriodId == Convert.ToInt32(dt.Rows[1][j])).FirstOrDefault();

                    if (orgSchedule != null)
                    {
                        tempArr[j] = orgSchedule.Subject;
                        var orgTeacher = OrgTeacherController.GetTeacherByOrgId(orgSchedule.OrgId);
                        if (orgTeacher != null)
                        {
                            tempArr[j] += "\n" + orgTeacher.TeacherName;
                        }
                    }
                }
                dt.Rows.Add(tempArr);
            }
            dt.Rows.RemoveAt(1);
            dt.Rows.RemoveAt(0);

            dt.Rows[0][0] = "班级名称";
            regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));

            Code.NpoiHelper.DataTableToExcel(file, dt, false, regions, "班级课表");

            if (string.IsNullOrEmpty(file) == false)
            {
                return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassAll(XkSystem.Areas.Course.Models.Schedule.ClassAll vm)
        {
            var error = new List<string>();

            return Code.MvcHelper.Post(error, Url.Action("ClassAll", new { searchText = vm.SearchText, yearId = vm.YearId, }));
        }

        public ActionResult Org()
        {
            return View();
        }

        public ActionResult Room(int id, int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.ClassAll();

                var baseYearId = db.Table<Basis.Entity.tbYear>().Include(d => d.tbYearParent.tbYearParent).Where(d => d.Id == yearId)
                .Select(d => d.tbYearParent.tbYearParent.Id).FirstOrDefault();

                vm.RoomList = Basis.Controllers.RoomController.SelectList(id);
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
                vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId ?? 0);

                return View(vm);
            }
        }

        public ActionResult RoomAll()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.ClassAll();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                var baseYearId = db.Table<Basis.Entity.tbYear>().Include(d => d.tbYearParent.tbYearParent).Where(d => d.Id == vm.YearId)
                  .Select(d => d.tbYearParent.tbYearParent.Id).FirstOrDefault();

                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
                vm.RoomList = Basis.Controllers.RoomController.SelectList();
                vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId ?? 0);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoomAll(XkSystem.Areas.Course.Models.Schedule.ClassAll vm)
        {
            var error = new List<string>();

            return Code.MvcHelper.Post(error, Url.Action("RoomAll", new { searchText = vm.SearchText, yearId = vm.YearId, }));
        }

        public ActionResult RoomExport()
        {
            var file = System.IO.Path.GetTempFileName();
            var classExport = new Models.Schedule.ClassExport();

            var RoomList = Basis.Controllers.RoomController.SelectList();
            var WeekList = Basis.Controllers.WeekController.SelectList();
            var PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
            var OrgScheduleList = OrgScheduleController.GetAll(Convert.ToInt32(classExport.year));

            string[] arrColumns = new string[WeekList.Count * PeriodList.Count + 1];
            for (int i = 0; i < arrColumns.Length; i++)
            {
                arrColumns[i] = (i + 1).ToString();
            }

            List<NPOI.SS.Util.CellRangeAddress> regions = new List<NPOI.SS.Util.CellRangeAddress>();
            DataTable dt = Code.Common.ArrayToDataTable(arrColumns);

            string[] weekArr = new string[WeekList.Count * PeriodList.Count + 1];
            string[] weekIdArr = new string[WeekList.Count * PeriodList.Count + 1];
            string[] periodArr = new string[WeekList.Count * PeriodList.Count + 1];
            string[] periodIdArr = new string[WeekList.Count * PeriodList.Count + 1];
            for (int i = 0; i < WeekList.Count; i++)
            {
                for (int j = 0; j < PeriodList.Count; j++)
                {
                    weekArr[i * PeriodList.Count + j + 1] = WeekList[i].Text;
                    weekIdArr[i * PeriodList.Count + j + 1] = WeekList[i].Value;
                    periodArr[i * PeriodList.Count + j + 1] = PeriodList[j].Text;
                    periodIdArr[i * PeriodList.Count + j + 1] = PeriodList[j].Value;
                }
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, i * PeriodList.Count + 1, (i + 1) * PeriodList.Count));
            }
            dt.Rows.Add(weekIdArr);
            dt.Rows.Add(periodIdArr);
            dt.Rows.Add(weekArr);
            dt.Rows.Add(periodArr);

            for (int i = 0; i < RoomList.Count; i++)
            {
                string[] tempArr = new string[WeekList.Count * PeriodList.Count + 1];
                tempArr[0] = RoomList[i].Text;

                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    var orgSchedule = OrgScheduleList.Where(d => d.RoomId == Convert.ToInt32(RoomList[i].Value)
                         && d.WeekId == Convert.ToInt32(dt.Rows[0][j])
                         && d.PeriodId == Convert.ToInt32(dt.Rows[1][j])).FirstOrDefault();

                    if (orgSchedule != null)
                    {
                        tempArr[j] = orgSchedule.Subject;
                        var orgTeacher = OrgTeacherController.GetTeacherByOrgId(orgSchedule.OrgId);
                        if (orgTeacher != null)
                        {
                            tempArr[j] += "\n" + orgTeacher.TeacherName;
                        }
                    }
                }
                dt.Rows.Add(tempArr);
            }
            dt.Rows.RemoveAt(1);
            dt.Rows.RemoveAt(0);

            dt.Rows[0][0] = "教室名称";
            regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));

            Code.NpoiHelper.DataTableToExcel(file, dt, false, regions, "教室课表");

            if (string.IsNullOrEmpty(file) == false)
            {
                return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
            }
            else
            {
                return View();
            }
        }

        public ActionResult RoomExportWord(int yearId)
        {
            var roomList = Basis.Controllers.RoomController.SelectList();
            var weekList = Basis.Controllers.WeekController.SelectList();
            var periodList = Basis.Controllers.PeriodController.SelectScheduleList();
            var orgScheduleList = OrgScheduleController.GetAll(yearId);
            var year = Areas.Basis.Controllers.YearController.SelectInfo(yearId);
            string strHeaderText = String.Format("{0}教室课程表", year.YearName);
            string[] arrColumns = new string[weekList.Count + 1];
            for (int i = 0; i < arrColumns.Length; i++)
            {
                arrColumns[i] = (i + 1).ToString();
            }

            List<Models.Schedule.ScheduleExportWord> scheduleList = new List<Models.Schedule.ScheduleExportWord>();

            foreach (var room in roomList)
            {
                Models.Schedule.ScheduleExportWord schedule = new Models.Schedule.ScheduleExportWord();
                schedule.HeaderText = strHeaderText;
                schedule.TableCaption = room.Text;

                DataTable dt = Code.Common.ArrayToDataTable(arrColumns);

                string[] arrWeek = new string[weekList.Count + 1];
                for (int i = 0; i < weekList.Count; i++)
                {
                    arrWeek[i + 1] = weekList[i].Text;
                }
                dt.Rows.Add(arrWeek);

                foreach (var period in periodList)
                {
                    string[] arrSchedule = new string[weekList.Count + 1];
                    arrSchedule[0] = period.Text;

                    for (int i = 0; i < weekList.Count; i++)
                    {
                        var orgSchedule = orgScheduleList.Where(d => d.RoomId == Convert.ToInt32(room.Value)
                          && d.WeekId == Convert.ToInt32(weekList[i].Value)
                          && d.PeriodId == Convert.ToInt32(period.Value)).FirstOrDefault();
                        if (orgSchedule != null)
                        {
                            arrSchedule[i + 1] = orgSchedule.CourseName;
                            var orgTeacher = OrgTeacherController.GetTeacherByOrgId(orgSchedule.OrgId);
                            if (orgTeacher != null)
                            {
                                arrSchedule[i + 1] += "|" + orgTeacher.TeacherName;
                            }
                        }
                    }

                    dt.Rows.Add(arrSchedule);
                }

                schedule.ScheduleList = dt;

                scheduleList.Add(schedule);
            }

            var file = System.IO.Path.GetTempFileName();
            Code.DocxHelper.ScheduleToWord(file, scheduleList);

            return File(file, Code.Common.DownloadType, Code.Common.ExportByWord);
        }

        public ActionResult Student(int Id, int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.ClassAll();

                var baseYearId = db.Table<Basis.Entity.tbYear>().Include(d => d.tbYearParent.tbYearParent).Where(d => d.Id == yearId)
                     .Select(d => d.tbYearParent.tbYearParent.Id).FirstOrDefault();

                vm.StudentInfoList = Areas.Student.Controllers.StudentController.GetStudentById(Id);
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
                vm.OrgScheduleList = OrgScheduleController.GetStudentAll(Id, vm.YearId ?? 0).ToList();

                return View(vm);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentAll()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.ClassAll();

                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                var baseYearId = db.Table<Basis.Entity.tbYear>().Include(d => d.tbYearParent.tbYearParent).Where(d => d.Id == vm.YearId)
                    .Select(d => d.tbYearParent.tbYearParent.Id).FirstOrDefault();

                //vm.WeekList = Basis.Controllers.WeekController.SelectList();
                //vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
                vm.ClassInfoList = Areas.Basis.Controllers.ClassController.SelectInfoList(Convert.ToInt32(baseYearId));
                if (vm.ClassId == 0 && vm.ClassInfoList.Count > 0)
                {
                    vm.ClassId = vm.ClassInfoList.FirstOrDefault().Id;
                }
                vm.StudentInfoList = Areas.Student.Controllers.StudentController.SelectInfoList(vm.ClassId ?? 0, vm.SearchText);
                //vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId ?? 0);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentAll(XkSystem.Areas.Course.Models.Schedule.ClassAll vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StudentAll", new { searchText = vm.SearchText, yearId = vm.YearId, classId = vm.ClassId }));
        }

        public ActionResult Teacher(int Id, int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.ClassAll();

                var baseYearId = db.Table<Basis.Entity.tbYear>().Include(d => d.tbYearParent.tbYearParent).Where(d => d.Id == yearId)
                    .Select(d => d.tbYearParent.tbYearParent.Id).FirstOrDefault();

                vm.TeacherList = XkSystem.Areas.Teacher.Controllers.TeacherController.SelectList("", Id);
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
                vm.OrgScheduleList = OrgScheduleController.GetTeacherAll(Id, vm.YearId ?? 0).ToList();

                return View(vm);
            }
        }

        public ActionResult TeacherAll()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.ClassAll();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                var baseYearId = db.Table<Basis.Entity.tbYear>().Include(d => d.tbYearParent.tbYearParent).Where(d => d.Id == vm.YearId)
                   .Select(d => d.tbYearParent.tbYearParent.Id).FirstOrDefault();

                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
                vm.TeacherInfoList = Areas.Teacher.Controllers.TeacherController.SelectInfoList(vm.SearchText);
                vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId ?? 0);

                return View(vm);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeacherAll(XkSystem.Areas.Course.Models.Schedule.ClassAll vm)
        {
            var error = new List<string>();

            return Code.MvcHelper.Post(error, Url.Action("TeacherAll", new { searchText = vm.SearchText, yearId = vm.YearId }));
        }

        public ActionResult Set()
        {
            var vm = new Models.Schedule.Set();
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
            if (vm.YearList.Count > 0 && vm.YearId == 0)
            {
                vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
            }

            var yearId = 0;
            using (var db = new XkSystem.Models.DbContext())
            {
                yearId = (from p in db.Table<Basis.Entity.tbYear>()
                          where p.Id == vm.YearId
                          select p.tbYearParent.tbYearParent.Id).FirstOrDefault();
            }

            vm.WeekList = Basis.Controllers.WeekController.SelectList();
            vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
            vm.ClassList = Basis.Controllers.ClassController.SelectList(yearId, 0, 0, vm.SearchText);
            vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Set(Models.Schedule.Set vm)
        {
            var error = new List<string>();

            return Code.MvcHelper.Post(error, Url.Action("Set", new { searchText = vm.SearchText, yearId = vm.YearId, }));
        }

        public ActionResult SetImport()
        {
            var vm = new Models.Schedule.SetImport();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetImport(Models.Schedule.SetImport vm)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

                if (Code.Common.GetFileType(file.FileName) != Code.FileType.Excel)
                {
                    ModelState.AddModelError("", "上传的文件不是正确的EXCLE文件!");
                    return View(vm);
                }

                int[] headerRowNums = new int[] { 0, 1 };
                var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), String.Empty, headerRowNums);
                if (dt == null)
                {
                    ModelState.AddModelError("", "无法读取上传的文件，请检查文件格式是否正确!");
                    return View(vm);
                }

                #region 校验导入的Excel数据模板格式
                var weekList = Basis.Controllers.WeekController.SelectList();
                var periodList = Basis.Controllers.PeriodController.SelectScheduleList();
                var tbList = new List<string>() { "班级名称" };
                weekList.ForEach(week =>
                {
                    periodList.ForEach(period =>
                    {
                        tbList.Add(week.Text + period.Text);
                    });
                });

                StringBuilder strNotExistsColumnName = new StringBuilder();
                tbList.ForEach(columnName =>
                {
                    if (!dt.Columns.Contains(columnName))
                    {
                        strNotExistsColumnName.AppendFormat("{0},", columnName);
                    }
                });

                if (strNotExistsColumnName.Length > 0)
                {
                    ModelState.AddModelError("", "上传的EXCEL内容与预期不一致，缺少对应的字段：" + strNotExistsColumnName.Remove(strNotExistsColumnName.Length - 1, 1).ToString());
                    return View(vm);
                }
                #endregion

                #region 将DataTable转为List
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    if (String.IsNullOrWhiteSpace(dr["班级名称"].ToString()))
                    {
                        continue;
                    }

                    var import = new Dto.Schedule.SetImport
                    {
                        ClassName = dr["班级名称"].ToString()
                    };

                    weekList.ForEach(week =>
                    {
                        periodList.ForEach(period =>
                        {
                            string cellValue = dr[week.Text + period.Text].ToString();
                            string courseName = String.Empty;
                            string teacherName = String.Empty;
                            if (!String.IsNullOrWhiteSpace(cellValue))
                            {
                                string[] arrCourseTeacher = cellValue.Replace("\n", "|").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                courseName = arrCourseTeacher[0];
                                if (arrCourseTeacher.Length > 1)
                                {
                                    teacherName = arrCourseTeacher[1];
                                }
                            }

                            var importItem = new Dto.Schedule.SetImportItem
                            {
                                WeekId = week.Value.ConvertToInt(),
                                WeekName = week.Text,
                                PeriodId = period.Value.ConvertToInt(),
                                PeriodName = period.Text,
                                CourseName = courseName,
                                TeacherName = teacherName
                            };

                            import.SetImportItemList.Add(importItem);
                        });
                    });

                    vm.SetImportList.Add(import);
                }
                #endregion

                using (var db = new XkSystem.Models.DbContext())
                {
                    var baseYearId = db.Table<Basis.Entity.tbYear>().Include(d => d.tbYearParent.tbYearParent).Where(d => d.Id == vm.YearId).Select(d => d.tbYearParent.tbYearParent.Id).FirstOrDefault();
                    #region 数据校验
                    var classList = db.Table<Basis.Entity.tbClass>().Where(d => d.tbYear.Id == baseYearId).ToList();
                    var courseList = Areas.Course.Controllers.CourseController.SelectList();
                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();

                    List<string> arrClassName = new List<string>();
                    List<string> arrDuplicateClassName = new List<string>();
                    List<string> arrNotExistsClassName = new List<string>();
                    List<string> arrNotExistsCourseName = new List<string>();
                    List<string> arrNotExistsTeacherName = new List<string>();

                    vm.SetImportList.ForEach(import =>
                    {
                        if (!arrClassName.Contains(import.ClassName))
                        {
                            arrClassName.Add(import.ClassName);

                            if (!classList.Select(t => t.ClassName).Contains(import.ClassName))
                            {
                                arrNotExistsClassName.Add(import.ClassName);
                            }

                            import.SetImportItemList.ForEach(importItem =>
                            {
                                if (!String.IsNullOrWhiteSpace(importItem.CourseName)
                                    && !courseList.Select(t => t.Text).Contains(importItem.CourseName))
                                {
                                    arrNotExistsCourseName.Add(importItem.CourseName);
                                }
                                if (!String.IsNullOrWhiteSpace(importItem.TeacherName)
                                    && !teacherList.Where(d => d.TeacherName == importItem.TeacherName).Any())
                                {
                                    arrNotExistsTeacherName.Add(importItem.TeacherName);
                                }
                            });
                        }
                        else
                        {
                            arrDuplicateClassName.Add(import.ClassName);
                        }
                    });

                    StringBuilder strErrorMessage = new StringBuilder();
                    if (arrDuplicateClassName.Count > 0)
                    {
                        arrDuplicateClassName.ForEach(className =>
                        {
                            vm.ErrorMessageList.Add(String.Format("Excel中重复的班级名称：{0}", className));
                        });
                    }
                    if (arrNotExistsClassName.Count > 0)
                    {
                        arrNotExistsClassName.ForEach(className =>
                        {
                            vm.ErrorMessageList.Add(String.Format("Excel中不存在的班级名称：{0}", className));
                        });
                    }
                    if (arrNotExistsCourseName.Count > 0)
                    {
                        arrNotExistsCourseName.ForEach(courseName =>
                        {
                            vm.ErrorMessageList.Add(String.Format("Excel中不存在的课程名称：{0}", courseName));
                        });
                    }
                    if (arrNotExistsTeacherName.Count > 0)
                    {
                        arrNotExistsTeacherName.ForEach(teacherName =>
                        {
                            vm.ErrorMessageList.Add(String.Format("Excel中不存在的教师姓名：{0}", teacherName));
                        });
                    }

                    if (vm.ErrorMessageList.Count > 0)
                    {
                        return View(vm);
                    }

                    #endregion

                    #region 数据导入
                    var year = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                    var dbTeacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();
                    var dbWeek = db.Table<Basis.Entity.tbWeek>().ToList();
                    var dbPeriod = db.Table<Basis.Entity.tbPeriod>().ToList();
                    var orgList = db.Table<Course.Entity.tbOrg>().Where(d => d.tbYear.Id == vm.YearId).ToList();

                    foreach (var import in vm.SetImportList)
                    {
                        var cla = classList.Where(t => t.ClassName == import.ClassName).FirstOrDefault();

                        foreach (var importItem in import.SetImportItemList)
                        {
                            if (String.IsNullOrWhiteSpace(importItem.CourseName))
                            {
                                continue;
                            }

                            var course = courseList.Where(t => t.Text == importItem.CourseName).FirstOrDefault();

                            string strOrgName = String.Format("{0}-{1}", course.Text, cla.No);
                            var org = (from p in orgList
                                       where p.OrgName == strOrgName
                                       orderby p.No
                                       select p).FirstOrDefault();
                            if (orgList.Where(d => d.OrgName == strOrgName).Any() == false)
                            {
                                var classRecord = (from d in db.Table<Basis.Entity.tbClass>()
                                                    .Include(d => d.tbGrade)
                                                   where d.Id == cla.Id
                                                   select d).FirstOrDefault();
                                org = new Course.Entity.tbOrg();
                                org.OrgName = String.Format("{0}-{1}", course.Text, cla.No);
                                org.IsClass = true;
                                org.No = cla.No;
                                org.tbClass = classRecord;
                                org.tbCourse = db.Set<Course.Entity.tbCourse>().Find(course.Value.ConvertToInt());
                                org.tbGrade = classRecord.tbGrade;
                                org.tbYear = year;
                                db.Set<Course.Entity.tbOrg>().Add(org);
                                orgList.Add(org);
                                db.SaveChanges();
                            }

                            if (!String.IsNullOrWhiteSpace(importItem.TeacherName))
                            {
                                var teacher = teacherList.Where(t => t.TeacherName == importItem.TeacherName).FirstOrDefault();
                                var orgTeacher = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                                  where p.tbOrg.Id == org.Id && p.tbTeacher.Id == teacher.Id
                                                  orderby p.No
                                                  select p).FirstOrDefault();
                                if (orgTeacher == null)
                                {
                                    orgTeacher = new Course.Entity.tbOrgTeacher();
                                    orgTeacher.tbOrg = org;
                                    orgTeacher.tbTeacher = dbTeacherList.Where(t => t.Id == teacher.Id).FirstOrDefault();
                                    db.Set<Course.Entity.tbOrgTeacher>().Add(orgTeacher);
                                }
                                else
                                {
                                    if (vm.IsUpdate)
                                    {
                                        orgTeacher.tbOrg = org;
                                        orgTeacher.tbTeacher = dbTeacherList.Where(t => t.Id == teacher.Id).FirstOrDefault();
                                    }
                                }
                                db.SaveChanges();
                            }

                            var orgSchedule = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                                               where p.tbOrg.Id == org.Id && p.tbWeek.Id == importItem.WeekId && p.tbPeriod.Id == importItem.PeriodId
                                               orderby p.No
                                               select p).FirstOrDefault();
                            if (orgSchedule == null)
                            {
                                orgSchedule = new Course.Entity.tbOrgSchedule();
                                orgSchedule.tbOrg = org;
                                orgSchedule.tbWeek = dbWeek.Where(t => t.Id == importItem.WeekId).FirstOrDefault();
                                orgSchedule.tbPeriod = dbPeriod.Where(t => t.Id == importItem.PeriodId).FirstOrDefault();
                                db.Set<Course.Entity.tbOrgSchedule>().Add(orgSchedule);
                            }
                            else
                            {
                                if (vm.IsUpdate)
                                {
                                    orgSchedule.tbOrg = org;
                                    orgSchedule.tbWeek = dbWeek.Where(t => t.Id == importItem.WeekId).FirstOrDefault();
                                    orgSchedule.tbPeriod = dbPeriod.Where(t => t.Id == importItem.PeriodId).FirstOrDefault();
                                }
                            }
                            db.SaveChanges();
                        }
                    }

                    db.SaveChanges();

                    vm.Status = true;
                }
                #endregion
            }

            return View(vm);
        }

        public ActionResult SetImportTemplate()
        {
            var file = System.IO.Path.GetTempFileName();
            var weekList = Basis.Controllers.WeekController.SelectList();
            var periodList = Basis.Controllers.PeriodController.SelectScheduleList();
            string[] arrColumns = new string[weekList.Count * periodList.Count + 1];
            for (int i = 0; i < arrColumns.Length; i++)
            {
                arrColumns[i] = (i + 1).ToString();
            }

            List<NPOI.SS.Util.CellRangeAddress> regions = new List<NPOI.SS.Util.CellRangeAddress>();
            DataTable dt = Code.Common.ArrayToDataTable(arrColumns);
            string[] arrWeek = new string[weekList.Count * periodList.Count + 1];
            string[] arrPeriod = new string[weekList.Count * periodList.Count + 1];
            for (int i = 0; i < weekList.Count; i++)
            {
                for (int j = 0; j < periodList.Count; j++)
                {
                    arrWeek[i * periodList.Count + j + 1] = weekList[i].Text;
                    arrPeriod[i * periodList.Count + j + 1] = periodList[j].Text;
                }

                regions.Add(new NPOI.SS.Util.CellRangeAddress(0, 0, i * periodList.Count + 1, (i + 1) * periodList.Count));
            }
            dt.Rows.Add(arrWeek);
            dt.Rows.Add(arrPeriod);

            string[] arrEmptyData = new string[weekList.Count * periodList.Count + 1];
            dt.Rows.Add(arrEmptyData);

            dt.Rows[0][0] = "班级名称";
            regions.Add(new NPOI.SS.Util.CellRangeAddress(0, 1, 0, 0));

            dt.Rows[2][0] = "一年级（1）班";
            dt.Rows[2][1] = "英语\n吴平";

            Code.NpoiHelper.DataTableToExcel(file, dt, false, regions, "班级课表", false);

            return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
        }

        public ActionResult ClassScheduleImport()
        {
            var vm = new Models.Schedule.ClassScheduleImport();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassScheduleImport(Models.Schedule.ClassScheduleImport vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (ModelState.IsValid)
                {
                    var file = Request.Files[nameof(vm.UploadFile)];
                    var fileSave = System.IO.Path.GetTempFileName();
                    file.SaveAs(fileSave);

                    if (Code.Common.GetFileType(file.FileName) != Code.FileType.Excel)
                    {
                        ModelState.AddModelError("", "上传的文件不是正确的EXCLE文件!");
                        return View(vm);
                    }

                    var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), 1);

                    #region 数据转换DataTable为List

                    List<Dto.Schedule.Class> classDtoList = new List<Dto.Schedule.Class>();
                    Dto.Schedule.Class classDto = new Dto.Schedule.Class();
                    DataRow drWeek = dt.NewRow();
                    int index = -1;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        if (dr[0].ToString().Contains("课程表"))
                        {
                            if (!String.IsNullOrWhiteSpace(classDto.ClassName))
                            {
                                classDto = new Dto.Schedule.Class();
                            }

                            classDto.ClassName = dr[0].ToString().Replace("课程表", "").Trim();
                            classDtoList.Add(classDto);
                            index = i;
                        }
                        else
                        {
                            if (index != -1 && i == index + 1)
                            {
                                drWeek = dr;
                            }
                            else
                            {
                                if (!String.IsNullOrWhiteSpace(dr[1].ToString()))
                                {
                                    for (int j = 1; j < dt.Columns.Count; j++)
                                    {
                                        string[] arrCourseTeacher = dr[j].ToString().Trim().Replace("\n", "|").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                        string[] arrCourse = arrCourseTeacher[0].Trim().Replace("／", "|").Replace("/", "|").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (arrCourse.Count() > 1)
                                        {
                                            for (int k = 0; k < arrCourse.Count(); k++)
                                            {
                                                Dto.Schedule.ClassSchedule classScheduleDto = new Dto.Schedule.ClassSchedule();
                                                classScheduleDto.CourseName = arrCourse[k].Trim();
                                                classScheduleDto.ScheduleType = k % 2 == 0
                                                    ? Code.EnumHelper.CourseScheduleType.Odd
                                                    : Code.EnumHelper.CourseScheduleType.Dual;
                                                classScheduleDto.PeriodName = dr[0].ToString().Trim();
                                                classScheduleDto.WeekName = drWeek[j].ToString().Trim();

                                                if (arrCourseTeacher.Length > 1)
                                                {
                                                    classScheduleDto.TeacherName = arrCourseTeacher[1].Trim();
                                                }

                                                classDto.ClassScheduleList.Add(classScheduleDto);
                                            }
                                        }
                                        else
                                        {
                                            Dto.Schedule.ClassSchedule classScheduleDto = new Dto.Schedule.ClassSchedule();
                                            classScheduleDto.CourseName = arrCourseTeacher[0].Trim();
                                            classScheduleDto.PeriodName = dr[0].ToString().Trim();
                                            classScheduleDto.WeekName = drWeek[j].ToString().Trim();

                                            if (arrCourseTeacher.Length > 1)
                                            {
                                                classScheduleDto.TeacherName = arrCourseTeacher[1].Trim();
                                            }

                                            classDto.ClassScheduleList.Add(classScheduleDto);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    int classDtoListCount = classDtoList.Count();
                    foreach (var dto in classDtoList)
                    {
                        int dtoCount = dto.ClassScheduleList.Count();
                    }

                    #endregion

                    var yearId = (from p in db.Table<Basis.Entity.tbYear>()//.Find(vm.YearId);
                                  where p.Id == vm.YearId
                                  select new
                                  {
                                      p.Id,
                                      YearTopId = p.tbYearParent.tbYearParent.Id
                                  }).FirstOrDefault();
                    var year = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                    var weekList = db.Table<Basis.Entity.tbWeek>().ToList();
                    var periodList = db.Table<Basis.Entity.tbPeriod>().ToList();
                    var classList = (from d in db.Table<Basis.Entity.tbClass>()
                                            .Include(d => d.tbGrade)
                                     where d.tbYear.Id == yearId.YearTopId
                                     select d).ToList();
                    var courseList = db.Table<Course.Entity.tbCourse>().ToList();
                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();

                    #region 数据校验

                    // 校验Excel中重复班级
                    var source = classDtoList.Select(t => new { ClassName = t.ClassName });
                    var expr = from t in source
                               group t by t.ClassName into g
                               where g.Count() >= 2
                               select g.Key;
                    if (expr.Count() > 0)
                    {
                        vm.ErrorMessageList.Add(String.Format("Excel中重复的行政班级：{0}", String.Join(",", expr)));
                    }

                    foreach (var classDtoItem in classDtoList)
                    {
                        #region 班级
                        var classItem = classList.Where(t => t.ClassName == classDtoItem.ClassName).FirstOrDefault();
                        if (classItem != null)
                        {
                            classDtoItem.ClassId = classItem.Id;
                            classDtoItem.ClassNo = classItem.No;
                        }
                        else
                        {
                            vm.ErrorMessageList.Add(String.Format("Excel中不存在的行政班级：{0}", classDtoItem.ClassName));
                        }
                        #endregion

                        foreach (var classScheduleDtoItem in classDtoItem.ClassScheduleList)
                        {
                            #region 星期
                            var week = weekList.Where(t => t.WeekName == classScheduleDtoItem.WeekName).FirstOrDefault();
                            if (week != null)
                            {
                                classScheduleDtoItem.WeekId = week.Id;
                            }
                            else
                            {
                                string strWeekErrorMessage = String.Format("Excel中不存在的星期：{0}", classScheduleDtoItem.WeekName);
                                if (!vm.ErrorMessageList.Contains(strWeekErrorMessage))
                                {
                                    vm.ErrorMessageList.Add(strWeekErrorMessage);
                                }
                            }
                            #endregion

                            #region 节次
                            var period = periodList.Where(t => t.PeriodName == classScheduleDtoItem.PeriodName).FirstOrDefault();
                            if (period != null)
                            {
                                classScheduleDtoItem.PeriodId = period.Id;
                            }
                            else
                            {
                                string strPeriodErrorMessage = String.Format("Excel中不存在的节次：{0}", classScheduleDtoItem.PeriodName);
                                if (!vm.ErrorMessageList.Contains(strPeriodErrorMessage))
                                {
                                    vm.ErrorMessageList.Add(strPeriodErrorMessage);
                                }
                            }
                            #endregion

                            #region 教师
                            if (!String.IsNullOrWhiteSpace(classScheduleDtoItem.TeacherName))
                            {
                                var teacher = teacherList.Where(t => t.TeacherName == classScheduleDtoItem.TeacherName).FirstOrDefault();
                                if (teacher != null)
                                {
                                    classScheduleDtoItem.TeacherId = teacher.Id;
                                }
                                else
                                {
                                    string strTeacherErrorMessage = String.Format("Excel中不存在的教师：{0}", classScheduleDtoItem.TeacherName);
                                    if (!vm.ErrorMessageList.Contains(strTeacherErrorMessage))
                                    {
                                        vm.ErrorMessageList.Add(strTeacherErrorMessage);
                                    }
                                }
                            }
                            #endregion

                            #region 课程
                            var course = courseList.Where(t => t.CourseName == classScheduleDtoItem.CourseName).FirstOrDefault();
                            if (course != null)
                            {
                                classScheduleDtoItem.CourseId = course.Id;
                            }
                            else
                            {
                                string strCourseErrorMessage = String.Format("Excel中不存在的课程：{0}", classScheduleDtoItem.CourseName);
                                if (!vm.ErrorMessageList.Contains(strCourseErrorMessage))
                                {
                                    vm.ErrorMessageList.Add(strCourseErrorMessage);
                                }
                            }
                            #endregion
                        }
                    }

                    if (vm.ErrorMessageList.Count > 0)
                    {
                        return View(vm);
                    }

                    #endregion

                    #region 数据导入

                    foreach (var importClass in classDtoList)
                    {
                        foreach (var importClassSchedule in importClass.ClassScheduleList)
                        {
                            string strOrgName = String.Format("{0}-{1}", importClassSchedule.CourseName, importClass.ClassNo);
                            var org = (from p in db.Table<Course.Entity.tbOrg>()
                                       where p.OrgName == strOrgName
                                       orderby p.No
                                       select p).FirstOrDefault();
                            if (org == null)
                            {
                                var classRecord = classList.Where(c => c.Id == importClass.ClassId).FirstOrDefault();
                                org = new Course.Entity.tbOrg();
                                org.OrgName = strOrgName;
                                org.IsClass = true;
                                org.No = importClass.ClassNo;
                                org.tbClass = classRecord;
                                org.tbCourse = courseList.Where(c => c.Id == importClassSchedule.CourseId).FirstOrDefault();
                                org.tbGrade = classRecord.tbGrade;
                                org.tbYear = year;
                                db.Set<Course.Entity.tbOrg>().Add(org);
                                db.SaveChanges();
                            }

                            if (!String.IsNullOrWhiteSpace(importClassSchedule.TeacherName))
                            {
                                var orgTeacher = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                                  where p.tbOrg.Id == org.Id && p.tbTeacher.Id == importClassSchedule.TeacherId
                                                  orderby p.No
                                                  select p).FirstOrDefault();
                                if (orgTeacher == null)
                                {
                                    orgTeacher = new Course.Entity.tbOrgTeacher();
                                    orgTeacher.tbOrg = org;
                                    orgTeacher.tbTeacher = teacherList.Where(t => t.Id == importClassSchedule.TeacherId).FirstOrDefault();
                                    db.Set<Course.Entity.tbOrgTeacher>().Add(orgTeacher);
                                }
                                else
                                {
                                    if (vm.IsUpdate)
                                    {
                                        orgTeacher.tbOrg = org;
                                        orgTeacher.tbTeacher = teacherList.Where(t => t.Id == importClassSchedule.TeacherId).FirstOrDefault();
                                    }
                                }
                                db.SaveChanges();
                            }

                            var orgSchedule = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                                               where p.tbOrg.Id == org.Id
                                                    && p.tbWeek.Id == importClassSchedule.WeekId
                                                    && p.tbPeriod.Id == importClassSchedule.PeriodId
                                               orderby p.No
                                               select p).FirstOrDefault();
                            if (orgSchedule == null)
                            {
                                orgSchedule = new Course.Entity.tbOrgSchedule();
                                orgSchedule.tbOrg = org;
                                orgSchedule.tbWeek = weekList.Where(t => t.Id == importClassSchedule.WeekId).FirstOrDefault();
                                orgSchedule.tbPeriod = periodList.Where(t => t.Id == importClassSchedule.PeriodId).FirstOrDefault();
                                db.Set<Course.Entity.tbOrgSchedule>().Add(orgSchedule);
                            }
                            else
                            {
                                if (vm.IsUpdate)
                                {
                                    orgSchedule.tbOrg = org;
                                    orgSchedule.tbWeek = weekList.Where(t => t.Id == importClassSchedule.WeekId).FirstOrDefault();
                                    orgSchedule.tbPeriod = periodList.Where(t => t.Id == importClassSchedule.PeriodId).FirstOrDefault();
                                }
                            }
                            db.SaveChanges();
                        }

                        db.SaveChanges();

                        vm.Status = true;
                    }

                    #endregion
                }

                return View(vm);
            }
        }

        public ActionResult ImportClassSchedule()
        {
            var vm = new Models.Schedule.ImportClassSchedule();
            return View(vm);
        }

        public ActionResult ImportClassScheduleTemplate()
        {
            var file = Server.MapPath("~/Areas/Course/Views/Schedule/ImportClassSchedule.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportClassSchedule(Models.Schedule.ImportClassSchedule vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 上传文件
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

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

                var tbList = new List<string>() { "班序", "班级名称", "年级", "课程", "星期", "节次", "任课老师", "任课老师编号", "教室" };

                var Text = string.Empty;
                foreach (var a in tbList)
                {
                    if (dt.Columns.Contains(a.ToString()) == false)
                    {
                        Text += a + ",";
                    }
                }

                if (string.IsNullOrEmpty(Text) == false)
                {
                    ModelState.AddModelError("", "上传的EXCEL内容与预期不一致，缺少对应的字段：" + Text);
                    return View(vm);
                }

                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var dto = new Dto.Schedule.ImportClassSchedule()
                    {
                        #region
                        ClassName = dr["班级名称"].ConvertToString(),
                        GradeName = dr["年级"].ConvertToString(),
                        CourseName = dr["课程"].ConvertToString(),
                        PeriodName = dr["节次"].ConvertToString(),
                        RoomName = dr["教室"].ConvertToString(),
                        TeacherCode = dr["任课老师编号"].ConvertToString(),
                        TeacherName = dr["任课老师"].ConvertToString(),
                        WeekName = dr["星期"].ConvertToString()
                        #endregion
                    };

                    var no = 0;
                    if (int.TryParse(dr["班序"].ConvertToString(), out no))
                    {
                        dto.No = no;
                    }
                    else
                    {
                        dto.Error += "班序必须为数字类型；";
                    }

                    if (vm.DataList.Where(d => d.ClassName == dto.ClassName
                                             && d.GradeName == dto.GradeName
                                             && d.CourseName == dto.CourseName
                                             && d.PeriodName == dto.PeriodName
                                             && d.RoomName == dto.RoomName
                                             && d.TeacherCode == dto.TeacherCode
                                             && d.TeacherName == dto.TeacherName
                                             && d.WeekName == dto.WeekName).Any() == false)
                    {
                        vm.DataList.Add(dto);
                    }
                }

                if (vm.DataList.Any() == false)
                {
                    ModelState.AddModelError("", "未读取到任何有效数据!");
                    return View(vm);
                }
                #endregion

                var classList = db.Table<Basis.Entity.tbClass>().Include(d => d.tbGrade).ToList();
                var gradeList = db.Table<Basis.Entity.tbGrade>().ToList();
                var courseList = db.Table<Entity.tbCourse>().ToList();
                var weekList = db.Table<Basis.Entity.tbWeek>().ToList();
                var periodList = db.Table<Basis.Entity.tbPeriod>().ToList();
                var teacherList = db.Table<Areas.Teacher.Entity.tbTeacher>().ToList();
                var roomList = db.Table<Basis.Entity.tbRoom>().ToList();
                var orgList = db.Table<Entity.tbOrg>()
                    .Include(d => d.tbClass)
                    .Include(d => d.tbGrade)
                    .Include(d => d.tbCourse).ToList();
                var orgScheduleList = db.Table<Entity.tbOrgSchedule>()
                    .Include(d => d.tbPeriod)
                    .Include(d => d.tbWeek)
                    .Include(d => d.tbOrg.tbClass)
                    .Include(d => d.tbOrg.tbGrade)
                    .Include(d => d.tbOrg).Where(d => d.tbOrg.IsDeleted == false).ToList();
                var orgTeacherList = db.Table<Entity.tbOrgTeacher>()
                    .Include(d => d.tbOrg)
                    .Include(d => d.tbTeacher).Where(d => d.tbOrg.IsDeleted == false).ToList();

                #region 验证数据

                foreach (var v in vm.DataList)
                {
                    if (vm.IsUpdate == false && orgScheduleList.Where(d => d.tbOrg.IsClass
                          && d.tbOrg.tbClass.ClassName == v.ClassName
                          && d.tbOrg.tbGrade.GradeName == v.GradeName
                          && d.tbOrg.tbCourse.CourseName == v.CourseName
                          && d.tbPeriod.PeriodName == v.PeriodName.Split(']')[1]
                          && d.tbWeek.WeekName == v.WeekName).Any())
                    {
                        v.Error += "课表节次已存在；";
                    }
                    //if (vm.IsUpdate == false && orgList.Where(d => d.IsClass && d.tbClass.ClassName == v.ClassName && d.tbCourse.CourseName == v.CourseName && d.tbGrade.GradeName == v.GradeName).Any())
                    //{
                    //    v.Error += "班级对应教学班已存在；";
                    //}
                    if (string.IsNullOrWhiteSpace(v.RoomName) == false && roomList.Where(d => d.RoomName == v.RoomName).Any() == false)
                    {
                        v.Error += "教室不存在；";
                    }
                    //if (string.IsNullOrWhiteSpace(v.TeacherCode) == false && string.IsNullOrWhiteSpace(v.TeacherName) == false
                    //    && teacherList.Where(d => d.TeacherName == v.TeacherName && d.TeacherCode == v.TeacherCode).Any() == false)
                    //{
                    //    v.Error += "教师不存在；";
                    //}
                    if (string.IsNullOrWhiteSpace(v.TeacherCode) == false
                        && teacherList.Where(d => d.TeacherCode == v.TeacherCode).Any() == false)
                    {
                        v.Error += "教师不存在；";
                    }
                    if (string.IsNullOrWhiteSpace(v.TeacherName) == false
                        && teacherList.Where(d => d.TeacherName == v.TeacherName).Any() == false)
                    {
                        v.Error += "教师不存在；";
                    }
                    if (periodList.Where(d => d.PeriodName == v.PeriodName.Split(']')[1]).Any() == false)
                    {
                        v.Error += "节次不存在；";
                    }
                    if (weekList.Where(d => d.WeekName == v.WeekName).Any() == false)
                    {
                        v.Error += "星期不存在；";
                    }
                    if (courseList.Where(d => d.CourseName == v.CourseName).Any() == false)
                    {
                        v.Error += "课程不存在；";
                    }
                    if (classList.Where(d => d.ClassName == v.ClassName && d.tbGrade.GradeName == v.GradeName).Any() == false)
                    {
                        v.Error += "班级不存在；";
                    }
                }

                if (vm.DataList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                {
                    vm.DataList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                    return View(vm);
                }
                #endregion

                #region 数据入库
                var year = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                var tbOrgList = new List<Entity.tbOrg>();
                var tbOrgTeacherList = new List<Entity.tbOrgTeacher>();
                var tbOrgScheduleList = new List<Entity.tbOrgSchedule>();
                for (var i = 0; i < vm.DataList.Count; i++)
                {
                    #region
                    if (orgList.Where(d => d.IsClass
                        && d.tbClass.ClassName == vm.DataList[i].ClassName
                        && d.tbClass.tbGrade.GradeName == vm.DataList[i].GradeName
                        && d.tbCourse.CourseName == vm.DataList[i].CourseName).Any() == false)
                    {
                        #region 教学班
                        string orgName = String.Format("{0}-{1}", vm.DataList[i].CourseName, vm.DataList[i].No);
                        var org = new Entity.tbOrg()
                        {
                            IsClass = true,
                            OrgName = orgName,
                            No = vm.DataList[i].No,
                            tbClass = classList.Where(d => d.ClassName == vm.DataList[i].ClassName && d.tbGrade.GradeName == vm.DataList[i].GradeName).FirstOrDefault(),
                            tbCourse = courseList.Where(d => d.CourseName == vm.DataList[i].CourseName).FirstOrDefault(),
                            tbGrade = gradeList.Where(d => d.GradeName == vm.DataList[i].GradeName).FirstOrDefault(),
                            tbYear = year
                        };
                        if (string.IsNullOrWhiteSpace(vm.DataList[i].RoomName) == false)
                        {
                            org.tbRoom = roomList.Where(d => d.RoomName == vm.DataList[i].RoomName).FirstOrDefault();
                        }

                        if (tbOrgList.Where(d => d.tbClass.ClassName == org.tbClass.ClassName && d.tbCourse.CourseName == org.tbCourse.CourseName).Any() == false)
                        {
                            tbOrgList.Add(org);
                        }
                        #endregion
                    }
                    #endregion
                }

                foreach (var v in vm.DataList)
                {
                    var org = new Entity.tbOrg();
                    if (orgList.Where(d => d.IsClass
                        && d.tbClass.ClassName == v.ClassName
                        && d.tbClass.tbGrade.GradeName == v.GradeName
                        && d.tbCourse.CourseName == v.CourseName).Any())
                    {
                        org = orgList.Where(d => d.IsClass
                             && d.tbClass.ClassName == v.ClassName
                             && d.tbClass.tbGrade.GradeName == v.GradeName
                             && d.tbCourse.CourseName == v.CourseName).FirstOrDefault();

                        #region 教学班教师
                        if (string.IsNullOrWhiteSpace(v.TeacherCode) == false || string.IsNullOrWhiteSpace(v.TeacherName) == false)
                        {
                            if (orgTeacherList.Where(d => d.tbOrg.OrgName == org.OrgName && (string.IsNullOrEmpty(v.TeacherCode) ? (d.tbTeacher.TeacherName == v.TeacherName) : (d.tbTeacher.TeacherCode == v.TeacherCode))).Any() == false)
                            {
                                var orgTeacher = new Entity.tbOrgTeacher()
                                {
                                    tbOrg = org,
                                    tbTeacher = teacherList.Where(d => (string.IsNullOrEmpty(v.TeacherCode) ? (d.TeacherName == v.TeacherName) : (d.TeacherCode == v.TeacherCode))).FirstOrDefault()
                                };

                                if (tbOrgTeacherList.Where(d => d.tbOrg.OrgName == org.OrgName && d.tbTeacher.TeacherCode == v.TeacherCode).Any() == false)
                                {
                                    tbOrgTeacherList.Add(orgTeacher);
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        org = tbOrgList.Where(d => d.tbClass.ClassName == v.ClassName && d.tbCourse.CourseName == v.CourseName).FirstOrDefault();

                        #region 教学班教师
                        if (string.IsNullOrWhiteSpace(v.TeacherCode) == false || string.IsNullOrWhiteSpace(v.TeacherName) == false)
                        {
                            var orgTeacher = new Entity.tbOrgTeacher()
                            {
                                tbOrg = org,
                                tbTeacher = teacherList.Where(d => (string.IsNullOrEmpty(v.TeacherCode) ? (d.TeacherName == v.TeacherName) : (d.TeacherCode == v.TeacherCode))).FirstOrDefault()
                            };
                            if (tbOrgTeacherList.Where(d => d.tbOrg.OrgName == org.OrgName && d.tbTeacher.TeacherCode == v.TeacherCode).Any() == false)
                            {
                                tbOrgTeacherList.Add(orgTeacher);
                            }
                        }
                        #endregion
                    }
                    
                    #region 课表节次
                    var periodArr = v.PeriodName.Split(new char[] { '[', ']' });

                    var orgSchedule = new Entity.tbOrgSchedule()
                    {
                        tbOrg = org,
                        tbPeriod = periodList.Where(d => d.PeriodName == periodArr[2]).FirstOrDefault(),
                        tbWeek = weekList.Where(d => d.WeekName == v.WeekName).FirstOrDefault()
                    };
                    
                    switch (periodArr[1])
                    {
                        case "单周":
                            orgSchedule.ScheduleType = Code.EnumHelper.CourseScheduleType.Odd; break;
                        case "双周":
                            orgSchedule.ScheduleType = Code.EnumHelper.CourseScheduleType.Dual; break;
                        default:
                            orgSchedule.ScheduleType = Code.EnumHelper.CourseScheduleType.All; break;
                    }
                    
                    if (tbOrgScheduleList.Where(d => d.ScheduleType == orgSchedule.ScheduleType
                        && d.tbOrg.OrgName == org.OrgName
                        && d.tbPeriod.PeriodName == v.PeriodName
                        && d.tbWeek.WeekName == v.WeekName).Any() == false)
                    {
                        tbOrgScheduleList.Add(orgSchedule);
                    }
                    #endregion
                }

                db.Set<Entity.tbOrg>().AddRange(tbOrgList);
                db.Set<Entity.tbOrgTeacher>().AddRange(tbOrgTeacherList);
                db.Set<Entity.tbOrgSchedule>().AddRange(tbOrgScheduleList);

                if (db.SaveChanges() > 0)
                {
                    vm.DataList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入班级课表");
                    vm.Status = true;
                }
                #endregion
            }

            return View(vm);
        }

        public ActionResult ClassScheduleImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Course/Views/Schedule/ClassScheduleImportTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult ClassExportWord(int yearId)
        {
            var file = System.IO.Path.GetTempFileName();
            var classList = Basis.Controllers.ClassController.SelectList();
            var weekList = Basis.Controllers.WeekController.SelectScheduleList();
            var periodList = Basis.Controllers.PeriodController.SelectScheduleList();
            var orgScheduleList = OrgScheduleController.GetAll(yearId);
            var year = Areas.Basis.Controllers.YearController.SelectInfo(yearId);
            string strHeaderText = String.Format("{0}班级课程表", year.YearName);

            string[] arrColumns = new string[weekList.Count + 1];
            for (int i = 0; i < arrColumns.Length; i++)
            {
                arrColumns[i] = (i + 1).ToString();
            }

            List<Models.Schedule.ScheduleExportWord> scheduleList = new List<Models.Schedule.ScheduleExportWord>();

            foreach (var cla in classList)
            {
                Models.Schedule.ScheduleExportWord schedule = new Models.Schedule.ScheduleExportWord();
                schedule.HeaderText = strHeaderText;
                schedule.TableCaption = cla.Text;

                DataTable dt = Code.Common.ArrayToDataTable(arrColumns);

                string[] arrWeek = new string[weekList.Count + 1];
                for (int i = 0; i < weekList.Count; i++)
                {
                    arrWeek[i + 1] = weekList[i].Text;
                }
                dt.Rows.Add(arrWeek);

                foreach (var period in periodList)
                {
                    string[] arrSchedule = new string[weekList.Count + 1];
                    arrSchedule[0] = period.Text;

                    for (int i = 0; i < weekList.Count; i++)
                    {
                        var orgSchedule = orgScheduleList.Where(d => d.ClassId == Convert.ToInt32(cla.Value)
                          && d.WeekId == Convert.ToInt32(weekList[i].Value)
                          && d.PeriodId == Convert.ToInt32(period.Value)).FirstOrDefault();
                        if (orgSchedule != null)
                        {
                            arrSchedule[i + 1] = orgSchedule.CourseName;
                            var orgTeacher = OrgTeacherController.GetTeacherByOrgId(orgSchedule.OrgId);
                            if (orgTeacher != null)
                            {
                                arrSchedule[i + 1] += "|" + orgTeacher.TeacherName;
                            }
                        }
                    }

                    dt.Rows.Add(arrSchedule);
                }

                schedule.ScheduleList = dt;

                scheduleList.Add(schedule);
            }

            Code.DocxHelper.ScheduleToWord(file, scheduleList);

            return File(file, Code.Common.DownloadType, Code.Common.ExportByWord);
        }

        public ActionResult ClassSetImportWord()
        {
            var vm = new Models.Schedule.ClassSetImportWord();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassSetImportWord(Models.Schedule.ClassSetImportWord vm)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

                if (Code.Common.GetFileType(file.FileName) != Code.FileType.Word)
                {
                    ModelState.AddModelError("", "上传的文件不是正确的Word文件!");
                    return View(vm);
                }

                var weekList = Basis.Controllers.WeekController.SelectScheduleList();
                var columnNameList = new List<string>() { String.Empty };
                weekList.ForEach(week =>
                {
                    columnNameList.Add(week.Text);
                });
                var periodList = Basis.Controllers.PeriodController.SelectScheduleList();
                var periodNameList = new List<string>();
                periodList.ForEach(period =>
                {
                    periodNameList.Add(period.Text);
                });
                //var classList = Areas.Basis.Controllers.ClassController.SelectInfoList(vm.YearId);
                var classList = Areas.Basis.Controllers.ClassController.SelectInfoListByYearType(Code.EnumHelper.YearType.Section, vm.YearId);

                var courseList = Areas.Course.Controllers.CourseController.SelectList();
                var teacherList = Areas.Teacher.Controllers.TeacherController.SelectList();

                List<Models.Schedule.ClassSetImportWordSchedule> scheduleList = Code.DocxHelper.WordToImportSchedule(fileSave);

                #region 数据校验
                List<string> classNames = new List<string>();
                foreach (var schedule in scheduleList)
                {
                    if (String.IsNullOrWhiteSpace(schedule.ClassName))
                    {
                        continue;
                    }

                    if (classNames.Contains(schedule.ClassName))
                    {
                        vm.ErrorMessageList.Add(String.Format("WORD中重复的行政班级：{0}", schedule.ClassName));
                    }
                    else
                    {
                        classNames.Add(schedule.ClassName);
                    }

                    if (!classList.Select(c => c.ClassName).Contains(schedule.ClassName))
                    {
                        vm.ErrorMessageList.Add(String.Format("系统中不存在的行政班级：{0}", schedule.ClassName));
                    }

                    foreach (DataColumn column in schedule.ClassScheduleList.Columns)
                    {
                        if (!column.ColumnName.Equals("Column1") && !columnNameList.Contains(column.ColumnName))
                        {
                            vm.ErrorMessageList.Add(String.Format("{0}中不存在的星期：{1}", schedule.ClassName, column.ColumnName));
                        }
                    }

                    foreach (DataRow dr in schedule.ClassScheduleList.Rows)
                    {
                        if (!periodNameList.Contains(dr[0].ToString()))
                        {
                            vm.ErrorMessageList.Add(String.Format("{0}中不存在的节次：{1}", schedule.ClassName, dr[0].ToString()));
                        }

                        for (int i = 1; i < schedule.ClassScheduleList.Columns.Count; i++)
                        {
                            if (!String.IsNullOrWhiteSpace(dr[i].ToString()))
                            {
                                string[] arrCourseTeacher = dr[i].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                string courseName = arrCourseTeacher[0];
                                string teacherName = String.Empty;
                                if (arrCourseTeacher.Count() > 1)
                                {
                                    teacherName = arrCourseTeacher[1];
                                }

                                if (!String.IsNullOrWhiteSpace(courseName) && !courseList.Select(c => c.Text).Contains(courseName))
                                {
                                    vm.ErrorMessageList.Add(String.Format("{0}中不存在的课程名称：{1}", schedule.ClassName, courseName));
                                }

                                if (!String.IsNullOrWhiteSpace(teacherName) && !teacherList.Select(t => t.Text).Contains(teacherName))
                                {
                                    vm.ErrorMessageList.Add(String.Format("{0}中不存在的教师姓名：{1}", schedule.ClassName, teacherName));
                                }
                            }
                        }
                    }
                }
                if (vm.ErrorMessageList.Count > 0)
                {
                    return View(vm);
                }
                #endregion

                #region 数据转换
                foreach (var schedule in scheduleList)
                {
                    if (String.IsNullOrWhiteSpace(schedule.ClassName))
                    {
                        continue;
                    }

                    var cla = classList.Where(t => t.ClassName == schedule.ClassName).FirstOrDefault();

                    var classSetImportWord = new Dto.Schedule.ClassSetImportWord();
                    classSetImportWord.ClassId = cla.Id;
                    classSetImportWord.ClassName = cla.ClassName;
                    classSetImportWord.ClassNo = cla.No;

                    foreach (DataRow row in schedule.ClassScheduleList.Rows)
                    {
                        foreach (DataColumn column in schedule.ClassScheduleList.Columns)
                        {
                            if (String.IsNullOrWhiteSpace(column.ColumnName)
                                || column.ColumnName.Equals("Column1")
                                || String.IsNullOrWhiteSpace(row[column.ColumnName].ToString()))
                            {
                                continue;
                            }

                            var classSetImportWordItem = new Dto.Schedule.ClassSetImportWordItem();

                            var period = periodList.Where(p => p.Text == row[0].ToString()).FirstOrDefault();
                            classSetImportWordItem.PeriodId = period.Value.ConvertToInt();
                            classSetImportWordItem.PeriodName = period.Text;

                            var week = weekList.Where(w => w.Text == column.ColumnName).FirstOrDefault();
                            classSetImportWordItem.WeekId = week.Value.ConvertToInt();
                            classSetImportWordItem.WeekName = week.Text;

                            string[] arrCourseTeacher = row[column.ColumnName].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            var course = courseList.Where(c => c.Text == arrCourseTeacher[0]).FirstOrDefault();
                            classSetImportWordItem.CourseId = course.Value.ConvertToInt();
                            classSetImportWordItem.CourseName = course.Text;

                            if (arrCourseTeacher.Count() > 1
                                && !String.IsNullOrWhiteSpace(arrCourseTeacher[1]))
                            {
                                var teacher = teacherList.Where(t => t.Text == arrCourseTeacher[1]).FirstOrDefault();
                                classSetImportWordItem.TeacherId = teacher.Value.ConvertToInt();
                                classSetImportWordItem.TeacherName = teacher.Text;
                            }

                            classSetImportWord.ClassSetImportWordItemList.Add(classSetImportWordItem);
                        }
                    }

                    vm.ClassSetImportWordList.Add(classSetImportWord);
                }
                #endregion

                #region 数据导入
                using (var db = new XkSystem.Models.DbContext())
                {
                    var year = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);

                    var classRecordList = (from d in db.Table<Basis.Entity.tbClass>()
                                                      .Include(d => d.tbGrade)
                                           select d).ToList();
                    var courseRecordList = db.Table<Course.Entity.tbCourse>().ToList();
                    var teacherRecordList = db.Table<Teacher.Entity.tbTeacher>().ToList();
                    var weekRecordList = db.Table<Basis.Entity.tbWeek>().ToList();
                    var periodRecordList = db.Table<Basis.Entity.tbPeriod>().ToList();

                    foreach (var importClass in vm.ClassSetImportWordList)
                    {
                        foreach (var importClassSchedule in importClass.ClassSetImportWordItemList)
                        {
                            string strOrgName = String.Format("{0}-{1}", importClassSchedule.CourseName, importClass.ClassNo);
                            var org = (from p in db.Table<Course.Entity.tbOrg>()
                                       where p.OrgName == strOrgName
                                       orderby p.No
                                       select p).FirstOrDefault();
                            if (org == null)
                            {
                                var classRecord = classRecordList.Where(c => c.Id == importClass.ClassId).FirstOrDefault();

                                org = new Course.Entity.tbOrg();
                                org.OrgName = strOrgName;
                                org.IsClass = true;
                                org.No = importClass.ClassNo;
                                org.tbClass = classRecord;
                                org.tbCourse = courseRecordList.Where(c => c.Id == importClassSchedule.CourseId).FirstOrDefault();
                                org.tbGrade = classRecord.tbGrade;
                                org.tbYear = year;
                                db.Set<Course.Entity.tbOrg>().Add(org);
                                db.SaveChanges();
                            }

                            if (!String.IsNullOrWhiteSpace(importClassSchedule.TeacherName))
                            {
                                var teacher = teacherList.Where(t => t.Text == importClassSchedule.TeacherName).FirstOrDefault();
                                int teacherId = Convert.ToInt32(teacher.Value);
                                var orgTeacher = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                                  where p.tbOrg.Id == org.Id && p.tbTeacher.Id == teacherId
                                                  orderby p.No
                                                  select p).FirstOrDefault();
                                if (orgTeacher == null)
                                {
                                    orgTeacher = new Course.Entity.tbOrgTeacher();
                                    orgTeacher.tbOrg = org;
                                    orgTeacher.tbTeacher = teacherRecordList.Where(t => t.Id == teacherId).FirstOrDefault();
                                    db.Set<Course.Entity.tbOrgTeacher>().Add(orgTeacher);
                                }
                                else
                                {
                                    if (vm.IsUpdate)
                                    {
                                        orgTeacher.tbOrg = org;
                                        orgTeacher.tbTeacher = teacherRecordList.Where(t => t.Id == teacherId).FirstOrDefault();
                                    }
                                }
                                db.SaveChanges();
                            }

                            var orgSchedule = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                                               where p.tbOrg.Id == org.Id && p.tbWeek.Id == importClassSchedule.WeekId && p.tbPeriod.Id == importClassSchedule.PeriodId
                                               orderby p.No
                                               select p).FirstOrDefault();
                            if (orgSchedule == null)
                            {
                                orgSchedule = new Course.Entity.tbOrgSchedule();
                                orgSchedule.tbOrg = org;
                                orgSchedule.tbWeek = weekRecordList.Where(t => t.Id == importClassSchedule.WeekId).FirstOrDefault();
                                orgSchedule.tbPeriod = periodRecordList.Where(t => t.Id == importClassSchedule.PeriodId).FirstOrDefault();
                                db.Set<Course.Entity.tbOrgSchedule>().Add(orgSchedule);
                            }
                            else
                            {
                                if (vm.IsUpdate)
                                {
                                    orgSchedule.tbOrg = org;
                                    orgSchedule.tbWeek = weekRecordList.Where(t => t.Id == importClassSchedule.WeekId).FirstOrDefault();
                                    orgSchedule.tbPeriod = periodRecordList.Where(t => t.Id == importClassSchedule.PeriodId).FirstOrDefault();
                                }
                            }
                            db.SaveChanges();
                        }
                    }

                    db.SaveChanges();

                    vm.Status = true;
                }
                #endregion
            }

            return View(vm);
        }

        public ActionResult ClassSetImportWordTemplate()
        {
            var vm = new Models.Schedule.ClassSetImportWord();

            var weekList = Basis.Controllers.WeekController.SelectScheduleList();
            var periodList = Basis.Controllers.PeriodController.SelectScheduleList();
            string[] arrColumns = new string[weekList.Count + 1];
            for (int i = 0; i < arrColumns.Length; i++)
            {
                arrColumns[i] = (i + 1).ToString();
            }

            DataTable dt = Code.Common.ArrayToDataTable(arrColumns);

            string[] arrWeek = new string[weekList.Count + 1];
            for (int i = 0; i < weekList.Count; i++)
            {
                arrWeek[i + 1] = weekList[i].Text;
            }
            dt.Rows.Add(arrWeek);

            foreach (var period in periodList)
            {
                string[] arrPeriod = new string[weekList.Count + 1];
                arrPeriod[0] = period.Text;

                dt.Rows.Add(arrPeriod);
            }

            var file = System.IO.Path.GetTempFileName();

            var year = Areas.Basis.Controllers.YearController.SelectInfo(vm.YearId);

            dt.Rows[1][1] = "英语|吴平";

            Code.DocxHelper.DataTableToWord(file, dt, String.Format("{0}班级课程表", year.YearName), "一年级（1）班");

            return File(file, Code.Common.DownloadType, Code.Common.ExportByWord);
        }

        public ActionResult ClassSetAll()
        {
            var vm = new Models.Schedule.ClassSetAll();
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
            if (vm.YearList.Count > 0 && vm.YearId == 0)
            {
                vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
            }

            var yearId = 0;
            using (var db = new XkSystem.Models.DbContext())
            {
                yearId = (from p in db.Table<Basis.Entity.tbYear>()
                          where p.Id == vm.YearId
                          select p.tbYearParent.tbYearParent.Id).FirstOrDefault();
            }

            vm.WeekList = Basis.Controllers.WeekController.SelectList();
            vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
            vm.ClassList = Basis.Controllers.ClassController.SelectList(yearId, 0, 0, vm.SearchText);
            vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassSetAll(Models.Schedule.ClassSetAll vm)
        {
            var error = new List<string>();

            return Code.MvcHelper.Post(error, Url.Action("ClassSetAll", new { searchText = vm.SearchText, yearId = vm.YearId, }));
        }

        public ActionResult ClassSet(int classId, int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.ClassSetAll();

                var baseYearId = db.Table<Basis.Entity.tbYear>().Include(d => d.tbYearParent.tbYearParent).Where(d => d.Id == yearId).Select(d => d.tbYearParent.tbYearParent.Id).FirstOrDefault();

                vm.ClassList = Basis.Controllers.ClassController.SelectList(0, 0, classId);
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
                vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId).Where(x => x.ClassId == classId).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassSet(int classId, int yearId, string jsonOrgSchedule)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var dbPeriod = db.Set<Basis.Entity.tbPeriod>().ToList();
                var dbWeek = db.Set<Basis.Entity.tbWeek>().ToList();
                var dbOrg = db.Table<Course.Entity.tbOrg>().Where(t => t.tbYear.Id == yearId
                                                        && t.tbClass.Id == classId
                                                    ).ToList();

                var orgScheduleList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.Schedule.ClassSetOrgSchedule>>(jsonOrgSchedule);
                foreach (var orgSchedule in orgScheduleList)
                {
                    var dbOrgSchedule = db.Table<Course.Entity.tbOrgSchedule>().Where(t => t.tbOrg.tbClass.Id == classId
                                                                            && t.tbOrg.tbYear.Id == yearId
                                                                        ).ToList();

                    int originalWeekId = orgSchedule.OriginalWeekId;
                    int originalPeriodId = orgSchedule.OriginalPeriodId;
                    int originalOrgId = orgSchedule.OriginalOrgId;
                    int modifyWeekId = orgSchedule.ModifyWeekId;
                    int modifyPeriodId = orgSchedule.ModifyPeriodId;
                    int modifyOrgId = orgSchedule.ModifyOrgId;

                    if (originalOrgId == 0)
                    {
                        var modifyOrgSchedule = dbOrgSchedule.Where(p => p.tbOrg.Id == modifyOrgId
                                                                        && p.tbWeek.Id == modifyWeekId
                                                                        && p.tbPeriod.Id == modifyPeriodId
                                                                    ).FirstOrDefault();
                        if (modifyOrgSchedule != null)
                        {
                            modifyOrgSchedule.IsDeleted = true;
                        }

                        var originalOrgSchedule = new Course.Entity.tbOrgSchedule();
                        originalOrgSchedule.tbOrg = dbOrg.Where(t => t.Id == modifyOrgId).FirstOrDefault();
                        originalOrgSchedule.tbPeriod = dbPeriod.Where(t => t.Id == originalPeriodId).FirstOrDefault();
                        originalOrgSchedule.tbWeek = dbWeek.Where(t => t.Id == originalWeekId).FirstOrDefault();

                        db.Set<Course.Entity.tbOrgSchedule>().Add(originalOrgSchedule);
                    }
                    else
                    {
                        if (modifyOrgId == 0)
                        {
                            var originalSchedule = dbOrgSchedule.Where(p => p.tbOrg.Id == originalOrgId
                                                                           && p.tbWeek.Id == originalWeekId
                                                                           && p.tbPeriod.Id == originalPeriodId
                                                                        ).FirstOrDefault();
                            if (originalSchedule != null)
                            {
                                originalSchedule.IsDeleted = true;
                            }

                            var modifyOrgSchedule = new Course.Entity.tbOrgSchedule();
                            modifyOrgSchedule.tbOrg = dbOrg.Where(t => t.Id == originalOrgId).FirstOrDefault();
                            modifyOrgSchedule.tbPeriod = dbPeriod.Where(t => t.Id == modifyPeriodId).FirstOrDefault();
                            modifyOrgSchedule.tbWeek = dbWeek.Where(t => t.Id == modifyWeekId).FirstOrDefault();

                            db.Set<Course.Entity.tbOrgSchedule>().Add(modifyOrgSchedule);
                        }
                        else
                        {
                            var modifyOrgSchedule = dbOrgSchedule.Where(p => p.tbOrg.Id == modifyOrgId
                                                                            && p.tbWeek.Id == modifyWeekId
                                                                            && p.tbPeriod.Id == modifyPeriodId
                                                                        ).FirstOrDefault();
                            if (modifyOrgSchedule != null)
                            {
                                modifyOrgSchedule.IsDeleted = true;
                            }

                            var newOriginalSchedule = new Course.Entity.tbOrgSchedule();
                            newOriginalSchedule.tbOrg = dbOrg.Where(t => t.Id == modifyOrgId).FirstOrDefault();
                            newOriginalSchedule.tbPeriod = dbPeriod.Where(t => t.Id == originalPeriodId).FirstOrDefault();
                            newOriginalSchedule.tbWeek = dbWeek.Where(t => t.Id == originalWeekId).FirstOrDefault();

                            db.Set<Course.Entity.tbOrgSchedule>().Add(newOriginalSchedule);

                            var originalSchedule = dbOrgSchedule.Where(p => p.tbOrg.Id == originalOrgId
                                                                           && p.tbWeek.Id == originalWeekId
                                                                           && p.tbPeriod.Id == originalPeriodId
                                                                        ).FirstOrDefault();
                            if (originalSchedule != null)
                            {
                                originalSchedule.IsDeleted = true;
                            }

                            var newModifyOrgSchedule = new Course.Entity.tbOrgSchedule();
                            newModifyOrgSchedule.tbOrg = dbOrg.Where(t => t.Id == originalOrgId).FirstOrDefault();
                            newModifyOrgSchedule.tbPeriod = dbPeriod.Where(t => t.Id == modifyPeriodId).FirstOrDefault();
                            newModifyOrgSchedule.tbWeek = dbWeek.Where(t => t.Id == modifyWeekId).FirstOrDefault();

                            db.Set<Course.Entity.tbOrgSchedule>().Add(newModifyOrgSchedule);
                        }
                    }
                    db.SaveChanges();
                }
                return Code.MvcHelper.Post();
            }
        }

        public ActionResult StudentSetAll()
        {
            var vm = new Models.Schedule.StudentSetAll();
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
            if (vm.YearList.Count > 0 && vm.YearId == 0)
            {
                vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
            }

            var yearId = 0;
            using (var db = new XkSystem.Models.DbContext())
            {
                yearId = (from p in db.Table<Basis.Entity.tbYear>()
                          where p.Id == vm.YearId
                          select p.tbYearParent.tbYearParent.Id).FirstOrDefault();
            }

            vm.ClassInfoList = Areas.Basis.Controllers.ClassController.SelectInfoList(yearId);
            if (vm.ClassId == 0 && vm.ClassInfoList.Count > 0)
            {
                vm.ClassId = vm.ClassInfoList.FirstOrDefault().Id;
            }
            vm.StudentInfoList = Areas.Student.Controllers.StudentController.SelectInfoList(vm.ClassId, vm.SearchText);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentSetAll(XkSystem.Areas.Course.Models.Schedule.ClassAll vm)
        {
            var error = new List<string>();

            return Code.MvcHelper.Post(error, Url.Action("StudentSetAll", new { searchText = vm.SearchText, yearId = vm.YearId, classId = vm.ClassId }));
        }

        public ActionResult StudentSet(int studentId, int yearId)
        {
            var vm = new Models.Schedule.StudentSet();

            var baseYearId = 0;
            using (var db = new XkSystem.Models.DbContext())
            {
                baseYearId = db.Table<Basis.Entity.tbYear>().Include(d => d.tbYearParent.tbYearParent).Where(d => d.Id == yearId).Select(d => d.tbYearParent.tbYearParent.Id).FirstOrDefault();
            }

            vm.StudentInfo = Areas.Student.Controllers.StudentController.GetStudentById(studentId).FirstOrDefault();
            vm.WeekList = Basis.Controllers.WeekController.SelectList();
            vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
            vm.OrgScheduleList = OrgScheduleController.GetStudentAll(studentId, vm.YearId).ToList();

            return View(vm);
        }

        public ActionResult StudentSetOrg()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.StudentSetOrg();
                var tb = from p in db.Table<Course.Entity.tbOrgSchedule>()
                         .Include(p => p.tbOrg)
                         where p.tbWeek.Id == vm.WeekId
                            && p.tbOrg.OrgName.Contains(vm.SearchText)
                            && p.tbPeriod.Id == vm.PeriodId
                            && p.tbOrg.tbYear.Id == vm.YearId
                            && p.tbOrg.IsClass == false
                            && p.tbOrg.IsDeleted == false
                         select p.tbOrg;

                vm.OrgList = (from p in tb
                              orderby p.tbGrade.No, p.tbCourse.tbSubject.No, p.tbCourse.CourseName, p.No
                              select new Dto.Org.List
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  OrgName = p.OrgName,
                                  SubjectName = p.tbCourse.tbSubject.SubjectName,
                                  CourseName = p.tbCourse.CourseName,
                                  YearName = p.tbYear.YearName,
                                  GradeName = p.tbGrade.GradeName,
                                  RoomName = p.tbRoom != null ? p.tbRoom.RoomName : string.Empty,
                                  StudentCount = p.IsClass && p.tbClass != null ?
                                        db.Set<Basis.Entity.tbClassStudent>().Where(d => d.IsDeleted == false && d.tbStudent.IsDeleted == false && d.tbClass.Id == p.tbClass.Id).Count()
                                        : db.Set<Course.Entity.tbOrgStudent>().Where(d => d.IsDeleted == false && d.tbStudent.IsDeleted == false && d.tbOrg.Id == p.Id).Count()
                              }).ToList();

                var OrgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                      where p.tbTeacher.IsDeleted == false
                                      && (p.tbOrg.tbYear.Id == vm.YearId || vm.YearId == 0)
                                      select new
                                      {
                                          OrgId = p.tbOrg.Id,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();
                foreach (var a in vm.OrgList)
                {
                    a.TeacherName = string.Join(",", OrgTeacherList.Where(d => d.OrgId == a.Id).Select(d => d.TeacherName));
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentSetOrg(Models.Schedule.StudentSetOrg vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StudentSetOrg", new
            {
                SearchText = vm.SearchText,
                WeekId = vm.WeekId,
                PeriodId = vm.PeriodId,
                YearId = vm.YearId
            }));
            //using (var db = new XkSystem.Models.DbContext())
            //{
            //    var org = db.Set<Course.Entity.tbOrg>().Find(vm.SelectedOrgId);
            //    var student = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);

            //    var orgStudent = new Course.Entity.tbOrgStudent();
            //    if (vm.OrgId != 0)
            //    {
            //        orgStudent = (from p in db.Table<Course.Entity.tbOrgStudent>()
            //                      .Include(t => t.tbOrg)
            //                      .Include(t => t.tbStudent)
            //                      where p.tbOrg.Id == vm.OrgId && p.tbStudent.Id == vm.StudentId
            //                      select p).FirstOrDefault();

            //        if (orgStudent != null)
            //        {
            //            orgStudent.tbOrg = org;
            //        }
            //    }
            //    else
            //    {
            //        orgStudent.No = db.Set<Course.Entity.tbOrgStudent>()
            //                        .Select(t => t.No)
            //                        .Max() + 1;
            //        orgStudent.No = db.Set<Course.Entity.tbOrgStudent>()
            //                        .Select(t => t.No)
            //                        .Max() + 1;
            //        orgStudent.tbOrg = org;
            //        orgStudent.tbStudent = student;

            //        db.Set<Course.Entity.tbOrgStudent>().Add(orgStudent);
            //    }

            //    db.SaveChanges();
            //}

            //return Code.MvcHelper.Post();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentSetOrgRemove(int studentId, int orgId, int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var orgStudent = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                 .Include(t => t.tbOrg)
                                 .Include(t => t.tbStudent)
                                  where p.tbOrg.Id == orgId && p.tbStudent.Id == studentId
                                  select p
                                 ).FirstOrDefault();
                if (orgStudent != null)
                {
                    orgStudent.IsDeleted = true;
                    db.SaveChanges();
                }

                return Code.MvcHelper.Post(null, Url.Action("StudentSet", new { studentId = studentId, yearId = yearId, }), "删除成功！");
            }
        }

        public ActionResult My()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.ClassAll();
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();

                var year = (from p in db.Table<Basis.Entity.tbYear>()
                            where p.YearType == Code.EnumHelper.YearType.Section
                            && p.IsDefault
                            select new
                            {
                                p.Id
                            }).FirstOrDefault();
                if (year != null)
                {
                    switch (XkSystem.Code.Common.UserType)
                    {
                        case Code.EnumHelper.SysUserType.Administrator:
                            var admin = Areas.Teacher.Controllers.TeacherController.GetTeacherByUserId(Code.Common.UserId);
                            if (admin != null)
                            {
                                vm.OrgScheduleList = OrgScheduleController.GetTeacherAll(admin.Id, year.Id).ToList();
                            }
                            break;
                        case Code.EnumHelper.SysUserType.Teacher:
                            var teacher = Areas.Teacher.Controllers.TeacherController.GetTeacherByUserId(Code.Common.UserId);
                            if (teacher != null)
                            {
                                vm.OrgScheduleList = OrgScheduleController.GetTeacherAll(teacher.Id, year.Id).ToList();
                            }
                            break;
                        case Code.EnumHelper.SysUserType.Student:
                            var student = Areas.Student.Controllers.StudentController.GetStudentInfoByUserId(Code.Common.UserId);
                            if (student != null)
                            {
                                vm.OrgScheduleList = OrgScheduleController.GetStudentAll(student.Id, year.Id).ToList();
                            }

                            break;
                        case Code.EnumHelper.SysUserType.Family:
                            var dd = db.Table<Areas.Student.Entity.tbStudent>().Where(d => d.tbSysUserFamily.Id == Code.Common.UserId).FirstOrDefault();
                            if (dd != null)
                            {
                                vm.OrgScheduleList = OrgScheduleController.GetStudentAll(dd.Id, year.Id).ToList();
                            }
                            break;
                    }
                }
                return View(vm);
            }
        }

        public ActionResult PartialSchedule()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.ClassAll();
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();

                var year = (from p in db.Table<Basis.Entity.tbYear>()
                            where p.YearType == Code.EnumHelper.YearType.Section
                            && p.IsDefault
                            select new
                            {
                                p.Id
                            }).FirstOrDefault();
                if (year != null)
                {
                    switch (XkSystem.Code.Common.UserType)
                    {
                        case Code.EnumHelper.SysUserType.Administrator:
                            var admin = Areas.Teacher.Controllers.TeacherController.GetTeacherByUserId(Code.Common.UserId);
                            if (admin != null)
                            {
                                vm.OrgScheduleList = OrgScheduleController.GetTeacherAll(admin.Id, year.Id).ToList();
                            }
                            break;
                        case Code.EnumHelper.SysUserType.Teacher:
                            var teacher = Areas.Teacher.Controllers.TeacherController.GetTeacherByUserId(Code.Common.UserId);
                            if (teacher != null)
                            {
                                vm.OrgScheduleList = OrgScheduleController.GetTeacherAll(teacher.Id, year.Id).ToList();
                            }
                            break;
                        case Code.EnumHelper.SysUserType.Student:
                            var student = Areas.Student.Controllers.StudentController.GetStudentInfoByUserId(Code.Common.UserId);
                            if (student != null)
                            {
                                vm.OrgScheduleList = OrgScheduleController.GetStudentAll(student.Id, year.Id).ToList();
                            }

                            break;
                    }
                }
                return PartialView(vm);
            }
        }

        public ActionResult PartialScheduleDaily(string dayOfWeek = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Schedule.ClassAll();
                vm.PeriodList2 = Basis.Controllers.PeriodController.SelectListForColor();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectList().ToList();
                var year = (from p in db.Table<Basis.Entity.tbYear>()
                            where p.YearType == Code.EnumHelper.YearType.Section
                            && p.IsDefault
                            select new
                            {
                                p.Id
                            }).FirstOrDefault();

                if (year != null)
                {
                    int weekNo = (int)DateTime.Now.DayOfWeek;
                    weekNo = weekNo == 0 ? 7 : weekNo;
                    var week = Areas.Basis.Controllers.WeekController.SelectInfo(weekNo);


                    switch (XkSystem.Code.Common.UserType)
                    {
                        case Code.EnumHelper.SysUserType.Administrator:
                            var admin = Areas.Teacher.Controllers.TeacherController.GetTeacherByUserId(Code.Common.UserId);
                            if (admin != null)
                            {
                                vm.OrgScheduleList = OrgScheduleController.GetTeacherAll(admin.Id, year.Id).Where(t => t.WeekId == week.Id).ToList();
                            }
                            break;
                        case Code.EnumHelper.SysUserType.Teacher:
                            var teacher = Areas.Teacher.Controllers.TeacherController.GetTeacherByUserId(Code.Common.UserId);
                            if (teacher != null)
                            {
                                vm.OrgScheduleList = OrgScheduleController.GetTeacherAll(teacher.Id, year.Id).Where(t => t.WeekId == week.Id).ToList();
                            }
                            break;
                        case Code.EnumHelper.SysUserType.Student:
                            var student = Areas.Student.Controllers.StudentController.GetStudentInfoByUserId(Code.Common.UserId);
                            if (student != null)
                            {
                                vm.OrgScheduleList = OrgScheduleController.GetStudentAll(student.Id, year.Id).Where(t => t.WeekId == week.Id).ToList();
                            }

                            break;
                    }
                }

                return PartialView(vm);
            }
        }

        public ActionResult OrgAll()
        {
            var vm = new Models.Schedule.OrgAll();
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
            if (vm.YearList.Count > 0 && vm.YearId == 0)
            {
                vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
            }

            vm.WeekList = Basis.Controllers.WeekController.SelectList();
            vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
            vm.OrgList = Areas.Course.Controllers.OrgController.SelectOrgList(vm.YearId, vm.SearchText);
            vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrgAll(Models.Schedule.OrgAll vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("OrgAll", new { searchText = vm.SearchText, yearId = vm.YearId, }));
        }

        public ActionResult OrgDetail(int orgId, int yearId)
        {
            var vm = new Models.Schedule.OrgAll();
            vm.WeekList = Basis.Controllers.WeekController.SelectList();
            vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
            vm.OrgInfo = Areas.Course.Controllers.OrgController.SelectInfo(orgId);
            vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId).Where(t => t.OrgId == orgId).ToList();

            return View(vm);
        }

        public ActionResult OrgExport(int yearId)
        {
            var file = System.IO.Path.GetTempFileName();
            var classExport = new Models.Schedule.ClassExport();

            var OrgList = Areas.Course.Controllers.OrgController.SelectOrgList(yearId);
            var WeekList = Basis.Controllers.WeekController.SelectList();
            var PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
            var OrgScheduleList = OrgScheduleController.GetAll(yearId);

            string[] arrColumns = new string[WeekList.Count * PeriodList.Count + 1];
            for (int i = 0; i < arrColumns.Length; i++)
            {
                arrColumns[i] = (i + 1).ToString();
            }

            List<NPOI.SS.Util.CellRangeAddress> regions = new List<NPOI.SS.Util.CellRangeAddress>();
            DataTable dt = Code.Common.ArrayToDataTable(arrColumns);

            string[] arrWeek = new string[WeekList.Count * PeriodList.Count + 1];
            string[] arrWeekId = new string[WeekList.Count * PeriodList.Count + 1];
            string[] arrPeriod = new string[WeekList.Count * PeriodList.Count + 1];
            string[] arrPeriodId = new string[WeekList.Count * PeriodList.Count + 1];
            for (int i = 0; i < WeekList.Count; i++)
            {
                for (int j = 0; j < PeriodList.Count; j++)
                {
                    arrWeek[i * PeriodList.Count + j + 1] = WeekList[i].Text;
                    arrWeekId[i * PeriodList.Count + j + 1] = WeekList[i].Value;
                    arrPeriod[i * PeriodList.Count + j + 1] = PeriodList[j].Text;
                    arrPeriodId[i * PeriodList.Count + j + 1] = PeriodList[j].Value;
                }

                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, i * PeriodList.Count + 1, (i + 1) * PeriodList.Count));
            }
            dt.Rows.Add(arrWeekId);
            dt.Rows.Add(arrPeriodId);
            dt.Rows.Add(arrWeek);
            dt.Rows.Add(arrPeriod);

            for (int i = 0; i < OrgList.Count; i++)
            {
                string[] arrTemp = new string[WeekList.Count * PeriodList.Count + 1];
                arrTemp[0] = OrgList[i].Text;

                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    var orgSchedule = OrgScheduleList.Where(d => d.OrgId == Convert.ToInt32(OrgList[i].Value)
                          && d.WeekId == Convert.ToInt32(dt.Rows[0][j])
                          && d.PeriodId == Convert.ToInt32(dt.Rows[1][j])).FirstOrDefault();

                    if (orgSchedule != null)
                    {
                        arrTemp[j] = orgSchedule.Subject;
                        var orgTeacher = OrgTeacherController.GetTeacherByOrgId(orgSchedule.OrgId);
                        if (orgTeacher != null)
                        {
                            arrTemp[j] += "\n" + orgTeacher.TeacherName;
                        }
                    }
                }
                dt.Rows.Add(arrTemp);
            }
            dt.Rows.RemoveAt(1);
            dt.Rows.RemoveAt(0);

            dt.Rows[0][0] = "教学班";
            regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));

            Code.NpoiHelper.DataTableToExcel(file, dt, false, regions, "教学班课表");

            return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
        }

        public ActionResult SubjectAll()
        {
            var vm = new Models.Schedule.SubjectAll();
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
            if (vm.YearList.Count > 0 && vm.YearId == 0)
            {
                vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
            }

            vm.WeekList = Basis.Controllers.WeekController.SelectList();
            vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
            vm.SubjectList = Areas.Course.Controllers.SubjectController.SelectList();
            vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubjectAll(Models.Schedule.OrgAll vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SubjectAll", new { searchText = vm.SearchText, yearId = vm.YearId, }));
        }

        public ActionResult SubjectDetail(int subjectId, int yearId)
        {
            var vm = new Models.Schedule.SubjectAll();
            vm.WeekList = Basis.Controllers.WeekController.SelectList();
            vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
            vm.SubjectList = Areas.Course.Controllers.SubjectController.SelectList();
            vm.SubjectInfo = Areas.Course.Controllers.SubjectController.SelectInfo(subjectId);
            vm.OrgScheduleList = OrgScheduleController.GetAll(vm.YearId).Where(t => t.SubjectId == subjectId).ToList();

            return View(vm);
        }

        public ActionResult SubjectExport(int yearId)
        {
            var file = System.IO.Path.GetTempFileName();
            var classExport = new Models.Schedule.ClassExport();

            var SubjectList = Areas.Course.Controllers.SubjectController.SelectList();
            var WeekList = Basis.Controllers.WeekController.SelectList();
            var PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
            var OrgScheduleList = OrgScheduleController.GetAll(yearId);

            string[] arrColumns = new string[WeekList.Count * PeriodList.Count + 1];
            for (int i = 0; i < arrColumns.Length; i++)
            {
                arrColumns[i] = (i + 1).ToString();
            }

            List<NPOI.SS.Util.CellRangeAddress> regions = new List<NPOI.SS.Util.CellRangeAddress>();
            DataTable dt = Code.Common.ArrayToDataTable(arrColumns);

            string[] arrWeek = new string[WeekList.Count * PeriodList.Count + 1];
            string[] arrWeekId = new string[WeekList.Count * PeriodList.Count + 1];
            string[] arrPeriod = new string[WeekList.Count * PeriodList.Count + 1];
            string[] arrPeriodId = new string[WeekList.Count * PeriodList.Count + 1];
            for (int i = 0; i < WeekList.Count; i++)
            {
                for (int j = 0; j < PeriodList.Count; j++)
                {
                    arrWeek[i * PeriodList.Count + j + 1] = WeekList[i].Text;
                    arrWeekId[i * PeriodList.Count + j + 1] = WeekList[i].Value;
                    arrPeriod[i * PeriodList.Count + j + 1] = PeriodList[j].Text;
                    arrPeriodId[i * PeriodList.Count + j + 1] = PeriodList[j].Value;
                }

                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, i * PeriodList.Count + 1, (i + 1) * PeriodList.Count));
            }
            dt.Rows.Add(arrWeekId);
            dt.Rows.Add(arrPeriodId);
            dt.Rows.Add(arrWeek);
            dt.Rows.Add(arrPeriod);

            for (int i = 0; i < SubjectList.Count; i++)
            {
                string[] arrTemp = new string[WeekList.Count * PeriodList.Count + 1];
                arrTemp[0] = SubjectList[i].Text;

                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    var orgSchedule = OrgScheduleList.Where(d => d.SubjectId == Convert.ToInt32(SubjectList[i].Value)
                          && d.WeekId == Convert.ToInt32(dt.Rows[0][j])
                          && d.PeriodId == Convert.ToInt32(dt.Rows[1][j])).FirstOrDefault();

                    if (orgSchedule != null)
                    {
                        arrTemp[j] = orgSchedule.Subject;
                        var orgTeacher = OrgTeacherController.GetTeacherByOrgId(orgSchedule.OrgId);
                        if (orgTeacher != null)
                        {
                            arrTemp[j] += "\n" + orgTeacher.TeacherName;
                        }
                    }
                }
                dt.Rows.Add(arrTemp);
            }
            dt.Rows.RemoveAt(1);
            dt.Rows.RemoveAt(0);

            dt.Rows[0][0] = "科目";
            regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));

            Code.NpoiHelper.DataTableToExcel(file, dt, false, regions, "科目课表");

            return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
        }
    }
}