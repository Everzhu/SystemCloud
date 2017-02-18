using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Attendance.Controllers
{
    public class AttendanceReportController : Controller
    {
        public ActionResult ClassList()
        {
            var vm = new Models.AttendanceReport.ClassList();
            using (var db = new XkSystem.Models.DbContext())
            {
                //开始日期
                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString(XkSystem.Code.Common.StringToDate);
                }
                //结束日期
                if (string.IsNullOrEmpty(vm.DateSearchTo))
                {
                    vm.DateSearchTo = DateTime.Now.ToString(XkSystem.Code.Common.StringToDate);
                }

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);
                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                //考勤类型
                vm.AttendanceTypeList = AttendanceTypeController.SelectAbnormalList();

                var tbSurveryDataList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                        join m in db.Table<Entity.tbAttendance>() on p.tbStudent.Id equals m.tbStudent.Id
                                        where m.AttendanceDate >= fromDate && m.AttendanceDate < toDate
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && m.tbAttendanceType.IsDeleted == false
                                        && m.tbOrg.IsDeleted == false
                                        && m.tbPeriod.IsDeleted == false
                                        select new
                                        {
                                            OrgId = m.tbOrg.Id,
                                            OrgName = m.tbOrg.OrgName,
                                            OrgNo = m.tbOrg.No,
                                            StudentId = m.tbStudent.Id,
                                            StudentCode = m.tbStudent.StudentCode,
                                            StudentName = m.tbStudent.StudentName,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            ClassNo = p.tbClass.No,
                                            AttendanceTypeId = m.tbAttendanceType.Id,
                                            AttendanceTypeName = m.tbAttendanceType.AttendanceTypeName,
                                            AttendanceTypeValue = m.tbAttendanceType.AttendanceValue,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            GradeNo = p.tbClass.tbGrade.No
                                        };

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => vm.SearchText.Contains(d.ClassName) || vm.SearchText.Contains(d.GradeName));
                }

                vm.AttendanceClassList = (from p in tbSurveryDataList
                                          group p by new
                                          {
                                              gradeId = p.GradeId,
                                              gradeName = p.GradeName,
                                              gradeNo = p.GradeNo,
                                              classId = p.ClassId,
                                              className = p.ClassName,
                                              classNo = p.ClassNo,
                                              attendanceTypeId = p.AttendanceTypeId,
                                              attendanceTypeName = p.AttendanceTypeName
                                          } into g
                                          select new Dto.AttendanceReport.ClassList
                                          {
                                              GradeId = g.Key.gradeId,
                                              GradeName = g.Key.gradeName,
                                              GradeNo = g.Key.gradeNo,
                                              ClassId = g.Key.classId,
                                              ClassName = g.Key.className,
                                              ClassNo = g.Key.classNo,
                                              AttendanceTypeId = g.Key.attendanceTypeId,
                                              AttendanceTypeName = g.Key.attendanceTypeName,
                                              AttendanceTypeCount = g.Count(),
                                              AttendanceTypeSum = g.Select(d => d.AttendanceTypeValue).Sum(),
                                              AttendanceTypeAvg = g.Select(d => d.AttendanceTypeValue).Average()
                                          }).Distinct().ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassList(Models.AttendanceReport.ClassList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassList", new
            {
                SearchText = vm.SearchText,
                DateSearchFrom = vm.DateSearchFrom,
                DateSearchTo = vm.DateSearchTo
            }));
        }

        public ActionResult ClassListExport()
        {
            var vm = new Models.AttendanceReport.ClassList();
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 准备数据
                //开始日期
                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString(XkSystem.Code.Common.StringToDate);
                }
                //结束日期
                if (string.IsNullOrEmpty(vm.DateSearchTo))
                {
                    vm.DateSearchTo = DateTime.Now.ToString(XkSystem.Code.Common.StringToDate);
                }

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);
                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                //考勤类型
                vm.AttendanceTypeList = AttendanceTypeController.SelectAbnormalList();

                var tbSurveryDataList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                        join m in db.Table<Entity.tbAttendance>() on p.tbStudent.Id equals m.tbStudent.Id
                                        where m.AttendanceDate >= fromDate && m.AttendanceDate < toDate
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && m.tbAttendanceType.IsDeleted == false
                                        && m.tbOrg.IsDeleted == false
                                        && m.tbPeriod.IsDeleted == false
                                        select new
                                        {
                                            OrgId = m.tbOrg.Id,
                                            OrgName = m.tbOrg.OrgName,
                                            OrgNo = m.tbOrg.No,
                                            StudentId = m.tbStudent.Id,
                                            StudentCode = m.tbStudent.StudentCode,
                                            StudentName = m.tbStudent.StudentName,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            ClassNo = p.tbClass.No,
                                            AttendanceTypeId = m.tbAttendanceType.Id,
                                            AttendanceTypeName = m.tbAttendanceType.AttendanceTypeName,
                                            AttendanceTypeValue = m.tbAttendanceType.AttendanceValue,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            GradeNo = p.tbClass.tbGrade.No
                                        };

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => vm.SearchText.Contains(d.ClassName) || vm.SearchText.Contains(d.GradeName));
                }

                vm.AttendanceClassList = (from p in tbSurveryDataList
                                          group p by new
                                          {
                                              gradeId = p.GradeId,
                                              gradeName = p.GradeName,
                                              gradeNo = p.GradeNo,
                                              classId = p.ClassId,
                                              className = p.ClassName,
                                              classNo = p.ClassNo,
                                              attendanceTypeId = p.AttendanceTypeId,
                                              attendanceTypeName = p.AttendanceTypeName
                                          } into g
                                          select new Dto.AttendanceReport.ClassList
                                          {
                                              GradeId = g.Key.gradeId,
                                              GradeName = g.Key.gradeName,
                                              GradeNo = g.Key.gradeNo,
                                              ClassId = g.Key.classId,
                                              ClassName = g.Key.className,
                                              ClassNo = g.Key.classNo,
                                              AttendanceTypeId = g.Key.attendanceTypeId,
                                              AttendanceTypeName = g.Key.attendanceTypeName,
                                              AttendanceTypeCount = g.Count(),
                                              AttendanceTypeSum = g.Select(d => d.AttendanceTypeValue).Sum(),
                                              AttendanceTypeAvg = g.Select(d => d.AttendanceTypeValue).Average()
                                          }).Distinct().ToList();
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();
                #region 全校考勤
                int surveyOptionCount = vm.AttendanceTypeList.Count();
                var allColumnLength = surveyOptionCount + 1;
                var arrColumns = new string[allColumnLength];

                for (int i = 0; i < arrColumns.Length; i++)
                {
                    arrColumns[i] = (i + 1).ToString();
                }

                var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                sheet.isColumnWritten = false;
                sheet.isWriteHeader = true;
                sheet.strHeaderText = "全校考勤";
                //开始表格
                var dt = Code.Common.ArrayToDataTable(arrColumns);
                var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                #region 增加标题
                var arrAttendanceTypeID = new string[allColumnLength];
                var arrAttendanceTypeName = new string[allColumnLength];
                var index = 0;
                for (int i = 0; i < surveyOptionCount; i++)
                {
                    index++;
                    arrAttendanceTypeID[index] = vm.AttendanceTypeList[i].Value.ToString();
                    arrAttendanceTypeName[index] = vm.AttendanceTypeList[i].Text.ToString();
                }
                dt.Rows.Add(arrAttendanceTypeName);
                #endregion
                #region DataList
                string[] arrSurveyData = new string[allColumnLength];
                arrSurveyData[0] = "异常人数";
                var indexR = 1;
                foreach (var type in vm.AttendanceTypeList)
                {
                    var typeCount = vm.AttendanceClassList.Where(d => d.AttendanceTypeId == type.Value.ConvertToInt()).Count();
                    if (typeCount > decimal.Zero)
                    {
                        arrSurveyData[indexR] = typeCount.ToString();
                    }
                    else
                    {
                        arrSurveyData[indexR] = "";
                    }
                    indexR++;
                }
                dt.Rows.Add(arrSurveyData);
                #endregion
                dt.Rows[0][0] = "考勤类型";
                sheet.data = dt;
                sheet.regions = regions;
                sheetList.Add(sheet);
                #endregion
                #region 班级考勤
                var sheetClass = new Code.NpoiHelper.DataTableToExcelPram();
                sheetClass.isColumnWritten = false;
                sheetClass.isWriteHeader = true;
                sheetClass.strHeaderText = "班级考勤";
                //开始表格
                var dtClass = Code.Common.ArrayToDataTable(arrColumns);
                var regionsClass = new List<NPOI.SS.Util.CellRangeAddress>();
                #region 增加标题
                var arrAttendanceTypeIDClass = new string[allColumnLength];
                var arrAttendanceTypeNameClass = new string[allColumnLength];
                var indexClass = 0;
                for (int i = 0; i < surveyOptionCount; i++)
                {
                    indexClass++;
                    arrAttendanceTypeIDClass[indexClass] = vm.AttendanceTypeList[i].Value.ToString();
                    arrAttendanceTypeNameClass[indexClass] = vm.AttendanceTypeList[i].Text.ToString();
                }
                dtClass.Rows.Add(arrAttendanceTypeNameClass);
                #endregion
                #region DataList
                foreach (var a in vm.AttendanceClassList.GroupBy(d => new { d.ClassId, d.ClassName, d.ClassNo }).Select(g => new { g.Key.ClassId, g.Key.ClassName, g.Key.ClassNo }).OrderBy(g => g.ClassNo))
                {
                    string[] arrSurveyDataClass = new string[allColumnLength];
                    arrSurveyDataClass[0] = a.ClassName;
                    var indexRClass = 1;
                    foreach (var type in vm.AttendanceTypeList)
                    {
                        var typeCountAll = vm.AttendanceClassList.Where(d => d.AttendanceTypeId == type.Value.ConvertToInt() && d.ClassId == a.ClassId).Count();
                        if (typeCountAll > decimal.Zero)
                        {
                            arrSurveyDataClass[indexRClass] = typeCountAll.ToString();
                        }
                        else
                        {
                            arrSurveyDataClass[indexRClass] = "";
                        }
                        indexRClass++;
                    }
                    dtClass.Rows.Add(arrSurveyDataClass);
                }
                #endregion
                dtClass.Rows[0][0] = "行政班级";
                sheetClass.data = dtClass;
                sheetClass.regions = regionsClass;
                sheetList.Add(sheetClass);
                #endregion
                Code.NpoiHelper.DataTableToExcel(file, sheetList);
                if (!String.IsNullOrEmpty(file))
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