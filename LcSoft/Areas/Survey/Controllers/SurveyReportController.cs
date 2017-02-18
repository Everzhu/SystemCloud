using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;

namespace XkSystem.Areas.Survey.Controllers
{
    public class SurveyReportController : Controller
    {
        public ActionResult Subject()
        {
            var vm = new Models.SurveyReport.Subject();

            vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
            if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
            {
                vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
            }

            vm.SurveyGroupList = Areas.Survey.Controllers.SurveyGroupController.SelectList(vm.SurveyId);
            if (vm.SurveyGroupId == 0 && vm.SurveyGroupList.Count > 0)
            {
                vm.SurveyGroupId = vm.SurveyGroupList.FirstOrDefault().Value.ConvertToInt();
            }
            else if (vm.SurveyGroupId != 0
                && vm.SurveyGroupList.Count > 0
                && vm.SurveyGroupList.Select(d => d.Value).Contains(vm.SurveyGroupId.ToString()) == false)
            {
                vm.SurveyGroupId = vm.SurveyGroupList.FirstOrDefault().Value.ConvertToInt();
            }

            var tbSurveyGroup = Areas.Survey.Controllers.SurveyGroupController.SelectInfo(vm.SurveyGroupId ?? 0);
            if (tbSurveyGroup != null && tbSurveyGroup.IsOrg)
            {
                vm.SurveyCourseList = Areas.Survey.Controllers.SurveyCourseController.SelectList(vm.SurveyGroupId ?? 0);
            }

            if (vm.SurveyCourseId != 0 && !vm.SurveyCourseList.Select(d => d.Value).Contains(vm.SurveyCourseId.ToString()))
            {
                vm.SurveyCourseId = 0;
            }

            vm.SurveyItemList = Areas.Survey.Controllers.SurveyItemController.SelectItemList(vm.SurveyGroupId ?? 0);
            vm.SurveyOptionList = Areas.Survey.Controllers.SurveyOptionController.SelectInfoListBySurveyGroup(vm.SurveyGroupId ?? 0);
            vm.SurveyTotalTeacherList = Areas.Survey.Controllers.SurveyDataController.SelectTeacherInfoListBySurveyGroup(vm.SurveyGroupId ?? 0);
            vm.SurveyTotalList = Areas.Survey.Controllers.SurveyDataController.SelectInfoListBySurveyCourse(vm.SurveyCourseId ?? 0);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Subject(Models.SurveyReport.Subject vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("Subject", new
            {
                searchText = vm.SearchText,
                surveyId = vm.SurveyId,
                surveyGroupId = vm.SurveyGroupId,
                surveyCourseId = vm.SurveyCourseId
            }));
        }

        public ActionResult Teacher(int surveyTeacherId, int surveyId, int surveyGroupId, int surveyCourseId)
        {
            var vm = new Models.SurveyReport.List();
            vm.TeacherList = XkSystem.Areas.Teacher.Controllers.TeacherController.GetTeacherById(surveyTeacherId);

            var tbSurveyGroup = Areas.Survey.Controllers.SurveyGroupController.SelectInfo(vm.SurveyGroupId ?? 0);
            if (tbSurveyGroup != null && tbSurveyGroup.IsOrg == false)
            {
                vm.ClassList = Areas.Survey.Controllers.SurveyClassController.SelectClassInfoListByTeacher(surveyId, surveyTeacherId);
            }
            else
            {
                vm.OrgList = Areas.Course.Controllers.OrgTeacherController.SelectOrgListByTeacher(surveyTeacherId);
            }

            vm.SurveyItemList = Areas.Survey.Controllers.SurveyItemController.SelectList(vm.SurveyGroupId ?? 0);
            vm.SurveyOptionList = Areas.Survey.Controllers.SurveyOptionController.SelectInfoListBySurveyGroup(vm.SurveyGroupId ?? 0);

            return View(vm);
        }

        public ActionResult SurveyReportExport(int surveyId = 0, int surveyGroupId = 0, int surveyCourseId = 0)
        {
            var vm = new Models.SurveyReport.Export();
            var surveyList = Areas.Survey.Controllers.SurveyController.SelectList();
            var surveyItemList = Areas.Survey.Controllers.SurveyItemController.SelectItemList(vm.SurveyGroupId);
            if (vm.SurveyId == 0 && surveyList.Count > 0)
            {
                vm.SurveyId = surveyList.FirstOrDefault().Value.ConvertToInt();
            }

            var surveyGroupList = Areas.Survey.Controllers.SurveyGroupController.SelectList(vm.SurveyId);
            if (vm.SurveyGroupId == 0 && surveyGroupList.Count > 0)
            {
                vm.SurveyGroupId = surveyGroupList.FirstOrDefault().Value.ConvertToInt();
            }
            else if (vm.SurveyGroupId != 0 && surveyGroupList.Count > 0 && surveyGroupList.Select(d => d.Value).Contains(vm.SurveyGroupId.ToString()) == false)
            {
                vm.SurveyGroupId = surveyGroupList.FirstOrDefault().Value.ConvertToInt();
            }

            var surveyCoursetList = Areas.Survey.Controllers.SurveyCourseController.SelectList(vm.SurveyGroupId);
            if (vm.SurveyCourseId != 0 && !surveyCoursetList.Select(d => d.Value).Contains(vm.SurveyCourseId.ToString()))
            {
                vm.SurveyCourseId = 0;
            }

            var surveyOptionList = Areas.Survey.Controllers.SurveyOptionController.SelectInfoListBySurveyGroup(surveyGroupId);
            int surveyOptionCount = 1;
            surveyItemList.ToList().ForEach(surveyItem =>
            {
                surveyOptionCount += surveyOptionList.Where(o => o.SurveyItemId == surveyItem.Id && surveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox).Count();
            });

            surveyOptionCount += surveyItemList.Where(d => d.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox).Count();

            var file = System.IO.Path.GetTempFileName();
            var arrColumns = new string[surveyOptionCount];
            for (int i = 0; i < arrColumns.Length; i++)
            {
                arrColumns[i] = (i + 1).ToString();
            }

            var dt = Code.Common.ArrayToDataTable(arrColumns);
            var regions = new List<NPOI.SS.Util.CellRangeAddress>();

            var arrSurveyItemID = new string[surveyOptionCount];
            var arrSurveyItemName = new string[surveyOptionCount];
            var arrSurveyOptionID = new string[surveyOptionCount];
            var arrSurveyOptionName = new string[surveyOptionCount];
            var index = 0;
            for (int i = 0; i < surveyItemList.Count(); i++)
            {
                if (surveyItemList[i].SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox)
                {
                    index++;
                    arrSurveyItemID[index] = surveyItemList[i].Id.ConvertToString();
                    arrSurveyItemName[index] = surveyItemList[i].SurveyItemName;
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index, index));
                }
                else
                {
                    var surveyOptions = surveyOptionList.Where(o => o.SurveyItemId == surveyItemList[i].Id && surveyItemList[i].SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox).ToList();
                    for (int j = 0; j < surveyOptions.Count(); j++)
                    {
                        index++;
                        arrSurveyItemID[index] = surveyItemList[i].Id.ConvertToString();
                        arrSurveyItemName[index] = surveyItemList[i].SurveyItemName;
                        arrSurveyOptionID[index] = surveyOptions[j].Id.ToString();
                        arrSurveyOptionName[index] = surveyOptions[j].OptionName;
                    }
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, index - surveyOptions.Count() + 1, index));
                }

            }
            dt.Rows.Add(arrSurveyItemName);
            dt.Rows.Add(arrSurveyOptionName);

            vm.SurveyTotalList = Areas.Survey.Controllers.SurveyDataController.SelectInfoListBySurveyCourse(vm.SurveyCourseId);
            var surveyDataTeacherList = Areas.Survey.Controllers.SurveyDataController.SelectTeacherInfoListBySurveyGroup(surveyGroupId);
            surveyDataTeacherList.ForEach(teacher =>
            {
                string[] arrSurveyData = new string[surveyOptionCount];
                arrSurveyData[0] = teacher.TeacherName;
                var i = 1;
                foreach (var item in surveyItemList)
                {
                    if (item.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox)
                    {
                        var records = vm.SurveyTotalList.Where(d => d.TeacherId == teacher.Id && d.SurveyItemId == item.Id).Select(d => d.TotalCount).FirstOrDefault();
                        var total = vm.SurveyTotalList.Where(d => d.TeacherId == teacher.Id && d.SurveyItemId == item.Id).Select(d => d.TotalCount).Sum();
                        if (total == 0 || records == 0)
                        {
                            arrSurveyData[i] = "0";
                        }
                        else
                        {
                            arrSurveyData[i] = Math.Round(Convert.ToDecimal(records * 100) / total, 2).ToString() + " %";
                        }
                    }
                    else
                    {
                        foreach (var option in surveyOptionList.Where(o => o.SurveyItemId == item.Id))
                        {
                            var records = vm.SurveyTotalList.Where(d => d.TeacherId == teacher.Id && d.SurveyOptionId == option.Id).Select(d => d.TotalCount).FirstOrDefault();
                            var total = vm.SurveyTotalList.Where(d => d.TeacherId == teacher.Id && d.SurveyItemId == item.Id).Select(d => d.TotalCount).Sum();
                            if (total == 0 || records == 0)
                            {
                                arrSurveyData[i] = "0";
                            }
                            else
                            {
                                arrSurveyData[i] = Math.Round(Convert.ToDecimal(records * 100) / total, 2).ToString() + " %";
                            }
                            i++;
                        }
                    }
                }
                dt.Rows.Add(arrSurveyData);
            });


            dt.Rows[0][0] = "教师姓名";
            regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));

            Code.NpoiHelper.DataTableToExcel(file, dt, false, regions, "评价统计");

            if (!String.IsNullOrEmpty(file))
            {
                return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
            }
            else
            {
                return View();
            }
        }

        public ActionResult ClassList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyReport.ClassList();
                vm.SurveyList = SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count() > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = SurveyGroupController.SelectInfoList(vm.SurveyId);

                #region 任课教师模式
                var tbCourseIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                   where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                   && p.tbSurveyGroup.IsDeleted == false
                                   && p.tbCourse.IsDeleted == false
                                   && p.tbSurveyGroup.IsOrg
                                   select p.tbCourse.Id).Distinct().ToList();

                var year = (from p in db.Table<Entity.tbSurvey>()
                            where p.Id == vm.SurveyId
                            select new
                            {
                                YearId = p.tbYear.Id,
                            }).FirstOrDefault();

                var tbOrgList = from p in db.Table<Course.Entity.tbOrgTeacher>()
                                where p.tbOrg.IsDeleted == false
                                    && tbCourseIds.Contains(p.tbOrg.tbCourse.Id)
                                    && p.tbOrg.tbYear.Id == year.YearId
                                    && p.tbOrg.tbCourse.IsDeleted == false
                                    && p.tbTeacher.IsDeleted == false
                                select new Dto.SurveyReport.ClassList
                                {
                                    OrgId = p.tbOrg.Id,
                                    OrgName = p.tbOrg.OrgName,
                                    TeacherId = p.tbTeacher.Id,
                                    TeacherCode = p.tbTeacher.TeacherCode,
                                    ClassTeacherName = p.tbTeacher.TeacherName,
                                    ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                    IsClass = p.tbOrg.IsClass
                                };

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbOrgList = tbOrgList.Where(d => (d.OrgName.Contains(vm.SearchText) || d.ClassTeacherName.Contains(vm.SearchText)));
                }

                vm.SurveyReportOrgList = (from p in tbOrgList
                                          select p).Distinct().ToList();

                var tbOrgStudentList = new List<Dto.SurveyReport.StudentList>();
                foreach (var org in vm.SurveyReportOrgList)
                {
                    if (org.IsClass)
                    {
                        var tbStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                         where p.tbClass.Id == org.ClassId
                                             && p.tbClass.IsDeleted == false
                                             && p.tbStudent.IsDeleted == false
                                         select new Dto.SurveyReport.StudentList
                                         {
                                             OrgId = org.OrgId,
                                             StudentId = p.tbStudent.Id
                                         }).ToList();

                        tbOrgStudentList.AddRange(tbStudent);
                    }
                    else
                    {
                        var tbStudent = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                         where p.tbOrg.Id == org.OrgId
                                             && p.tbStudent.IsDeleted == false
                                         select new Dto.SurveyReport.StudentList
                                         {
                                             OrgId = org.OrgId,
                                             StudentId = p.tbStudent.Id
                                         }).ToList();
                        tbOrgStudentList.AddRange(tbStudent);
                    }
                }

                var tbSurveyDataOrg = (from p in db.Table<Entity.tbSurveyData>()
                                       where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                           && p.tbOrg.IsDeleted == false
                                           && p.tbStudent.IsDeleted == false
                                           && p.tbSurveyItem.IsDeleted == false
                                           && p.tbSurveyOption.IsDeleted == false
                                           && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                           && p.tbTeacher.IsDeleted == false
                                       group p by new { orgId = p.tbOrg.Id, studentId = p.tbStudent.Id, teacherId = p.tbTeacher.Id } into g
                                       select new
                                       {
                                           OrgId = g.Key.orgId,
                                           StudentId = g.Key.studentId,
                                           TeacherId = g.Key.teacherId
                                       }).Distinct().ToList();

                foreach (var item in vm.SurveyReportOrgList)
                {
                    item.StudentAllCount = tbOrgStudentList.Where(d => d.OrgId == item.OrgId).Count();
                    var legOrg = tbOrgStudentList.Where(d => d.OrgId == item.OrgId).Select(d => d.StudentId).ToList();
                    var dataOrg = tbSurveyDataOrg.Where(d => d.OrgId == item.OrgId && d.TeacherId == item.TeacherId).Select(d => d.StudentId).ToList();
                    item.SelectedCount = legOrg.Intersect(dataOrg).ToList().Count();
                    item.UnSelectedCount = item.StudentAllCount - item.SelectedCount;
                }
                #endregion

                #region 班主任模式
                //班主任模式
                var tbClassList = from p in db.Table<Entity.tbSurveyClass>()
                                  join q in db.Table<Entity.tbSurveyGroup>() on p.tbSurvey.Id equals q.tbSurvey.Id
                                  join m in db.Table<Basis.Entity.tbClassTeacher>() on p.tbClass.Id equals m.tbTeacher.Id
                                  where p.tbSurvey.Id == vm.SurveyId
                                      && q.IsOrg == false
                                      && p.tbClass.IsDeleted == false
                                      && m.tbTeacher.IsDeleted == false
                                  select new Dto.SurveyReport.ClassList
                                  {
                                      ClassId = p.tbClass.Id,
                                      ClassName = p.tbClass.ClassName,
                                      TeacherId = m.tbTeacher.Id,
                                      TeacherCode = m.tbTeacher.TeacherCode,
                                      TeacherName = m.tbTeacher.TeacherName
                                  };

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbClassList = tbClassList.Where(d => (d.ClassName.Contains(vm.SearchText) || d.TeacherName.Contains(vm.SearchText)));
                }

                vm.SurveyReportClassList = (from p in tbClassList
                                            select p).Distinct().ToList();

                var tbClassStudentList = (from p in db.Table<Entity.tbSurveyClass>()
                                          join q in db.Table<Entity.tbSurveyGroup>() on p.tbSurvey.Id equals q.tbSurvey.Id
                                          join s in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals s.tbClass.Id
                                          where p.tbSurvey.Id == vm.SurveyId
                                              && p.tbClass.IsDeleted == false
                                              && s.tbStudent.IsDeleted == false
                                          select new
                                          {
                                              ClassId = p.tbClass.Id,
                                              StudentId = s.tbStudent.Id
                                          }).Distinct().ToList();

                var tbSurveyData = (from p in db.Table<Entity.tbSurveyData>()
                                    where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbSurveyItem.IsDeleted == false
                                        && p.tbSurveyOption.IsDeleted == false
                                        && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                        && p.tbTeacher.IsDeleted == false
                                    group p by new { classsId = p.tbClass.Id, studentId = p.tbStudent.Id, teacherId = p.tbTeacher.Id } into g
                                    select new
                                    {
                                        ClassId = g.Key.classsId,
                                        StudentId = g.Key.studentId,
                                        TeacherId = g.Key.teacherId
                                    }).Distinct().ToList();

                foreach (var item in vm.SurveyReportClassList)
                {
                    item.StudentAllCount = tbClassStudentList.Where(d => d.ClassId == item.ClassId).Count();
                    item.SelectedCount = tbSurveyData.Where(d => d.ClassId == item.ClassId && d.TeacherId == item.TeacherId).Count();
                    item.UnSelectedCount = item.StudentAllCount - item.SelectedCount;
                }
                #endregion
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassList(Models.SurveyReport.ClassList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassList", new
            {
                SurveyId = vm.SurveyId,
                SearchText = vm.SearchText
            }));
        }

        public ActionResult ExportClassList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyReport.ClassList();

                var file = System.IO.Path.GetTempFileName();

                #region 班主任模式
                //班主任模式
                var tbClassList = from p in db.Table<Entity.tbSurveyClass>()
                                  join q in db.Table<Entity.tbSurveyGroup>() on p.tbSurvey.Id equals q.tbSurvey.Id
                                  join m in db.Table<Basis.Entity.tbClassTeacher>() on p.tbClass.Id equals m.tbTeacher.Id
                                  where p.tbSurvey.Id == vm.SurveyId
                                      && q.IsOrg == false
                                      && p.tbClass.IsDeleted == false
                                      && m.tbTeacher.IsDeleted == false
                                  select new Dto.SurveyReport.ClassList
                                  {
                                      ClassId = p.tbClass.Id,
                                      ClassName = p.tbClass.ClassName,
                                      TeacherId = m.tbTeacher.Id,
                                      TeacherCode = m.tbTeacher.TeacherCode,
                                      TeacherName = m.tbTeacher.TeacherName
                                  };

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbClassList = tbClassList.Where(d => (d.ClassName.Contains(vm.SearchText) || d.TeacherName.Contains(vm.SearchText)));
                }

                vm.SurveyReportClassList = (from p in tbClassList
                                            select p).Distinct().ToList();

                var tbClassStudentList = (from p in db.Table<Entity.tbSurveyClass>()
                                          join q in db.Table<Entity.tbSurveyGroup>() on p.tbSurvey.Id equals q.tbSurvey.Id
                                          join s in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals s.tbClass.Id
                                          where p.tbSurvey.Id == vm.SurveyId
                                              && p.tbClass.IsDeleted == false
                                              && s.tbStudent.IsDeleted == false
                                          select new
                                          {
                                              ClassId = p.tbClass.Id,
                                              StudentId = s.tbStudent.Id
                                          }).Distinct().ToList();

                var tbSurveyData = (from p in db.Table<Entity.tbSurveyData>()
                                    where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbSurveyItem.IsDeleted == false
                                        && p.tbSurveyOption.IsDeleted == false
                                        && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                        && p.tbTeacher.IsDeleted == false
                                    group p by new { classsId = p.tbClass.Id, studentId = p.tbStudent.Id, teacherId = p.tbTeacher.Id } into g
                                    select new
                                    {
                                        ClassId = g.Key.classsId,
                                        StudentId = g.Key.studentId,
                                        TeacherId = g.Key.teacherId
                                    }).Distinct().ToList();

                foreach (var item in vm.SurveyReportClassList)
                {
                    item.StudentAllCount = tbClassStudentList.Where(d => d.ClassId == item.ClassId).Count();
                    item.SelectedCount = tbSurveyData.Where(d => d.ClassId == item.ClassId && d.TeacherId == item.TeacherId).Count();
                    item.UnSelectedCount = item.StudentAllCount - item.SelectedCount;
                }
                #endregion

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("序号"),
                        new System.Data.DataColumn("行政班"),
                        new System.Data.DataColumn("班主任"),
                        new System.Data.DataColumn("全部人数"),
                        new System.Data.DataColumn("已评人数"),
                        new System.Data.DataColumn("未评人数")
                    });

                var index = 0;
                foreach (var a in vm.SurveyReportClassList)
                {
                    index++;
                    var dr = dt.NewRow();
                    dr["序号"] = index.ToString();
                    dr["行政班"] = a.ClassName;
                    dr["班主任"] = a.TeacherName;
                    dr["全部人数"] = a.StudentAllCount;
                    dr["已评人数"] = a.SelectedCount;
                    dr["未评人数"] = a.UnSelectedCount;
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

        public ActionResult ExportOrgList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyReport.ClassList();

                var file = System.IO.Path.GetTempFileName();

                #region 任课教师模式
                var tbCourseIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                   where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                   select p.tbCourse.Id).Distinct().ToList();

                var year = (from p in db.Table<Entity.tbSurvey>()
                            where p.Id == vm.SurveyId
                            select new
                            {
                                YearId = p.tbYear.Id,
                            }).FirstOrDefault();

                var tbOrgList = from p in db.Table<Course.Entity.tbOrgTeacher>()
                                where p.tbOrg.IsDeleted == false
                                    && tbCourseIds.Contains(p.tbOrg.tbCourse.Id)
                                    && p.tbOrg.tbYear.Id == year.YearId
                                    && p.tbOrg.tbCourse.IsDeleted == false
                                    && p.tbTeacher.IsDeleted == false
                                select new Dto.SurveyReport.ClassList
                                {
                                    OrgId = p.tbOrg.Id,
                                    OrgName = p.tbOrg.OrgName,
                                    TeacherId = p.tbTeacher.Id,
                                    TeacherCode = p.tbTeacher.TeacherCode,
                                    ClassTeacherName = p.tbTeacher.TeacherName,
                                    ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                    IsClass = p.tbOrg.IsClass
                                };

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbOrgList = tbOrgList.Where(d => (d.OrgName.Contains(vm.SearchText) || d.ClassTeacherName.Contains(vm.SearchText)));
                }

                vm.SurveyReportOrgList = (from p in tbOrgList
                                          select p).Distinct().ToList();

                var tbOrgStudentList = new List<Dto.SurveyReport.StudentList>();
                foreach (var org in vm.SurveyReportOrgList)
                {
                    if (org.IsClass)
                    {
                        var tbStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                         where p.tbClass.Id == org.ClassId
                                             && p.tbClass.IsDeleted == false
                                             && p.tbStudent.IsDeleted == false
                                         select new Dto.SurveyReport.StudentList
                                         {
                                             OrgId = org.OrgId,
                                             StudentId = p.tbStudent.Id
                                         }).ToList();

                        tbOrgStudentList.AddRange(tbStudent);
                    }
                    else
                    {
                        var tbStudent = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                         where p.tbOrg.Id == org.OrgId
                                             && p.tbOrg.IsDeleted == false
                                             && p.tbStudent.IsDeleted == false
                                         select new Dto.SurveyReport.StudentList
                                         {
                                             OrgId = org.OrgId,
                                             StudentId = p.tbStudent.Id
                                         }).ToList();
                        tbOrgStudentList.AddRange(tbStudent);
                    }
                }

                var tbSurveyDataOrg = (from p in db.Table<Entity.tbSurveyData>()
                                       where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                           && p.tbOrg.IsDeleted == false
                                           && p.tbStudent.IsDeleted == false
                                           && p.tbSurveyItem.IsDeleted == false
                                           && p.tbSurveyOption.IsDeleted == false
                                           && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                           && p.tbTeacher.IsDeleted == false
                                       group p by new { orgId = p.tbOrg.Id, studentId = p.tbStudent.Id, teacherId = p.tbTeacher.Id } into g
                                       select new
                                       {
                                           OrgId = g.Key.orgId,
                                           StudentId = g.Key.studentId,
                                           TeacherId = g.Key.teacherId
                                       }).Distinct().ToList();

                foreach (var item in vm.SurveyReportOrgList)
                {
                    item.StudentAllCount = tbOrgStudentList.Where(d => d.OrgId == item.OrgId).Count();
                    var legOrg = tbOrgStudentList.Where(d => d.OrgId == item.OrgId).Select(d => d.StudentId).ToList();
                    var dataOrg = tbSurveyDataOrg.Where(d => d.OrgId == item.OrgId && d.TeacherId == item.TeacherId).Select(d => d.StudentId).ToList();
                    item.SelectedCount = legOrg.Intersect(dataOrg).ToList().Count();
                    item.UnSelectedCount = item.StudentAllCount - item.SelectedCount;
                }
                #endregion

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("教学班"),
                        new System.Data.DataColumn("任课教师"),
                        new System.Data.DataColumn("全部人数"),
                        new System.Data.DataColumn("已评人数"),
                        new System.Data.DataColumn("未评人数")
                    });

                foreach (var a in vm.SurveyReportOrgList)
                {
                    var dr = dt.NewRow();
                    dr["教学班"] = a.OrgName;
                    dr["任课教师"] = a.ClassTeacherName;
                    dr["全部人数"] = a.StudentAllCount;
                    dr["已评人数"] = a.SelectedCount;
                    dr["未评人数"] = a.UnSelectedCount;
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

        public ActionResult FullList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyReport.FullList();
                if (vm.IsOrgOrClass == 0)
                {
                    #region 任课教师模式

                    var tbSurveyDataOrg = (from p in db.Table<Entity.tbSurveyData>()
                                           where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                               && p.tbOrg.Id == vm.ClassId
                                               && p.tbTeacher.Id == vm.TeacherId
                                               && p.tbOrg.IsDeleted == false
                                               && p.tbStudent.IsDeleted == false
                                               && p.tbSurveyItem.IsDeleted == false
                                               && p.tbSurveyOption.IsDeleted == false
                                               && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                               && p.tbTeacher.IsDeleted == false
                                           group p by new { orgId = p.tbOrg.Id, studentId = p.tbStudent.Id, teacherId = p.tbTeacher.Id } into g
                                           select new
                                           {
                                               OrgId = g.Key.orgId,
                                               StudentId = g.Key.studentId,
                                               TeacherId = g.Key.teacherId
                                           }).Distinct().ToList();

                    var tbOrgList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                     where p.tbOrg.Id == vm.ClassId
                                        && p.tbTeacher.IsDeleted == false
                                     select new Dto.SurveyReport.ClassList
                                     {
                                         OrgId = p.tbOrg.Id,
                                         OrgName = p.tbOrg.OrgName,
                                         TeacherId = p.tbTeacher.Id,
                                         TeacherCode = p.tbTeacher.TeacherCode,
                                         ClassTeacherName = p.tbTeacher.TeacherName,
                                         ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                         IsClass = p.tbOrg.IsClass
                                     }).FirstOrDefault();

                    vm.SurveyFullList = new List<Dto.SurveyReport.FullList>();
                    if (tbOrgList.IsClass)
                    {
                        vm.SurveyFullList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                             where p.tbClass.Id == tbOrgList.ClassId
                                                 && p.tbStudent.IsDeleted == false
                                             select new Dto.SurveyReport.FullList
                                             {
                                                 OrgId = tbOrgList.OrgId,
                                                 StudentId = p.tbStudent.Id,
                                                 StudentCode = p.tbStudent.StudentCode,
                                                 StudentName = p.tbStudent.StudentName,
                                                 IsSelected = false,
                                                 TeacherId = tbOrgList.TeacherId
                                             }).ToList();
                    }
                    else
                    {
                        vm.SurveyFullList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                             where p.tbOrg.Id == tbOrgList.OrgId
                                                 && p.tbStudent.IsDeleted == false
                                             select new Dto.SurveyReport.FullList
                                             {
                                                 OrgId = tbOrgList.OrgId,
                                                 StudentId = p.tbStudent.Id,
                                                 StudentCode = p.tbStudent.StudentCode,
                                                 StudentName = p.tbStudent.StudentName,
                                                 IsSelected = false,
                                                 TeacherId = tbOrgList.TeacherId
                                             }).ToList();
                    }

                    foreach (var item in vm.SurveyFullList)
                    {
                        if (tbSurveyDataOrg.Where(d => d.OrgId == item.OrgId && d.TeacherId == item.TeacherId && d.StudentId == item.StudentId).Count() > 0)
                        {
                            item.IsSelected = true;
                        }
                    }

                    if (vm.IsSelected == 0)
                    {
                        vm.SurveyFullList = vm.SurveyFullList.Where(d => d.IsSelected == false).ToList();
                    }
                    else
                    {
                        vm.SurveyFullList = vm.SurveyFullList.Where(d => d.IsSelected == true).ToList();
                    }
                    #endregion
                }
                else
                {
                    #region 班主任模式

                    var tbSurveyData = (from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                            && p.tbClass.Id == vm.ClassId
                                            && p.tbTeacher.Id == vm.TeacherId
                                            && p.tbClass.IsDeleted == false
                                            && p.tbStudent.IsDeleted == false
                                            && p.tbSurveyItem.IsDeleted == false
                                            && p.tbSurveyOption.IsDeleted == false
                                            && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                            && p.tbTeacher.IsDeleted == false
                                        group p by new { classsId = p.tbClass.Id, studentId = p.tbStudent.Id, teacherId = p.tbTeacher.Id } into g
                                        select new
                                        {
                                            ClassId = g.Key.classsId,
                                            StudentId = g.Key.studentId,
                                            TeacherId = g.Key.teacherId
                                        }).Distinct().ToList();

                    //班主任模式
                    var tbClassList = from p in db.Table<Entity.tbSurveyClass>()
                                      join q in db.Table<Entity.tbSurveyGroup>() on p.tbSurvey.Id equals q.tbSurvey.Id
                                      join m in db.Table<Basis.Entity.tbClassTeacher>() on p.tbClass.Id equals m.tbTeacher.Id
                                      where p.tbSurvey.Id == vm.SurveyId
                                          && q.IsOrg == false
                                          && p.tbClass.Id == vm.ClassId
                                          && m.tbTeacher.IsDeleted == false
                                      select new Dto.SurveyReport.ClassList
                                      {
                                          ClassId = p.tbClass.Id,
                                          ClassName = p.tbClass.ClassName,
                                          TeacherId = m.tbTeacher.Id,
                                          TeacherCode = m.tbTeacher.TeacherCode,
                                          TeacherName = m.tbTeacher.TeacherName
                                      };

                    vm.SurveyFullList = (from p in db.Table<Entity.tbSurveyClass>()
                                         join q in db.Table<Entity.tbSurveyGroup>() on p.tbSurvey.Id equals q.tbSurvey.Id
                                         join s in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals s.tbClass.Id
                                         where p.tbSurvey.Id == vm.SurveyId
                                             && p.tbClass.Id == vm.ClassId
                                             && s.tbStudent.IsDeleted == false
                                         select new Dto.SurveyReport.FullList
                                         {
                                             ClassId = p.tbClass.Id,
                                             StudentId = s.tbStudent.Id,
                                             StudentCode = s.tbStudent.StudentCode,
                                             StudentName = s.tbStudent.StudentName,
                                             IsSelected = false
                                         }).Distinct().ToList();

                    foreach (var item in vm.SurveyFullList)
                    {
                        if (tbSurveyData.Where(d => d.ClassId == item.ClassId && d.StudentId == item.StudentId).Count() > 0)
                        {
                            item.IsSelected = true;
                        }
                    }

                    if (vm.IsSelected == 0)
                    {
                        vm.SurveyFullList = vm.SurveyFullList.Where(d => d.IsSelected == false).ToList();
                    }
                    else
                    {
                        vm.SurveyFullList = vm.SurveyFullList.Where(d => d.IsSelected == true).ToList();
                    }

                    #endregion
                }

                return View(vm);
            }
        }
        public ActionResult Detail()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyReport.Detail();

                vm.SurveyList = SurveyController.SelectList();

                if (vm.SurveyId == 0 && vm.SurveyList.Count() > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                #region 任课教师模式
                var tbCourseIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                   where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                   select p.tbCourse.Id).Distinct().ToList();

                var year = (from p in db.Table<Entity.tbSurvey>()
                            where p.Id == vm.SurveyId
                            select new
                            {
                                YearId = p.tbYear.Id,
                            }).FirstOrDefault();

                var tbOrgList = from p in db.Table<Course.Entity.tbOrgTeacher>()
                                where tbCourseIds.Contains(p.tbOrg.tbCourse.Id)
                                    && p.tbOrg.tbYear.Id == year.YearId
                                    && p.tbOrg.tbCourse.IsDeleted == false
                                    && p.tbTeacher.IsDeleted == false
                                select new Dto.SurveyReport.Detail
                                {
                                    OrgId = p.tbOrg.Id,
                                    OrgName = p.tbOrg.OrgName,
                                    TeacherId = p.tbTeacher.Id,
                                    TeacherCode = p.tbTeacher.TeacherCode,
                                    TeacherName = p.tbTeacher.TeacherName,
                                    ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                    IsClass = p.tbOrg.IsClass
                                };

                var OrgList = (from p in tbOrgList
                               select p).Distinct().ToList();

                vm.SurveyDetailOrgList = new List<Dto.SurveyReport.Detail>();
                foreach (var org in OrgList)
                {
                    if (org.IsClass)
                    {
                        var tbStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                         where p.tbClass.Id == org.ClassId
                                             && p.tbStudent.IsDeleted == false
                                         select new Dto.SurveyReport.Detail
                                         {
                                             ClassId = org.OrgId,
                                             ClassName = org.OrgName,
                                             StudentId = p.tbStudent.Id,
                                             StudentCode = p.tbStudent.StudentCode,
                                             StudentName = p.tbStudent.StudentName,
                                             TeacherId = org.TeacherId,
                                             TeacherCode = org.TeacherCode,
                                             TeacherName = org.TeacherName,
                                             IsSelected = false
                                         }).ToList();

                        vm.SurveyDetailOrgList.AddRange(tbStudent);
                    }
                    else
                    {
                        var tbStudent = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                         where p.tbOrg.Id == org.OrgId
                                            && p.tbStudent.IsDeleted == false
                                         select new Dto.SurveyReport.Detail
                                         {
                                             ClassId = org.OrgId,
                                             ClassName = org.OrgName,
                                             StudentId = p.tbStudent.Id,
                                             StudentCode = p.tbStudent.StudentCode,
                                             StudentName = p.tbStudent.StudentName,
                                             TeacherId = org.TeacherId,
                                             TeacherCode = org.TeacherCode,
                                             TeacherName = org.TeacherName,
                                             IsSelected = false
                                         }).ToList();
                        vm.SurveyDetailOrgList.AddRange(tbStudent);
                    }
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    vm.SurveyDetailOrgList = vm.SurveyDetailOrgList.Where(d => (d.StudentCode.Contains(vm.SearchText) || d.StudentName.Contains(vm.SearchText))).ToList();
                }

                var tbSurveyDataOrg = (from p in db.Table<Entity.tbSurveyData>()
                                       where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                           && p.tbOrg.IsDeleted == false
                                           && p.tbStudent.IsDeleted == false
                                           && p.tbSurveyItem.IsDeleted == false
                                           && p.tbSurveyOption.IsDeleted == false
                                           && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                           && p.tbTeacher.IsDeleted == false
                                       group p by new { orgId = p.tbOrg.Id, studentId = p.tbStudent.Id, teacherId = p.tbTeacher.Id } into g
                                       select new
                                       {
                                           OrgId = g.Key.orgId,
                                           StudentId = g.Key.studentId,
                                           TeacherId = g.Key.teacherId
                                       }).Distinct().ToList();

                foreach (var item in vm.SurveyDetailOrgList)
                {
                    if (tbSurveyDataOrg.Where(d => d.OrgId == item.ClassId && d.StudentId == item.StudentId && d.TeacherId == item.TeacherId).Count() > 0)
                    {
                        item.IsSelected = true;
                    }
                }
                #endregion

                #region 班主任模式
                //班主任模式
                vm.SurveyDetailClassList = (from p in db.Table<Entity.tbSurveyClass>()
                                            join q in db.Table<Entity.tbSurveyGroup>() on p.tbSurvey.Id equals q.tbSurvey.Id
                                            join s in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals s.tbClass.Id
                                            join t in db.Table<Basis.Entity.tbClassTeacher>() on p.tbClass.Id equals t.tbClass.Id
                                            where p.tbSurvey.Id == vm.SurveyId
                                                && q.IsOrg == false
                                                && p.tbClass.IsDeleted == false
                                                && s.tbStudent.IsDeleted == false
                                            select new Dto.SurveyReport.Detail
                                            {
                                                ClassId = p.tbClass.Id,
                                                ClassName = p.tbClass.ClassName,
                                                TeacherId = t.tbTeacher.Id,
                                                TeacherCode = t.tbTeacher.TeacherCode,
                                                TeacherName = t.tbTeacher.TeacherName,
                                                StudentId = s.tbStudent.Id,
                                                StudentCode = s.tbStudent.StudentCode,
                                                StudentName = s.tbStudent.StudentName,
                                                IsSelected = false
                                            }).Distinct().ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    vm.SurveyDetailClassList = vm.SurveyDetailClassList.Where(d => (d.StudentName.Contains(vm.SearchText) || d.StudentCode.Contains(vm.SearchText))).ToList();
                }

                var tbSurveyData = (from p in db.Table<Entity.tbSurveyData>()
                                    where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbSurveyItem.IsDeleted == false
                                        && p.tbSurveyOption.IsDeleted == false
                                        && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                        && p.tbTeacher.IsDeleted == false
                                    group p by new { classsId = p.tbClass.Id, studentId = p.tbStudent.Id, teacherId = p.tbTeacher.Id } into g
                                    select new
                                    {
                                        ClassId = g.Key.classsId,
                                        StudentId = g.Key.studentId,
                                        TeacherId = g.Key.teacherId
                                    }).Distinct().ToList();

                foreach (var item in vm.SurveyDetailClassList)
                {
                    if (tbSurveyData.Where(d => d.ClassId == item.ClassId && d.StudentId == item.StudentId && d.TeacherId == item.TeacherId).Count() > 0)
                    {
                        item.IsSelected = true;
                    }
                }
                #endregion

                vm.SurveyDetailOrgList = vm.SurveyDetailOrgList.Union(vm.SurveyDetailClassList).Distinct().ToList();
                vm.SurveyDetailOrgList = vm.SurveyDetailOrgList.Where(d => d.IsSelected == false).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detail(Models.SurveyReport.Detail vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("Detail", new
            {
                SurveyId = vm.SurveyId,
                SearchText = vm.SearchText
            }));
        }

        public ActionResult ExportDetail()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyReport.Detail();

                var file = System.IO.Path.GetTempFileName();

                #region 任课教师模式
                var tbCourseIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                   where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                   select p.tbCourse.Id).Distinct().ToList();

                var year = (from p in db.Table<Entity.tbSurvey>()
                            where p.Id == vm.SurveyId
                            select new
                            {
                                YearId = p.tbYear.Id,
                            }).FirstOrDefault();

                var tbOrgList = from p in db.Table<Course.Entity.tbOrgTeacher>()
                                where tbCourseIds.Contains(p.tbOrg.tbCourse.Id)
                                    && p.tbOrg.IsDeleted == false
                                    && p.tbOrg.tbYear.Id == year.YearId
                                    && p.tbOrg.tbCourse.IsDeleted == false
                                    && p.tbTeacher.IsDeleted == false
                                select new Dto.SurveyReport.Detail
                                {
                                    OrgId = p.tbOrg.Id,
                                    OrgName = p.tbOrg.OrgName,
                                    TeacherId = p.tbTeacher.Id,
                                    TeacherCode = p.tbTeacher.TeacherCode,
                                    TeacherName = p.tbTeacher.TeacherName,
                                    ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                    IsClass = p.tbOrg.IsClass
                                };

                var OrgList = (from p in tbOrgList
                               select p).Distinct().ToList();

                vm.SurveyDetailOrgList = new List<Dto.SurveyReport.Detail>();
                foreach (var org in OrgList)
                {
                    if (org.IsClass)
                    {
                        var tbStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                         where p.tbClass.Id == org.ClassId
                                            && p.tbStudent.IsDeleted == false
                                         select new Dto.SurveyReport.Detail
                                         {
                                             ClassId = org.OrgId,
                                             ClassName = org.OrgName,
                                             StudentId = p.tbStudent.Id,
                                             StudentCode = p.tbStudent.StudentCode,
                                             StudentName = p.tbStudent.StudentName,
                                             TeacherId = org.TeacherId,
                                             TeacherCode = org.TeacherCode,
                                             TeacherName = org.TeacherName,
                                             IsSelected = false
                                         }).ToList();

                        vm.SurveyDetailOrgList.AddRange(tbStudent);
                    }
                    else
                    {
                        var tbStudent = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                         where p.tbOrg.Id == org.OrgId
                                            && p.tbStudent.IsDeleted == false
                                         select new Dto.SurveyReport.Detail
                                         {
                                             ClassId = org.OrgId,
                                             ClassName = org.OrgName,
                                             StudentId = p.tbStudent.Id,
                                             StudentCode = p.tbStudent.StudentCode,
                                             StudentName = p.tbStudent.StudentName,
                                             TeacherId = org.TeacherId,
                                             TeacherCode = org.TeacherCode,
                                             TeacherName = org.TeacherName,
                                             IsSelected = false
                                         }).ToList();
                        vm.SurveyDetailOrgList.AddRange(tbStudent);
                    }
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    vm.SurveyDetailOrgList = vm.SurveyDetailOrgList.Where(d => (d.StudentCode.Contains(vm.SearchText) || d.StudentName.Contains(vm.SearchText))).ToList();
                }

                var tbSurveyDataOrg = (from p in db.Table<Entity.tbSurveyData>()
                                       where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                           && p.tbOrg.IsDeleted == false
                                           && p.tbStudent.IsDeleted == false
                                           && p.tbSurveyItem.IsDeleted == false
                                           && p.tbSurveyOption.IsDeleted == false
                                           && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                           && p.tbTeacher.IsDeleted == false
                                       group p by new { orgId = p.tbOrg.Id, studentId = p.tbStudent.Id, teacherId = p.tbTeacher.Id } into g
                                       select new
                                       {
                                           OrgId = g.Key.orgId,
                                           StudentId = g.Key.studentId,
                                           TeacherId = g.Key.teacherId
                                       }).Distinct().ToList();

                foreach (var item in vm.SurveyDetailOrgList)
                {
                    if (tbSurveyDataOrg.Where(d => d.OrgId == item.ClassId && d.StudentId == item.StudentId && d.TeacherId == item.TeacherId).Count() > 0)
                    {
                        item.IsSelected = true;
                    }
                }
                #endregion

                #region 班主任模式
                //班主任模式
                vm.SurveyDetailClassList = (from p in db.Table<Entity.tbSurveyClass>()
                                            join q in db.Table<Entity.tbSurveyGroup>() on p.tbSurvey.Id equals q.tbSurvey.Id
                                            join s in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals s.tbClass.Id
                                            join t in db.Table<Basis.Entity.tbClassTeacher>() on p.tbClass.Id equals t.tbClass.Id
                                            where p.tbSurvey.Id == vm.SurveyId
                                                && q.IsOrg == false
                                                && p.tbClass.IsDeleted == false
                                                && s.tbStudent.IsDeleted == false
                                            select new Dto.SurveyReport.Detail
                                            {
                                                ClassId = p.tbClass.Id,
                                                ClassName = p.tbClass.ClassName,
                                                TeacherId = t.tbTeacher.Id,
                                                TeacherCode = t.tbTeacher.TeacherCode,
                                                TeacherName = t.tbTeacher.TeacherName,
                                                StudentId = s.tbStudent.Id,
                                                StudentCode = s.tbStudent.StudentCode,
                                                StudentName = s.tbStudent.StudentName,
                                                IsSelected = false
                                            }).Distinct().ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    vm.SurveyDetailClassList = vm.SurveyDetailClassList.Where(d => (d.StudentName.Contains(vm.SearchText) || d.StudentCode.Contains(vm.SearchText))).ToList();
                }

                var tbSurveyData = (from p in db.Table<Entity.tbSurveyData>()
                                    where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbSurveyItem.IsDeleted == false
                                        && p.tbSurveyOption.IsDeleted == false
                                        && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                        && p.tbTeacher.IsDeleted == false
                                    group p by new { classsId = p.tbClass.Id, studentId = p.tbStudent.Id, teacherId = p.tbTeacher.Id } into g
                                    select new
                                    {
                                        ClassId = g.Key.classsId,
                                        StudentId = g.Key.studentId,
                                        TeacherId = g.Key.teacherId
                                    }).Distinct().ToList();

                foreach (var item in vm.SurveyDetailClassList)
                {
                    if (tbSurveyData.Where(d => d.ClassId == item.ClassId && d.StudentId == item.StudentId && d.TeacherId == item.TeacherId).Count() > 0)
                    {
                        item.IsSelected = true;
                    }
                }
                #endregion
                vm.SurveyDetailOrgList = vm.SurveyDetailOrgList.Union(vm.SurveyDetailClassList).Distinct().ToList();
                vm.SurveyDetailOrgList = vm.SurveyDetailOrgList.Where(d => d.IsSelected == false).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("参评班级"),
                        new System.Data.DataColumn("教师姓名"),
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("状态")
                    });

                foreach (var a in vm.SurveyDetailOrgList)
                {
                    var dr = dt.NewRow();
                    dr["参评班级"] = a.ClassName;
                    dr["教师姓名"] = a.TeacherName;
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["状态"] = "未评";
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

        public ActionResult UnSurveyClass()
        {
            var vm = new Models.SurveyReport.UnSurveyClass();

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                var surveyDataList = db.Table<Entity.tbSurveyData>().Include(d => d.tbClass).Where(d => d.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId).ToList();
                var classStudentList = db.Table<Basis.Entity.tbClassStudent>()
                    .Include(d => d.tbClass).ToList();
                var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>().Include(d => d.tbTeacher).Include(d => d.tbClass).ToList();

                vm.List = (from p in db.Table<Entity.tbSurveyClass>()
                           where p.tbSurvey.Id == vm.SurveyId
                           orderby p.tbClass.No
                           select new Dto.SurveyReport.UnSurveyClass()
                           {
                               Id = p.tbClass.Id,
                               No = p.tbClass.No,
                               ClassName = p.tbClass.ClassName
                           }).ToList();

                foreach (var v in vm.List)
                {
                    v.Num = classStudentList.Where(d => d.tbClass.Id == v.Id).Count();
                    v.UnNum = v.Num - surveyDataList.Where(d => (d.tbClass == null ? 0 : d.tbClass.Id) == v.Id).Count();
                    if (classTeacherList.Where(d => d.tbClass.Id == v.Id).Any())
                    {
                        v.TeacherName = classTeacherList.Where(d => d.tbClass.Id == v.Id).FirstOrDefault().tbTeacher.TeacherName;
                    }
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnSurveyClass(Models.SurveyReport.UnSurveyClass vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("UnSurveyClass", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId
            }));
        }

        public ActionResult UnSurveyClassStudent(int classId)
        {
            var vm = new Models.SurveyReport.UnSurveyClassStudent();
            vm.ClassId = classId;

            using (var db = new XkSystem.Models.DbContext())
            {
                var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>()
                    .Include(d => d.tbClass)
                    .Include(d => d.tbTeacher).ToList();
                var tb = db.Table<Basis.Entity.tbClassStudent>()
                    .Include(d => d.tbClass).Where(d => d.tbClass.Id == classId);

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText)
                                  || d.tbStudent.StudentName.Contains(vm.SearchText)
                                  || d.tbStudent.StudentNameEn.Contains(vm.SearchText));
                }

                vm.List = (from p in tb
                           orderby p.No
                           select new Dto.SurveyReport.UnSurveyClassStudent()
                           {
                               Id = p.tbClass.Id,
                               No = p.tbClass.No,
                               ClassName = p.tbClass.ClassName,
                               StudentCode = p.tbStudent.StudentCode,
                               StudentName = p.tbStudent.StudentName,
                               StudentSex = p.tbStudent.tbSysUser.tbSex.SexName
                           }).ToList();

                foreach (var v in vm.List)
                {
                    v.TeacherName = classTeacherList.Where(d => d.tbClass.Id == v.Id).FirstOrDefault().tbTeacher.TeacherName;
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnSurveyClassStudent(Models.SurveyReport.UnSurveyClassStudent vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("UnSurveyClassStudent", new
            {
                SearchText = vm.SearchText
            }));
        }

        public ActionResult UnSurveyStudent()
        {
            var vm = new Models.SurveyReport.UnSurveyStudent();

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                var classIds = db.Table<Entity.tbSurveyClass>()
                    .Where(d => d.tbSurvey.Id == vm.SurveyId).Select(d => d.tbClass.Id).ToList();
                var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>()
                    .Include(d => d.tbClass)
                    .Include(d => d.tbTeacher).ToList();
                var studentIds = db.Table<Entity.tbSurveyData>().Select(d => d.tbStudent.Id).ToList();

                vm.List = (from p in db.Table<Basis.Entity.tbClassStudent>()
                           where classIds.Contains(p.tbClass.Id) && !studentIds.Contains(p.tbStudent.Id)
                           orderby p.No
                           select new Dto.SurveyReport.UnSurveyStudent()
                           {
                               Id = p.tbClass.Id,
                               No = p.tbClass.No,
                               ClassName = p.tbClass.ClassName,
                               StudentCode = p.tbStudent.StudentCode,
                               StudentName = p.tbStudent.StudentName,
                               StudentSex = p.tbStudent.tbSysUser.tbSex.SexName
                           }).ToPageList(vm.Page);

                foreach (var v in vm.List)
                {
                    v.TeacherName = classTeacherList.Where(d => d.tbClass.Id == v.Id).FirstOrDefault().tbTeacher.TeacherName;
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnSurveyStudent(Models.SurveyReport.UnSurveyStudent vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("UnSurveyStudent", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult UnSurveyExport()
        {
            var vm = new Models.SurveyReport.UnSurveyExport();
            using (var db = new XkSystem.Models.DbContext())
            {
                var classIds = new List<int>();
                if (vm.ClassId > 0)
                {
                    classIds.Add(Convert.ToInt32(vm.ClassId));
                }
                else
                {
                    classIds = db.Table<Entity.tbSurveyClass>()
                       .Where(d => d.tbSurvey.Id == vm.SurveyId || vm.SurveyId <= 0)
                       .Where(d => d.tbSurvey.SurveyName.Contains(vm.SearchText))
                       .Select(d => d.tbClass.Id).ToList();
                }
                var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>()
                    .Include(d => d.tbClass)
                    .Include(d => d.tbTeacher).ToList();

                vm.ExportList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                 where classIds.Contains(p.tbClass.Id)
                                 orderby p.No
                                 select new Dto.SurveyReport.UnSurveyExport()
                                 {
                                     Id = p.tbClass.Id,
                                     No = p.tbClass.No,
                                     ClassName = p.tbClass.ClassName,
                                     StudentCode = p.tbStudent.StudentCode,
                                     StudentName = p.tbStudent.StudentName,
                                     StudentSex = p.tbStudent.tbSysUser.tbSex.SexName
                                 }).ToList();

                foreach (var v in vm.ExportList)
                {
                    v.TeacherName = classTeacherList.Where(d => d.tbClass.Id == v.Id).FirstOrDefault().tbTeacher.TeacherName;
                }


                var file = System.IO.Path.GetTempFileName();
                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("评价教师"),
                        new System.Data.DataColumn("对应行政班"),
                        new System.Data.DataColumn("学生学号"),
                        new System.Data.DataColumn("学生姓名"),
                        new System.Data.DataColumn("学生性别")
                    });
                foreach (var a in vm.ExportList)
                {
                    var dr = dt.NewRow();
                    dr["评价教师"] = a.TeacherName;
                    dr["对应行政班"] = a.ClassName;
                    dr["学生学号"] = a.StudentCode;
                    dr["学生姓名"] = a.StudentName;
                    dr["学生性别"] = a.StudentSex;
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

        public ActionResult SubjectOrg()
        {
            var vm = new Models.SurveyReport.SubjectOrg();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveySubjectList = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                        && p.tbSurveyGroup.IsDeleted == false
                                        && p.tbSurveyGroup.IsOrg
                                        && p.tbCourse.IsDeleted == false
                                        && p.tbCourse.tbSubject.IsDeleted == false
                                        orderby p.tbCourse.tbSubject.No
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbCourse.tbSubject.SubjectName,
                                            Value = p.tbCourse.tbSubject.Id.ToString()
                                        }).Distinct().ToList();

                if (vm.SurveySubjectId == 0 && vm.SurveySubjectList.Count > 0)
                {
                    vm.SurveySubjectId = vm.SurveySubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbCourse.tbSubject.IsDeleted == false
                                       && p.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                if (vm.SurveyCourseId == 0 && vm.SurveyCourseList.Count > 0)
                {
                    vm.SurveyCourseId = vm.SurveyCourseList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyTeacherList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                        join m in db.Table<Entity.tbSurveyCourse>()
                                        on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                        equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                        where p.tbOrg.IsDeleted == false
                                        && (m.tbCourse.Id == vm.SurveyCourseId)
                                        orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbTeacher.TeacherName,
                                            Value = p.tbTeacher.Id.ToString()
                                        }).Distinct().ToList();

                var teacherIds = vm.SurveyTeacherList.Select(d => d.Value.ConvertToInt()).Distinct().ToList();

                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && teacherIds.Contains(p.tbTeacher.Id)
                                        && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        select p;

                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbCourse.Id == vm.SurveyCourseId
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        select p.tbSurveyGroup.Id).Distinct().ToList();

                if (vm.SurveyCourseId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => tbSurveyGroupIds.Contains(d.tbSurveyItem.tbSurveyGroup.Id));
                }

                if (vm.SurveyTeacherId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.tbTeacher.Id == vm.SurveyTeacherId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectOrgList = (from p in tbSurveryDataList
                                           group p by new
                                           {
                                               orgId = p.tbOrg.Id,
                                               orgName = p.tbOrg.OrgName,
                                               teacherId = p.tbTeacher.Id,
                                               teacherName = p.tbTeacher.TeacherName,
                                               surveryItemId = p.tbSurveyItem.Id,
                                               surveryItemNo = p.tbSurveyItem.No,
                                               surveryItemName = p.tbSurveyItem.SurveyItemName,
                                               surverOptionId = p.tbSurveyOption.Id,
                                               surverOptionName = p.tbSurveyOption.OptionName
                                           } into g
                                           select new Dto.SurveyReport.SubjectOrg
                                           {
                                               OrgId = g.Key.orgId,
                                               OrgName = g.Key.orgName,
                                               SurveyItemId = g.Key.surveryItemId,
                                               SurveyItemNo = g.Key.surveryItemNo,
                                               SurveyItemName = g.Key.surveryItemName,
                                               SurveyOptionId = g.Key.surverOptionId,
                                               SurveyOptionName = g.Key.surverOptionName,
                                               TeacherId = g.Key.teacherId,
                                               TeacherName = g.Key.teacherName,
                                               SurveyOptionCount = g.Count(),
                                               SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                               SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                           }).ToList();

                vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                     where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                     && tbSurveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                     orderby p.tbSurveyGroup.No, p.No
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Text = p.SurveyItemName,
                                         Value = p.Id.ToString()
                                     }).Distinct().ToList();

                vm.SurveyOptionList = (from p in db.Table<Entity.tbSurveyOption>()
                                       where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                       orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.OptionName,
                                           Value = p.OptionName
                                       }).Distinct().ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubjectOrg(Models.SurveyReport.SubjectOrg vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SubjectOrg", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveySubjectId = vm.SurveySubjectId,
                SurveyCourseId = vm.SurveyCourseId,
                SurveyTeacherId = vm.SurveyTeacherId,
            }));
        }

        public ActionResult SubjectOrgExport(int SurveyId = 0, int SurveySubjectId = 0, int SurveyTeacherId = 0)
        {
            var vm = new Models.SurveyReport.SubjectOrg();
            var surveyName = "";
            var surveySubjectName = "";
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                #region 准备数据
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                    surveyName = vm.SurveyList.FirstOrDefault().Text;
                }
                else
                {
                    if (vm.SurveyId > decimal.Zero && vm.SurveyList.Count > decimal.Zero)
                    {
                        var grade = vm.SurveyList.Where(d => d.Value.ConvertToInt() == vm.SurveyId).FirstOrDefault();
                        if (grade != null)
                        {
                            surveyName = grade.Text;
                        }
                    }
                }
                vm.SurveySubjectList = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                        && p.tbSurveyGroup.IsDeleted == false
                                        && p.tbSurveyGroup.IsOrg
                                        && p.tbCourse.IsDeleted == false
                                        && p.tbCourse.tbSubject.IsDeleted == false
                                        orderby p.tbCourse.tbSubject.No
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbCourse.tbSubject.SubjectName,
                                            Value = p.tbCourse.tbSubject.Id.ToString()
                                        }).Distinct().ToList();

                if (vm.SurveySubjectId == 0 && vm.SurveySubjectList.Count > 0)
                {
                    vm.SurveySubjectId = vm.SurveySubjectList.FirstOrDefault().Value.ConvertToInt();
                    surveySubjectName = vm.SurveySubjectList.FirstOrDefault().Text;
                }
                else
                {
                    if (vm.SurveySubjectId > decimal.Zero && vm.SurveySubjectList.Count > decimal.Zero)
                    {
                        var grade = vm.SurveySubjectList.Where(d => d.Value.ConvertToInt() == vm.SurveySubjectId).FirstOrDefault();
                        if (grade != null)
                        {
                            surveySubjectName = grade.Text;
                        }
                    }
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbCourse.tbSubject.IsDeleted == false
                                       && p.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                if (vm.SurveyCourseId == 0 && vm.SurveyCourseList.Count > 0)
                {
                    vm.SurveyCourseId = vm.SurveyCourseList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyTeacherList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                        join m in db.Table<Entity.tbSurveyCourse>()
                                        on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                        equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                        where p.tbOrg.IsDeleted == false
                                        && (m.tbCourse.Id == vm.SurveyCourseId)
                                        orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbTeacher.TeacherName,
                                            Value = p.tbTeacher.Id.ToString()
                                        }).Distinct().ToList();

                var teacherIds = vm.SurveyTeacherList.Select(d => d.Value.ConvertToInt()).Distinct().ToList();

                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && teacherIds.Contains(p.tbTeacher.Id)
                                        && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        select p;

                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbCourse.Id == vm.SurveyCourseId
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        select p.tbSurveyGroup.Id).Distinct().ToList();

                if (vm.SurveyCourseId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => tbSurveyGroupIds.Contains(d.tbSurveyItem.tbSurveyGroup.Id));
                }

                if (vm.SurveyTeacherId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.tbTeacher.Id == vm.SurveyTeacherId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectOrgList = (from p in tbSurveryDataList
                                           group p by new
                                           {
                                               orgId = p.tbOrg.Id,
                                               orgName = p.tbOrg.OrgName,
                                               teacherId = p.tbTeacher.Id,
                                               teacherName = p.tbTeacher.TeacherName,
                                               surveryItemId = p.tbSurveyItem.Id,
                                               surveryItemName = p.tbSurveyItem.SurveyItemName,
                                               surverOptionId = p.tbSurveyOption.Id,
                                               surverOptionName = p.tbSurveyOption.OptionName
                                           } into g
                                           select new Dto.SurveyReport.SubjectOrg
                                           {
                                               OrgId = g.Key.orgId,
                                               OrgName = g.Key.orgName,
                                               SurveyItemId = g.Key.surveryItemId,
                                               SurveyItemName = g.Key.surveryItemName,
                                               SurveyOptionId = g.Key.surverOptionId,
                                               SurveyOptionName = g.Key.surverOptionName,
                                               TeacherId = g.Key.teacherId,
                                               TeacherName = g.Key.teacherName,
                                               SurveyOptionCount = g.Count(),
                                               SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                               SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                           }).ToList();

                vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                     where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                     && tbSurveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                     orderby p.tbSurveyGroup.No, p.No
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Text = p.SurveyItemName,
                                         Value = p.Id.ToString()
                                     }).Distinct().ToList();

                vm.SurveyOptionList = (from p in db.Table<Entity.tbSurveyOption>()
                                       where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                       orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.OptionName,
                                           Value = p.OptionName
                                       }).Distinct().ToList();
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();
                int surveyOptionCount = vm.SurveyOptionList.Count();
                var allColumnLength = surveyOptionCount + 4;
                var arrColumns = new string[allColumnLength];

                for (int i = 0; i < arrColumns.Length; i++)
                {
                    arrColumns[i] = (i + 1).ToString();
                }

                var teacherList = from p in vm.SurveyTeacherList
                                  select p;

                if (vm.SurveyTeacherId > decimal.Zero)
                {
                    teacherList = teacherList.Where(d => d.Value.ConvertToInt() == vm.SurveyTeacherId);
                }

                vm.SurveyTeacherList = teacherList.ToList();
                foreach (var teacher in vm.SurveyTeacherList)
                {
                    var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                    sheet.isColumnWritten = false;
                    sheet.isWriteHeader = true;
                    sheet.strHeaderText = teacher.Text;
                    //开始表格
                    var dt = Code.Common.ArrayToDataTable(arrColumns);
                    var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                    var arrSurveyOptionID = new string[allColumnLength];
                    var arrSurveyOptionName = new string[allColumnLength];
                    arrSurveyOptionName[0] = "评价内容";
                    arrSurveyOptionName[1] = "教学班";
                    for (int i = 0; i < surveyOptionCount; i++)
                    {
                        arrSurveyOptionID[i + 2] = vm.SurveyOptionList[i].Value.ToString();
                        arrSurveyOptionName[i + 2] = "选" + vm.SurveyOptionList[i].Text + "人数";
                    }
                    arrSurveyOptionName[allColumnLength - 2] = "参评总人数";
                    arrSurveyOptionName[allColumnLength - 1] = "项平均得分";
                    #region 处理DataList
                    foreach (var org in vm.SurveySubjectOrgList.Where(d => d.TeacherId == teacher.Value.ConvertToInt())
                .GroupBy(d => new { d.OrgId, d.OrgName }).Select(g => new { orgId = g.Key.OrgId, orgName = g.Key.OrgName }))
                    {
                        string[] arrSurveyDataFirst = new string[allColumnLength];
                        string[] arrSurveyDataLast = new string[allColumnLength];
                        dt.Rows.Add(arrSurveyDataLast);
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(dt.Rows.Count, dt.Rows.Count, 0, allColumnLength - 1));
                        arrSurveyDataFirst[0] = "[" + surveyName + "/" + surveySubjectName + "]" + "  " + org.orgName;
                        dt.Rows.Add(arrSurveyDataFirst);
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(dt.Rows.Count, dt.Rows.Count, 0, allColumnLength - 1));
                        dt.Rows.Add(arrSurveyOptionName);
                        foreach (var item in vm.SurveySubjectOrgList.Where(d => d.OrgId == org.orgId && d.TeacherId == teacher.Value.ConvertToInt())
                            .GroupBy(d => new { d.OrgId, d.OrgName, d.SurveyItemName }).
                            Select(g => new { orgId = g.Key.OrgId, orgName = g.Key.OrgName, surverItemName = g.Key.SurveyItemName }))
                        {
                            var optionAllCount = 0;
                            var optionAllSum = 0m;
                            var optionAllAvg = 0m;
                            string[] arrSurveyData = new string[allColumnLength];
                            arrSurveyData[0] = item.surverItemName;
                            arrSurveyData[1] = org.orgName;
                            for (var i = 2; i < vm.SurveyOptionList.Count; i++)
                            {
                                var optionCount = vm.SurveySubjectOrgList.Where(d => d.OrgId == org.orgId && d.TeacherId == teacher.Value.ConvertToInt()
                                && d.SurveyItemName == item.surverItemName && d.SurveyOptionName == arrSurveyOptionID[i].ToString()).FirstOrDefault();
                                if (optionCount != null)
                                {
                                    optionAllCount += optionCount.SurveyOptionCount;
                                    optionAllSum += optionCount.SurveyOptionSum;
                                    if (optionCount.SurveyOptionCount > decimal.Zero)
                                    {
                                        arrSurveyData[i] = optionCount.SurveyOptionCount.ToString();
                                    }
                                }
                            }
                            if (optionAllCount > decimal.Zero)
                            {
                                //总人数
                                arrSurveyData[allColumnLength - 2] = optionAllCount.ToString();
                                //平均分
                                optionAllAvg = Math.Round(optionAllSum / optionAllCount, 2);
                                arrSurveyData[allColumnLength - 1] = optionAllAvg.ToString();
                            }
                            dt.Rows.Add(arrSurveyData);
                        }
                    }
                    #endregion
                    sheet.data = dt;
                    sheet.regions = regions;
                    sheetList.Add(sheet);
                }
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

        public ActionResult TeacherOrg()
        {
            var vm = new Models.SurveyReport.TeacherOrg();
            using (var db = new XkSystem.Models.DbContext())
            {
                db.Database.CommandTimeout = 0;
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveySubjectList = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                        && p.tbSurveyGroup.IsDeleted == false
                                        && p.tbSurveyGroup.SurveyGroupName.Contains("校本") == false
                                        && p.tbSurveyGroup.IsOrg
                                        && p.tbCourse.IsDeleted == false
                                        && p.tbCourse.tbSubject.IsDeleted == false
                                        orderby p.tbCourse.tbSubject.No
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbCourse.tbSubject.SubjectName,
                                            Value = p.tbCourse.tbSubject.Id.ToString()
                                        }).Distinct().ToList();

                if (vm.SurveySubjectId == 0 && vm.SurveySubjectList.Count > 0)
                {
                    vm.SurveySubjectId = vm.SurveySubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.SurveyGroupName.Contains("校本") == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbCourse.tbSubject.IsDeleted == false
                                       && p.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                //if (vm.SurveyCourseId == 0 && vm.SurveyCourseList.Count > 0)
                //{
                //    vm.SurveyCourseId = vm.SurveyCourseList.FirstOrDefault().Value.ConvertToInt();
                //}

                vm.SurveyTeacherList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                        join m in db.Table<Entity.tbSurveyCourse>()
                                        on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                        equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                        where p.tbOrg.IsDeleted == false
                                        && (m.tbCourse.Id == vm.SurveyCourseId || vm.SurveyCourseId == 0)
                                        && m.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                        && m.tbSurveyGroup.SurveyGroupName.Contains("校本") == false
                                        orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbTeacher.TeacherName,
                                            Value = p.tbTeacher.Id.ToString()
                                        }).Distinct().ToList();

                var teacherIds = vm.SurveyTeacherList.Select(d => d.Value.ConvertToInt()).Distinct().ToList();

                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && teacherIds.Contains(p.tbTeacher.Id)
                                        && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        && p.tbSurveyItem.SurveyItemName.Contains("总体感受") == false
                                        && p.tbSurveyItem.tbSurveyGroup.SurveyGroupName.Contains("校本") == false
                                        select p;

                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where (p.tbCourse.Id == vm.SurveyCourseId || vm.SurveyCourseId == 0)
                                        && p.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbSurveyGroup.SurveyGroupName.Contains("校本") == false
                                        select p.tbSurveyGroup.Id).Distinct().ToList();

                if (vm.SurveySubjectId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => tbSurveyGroupIds.Contains(d.tbSurveyItem.tbSurveyGroup.Id));
                }

                if (vm.SurveyTeacherId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.tbTeacher.Id == vm.SurveyTeacherId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyTeacherOrgList = (from p in tbSurveryDataList
                                           group p by new
                                           {
                                               orgId = p.tbOrg.Id,
                                               orgName = p.tbOrg.OrgName,
                                               classId = p.tbOrg.IsClass ? (p.tbOrg.tbClass == null ? 0 : p.tbOrg.tbClass.Id) : 0,
                                               className = p.tbOrg.IsClass ? (p.tbOrg.tbClass == null ? "" : p.tbOrg.tbClass.ClassName) : "",
                                               teacherId = p.tbTeacher.Id,
                                               teacherName = p.tbTeacher.TeacherName,
                                               surveryItemId = p.tbSurveyItem.Id,
                                               surveryItemName = p.tbSurveyItem.SurveyItemName
                                           } into g
                                           select new Dto.SurveyReport.TeacherOrg
                                           {
                                               OrgId = g.Key.orgId,
                                               OrgName = g.Key.orgName,
                                               ClassId = g.Key.classId,
                                               ClassName = g.Key.className,
                                               SurveyItemId = g.Key.surveryItemId,
                                               SurveyItemName = g.Key.surveryItemName,
                                               TeacherId = g.Key.teacherId,
                                               TeacherName = g.Key.teacherName,
                                               SurveyOptionCount = g.Count(),
                                               SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                               SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                           }).ToList();

                vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                     where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                     && tbSurveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                     && p.SurveyItemName.Contains("总体感受") == false
                                     && p.tbSurveyGroup.SurveyGroupName.Contains("校本") == false
                                     orderby p.tbSurveyGroup.No, p.No
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Text = p.SurveyItemName,
                                         Value = p.Id.ToString()
                                     }).Distinct().ToList();

                vm.SurveyOptionList = (from p in db.Table<Entity.tbSurveyOption>()
                                       where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                       orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.OptionName,
                                           Value = p.OptionName
                                       }).Distinct().ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeacherOrg(Models.SurveyReport.TeacherOrg vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("TeacherOrg", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveySubjectId = vm.SurveySubjectId,
                SurveyCourseId = vm.SurveyCourseId,
                SurveyTeacherId = vm.SurveyTeacherId,
            }));
        }

        public ActionResult TeacherOrgExport(int SurveyId = 0, int SurveySubjectId = 0, int SurveyCourseId = 0, int SurveyTeacherId = 0)
        {
            var vm = new Models.SurveyReport.TeacherOrg();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                #region 准备数据
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveySubjectList = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                        && p.tbSurveyGroup.IsDeleted == false
                                        && p.tbSurveyGroup.SurveyGroupName.Contains("校本") == false
                                        && p.tbSurveyGroup.IsOrg
                                        && p.tbCourse.IsDeleted == false
                                        && p.tbCourse.tbSubject.IsDeleted == false
                                        orderby p.tbCourse.tbSubject.No
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbCourse.tbSubject.SubjectName,
                                            Value = p.tbCourse.tbSubject.Id.ToString()
                                        }).Distinct().ToList();

                if (vm.SurveySubjectId == 0 && vm.SurveySubjectList.Count > 0)
                {
                    vm.SurveySubjectId = vm.SurveySubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.SurveyGroupName.Contains("校本") == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbCourse.tbSubject.IsDeleted == false
                                       && p.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                //if (vm.SurveyCourseId == 0 && vm.SurveyCourseList.Count > 0)
                //{
                //    vm.SurveyCourseId = vm.SurveyCourseList.FirstOrDefault().Value.ConvertToInt();
                //}

                vm.SurveyTeacherList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                        join m in db.Table<Entity.tbSurveyCourse>()
                                        on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                        equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                        where p.tbOrg.IsDeleted == false
                                        && (m.tbCourse.Id == vm.SurveyCourseId || vm.SurveyCourseId == 0)
                                        && m.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                        && m.tbSurveyGroup.SurveyGroupName.Contains("校本") == false
                                        orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbTeacher.TeacherName,
                                            Value = p.tbTeacher.Id.ToString()
                                        }).Distinct().ToList();

                var teacherIds = vm.SurveyTeacherList.Select(d => d.Value.ConvertToInt()).Distinct().ToList();

                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && teacherIds.Contains(p.tbTeacher.Id)
                                        && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        && p.tbSurveyItem.SurveyItemName.Contains("总体感受") == false
                                        && p.tbSurveyItem.tbSurveyGroup.SurveyGroupName.Contains("校本") == false
                                        select p;

                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where (p.tbCourse.Id == vm.SurveyCourseId || vm.SurveyCourseId == 0)
                                        && p.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbSurveyGroup.SurveyGroupName.Contains("校本") == false
                                        select p.tbSurveyGroup.Id).Distinct().ToList();

                if (vm.SurveySubjectId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => tbSurveyGroupIds.Contains(d.tbSurveyItem.tbSurveyGroup.Id));
                }

                if (vm.SurveyTeacherId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.tbTeacher.Id == vm.SurveyTeacherId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyTeacherOrgList = (from p in tbSurveryDataList
                                           group p by new
                                           {
                                               orgId = p.tbOrg.Id,
                                               orgName = p.tbOrg.OrgName,
                                               classId = p.tbOrg.IsClass ? (p.tbOrg.tbClass == null ? 0 : p.tbOrg.tbClass.Id) : 0,
                                               className = p.tbOrg.IsClass ? (p.tbOrg.tbClass == null ? "" : p.tbOrg.tbClass.ClassName) : "",
                                               teacherId = p.tbTeacher.Id,
                                               teacherName = p.tbTeacher.TeacherName,
                                               surveryItemId = p.tbSurveyItem.Id,
                                               surveryItemName = p.tbSurveyItem.SurveyItemName
                                           } into g
                                           select new Dto.SurveyReport.TeacherOrg
                                           {
                                               OrgId = g.Key.orgId,
                                               OrgName = g.Key.orgName,
                                               ClassId = g.Key.classId,
                                               ClassName = g.Key.className,
                                               SurveyItemId = g.Key.surveryItemId,
                                               SurveyItemName = g.Key.surveryItemName,
                                               TeacherId = g.Key.teacherId,
                                               TeacherName = g.Key.teacherName,
                                               SurveyOptionCount = g.Count(),
                                               SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                               SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                           }).ToList();

                vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                     where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                     && tbSurveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                     && p.SurveyItemName.Contains("总体感受") == false
                                     orderby p.tbSurveyGroup.No, p.No
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Text = p.SurveyItemName,
                                         Value = p.Id.ToString()
                                     }).Distinct().ToList();

                vm.SurveyOptionList = (from p in db.Table<Entity.tbSurveyOption>()
                                       where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                       orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.OptionName,
                                           Value = p.OptionName
                                       }).Distinct().ToList();
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();

                var teacherList = from p in vm.SurveyTeacherList
                                  select p;

                if (vm.SurveyTeacherId > decimal.Zero)
                {
                    teacherList = teacherList.Where(d => d.Value.ConvertToInt() == vm.SurveyTeacherId);
                }

                vm.SurveyTeacherList = teacherList.ToList();
                foreach (var teacher in vm.SurveyTeacherList)
                {
                    var teacherOrgList = vm.SurveyTeacherOrgList.Where(d => d.TeacherId == teacher.Value.ConvertToInt()).GroupBy(d => new { d.OrgId, d.OrgName, d.ClassId, d.ClassName }).Select(g => new { orgId = g.Key.OrgId, orgName = g.Key.OrgName, classId = g.Key.ClassId, g.Key.ClassName }).ToList();
                    int surveyOptionCount = teacherOrgList.Count();
                    var allColumnLength = surveyOptionCount * 2 + 3;
                    var arrColumns = new string[allColumnLength];

                    for (int i = 0; i < arrColumns.Length; i++)
                    {
                        arrColumns[i] = (i + 1).ToString();
                    }

                    var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                    sheet.isColumnWritten = false;
                    sheet.isWriteHeader = true;
                    sheet.strHeaderText = teacher.Text;
                    //开始表格
                    var dt = Code.Common.ArrayToDataTable(arrColumns);
                    var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                    var arrSurveyOrgID = new string[allColumnLength];
                    var arrSurveyOrgName = new string[allColumnLength];
                    var arrSurveyOptionID = new string[allColumnLength];
                    var arrSurveyOptionName = new string[allColumnLength];
                    var index = 0;
                    for (int i = 0; i < surveyOptionCount; i++)
                    {
                        for (var j = 0; j < 2; j++)
                        {
                            index++;
                            arrSurveyOrgID[index] = teacherOrgList[i].orgId.ToString();
                            arrSurveyOrgName[index] = teacherOrgList[i].orgName.ToString() + "(" + teacherOrgList[i].ClassName + ")";
                            if (j == 0)
                            {
                                arrSurveyOptionID[index] = "总人数";
                                arrSurveyOptionName[index] = "总人数";
                            }
                            else if (j == 1)
                            {
                                arrSurveyOptionID[index] = "平均分";
                                arrSurveyOptionName[index] = "平均分";
                            }
                        }
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, index - 1, index));
                    }
                    dt.Rows.Add(arrSurveyOrgName);
                    dt.Rows.Add(arrSurveyOptionName);
                    #region 处理DataList
                    foreach (var item in vm.SurveyItemList)
                    {
                        string[] arrSurveyData = new string[allColumnLength];
                        arrSurveyData[0] = item.Text;
                        var optionAllCount = 0;
                        var optionAllSum = 0m;
                        var optionAllAvg = 0m;
                        var indexR = 1;
                        foreach (var orgTh in vm.SurveyTeacherOrgList.Where(d => d.TeacherId == teacher.Value.ConvertToInt())
                            .GroupBy(d => new { d.OrgId, d.OrgName }).Select(g => new { orgId = g.Key.OrgId, g.Key.OrgName }))
                        {
                            var optionCount = vm.SurveyTeacherOrgList.Where(d => d.OrgId == orgTh.orgId && d.TeacherId == teacher.Value.ConvertToInt() && d.SurveyItemName == item.Text).FirstOrDefault();
                            if (optionCount != null)
                            {
                                optionAllCount += optionCount.SurveyOptionCount;
                                optionAllSum += optionCount.SurveyOptionSum;
                                arrSurveyData[indexR] = optionCount.SurveyOptionCount.ToString();
                                arrSurveyData[indexR + 1] = Decimal.Round(optionCount.SurveyOptionAvg, 2).ToString();
                            }
                            indexR = indexR + 2;
                        }
                        arrSurveyData[allColumnLength - 2] = optionAllCount.ToString();
                        if (optionAllCount > decimal.Zero)
                        {
                            optionAllAvg = Decimal.Round(optionAllSum / optionAllCount, 2);
                        }
                        arrSurveyData[allColumnLength - 1] = optionAllAvg.ToString();
                        dt.Rows.Add(arrSurveyData);
                    }
                    #endregion
                    #region 增加合计
                    var orgIndexCount = 0;
                    var orgAllCount = 0;
                    var orgAllSum = 0m;
                    var orgAllAvg = 0m;
                    string[] arrSurveyDataCount = new string[allColumnLength];
                    arrSurveyDataCount[0] = "合计";
                    var indexL = 1;
                    foreach (var orgTh in vm.SurveyTeacherOrgList.Where(d => d.TeacherId == teacher.Value.ConvertToInt())
                     .GroupBy(d => new { d.OrgId, d.OrgName })
                     .Select(g => new { orgId = g.Key.OrgId, g.Key.OrgName, orgCount = g.Select(d => d.SurveyOptionCount).Sum(), orgSum = g.Select(d => d.SurveyOptionAvg).Sum() }))
                    {
                        orgIndexCount++;
                        orgAllCount += orgTh.orgCount;
                        orgAllSum += orgTh.orgSum;
                        arrSurveyDataCount[indexL] = orgTh.orgCount.ToString();
                        arrSurveyDataCount[indexL + 1] = Decimal.Round(orgTh.orgSum, 2).ToString();
                        indexL = indexL + 2;
                    }
                    arrSurveyDataCount[allColumnLength - 2] = orgAllCount.ToString();
                    if (orgIndexCount > decimal.Zero)
                    {
                        orgAllAvg = Decimal.Round(orgAllSum / orgIndexCount, 2);
                        arrSurveyDataCount[allColumnLength - 1] = orgAllAvg.ToString();
                    }
                    dt.Rows.Add(arrSurveyDataCount);
                    #endregion

                    dt.Rows[0][0] = "评价内容";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));
                    dt.Rows[0][allColumnLength - 2] = "总人数汇总";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, allColumnLength - 2, allColumnLength - 2));
                    dt.Rows[0][allColumnLength - 1] = "平均分汇总";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, allColumnLength - 1, allColumnLength - 1));
                    sheet.data = dt;
                    sheet.regions = regions;
                    sheetList.Add(sheet);
                }
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

        public ActionResult GradeOrg()
        {
            var vm = new Models.SurveyReport.GradeOrg();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                var surveyGroupIds = new List<int>();
                if (vm.SurveyGroupId == 0)
                {
                    surveyGroupIds = vm.SurveyGroupList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGroupIds.Add((int)vm.SurveyGroupId);
                }

                vm.SurveyGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      orderby p.tbClass.tbGrade.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbClass.tbGrade.GradeName,
                                          Value = p.tbClass.tbGrade.Id.ToString()
                                      }).Distinct().ToList();

                if (vm.SurveyGradeId == 0 && vm.SurveyGradeList.Count > 0)
                {
                    vm.SurveyGradeId = vm.SurveyGradeList.FirstOrDefault().Value.ConvertToInt();
                    vm.GradeName = vm.SurveyGradeList.FirstOrDefault().Text;
                }
                else
                {
                    if (vm.SurveyGradeId > decimal.Zero && vm.SurveyGradeList.Count > decimal.Zero)
                    {
                        var grade = vm.SurveyGradeList.Where(d => d.Value.ConvertToInt() == vm.SurveyGradeId).FirstOrDefault();
                        if (grade != null)
                        {
                            vm.GradeName = grade.Text;
                        }
                    }
                }
                //学段
                var yearId = db.Table<Entity.tbSurvey>().Where(d => d.Id == vm.SurveyId).Select(d => d.tbYear.Id).FirstOrDefault();

                vm.SurveyClassList = Basis.Controllers.ClassController.SelectListByYearType(Code.EnumHelper.YearType.Section, yearId, vm.SurveyGradeId ?? 0);

                var tbSurveryDataList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                        join m in db.Table<Entity.tbSurveyData>() on p.tbStudent.Id equals m.tbStudent.Id
                                        join c in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals c.tbClass.Id
                                        where m.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && m.tbSurveyItem.tbSurveyGroup.IsOrg
                                        && c.tbSurvey.Id == vm.SurveyId
                                        && p.tbClass.tbGrade.Id == vm.SurveyGradeId
                                        && m.tbClass == null
                                        && m.tbTeacher.IsDeleted == false
                                        && m.tbStudent.IsDeleted == false
                                        && surveyGroupIds.Contains(m.tbSurveyItem.tbSurveyGroup.Id)
                                        && m.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        && m.tbSurveyItem.SurveyItemName.Contains("总体感受") == false
                                        select new
                                        {
                                            OrgId = m.tbOrg.Id,
                                            OrgName = m.tbOrg.OrgName,
                                            OrgNo = m.tbOrg.No,
                                            CourseId = m.tbOrg.tbCourse.Id,
                                            CourseName = m.tbOrg.tbCourse.CourseName,
                                            CourseNo = m.tbOrg.tbCourse.No,
                                            SubjectId = m.tbOrg.tbCourse.tbSubject.Id,
                                            SubjectName = m.tbOrg.tbCourse.tbSubject.SubjectName,
                                            SubjectNo = m.tbOrg.tbCourse.tbSubject.No,
                                            StudentId = m.tbStudent.Id,
                                            StudentName = m.tbStudent.StudentName,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            ClassNo = p.tbClass.No,
                                            SurveyItemId = m.tbSurveyItem.Id,
                                            SurveyItemName = m.tbSurveyItem.SurveyItemName,
                                            SurveyOptionId = m.tbSurveyOption.Id,
                                            SurveyOptionName = m.tbSurveyOption.OptionName,
                                            SurveyOptionValue = m.tbSurveyOption.OptionValue,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            GradeNo = p.tbClass.tbGrade.No,
                                            TeacherId = m.tbTeacher.Id,
                                            TeacherName = m.tbTeacher.TeacherName,
                                        };

                if (vm.SurveyClassId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.ClassId == vm.SurveyClassId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => vm.SearchText.Contains(d.ClassName) || vm.SearchText.Contains(d.SubjectName));
                }

                vm.SurveyGradeOrgList = (from p in tbSurveryDataList
                                         group p by new
                                         {
                                             classId = p.ClassId,
                                             className = p.ClassName,
                                             subjectId = p.SubjectId,
                                             subjectName = p.SubjectName,
                                             surveryItemId = p.SurveyItemId,
                                             surveryItemName = p.SurveyItemName
                                         } into g
                                         select new Dto.SurveyReport.GradeOrg
                                         {
                                             ClassId = g.Key.classId,
                                             ClassName = g.Key.className,
                                             SubjectId = g.Key.subjectId,
                                             SubjectName = g.Key.subjectName,
                                             SurveyItemId = g.Key.surveryItemId,
                                             SurveyItemName = g.Key.surveryItemName,
                                             SurveyOptionCount = g.Count(),
                                             SurveyOptionSum = g.Select(d => d.SurveyOptionValue).Sum(),
                                             SurveyOptionAvg = g.Select(d => d.SurveyOptionValue).Average()
                                         }).Distinct().ToList();

                var tbSubjectIds = vm.SurveyGradeOrgList.Select(d => d.SubjectId).Distinct().ToList();

                vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                     where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                     && surveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                     && p.SurveyItemName.Contains("总体感受") == false
                                     orderby p.tbSurveyGroup.No, p.No
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Text = p.SurveyItemName,
                                         Value = p.Id.ToString()
                                     }).Distinct().ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GradeOrg(Models.SurveyReport.GradeOrg vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("GradeOrg", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveyGroupId = vm.SurveyGroupId,
                SurveyGradeId = vm.SurveyGradeId,
                SurveyClassId = vm.SurveyClassId
            }));
        }

        public ActionResult GradeOrgExport()
        {
            var vm = new Models.SurveyReport.GradeOrg();
            var surveyName = "";
            var surveyGradeName = "";
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 准备数据
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                    surveyName = vm.SurveyList.FirstOrDefault().Text;
                }
                else
                {
                    if (vm.SurveyId > decimal.Zero && vm.SurveyList.Count > decimal.Zero)
                    {
                        var grade = vm.SurveyList.Where(d => d.Value.ConvertToInt() == vm.SurveyId).FirstOrDefault();
                        if (grade != null)
                        {
                            surveyName = grade.Text;
                        }
                    }
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                var surveyGroupIds = new List<int>();
                if (vm.SurveyGroupId == 0)
                {
                    surveyGroupIds = vm.SurveyGroupList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGroupIds.Add((int)vm.SurveyGroupId);
                }

                vm.SurveyGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      orderby p.tbClass.tbGrade.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbClass.tbGrade.GradeName,
                                          Value = p.tbClass.tbGrade.Id.ToString()
                                      }).Distinct().ToList();

                if (vm.SurveyGradeId == 0 && vm.SurveyGradeList.Count > 0)
                {
                    vm.SurveyGradeId = vm.SurveyGradeList.FirstOrDefault().Value.ConvertToInt();
                    surveyGradeName = vm.SurveyGradeList.FirstOrDefault().Text;
                }
                else
                {
                    if (vm.SurveyGradeId > decimal.Zero && vm.SurveyGradeList.Count > decimal.Zero)
                    {
                        var grade = vm.SurveyGradeList.Where(d => d.Value.ConvertToInt() == vm.SurveyGradeId).FirstOrDefault();
                        if (grade != null)
                        {
                            surveyGradeName = grade.Text;
                        }
                    }
                }
                //学段
                var yearId = db.Table<Entity.tbSurvey>().Where(d => d.Id == vm.SurveyId).Select(d => d.tbYear.Id).FirstOrDefault();

                vm.SurveyClassList = Basis.Controllers.ClassController.SelectListByYearType(Code.EnumHelper.YearType.Section, yearId, vm.SurveyGradeId ?? 0);

                vm.SurveyClassList = Basis.Controllers.ClassController.SelectListByYearType(Code.EnumHelper.YearType.Section, yearId, vm.SurveyGradeId ?? 0);

                var tbSurveryDataList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                        join m in db.Table<Entity.tbSurveyData>() on p.tbStudent.Id equals m.tbStudent.Id
                                        join c in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals c.tbClass.Id
                                        where m.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && m.tbSurveyItem.tbSurveyGroup.IsOrg
                                        && c.tbSurvey.Id == vm.SurveyId
                                        && p.tbClass.tbGrade.Id == vm.SurveyGradeId
                                        && m.tbClass == null
                                        && m.tbTeacher.IsDeleted == false
                                        && m.tbStudent.IsDeleted == false
                                        && surveyGroupIds.Contains(m.tbSurveyItem.tbSurveyGroup.Id)
                                        && m.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        && m.tbSurveyItem.SurveyItemName.Contains("总体感受") == false
                                        select new
                                        {
                                            OrgId = m.tbOrg.Id,
                                            OrgName = m.tbOrg.OrgName,
                                            OrgNo = m.tbOrg.No,
                                            CourseId = m.tbOrg.tbCourse.Id,
                                            CourseName = m.tbOrg.tbCourse.CourseName,
                                            CourseNo = m.tbOrg.tbCourse.No,
                                            SubjectId = m.tbOrg.tbCourse.tbSubject.Id,
                                            SubjectName = m.tbOrg.tbCourse.tbSubject.SubjectName,
                                            SubjectNo = m.tbOrg.tbCourse.tbSubject.No,
                                            StudentId = m.tbStudent.Id,
                                            StudentName = m.tbStudent.StudentName,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            ClassNo = p.tbClass.No,
                                            SurveyItemId = m.tbSurveyItem.Id,
                                            SurveyItemName = m.tbSurveyItem.SurveyItemName,
                                            SurveyOptionId = m.tbSurveyOption.Id,
                                            SurveyOptionName = m.tbSurveyOption.OptionName,
                                            SurveyOptionValue = m.tbSurveyOption.OptionValue,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            GradeNo = p.tbClass.tbGrade.No,
                                            TeacherId = m.tbTeacher.Id,
                                            TeacherName = m.tbTeacher.TeacherName,
                                        };

                if (vm.SurveyClassId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.ClassId == vm.SurveyClassId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => vm.SearchText.Contains(d.ClassName) || vm.SearchText.Contains(d.SubjectName));
                }

                vm.SurveyGradeOrgList = (from p in tbSurveryDataList
                                         group p by new
                                         {
                                             classId = p.ClassId,
                                             className = p.ClassName,
                                             subjectId = p.SubjectId,
                                             subjectName = p.SubjectName,
                                             surveryItemId = p.SurveyItemId,
                                             surveryItemName = p.SurveyItemName
                                         } into g
                                         select new Dto.SurveyReport.GradeOrg
                                         {
                                             ClassId = g.Key.classId,
                                             ClassName = g.Key.className,
                                             SubjectId = g.Key.subjectId,
                                             SubjectName = g.Key.subjectName,
                                             SurveyItemId = g.Key.surveryItemId,
                                             SurveyItemName = g.Key.surveryItemName,
                                             SurveyOptionCount = g.Count(),
                                             SurveyOptionSum = g.Select(d => d.SurveyOptionValue).Sum(),
                                             SurveyOptionAvg = g.Select(d => d.SurveyOptionValue).Average()
                                         }).Distinct().ToList();

                var tbSubjectIds = vm.SurveyGradeOrgList.Select(d => d.SubjectId).Distinct().ToList();

                vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                     where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                     && surveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                     && p.SurveyItemName.Contains("总体感受") == false
                                     orderby p.tbSurveyGroup.No, p.No
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Text = p.SurveyItemName,
                                         Value = p.Id.ToString()
                                     }).Distinct().ToList();
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();
                if (vm.SurveyClassId == decimal.Zero)
                {
                    #region 年级汇总
                    var orgListGrade = vm.SurveyGradeOrgList.GroupBy(d => new { d.SubjectId, d.SubjectName }).Select(g => new { g.Key.SubjectId, g.Key.SubjectName }).ToList();
                    int surveyOptionCountGrade = orgListGrade.Count();
                    var allColumnLengthGrade = surveyOptionCountGrade * 2 + 3;
                    var arrColumnsGrade = new string[allColumnLengthGrade];
                    for (int i = 0; i < arrColumnsGrade.Length; i++)
                    {
                        arrColumnsGrade[i] = (i + 1).ToString();
                    }
                    var sheetGrade = new Code.NpoiHelper.DataTableToExcelPram();
                    sheetGrade.isColumnWritten = false;
                    sheetGrade.isWriteHeader = true;
                    sheetGrade.strHeaderText = surveyGradeName;
                    //开始表格
                    var dtGrade = Code.Common.ArrayToDataTable(arrColumnsGrade);
                    var regionsGrade = new List<NPOI.SS.Util.CellRangeAddress>();
                    #region 增加标题
                    var arrSurveySubjectIDGrade = new string[allColumnLengthGrade];
                    var arrSurveySubjectNameGrade = new string[allColumnLengthGrade];
                    var arrSurveyOptionIDGrade = new string[allColumnLengthGrade];
                    var arrSurveyOptionNameGrade = new string[allColumnLengthGrade];
                    var indexGrade = 0;
                    for (int i = 0; i < surveyOptionCountGrade; i++)
                    {
                        for (var j = 0; j < 2; j++)
                        {
                            indexGrade++;
                            arrSurveySubjectIDGrade[indexGrade] = orgListGrade[i].SubjectId.ToString();
                            arrSurveySubjectNameGrade[indexGrade] = orgListGrade[i].SubjectName.ToString();
                            if (j == 0)
                            {
                                arrSurveyOptionIDGrade[indexGrade] = "总人数";
                                arrSurveyOptionNameGrade[indexGrade] = "总人数";
                            }
                            else if (j == 1)
                            {
                                arrSurveyOptionIDGrade[indexGrade] = "平均分";
                                arrSurveyOptionNameGrade[indexGrade] = "平均分";
                            }
                        }
                        regionsGrade.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, indexGrade - 1, indexGrade));
                    }
                    dtGrade.Rows.Add(arrSurveySubjectNameGrade);
                    dtGrade.Rows.Add(arrSurveyOptionNameGrade);
                    #endregion
                    #region DataList
                    foreach (var item in vm.SurveyItemList)
                    {
                        string[] arrSurveyData = new string[allColumnLengthGrade];
                        arrSurveyData[0] = item.Text;
                        var optionAllCount = 0;
                        var optionAllAvg = new List<decimal>();
                        var indexR = 1;
                        foreach (var orgTh in vm.SurveyGradeOrgList.GroupBy(d => new { d.SubjectId, d.SubjectName }).Select(g => new { g.Key.SubjectId, g.Key.SubjectName }))
                        {
                            var optionCount = vm.SurveyGradeOrgList.Where(d => d.SubjectId == orgTh.SubjectId && d.SurveyItemName == item.Text)
                                .GroupBy(d => new { d.SubjectId, d.SubjectName }).Select(g => new { g.Key.SubjectId, g.Key.SubjectName, subjectcount = g.Select(d => d.SurveyOptionCount).Sum(), subjectAvg = g.Select(d => d.SurveyOptionAvg).Average() }).FirstOrDefault();
                            if (optionCount != null)
                            {
                                optionAllCount += optionCount.subjectcount;
                                optionAllAvg.Add(optionCount.subjectAvg);
                                arrSurveyData[indexR] = optionCount.subjectcount.ToString();
                                arrSurveyData[indexR + 1] = Decimal.Round(optionCount.subjectAvg, 2).ToString();
                            }
                            indexR = indexR + 2;
                        }
                        arrSurveyData[allColumnLengthGrade - 2] = optionAllCount.ToString();
                        arrSurveyData[allColumnLengthGrade - 1] = Decimal.Round(optionAllAvg.Average(), 2).ToString();
                        dtGrade.Rows.Add(arrSurveyData);
                    }
                    #endregion
                    #region 增加合计
                    var orgAllCountGrade = 0;
                    var orgAllAvgGrade = new List<decimal>();
                    string[] arrSurveyDataCountGrade = new string[allColumnLengthGrade];
                    arrSurveyDataCountGrade[0] = "合计";
                    var indexLGrade = 1;
                    foreach (var orgTh in vm.SurveyGradeOrgList.GroupBy(d => new { d.SubjectId, d.SubjectName })
                        .Select(g => new { g.Key.SubjectId, g.Key.SubjectName, subjectCount = g.Select(d => d.SurveyOptionCount).Sum() }))
                    {
                        var avg = vm.SurveyGradeOrgList.Where(d => d.SubjectId == orgTh.SubjectId)
                            .GroupBy(d => new { d.SubjectId, d.SubjectName, d.SurveyItemName })
                            .Select(g => new { g.Key.SubjectId, g.Key.SubjectName, subjectcount = g.Select(d => d.SurveyOptionCount).Sum(), subjectAvg = g.Select(d => d.SurveyOptionAvg).Average() }).ToList().Select(d => d.subjectAvg).Sum();
                        orgAllCountGrade += orgTh.subjectCount;
                        orgAllAvgGrade.Add(avg);
                        arrSurveyDataCountGrade[indexLGrade] = orgTh.subjectCount.ToString();
                        arrSurveyDataCountGrade[indexLGrade + 1] = Decimal.Round(avg, 2).ToString();
                        indexLGrade = indexLGrade + 2;
                    }
                    arrSurveyDataCountGrade[allColumnLengthGrade - 2] = orgAllCountGrade.ToString();
                    arrSurveyDataCountGrade[allColumnLengthGrade - 1] = Decimal.Round(orgAllAvgGrade.Average(), 2).ToString();
                    dtGrade.Rows.Add(arrSurveyDataCountGrade);
                    #endregion
                    dtGrade.Rows[0][0] = "评价内容";
                    regionsGrade.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));
                    dtGrade.Rows[0][allColumnLengthGrade - 2] = "总人数汇总";
                    regionsGrade.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, allColumnLengthGrade - 2, allColumnLengthGrade - 2));
                    dtGrade.Rows[0][allColumnLengthGrade - 1] = "平均分汇总";
                    regionsGrade.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, allColumnLengthGrade - 1, allColumnLengthGrade - 1));
                    sheetGrade.data = dtGrade;
                    sheetGrade.regions = regionsGrade;
                    sheetList.Add(sheetGrade);
                    #endregion
                }
                #region 分班统计
                foreach (var org in vm.SurveyGradeOrgList.GroupBy(d => new { d.ClassId, d.ClassName }).Select(g => new { classId = g.Key.ClassId, className = g.Key.ClassName }))
                {
                    var orgList = vm.SurveyGradeOrgList.Where(d => d.ClassId == org.classId)
                        .GroupBy(d => new { d.ClassId, d.ClassName, d.SubjectId, d.SubjectName })
                        .Select(g => new { classId = g.Key.ClassId, g.Key.ClassName, g.Key.SubjectId, g.Key.SubjectName }).ToList();

                    int surveyOptionCount = orgList.Count();
                    var allColumnLength = surveyOptionCount * 2 + 3;
                    var arrColumns = new string[allColumnLength];

                    for (int i = 0; i < arrColumns.Length; i++)
                    {
                        arrColumns[i] = (i + 1).ToString();
                    }

                    var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                    sheet.isColumnWritten = false;
                    sheet.isWriteHeader = true;
                    sheet.strHeaderText = org.className;
                    //开始表格
                    var dt = Code.Common.ArrayToDataTable(arrColumns);
                    var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                    #region 增加标题
                    var arrSurveySubjectID = new string[allColumnLength];
                    var arrSurveySubjectName = new string[allColumnLength];
                    var arrSurveyOptionID = new string[allColumnLength];
                    var arrSurveyOptionName = new string[allColumnLength];
                    var index = 0;
                    for (int i = 0; i < surveyOptionCount; i++)
                    {
                        for (var j = 0; j < 2; j++)
                        {
                            index++;
                            arrSurveySubjectID[index] = orgList[i].SubjectId.ToString();
                            arrSurveySubjectName[index] = orgList[i].SubjectName.ToString();
                            if (j == 0)
                            {
                                arrSurveyOptionID[index] = "总人数";
                                arrSurveyOptionName[index] = "总人数";
                            }
                            else if (j == 1)
                            {
                                arrSurveyOptionID[index] = "平均分";
                                arrSurveyOptionName[index] = "平均分";
                            }
                        }
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, index - 1, index));
                    }
                    dt.Rows.Add(arrSurveySubjectName);
                    dt.Rows.Add(arrSurveyOptionName);
                    #endregion
                    #region DataList
                    foreach (var item in vm.SurveyItemList)
                    {
                        string[] arrSurveyData = new string[allColumnLength];
                        arrSurveyData[0] = item.Text;
                        var optionAllCount = 0;
                        var optionAllAvg = new List<decimal>();
                        var indexR = 1;
                        foreach (var orgTh in vm.SurveyGradeOrgList.Where(d => d.ClassId == org.classId).GroupBy(d => new { d.ClassId, d.ClassName, d.SubjectId, d.SubjectName }).Select(g => new { classId = g.Key.ClassId, g.Key.ClassName, g.Key.SubjectId, g.Key.SubjectName }))
                        {
                            var optionCount = vm.SurveyGradeOrgList.Where(d => d.ClassId == orgTh.classId && d.SubjectId == orgTh.SubjectId && d.SurveyItemName == item.Text).FirstOrDefault();
                            if (optionCount != null)
                            {
                                optionAllCount += optionCount.SurveyOptionCount;
                                optionAllAvg.Add(optionCount.SurveyOptionAvg);
                                arrSurveyData[indexR] = optionCount.SurveyOptionCount.ToString();
                                arrSurveyData[indexR + 1] = Decimal.Round(optionCount.SurveyOptionAvg, 2).ToString();
                            }
                            indexR = indexR + 2;
                        }
                        arrSurveyData[allColumnLength - 2] = optionAllCount.ToString();
                        arrSurveyData[allColumnLength - 1] = Decimal.Round(optionAllAvg.Average(), 2).ToString();
                        dt.Rows.Add(arrSurveyData);
                    }
                    #endregion
                    #region 增加合计
                    var orgAllCount = 0;
                    var orgAllAvg = new List<decimal>();
                    string[] arrSurveyDataCount = new string[allColumnLength];
                    arrSurveyDataCount[0] = "合计";
                    var indexL = 1;
                    foreach (var orgTh in vm.SurveyGradeOrgList.Where(d => d.ClassId == org.classId).GroupBy(d => new { d.ClassId, d.ClassName, d.SubjectId, d.SubjectName }).Select(g => new { classId = g.Key.ClassId, g.Key.ClassName, g.Key.SubjectId, g.Key.SubjectName, subjectCount = g.Select(d => d.SurveyOptionCount).Sum(), subjectAvg = g.Select(d => d.SurveyOptionAvg).Sum() }))
                    {
                        orgAllCount += orgTh.subjectCount;
                        orgAllAvg.Add(orgTh.subjectAvg);
                        arrSurveyDataCount[indexL] = orgTh.subjectCount.ToString();
                        arrSurveyDataCount[indexL + 1] = Decimal.Round(orgTh.subjectAvg, 2).ToString();
                        indexL = indexL + 2;
                    }
                    arrSurveyDataCount[allColumnLength - 2] = orgAllCount.ToString();
                    arrSurveyDataCount[allColumnLength - 1] = Decimal.Round(orgAllAvg.Average(), 2).ToString();
                    dt.Rows.Add(arrSurveyDataCount);
                    #endregion
                    dt.Rows[0][0] = "评价内容";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));
                    dt.Rows[0][allColumnLength - 2] = "总人数汇总";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, allColumnLength - 2, allColumnLength - 2));
                    dt.Rows[0][allColumnLength - 1] = "班级均分";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, allColumnLength - 1, allColumnLength - 1));
                    sheet.data = dt;
                    sheet.regions = regions;
                    sheetList.Add(sheet);
                }
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

        public ActionResult GradeSubject()
        {
            var vm = new Models.SurveyReport.GradeSubject();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                var surveyGroupIds = new List<int>();
                if (vm.SurveyGroupId == 0)
                {
                    surveyGroupIds = vm.SurveyGroupList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGroupIds.Add((int)vm.SurveyGroupId);
                }

                vm.SurveyGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      orderby p.tbClass.tbGrade.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbClass.tbGrade.GradeName,
                                          Value = p.tbClass.tbGrade.Id.ToString()
                                      }).Distinct().ToList();

                var tbSurveryDataList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                        join m in db.Table<Entity.tbSurveyData>() on p.tbStudent.Id equals m.tbStudent.Id
                                        join c in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals c.tbClass.Id
                                        where m.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && m.tbSurveyItem.tbSurveyGroup.IsOrg
                                        && c.tbSurvey.Id == vm.SurveyId
                                        && m.tbClass == null
                                        && surveyGroupIds.Contains(m.tbSurveyItem.tbSurveyGroup.Id)
                                        && m.tbTeacher.IsDeleted == false
                                        && m.tbStudent.IsDeleted == false
                                        && m.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        && m.tbSurveyItem.SurveyItemName.Contains("总体感受") == false
                                        select new
                                        {
                                            OrgId = m.tbOrg.Id,
                                            OrgName = m.tbOrg.OrgName,
                                            OrgNo = m.tbOrg.No,
                                            CourseId = m.tbOrg.tbCourse.Id,
                                            CourseName = m.tbOrg.tbCourse.CourseName,
                                            CourseNo = m.tbOrg.tbCourse.No,
                                            SubjectId = m.tbOrg.tbCourse.tbSubject.Id,
                                            SubjectName = m.tbOrg.tbCourse.tbSubject.SubjectName,
                                            SubjectNo = m.tbOrg.tbCourse.tbSubject.No,
                                            StudentId = m.tbStudent.Id,
                                            StudentName = m.tbStudent.StudentName,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            ClassNo = p.tbClass.No,
                                            SurveyItemId = m.tbSurveyItem.Id,
                                            SurveyItemName = m.tbSurveyItem.SurveyItemName,
                                            SurveyOptionId = m.tbSurveyOption.Id,
                                            SurveyOptionName = m.tbSurveyOption.OptionName,
                                            SurveyOptionValue = m.tbSurveyOption.OptionValue,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            GradeNo = p.tbClass.tbGrade.No,
                                            TeacherId = m.tbTeacher.Id,
                                            TeacherName = m.tbTeacher.TeacherName,
                                        };

                if (vm.SurveyGradeId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.GradeId == vm.SurveyGradeId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => vm.SearchText.Contains(d.GradeName) || vm.SearchText.Contains(d.SubjectName));
                }

                vm.SurveyGradeSubjectList = (from p in tbSurveryDataList
                                             group p by new
                                             {
                                                 gradeId = p.GradeId,
                                                 gradeName = p.GradeName,
                                                 subjectId = p.SubjectId,
                                                 subjectName = p.SubjectName,
                                                 surveryItemId = p.SurveyItemId,
                                                 surveryItemName = p.SurveyItemName
                                             } into g
                                             select new Dto.SurveyReport.GradeSubject
                                             {
                                                 GradeId = g.Key.gradeId,
                                                 GradeName = g.Key.gradeName,
                                                 SubjectId = g.Key.subjectId,
                                                 SubjectName = g.Key.subjectName,
                                                 SurveyItemId = g.Key.surveryItemId,
                                                 SurveyItemName = g.Key.surveryItemName,
                                                 SurveyOptionCount = g.Count(),
                                                 SurveyOptionSum = g.Select(d => d.SurveyOptionValue).Sum(),
                                                 SurveyOptionAvg = g.Select(d => d.SurveyOptionValue).Average()
                                             }).Distinct().ToList();

                vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                     where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                     && surveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                     && p.SurveyItemName.Contains("总体感受") == false
                                     orderby p.tbSurveyGroup.No, p.No
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Text = p.SurveyItemName,
                                         Value = p.Id.ToString()
                                     }).Distinct().ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GradeSubject(Models.SurveyReport.GradeSubject vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("GradeSubject", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveyGroupId = vm.SurveyGroupId,
                SurveyGradeId = vm.SurveyGradeId
            }));
        }

        public ActionResult GradeSubjectExport()
        {
            var vm = new Models.SurveyReport.GradeSubject();
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 准备数据
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                var surveyGroupIds = new List<int>();
                if (vm.SurveyGroupId == 0)
                {
                    surveyGroupIds = vm.SurveyGroupList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGroupIds.Add((int)vm.SurveyGroupId);
                }

                vm.SurveyGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      orderby p.tbClass.tbGrade.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbClass.tbGrade.GradeName,
                                          Value = p.tbClass.tbGrade.Id.ToString()
                                      }).Distinct().ToList();

                var tbSurveryDataList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                        join m in db.Table<Entity.tbSurveyData>() on p.tbStudent.Id equals m.tbStudent.Id
                                        join c in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals c.tbClass.Id
                                        where m.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && m.tbSurveyItem.tbSurveyGroup.IsOrg
                                        && c.tbSurvey.Id == vm.SurveyId
                                        && m.tbClass == null
                                        && surveyGroupIds.Contains(m.tbSurveyItem.tbSurveyGroup.Id)
                                        && m.tbTeacher.IsDeleted == false
                                        && m.tbStudent.IsDeleted == false
                                        && m.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        && m.tbSurveyItem.SurveyItemName.Contains("总体感受") == false
                                        select new
                                        {
                                            OrgId = m.tbOrg.Id,
                                            OrgName = m.tbOrg.OrgName,
                                            OrgNo = m.tbOrg.No,
                                            CourseId = m.tbOrg.tbCourse.Id,
                                            CourseName = m.tbOrg.tbCourse.CourseName,
                                            CourseNo = m.tbOrg.tbCourse.No,
                                            SubjectId = m.tbOrg.tbCourse.tbSubject.Id,
                                            SubjectName = m.tbOrg.tbCourse.tbSubject.SubjectName,
                                            SubjectNo = m.tbOrg.tbCourse.tbSubject.No,
                                            StudentId = m.tbStudent.Id,
                                            StudentName = m.tbStudent.StudentName,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            ClassNo = p.tbClass.No,
                                            SurveyItemId = m.tbSurveyItem.Id,
                                            SurveyItemName = m.tbSurveyItem.SurveyItemName,
                                            SurveyOptionId = m.tbSurveyOption.Id,
                                            SurveyOptionName = m.tbSurveyOption.OptionName,
                                            SurveyOptionValue = m.tbSurveyOption.OptionValue,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            GradeNo = p.tbClass.tbGrade.No,
                                            TeacherId = m.tbTeacher.Id,
                                            TeacherName = m.tbTeacher.TeacherName,
                                        };

                if (vm.SurveyGradeId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.GradeId == vm.SurveyGradeId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => vm.SearchText.Contains(d.GradeName) || vm.SearchText.Contains(d.SubjectName));
                }

                vm.SurveyGradeSubjectList = (from p in tbSurveryDataList
                                             group p by new
                                             {
                                                 gradeId = p.GradeId,
                                                 gradeName = p.GradeName,
                                                 subjectId = p.SubjectId,
                                                 subjectName = p.SubjectName,
                                                 surveryItemId = p.SurveyItemId,
                                                 surveryItemName = p.SurveyItemName
                                             } into g
                                             select new Dto.SurveyReport.GradeSubject
                                             {
                                                 GradeId = g.Key.gradeId,
                                                 GradeName = g.Key.gradeName,
                                                 SubjectId = g.Key.subjectId,
                                                 SubjectName = g.Key.subjectName,
                                                 SurveyItemId = g.Key.surveryItemId,
                                                 SurveyItemName = g.Key.surveryItemName,
                                                 SurveyOptionCount = g.Count(),
                                                 SurveyOptionSum = g.Select(d => d.SurveyOptionValue).Sum(),
                                                 SurveyOptionAvg = g.Select(d => d.SurveyOptionValue).Average()
                                             }).Distinct().ToList();

                vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                     where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                     && surveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                     orderby p.tbSurveyGroup.No, p.No
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Text = p.SurveyItemName,
                                         Value = p.Id.ToString()
                                     }).Distinct().ToList();
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();
                #region 分班统计
                foreach (var org in vm.SurveyGradeSubjectList.GroupBy(d => new { d.GradeId, d.GradeName }).Select(g => new { GradeId = g.Key.GradeId, GradeName = g.Key.GradeName }))
                {
                    var orgList = vm.SurveyGradeSubjectList.Where(d => d.GradeId == org.GradeId)
                        .GroupBy(d => new { d.GradeId, d.GradeName, d.SubjectId, d.SubjectName })
                        .Select(g => new { GradeId = g.Key.GradeId, g.Key.GradeName, g.Key.SubjectId, g.Key.SubjectName }).ToList();

                    int surveyOptionCount = orgList.Count();
                    var allColumnLength = surveyOptionCount * 2 + 3;
                    var arrColumns = new string[allColumnLength];

                    for (int i = 0; i < arrColumns.Length; i++)
                    {
                        arrColumns[i] = (i + 1).ToString();
                    }

                    var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                    sheet.isColumnWritten = false;
                    sheet.isWriteHeader = true;
                    sheet.strHeaderText = org.GradeName;
                    //开始表格
                    var dt = Code.Common.ArrayToDataTable(arrColumns);
                    var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                    #region 增加标题
                    var arrSurveySubjectID = new string[allColumnLength];
                    var arrSurveySubjectName = new string[allColumnLength];
                    var arrSurveyOptionID = new string[allColumnLength];
                    var arrSurveyOptionName = new string[allColumnLength];
                    var index = 0;
                    for (int i = 0; i < surveyOptionCount; i++)
                    {
                        for (var j = 0; j < 2; j++)
                        {
                            index++;
                            arrSurveySubjectID[index] = orgList[i].SubjectId.ToString();
                            arrSurveySubjectName[index] = orgList[i].SubjectName.ToString();
                            if (j == 0)
                            {
                                arrSurveyOptionID[index] = "总人数";
                                arrSurveyOptionName[index] = "总人数";
                            }
                            else if (j == 1)
                            {
                                arrSurveyOptionID[index] = "平均分";
                                arrSurveyOptionName[index] = "平均分";
                            }
                        }
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, index - 1, index));
                    }
                    dt.Rows.Add(arrSurveySubjectName);
                    dt.Rows.Add(arrSurveyOptionName);
                    #endregion
                    #region DataList
                    foreach (var item in vm.SurveyItemList)
                    {
                        string[] arrSurveyData = new string[allColumnLength];
                        arrSurveyData[0] = item.Text;
                        var optionAllCount = 0;
                        var optionAllAvg = 0m;
                        var indexR = 1;
                        foreach (var orgTh in vm.SurveyGradeSubjectList.Where(d => d.GradeId == org.GradeId).GroupBy(d => new { d.GradeId, d.GradeName, d.SubjectId, d.SubjectName }).Select(g => new { GradeId = g.Key.GradeId, g.Key.GradeName, g.Key.SubjectId, g.Key.SubjectName }))
                        {
                            var optionCount = vm.SurveyGradeSubjectList.Where(d => d.GradeId == orgTh.GradeId && d.SubjectId == orgTh.SubjectId && d.SurveyItemName == item.Text).FirstOrDefault();
                            if (optionCount != null)
                            {
                                optionAllCount += optionCount.SurveyOptionCount;
                                optionAllAvg += optionCount.SurveyOptionAvg;
                                arrSurveyData[indexR] = optionCount.SurveyOptionCount.ToString();
                                arrSurveyData[indexR + 1] = Decimal.Round(optionCount.SurveyOptionAvg, 2).ToString();
                            }
                            indexR = indexR + 2;
                        }
                        arrSurveyData[allColumnLength - 2] = optionAllCount.ToString();
                        arrSurveyData[allColumnLength - 1] = Decimal.Round(optionAllAvg, 2).ToString();
                        dt.Rows.Add(arrSurveyData);
                    }
                    #endregion
                    #region 增加合计
                    var orgAllCount = 0;
                    var orgAllAvg = 0m;
                    string[] arrSurveyDataCount = new string[allColumnLength];
                    arrSurveyDataCount[0] = "合计";
                    var indexL = 1;
                    foreach (var orgTh in vm.SurveyGradeSubjectList.Where(d => d.GradeId == org.GradeId).GroupBy(d => new { d.GradeId, d.GradeName, d.SubjectId, d.SubjectName }).Select(g => new { GradeId = g.Key.GradeId, g.Key.GradeName, g.Key.SubjectId, g.Key.SubjectName, subjectCount = g.Select(d => d.SurveyOptionCount).Sum(), subjectAvg = g.Select(d => d.SurveyOptionAvg).Sum() }))
                    {
                        orgAllCount += orgTh.subjectCount;
                        orgAllAvg += orgTh.subjectAvg;
                        arrSurveyDataCount[indexL] = orgTh.subjectCount.ToString();
                        arrSurveyDataCount[indexL + 1] = Decimal.Round(orgTh.subjectAvg, 2).ToString();
                        indexL = indexL + 2;
                    }
                    arrSurveyDataCount[allColumnLength - 2] = orgAllCount.ToString();
                    arrSurveyDataCount[allColumnLength - 1] = Decimal.Round(orgAllAvg, 2).ToString();
                    dt.Rows.Add(arrSurveyDataCount);
                    #endregion
                    dt.Rows[0][0] = "评价内容";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));
                    dt.Rows[0][allColumnLength - 2] = "总人数汇总";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, allColumnLength - 2, allColumnLength - 2));
                    dt.Rows[0][allColumnLength - 1] = "班级均分";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, allColumnLength - 1, allColumnLength - 1));
                    sheet.data = dt;
                    sheet.regions = regions;
                    sheetList.Add(sheet);
                }
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

        public ActionResult GradeClass()
        {
            var vm = new Models.SurveyReport.GradeClass();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                var surveyGroupIds = new List<int>();
                if (vm.SurveyGroupId == 0)
                {
                    surveyGroupIds = vm.SurveyGroupList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGroupIds.Add((int)vm.SurveyGroupId);
                }

                vm.SurveyGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      orderby p.tbClass.tbGrade.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbClass.tbGrade.GradeName,
                                          Value = p.tbClass.tbGrade.Id.ToString()
                                      }).Distinct().ToList();

                var tbSurveryDataList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                        join m in db.Table<Entity.tbSurveyData>() on p.tbStudent.Id equals m.tbStudent.Id
                                        join c in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals c.tbClass.Id
                                        where m.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && m.tbSurveyItem.tbSurveyGroup.IsOrg
                                        && c.tbSurvey.Id == vm.SurveyId
                                        && m.tbClass == null
                                        && m.tbTeacher.IsDeleted == false
                                        && surveyGroupIds.Contains(m.tbSurveyItem.tbSurveyGroup.Id)
                                        && m.tbStudent.IsDeleted == false
                                        && m.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        && m.tbSurveyItem.SurveyItemName.Contains("总体感受") == false
                                        select new
                                        {
                                            OrgId = m.tbOrg.Id,
                                            OrgName = m.tbOrg.OrgName,
                                            OrgNo = m.tbOrg.No,
                                            CourseId = m.tbOrg.tbCourse.Id,
                                            CourseName = m.tbOrg.tbCourse.CourseName,
                                            CourseNo = m.tbOrg.tbCourse.No,
                                            SubjectId = m.tbOrg.tbCourse.tbSubject.Id,
                                            SubjectName = m.tbOrg.tbCourse.tbSubject.SubjectName,
                                            SubjectNo = m.tbOrg.tbCourse.tbSubject.No,
                                            StudentId = m.tbStudent.Id,
                                            StudentName = m.tbStudent.StudentName,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            ClassNo = p.tbClass.No,
                                            SurveyItemId = m.tbSurveyItem.Id,
                                            SurveyItemName = m.tbSurveyItem.SurveyItemName,
                                            SurveyOptionId = m.tbSurveyOption.Id,
                                            SurveyOptionName = m.tbSurveyOption.OptionName,
                                            SurveyOptionValue = m.tbSurveyOption.OptionValue,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            GradeNo = p.tbClass.tbGrade.No,
                                            TeacherId = m.tbTeacher.Id,
                                            TeacherName = m.tbTeacher.TeacherName,
                                        };

                if (vm.SurveyGradeId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.GradeId == vm.SurveyGradeId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => vm.SearchText.Contains(d.GradeName) || vm.SearchText.Contains(d.SubjectName));
                }

                vm.SurveyGradeClassList = (from p in tbSurveryDataList
                                           group p by new
                                           {
                                               gradeId = p.GradeId,
                                               gradeName = p.GradeName,
                                               gradeNo = p.GradeNo,
                                               classId = p.ClassId,
                                               className = p.ClassName,
                                               classNo = p.ClassNo,
                                               subjectId = p.SubjectId,
                                               subjectName = p.SubjectName,
                                               subjectNo = p.SubjectNo,
                                               itemId = p.SurveyItemId
                                           } into g
                                           select new Dto.SurveyReport.GradeClass
                                           {
                                               GradeId = g.Key.gradeId,
                                               GradeName = g.Key.gradeName,
                                               GradeNo = g.Key.gradeNo,
                                               ClassId = g.Key.classId,
                                               ClassName = g.Key.className,
                                               ClassNo = g.Key.classNo,
                                               SubjectId = g.Key.subjectId,
                                               SubjectName = g.Key.subjectName,
                                               SubjectNo = g.Key.subjectNo,
                                               SurveyItemId = g.Key.itemId,
                                               SurveyOptionCount = g.Count(),
                                               SurveyOptionSum = g.Select(d => d.SurveyOptionValue).Sum(),
                                               SurveyOptionAvg = g.Select(d => d.SurveyOptionValue).Average()
                                           }).Distinct().ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GradeClass(Models.SurveyReport.GradeClass vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("GradeClass", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveyGroupId = vm.SurveyGroupId,
                SurveyGradeId = vm.SurveyGradeId
            }));
        }

        public ActionResult GradeClassExport()
        {
            var vm = new Models.SurveyReport.GradeClass();
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 准备数据
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                var surveyGroupIds = new List<int>();
                if (vm.SurveyGroupId == 0)
                {
                    surveyGroupIds = vm.SurveyGroupList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGroupIds.Add((int)vm.SurveyGroupId);
                }

                vm.SurveyGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      orderby p.tbClass.tbGrade.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbClass.tbGrade.GradeName,
                                          Value = p.tbClass.tbGrade.Id.ToString()
                                      }).Distinct().ToList();

                var tbSurveryDataList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                        join m in db.Table<Entity.tbSurveyData>() on p.tbStudent.Id equals m.tbStudent.Id
                                        join c in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals c.tbClass.Id
                                        where m.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && m.tbSurveyItem.tbSurveyGroup.IsOrg
                                        && c.tbSurvey.Id == vm.SurveyId
                                        && m.tbClass == null
                                        && m.tbTeacher.IsDeleted == false
                                        && surveyGroupIds.Contains(m.tbSurveyItem.tbSurveyGroup.Id)
                                        && m.tbStudent.IsDeleted == false
                                        && m.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        && m.tbSurveyItem.SurveyItemName.Contains("总体感受") == false
                                        select new
                                        {
                                            OrgId = m.tbOrg.Id,
                                            OrgName = m.tbOrg.OrgName,
                                            OrgNo = m.tbOrg.No,
                                            CourseId = m.tbOrg.tbCourse.Id,
                                            CourseName = m.tbOrg.tbCourse.CourseName,
                                            CourseNo = m.tbOrg.tbCourse.No,
                                            SubjectId = m.tbOrg.tbCourse.tbSubject.Id,
                                            SubjectName = m.tbOrg.tbCourse.tbSubject.SubjectName,
                                            SubjectNo = m.tbOrg.tbCourse.tbSubject.No,
                                            StudentId = m.tbStudent.Id,
                                            StudentName = m.tbStudent.StudentName,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            ClassNo = p.tbClass.No,
                                            SurveyItemId = m.tbSurveyItem.Id,
                                            SurveyItemName = m.tbSurveyItem.SurveyItemName,
                                            SurveyOptionId = m.tbSurveyOption.Id,
                                            SurveyOptionName = m.tbSurveyOption.OptionName,
                                            SurveyOptionValue = m.tbSurveyOption.OptionValue,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            GradeNo = p.tbClass.tbGrade.No,
                                            TeacherId = m.tbTeacher.Id,
                                            TeacherName = m.tbTeacher.TeacherName,
                                        };

                if (vm.SurveyGradeId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.GradeId == vm.SurveyGradeId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => vm.SearchText.Contains(d.GradeName) || vm.SearchText.Contains(d.SubjectName));
                }

                vm.SurveyGradeClassList = (from p in tbSurveryDataList
                                           group p by new
                                           {
                                               gradeId = p.GradeId,
                                               gradeName = p.GradeName,
                                               gradeNo = p.GradeNo,
                                               classId = p.ClassId,
                                               className = p.ClassName,
                                               classNo = p.ClassNo,
                                               subjectId = p.SubjectId,
                                               subjectName = p.SubjectName,
                                               subjectNo = p.SubjectNo,
                                               studentId = p.StudentId,
                                               studentName = p.StudentName
                                           } into g
                                           select new Dto.SurveyReport.GradeClass
                                           {
                                               GradeId = g.Key.gradeId,
                                               GradeName = g.Key.gradeName,
                                               GradeNo = g.Key.gradeNo,
                                               ClassId = g.Key.classId,
                                               ClassName = g.Key.className,
                                               ClassNo = g.Key.classNo,
                                               SubjectId = g.Key.subjectId,
                                               SubjectName = g.Key.subjectName,
                                               SubjectNo = g.Key.subjectNo,
                                               StudentId = g.Key.studentId,
                                               StudentName = g.Key.studentName,
                                               SurveyOptionCount = g.Count(),
                                               SurveyOptionSum = g.Select(d => d.SurveyOptionValue).Sum(),
                                               SurveyOptionAvg = g.Select(d => d.SurveyOptionValue).Average()
                                           }).Distinct().ToList();
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();
                #region 分班统计
                foreach (var org in vm.SurveyGradeClassList.GroupBy(d => new { d.GradeId, d.GradeName }).Select(g => new { GradeId = g.Key.GradeId, GradeName = g.Key.GradeName }))
                {
                    var orgList = vm.SurveyGradeClassList.Where(d => d.GradeId == org.GradeId)
                        .GroupBy(d => new { d.GradeId, d.GradeName, d.SubjectId, d.SubjectName })
                        .Select(g => new { GradeId = g.Key.GradeId, g.Key.GradeName, g.Key.SubjectId, g.Key.SubjectName }).ToList();

                    int surveyOptionCount = orgList.Count();
                    var allColumnLength = surveyOptionCount + 2;
                    var arrColumns = new string[allColumnLength];

                    for (int i = 0; i < arrColumns.Length; i++)
                    {
                        arrColumns[i] = (i + 1).ToString();
                    }

                    var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                    sheet.isColumnWritten = false;
                    sheet.isWriteHeader = true;
                    sheet.strHeaderText = org.GradeName;
                    //开始表格
                    var dt = Code.Common.ArrayToDataTable(arrColumns);
                    var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                    #region 增加标题
                    var arrSurveySubjectID = new string[allColumnLength];
                    var arrSurveySubjectName = new string[allColumnLength];
                    var index = 0;
                    for (int i = 0; i < surveyOptionCount; i++)
                    {
                        index++;
                        arrSurveySubjectID[index] = orgList[i].SubjectId.ToString();
                        arrSurveySubjectName[index] = orgList[i].SubjectName.ToString();
                    }
                    dt.Rows.Add(arrSurveySubjectName);
                    #endregion
                    #region DataList
                    foreach (var classItem in vm.SurveyGradeClassList.Where(d => d.GradeId == org.GradeId).GroupBy(d => new { d.GradeId, d.GradeName, d.ClassId, d.ClassName }).Select(g => new { GradeId = g.Key.GradeId, g.Key.GradeName, g.Key.ClassId, g.Key.ClassName }))
                    {
                        string[] arrSurveyData = new string[allColumnLength];
                        arrSurveyData[0] = classItem.ClassName;
                        var optionAllCount = 0;
                        var optionAllSum = 0m;
                        var indexR = 1;
                        foreach (var orgTh in vm.SurveyGradeClassList.Where(d => d.GradeId == org.GradeId)
                            .GroupBy(d => new { d.GradeId, d.GradeName, d.SubjectId, d.SubjectName })
                            .Select(g => new { GradeId = g.Key.GradeId, g.Key.GradeName, g.Key.SubjectId, g.Key.SubjectName }))
                        {
                            var optionCount = vm.SurveyGradeClassList.Where(d => d.GradeId == orgTh.GradeId && d.ClassId == classItem.ClassId && d.SubjectId == orgTh.SubjectId)
                                            .GroupBy(d => new { d.StudentId }).
                                            Select(g => new { g.Key.StudentId, SurveyOptionAvg = g.Select(d => d.SurveyOptionSum).Average() }).FirstOrDefault();
                            if (optionCount != null)
                            {
                                optionAllSum += optionCount.SurveyOptionAvg;
                                arrSurveyData[indexR] = Decimal.Round(optionCount.SurveyOptionAvg, 2).ToString();
                            }
                            indexR++;
                            optionAllCount++;
                        }
                        if (optionAllCount > decimal.Zero)
                        {
                            arrSurveyData[allColumnLength - 1] = Decimal.Round(optionAllSum / optionAllCount, 2).ToString();
                        }
                        dt.Rows.Add(arrSurveyData);
                    }
                    #endregion
                    #region 增加合计
                    var orgAllCount = 0;
                    var orgAllSum = 0m;
                    string[] arrSurveyDataCount = new string[allColumnLength];
                    arrSurveyDataCount[0] = "年级均分";
                    var indexL = 1;
                    foreach (var orgTh in vm.SurveyGradeClassList.Where(d => d.GradeId == org.GradeId)
                            .GroupBy(d => new { d.GradeId, d.GradeName, d.SubjectId, d.SubjectName })
                            .Select(g => new
                            {
                                GradeId = g.Key.GradeId,
                                g.Key.GradeName,
                                g.Key.SubjectId,
                                g.Key.SubjectName
                            }))
                    {
                        var orgThe = vm.SurveyGradeClassList.Where(d => d.GradeId == org.GradeId && d.SubjectId == orgTh.SubjectId)
                        .GroupBy(d => new { d.GradeId, d.GradeName, d.SubjectId, d.SubjectName, d.StudentId })
                        .Select(g => new
                        {
                            GradeId = g.Key.GradeId,
                            g.Key.GradeName,
                            g.Key.SubjectId,
                            g.Key.SubjectName,
                            subjectCount = g.Select(d => d.SurveyOptionCount).Sum(),
                            subjectAvg = g.Select(d => d.SurveyOptionSum).Average()
                        }).FirstOrDefault();
                        if (orgThe != null)
                        {
                            orgAllSum += orgThe.subjectAvg;
                            arrSurveyDataCount[indexL] = Decimal.Round(orgThe.subjectAvg, 2).ToString();
                            orgAllCount++;
                        }
                        indexL++;
                    }
                    if (orgAllCount > decimal.Zero)
                    {
                        arrSurveyDataCount[allColumnLength - 1] = Decimal.Round(orgAllSum / orgAllCount, 2).ToString();
                    }
                    dt.Rows.Add(arrSurveyDataCount);
                    #endregion
                    dt.Rows[0][0] = "班级";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, 0, 0));
                    dt.Rows[0][allColumnLength - 1] = "班级均分";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, allColumnLength - 1, allColumnLength - 1));
                    sheet.data = dt;
                    sheet.regions = regions;
                    sheetList.Add(sheet);
                }
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

        public ActionResult GradeTeacher()
        {
            var vm = new Models.SurveyReport.GradeTeacher();
            using (var db = new XkSystem.Models.DbContext())
            {
                db.Database.CommandTimeout = 0;
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                var surveyGroupIds = new List<int>();
                if (vm.SurveyGroupId == 0)
                {
                    surveyGroupIds = vm.SurveyGroupList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGroupIds.Add((int)vm.SurveyGroupId);
                }

                vm.SurveyGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      orderby p.tbClass.tbGrade.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbClass.tbGrade.GradeName,
                                          Value = p.tbClass.tbGrade.Id.ToString()
                                      }).Distinct().ToList();

                var tbSurveryDataList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                        join m in db.Table<Entity.tbSurveyData>() on p.tbStudent.Id equals m.tbStudent.Id
                                        join c in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals c.tbClass.Id
                                        where m.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && m.tbSurveyItem.tbSurveyGroup.IsOrg
                                        && c.tbSurvey.Id == vm.SurveyId
                                        && m.tbClass == null
                                        && m.tbTeacher.IsDeleted == false
                                        && m.tbStudent.IsDeleted == false
                                        && surveyGroupIds.Contains(m.tbSurveyItem.tbSurveyGroup.Id)
                                        && m.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        && m.tbSurveyItem.SurveyItemName.Contains("总体感受") == false
                                        select new
                                        {
                                            OrgId = m.tbOrg.Id,
                                            OrgName = m.tbOrg.OrgName,
                                            OrgNo = m.tbOrg.No,
                                            CourseId = m.tbOrg.tbCourse.Id,
                                            CourseName = m.tbOrg.tbCourse.CourseName,
                                            CourseNo = m.tbOrg.tbCourse.No,
                                            SubjectId = m.tbOrg.tbCourse.tbSubject.Id,
                                            SubjectName = m.tbOrg.tbCourse.tbSubject.SubjectName,
                                            SubjectNo = m.tbOrg.tbCourse.tbSubject.No,
                                            StudentId = m.tbStudent.Id,
                                            StudentName = m.tbStudent.StudentName,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            ClassNo = p.tbClass.No,
                                            SurveyItemId = m.tbSurveyItem.Id,
                                            SurveyItemName = m.tbSurveyItem.SurveyItemName,
                                            SurveyOptionId = m.tbSurveyOption.Id,
                                            SurveyOptionName = m.tbSurveyOption.OptionName,
                                            SurveyOptionValue = m.tbSurveyOption.OptionValue,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            GradeNo = p.tbClass.tbGrade.No,
                                            TeacherId = m.tbTeacher.Id,
                                            TeacherName = m.tbTeacher.TeacherName,
                                        };

                if (vm.SurveyGradeId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.GradeId == vm.SurveyGradeId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => vm.SearchText.Contains(d.GradeName) || vm.SearchText.Contains(d.SubjectName));
                }

                vm.SurveyGradeTeacherList = (from p in tbSurveryDataList
                                             group p by new
                                             {
                                                 gradeId = p.GradeId,
                                                 gradeName = p.GradeName,
                                                 gradeNo = p.GradeNo,
                                                 classId = p.ClassId,
                                                 className = p.ClassName,
                                                 classNo = p.ClassNo,
                                                 subjectId = p.SubjectId,
                                                 subjectName = p.SubjectName,
                                                 subjectNo = p.SubjectNo,
                                                 teacherId = p.TeacherId,
                                                 teacherName = p.TeacherName,
                                                 itemId = p.SurveyItemId
                                             } into g
                                             select new Dto.SurveyReport.GradeTeacher
                                             {
                                                 GradeId = g.Key.gradeId,
                                                 GradeName = g.Key.gradeName,
                                                 GradeNo = g.Key.gradeNo,
                                                 ClassId = g.Key.classId,
                                                 ClassName = g.Key.className,
                                                 ClassNo = g.Key.classNo,
                                                 SubjectId = g.Key.subjectId,
                                                 SubjectName = g.Key.subjectName,
                                                 SubjectNo = g.Key.subjectNo,
                                                 TeacherId = g.Key.teacherId,
                                                 TeacherName = g.Key.teacherName,
                                                 SurveyItemId = g.Key.itemId,
                                                 SurveyOptionCount = g.Count(),
                                                 SurveyOptionSum = g.Select(d => d.SurveyOptionValue).Sum(),
                                                 SurveyOptionAvg = g.Average(d=>d.SurveyOptionValue)
                                             }).Distinct().ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GradeTeacher(Models.SurveyReport.GradeTeacher vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("GradeTeacher", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveyGroupId = vm.SurveyGroupId,
                SurveyGradeId = vm.SurveyGradeId
            }));
        }

        public ActionResult GradeTeacherExport()
        {
            var vm = new Models.SurveyReport.GradeTeacher();
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 准备数据
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                var surveyGroupIds = new List<int>();
                if (vm.SurveyGroupId == 0)
                {
                    surveyGroupIds = vm.SurveyGroupList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGroupIds.Add((int)vm.SurveyGroupId);
                }

                vm.SurveyGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      orderby p.tbClass.tbGrade.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbClass.tbGrade.GradeName,
                                          Value = p.tbClass.tbGrade.Id.ToString()
                                      }).Distinct().ToList();

                var tbSurveryDataList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                        join m in db.Table<Entity.tbSurveyData>() on p.tbStudent.Id equals m.tbStudent.Id
                                        join c in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals c.tbClass.Id
                                        where m.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && m.tbSurveyItem.tbSurveyGroup.IsOrg
                                        && c.tbSurvey.Id == vm.SurveyId
                                        && m.tbClass == null
                                        && m.tbTeacher.IsDeleted == false
                                        && m.tbStudent.IsDeleted == false
                                        && surveyGroupIds.Contains(m.tbSurveyItem.tbSurveyGroup.Id)
                                        && m.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        && m.tbSurveyItem.SurveyItemName.Contains("总体感受") == false
                                        select new
                                        {
                                            OrgId = m.tbOrg.Id,
                                            OrgName = m.tbOrg.OrgName,
                                            OrgNo = m.tbOrg.No,
                                            CourseId = m.tbOrg.tbCourse.Id,
                                            CourseName = m.tbOrg.tbCourse.CourseName,
                                            CourseNo = m.tbOrg.tbCourse.No,
                                            SubjectId = m.tbOrg.tbCourse.tbSubject.Id,
                                            SubjectName = m.tbOrg.tbCourse.tbSubject.SubjectName,
                                            SubjectNo = m.tbOrg.tbCourse.tbSubject.No,
                                            StudentId = m.tbStudent.Id,
                                            StudentName = m.tbStudent.StudentName,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            ClassNo = p.tbClass.No,
                                            SurveyItemId = m.tbSurveyItem.Id,
                                            SurveyItemName = m.tbSurveyItem.SurveyItemName,
                                            SurveyOptionId = m.tbSurveyOption.Id,
                                            SurveyOptionName = m.tbSurveyOption.OptionName,
                                            SurveyOptionValue = m.tbSurveyOption.OptionValue,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            GradeNo = p.tbClass.tbGrade.No,
                                            TeacherId = m.tbTeacher.Id,
                                            TeacherName = m.tbTeacher.TeacherName,
                                        };

                if (vm.SurveyGradeId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.GradeId == vm.SurveyGradeId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => vm.SearchText.Contains(d.GradeName) || vm.SearchText.Contains(d.SubjectName));
                }

                vm.SurveyGradeTeacherList = (from p in tbSurveryDataList
                                             group p by new
                                             {
                                                 gradeId = p.GradeId,
                                                 gradeName = p.GradeName,
                                                 gradeNo = p.GradeNo,
                                                 classId = p.ClassId,
                                                 className = p.ClassName,
                                                 classNo = p.ClassNo,
                                                 subjectId = p.SubjectId,
                                                 subjectName = p.SubjectName,
                                                 subjectNo = p.SubjectNo,
                                                 teacherId = p.TeacherId,
                                                 teacherName = p.TeacherName,
                                                 studentId = p.StudentId,
                                                 studentName = p.StudentName
                                             } into g
                                             select new Dto.SurveyReport.GradeTeacher
                                             {
                                                 GradeId = g.Key.gradeId,
                                                 GradeName = g.Key.gradeName,
                                                 GradeNo = g.Key.gradeNo,
                                                 ClassId = g.Key.classId,
                                                 ClassName = g.Key.className,
                                                 ClassNo = g.Key.classNo,
                                                 SubjectId = g.Key.subjectId,
                                                 SubjectName = g.Key.subjectName,
                                                 SubjectNo = g.Key.subjectNo,
                                                 TeacherId = g.Key.teacherId,
                                                 TeacherName = g.Key.teacherName,
                                                 StudentId = g.Key.studentId,
                                                 StudentName = g.Key.studentName,
                                                 SurveyOptionCount = g.Count(),
                                                 SurveyOptionSum = g.Select(d => d.SurveyOptionValue).Sum(),
                                                 SurveyOptionAvg = g.Select(d => d.SurveyOptionValue).Average()
                                             }).Distinct().ToList();
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();
                #region 分班统计
                foreach (var org in vm.SurveyGradeTeacherList.GroupBy(d => new { d.GradeId, d.GradeName }).Select(g => new { GradeId = g.Key.GradeId, GradeName = g.Key.GradeName }))
                {
                    var orgList = vm.SurveyGradeTeacherList.Where(d => d.GradeId == org.GradeId)
                        .GroupBy(d => new { d.GradeId, d.GradeName, d.ClassId, d.ClassName })
                        .Select(g => new { GradeId = g.Key.GradeId, g.Key.GradeName, g.Key.ClassId, g.Key.ClassName }).ToList();

                    int surveyOptionCount = orgList.Count();
                    var allColumnLength = surveyOptionCount + 5;
                    var arrColumns = new string[allColumnLength];

                    for (int i = 0; i < arrColumns.Length; i++)
                    {
                        arrColumns[i] = (i + 1).ToString();
                    }

                    var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                    sheet.isColumnWritten = false;
                    sheet.isWriteHeader = true;
                    sheet.strHeaderText = org.GradeName;
                    //开始表格
                    var dt = Code.Common.ArrayToDataTable(arrColumns);
                    var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                    #region 增加标题
                    var arrSurveyClassID = new string[allColumnLength];
                    var arrSurveyClassName = new string[allColumnLength];
                    var arrSurveyClassAvg = new string[allColumnLength];
                    var index = 1;
                    var classAvg = vm.SurveyGradeTeacherList.Where(d => d.GradeId == org.GradeId).GroupBy(d => new { d.GradeId, d.GradeName, d.ClassId, d.ClassName }).Select(g => new { GradeId = g.Key.GradeId, g.Key.GradeName, g.Key.ClassId, g.Key.ClassName, classAvg = g.Select(d => d.SurveyOptionSum).Average() }).ToList();
                    for (int i = 0; i < surveyOptionCount; i++)
                    {
                        index++;
                        arrSurveyClassID[index] = orgList[i].ClassId.ToString();
                        arrSurveyClassName[index] = orgList[i].ClassName.ToString();
                        arrSurveyClassAvg[index] = Decimal.Round(classAvg[i].classAvg, 2).ToString();
                    }
                    dt.Rows.Add(arrSurveyClassAvg);
                    dt.Rows.Add(arrSurveyClassName);
                    #endregion
                    #region DataList
                    foreach (var classItem in vm.SurveyGradeTeacherList.Where(d => d.GradeId == org.GradeId).GroupBy(d => new { d.GradeId, d.GradeName, d.SubjectId, d.SubjectName, d.TeacherId, d.TeacherName }).Select(g => new { GradeId = g.Key.GradeId, g.Key.GradeName, g.Key.SubjectId, g.Key.SubjectName, g.Key.TeacherId, g.Key.TeacherName }))
                    {
                        string[] arrSurveyData = new string[allColumnLength];
                        arrSurveyData[0] = classItem.SubjectName;
                        arrSurveyData[1] = classItem.TeacherName;
                        var optionAllCount = 0;
                        var optionAllSum = 0m;
                        var optionAllAvg = 0m;
                        var indexR = 2;
                        foreach (var orgTh in vm.SurveyGradeTeacherList.Where(d => d.GradeId == org.GradeId).GroupBy(d => new { d.GradeId, d.GradeName, d.ClassId, d.ClassName }).Select(g => new { GradeId = g.Key.GradeId, g.Key.GradeName, g.Key.ClassId, g.Key.ClassName }))
                        {
                            var optionCount = vm.SurveyGradeTeacherList
                                    .Where(d => d.GradeId == orgTh.GradeId && d.ClassId == orgTh.ClassId && d.SubjectId == classItem.SubjectId && d.TeacherId == classItem.TeacherId)
                                    .GroupBy(d => new { d.TeacherId })
                                    .Select(g => new { g.Key.TeacherId, SurveyOptionAvg = g.Select(d => d.SurveyOptionSum).Average() }).FirstOrDefault();

                            if (optionCount != null)
                            {
                                optionAllSum += optionCount.SurveyOptionAvg;
                                arrSurveyData[indexR] = Decimal.Round(optionCount.SurveyOptionAvg, 2).ToString();
                                optionAllCount++;
                            }
                            indexR++;
                        }
                        if (optionAllCount > decimal.Zero)
                        {
                            optionAllAvg = Decimal.Round(optionAllSum / optionAllCount, 2);
                            arrSurveyData[allColumnLength - 3] = optionAllAvg.ToString();
                        }
                        var averageRate = 0m;
                        var classAvgL = vm.SurveyGradeTeacherList.Where(d => d.GradeId == org.GradeId)
                            .GroupBy(d => new { d.GradeId, d.GradeName, d.ClassId, d.ClassName })
                            .Select(g => new { GradeId = g.Key.GradeId, g.Key.GradeName, g.Key.ClassId, g.Key.ClassName, classAvg = g.Select(d => d.SurveyOptionSum).Average() }).ToList();
                        var myClassIds = vm.SurveyGradeTeacherList.Where(d => d.GradeId == org.GradeId && d.TeacherId == classItem.TeacherId)
                            .GroupBy(d => new { d.GradeId, d.ClassId }).Select(g => g.Key.ClassId).ToList();
                        var MyClassAvg = classAvgL.Where(d => myClassIds.Contains(d.ClassId)).Select(d => d.classAvg).Average();

                        arrSurveyData[allColumnLength - 2] = Decimal.Round(MyClassAvg, 2).ToString();
                        if (MyClassAvg > decimal.Zero)
                        {
                            averageRate = (optionAllAvg - MyClassAvg) / MyClassAvg * (decimal)100.0;
                            arrSurveyData[allColumnLength - 1] = Decimal.Round(averageRate, 2).ToString() + "%";
                        }
                        dt.Rows.Add(arrSurveyData);
                    }
                    #endregion                    
                    dt.Rows[0][0] = "班级均分";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, 0, 1));
                    dt.Rows[1][0] = "科目";
                    dt.Rows[1][1] = "任课教师";
                    dt.Rows[0][allColumnLength - 3] = "教师均分";
                    dt.Rows[0][allColumnLength - 2] = "所教班级均分";
                    dt.Rows[0][allColumnLength - 1] = "离均率";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, allColumnLength - 3, allColumnLength - 3));
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, allColumnLength - 2, allColumnLength - 2));
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, allColumnLength - 1, allColumnLength - 1));
                    sheet.data = dt;
                    sheet.regions = regions;
                    sheetList.Add(sheet);
                }
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

        public ActionResult GradeTeacherRanking()
        {
            var vm = new Models.SurveyReport.GradeTeacherRanking();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                var surveyGroupIds = new List<int>();
                if (vm.SurveyGroupId == 0)
                {
                    surveyGroupIds = vm.SurveyGroupList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGroupIds.Add((int)vm.SurveyGroupId);
                }

                vm.SurveyGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      orderby p.tbClass.tbGrade.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbClass.tbGrade.GradeName,
                                          Value = p.tbClass.tbGrade.Id.ToString()
                                      }).Distinct().ToList();

                var tbSurveryDataList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                        join m in db.Table<Entity.tbSurveyData>() on p.tbStudent.Id equals m.tbStudent.Id
                                        join c in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals c.tbClass.Id
                                        where m.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && m.tbSurveyItem.tbSurveyGroup.IsOrg
                                        && c.tbSurvey.Id == vm.SurveyId
                                        && m.tbClass == null
                                        && m.tbTeacher.IsDeleted == false
                                        && m.tbStudent.IsDeleted == false
                                        && surveyGroupIds.Contains(m.tbSurveyItem.tbSurveyGroup.Id)
                                        && m.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        && m.tbSurveyItem.SurveyItemName.Contains("总体感受") == false
                                        select new
                                        {
                                            OrgId = m.tbOrg.Id,
                                            OrgName = m.tbOrg.OrgName,
                                            OrgNo = m.tbOrg.No,
                                            CourseId = m.tbOrg.tbCourse.Id,
                                            CourseName = m.tbOrg.tbCourse.CourseName,
                                            CourseNo = m.tbOrg.tbCourse.No,
                                            SubjectId = m.tbOrg.tbCourse.tbSubject.Id,
                                            SubjectName = m.tbOrg.tbCourse.tbSubject.SubjectName,
                                            SubjectNo = m.tbOrg.tbCourse.tbSubject.No,
                                            StudentId = m.tbStudent.Id,
                                            StudentName = m.tbStudent.StudentName,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            ClassNo = p.tbClass.No,
                                            SurveyItemId = m.tbSurveyItem.Id,
                                            SurveyItemName = m.tbSurveyItem.SurveyItemName,
                                            SurveyOptionId = m.tbSurveyOption.Id,
                                            SurveyOptionName = m.tbSurveyOption.OptionName,
                                            SurveyOptionValue = m.tbSurveyOption.OptionValue,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            GradeNo = p.tbClass.tbGrade.No,
                                            TeacherId = m.tbTeacher.Id,
                                            TeacherName = m.tbTeacher.TeacherName,
                                        };

                if (vm.SurveyGradeId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.GradeId == vm.SurveyGradeId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => vm.SearchText.Contains(d.GradeName) || vm.SearchText.Contains(d.SubjectName));
                }

                vm.SurveyGradeTeacherRankingList = (from p in tbSurveryDataList
                                                    group p by new
                                                    {
                                                        gradeId = p.GradeId,
                                                        gradeName = p.GradeName,
                                                        gradeNo = p.GradeNo,
                                                        subjectId = p.SubjectId,
                                                        subjectName = p.SubjectName,
                                                        subjectNo = p.SubjectNo,
                                                        classId = p.ClassId,
                                                        teacherId = p.TeacherId,
                                                        teacherName = p.TeacherName,
                                                        itemId = p.SurveyItemId
                                                    } into g
                                                    select new Dto.SurveyReport.GradeTeacherRanking
                                                    {
                                                        GradeId = g.Key.gradeId,
                                                        GradeName = g.Key.gradeName,
                                                        GradeNo = g.Key.gradeNo,
                                                        ClassId = g.Key.classId,
                                                        SubjectId = g.Key.subjectId,
                                                        SubjectName = g.Key.subjectName,
                                                        SubjectNo = g.Key.subjectNo,
                                                        TeacherId = g.Key.teacherId,
                                                        TeacherName = g.Key.teacherName,
                                                        SurveyOptionCount = g.Count(),
                                                        SurveyOptionSum = g.Select(d => d.SurveyOptionValue).Sum(),
                                                        SurveyOptionAvg = g.Select(d => d.SurveyOptionValue).Average(),
                                                        SurveyItemId = g.Key.itemId
                                                    }).Distinct().ToList();
                var allList = (from p in vm.SurveyGradeTeacherRankingList
                               group p by new { p.GradeId, p.GradeName, p.GradeNo, p.SubjectId, p.SubjectName, p.SubjectNo, p.TeacherId, p.TeacherName, p.SurveyItemId } into g
                               select new Dto.SurveyReport.GradeTeacherRanking
                               {
                                   GradeId = g.Key.GradeId,
                                   GradeName = g.Key.GradeName,
                                   GradeNo = g.Key.GradeNo,
                                   SubjectId = g.Key.SubjectId,
                                   SubjectName = g.Key.SubjectName,
                                   SubjectNo = g.Key.SubjectNo,
                                   TeacherId = g.Key.TeacherId,
                                   TeacherName = g.Key.TeacherName,
                                   SurveyOptionCount = g.Select(d => d.SurveyOptionCount).Sum(),
                                   SurveyOptionSum = g.Select(d => d.SurveyOptionSum).Sum(),
                                   SurveyOptionAvg = g.Select(d => d.SurveyOptionAvg).Average(),
                                   SurveyItemId = g.Key.SurveyItemId
                               }).ToList();

                foreach (var org in vm.SurveyGradeTeacherRankingList.GroupBy(d => new { d.GradeId, d.GradeName }).Select(g => new { GradeId = g.Key.GradeId, GradeName = g.Key.GradeName }))
                {
                    var rank = 1;
                    foreach (var classItem in vm.SurveyGradeTeacherRankingList.Where(d => d.GradeId == org.GradeId)
                        .GroupBy(d => new { d.GradeId, d.SubjectId, d.SubjectName, d.TeacherId, d.TeacherName })
                        .Select(g => new { g.Key.GradeId, g.Key.SubjectName, g.Key.TeacherName, g.Key.SubjectId, g.Key.TeacherId, }))
                    {
                        var gradeTeacher = new Dto.SurveyReport.GradeTeacherRanking();
                        var studentCount = 0;
                        var studentSumAvg = 0m;
                        gradeTeacher.GradeId = org.GradeId;
                        gradeTeacher.TeacherNameStr = classItem.TeacherName;
                        gradeTeacher.SubjectNameStr = classItem.SubjectName;

                        var teacherCount = allList
                            .Where(d => d.GradeId == org.GradeId && d.TeacherId == classItem.TeacherId)
                            .GroupBy(d => new { d.GradeId, d.SubjectId, d.TeacherId })
                            .Select(g => new { g.Key.GradeId, g.Key.SubjectId, studentCount = g.Select(d=>d.SurveyOptionCount).Sum(), studentSum = g.Select(d => d.SurveyOptionAvg).Sum() }).FirstOrDefault();
                        if (teacherCount != null)
                        {
                            studentCount = teacherCount.studentCount;
                            studentSumAvg = teacherCount.studentSum;

                            gradeTeacher.StudentCountStr = studentCount / 10;
                            gradeTeacher.TeacherSumStr = studentSumAvg;
                        }
                        var classAvgSumTemp = vm.SurveyGradeTeacherRankingList.Where(d => d.GradeId == org.GradeId)
                                          .GroupBy(d => new { d.GradeId, d.ClassId, d.SubjectId, d.SurveyItemId })
                                          .Select(g => new { g.Key.GradeId, g.Key.ClassId, g.Key.SubjectId, g.Key.SurveyItemId, classAvg = g.Select(d => d.SurveyOptionSum).Sum() / g.Select(d => d.SurveyOptionCount).Sum() }).ToList();
                        var classAvgSum = classAvgSumTemp
                                            .GroupBy(d => new { d.GradeId, d.ClassId })
                                          .Select(g => new { g.Key.GradeId, g.Key.ClassId, classAvg = g.Select(d => d.classAvg).Sum()/ g.Select(d=>d.SubjectId).Distinct().Count() }).ToList();

                        var myClassIds = vm.SurveyGradeTeacherRankingList.Where(d => d.GradeId == org.GradeId && d.TeacherId == classItem.TeacherId)
                                        .GroupBy(d => new { d.GradeId, d.ClassId }).Select(g => g.Key.ClassId).ToList();

                        var MyClassAvg = classAvgSum.Where(d => myClassIds.Contains(d.ClassId)).Select(d => d.classAvg).Average();

                        gradeTeacher.ClassAvgStr = MyClassAvg;
                        if (MyClassAvg > decimal.Zero)
                        {
                            gradeTeacher.AverageRate = Decimal.Round((studentSumAvg - MyClassAvg) / MyClassAvg * (decimal)100.0, 2);
                        }
                        gradeTeacher.OldRanking = rank;
                        rank++;
                        vm.SurveyGradeTeacherNoRankingList.Add(gradeTeacher);
                    }
                }

                var teacherKing = 1;
                foreach (var a in vm.SurveyGradeTeacherNoRankingList.OrderBy(d => d.GradeId).OrderByDescending(d => d.TeacherSumStr))
                {
                    a.TeacherRanking = teacherKing;
                    teacherKing++;
                }

                teacherKing = 1;
                foreach (var a in vm.SurveyGradeTeacherNoRankingList.OrderBy(d => d.GradeId).OrderByDescending(d => d.AverageRate))
                {
                    a.AverageRateRanking = teacherKing;
                    teacherKing++;
                }
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GradeTeacherRanking(Models.SurveyReport.GradeTeacherRanking vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("GradeTeacherRanking", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveyGroupId = vm.SurveyGroupId,
                SurveyGradeId = vm.SurveyGradeId
            }));
        }

        public ActionResult GradeTeacherRankingExport()
        {
            var vm = new Models.SurveyReport.GradeTeacherRanking();
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 准备数据
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                var surveyGroupIds = new List<int>();
                if (vm.SurveyGroupId == 0)
                {
                    surveyGroupIds = vm.SurveyGroupList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGroupIds.Add((int)vm.SurveyGroupId);
                }

                vm.SurveyGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      orderby p.tbClass.tbGrade.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbClass.tbGrade.GradeName,
                                          Value = p.tbClass.tbGrade.Id.ToString()
                                      }).Distinct().ToList();

                var tbSurveryDataList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                        join m in db.Table<Entity.tbSurveyData>() on p.tbStudent.Id equals m.tbStudent.Id
                                        join c in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals c.tbClass.Id
                                        where m.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && m.tbSurveyItem.tbSurveyGroup.IsOrg
                                        && c.tbSurvey.Id == vm.SurveyId
                                        && m.tbClass == null
                                        && m.tbTeacher.IsDeleted == false
                                        && m.tbStudent.IsDeleted == false
                                        && surveyGroupIds.Contains(m.tbSurveyItem.tbSurveyGroup.Id)
                                        && m.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        && m.tbSurveyItem.SurveyItemName.Contains("总体感受") == false
                                        select new
                                        {
                                            OrgId = m.tbOrg.Id,
                                            OrgName = m.tbOrg.OrgName,
                                            OrgNo = m.tbOrg.No,
                                            CourseId = m.tbOrg.tbCourse.Id,
                                            CourseName = m.tbOrg.tbCourse.CourseName,
                                            CourseNo = m.tbOrg.tbCourse.No,
                                            SubjectId = m.tbOrg.tbCourse.tbSubject.Id,
                                            SubjectName = m.tbOrg.tbCourse.tbSubject.SubjectName,
                                            SubjectNo = m.tbOrg.tbCourse.tbSubject.No,
                                            StudentId = m.tbStudent.Id,
                                            StudentName = m.tbStudent.StudentName,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            ClassNo = p.tbClass.No,
                                            SurveyItemId = m.tbSurveyItem.Id,
                                            SurveyItemName = m.tbSurveyItem.SurveyItemName,
                                            SurveyOptionId = m.tbSurveyOption.Id,
                                            SurveyOptionName = m.tbSurveyOption.OptionName,
                                            SurveyOptionValue = m.tbSurveyOption.OptionValue,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            GradeNo = p.tbClass.tbGrade.No,
                                            TeacherId = m.tbTeacher.Id,
                                            TeacherName = m.tbTeacher.TeacherName,
                                        };

                if (vm.SurveyGradeId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.GradeId == vm.SurveyGradeId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => vm.SearchText.Contains(d.GradeName) || vm.SearchText.Contains(d.SubjectName));
                }

                vm.SurveyGradeTeacherRankingList = (from p in tbSurveryDataList
                                                    group p by new
                                                    {
                                                        gradeId = p.GradeId,
                                                        gradeName = p.GradeName,
                                                        gradeNo = p.GradeNo,
                                                        classId = p.ClassId,
                                                        className = p.ClassName,
                                                        classNo = p.ClassNo,
                                                        subjectId = p.SubjectId,
                                                        subjectName = p.SubjectName,
                                                        studentId = p.StudentId,
                                                        subjectNo = p.SubjectNo,
                                                        teacherId = p.TeacherId,
                                                        teacherName = p.TeacherName
                                                    } into g
                                                    select new Dto.SurveyReport.GradeTeacherRanking
                                                    {
                                                        GradeId = g.Key.gradeId,
                                                        GradeName = g.Key.gradeName,
                                                        GradeNo = g.Key.gradeNo,
                                                        ClassId = g.Key.classId,
                                                        ClassName = g.Key.className,
                                                        ClassNo = g.Key.classNo,
                                                        StudentId = g.Key.studentId,
                                                        SubjectId = g.Key.subjectId,
                                                        SubjectName = g.Key.subjectName,
                                                        SubjectNo = g.Key.subjectNo,
                                                        TeacherId = g.Key.teacherId,
                                                        TeacherName = g.Key.teacherName,
                                                        SurveyOptionCount = g.Count(),
                                                        SurveyOptionSum = g.Select(d => d.SurveyOptionValue).Sum(),
                                                        SurveyOptionAvg = g.Select(d => d.SurveyOptionValue).Average()
                                                    }).Distinct().ToList();

                foreach (var org in vm.SurveyGradeTeacherRankingList.GroupBy(d => new { d.GradeId, d.GradeName }).Select(g => new { GradeId = g.Key.GradeId, GradeName = g.Key.GradeName }))
                {
                    var rank = 1;
                    foreach (var classItem in vm.SurveyGradeTeacherRankingList.Where(d => d.GradeId == org.GradeId).GroupBy(d => new { d.GradeId, d.SubjectId, d.SubjectName, d.TeacherId, d.TeacherName }).Select(g => new { g.Key.GradeId, g.Key.SubjectName, g.Key.TeacherName, g.Key.SubjectId, g.Key.TeacherId, }))
                    {
                        var gradeTeacher = new Dto.SurveyReport.GradeTeacherRanking();
                        var studentCount = 0;
                        var studentSumAvg = 0m;
                        gradeTeacher.GradeId = org.GradeId;
                        gradeTeacher.TeacherNameStr = classItem.TeacherName;
                        gradeTeacher.SubjectNameStr = classItem.SubjectName;

                        var teacherCount = vm.SurveyGradeTeacherRankingList
                            .Where(d => d.GradeId == org.GradeId && d.TeacherId == classItem.TeacherId)
                            .GroupBy(d => new { d.GradeId, d.SubjectId, d.TeacherId })
                            .Select(g => new { g.Key.GradeId, g.Key.SubjectId, studentCount = g.Count(), studentSum = g.Select(d => d.SurveyOptionSum).Average() }).FirstOrDefault();
                        if (teacherCount != null)
                        {
                            studentCount = teacherCount.studentCount;
                            studentSumAvg = teacherCount.studentSum;

                            gradeTeacher.StudentCountStr = studentCount;
                            gradeTeacher.TeacherSumStr = studentSumAvg;
                        }
                        var classAvgSum = vm.SurveyGradeTeacherRankingList.Where(d => d.GradeId == org.GradeId)
                                          .GroupBy(d => new { d.GradeId, d.ClassId })
                                          .Select(g => new { g.Key.GradeId, g.Key.ClassId, classAvg = g.Select(d => d.SurveyOptionSum).Average() }).ToList();

                        var myClassIds = vm.SurveyGradeTeacherRankingList.Where(d => d.GradeId == org.GradeId && d.TeacherId == classItem.TeacherId)
                                        .GroupBy(d => new { d.GradeId, d.ClassId }).Select(g => g.Key.ClassId).ToList();

                        var MyClassAvg = classAvgSum.Where(d => myClassIds.Contains(d.ClassId)).Select(d => d.classAvg).FirstOrDefault();

                        gradeTeacher.ClassAvgStr = MyClassAvg;
                        if (MyClassAvg > decimal.Zero)
                        {
                            gradeTeacher.AverageRate = Decimal.Round((studentSumAvg - MyClassAvg) / MyClassAvg * (decimal)100.0, 2);
                        }
                        gradeTeacher.OldRanking = rank;
                        rank++;
                        vm.SurveyGradeTeacherNoRankingList.Add(gradeTeacher);
                    }
                }

                var teacherKing = 1;
                foreach (var a in vm.SurveyGradeTeacherNoRankingList.OrderBy(d => d.GradeId).OrderByDescending(d => d.TeacherSumStr))
                {
                    a.TeacherRanking = teacherKing;
                    teacherKing++;
                }

                teacherKing = 1;
                foreach (var a in vm.SurveyGradeTeacherNoRankingList.OrderBy(d => d.GradeId).OrderByDescending(d => d.AverageRate))
                {
                    a.AverageRateRanking = teacherKing;
                    teacherKing++;
                }
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();
                #region 分班统计
                foreach (var org in vm.SurveyGradeTeacherRankingList.GroupBy(d => new { d.GradeId, d.GradeName }).Select(g => new { GradeId = g.Key.GradeId, GradeName = g.Key.GradeName }))
                {
                    var allColumnLength = 8;
                    var arrColumns = new string[allColumnLength];

                    for (int i = 0; i < arrColumns.Length; i++)
                    {
                        arrColumns[i] = (i + 1).ToString();
                    }

                    var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                    sheet.isColumnWritten = false;
                    sheet.isWriteHeader = true;
                    sheet.strHeaderText = org.GradeName;
                    //开始表格
                    var dt = Code.Common.ArrayToDataTable(arrColumns);
                    var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                    #region 增加标题
                    var arrSurveySubjectName = new string[allColumnLength];
                    dt.Rows.Add(arrSurveySubjectName);
                    #endregion
                    #region DataList
                    foreach (var classItem in vm.SurveyGradeTeacherNoRankingList.Where(d => d.GradeId == org.GradeId))
                    {
                        string[] arrSurveyData = new string[allColumnLength];
                        arrSurveyData[0] = classItem.TeacherNameStr;
                        arrSurveyData[1] = classItem.SubjectNameStr;
                        arrSurveyData[2] = classItem.StudentCountStr.ToString();
                        arrSurveyData[3] = Decimal.Round(classItem.TeacherSumStr, 2).ToString();
                        arrSurveyData[4] = Decimal.Round(classItem.ClassAvgStr, 2).ToString();
                        arrSurveyData[5] = classItem.TeacherRanking.ToString();
                        arrSurveyData[6] = Decimal.Round(classItem.AverageRate, 2) + "%";
                        arrSurveyData[7] = classItem.AverageRateRanking.ToString();
                        dt.Rows.Add(arrSurveyData);
                    }
                    #endregion                    
                    dt.Rows[0][0] = "教师";
                    dt.Rows[0][1] = "科目";
                    dt.Rows[0][2] = "参评人数";
                    dt.Rows[0][3] = "教师得分";
                    dt.Rows[0][4] = "所教班级各科均分(所有学科)";
                    dt.Rows[0][5] = "教师满意率排名";
                    dt.Rows[0][6] = "离均率";
                    dt.Rows[0][7] = "离均率排名";
                    sheet.data = dt;
                    sheet.regions = regions;
                    sheetList.Add(sheet);
                }
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

        public ActionResult SubjectList()
        {
            var vm = new Models.SurveyReport.SubjectList();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveySubjectList = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                        && p.tbSurveyGroup.IsDeleted == false
                                        && p.tbSurveyGroup.IsOrg
                                        && p.tbCourse.IsDeleted == false
                                        && p.tbCourse.tbSubject.IsDeleted == false
                                        orderby p.tbCourse.tbSubject.No
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbCourse.tbSubject.SubjectName,
                                            Value = p.tbCourse.tbSubject.Id.ToString()
                                        }).Distinct().ToList();

                if (vm.SurveySubjectId == 0 && vm.SurveySubjectList.Count > 0)
                {
                    vm.SurveySubjectId = vm.SurveySubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbCourse.tbSubject.IsDeleted == false
                                       && p.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                var surveyCourseIds = new List<int>();
                if (vm.SurveyCourseId == 0)
                {
                    surveyCourseIds = vm.SurveyCourseList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyCourseIds.Add((int)vm.SurveyCourseId);
                }

                vm.SurveyTeacherList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                        join m in db.Table<Entity.tbSurveyCourse>()
                                        on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                        equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                        where p.tbOrg.IsDeleted == false
                                        && surveyCourseIds.Contains(m.tbCourse.Id)
                                        orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new Dto.SurveyReport.SubjectList
                                        {
                                            CourseNo = p.tbOrg.tbCourse.No,
                                            CourseId = p.tbOrg.tbCourse.Id,
                                            CourseName = p.tbOrg.tbCourse.CourseName,
                                            TeacherId = p.tbTeacher.Id,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).Distinct().ToList();

                var teacherIds = vm.SurveyTeacherList.Select(d => d.TeacherId).Distinct().ToList();
                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where surveyCourseIds.Contains(p.tbCourse.Id)
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        select p.tbSurveyGroup.Id).Distinct().ToList();
                #region 单选分数
                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && teacherIds.Contains(p.tbTeacher.Id)
                                        && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        select p;



                if (vm.SurveyCourseId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => tbSurveyGroupIds.Contains(d.tbSurveyItem.tbSurveyGroup.Id));
                }

                if (vm.SurveyTeacherId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.tbTeacher.Id == vm.SurveyTeacherId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectReportList = (from p in tbSurveryDataList
                                              group p by new
                                              {
                                                  orgId = p.tbOrg.Id,
                                                  orgName = p.tbOrg.OrgName,
                                                  courseId = p.tbOrg.tbCourse.Id,
                                                  courseName = p.tbOrg.tbCourse.CourseName,
                                                  teacherId = p.tbTeacher.Id,
                                                  teacherName = p.tbTeacher.TeacherName,
                                                  surveryItemId = p.tbSurveyItem.Id,
                                                  surveryItemName = p.tbSurveyItem.SurveyItemName,
                                                  surverOptionId = p.tbSurveyOption.Id,
                                                  surverOptionName = p.tbSurveyOption.OptionName
                                              } into g
                                              select new Dto.SurveyReport.SubjectList
                                              {
                                                  OrgId = g.Key.orgId,
                                                  OrgName = g.Key.orgName,
                                                  CourseId = g.Key.courseId,
                                                  CourseName = g.Key.courseName,
                                                  SurveyItemId = g.Key.surveryItemId,
                                                  SurveyItemName = g.Key.surveryItemName,
                                                  SurveyOptionId = g.Key.surverOptionId,
                                                  SurveyOptionName = g.Key.surverOptionName,
                                                  TeacherId = g.Key.teacherId,
                                                  TeacherName = g.Key.teacherName,
                                                  SurveyOptionCount = g.Count(),
                                                  SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                                  SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                              }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataTextList = from p in db.Table<Entity.tbSurveyData>()
                                            where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                            && p.tbOrg != null
                                            && p.tbClass == null
                                            && teacherIds.Contains(p.tbTeacher.Id)
                                            && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                            select p;

                if (vm.SurveyCourseId > decimal.Zero)
                {
                    tbSurveryDataTextList = tbSurveryDataTextList.Where(d => tbSurveyGroupIds.Contains(d.tbSurveyItem.tbSurveyGroup.Id));
                }

                if (vm.SurveyTeacherId > decimal.Zero)
                {
                    tbSurveryDataTextList = tbSurveryDataTextList.Where(d => d.tbTeacher.Id == vm.SurveyTeacherId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataTextList = tbSurveryDataTextList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectReportTextList = (from p in tbSurveryDataTextList
                                                  group p by new
                                                  {
                                                      orgId = p.tbOrg.Id,
                                                      orgName = p.tbOrg.OrgName,
                                                      courseId = p.tbOrg.tbCourse.Id,
                                                      courseName = p.tbOrg.tbCourse.CourseName,
                                                      teacherId = p.tbTeacher.Id,
                                                      teacherName = p.tbTeacher.TeacherName,
                                                      surveryItemId = p.tbSurveyItem.Id,
                                                      surveryItemName = p.tbSurveyItem.SurveyItemName
                                                  } into g
                                                  select new Dto.SurveyReport.SubjectList
                                                  {
                                                      OrgId = g.Key.orgId,
                                                      OrgName = g.Key.orgName,
                                                      CourseId = g.Key.courseId,
                                                      CourseName = g.Key.courseName,
                                                      SurveyItemId = g.Key.surveryItemId,
                                                      SurveyItemName = g.Key.surveryItemName,
                                                      TeacherId = g.Key.teacherId,
                                                      TeacherName = g.Key.teacherName,
                                                      SurveyOptionCount = g.Count()
                                                  }).ToList();
                #endregion

                vm.SurveyItemInfoList = (from p in db.Table<Entity.tbSurveyItem>()
                                         where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                         && tbSurveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                         orderby p.tbSurveyGroup.No, p.No
                                         select new Dto.SurveyItem.Info
                                         {
                                             Id = p.Id,
                                             SurveyItemName = p.SurveyItemName,
                                             SurveyItemType = p.SurveyItemType,
                                             No = p.No
                                         }).Distinct().ToList();

                vm.SurveyOptionInfoList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                           && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                           orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               OptionValue = p.OptionValue,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).Distinct().ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubjectList(Models.SurveyReport.SubjectList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SubjectList", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveySubjectId = vm.SurveySubjectId,
                SurveyCourseId = vm.SurveyCourseId
            }));
        }

        public ActionResult SubjectTextList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyReport.SubjectTextList();

                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbCourse.Id == vm.SurveyCourseId
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        select p.tbSurveyGroup.Id).ToList();


                if (vm.OpenFlag == decimal.Zero)
                {
                    var tbOrgIds = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                    join m in db.Table<Entity.tbSurvey>() on p.tbOrg.tbYear.Id equals m.tbYear.Id
                                    where p.tbTeacher.Id == vm.SurveyTeacherId
                                    && p.tbOrg.tbCourse.Id == vm.SurveyCourseId
                                    && m.Id == vm.SurveyId
                                    select p.tbOrg.Id).ToList();


                    vm.SurveySubjectTextList = (from p in db.Table<Entity.tbSurveyData>()
                                                where p.tbTeacher.Id == vm.SurveyTeacherId
                                                && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                && p.tbSurveyItem.Id == vm.SurveyItemId
                                                && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                                && tbOrgIds.Contains(p.tbOrg.Id)
                                                && p.tbClass == null
                                                && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                                orderby p.tbStudent.StudentCode, p.tbStudent.StudentName
                                                select new Dto.SurveyReport.SubjectTextList
                                                {
                                                    StudentCode = p.tbStudent.StudentCode,
                                                    StudentName = p.tbStudent.StudentName,
                                                    SurveyText = p.SurveyText
                                                }).ToPageList(vm.Page);
                }
                else
                {
                    var tbOrgIds = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                    join m in db.Table<Entity.tbSurvey>() on p.tbOrg.tbYear.Id equals m.tbYear.Id
                                    where p.tbOrg.tbCourse.Id == vm.SurveyCourseId
                                    && m.Id == vm.SurveyId
                                    select p.tbOrg.Id).ToList();


                    vm.SurveySubjectTextList = (from p in db.Table<Entity.tbSurveyData>()
                                                where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                && p.tbSurveyItem.Id == vm.SurveyItemId
                                                && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                                && tbOrgIds.Contains(p.tbOrg.Id)
                                                && p.tbClass == null
                                                && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                                orderby p.tbStudent.StudentCode, p.tbStudent.StudentName
                                                select new Dto.SurveyReport.SubjectTextList
                                                {
                                                    StudentCode = p.tbStudent.StudentCode,
                                                    StudentName = p.tbStudent.StudentName,
                                                    SurveyText = p.SurveyText
                                                }).ToPageList(vm.Page);
                }


                if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator)
                {
                    foreach (var a in vm.SurveySubjectTextList)
                    {
                        a.StudentCode = "***";
                        a.StudentName = "***";
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubjectTextList(Models.SurveyReport.SubjectTextList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SubjectTextList", new
            {
                SurveyId = vm.SurveyId,
                SurveyCourseId = vm.SurveyCourseId,
                SurveyTeacherId = vm.SurveyTeacherId,
                SurveyItemId = vm.SurveyItemId,
                OpenFlag = vm.OpenFlag,
                pageSize = vm.Page.PageSize,
                pageCount = vm.Page.PageCount,
                pageIndex = vm.Page.PageIndex
            }));
        }

        public ActionResult SubjectListExport()
        {
            var vm = new Models.SurveyReport.SubjectList();
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 准备数据
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveySubjectList = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                        && p.tbSurveyGroup.IsDeleted == false
                                        && p.tbSurveyGroup.IsOrg
                                        && p.tbCourse.IsDeleted == false
                                        && p.tbCourse.tbSubject.IsDeleted == false
                                        orderby p.tbCourse.tbSubject.No
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbCourse.tbSubject.SubjectName,
                                            Value = p.tbCourse.tbSubject.Id.ToString()
                                        }).Distinct().ToList();

                if (vm.SurveySubjectId == 0 && vm.SurveySubjectList.Count > 0)
                {
                    vm.SurveySubjectId = vm.SurveySubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbCourse.tbSubject.IsDeleted == false
                                       && p.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                var surveyCourseIds = new List<int>();
                if (vm.SurveyCourseId == 0)
                {
                    surveyCourseIds = vm.SurveyCourseList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyCourseIds.Add((int)vm.SurveyCourseId);
                }

                vm.SurveyTeacherList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                        join m in db.Table<Entity.tbSurveyCourse>()
                                        on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                        equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                        where p.tbOrg.IsDeleted == false
                                        && surveyCourseIds.Contains(m.tbCourse.Id)
                                        orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new Dto.SurveyReport.SubjectList
                                        {
                                            CourseNo = p.tbOrg.tbCourse.No,
                                            CourseId = p.tbOrg.tbCourse.Id,
                                            CourseName = p.tbOrg.tbCourse.CourseName,
                                            TeacherId = p.tbTeacher.Id,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).Distinct().ToList();

                var teacherIds = vm.SurveyTeacherList.Select(d => d.TeacherId).Distinct().ToList();
                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where surveyCourseIds.Contains(p.tbCourse.Id)
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        select p.tbSurveyGroup.Id).Distinct().ToList();
                #region 单选分数
                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && teacherIds.Contains(p.tbTeacher.Id)
                                        && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        select p;



                if (vm.SurveyCourseId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => tbSurveyGroupIds.Contains(d.tbSurveyItem.tbSurveyGroup.Id));
                }

                if (vm.SurveyTeacherId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.tbTeacher.Id == vm.SurveyTeacherId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectReportList = (from p in tbSurveryDataList
                                              group p by new
                                              {
                                                  orgId = p.tbOrg.Id,
                                                  orgName = p.tbOrg.OrgName,
                                                  courseId = p.tbOrg.tbCourse.Id,
                                                  courseName = p.tbOrg.tbCourse.CourseName,
                                                  teacherId = p.tbTeacher.Id,
                                                  teacherName = p.tbTeacher.TeacherName,
                                                  surveryItemId = p.tbSurveyItem.Id,
                                                  surveryItemName = p.tbSurveyItem.SurveyItemName,
                                                  surverOptionId = p.tbSurveyOption.Id,
                                                  surverOptionName = p.tbSurveyOption.OptionName
                                              } into g
                                              select new Dto.SurveyReport.SubjectList
                                              {
                                                  OrgId = g.Key.orgId,
                                                  OrgName = g.Key.orgName,
                                                  CourseId = g.Key.courseId,
                                                  CourseName = g.Key.courseName,
                                                  SurveyItemId = g.Key.surveryItemId,
                                                  SurveyItemName = g.Key.surveryItemName,
                                                  SurveyOptionId = g.Key.surverOptionId,
                                                  SurveyOptionName = g.Key.surverOptionName,
                                                  TeacherId = g.Key.teacherId,
                                                  TeacherName = g.Key.teacherName,
                                                  SurveyOptionCount = g.Count(),
                                                  SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                                  SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                              }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataTextList = from p in db.Table<Entity.tbSurveyData>()
                                            where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                            && p.tbOrg != null
                                            && p.tbClass == null
                                            && teacherIds.Contains(p.tbTeacher.Id)
                                            && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                            select p;

                if (vm.SurveyCourseId > decimal.Zero)
                {
                    tbSurveryDataTextList = tbSurveryDataTextList.Where(d => tbSurveyGroupIds.Contains(d.tbSurveyItem.tbSurveyGroup.Id));
                }

                if (vm.SurveyTeacherId > decimal.Zero)
                {
                    tbSurveryDataTextList = tbSurveryDataTextList.Where(d => d.tbTeacher.Id == vm.SurveyTeacherId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataTextList = tbSurveryDataTextList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectReportTextList = (from p in tbSurveryDataTextList
                                                  group p by new
                                                  {
                                                      orgId = p.tbOrg.Id,
                                                      orgName = p.tbOrg.OrgName,
                                                      courseId = p.tbOrg.tbCourse.Id,
                                                      courseName = p.tbOrg.tbCourse.CourseName,
                                                      teacherId = p.tbTeacher.Id,
                                                      teacherName = p.tbTeacher.TeacherName,
                                                      surveryItemId = p.tbSurveyItem.Id,
                                                      surveryItemName = p.tbSurveyItem.SurveyItemName
                                                  } into g
                                                  select new Dto.SurveyReport.SubjectList
                                                  {
                                                      OrgId = g.Key.orgId,
                                                      OrgName = g.Key.orgName,
                                                      CourseId = g.Key.courseId,
                                                      CourseName = g.Key.courseName,
                                                      SurveyItemId = g.Key.surveryItemId,
                                                      SurveyItemName = g.Key.surveryItemName,
                                                      TeacherId = g.Key.teacherId,
                                                      TeacherName = g.Key.teacherName,
                                                      SurveyOptionCount = g.Count()
                                                  }).ToList();
                #endregion

                vm.SurveyItemInfoList = (from p in db.Table<Entity.tbSurveyItem>()
                                         where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                         && tbSurveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                         orderby p.tbSurveyGroup.No, p.No
                                         select new Dto.SurveyItem.Info
                                         {
                                             Id = p.Id,
                                             SurveyItemName = p.SurveyItemName,
                                             SurveyItemType = p.SurveyItemType,
                                             No = p.No
                                         }).Distinct().ToList();

                vm.SurveyOptionInfoList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                           && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                           orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               OptionValue = p.OptionValue,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).Distinct().ToList();
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();
                var surveyOptionCount = 0;
                var itemList = vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                foreach (var item in itemList)
                {
                    var optionList = vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList();
                    foreach (var option in optionList)
                    {
                        surveyOptionCount++;
                    }
                }
                var itemTextList = vm.SurveyItemInfoList.Where(d => d.SurveyItemType == XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                foreach (var item in itemTextList)
                {
                    surveyOptionCount++;
                }
                var allColumnLength = surveyOptionCount + 4;
                var arrColumns = new string[allColumnLength];

                for (int i = 0; i < arrColumns.Length; i++)
                {
                    arrColumns[i] = (i + 1).ToString();
                }

                var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                sheet.isColumnWritten = false;
                sheet.isWriteHeader = true;
                sheet.strHeaderText = "科目教师统计";
                //开始表格
                var dt = Code.Common.ArrayToDataTable(arrColumns);
                var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                var arrSurveyItemID = new string[allColumnLength];
                var arrSurveyItemName = new string[allColumnLength];
                var arrSurveyOptionID = new string[allColumnLength];
                var arrSurveyOptionName = new string[allColumnLength];
                var index = 1;
                for (int i = 0; i < itemList.Count(); i++)
                {
                    var optionList = vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == itemList[i].Id).Distinct().ToList();
                    for (var j = 0; j < optionList.Count(); j++)
                    {
                        index++;
                        arrSurveyItemID[index] = itemList[i].Id.ToString();
                        arrSurveyItemName[index] = itemList[i].SurveyItemName.ToString();
                        arrSurveyOptionID[index] = optionList[j].Id.ToString();
                        arrSurveyOptionName[index] = optionList[j].OptionName.ToString();
                    }
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, index - optionList.Count() + 1, index));
                }
                arrSurveyItemName[index + 1] = "评价次数";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index + 1, index + 1));
                arrSurveyItemName[index + 2] = "满意度比例";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index + 2, index + 2));
                index = index + 2;
                for (int i = 0; i < itemTextList.Count(); i++)
                {
                    index++;
                    arrSurveyItemID[index] = itemTextList[i].Id.ToString();
                    arrSurveyItemName[index] = itemTextList[i].SurveyItemName.ToString();
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index, index));
                }
                dt.Rows.Add(arrSurveyItemName);
                dt.Rows.Add(arrSurveyOptionName);
                #region 处理DataList
                foreach (var teacher in vm.SurveyTeacherList.OrderBy(d => d.CourseNo).ThenBy(d => d.CourseName).ThenBy(d => d.TeacherName).ToList())
                {
                    string[] arrSurveyData = new string[allColumnLength];
                    arrSurveyData[0] = teacher.CourseName;
                    arrSurveyData[1] = teacher.TeacherName;
                    var allCount = 0;
                    var goodCount = 0m;
                    var goodRate = 0m;
                    var indexR = 1;
                    foreach (var item in vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                    {
                        foreach (var option in vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList())
                        {
                            indexR++;
                            var optionCount = vm.SurveySubjectReportList
                                .Where(d => d.TeacherId == teacher.TeacherId && d.CourseId == teacher.CourseId && d.SurveyItemId == item.Id && d.SurveyOptionId == option.Id)
                                .Sum(d => d.SurveyOptionCount);
                            if (option.OptionName.Contains("A：非常满意") || option.OptionName.Contains("B：满意"))
                            {
                                goodCount += optionCount;
                            }
                            allCount += optionCount;
                            if (optionCount > decimal.Zero)
                            {
                                arrSurveyData[indexR] = optionCount.ToString();
                            }
                        }
                    }
                    indexR++;
                    if (allCount > decimal.Zero)
                    {
                        arrSurveyData[indexR] = allCount.ToString();
                    }
                    if (allCount > decimal.Zero)
                    {
                        goodRate = Decimal.Round(goodCount / allCount * 100, 2);
                    }
                    indexR++;
                    if (goodRate > decimal.Zero)
                    {
                        arrSurveyData[indexR] = goodRate.ToString() + "%";
                    }
                    foreach (var item in vm.SurveyItemInfoList.Where(d => d.SurveyItemType == XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                    {
                        indexR++;
                        var optionTextCount = vm.SurveySubjectReportTextList
                            .Where(d => d.TeacherId == teacher.TeacherId && d.CourseId == teacher.CourseId && d.SurveyItemId == item.Id)
                            .Sum(d => d.SurveyOptionCount);
                        if (optionTextCount > 0)
                        {
                            arrSurveyData[indexR] = optionTextCount.ToString();
                        }
                    }
                    dt.Rows.Add(arrSurveyData);
                }
                #endregion
                dt.Rows[0][0] = "课程";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));
                dt.Rows[0][1] = "教师姓名";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 1, 1));
                sheet.data = dt;
                sheet.regions = regions;
                sheetList.Add(sheet);
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

        public ActionResult SubjectTotalList()
        {
            var vm = new Models.SurveyReport.SubjectTotalList();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveySubjectList = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                        && p.tbSurveyGroup.IsDeleted == false
                                        && p.tbSurveyGroup.IsOrg
                                        && p.tbCourse.IsDeleted == false
                                        && p.tbCourse.tbSubject.IsDeleted == false
                                        orderby p.tbCourse.tbSubject.No
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbCourse.tbSubject.SubjectName,
                                            Value = p.tbCourse.tbSubject.Id.ToString()
                                        }).Distinct().ToList();

                if (vm.SurveySubjectId == 0 && vm.SurveySubjectList.Count > 0)
                {
                    vm.SurveySubjectId = vm.SurveySubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbCourse.tbSubject.IsDeleted == false
                                       && p.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                var surveyCourseIds = new List<int>();
                surveyCourseIds = vm.SurveyCourseList.Select(d => d.Value.ConvertToInt()).ToList();

                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where surveyCourseIds.Contains(p.tbCourse.Id)
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        select p.tbSurveyGroup.Id).Distinct().ToList();

                #region 单选分数
                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectTotalList = (from p in tbSurveryDataList
                                             group p by new
                                             {
                                                 courseId = p.tbOrg.tbCourse.Id,
                                                 courseName = p.tbOrg.tbCourse.CourseName,
                                                 surveryItemId = p.tbSurveyItem.Id,
                                                 surveryItemName = p.tbSurveyItem.SurveyItemName,
                                                 surverOptionId = p.tbSurveyOption.Id,
                                                 surverOptionName = p.tbSurveyOption.OptionName
                                             } into g
                                             select new Dto.SurveyReport.SubjectTotalList
                                             {
                                                 CourseId = g.Key.courseId,
                                                 CourseName = g.Key.courseName,
                                                 SurveyItemId = g.Key.surveryItemId,
                                                 SurveyItemName = g.Key.surveryItemName,
                                                 SurveyOptionId = g.Key.surverOptionId,
                                                 SurveyOptionName = g.Key.surverOptionName,
                                                 SurveyOptionCount = g.Count(),
                                                 SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                                 SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                             }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataTextList = from p in db.Table<Entity.tbSurveyData>()
                                            where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                            && p.tbOrg != null
                                            && p.tbClass == null
                                            && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                            select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataTextList = tbSurveryDataTextList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectTotalTextList = (from p in tbSurveryDataTextList
                                                 group p by new
                                                 {
                                                     courseId = p.tbOrg.tbCourse.Id,
                                                     courseName = p.tbOrg.tbCourse.CourseName,
                                                     surveryItemId = p.tbSurveyItem.Id,
                                                     surveryItemName = p.tbSurveyItem.SurveyItemName
                                                 } into g
                                                 select new Dto.SurveyReport.SubjectTotalList
                                                 {
                                                     CourseId = g.Key.courseId,
                                                     CourseName = g.Key.courseName,
                                                     SurveyItemId = g.Key.surveryItemId,
                                                     SurveyItemName = g.Key.surveryItemName,
                                                     SurveyOptionCount = g.Count()
                                                 }).ToList();
                #endregion

                vm.SurveyItemInfoList = (from p in db.Table<Entity.tbSurveyItem>()
                                         where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                         && tbSurveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                         orderby p.tbSurveyGroup.No, p.No
                                         select new Dto.SurveyItem.Info
                                         {
                                             Id = p.Id,
                                             SurveyItemName = p.SurveyItemName,
                                             SurveyItemType = p.SurveyItemType,
                                             No = p.No
                                         }).Distinct().ToList();

                vm.SurveyOptionInfoList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                           && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                           orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               OptionValue = p.OptionValue,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).Distinct().ToList();

                vm.SurveySubjectTotalRankingList = (from p in vm.SurveyCourseList
                                                    select new Dto.SurveyReport.SubjectTotalList
                                                    {
                                                        CourseId = p.Value.ConvertToInt(),
                                                        CourseName = p.Text
                                                    }).ToList();

                foreach (var coures in vm.SurveySubjectTotalRankingList)
                {
                    var allCount = 0;
                    var goodCount = 0m;
                    var goodRate = 0m;
                    foreach (var item in vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                    {
                        foreach (var option in vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList())
                        {
                            var optionCount = vm.SurveySubjectTotalList
                                .Where(d => d.CourseId == coures.CourseId && d.SurveyItemId == item.Id && d.SurveyOptionId == option.Id)
                                .Sum(d => d.SurveyOptionCount);
                            if (option.OptionName.Contains("A：非常满意") || option.OptionName.Contains("B：满意"))
                            {
                                goodCount += optionCount;
                            }
                            allCount += optionCount;
                        }
                    }
                    if (allCount > decimal.Zero)
                    {
                        goodRate = Decimal.Round(goodCount / allCount * 100, 2);
                    }
                    coures.SurveyAllCount = allCount;
                    coures.CourseGoodRate = goodRate;
                }
                vm.SurveySubjectTotalRankingList = vm.SurveySubjectTotalRankingList.OrderByDescending(d => d.CourseGoodRate).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubjectTotalList(Models.SurveyReport.SubjectTotalList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SubjectTotalList", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveySubjectId = vm.SurveySubjectId
            }));
        }

        public ActionResult SubjectTotalListExport()
        {
            var vm = new Models.SurveyReport.SubjectTotalList();
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 准备数据
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveySubjectList = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                        && p.tbSurveyGroup.IsDeleted == false
                                        && p.tbSurveyGroup.IsOrg
                                        && p.tbCourse.IsDeleted == false
                                        && p.tbCourse.tbSubject.IsDeleted == false
                                        orderby p.tbCourse.tbSubject.No
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbCourse.tbSubject.SubjectName,
                                            Value = p.tbCourse.tbSubject.Id.ToString()
                                        }).Distinct().ToList();

                if (vm.SurveySubjectId == 0 && vm.SurveySubjectList.Count > 0)
                {
                    vm.SurveySubjectId = vm.SurveySubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbCourse.tbSubject.IsDeleted == false
                                       && p.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                var surveyCourseIds = new List<int>();
                surveyCourseIds = vm.SurveyCourseList.Select(d => d.Value.ConvertToInt()).ToList();

                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where surveyCourseIds.Contains(p.tbCourse.Id)
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        select p.tbSurveyGroup.Id).Distinct().ToList();

                #region 单选分数
                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectTotalList = (from p in tbSurveryDataList
                                             group p by new
                                             {
                                                 courseId = p.tbOrg.tbCourse.Id,
                                                 courseName = p.tbOrg.tbCourse.CourseName,
                                                 surveryItemId = p.tbSurveyItem.Id,
                                                 surveryItemName = p.tbSurveyItem.SurveyItemName,
                                                 surverOptionId = p.tbSurveyOption.Id,
                                                 surverOptionName = p.tbSurveyOption.OptionName
                                             } into g
                                             select new Dto.SurveyReport.SubjectTotalList
                                             {
                                                 CourseId = g.Key.courseId,
                                                 CourseName = g.Key.courseName,
                                                 SurveyItemId = g.Key.surveryItemId,
                                                 SurveyItemName = g.Key.surveryItemName,
                                                 SurveyOptionId = g.Key.surverOptionId,
                                                 SurveyOptionName = g.Key.surverOptionName,
                                                 SurveyOptionCount = g.Count(),
                                                 SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                                 SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                             }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataTextList = from p in db.Table<Entity.tbSurveyData>()
                                            where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                            && p.tbOrg != null
                                            && p.tbClass == null
                                            && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                            select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataTextList = tbSurveryDataTextList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectTotalTextList = (from p in tbSurveryDataTextList
                                                 group p by new
                                                 {
                                                     courseId = p.tbOrg.tbCourse.Id,
                                                     courseName = p.tbOrg.tbCourse.CourseName,
                                                     surveryItemId = p.tbSurveyItem.Id,
                                                     surveryItemName = p.tbSurveyItem.SurveyItemName
                                                 } into g
                                                 select new Dto.SurveyReport.SubjectTotalList
                                                 {
                                                     CourseId = g.Key.courseId,
                                                     CourseName = g.Key.courseName,
                                                     SurveyItemId = g.Key.surveryItemId,
                                                     SurveyItemName = g.Key.surveryItemName,
                                                     SurveyOptionCount = g.Count()
                                                 }).ToList();
                #endregion

                vm.SurveyItemInfoList = (from p in db.Table<Entity.tbSurveyItem>()
                                         where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                         && tbSurveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                         orderby p.tbSurveyGroup.No, p.No
                                         select new Dto.SurveyItem.Info
                                         {
                                             Id = p.Id,
                                             SurveyItemName = p.SurveyItemName,
                                             SurveyItemType = p.SurveyItemType,
                                             No = p.No
                                         }).Distinct().ToList();

                vm.SurveyOptionInfoList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                           && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                           orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               OptionValue = p.OptionValue,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).Distinct().ToList();

                vm.SurveySubjectTotalRankingList = (from p in vm.SurveyCourseList
                                                    select new Dto.SurveyReport.SubjectTotalList
                                                    {
                                                        CourseId = p.Value.ConvertToInt(),
                                                        CourseName = p.Text
                                                    }).ToList();

                foreach (var coures in vm.SurveySubjectTotalRankingList)
                {
                    var allCount = 0;
                    var goodCount = 0m;
                    var goodRate = 0m;
                    foreach (var item in vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                    {
                        foreach (var option in vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList())
                        {
                            var optionCount = vm.SurveySubjectTotalList
                                .Where(d => d.CourseId == coures.CourseId && d.SurveyItemId == item.Id && d.SurveyOptionId == option.Id)
                                .Sum(d => d.SurveyOptionCount);
                            if (option.OptionName.Contains("A：非常满意") || option.OptionName.Contains("B：满意"))
                            {
                                goodCount += optionCount;
                            }
                            allCount += optionCount;
                        }
                    }
                    if (allCount > decimal.Zero)
                    {
                        goodRate = Decimal.Round(goodCount / allCount * 100, 2);
                    }
                    coures.SurveyAllCount = allCount;
                    coures.CourseGoodRate = goodRate;
                }
                vm.SurveySubjectTotalRankingList = vm.SurveySubjectTotalRankingList.OrderByDescending(d => d.CourseGoodRate).ToList();
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();
                var surveyOptionCount = 0;
                var itemList = vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                foreach (var item in itemList)
                {
                    var optionList = vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList();
                    foreach (var option in optionList)
                    {
                        surveyOptionCount++;
                    }
                }
                var itemTextList = vm.SurveyItemInfoList.Where(d => d.SurveyItemType == XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                foreach (var item in itemTextList)
                {
                    surveyOptionCount++;
                }
                var allColumnLength = surveyOptionCount + 3;
                var arrColumns = new string[allColumnLength];

                for (int i = 0; i < arrColumns.Length; i++)
                {
                    arrColumns[i] = (i + 1).ToString();
                }

                var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                sheet.isColumnWritten = false;
                sheet.isWriteHeader = true;
                sheet.strHeaderText = "科目满意统计";
                //开始表格
                var dt = Code.Common.ArrayToDataTable(arrColumns);
                var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                var arrSurveyItemID = new string[allColumnLength];
                var arrSurveyItemName = new string[allColumnLength];
                var arrSurveyOptionID = new string[allColumnLength];
                var arrSurveyOptionName = new string[allColumnLength];
                var index = 0;
                for (int i = 0; i < itemList.Count(); i++)
                {
                    var optionList = vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == itemList[i].Id).Distinct().ToList();
                    for (var j = 0; j < optionList.Count(); j++)
                    {
                        index++;
                        arrSurveyItemID[index] = itemList[i].Id.ToString();
                        arrSurveyItemName[index] = itemList[i].SurveyItemName.ToString();
                        arrSurveyOptionID[index] = optionList[j].Id.ToString();
                        arrSurveyOptionName[index] = optionList[j].OptionName.ToString();
                    }
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, index - optionList.Count() + 1, index));
                }
                arrSurveyItemName[index + 1] = "评价次数";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index + 1, index + 1));
                arrSurveyItemName[index + 2] = "满意度比例";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index + 2, index + 2));
                index = index + 2;
                for (int i = 0; i < itemTextList.Count(); i++)
                {
                    index++;
                    arrSurveyItemID[index] = itemTextList[i].Id.ToString();
                    arrSurveyItemName[index] = itemTextList[i].SurveyItemName.ToString();
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index, index));
                }
                dt.Rows.Add(arrSurveyItemName);
                dt.Rows.Add(arrSurveyOptionName);
                #region 处理DataList
                foreach (var coures in vm.SurveySubjectTotalRankingList)
                {
                    string[] arrSurveyData = new string[allColumnLength];
                    arrSurveyData[0] = coures.CourseName;
                    var indexR = 0;
                    foreach (var item in vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                    {
                        foreach (var option in vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList())
                        {
                            indexR++;
                            var optionCount = vm.SurveySubjectTotalList
                                .Where(d => d.CourseId == coures.CourseId && d.SurveyItemId == item.Id && d.SurveyOptionId == option.Id)
                                .Sum(d => d.SurveyOptionCount);

                            if (optionCount > decimal.Zero)
                            {
                                arrSurveyData[indexR] = optionCount.ToString();
                            }
                        }
                    }
                    indexR++;
                    if (coures.SurveyAllCount > decimal.Zero)
                    {
                        arrSurveyData[indexR] = coures.SurveyAllCount.ToString();
                    }
                    indexR++;
                    if (coures.CourseGoodRate > decimal.Zero)
                    {
                        arrSurveyData[indexR] = coures.CourseGoodRate.ToString() + "%";
                    }
                    foreach (var item in vm.SurveyItemInfoList.Where(d => d.SurveyItemType == XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                    {
                        indexR++;
                        var optionTextCount = vm.SurveySubjectTotalTextList
                            .Where(d => d.CourseId == coures.CourseId && d.SurveyItemId == item.Id)
                            .Sum(d => d.SurveyOptionCount);

                        if (optionTextCount > decimal.Zero)
                        {
                            arrSurveyData[indexR] = optionTextCount.ToString();
                        }
                    }
                    dt.Rows.Add(arrSurveyData);
                }
                #endregion
                dt.Rows[0][0] = "课程";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));
                sheet.data = dt;
                sheet.regions = regions;
                sheetList.Add(sheet);
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

        public ActionResult SubjectTeacherList()
        {
            var vm = new Models.SurveyReport.SubjectTeacherList();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                #region 任课教师
                var tbCourseIds = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                   join m in db.Table<Entity.tbSurvey>() on p.tbOrg.tbYear.Id equals m.tbYear.Id
                                   where p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                   && p.tbOrg.IsDeleted == false
                                   && p.tbTeacher.IsDeleted == false
                                   && m.Id == vm.SurveyId
                                   select p.tbOrg.tbCourse.Id).Distinct().ToList();

                vm.SurveyCourseList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                       join m in db.Table<Entity.tbSurveyCourse>()
                                       on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                       equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                       where p.tbOrg.IsDeleted == false
                                       && tbCourseIds.Contains(m.tbCourse.Id)
                                       && m.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                       orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                       select new Dto.SurveyReport.SubjectTeacherList
                                       {
                                           CourseNo = p.tbOrg.tbCourse.No,
                                           CourseId = p.tbOrg.tbCourse.Id,
                                           CourseName = p.tbOrg.tbCourse.CourseName,
                                           TeacherId = p.tbTeacher.Id,
                                           TeacherName = p.tbTeacher.TeacherName
                                       }).Distinct().ToList();

                var surveyCourseIds = new List<int>();
                surveyCourseIds = vm.SurveyCourseList.Select(d => d.CourseId).ToList();

                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where surveyCourseIds.Contains(p.tbCourse.Id)
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        select p.tbSurveyGroup.Id).Distinct().ToList();
                #region 单选分数
                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                        && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectTeacherList = (from p in tbSurveryDataList
                                               group p by new
                                               {
                                                   courseId = p.tbOrg.tbCourse.Id,
                                                   courseName = p.tbOrg.tbCourse.CourseName,
                                                   surveryItemId = p.tbSurveyItem.Id,
                                                   surveryItemName = p.tbSurveyItem.SurveyItemName,
                                                   surverOptionId = p.tbSurveyOption.Id,
                                                   surverOptionName = p.tbSurveyOption.OptionName
                                               } into g
                                               select new Dto.SurveyReport.SubjectTeacherList
                                               {
                                                   CourseId = g.Key.courseId,
                                                   CourseName = g.Key.courseName,
                                                   SurveyItemId = g.Key.surveryItemId,
                                                   SurveyItemName = g.Key.surveryItemName,
                                                   SurveyOptionId = g.Key.surverOptionId,
                                                   SurveyOptionName = g.Key.surverOptionName,
                                                   SurveyOptionCount = g.Count(),
                                                   SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                                   SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                               }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataTextList = from p in db.Table<Entity.tbSurveyData>()
                                            where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                            && p.tbOrg != null
                                            && p.tbClass == null
                                            && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                            && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                            select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataTextList = tbSurveryDataTextList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectTeacherTextList = (from p in tbSurveryDataTextList
                                                   group p by new
                                                   {
                                                       courseId = p.tbOrg.tbCourse.Id,
                                                       courseName = p.tbOrg.tbCourse.CourseName,
                                                       surveryItemId = p.tbSurveyItem.Id,
                                                       surveryItemName = p.tbSurveyItem.SurveyItemName
                                                   } into g
                                                   select new Dto.SurveyReport.SubjectTeacherList
                                                   {
                                                       CourseId = g.Key.courseId,
                                                       CourseName = g.Key.courseName,
                                                       SurveyItemId = g.Key.surveryItemId,
                                                       SurveyItemName = g.Key.surveryItemName,
                                                       SurveyOptionCount = g.Count()
                                                   }).ToList();
                #endregion

                vm.SurveyItemInfoList = (from p in db.Table<Entity.tbSurveyItem>()
                                         where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                         && tbSurveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                         orderby p.tbSurveyGroup.No, p.No
                                         select new Dto.SurveyItem.Info
                                         {
                                             Id = p.Id,
                                             SurveyItemName = p.SurveyItemName,
                                             SurveyItemType = p.SurveyItemType,
                                             No = p.No
                                         }).Distinct().ToList();

                vm.SurveyOptionInfoList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                           && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                           orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               OptionValue = p.OptionValue,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).Distinct().ToList();
                #endregion

                #region 班 主 任

                vm.SurveyTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                        join m in db.Table<Entity.tbSurveyClass>()
                                        on p.tbClass.Id equals m.tbClass.Id
                                        where p.tbClass.IsDeleted == false
                                        && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                        && m.tbSurvey.Id == vm.SurveyId
                                        orderby p.tbClass.No, p.tbClass.ClassName, p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new Dto.SurveyReport.ClassTeacherList
                                        {
                                            GradeNo = p.tbClass.tbGrade.No,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            ClassNo = p.tbClass.No,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            TeacherId = p.tbTeacher.Id,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).Distinct().ToList();

                var classIds = vm.SurveyTeacherList.Select(d => d.ClassId).Distinct().ToList();
                var teacherIds = vm.SurveyTeacherList.Select(d => d.TeacherId).Distinct().ToList();

                #region 单选分数
                var tbSurveryDataClassList = from p in db.Table<Entity.tbSurveyData>()
                                             where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                             && p.tbClass != null
                                             && classIds.Contains(p.tbClass.Id)
                                             && teacherIds.Contains(p.tbTeacher.Id)
                                             && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                             && p.tbClass.IsDeleted == false
                                             && p.tbStudent.IsDeleted == false
                                             && p.tbSurveyItem.IsDeleted == false
                                             && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                             && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                             select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataClassList = tbSurveryDataClassList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyClassTeacherList = (from p in tbSurveryDataClassList
                                             group p by new
                                             {
                                                 classId = p.tbClass.Id,
                                                 className = p.tbClass.ClassName,
                                                 teacherId = p.tbTeacher.Id,
                                                 teacherName = p.tbTeacher.TeacherName,
                                                 surveryItemId = p.tbSurveyItem.Id,
                                                 surveryItemName = p.tbSurveyItem.SurveyItemName,
                                                 surverOptionId = p.tbSurveyOption.Id,
                                                 surverOptionName = p.tbSurveyOption.OptionName
                                             } into g
                                             select new Dto.SurveyReport.ClassTeacherList
                                             {
                                                 ClassId = g.Key.classId,
                                                 ClassName = g.Key.className,
                                                 SurveyItemId = g.Key.surveryItemId,
                                                 SurveyItemName = g.Key.surveryItemName,
                                                 SurveyOptionId = g.Key.surverOptionId,
                                                 SurveyOptionName = g.Key.surverOptionName,
                                                 TeacherId = g.Key.teacherId,
                                                 TeacherName = g.Key.teacherName,
                                                 SurveyOptionCount = g.Count(),
                                                 SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                                 SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                             }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataTextClassList = from p in db.Table<Entity.tbSurveyData>()
                                                 where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                 && p.tbClass != null
                                                 && classIds.Contains(p.tbClass.Id)
                                                 && teacherIds.Contains(p.tbTeacher.Id)
                                                 && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                                 && p.tbClass.IsDeleted == false
                                                 && p.tbStudent.IsDeleted == false
                                                 && p.tbSurveyItem.IsDeleted == false
                                                 && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                                 && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                                 select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataTextClassList = tbSurveryDataTextClassList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyClassTeacherTextList = (from p in tbSurveryDataTextClassList
                                                 group p by new
                                                 {
                                                     classId = p.tbClass.Id,
                                                     className = p.tbClass.ClassName,
                                                     teacherId = p.tbTeacher.Id,
                                                     teacherName = p.tbTeacher.TeacherName,
                                                     surveryItemId = p.tbSurveyItem.Id,
                                                     surveryItemName = p.tbSurveyItem.SurveyItemName
                                                 } into g
                                                 select new Dto.SurveyReport.ClassTeacherList
                                                 {
                                                     ClassId = g.Key.classId,
                                                     ClassName = g.Key.className,
                                                     SurveyItemId = g.Key.surveryItemId,
                                                     SurveyItemName = g.Key.surveryItemName,
                                                     TeacherId = g.Key.teacherId,
                                                     TeacherName = g.Key.teacherName,
                                                     SurveyOptionCount = g.Count()
                                                 }).ToList();
                #endregion

                vm.SurveyItemInfoClassList = (from p in db.Table<Entity.tbSurveyItem>()
                                              where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                              && p.tbSurveyGroup.IsOrg == false
                                              orderby p.tbSurveyGroup.No, p.No
                                              select new Dto.SurveyItem.Info
                                              {
                                                  Id = p.Id,
                                                  SurveyItemName = p.SurveyItemName,
                                                  SurveyItemType = p.SurveyItemType,
                                                  No = p.No
                                              }).Distinct().ToList();

                vm.SurveyOptionInfoClassList = (from p in db.Table<Entity.tbSurveyOption>()
                                                where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                                orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                                select new Dto.SurveyOption.Info
                                                {
                                                    Id = p.Id,
                                                    No = p.No,
                                                    OptionName = p.OptionName,
                                                    OptionValue = p.OptionValue,
                                                    SurveyItemId = p.tbSurveyItem.Id
                                                }).Distinct().ToList();
                #endregion
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubjectTeacherList(Models.SurveyReport.SubjectTeacherList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SubjectTeacherList", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId
            }));
        }

        public ActionResult SubjectTeacherListExport()
        {
            var vm = new Models.SurveyReport.SubjectTeacherList();
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 准备数据
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                #region 任课教师
                var tbCourseIds = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                   join m in db.Table<Entity.tbSurvey>() on p.tbOrg.tbYear.Id equals m.tbYear.Id
                                   where p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                   && p.tbOrg.IsDeleted == false
                                   && p.tbTeacher.IsDeleted == false
                                   && m.Id == vm.SurveyId
                                   select p.tbOrg.tbCourse.Id).Distinct().ToList();

                vm.SurveyCourseList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                       join m in db.Table<Entity.tbSurveyCourse>()
                                       on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                       equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                       where p.tbOrg.IsDeleted == false
                                       && tbCourseIds.Contains(m.tbCourse.Id)
                                       && m.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                       orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                       select new Dto.SurveyReport.SubjectTeacherList
                                       {
                                           CourseNo = p.tbOrg.tbCourse.No,
                                           CourseId = p.tbOrg.tbCourse.Id,
                                           CourseName = p.tbOrg.tbCourse.CourseName,
                                           TeacherId = p.tbTeacher.Id,
                                           TeacherName = p.tbTeacher.TeacherName
                                       }).Distinct().ToList();

                var surveyCourseIds = new List<int>();
                surveyCourseIds = vm.SurveyCourseList.Select(d => d.CourseId).ToList();

                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where surveyCourseIds.Contains(p.tbCourse.Id)
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        select p.tbSurveyGroup.Id).Distinct().ToList();
                #region 单选分数
                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                        && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectTeacherList = (from p in tbSurveryDataList
                                               group p by new
                                               {
                                                   courseId = p.tbOrg.tbCourse.Id,
                                                   courseName = p.tbOrg.tbCourse.CourseName,
                                                   surveryItemId = p.tbSurveyItem.Id,
                                                   surveryItemName = p.tbSurveyItem.SurveyItemName,
                                                   surverOptionId = p.tbSurveyOption.Id,
                                                   surverOptionName = p.tbSurveyOption.OptionName
                                               } into g
                                               select new Dto.SurveyReport.SubjectTeacherList
                                               {
                                                   CourseId = g.Key.courseId,
                                                   CourseName = g.Key.courseName,
                                                   SurveyItemId = g.Key.surveryItemId,
                                                   SurveyItemName = g.Key.surveryItemName,
                                                   SurveyOptionId = g.Key.surverOptionId,
                                                   SurveyOptionName = g.Key.surverOptionName,
                                                   SurveyOptionCount = g.Count(),
                                                   SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                                   SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                               }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataTextList = from p in db.Table<Entity.tbSurveyData>()
                                            where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                            && p.tbOrg != null
                                            && p.tbClass == null
                                            && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                            && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                            select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataTextList = tbSurveryDataTextList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveySubjectTeacherTextList = (from p in tbSurveryDataTextList
                                                   group p by new
                                                   {
                                                       courseId = p.tbOrg.tbCourse.Id,
                                                       courseName = p.tbOrg.tbCourse.CourseName,
                                                       surveryItemId = p.tbSurveyItem.Id,
                                                       surveryItemName = p.tbSurveyItem.SurveyItemName
                                                   } into g
                                                   select new Dto.SurveyReport.SubjectTeacherList
                                                   {
                                                       CourseId = g.Key.courseId,
                                                       CourseName = g.Key.courseName,
                                                       SurveyItemId = g.Key.surveryItemId,
                                                       SurveyItemName = g.Key.surveryItemName,
                                                       SurveyOptionCount = g.Count()
                                                   }).ToList();
                #endregion

                vm.SurveyItemInfoList = (from p in db.Table<Entity.tbSurveyItem>()
                                         where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                         && tbSurveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                         orderby p.tbSurveyGroup.No, p.No
                                         select new Dto.SurveyItem.Info
                                         {
                                             Id = p.Id,
                                             SurveyItemName = p.SurveyItemName,
                                             SurveyItemType = p.SurveyItemType,
                                             No = p.No
                                         }).Distinct().ToList();

                vm.SurveyOptionInfoList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                           && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                           orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               OptionValue = p.OptionValue,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).Distinct().ToList();
                #endregion

                #region 班 主 任

                vm.SurveyTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                        join m in db.Table<Entity.tbSurveyClass>()
                                        on p.tbClass.Id equals m.tbClass.Id
                                        where p.tbClass.IsDeleted == false
                                        && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                        && m.tbSurvey.Id == vm.SurveyId
                                        orderby p.tbClass.No, p.tbClass.ClassName, p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new Dto.SurveyReport.ClassTeacherList
                                        {
                                            GradeNo = p.tbClass.tbGrade.No,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            ClassNo = p.tbClass.No,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            TeacherId = p.tbTeacher.Id,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).Distinct().ToList();

                var classIds = vm.SurveyTeacherList.Select(d => d.ClassId).Distinct().ToList();
                var teacherIds = vm.SurveyTeacherList.Select(d => d.TeacherId).Distinct().ToList();

                #region 单选分数
                var tbSurveryDataClassList = from p in db.Table<Entity.tbSurveyData>()
                                             where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                             && p.tbClass != null
                                             && classIds.Contains(p.tbClass.Id)
                                             && teacherIds.Contains(p.tbTeacher.Id)
                                             && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                             && p.tbClass.IsDeleted == false
                                             && p.tbStudent.IsDeleted == false
                                             && p.tbSurveyItem.IsDeleted == false
                                             && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                             && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                             select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataClassList = tbSurveryDataClassList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyClassTeacherList = (from p in tbSurveryDataClassList
                                             group p by new
                                             {
                                                 classId = p.tbClass.Id,
                                                 className = p.tbClass.ClassName,
                                                 teacherId = p.tbTeacher.Id,
                                                 teacherName = p.tbTeacher.TeacherName,
                                                 surveryItemId = p.tbSurveyItem.Id,
                                                 surveryItemName = p.tbSurveyItem.SurveyItemName,
                                                 surverOptionId = p.tbSurveyOption.Id,
                                                 surverOptionName = p.tbSurveyOption.OptionName
                                             } into g
                                             select new Dto.SurveyReport.ClassTeacherList
                                             {
                                                 ClassId = g.Key.classId,
                                                 ClassName = g.Key.className,
                                                 SurveyItemId = g.Key.surveryItemId,
                                                 SurveyItemName = g.Key.surveryItemName,
                                                 SurveyOptionId = g.Key.surverOptionId,
                                                 SurveyOptionName = g.Key.surverOptionName,
                                                 TeacherId = g.Key.teacherId,
                                                 TeacherName = g.Key.teacherName,
                                                 SurveyOptionCount = g.Count(),
                                                 SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                                 SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                             }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataTextClassList = from p in db.Table<Entity.tbSurveyData>()
                                                 where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                 && p.tbClass != null
                                                 && classIds.Contains(p.tbClass.Id)
                                                 && teacherIds.Contains(p.tbTeacher.Id)
                                                 && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                                 && p.tbClass.IsDeleted == false
                                                 && p.tbStudent.IsDeleted == false
                                                 && p.tbSurveyItem.IsDeleted == false
                                                 && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                                 && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                                 select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataTextClassList = tbSurveryDataTextClassList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyClassTeacherTextList = (from p in tbSurveryDataTextClassList
                                                 group p by new
                                                 {
                                                     classId = p.tbClass.Id,
                                                     className = p.tbClass.ClassName,
                                                     teacherId = p.tbTeacher.Id,
                                                     teacherName = p.tbTeacher.TeacherName,
                                                     surveryItemId = p.tbSurveyItem.Id,
                                                     surveryItemName = p.tbSurveyItem.SurveyItemName
                                                 } into g
                                                 select new Dto.SurveyReport.ClassTeacherList
                                                 {
                                                     ClassId = g.Key.classId,
                                                     ClassName = g.Key.className,
                                                     SurveyItemId = g.Key.surveryItemId,
                                                     SurveyItemName = g.Key.surveryItemName,
                                                     TeacherId = g.Key.teacherId,
                                                     TeacherName = g.Key.teacherName,
                                                     SurveyOptionCount = g.Count()
                                                 }).ToList();
                #endregion

                vm.SurveyItemInfoClassList = (from p in db.Table<Entity.tbSurveyItem>()
                                              where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                              && p.tbSurveyGroup.IsOrg == false
                                              orderby p.tbSurveyGroup.No, p.No
                                              select new Dto.SurveyItem.Info
                                              {
                                                  Id = p.Id,
                                                  SurveyItemName = p.SurveyItemName,
                                                  SurveyItemType = p.SurveyItemType,
                                                  No = p.No
                                              }).Distinct().ToList();

                vm.SurveyOptionInfoClassList = (from p in db.Table<Entity.tbSurveyOption>()
                                                where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                                orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                                select new Dto.SurveyOption.Info
                                                {
                                                    Id = p.Id,
                                                    No = p.No,
                                                    OptionName = p.OptionName,
                                                    OptionValue = p.OptionValue,
                                                    SurveyItemId = p.tbSurveyItem.Id
                                                }).Distinct().ToList();
                #endregion
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();

                if (vm.SurveyTeacherList != null && vm.SurveyTeacherList.Count > decimal.Zero)
                {
                    #region 班 主 任
                    var surveyOptionCount = 0;
                    var itemList = vm.SurveyItemInfoClassList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                    foreach (var item in itemList)
                    {
                        var optionList = vm.SurveyOptionInfoClassList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList();
                        foreach (var option in optionList)
                        {
                            surveyOptionCount++;
                        }
                    }
                    var itemTextList = vm.SurveyItemInfoClassList.Where(d => d.SurveyItemType == XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                    foreach (var item in itemTextList)
                    {
                        surveyOptionCount++;
                    }
                    var allColumnLength = surveyOptionCount + 5;
                    var arrColumns = new string[allColumnLength];

                    for (int i = 0; i < arrColumns.Length; i++)
                    {
                        arrColumns[i] = (i + 1).ToString();
                    }

                    var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                    sheet.isColumnWritten = false;
                    sheet.isWriteHeader = true;
                    sheet.strHeaderText = "班主任评价统计";
                    //开始表格
                    var dt = Code.Common.ArrayToDataTable(arrColumns);
                    var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                    var arrSurveyItemID = new string[allColumnLength];
                    var arrSurveyItemName = new string[allColumnLength];
                    var arrSurveyOptionID = new string[allColumnLength];
                    var arrSurveyOptionName = new string[allColumnLength];
                    var index = 2;
                    for (int i = 0; i < itemList.Count(); i++)
                    {
                        var optionList = vm.SurveyOptionInfoClassList.Where(d => d.SurveyItemId == itemList[i].Id).Distinct().ToList();
                        for (var j = 0; j < optionList.Count(); j++)
                        {
                            index++;
                            arrSurveyItemID[index] = itemList[i].Id.ToString();
                            arrSurveyItemName[index] = itemList[i].SurveyItemName.ToString();
                            arrSurveyOptionID[index] = optionList[j].Id.ToString();
                            arrSurveyOptionName[index] = optionList[j].OptionName.ToString();
                        }
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, index - optionList.Count() + 1, index));
                    }
                    arrSurveyItemName[index + 1] = "评价次数";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index + 1, index + 1));
                    arrSurveyItemName[index + 2] = "满意度比例";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index + 2, index + 2));
                    index = index + 2;
                    for (int i = 0; i < itemTextList.Count(); i++)
                    {
                        index++;
                        arrSurveyItemID[index] = itemTextList[i].Id.ToString();
                        arrSurveyItemName[index] = itemTextList[i].SurveyItemName.ToString();
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index, index));
                    }
                    dt.Rows.Add(arrSurveyItemName);
                    dt.Rows.Add(arrSurveyOptionName);
                    #region 处理DataList
                    foreach (var teacher in vm.SurveyTeacherList.OrderBy(d => d.GradeNo).ThenBy(d => d.GradeName).ThenBy(d => d.TeacherName).ToList())
                    {
                        string[] arrSurveyData = new string[allColumnLength];
                        arrSurveyData[0] = teacher.GradeName;
                        arrSurveyData[1] = teacher.ClassName;
                        arrSurveyData[2] = teacher.TeacherName;
                        var allCount = 0;
                        var goodCount = 0m;
                        var goodRate = 0m;
                        var indexR = 2;
                        foreach (var item in vm.SurveyItemInfoClassList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                        {
                            foreach (var option in vm.SurveyOptionInfoClassList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList())
                            {
                                indexR++;
                                var optionCount = vm.SurveyClassTeacherList
                                    .Where(d => d.TeacherId == teacher.TeacherId && d.ClassId == teacher.ClassId && d.SurveyItemId == item.Id && d.SurveyOptionId == option.Id)
                                    .Sum(d => d.SurveyOptionCount);
                                if (option.OptionName.Contains("A：非常满意") || option.OptionName.Contains("B：满意"))
                                {
                                    goodCount += optionCount;
                                }
                                allCount += optionCount;
                                if (optionCount > decimal.Zero)
                                {
                                    arrSurveyData[indexR] = optionCount.ToString();
                                }
                            }
                        }
                        indexR++;
                        if (allCount > decimal.Zero)
                        {
                            arrSurveyData[indexR] = allCount.ToString();
                        }
                        if (allCount > decimal.Zero)
                        {
                            goodRate = Decimal.Round(goodCount / allCount * 100, 2);
                        }
                        indexR++;
                        if (goodRate > decimal.Zero)
                        {
                            arrSurveyData[indexR] = goodRate.ToString() + "%";
                        }
                        foreach (var item in vm.SurveyItemInfoClassList.Where(d => d.SurveyItemType == XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                        {
                            indexR++;
                            var optionTextCount = vm.SurveyClassTeacherTextList
                                .Where(d => d.TeacherId == teacher.TeacherId && d.ClassId == teacher.ClassId && d.SurveyItemId == item.Id)
                                .Sum(d => d.SurveyOptionCount);
                            if (optionTextCount > 0)
                            {
                                arrSurveyData[indexR] = optionTextCount.ToString();
                            }
                        }
                        dt.Rows.Add(arrSurveyData);
                    }
                    #endregion
                    dt.Rows[0][0] = "课程";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));
                    dt.Rows[0][1] = "行政班";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 1, 1));
                    dt.Rows[0][2] = "班主任";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 2, 2));
                    sheet.data = dt;
                    sheet.regions = regions;
                    sheetList.Add(sheet);
                    #endregion
                }

                if (vm.SurveyCourseList != null && vm.SurveyCourseList.Count > decimal.Zero)
                {
                    #region 任课教师
                    var surveyOptionCount = 0;
                    var itemList = vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                    foreach (var item in itemList)
                    {
                        var optionList = vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList();
                        foreach (var option in optionList)
                        {
                            surveyOptionCount++;
                        }
                    }
                    var itemTextList = vm.SurveyItemInfoList.Where(d => d.SurveyItemType == XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                    foreach (var item in itemTextList)
                    {
                        surveyOptionCount++;
                    }
                    var allColumnLength = surveyOptionCount + 3;
                    var arrColumns = new string[allColumnLength];

                    for (int i = 0; i < arrColumns.Length; i++)
                    {
                        arrColumns[i] = (i + 1).ToString();
                    }

                    var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                    sheet.isColumnWritten = false;
                    sheet.isWriteHeader = true;
                    sheet.strHeaderText = "任课教师评价统计";
                    //开始表格
                    var dt = Code.Common.ArrayToDataTable(arrColumns);
                    var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                    var arrSurveyItemID = new string[allColumnLength];
                    var arrSurveyItemName = new string[allColumnLength];
                    var arrSurveyOptionID = new string[allColumnLength];
                    var arrSurveyOptionName = new string[allColumnLength];
                    var index = 0;
                    for (int i = 0; i < itemList.Count(); i++)
                    {
                        var optionList = vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == itemList[i].Id).Distinct().ToList();
                        for (var j = 0; j < optionList.Count(); j++)
                        {
                            index++;
                            arrSurveyItemID[index] = itemList[i].Id.ToString();
                            arrSurveyItemName[index] = itemList[i].SurveyItemName.ToString();
                            arrSurveyOptionID[index] = optionList[j].Id.ToString();
                            arrSurveyOptionName[index] = optionList[j].OptionName.ToString();
                        }
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, index - optionList.Count() + 1, index));
                    }
                    arrSurveyItemName[index + 1] = "评价次数";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index + 1, index + 1));
                    arrSurveyItemName[index + 2] = "满意度比例";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index + 2, index + 2));
                    index = index + 2;
                    for (int i = 0; i < itemTextList.Count(); i++)
                    {
                        index++;
                        arrSurveyItemID[index] = itemTextList[i].Id.ToString();
                        arrSurveyItemName[index] = itemTextList[i].SurveyItemName.ToString();
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index, index));
                    }
                    dt.Rows.Add(arrSurveyItemName);
                    dt.Rows.Add(arrSurveyOptionName);
                    #region 处理DataList
                    foreach (var coures in vm.SurveyCourseList)
                    {
                        string[] arrSurveyData = new string[allColumnLength];
                        arrSurveyData[0] = coures.CourseName;
                        var allCount = 0;
                        var goodCount = 0m;
                        var goodRate = 0m;
                        var indexR = 0;
                        foreach (var item in vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                        {
                            foreach (var option in vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList())
                            {
                                indexR++;
                                var optionCount = vm.SurveySubjectTeacherList
                                    .Where(d => d.CourseId == coures.CourseId && d.SurveyItemId == item.Id && d.SurveyOptionId == option.Id)
                                    .Sum(d => d.SurveyOptionCount);
                                if (option.OptionName.Contains("A：非常满意") || option.OptionName.Contains("B：满意"))
                                {
                                    goodCount += optionCount;
                                }
                                allCount += optionCount;
                                if (optionCount > decimal.Zero)
                                {
                                    arrSurveyData[indexR] = optionCount.ToString();
                                }
                            }
                        }
                        indexR++;
                        if (allCount > decimal.Zero)
                        {
                            arrSurveyData[indexR] = allCount.ToString();
                        }
                        if (allCount > decimal.Zero)
                        {
                            goodRate = Decimal.Round(goodCount / allCount * 100, 2);
                        }
                        indexR++;
                        if (goodRate > decimal.Zero)
                        {
                            arrSurveyData[indexR] = goodRate.ToString() + "%";
                        }
                        foreach (var item in vm.SurveyItemInfoList.Where(d => d.SurveyItemType == XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                        {
                            indexR++;
                            var optionTextCount = vm.SurveySubjectTeacherTextList
                                .Where(d => d.CourseId == coures.CourseId && d.SurveyItemId == item.Id)
                                .Sum(d => d.SurveyOptionCount);
                            if (optionTextCount > 0)
                            {
                                arrSurveyData[indexR] = optionTextCount.ToString();
                            }
                        }
                        dt.Rows.Add(arrSurveyData);
                    }
                    #endregion
                    dt.Rows[0][0] = "课程";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));
                    sheet.data = dt;
                    sheet.regions = regions;
                    sheetList.Add(sheet);
                    #endregion
                }

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

        public ActionResult UnSubjectList()
        {
            var vm = new Models.SurveyReport.UnSubjectList();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveySubjectList = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                        && p.tbSurveyGroup.IsDeleted == false
                                        && p.tbSurveyGroup.IsOrg
                                        && p.tbCourse.IsDeleted == false
                                        && p.tbCourse.tbSubject.IsDeleted == false
                                        orderby p.tbCourse.tbSubject.No
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbCourse.tbSubject.SubjectName,
                                            Value = p.tbCourse.tbSubject.Id.ToString()
                                        }).Distinct().ToList();

                if (vm.SurveySubjectId == 0 && vm.SurveySubjectList.Count > 0)
                {
                    vm.SurveySubjectId = vm.SurveySubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbCourse.tbSubject.IsDeleted == false
                                       && p.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                var surveyCourseIds = new List<int>();

                if (vm.SurveyCourseId == 0)
                {
                    surveyCourseIds = vm.SurveyCourseList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyCourseIds.Add((int)vm.SurveyCourseId);
                }

                vm.SurveyTeacherList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                        join m in db.Table<Entity.tbSurveyCourse>()
                                        on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                        equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                        where p.tbOrg.IsDeleted == false
                                        && surveyCourseIds.Contains(m.tbCourse.Id)
                                        orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new Dto.SurveyReport.UnSubjectList
                                        {
                                            OrgId = p.tbOrg.Id,
                                            OrgName = p.tbOrg.OrgName,
                                            IsClass = p.tbOrg.IsClass,
                                            ClassId = p.tbOrg.tbClass == null ? 0 : p.tbOrg.tbClass.Id,
                                            ClassName = p.tbOrg.tbClass == null ? "" : p.tbOrg.tbClass.ClassName,
                                            CourseNo = p.tbOrg.tbCourse.No,
                                            CourseId = p.tbOrg.tbCourse.Id,
                                            CourseName = p.tbOrg.tbCourse.CourseName,
                                            TeacherId = p.tbTeacher.Id,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).Distinct().ToList();

                #region 学生信息
                var tbClassIds = vm.SurveyTeacherList.Where(d => d.IsClass && d.ClassId != 0).Select(d => d.ClassId).Distinct().ToList();
                var tbOrgIds = vm.SurveyTeacherList.Select(d => d.OrgId).Distinct().ToList();

                var tbClassStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                          where tbClassIds.Contains(p.tbClass.Id)
                                          && p.tbClass.IsDeleted == false
                                          && p.tbStudent.IsDeleted == false
                                          select new
                                          {
                                              classId = p.tbClass.Id,
                                              studentId = p.tbStudent.Id
                                          }).ToList();

                var tbOrgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                        where tbOrgIds.Contains(p.tbOrg.Id)
                                        && p.tbOrg.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        select new
                                        {
                                            orgId = p.tbOrg.Id,
                                            studentId = p.tbStudent.Id
                                        }).ToList();

                var allStudentList = new List<Dto.SurveyReport.UnSubjectList>();
                foreach (var a in vm.SurveyTeacherList.GroupBy(d => new { d.OrgId, d.IsClass, d.ClassId }).Select(g => new { g.Key.OrgId, g.Key.IsClass, g.Key.ClassId }))
                {
                    if (a.IsClass)
                    {
                        if (a.ClassId > decimal.Zero)
                        {
                            var studengClass = (from p in tbClassStudentList
                                                where p.classId == a.ClassId
                                                select new Dto.SurveyReport.UnSubjectList
                                                {
                                                    OrgId = a.OrgId,
                                                    StudentId = p.studentId
                                                }).ToList();

                            allStudentList = allStudentList.Union(studengClass).ToList();
                        }
                    }
                    else
                    {
                        var studengClass = (from p in tbOrgStudentList
                                            where p.orgId == a.OrgId
                                            select new Dto.SurveyReport.UnSubjectList
                                            {
                                                OrgId = p.orgId,
                                                StudentId = p.studentId
                                            }).ToList();

                        allStudentList = allStudentList.Union(studengClass).ToList();
                    }
                }
                #endregion

                var studentGroup = (from p in allStudentList
                                    group p by new { p.OrgId } into g
                                    select new
                                    {
                                        orgId = g.Key.OrgId,
                                        count = g.Count()
                                    }).ToList();

                var teacherIds = vm.SurveyTeacherList.Select(d => d.TeacherId).Distinct().ToList();

                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where surveyCourseIds.Contains(p.tbCourse.Id)
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        select p.tbSurveyGroup.Id).Distinct().ToList();
                #region 单选分数
                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && tbOrgIds.Contains(p.tbOrg.Id)
                                        && teacherIds.Contains(p.tbTeacher.Id)
                                        select p;

                if (vm.SurveyCourseId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => tbSurveyGroupIds.Contains(d.tbSurveyItem.tbSurveyGroup.Id));
                }

                if (vm.SurveyTeacherId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.tbTeacher.Id == vm.SurveyTeacherId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyUnSubjectList = (from p in tbSurveryDataList
                                          group p by new
                                          {
                                              orgId = p.tbOrg.Id,
                                              courseId = p.tbOrg.tbCourse.Id,
                                              teacherId = p.tbTeacher.Id,
                                              studentId = p.tbStudent.Id
                                          } into g
                                          select new Dto.SurveyReport.UnSubjectList
                                          {
                                              OrgId = g.Key.orgId,
                                              CourseId = g.Key.courseId,
                                              TeacherId = g.Key.teacherId,
                                              StudentId = g.Key.studentId
                                          }).ToList();
                #endregion

                var studentAllList = vm.SurveyUnSubjectList.Select(d => new { d.OrgId, d.CourseId, d.TeacherId, d.StudentId }).Distinct().ToList();

                var surveyGroupList = (from p in studentAllList
                                       group p by new { p.OrgId, p.CourseId, p.TeacherId } into g
                                       select new Dto.SurveyReport.UnSubjectList
                                       {
                                           OrgId = g.Key.OrgId,
                                           CourseId = g.Key.CourseId,
                                           TeacherId = g.Key.TeacherId,
                                           SurveyCount = g.Count()
                                       }).Distinct().ToList();

                vm.SurveyUnSubjectEachList = vm.SurveyTeacherList;

                foreach (var a in vm.SurveyUnSubjectEachList)
                {
                    var surveyCount = surveyGroupList.Where(d => d.OrgId == a.OrgId && d.CourseId == a.CourseId && d.TeacherId == a.TeacherId).FirstOrDefault();
                    if (surveyCount == null)
                    {

                    }
                    else
                    {
                        //已选
                        a.SurveyCount = surveyCount.SurveyCount;
                    }

                    var studentallCount = studentGroup.Where(d => d.orgId == a.OrgId).FirstOrDefault();
                    if (studentallCount == null)
                    {

                    }
                    else
                    {
                        //未选
                        a.SurveyAllCount = studentallCount.count;
                        a.UnSurveyCount = a.SurveyAllCount - a.SurveyCount;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnSubjectList(Models.SurveyReport.UnSubjectList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("UnSubjectList", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveySubjectId = vm.SurveySubjectId,
                SurveyCourseId = vm.SurveyCourseId
            }));
        }

        public ActionResult UnSubjectListExport()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                #region 数据准备
                var vm = new Models.SurveyReport.UnSubjectList();
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveySubjectList = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                        && p.tbSurveyGroup.IsDeleted == false
                                        && p.tbSurveyGroup.IsOrg
                                        && p.tbCourse.IsDeleted == false
                                        && p.tbCourse.tbSubject.IsDeleted == false
                                        orderby p.tbCourse.tbSubject.No
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Text = p.tbCourse.tbSubject.SubjectName,
                                            Value = p.tbCourse.tbSubject.Id.ToString()
                                        }).Distinct().ToList();

                if (vm.SurveySubjectId == 0 && vm.SurveySubjectList.Count > 0)
                {
                    vm.SurveySubjectId = vm.SurveySubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbCourse.tbSubject.IsDeleted == false
                                       && p.tbCourse.tbSubject.Id == vm.SurveySubjectId
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                var surveyCourseIds = new List<int>();

                if (vm.SurveyCourseId == 0)
                {
                    surveyCourseIds = vm.SurveyCourseList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyCourseIds.Add((int)vm.SurveyCourseId);
                }

                vm.SurveyTeacherList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                        join m in db.Table<Entity.tbSurveyCourse>()
                                        on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                        equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                        where p.tbOrg.IsDeleted == false
                                        && surveyCourseIds.Contains(m.tbCourse.Id)
                                        orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new Dto.SurveyReport.UnSubjectList
                                        {
                                            OrgId = p.tbOrg.Id,
                                            OrgName = p.tbOrg.OrgName,
                                            IsClass = p.tbOrg.IsClass,
                                            ClassId = p.tbOrg.tbClass == null ? 0 : p.tbOrg.tbClass.Id,
                                            ClassName = p.tbOrg.tbClass == null ? "" : p.tbOrg.tbClass.ClassName,
                                            CourseNo = p.tbOrg.tbCourse.No,
                                            CourseId = p.tbOrg.tbCourse.Id,
                                            CourseName = p.tbOrg.tbCourse.CourseName,
                                            TeacherId = p.tbTeacher.Id,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).Distinct().ToList();

                #region 学生信息
                var tbClassIds = vm.SurveyTeacherList.Where(d => d.IsClass && d.ClassId != 0).Select(d => d.ClassId).Distinct().ToList();
                var tbOrgIds = vm.SurveyTeacherList.Select(d => d.OrgId).Distinct().ToList();

                var tbClassStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                          where tbClassIds.Contains(p.tbClass.Id)
                                          && p.tbClass.IsDeleted == false
                                          && p.tbStudent.IsDeleted == false
                                          select new
                                          {
                                              classId = p.tbClass.Id,
                                              studentId = p.tbStudent.Id
                                          }).ToList();

                var tbOrgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                        where tbOrgIds.Contains(p.tbOrg.Id)
                                        && p.tbOrg.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        select new
                                        {
                                            orgId = p.tbOrg.Id,
                                            studentId = p.tbStudent.Id
                                        }).ToList();

                var allStudentList = new List<Dto.SurveyReport.UnSubjectList>();
                foreach (var a in vm.SurveyTeacherList.GroupBy(d => new { d.OrgId, d.IsClass, d.ClassId }).Select(g => new { g.Key.OrgId, g.Key.IsClass, g.Key.ClassId }))
                {
                    if (a.IsClass)
                    {
                        if (a.ClassId > decimal.Zero)
                        {
                            var studengClass = (from p in tbClassStudentList
                                                where p.classId == a.ClassId
                                                select new Dto.SurveyReport.UnSubjectList
                                                {
                                                    OrgId = a.OrgId,
                                                    StudentId = p.studentId
                                                }).ToList();

                            allStudentList = allStudentList.Union(studengClass).ToList();
                        }
                    }
                    else
                    {
                        var studengClass = (from p in tbOrgStudentList
                                            where p.orgId == a.OrgId
                                            select new Dto.SurveyReport.UnSubjectList
                                            {
                                                OrgId = p.orgId,
                                                StudentId = p.studentId
                                            }).ToList();

                        allStudentList = allStudentList.Union(studengClass).ToList();
                    }
                }
                #endregion

                var studentGroup = (from p in allStudentList
                                    group p by new { p.OrgId } into g
                                    select new
                                    {
                                        orgId = g.Key.OrgId,
                                        count = g.Count()
                                    }).ToList();

                var teacherIds = vm.SurveyTeacherList.Select(d => d.TeacherId).Distinct().ToList();

                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where surveyCourseIds.Contains(p.tbCourse.Id)
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        select p.tbSurveyGroup.Id).Distinct().ToList();
                #region 单选分数
                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && tbOrgIds.Contains(p.tbOrg.Id)
                                        && teacherIds.Contains(p.tbTeacher.Id)
                                        select p;

                if (vm.SurveyCourseId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => tbSurveyGroupIds.Contains(d.tbSurveyItem.tbSurveyGroup.Id));
                }

                if (vm.SurveyTeacherId > decimal.Zero)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => d.tbTeacher.Id == vm.SurveyTeacherId);
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyUnSubjectList = (from p in tbSurveryDataList
                                          group p by new
                                          {
                                              orgId = p.tbOrg.Id,
                                              orgName = p.tbOrg.OrgName,
                                              courseId = p.tbOrg.tbCourse.Id,
                                              courseName = p.tbOrg.tbCourse.CourseName,
                                              teacherId = p.tbTeacher.Id,
                                              teacherName = p.tbTeacher.TeacherName,
                                              studentId = p.tbStudent.Id,
                                              studentName = p.tbStudent.StudentName,
                                          } into g
                                          select new Dto.SurveyReport.UnSubjectList
                                          {
                                              OrgId = g.Key.orgId,
                                              OrgName = g.Key.orgName,
                                              CourseId = g.Key.courseId,
                                              TeacherId = g.Key.teacherId,
                                              TeacherName = g.Key.teacherName,
                                              StudentId = g.Key.studentId,
                                              StudentName = g.Key.studentName
                                          }).ToList();
                #endregion

                var studentAllList = vm.SurveyUnSubjectList.Select(d => new { d.OrgId, d.CourseId, d.TeacherId, d.StudentId }).Distinct().ToList();

                var surveyGroupList = (from p in studentAllList
                                       group p by new { p.OrgId, p.CourseId, p.TeacherId } into g
                                       select new Dto.SurveyReport.UnSubjectList
                                       {
                                           OrgId = g.Key.OrgId,
                                           CourseId = g.Key.CourseId,
                                           TeacherId = g.Key.TeacherId,
                                           SurveyCount = g.Count()
                                       }).Distinct().ToList();

                vm.SurveyUnSubjectEachList = vm.SurveyTeacherList;

                foreach (var a in vm.SurveyUnSubjectEachList)
                {
                    var surveyCount = surveyGroupList.Where(d => d.OrgId == a.OrgId && d.CourseId == a.CourseId && d.TeacherId == a.TeacherId).FirstOrDefault();
                    if (surveyCount == null)
                    {

                    }
                    else
                    {
                        //已选
                        a.SurveyCount = surveyCount.SurveyCount;
                    }

                    var studentallCount = studentGroup.Where(d => d.orgId == a.OrgId).FirstOrDefault();
                    if (studentallCount == null)
                    {

                    }
                    else
                    {
                        //未选
                        a.SurveyAllCount = studentallCount.count;
                        a.UnSurveyCount = a.SurveyAllCount - a.SurveyCount;
                    }
                }
                #endregion

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("序号"),
                        new System.Data.DataColumn("课程"),
                        new System.Data.DataColumn("教师"),
                        new System.Data.DataColumn("教学班"),
                        new System.Data.DataColumn("已评人数"),
                        new System.Data.DataColumn("未评人数"),
                        new System.Data.DataColumn("参评总人数")
                    });

                var index = 1;
                foreach (var a in vm.SurveyUnSubjectEachList)
                {
                    var dr = dt.NewRow();
                    dr["序号"] = index.ToString();
                    dr["课程"] = a.CourseName;
                    dr["教师"] = a.TeacherName;
                    dr["教学班"] = a.OrgName;
                    dr["已评人数"] = a.SurveyCount.ToString();
                    dr["未评人数"] = a.UnSurveyCount.ToString();
                    dr["参评总人数"] = a.SurveyAllCount.ToString();
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

        public ActionResult UnSubjectFullList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyReport.UnSubjectFullList();

                var tbSurveyGroupIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                        where p.tbCourse.Id == vm.SurveyCourseId
                                        && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        select p.tbSurveyGroup.Id).ToList();

                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                                 .Include(d => d.tbClass)
                             where p.Id == vm.SurveyOrgId
                             && p.tbCourse.Id == vm.SurveyCourseId
                             select p).FirstOrDefault();

                if (tbOrg == null)
                {
                    return View(vm);
                }
                else
                {
                    var allStudentList = new List<Dto.SurveyReport.UnSubjectFullList>();
                    if (tbOrg.IsClass)
                    {
                        var studengClass = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                            where p.tbClass.Id == tbOrg.tbClass.Id
                                            && p.tbClass.IsDeleted == false
                                            && p.tbStudent.IsDeleted == false
                                            select new Dto.SurveyReport.UnSubjectFullList
                                            {
                                                OrgId = p.tbClass.Id,
                                                StudentId = p.tbStudent.Id
                                            }).ToList();

                        allStudentList = allStudentList.Union(studengClass).ToList();
                    }
                    else
                    {
                        var studengClass = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                            where p.tbOrg.Id == tbOrg.Id
                                            && p.tbOrg.IsDeleted == false
                                            && p.tbStudent.IsDeleted == false
                                            select new Dto.SurveyReport.UnSubjectFullList
                                            {
                                                OrgId = p.tbOrg.Id,
                                                StudentId = p.tbStudent.Id
                                            }).ToList();

                        allStudentList = allStudentList.Union(studengClass).ToList();
                    }

                    var studentIds = allStudentList.Select(d => d.StudentId).Distinct().ToList();//所有学生
                    if (vm.OpenFlag == decimal.Zero)
                    {
                        //已评价
                        vm.SurveyUnSubjectFullList = (from p in db.Table<Entity.tbSurveyData>()
                                                      where p.tbTeacher.Id == vm.SurveyTeacherId
                                                      && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                      && p.tbOrg.Id == vm.SurveyOrgId
                                                      && p.tbClass == null
                                                      && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                                      && studentIds.Contains(p.tbStudent.Id)
                                                      && p.tbStudent.IsDeleted == false
                                                      && p.tbOrg.IsDeleted == false
                                                      && p.tbSurveyItem.IsDeleted == false
                                                      && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                                      && p.tbSurveyItem.tbSurveyGroup.tbSurvey.IsDeleted == false
                                                      orderby p.tbStudent.StudentCode, p.tbStudent.StudentName
                                                      select new Dto.SurveyReport.UnSubjectFullList
                                                      {
                                                          StudentCode = p.tbStudent.StudentCode,
                                                          StudentName = p.tbStudent.StudentName
                                                      }).Distinct().ToList();
                    }
                    else
                    {
                        //未评价
                        var surveyStudentIds = (from p in db.Table<Entity.tbSurveyData>()
                                                where p.tbTeacher.Id == vm.SurveyTeacherId
                                                && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                && p.tbOrg.Id == vm.SurveyOrgId
                                                && p.tbClass == null
                                                && tbSurveyGroupIds.Contains(p.tbSurveyItem.tbSurveyGroup.Id)
                                                && studentIds.Contains(p.tbStudent.Id)
                                                && p.tbStudent.IsDeleted == false
                                                && p.tbOrg.IsDeleted == false
                                                && p.tbSurveyItem.IsDeleted == false
                                                && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                                && p.tbSurveyItem.tbSurveyGroup.tbSurvey.IsDeleted == false
                                                select p.tbStudent.Id).Distinct().ToList();

                        var unStudentIds = (from p in studentIds
                                            where surveyStudentIds.Contains(p) == false
                                            select p).Distinct().ToList();

                        vm.SurveyUnSubjectFullList = (from p in db.Table<Student.Entity.tbStudent>()
                                                      where unStudentIds.Contains(p.Id)
                                                      select new Dto.SurveyReport.UnSubjectFullList
                                                      {
                                                          StudentCode = p.StudentCode,
                                                          StudentName = p.StudentName
                                                      }).Distinct().ToList();
                    }
                }
                return View(vm);
            }
        }

        public ActionResult UnClassTeacherList()
        {
            var vm = new Models.SurveyReport.UnClassTeacherList();
            using (var db = new XkSystem.Models.DbContext())
            {
                //评价列表
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                var tbGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                   where p.tbSurvey.Id == vm.SurveyId
                                   && p.tbClass.tbGrade.IsDeleted == false
                                   && p.tbClass.IsDeleted == false
                                   select new
                                   {
                                       gradeId = p.tbClass.tbGrade.Id,
                                       gradeName = p.tbClass.tbGrade.GradeName,
                                       gradeNo = p.tbClass.tbGrade.No,
                                   }).Distinct().ToList();

                vm.SurveyGradeList = (from p in tbGradeList
                                      orderby p.gradeNo
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.gradeName,
                                          Value = p.gradeId.ToString()
                                      }).Distinct().ToList();

                //年级条件
                var surveyGradeIds = new List<int>();
                if (vm.SurveyGradeId == 0)
                {
                    surveyGradeIds = vm.SurveyGradeList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGradeIds.Add((int)vm.SurveyGradeId);
                }

                vm.SurveyClassTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                             join m in db.Table<Entity.tbSurveyClass>()
                                             on p.tbClass.Id equals m.tbClass.Id
                                             where p.tbClass.IsDeleted == false
                                             && surveyGradeIds.Contains(m.tbClass.tbGrade.Id)
                                             orderby p.tbClass.No, p.tbClass.ClassName, p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                             select new Dto.SurveyReport.UnClassTeacherList
                                             {
                                                 GradeNo = p.tbClass.tbGrade.No,
                                                 GradeId = p.tbClass.tbGrade.Id,
                                                 GradeName = p.tbClass.tbGrade.GradeName,
                                                 ClassNo = p.tbClass.No,
                                                 ClassId = p.tbClass.Id,
                                                 ClassName = p.tbClass.ClassName,
                                                 TeacherId = p.tbTeacher.Id,
                                                 TeacherName = p.tbTeacher.TeacherName
                                             }).Distinct().ToList();

                var tbClassIds = vm.SurveyClassTeacherList.Select(d => d.ClassId).Distinct().ToList();

                var tbClassStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                          where tbClassIds.Contains(p.tbClass.Id)
                                          && p.tbClass.IsDeleted == false
                                          && p.tbStudent.IsDeleted == false
                                          select new
                                          {
                                              classId = p.tbClass.Id,
                                              studentId = p.tbStudent.Id
                                          }).Distinct().ToList();

                var studentGroup = (from p in tbClassStudentList
                                    group p by new { p.classId } into g
                                    select new
                                    {
                                        classId = g.Key.classId,
                                        count = g.Count()
                                    }).ToList();

                var teacherIds = vm.SurveyClassTeacherList.Select(d => d.TeacherId).Distinct().ToList();

                #region 选择结果
                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbClass != null
                                        && tbClassIds.Contains(p.tbClass.Id)
                                        && teacherIds.Contains(p.tbTeacher.Id)
                                        && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                        select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyUnClassTeacherList = (from p in tbSurveryDataList
                                               group p by new
                                               {
                                                   classId = p.tbClass.Id,
                                                   teacherId = p.tbTeacher.Id,
                                                   studentId = p.tbStudent.Id
                                               } into g
                                               select new Dto.SurveyReport.UnClassTeacherList
                                               {
                                                   ClassId = g.Key.classId,
                                                   TeacherId = g.Key.teacherId,
                                                   StudentId = g.Key.studentId
                                               }).Distinct().ToList();
                #endregion

                var surveyGroupList = (from p in vm.SurveyUnClassTeacherList
                                       group p by new { p.ClassId, p.TeacherId } into g
                                       select new Dto.SurveyReport.UnSubjectList
                                       {
                                           ClassId = g.Key.ClassId,
                                           TeacherId = g.Key.TeacherId,
                                           SurveyCount = g.Count()
                                       }).Distinct().ToList();

                vm.SurveyUnClassTeacherEachList = vm.SurveyClassTeacherList;

                foreach (var a in vm.SurveyUnClassTeacherEachList)
                {
                    var surveyCount = surveyGroupList.Where(d => d.ClassId == a.ClassId && d.TeacherId == a.TeacherId).FirstOrDefault();
                    if (surveyCount == null)
                    {

                    }
                    else
                    {
                        //已选
                        a.SurveyCount = surveyCount.SurveyCount;
                    }

                    var studentallCount = studentGroup.Where(d => d.classId == a.ClassId).FirstOrDefault();
                    if (studentallCount == null)
                    {

                    }
                    else
                    {
                        //未选
                        a.SurveyAllCount = studentallCount.count;
                        a.UnSurveyCount = a.SurveyAllCount - a.SurveyCount;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnClassTeacherList(Models.SurveyReport.UnClassTeacherList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("UnClassTeacherList", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveyGradeId = vm.SurveyGradeId
            }));
        }

        public ActionResult UnClassTeacherListExport()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                #region 数据准备
                var vm = new Models.SurveyReport.UnClassTeacherList();
                //评价列表
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                var tbGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                   where p.tbSurvey.Id == vm.SurveyId
                                   && p.tbClass.tbGrade.IsDeleted == false
                                   && p.tbClass.IsDeleted == false
                                   select new
                                   {
                                       gradeId = p.tbClass.tbGrade.Id,
                                       gradeName = p.tbClass.tbGrade.GradeName,
                                       gradeNo = p.tbClass.tbGrade.No,
                                   }).Distinct().ToList();

                vm.SurveyGradeList = (from p in tbGradeList
                                      orderby p.gradeNo
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.gradeName,
                                          Value = p.gradeId.ToString()
                                      }).Distinct().ToList();

                //年级条件
                var surveyGradeIds = new List<int>();
                if (vm.SurveyGradeId == 0)
                {
                    surveyGradeIds = vm.SurveyGradeList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGradeIds.Add((int)vm.SurveyGradeId);
                }

                vm.SurveyClassTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                             join m in db.Table<Entity.tbSurveyClass>()
                                             on p.tbClass.Id equals m.tbClass.Id
                                             where p.tbClass.IsDeleted == false
                                             && surveyGradeIds.Contains(m.tbClass.tbGrade.Id)
                                             orderby p.tbClass.No, p.tbClass.ClassName, p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                             select new Dto.SurveyReport.UnClassTeacherList
                                             {
                                                 GradeNo = p.tbClass.tbGrade.No,
                                                 GradeId = p.tbClass.tbGrade.Id,
                                                 GradeName = p.tbClass.tbGrade.GradeName,
                                                 ClassNo = p.tbClass.No,
                                                 ClassId = p.tbClass.Id,
                                                 ClassName = p.tbClass.ClassName,
                                                 TeacherId = p.tbTeacher.Id,
                                                 TeacherName = p.tbTeacher.TeacherName
                                             }).Distinct().ToList();

                var tbClassIds = vm.SurveyClassTeacherList.Select(d => d.ClassId).Distinct().ToList();

                var tbClassStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                          where tbClassIds.Contains(p.tbClass.Id)
                                          && p.tbClass.IsDeleted == false
                                          && p.tbStudent.IsDeleted == false
                                          select new
                                          {
                                              classId = p.tbClass.Id,
                                              studentId = p.tbStudent.Id
                                          }).Distinct().ToList();

                var studentGroup = (from p in tbClassStudentList
                                    group p by new { p.classId } into g
                                    select new
                                    {
                                        classId = g.Key.classId,
                                        count = g.Count()
                                    }).ToList();

                var teacherIds = vm.SurveyClassTeacherList.Select(d => d.TeacherId).Distinct().ToList();

                #region 选择结果
                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbClass != null
                                        && tbClassIds.Contains(p.tbClass.Id)
                                        && teacherIds.Contains(p.tbTeacher.Id)
                                        && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                        select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyUnClassTeacherList = (from p in tbSurveryDataList
                                               group p by new
                                               {
                                                   classId = p.tbClass.Id,
                                                   teacherId = p.tbTeacher.Id,
                                                   studentId = p.tbStudent.Id
                                               } into g
                                               select new Dto.SurveyReport.UnClassTeacherList
                                               {
                                                   ClassId = g.Key.classId,
                                                   TeacherId = g.Key.teacherId,
                                                   StudentId = g.Key.studentId
                                               }).Distinct().ToList();
                #endregion

                var surveyGroupList = (from p in vm.SurveyUnClassTeacherList
                                       group p by new { p.ClassId, p.TeacherId } into g
                                       select new Dto.SurveyReport.UnSubjectList
                                       {
                                           ClassId = g.Key.ClassId,
                                           TeacherId = g.Key.TeacherId,
                                           SurveyCount = g.Count()
                                       }).Distinct().ToList();

                vm.SurveyUnClassTeacherEachList = vm.SurveyClassTeacherList;

                foreach (var a in vm.SurveyUnClassTeacherEachList)
                {
                    var surveyCount = surveyGroupList.Where(d => d.ClassId == a.ClassId && d.TeacherId == a.TeacherId).FirstOrDefault();
                    if (surveyCount == null)
                    {

                    }
                    else
                    {
                        //已选
                        a.SurveyCount = surveyCount.SurveyCount;
                    }

                    var studentallCount = studentGroup.Where(d => d.classId == a.ClassId).FirstOrDefault();
                    if (studentallCount == null)
                    {

                    }
                    else
                    {
                        //未选
                        a.SurveyAllCount = studentallCount.count;
                        a.UnSurveyCount = a.SurveyAllCount - a.SurveyCount;
                    }
                }
                #endregion

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("序号"),
                        new System.Data.DataColumn("年级"),
                        new System.Data.DataColumn("班主任"),
                        new System.Data.DataColumn("行政班"),
                        new System.Data.DataColumn("已评人数"),
                        new System.Data.DataColumn("未评人数"),
                        new System.Data.DataColumn("参评总人数")
                    });

                var index = 1;
                foreach (var a in vm.SurveyUnClassTeacherEachList)
                {
                    var dr = dt.NewRow();
                    dr["序号"] = index.ToString();
                    dr["年级"] = a.GradeName;
                    dr["班主任"] = a.TeacherName;
                    dr["行政班"] = a.ClassName;
                    dr["已评人数"] = a.SurveyCount.ToString();
                    dr["未评人数"] = a.UnSurveyCount.ToString();
                    dr["参评总人数"] = a.SurveyAllCount.ToString();
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

        public ActionResult UnClassTeacherFullList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyReport.UnClassTeacherFullList();

                var studentClass = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbClass.Id == vm.SurveyClassId
                                    && p.tbClass.IsDeleted == false
                                    && p.tbStudent.IsDeleted == false
                                    select new Dto.SurveyReport.UnClassTeacherFullList
                                    {
                                        ClassId = p.tbClass.Id,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                var studentIds = studentClass.Select(d => d.StudentId).Distinct().ToList();//所有学生
                if (vm.OpenFlag == decimal.Zero)
                {
                    //已评价
                    vm.SurveyUnClassTeacherFullList = (from p in db.Table<Entity.tbSurveyData>()
                                                       where p.tbTeacher.Id == vm.SurveyTeacherId
                                                       && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                       && p.tbClass != null
                                                       && p.tbClass.Id == vm.SurveyClassId
                                                       && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                                       && studentIds.Contains(p.tbStudent.Id)
                                                       && p.tbStudent.IsDeleted == false
                                                       && p.tbClass.IsDeleted == false
                                                       && p.tbSurveyItem.IsDeleted == false
                                                       && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                                       && p.tbSurveyItem.tbSurveyGroup.tbSurvey.IsDeleted == false
                                                       orderby p.tbStudent.StudentCode, p.tbStudent.StudentName
                                                       select new Dto.SurveyReport.UnClassTeacherFullList
                                                       {
                                                           StudentCode = p.tbStudent.StudentCode,
                                                           StudentName = p.tbStudent.StudentName
                                                       }).Distinct().ToList();
                }
                else
                {
                    //未评价
                    var surveyStudentIds = (from p in db.Table<Entity.tbSurveyData>()
                                            where p.tbTeacher.Id == vm.SurveyTeacherId
                                            && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                            && p.tbClass != null
                                            && p.tbClass.Id == vm.SurveyClassId
                                            && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                            && studentIds.Contains(p.tbStudent.Id)
                                            && p.tbStudent.IsDeleted == false
                                            && p.tbClass.IsDeleted == false
                                            && p.tbSurveyItem.IsDeleted == false
                                            && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                            && p.tbSurveyItem.tbSurveyGroup.tbSurvey.IsDeleted == false
                                            select p.tbStudent.Id).Distinct().ToList();

                    var unStudentIds = (from p in studentIds
                                        where surveyStudentIds.Contains(p) == false
                                        select p).Distinct().ToList();

                    vm.SurveyUnClassTeacherFullList = (from p in db.Table<Student.Entity.tbStudent>()
                                                       where unStudentIds.Contains(p.Id)
                                                       select new Dto.SurveyReport.UnClassTeacherFullList
                                                       {
                                                           StudentCode = p.StudentCode,
                                                           StudentName = p.StudentName
                                                       }).Distinct().ToList();
                }
                return View(vm);
            }
        }

        public ActionResult ClassTeacherList()
        {
            var vm = new Models.SurveyReport.ClassTeacherList();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                var tbGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                   where p.tbSurvey.Id == vm.SurveyId
                                   && p.tbClass.tbGrade.IsDeleted == false
                                   && p.tbClass.IsDeleted == false
                                   select new
                                   {
                                       gradeId = p.tbClass.tbGrade.Id,
                                       gradeName = p.tbClass.tbGrade.GradeName,
                                       gradeNo = p.tbClass.tbGrade.No,
                                   }).Distinct().ToList();

                vm.SurveyGradeList = (from p in tbGradeList
                                      orderby p.gradeNo
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.gradeName,
                                          Value = p.gradeId.ToString()
                                      }).Distinct().ToList();

                //年级条件
                var surveyGradeIds = new List<int>();
                if (vm.SurveyGradeId == 0)
                {
                    surveyGradeIds = vm.SurveyGradeList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGradeIds.Add((int)vm.SurveyGradeId);
                }

                vm.SurveyTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                        join m in db.Table<Entity.tbSurveyClass>()
                                        on p.tbClass.Id equals m.tbClass.Id
                                        where p.tbClass.IsDeleted == false
                                        && surveyGradeIds.Contains(m.tbClass.tbGrade.Id)
                                        && m.tbSurvey.Id == vm.SurveyId
                                        orderby p.tbClass.No, p.tbClass.ClassName, p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new Dto.SurveyReport.ClassTeacherList
                                        {
                                            GradeNo = p.tbClass.tbGrade.No,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            ClassNo = p.tbClass.No,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            TeacherId = p.tbTeacher.Id,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).Distinct().ToList();

                var classIds = vm.SurveyTeacherList.Select(d => d.ClassId).Distinct().ToList();
                var teacherIds = vm.SurveyTeacherList.Select(d => d.TeacherId).Distinct().ToList();

                #region 单选分数
                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbClass != null
                                        && classIds.Contains(p.tbClass.Id)
                                        && teacherIds.Contains(p.tbTeacher.Id)
                                        && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbSurveyItem.IsDeleted == false
                                        && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyClassTeacherList = (from p in tbSurveryDataList
                                             group p by new
                                             {
                                                 classId = p.tbClass.Id,
                                                 className = p.tbClass.ClassName,
                                                 teacherId = p.tbTeacher.Id,
                                                 teacherName = p.tbTeacher.TeacherName,
                                                 surveryItemId = p.tbSurveyItem.Id,
                                                 surveryItemName = p.tbSurveyItem.SurveyItemName,
                                                 surverOptionId = p.tbSurveyOption.Id,
                                                 surverOptionName = p.tbSurveyOption.OptionName
                                             } into g
                                             select new Dto.SurveyReport.ClassTeacherList
                                             {
                                                 ClassId = g.Key.classId,
                                                 ClassName = g.Key.className,
                                                 SurveyItemId = g.Key.surveryItemId,
                                                 SurveyItemName = g.Key.surveryItemName,
                                                 SurveyOptionId = g.Key.surverOptionId,
                                                 SurveyOptionName = g.Key.surverOptionName,
                                                 TeacherId = g.Key.teacherId,
                                                 TeacherName = g.Key.teacherName,
                                                 SurveyOptionCount = g.Count(),
                                                 SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                                 SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                             }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataTextList = from p in db.Table<Entity.tbSurveyData>()
                                            where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                            && p.tbClass != null
                                            && classIds.Contains(p.tbClass.Id)
                                            && teacherIds.Contains(p.tbTeacher.Id)
                                            && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                            && p.tbClass.IsDeleted == false
                                            && p.tbStudent.IsDeleted == false
                                            && p.tbSurveyItem.IsDeleted == false
                                            && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                            select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataTextList = tbSurveryDataTextList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyClassTeacherTextList = (from p in tbSurveryDataTextList
                                                 group p by new
                                                 {
                                                     classId = p.tbClass.Id,
                                                     className = p.tbClass.ClassName,
                                                     teacherId = p.tbTeacher.Id,
                                                     teacherName = p.tbTeacher.TeacherName,
                                                     surveryItemId = p.tbSurveyItem.Id,
                                                     surveryItemName = p.tbSurveyItem.SurveyItemName
                                                 } into g
                                                 select new Dto.SurveyReport.ClassTeacherList
                                                 {
                                                     ClassId = g.Key.classId,
                                                     ClassName = g.Key.className,
                                                     SurveyItemId = g.Key.surveryItemId,
                                                     SurveyItemName = g.Key.surveryItemName,
                                                     TeacherId = g.Key.teacherId,
                                                     TeacherName = g.Key.teacherName,
                                                     SurveyOptionCount = g.Count()
                                                 }).ToList();
                #endregion

                vm.SurveyItemInfoList = (from p in db.Table<Entity.tbSurveyItem>()
                                         where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                         && p.tbSurveyGroup.IsOrg == false
                                         orderby p.tbSurveyGroup.No, p.No
                                         select new Dto.SurveyItem.Info
                                         {
                                             Id = p.Id,
                                             SurveyItemName = p.SurveyItemName,
                                             SurveyItemType = p.SurveyItemType,
                                             No = p.No
                                         }).Distinct().ToList();

                vm.SurveyOptionInfoList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                           && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                           orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               OptionValue = p.OptionValue,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).Distinct().ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassTeacherList(Models.SurveyReport.ClassTeacherList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassTeacherList", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveyGradeId = vm.SurveyGradeId
            }));
        }

        public ActionResult ClassTeacherListExport()
        {
            var vm = new Models.SurveyReport.ClassTeacherList();
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 准备数据
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                var tbGradeList = (from p in db.Table<Entity.tbSurveyClass>()
                                   where p.tbSurvey.Id == vm.SurveyId
                                   && p.tbClass.tbGrade.IsDeleted == false
                                   && p.tbClass.IsDeleted == false
                                   select new
                                   {
                                       gradeId = p.tbClass.tbGrade.Id,
                                       gradeName = p.tbClass.tbGrade.GradeName,
                                       gradeNo = p.tbClass.tbGrade.No,
                                   }).Distinct().ToList();

                vm.SurveyGradeList = (from p in tbGradeList
                                      orderby p.gradeNo
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.gradeName,
                                          Value = p.gradeId.ToString()
                                      }).Distinct().ToList();

                //年级条件
                var surveyGradeIds = new List<int>();
                if (vm.SurveyGradeId == 0)
                {
                    surveyGradeIds = vm.SurveyGradeList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyGradeIds.Add((int)vm.SurveyGradeId);
                }

                vm.SurveyTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                        join m in db.Table<Entity.tbSurveyClass>()
                                        on p.tbClass.Id equals m.tbClass.Id
                                        where p.tbClass.IsDeleted == false
                                        && m.tbSurvey.Id == vm.SurveyId
                                        && surveyGradeIds.Contains(m.tbClass.tbGrade.Id)
                                        orderby p.tbClass.No, p.tbClass.ClassName, p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new Dto.SurveyReport.ClassTeacherList
                                        {
                                            GradeNo = p.tbClass.tbGrade.No,
                                            GradeId = p.tbClass.tbGrade.Id,
                                            GradeName = p.tbClass.tbGrade.GradeName,
                                            ClassNo = p.tbClass.No,
                                            ClassId = p.tbClass.Id,
                                            ClassName = p.tbClass.ClassName,
                                            TeacherId = p.tbTeacher.Id,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).Distinct().ToList();

                var classIds = vm.SurveyTeacherList.Select(d => d.ClassId).Distinct().ToList();
                var teacherIds = vm.SurveyTeacherList.Select(d => d.TeacherId).Distinct().ToList();

                #region 单选分数
                var tbSurveryDataList = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                        && p.tbClass != null
                                        && classIds.Contains(p.tbClass.Id)
                                        && teacherIds.Contains(p.tbTeacher.Id)
                                        && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbSurveyItem.IsDeleted == false
                                        && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                        select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataList = tbSurveryDataList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyClassTeacherList = (from p in tbSurveryDataList
                                             group p by new
                                             {
                                                 classId = p.tbClass.Id,
                                                 className = p.tbClass.ClassName,
                                                 teacherId = p.tbTeacher.Id,
                                                 teacherName = p.tbTeacher.TeacherName,
                                                 surveryItemId = p.tbSurveyItem.Id,
                                                 surveryItemName = p.tbSurveyItem.SurveyItemName,
                                                 surverOptionId = p.tbSurveyOption.Id,
                                                 surverOptionName = p.tbSurveyOption.OptionName
                                             } into g
                                             select new Dto.SurveyReport.ClassTeacherList
                                             {
                                                 ClassId = g.Key.classId,
                                                 ClassName = g.Key.className,
                                                 SurveyItemId = g.Key.surveryItemId,
                                                 SurveyItemName = g.Key.surveryItemName,
                                                 SurveyOptionId = g.Key.surverOptionId,
                                                 SurveyOptionName = g.Key.surverOptionName,
                                                 TeacherId = g.Key.teacherId,
                                                 TeacherName = g.Key.teacherName,
                                                 SurveyOptionCount = g.Count(),
                                                 SurveyOptionSum = g.Select(d => d.tbSurveyOption.OptionValue).Sum(),
                                                 SurveyOptionAvg = g.Select(d => d.tbSurveyOption.OptionValue).Average()
                                             }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataTextList = from p in db.Table<Entity.tbSurveyData>()
                                            where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                            && p.tbClass != null
                                            && classIds.Contains(p.tbClass.Id)
                                            && teacherIds.Contains(p.tbTeacher.Id)
                                            && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                            && p.tbClass.IsDeleted == false
                                            && p.tbStudent.IsDeleted == false
                                            && p.tbSurveyItem.IsDeleted == false
                                            && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                            select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataTextList = tbSurveryDataTextList.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                vm.SurveyClassTeacherTextList = (from p in tbSurveryDataTextList
                                                 group p by new
                                                 {
                                                     classId = p.tbClass.Id,
                                                     className = p.tbClass.ClassName,
                                                     teacherId = p.tbTeacher.Id,
                                                     teacherName = p.tbTeacher.TeacherName,
                                                     surveryItemId = p.tbSurveyItem.Id,
                                                     surveryItemName = p.tbSurveyItem.SurveyItemName
                                                 } into g
                                                 select new Dto.SurveyReport.ClassTeacherList
                                                 {
                                                     ClassId = g.Key.classId,
                                                     ClassName = g.Key.className,
                                                     SurveyItemId = g.Key.surveryItemId,
                                                     SurveyItemName = g.Key.surveryItemName,
                                                     TeacherId = g.Key.teacherId,
                                                     TeacherName = g.Key.teacherName,
                                                     SurveyOptionCount = g.Count()
                                                 }).ToList();
                #endregion

                vm.SurveyItemInfoList = (from p in db.Table<Entity.tbSurveyItem>()
                                         where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                         && p.tbSurveyGroup.IsOrg == false
                                         orderby p.tbSurveyGroup.No, p.No
                                         select new Dto.SurveyItem.Info
                                         {
                                             Id = p.Id,
                                             SurveyItemName = p.SurveyItemName,
                                             SurveyItemType = p.SurveyItemType,
                                             No = p.No
                                         }).Distinct().ToList();

                vm.SurveyOptionInfoList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                           && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                           orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               OptionValue = p.OptionValue,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).Distinct().ToList();
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();
                var surveyOptionCount = 0;
                var itemList = vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                foreach (var item in itemList)
                {
                    var optionList = vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList();
                    foreach (var option in optionList)
                    {
                        surveyOptionCount++;
                    }
                }
                var itemTextList = vm.SurveyItemInfoList.Where(d => d.SurveyItemType == XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                foreach (var item in itemTextList)
                {
                    surveyOptionCount++;
                }
                var allColumnLength = surveyOptionCount + 5;
                var arrColumns = new string[allColumnLength];

                for (int i = 0; i < arrColumns.Length; i++)
                {
                    arrColumns[i] = (i + 1).ToString();
                }

                var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                sheet.isColumnWritten = false;
                sheet.isWriteHeader = true;
                sheet.strHeaderText = "班主任满意统计";
                //开始表格
                var dt = Code.Common.ArrayToDataTable(arrColumns);
                var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                var arrSurveyItemID = new string[allColumnLength];
                var arrSurveyItemName = new string[allColumnLength];
                var arrSurveyOptionID = new string[allColumnLength];
                var arrSurveyOptionName = new string[allColumnLength];
                var index = 2;
                for (int i = 0; i < itemList.Count(); i++)
                {
                    var optionList = vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == itemList[i].Id).Distinct().ToList();
                    for (var j = 0; j < optionList.Count(); j++)
                    {
                        index++;
                        arrSurveyItemID[index] = itemList[i].Id.ToString();
                        arrSurveyItemName[index] = itemList[i].SurveyItemName.ToString();
                        arrSurveyOptionID[index] = optionList[j].Id.ToString();
                        arrSurveyOptionName[index] = optionList[j].OptionName.ToString();
                    }
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, index - optionList.Count() + 1, index));
                }
                arrSurveyItemName[index + 1] = "评价次数";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index + 1, index + 1));
                arrSurveyItemName[index + 2] = "满意度比例";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index + 2, index + 2));
                index = index + 2;
                for (int i = 0; i < itemTextList.Count(); i++)
                {
                    index++;
                    arrSurveyItemID[index] = itemTextList[i].Id.ToString();
                    arrSurveyItemName[index] = itemTextList[i].SurveyItemName.ToString();
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, index, index));
                }
                dt.Rows.Add(arrSurveyItemName);
                dt.Rows.Add(arrSurveyOptionName);
                #region 处理DataList
                foreach (var teacher in vm.SurveyTeacherList.OrderBy(d => d.GradeNo).ThenBy(d => d.GradeName).ThenBy(d => d.TeacherName).ToList())
                {
                    string[] arrSurveyData = new string[allColumnLength];
                    arrSurveyData[0] = teacher.GradeName;
                    arrSurveyData[1] = teacher.ClassName;
                    arrSurveyData[2] = teacher.TeacherName;
                    var allCount = 0;
                    var goodCount = 0m;
                    var goodRate = 0m;
                    var indexR = 2;
                    foreach (var item in vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                    {
                        foreach (var option in vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList())
                        {
                            indexR++;
                            var optionCount = vm.SurveyClassTeacherList
                                .Where(d => d.TeacherId == teacher.TeacherId && d.ClassId == teacher.ClassId && d.SurveyItemId == item.Id && d.SurveyOptionId == option.Id)
                                .Sum(d => d.SurveyOptionCount);
                            if (option.OptionName.Contains("A：非常满意") || option.OptionName.Contains("B：满意"))
                            {
                                goodCount += optionCount;
                            }
                            allCount += optionCount;
                            if (optionCount > decimal.Zero)
                            {
                                arrSurveyData[indexR] = optionCount.ToString();
                            }
                        }
                    }
                    indexR++;
                    if (allCount > decimal.Zero)
                    {
                        arrSurveyData[indexR] = allCount.ToString();
                    }
                    if (allCount > decimal.Zero)
                    {
                        goodRate = Decimal.Round(goodCount / allCount * 100, 2);
                    }
                    indexR++;
                    if (goodRate > decimal.Zero)
                    {
                        arrSurveyData[indexR] = goodRate.ToString() + "%";
                    }
                    foreach (var item in vm.SurveyItemInfoList.Where(d => d.SurveyItemType == XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                    {
                        indexR++;
                        var optionTextCount = vm.SurveyClassTeacherTextList
                            .Where(d => d.TeacherId == teacher.TeacherId && d.ClassId == teacher.ClassId && d.SurveyItemId == item.Id)
                            .Sum(d => d.SurveyOptionCount);
                        if (optionTextCount > 0)
                        {
                            arrSurveyData[indexR] = optionTextCount.ToString();
                        }
                    }
                    dt.Rows.Add(arrSurveyData);
                }
                #endregion
                dt.Rows[0][0] = "课程";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));
                dt.Rows[0][1] = "行政班";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 1, 1));
                dt.Rows[0][2] = "班主任";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 2, 2));
                sheet.data = dt;
                sheet.regions = regions;
                sheetList.Add(sheet);
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

        public ActionResult ClassTeacherTextList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyReport.ClassTeacherTextList();

                vm.SurveyClassTeacherTextList = (from p in db.Table<Entity.tbSurveyData>()
                                                 where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                 && p.tbSurveyItem.Id == vm.SurveyItemId
                                                 && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                                 && p.tbClass.Id == vm.SurveyClassId
                                                 && p.tbSurveyItem.tbSurveyGroup.IsOrg == false
                                                 && p.tbTeacher.Id == vm.SurveyTeacherId
                                                 orderby p.tbStudent.StudentCode, p.tbStudent.StudentName
                                                 select new Dto.SurveyReport.ClassTeacherTextList
                                                 {
                                                     StudentCode = p.tbStudent.StudentCode,
                                                     StudentName = p.tbStudent.StudentName,
                                                     SurveyText = p.SurveyText
                                                 }).ToPageList(vm.Page);

                if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator)
                {
                    foreach (var a in vm.SurveyClassTeacherTextList)
                    {
                        a.StudentCode = "***";
                        a.StudentName = "***";
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassTeacherTextList(Models.SurveyReport.ClassTeacherTextList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassTeacherTextList", new
            {
                SurveyId = vm.SurveyId,
                SurveyClassId = vm.SurveyClassId,
                SurveyTeacherId = vm.SurveyTeacherId,
                SurveyItemId = vm.SurveyItemId,
                pageSize = vm.Page.PageSize,
                pageCount = vm.Page.PageCount,
                pageIndex = vm.Page.PageIndex
            }));
        }

        public ActionResult SubjectCourseList()
        {
            var vm = new Models.SurveyReport.SubjectCourseList();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                if (vm.SurveyGroupId == 0 && vm.SurveyGroupList.Count > 0)
                {
                    vm.SurveyGroupId = vm.SurveyGroupList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.Id == vm.SurveyGroupId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                var surveyCourseIds = new List<int>();
                if (vm.SurveyCourseId == 0)
                {
                    surveyCourseIds = vm.SurveyCourseList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyCourseIds.Add((int)vm.SurveyCourseId);
                }

                vm.SurveyTeacherList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                        join m in db.Table<Entity.tbSurveyCourse>()
                                        on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                        equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                        where p.tbOrg.IsDeleted == false
                                        orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new Dto.SurveyReport.SubjectCourseList
                                        {
                                            CourseNo = p.tbOrg.tbCourse.No,
                                            CourseId = p.tbOrg.tbCourse.Id,
                                            CourseName = p.tbOrg.tbCourse.CourseName,
                                            TeacherId = p.tbTeacher.Id,
                                            TeacherName = p.tbTeacher.TeacherName,
                                            OrgId = p.tbOrg.Id,
                                            OrgName = p.tbOrg.OrgName,
                                            ClassId = p.tbOrg.IsClass ? (p.tbOrg.tbClass == null ? 0 : p.tbOrg.tbClass.Id) : 0,
                                            ClassName = p.tbOrg.IsClass ? (p.tbOrg.tbClass == null ? "" : p.tbOrg.tbClass.ClassName) : ""
                                        }).Distinct().ToList();

                vm.SurveyTeacherList = vm.SurveyTeacherList.Where(d => surveyCourseIds.Contains(d.CourseId)).ToList();

                var teacherIds = vm.SurveyTeacherList.Select(d => d.TeacherId).Distinct().ToList();

                #region 单选分数
                var tbSurveryData = from p in db.Table<Entity.tbSurveyData>()
                                    where p.tbSurveyItem.tbSurveyGroup.Id == vm.SurveyGroupId
                                    && p.tbOrg != null
                                    && p.tbClass == null
                                    && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                    select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryData = tbSurveryData.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                var tbSurveryDataList = (from p in tbSurveryData
                                         select new
                                         {
                                             orgId = p.tbOrg.Id,
                                             orgName = p.tbOrg.OrgName,
                                             courseId = p.tbOrg.tbCourse.Id,
                                             courseName = p.tbOrg.tbCourse.CourseName,
                                             teacherId = p.tbTeacher.Id,
                                             teacherName = p.tbTeacher.TeacherName,
                                             surveryItemId = p.tbSurveyItem.Id,
                                             surveryItemName = p.tbSurveyItem.SurveyItemName,
                                             surverOptionId = p.tbSurveyOption.Id,
                                             surverOptionName = p.tbSurveyOption.OptionName,
                                             surverOptionValue = p.tbSurveyOption.OptionValue
                                         }).ToList();

                vm.SurveySubjectReportList = (from p in tbSurveryDataList
                                              where teacherIds.Contains(p.teacherId)
                                              group p by new
                                              {
                                                  orgId = p.orgId,
                                                  orgName = p.orgName,
                                                  courseId = p.courseId,
                                                  courseName = p.courseName,
                                                  teacherId = p.teacherId,
                                                  teacherName = p.teacherName,
                                                  surveryItemId = p.surveryItemId,
                                                  surveryItemName = p.surveryItemName,
                                                  surverOptionId = p.surverOptionId,
                                                  surverOptionName = p.surverOptionName
                                              } into g
                                              select new Dto.SurveyReport.SubjectCourseList
                                              {
                                                  OrgId = g.Key.orgId,
                                                  OrgName = g.Key.orgName,
                                                  CourseId = g.Key.courseId,
                                                  CourseName = g.Key.courseName,
                                                  SurveyItemId = g.Key.surveryItemId,
                                                  SurveyItemName = g.Key.surveryItemName,
                                                  SurveyOptionId = g.Key.surverOptionId,
                                                  SurveyOptionName = g.Key.surverOptionName,
                                                  TeacherId = g.Key.teacherId,
                                                  TeacherName = g.Key.teacherName,
                                                  SurveyOptionCount = g.Count(),
                                                  SurveyOptionSum = g.Select(d => d.surverOptionValue).Sum()
                                              }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataText = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.Id == vm.SurveyGroupId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                        select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataText = tbSurveryDataText.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                var tbSurveryDataTextList = (from p in tbSurveryDataText
                                             select new
                                             {
                                                 orgId = p.tbOrg.Id,
                                                 orgName = p.tbOrg.OrgName,
                                                 courseId = p.tbOrg.tbCourse.Id,
                                                 courseName = p.tbOrg.tbCourse.CourseName,
                                                 teacherId = p.tbTeacher.Id,
                                                 teacherName = p.tbTeacher.TeacherName,
                                                 surveryItemId = p.tbSurveyItem.Id,
                                                 surveryItemName = p.tbSurveyItem.SurveyItemName
                                             }).ToList();


                vm.SurveySubjectReportTextList = (from p in tbSurveryDataTextList
                                                  where teacherIds.Contains(p.teacherId)
                                                  group p by new
                                                  {
                                                      orgId = p.orgId,
                                                      orgName = p.orgName,
                                                      courseId = p.courseId,
                                                      courseName = p.courseName,
                                                      teacherId = p.teacherId,
                                                      teacherName = p.teacherName,
                                                      surveryItemId = p.surveryItemId,
                                                      surveryItemName = p.surveryItemName
                                                  } into g
                                                  select new Dto.SurveyReport.SubjectCourseList
                                                  {
                                                      OrgId = g.Key.orgId,
                                                      OrgName = g.Key.orgName,
                                                      CourseId = g.Key.courseId,
                                                      CourseName = g.Key.courseName,
                                                      SurveyItemId = g.Key.surveryItemId,
                                                      SurveyItemName = g.Key.surveryItemName,
                                                      TeacherId = g.Key.teacherId,
                                                      TeacherName = g.Key.teacherName,
                                                      SurveyOptionCount = g.Count()
                                                  }).ToList();
                #endregion

                vm.SurveyItemInfoList = (from p in db.Table<Entity.tbSurveyItem>()
                                         where p.tbSurveyGroup.Id == vm.SurveyGroupId
                                         orderby p.tbSurveyGroup.No, p.No
                                         select new Dto.SurveyItem.Info
                                         {
                                             Id = p.Id,
                                             SurveyItemName = p.SurveyItemName,
                                             SurveyItemType = p.SurveyItemType,
                                             No = p.No
                                         }).Distinct().ToList();

                vm.SurveyOptionInfoList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where p.tbSurveyItem.tbSurveyGroup.Id == vm.SurveyGroupId
                                           orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               OptionValue = p.OptionValue,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).Distinct().ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubjectCourseList(Models.SurveyReport.SubjectCourseList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SubjectCourseList", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveyGroupId = vm.SurveyGroupId,
                SurveyCourseId = vm.SurveyCourseId
            }));
        }
        public ActionResult SubjectCourseListExport()
        {
            var vm = new Models.SurveyReport.SubjectCourseList();
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 准备数据
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                if (vm.SurveyGroupId == 0 && vm.SurveyGroupList.Count > 0)
                {
                    vm.SurveyGroupId = vm.SurveyGroupList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.Id == vm.SurveyGroupId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                var surveyCourseIds = new List<int>();
                if (vm.SurveyCourseId == 0)
                {
                    surveyCourseIds = vm.SurveyCourseList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyCourseIds.Add((int)vm.SurveyCourseId);
                }

                vm.SurveyTeacherList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                        join m in db.Table<Entity.tbSurveyCourse>()
                                        on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                        equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                        where p.tbOrg.IsDeleted == false
                                        orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new Dto.SurveyReport.SubjectCourseList
                                        {
                                            CourseNo = p.tbOrg.tbCourse.No,
                                            CourseId = p.tbOrg.tbCourse.Id,
                                            CourseName = p.tbOrg.tbCourse.CourseName,
                                            TeacherId = p.tbTeacher.Id,
                                            TeacherName = p.tbTeacher.TeacherName,
                                            OrgId = p.tbOrg.Id,
                                            OrgName = p.tbOrg.OrgName,
                                            ClassId = p.tbOrg.IsClass ? (p.tbOrg.tbClass == null ? 0 : p.tbOrg.tbClass.Id) : 0,
                                            ClassName = p.tbOrg.IsClass ? (p.tbOrg.tbClass == null ? "" : p.tbOrg.tbClass.ClassName) : ""
                                        }).Distinct().ToList();

                vm.SurveyTeacherList = vm.SurveyTeacherList.Where(d => surveyCourseIds.Contains(d.CourseId)).ToList();

                var teacherIds = vm.SurveyTeacherList.Select(d => d.TeacherId).Distinct().ToList();

                #region 单选分数
                var tbSurveryData = from p in db.Table<Entity.tbSurveyData>()
                                    where p.tbSurveyItem.tbSurveyGroup.Id == vm.SurveyGroupId
                                    && p.tbOrg != null
                                    && p.tbClass == null
                                    && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                    select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryData = tbSurveryData.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                var tbSurveryDataList = (from p in tbSurveryData
                                         select new
                                         {
                                             orgId = p.tbOrg.Id,
                                             orgName = p.tbOrg.OrgName,
                                             courseId = p.tbOrg.tbCourse.Id,
                                             courseName = p.tbOrg.tbCourse.CourseName,
                                             teacherId = p.tbTeacher.Id,
                                             teacherName = p.tbTeacher.TeacherName,
                                             surveryItemId = p.tbSurveyItem.Id,
                                             surveryItemName = p.tbSurveyItem.SurveyItemName,
                                             surverOptionId = p.tbSurveyOption.Id,
                                             surverOptionName = p.tbSurveyOption.OptionName,
                                             surverOptionValue = p.tbSurveyOption.OptionValue
                                         }).ToList();

                vm.SurveySubjectReportList = (from p in tbSurveryDataList
                                              where teacherIds.Contains(p.teacherId)
                                              group p by new
                                              {
                                                  orgId = p.orgId,
                                                  orgName = p.orgName,
                                                  courseId = p.courseId,
                                                  courseName = p.courseName,
                                                  teacherId = p.teacherId,
                                                  teacherName = p.teacherName,
                                                  surveryItemId = p.surveryItemId,
                                                  surveryItemName = p.surveryItemName,
                                                  surverOptionId = p.surverOptionId,
                                                  surverOptionName = p.surverOptionName
                                              } into g
                                              select new Dto.SurveyReport.SubjectCourseList
                                              {
                                                  OrgId = g.Key.orgId,
                                                  OrgName = g.Key.orgName,
                                                  CourseId = g.Key.courseId,
                                                  CourseName = g.Key.courseName,
                                                  SurveyItemId = g.Key.surveryItemId,
                                                  SurveyItemName = g.Key.surveryItemName,
                                                  SurveyOptionId = g.Key.surverOptionId,
                                                  SurveyOptionName = g.Key.surverOptionName,
                                                  TeacherId = g.Key.teacherId,
                                                  TeacherName = g.Key.teacherName,
                                                  SurveyOptionCount = g.Count(),
                                                  SurveyOptionSum = g.Select(d => d.surverOptionValue).Sum()
                                              }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataText = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.Id == vm.SurveyGroupId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                        select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataText = tbSurveryDataText.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                var tbSurveryDataTextList = (from p in tbSurveryDataText
                                             select new
                                             {
                                                 orgId = p.tbOrg.Id,
                                                 orgName = p.tbOrg.OrgName,
                                                 courseId = p.tbOrg.tbCourse.Id,
                                                 courseName = p.tbOrg.tbCourse.CourseName,
                                                 teacherId = p.tbTeacher.Id,
                                                 teacherName = p.tbTeacher.TeacherName,
                                                 surveryItemId = p.tbSurveyItem.Id,
                                                 surveryItemName = p.tbSurveyItem.SurveyItemName
                                             }).ToList();


                vm.SurveySubjectReportTextList = (from p in tbSurveryDataTextList
                                                  where teacherIds.Contains(p.teacherId)
                                                  group p by new
                                                  {
                                                      orgId = p.orgId,
                                                      orgName = p.orgName,
                                                      courseId = p.courseId,
                                                      courseName = p.courseName,
                                                      teacherId = p.teacherId,
                                                      teacherName = p.teacherName,
                                                      surveryItemId = p.surveryItemId,
                                                      surveryItemName = p.surveryItemName
                                                  } into g
                                                  select new Dto.SurveyReport.SubjectCourseList
                                                  {
                                                      OrgId = g.Key.orgId,
                                                      OrgName = g.Key.orgName,
                                                      CourseId = g.Key.courseId,
                                                      CourseName = g.Key.courseName,
                                                      SurveyItemId = g.Key.surveryItemId,
                                                      SurveyItemName = g.Key.surveryItemName,
                                                      TeacherId = g.Key.teacherId,
                                                      TeacherName = g.Key.teacherName,
                                                      SurveyOptionCount = g.Count()
                                                  }).ToList();
                #endregion

                vm.SurveyItemInfoList = (from p in db.Table<Entity.tbSurveyItem>()
                                         where p.tbSurveyGroup.Id == vm.SurveyGroupId
                                         orderby p.tbSurveyGroup.No, p.No
                                         select new Dto.SurveyItem.Info
                                         {
                                             Id = p.Id,
                                             SurveyItemName = p.SurveyItemName,
                                             SurveyItemType = p.SurveyItemType,
                                             No = p.No
                                         }).Distinct().ToList();

                vm.SurveyOptionInfoList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where p.tbSurveyItem.tbSurveyGroup.Id == vm.SurveyGroupId
                                           orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               OptionValue = p.OptionValue,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).Distinct().ToList();
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();
                var surveyOptionCount = 0;
                var itemList = vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                foreach (var item in itemList)
                {
                    var optionList = vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList();
                    foreach (var option in optionList)
                    {
                        surveyOptionCount = surveyOptionCount + 2;
                    }
                }
                var itemTextList = vm.SurveyItemInfoList.Where(d => d.SurveyItemType == XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                foreach (var item in itemTextList)
                {
                    surveyOptionCount++;
                }
                var allColumnLength = surveyOptionCount + 7;
                var arrColumns = new string[allColumnLength];

                for (int i = 0; i < arrColumns.Length; i++)
                {
                    arrColumns[i] = (i + 1).ToString();
                }

                var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                sheet.isColumnWritten = false;
                sheet.isWriteHeader = true;
                sheet.strHeaderText = "任课教师统计";
                //开始表格
                var dt = Code.Common.ArrayToDataTable(arrColumns);
                var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                var arrSurveyItemID = new string[allColumnLength];
                var arrSurveyItemName = new string[allColumnLength];
                var arrSurveyOptionID = new string[allColumnLength];
                var arrSurveyOptionName = new string[allColumnLength];
                var arrSurveyOptionSonID = new string[allColumnLength];
                var arrSurveyOptionSonName = new string[allColumnLength];
                var index = 3;
                for (int i = 0; i < itemList.Count(); i++)
                {
                    var optionList = vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == itemList[i].Id).Distinct().ToList();
                    for (var j = 0; j < optionList.Count(); j++)
                    {
                        for (var n = 0; n < 2; n++)
                        {
                            index++;
                            if (n == 0)
                            {
                                arrSurveyItemID[index] = itemList[i].Id.ToString();
                                arrSurveyItemName[index] = itemList[i].SurveyItemName.ToString();
                                arrSurveyOptionID[index] = optionList[j].Id.ToString();
                                arrSurveyOptionName[index] = optionList[j].OptionName.ToString();
                                arrSurveyOptionSonID[index] = "人数";
                                arrSurveyOptionSonName[index] = "人数";
                            }
                            else if (n == 1)
                            {
                                arrSurveyItemID[index] = itemList[i].Id.ToString();
                                arrSurveyItemName[index] = itemList[i].SurveyItemName.ToString();
                                arrSurveyOptionID[index] = optionList[j].Id.ToString();
                                arrSurveyOptionName[index] = optionList[j].OptionName.ToString();
                                arrSurveyOptionSonID[index] = "比例%";
                                arrSurveyOptionSonName[index] = "比例%";
                            }
                        }
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(2, 2, index - 1, index));
                    }
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, index - optionList.Count() * 2 + 1, index));
                }
                arrSurveyItemName[index + 1] = "评价次数";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 3, index + 1, index + 1));
                arrSurveyItemName[index + 2] = "全部得分";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 3, index + 2, index + 2));
                arrSurveyItemName[index + 3] = "优秀比例";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 3, index + 3, index + 3));
                index = index + 3;
                for (int i = 0; i < itemTextList.Count(); i++)
                {
                    index++;
                    arrSurveyItemID[index] = itemTextList[i].Id.ToString();
                    arrSurveyItemName[index] = itemTextList[i].SurveyItemName.ToString();
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 3, index, index));
                }
                dt.Rows.Add(arrSurveyItemName);
                dt.Rows.Add(arrSurveyOptionName);
                dt.Rows.Add(arrSurveyOptionSonName);
                #region 处理DataList
                foreach (var teacher in vm.SurveyTeacherList.OrderBy(d => d.CourseNo).ThenBy(d => d.CourseName).ThenBy(d => d.TeacherName).ToList())
                {
                    string[] arrSurveyData = new string[allColumnLength];
                    arrSurveyData[0] = teacher.CourseName;
                    arrSurveyData[1] = teacher.TeacherName;
                    arrSurveyData[2] = teacher.OrgName;
                    arrSurveyData[3] = teacher.ClassName;
                    var allCount = 0;
                    var goodCount = 0m;
                    var goodRate = 0m;
                    var indexR = 3;
                    foreach (var item in vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                    {
                        decimal itemCount = vm.SurveySubjectReportList
                                .Where(d => d.TeacherId == teacher.TeacherId && d.CourseId == teacher.CourseId && d.OrgId == teacher.OrgId && d.SurveyItemId == item.Id)
                                .Sum(d => d.SurveyOptionCount);
                        foreach (var option in vm.SurveyOptionInfoList.Where(d => d.SurveyItemId == item.Id).Distinct().ToList())
                        {
                            indexR++;
                            var optionCount = vm.SurveySubjectReportList
                                .Where(d => d.TeacherId == teacher.TeacherId && d.CourseId == teacher.CourseId && d.OrgId == teacher.OrgId && d.SurveyItemId == item.Id && d.SurveyOptionId == option.Id)
                                .Sum(d => d.SurveyOptionCount);
                            if (option.OptionName.Contains("A：优秀"))
                            {
                                goodCount += optionCount;
                            }
                            allCount += optionCount;
                            if (optionCount > decimal.Zero)
                            {
                                arrSurveyData[indexR] = optionCount.ToString();
                            }
                            indexR++;
                            if (itemCount > decimal.Zero)
                            {
                                var optionRate = Decimal.Round(optionCount / itemCount * 100, 2);
                                if (optionRate > decimal.Zero)
                                {
                                    arrSurveyData[indexR] = optionRate.ToString() + "%";
                                }
                            }
                        }
                    }
                    indexR++;
                    if (allCount > decimal.Zero)
                    {
                        arrSurveyData[indexR] = allCount.ToString();
                    }
                    indexR++;
                    var optionSum = vm.SurveySubjectReportList.Where(d => d.TeacherId == teacher.TeacherId && d.CourseId == teacher.CourseId && d.OrgId == teacher.OrgId).Sum(d => d.SurveyOptionSum);
                    if (optionSum > decimal.Zero)
                    {
                        arrSurveyData[indexR] = optionSum.ToString();
                    }
                    if (allCount > decimal.Zero)
                    {
                        goodRate = Decimal.Round(goodCount / allCount * 100, 2);
                    }
                    indexR++;
                    if (goodRate > decimal.Zero)
                    {
                        arrSurveyData[indexR] = goodRate.ToString() + "%";
                    }
                    foreach (var item in vm.SurveyItemInfoList.Where(d => d.SurveyItemType == XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No))
                    {
                        indexR++;
                        var optionTextCount = vm.SurveySubjectReportTextList
                            .Where(d => d.TeacherId == teacher.TeacherId && d.CourseId == teacher.CourseId && d.OrgId == teacher.OrgId && d.SurveyItemId == item.Id)
                            .Sum(d => d.SurveyOptionCount);
                        if (optionTextCount > 0)
                        {
                            arrSurveyData[indexR] = optionTextCount.ToString();
                        }
                    }
                    dt.Rows.Add(arrSurveyData);
                }
                #endregion
                dt.Rows[0][0] = "课程";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 3, 0, 0));
                dt.Rows[0][1] = "教师";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 3, 1, 1));
                dt.Rows[0][2] = "教学班";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 3, 2, 2));
                dt.Rows[0][3] = "行政班";
                regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 3, 3, 3));
                sheet.data = dt;
                sheet.regions = regions;
                sheetList.Add(sheet);
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

        public ActionResult SubjectCourseListHor()
        {
            var vm = new Models.SurveyReport.SubjectCourseListHor();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                if (vm.SurveyGroupId == 0 && vm.SurveyGroupList.Count > 0)
                {
                    vm.SurveyGroupId = vm.SurveyGroupList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.Id == vm.SurveyGroupId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                var surveyCourseIds = new List<int>();
                if (vm.SurveyCourseId == 0)
                {
                    surveyCourseIds = vm.SurveyCourseList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyCourseIds.Add((int)vm.SurveyCourseId);
                }

                vm.SurveyTeacherList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                        join m in db.Table<Entity.tbSurveyCourse>()
                                        on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                        equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                        where p.tbOrg.IsDeleted == false
                                        orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new Dto.SurveyReport.SubjectCourseListHor
                                        {
                                            CourseNo = p.tbOrg.tbCourse.No,
                                            CourseId = p.tbOrg.tbCourse.Id,
                                            CourseName = p.tbOrg.tbCourse.CourseName,
                                            TeacherId = p.tbTeacher.Id,
                                            TeacherName = p.tbTeacher.TeacherName,
                                            OrgId = p.tbOrg.Id,
                                            OrgName = p.tbOrg.OrgName,
                                            ClassId = p.tbOrg.IsClass ? (p.tbOrg.tbClass == null ? 0 : p.tbOrg.tbClass.Id) : 0,
                                            ClassName = p.tbOrg.IsClass ? (p.tbOrg.tbClass == null ? "" : p.tbOrg.tbClass.ClassName) : ""
                                        }).Distinct().ToList();

                vm.SurveyTeacherList = vm.SurveyTeacherList.Where(d => surveyCourseIds.Contains(d.CourseId)).ToList();

                var teacherIds = vm.SurveyTeacherList.Select(d => d.TeacherId).Distinct().ToList();

                #region 单选分数
                var tbSurveryData = from p in db.Table<Entity.tbSurveyData>()
                                    where p.tbSurveyItem.tbSurveyGroup.Id == vm.SurveyGroupId
                                    && p.tbOrg != null
                                    && p.tbClass == null
                                    && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                    select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryData = tbSurveryData.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                var tbSurveryDataList = (from p in tbSurveryData
                                         select new
                                         {
                                             orgId = p.tbOrg.Id,
                                             orgName = p.tbOrg.OrgName,
                                             courseId = p.tbOrg.tbCourse.Id,
                                             courseName = p.tbOrg.tbCourse.CourseName,
                                             teacherId = p.tbTeacher.Id,
                                             teacherName = p.tbTeacher.TeacherName,
                                             surveryItemId = p.tbSurveyItem.Id,
                                             surveryItemName = p.tbSurveyItem.SurveyItemName,
                                             surverOptionId = p.tbSurveyOption.Id,
                                             surverOptionName = p.tbSurveyOption.OptionName,
                                             surverOptionValue = p.tbSurveyOption.OptionValue
                                         }).ToList();

                vm.SurveySubjectReportList = (from p in tbSurveryDataList
                                              where teacherIds.Contains(p.teacherId)
                                              group p by new
                                              {
                                                  orgId = p.orgId,
                                                  orgName = p.orgName,
                                                  courseId = p.courseId,
                                                  courseName = p.courseName,
                                                  teacherId = p.teacherId,
                                                  teacherName = p.teacherName,
                                                  surveryItemId = p.surveryItemId,
                                                  surveryItemName = p.surveryItemName,
                                                  surverOptionId = p.surverOptionId,
                                                  surverOptionName = p.surverOptionName
                                              } into g
                                              select new Dto.SurveyReport.SubjectCourseListHor
                                              {
                                                  OrgId = g.Key.orgId,
                                                  OrgName = g.Key.orgName,
                                                  CourseId = g.Key.courseId,
                                                  CourseName = g.Key.courseName,
                                                  SurveyItemId = g.Key.surveryItemId,
                                                  SurveyItemName = g.Key.surveryItemName,
                                                  SurveyOptionId = g.Key.surverOptionId,
                                                  SurveyOptionName = g.Key.surverOptionName,
                                                  TeacherId = g.Key.teacherId,
                                                  TeacherName = g.Key.teacherName,
                                                  SurveyOptionCount = g.Count(),
                                                  SurveyOptionSum = g.Select(d => d.surverOptionValue).Sum()
                                              }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataText = from p in db.Table<Entity.tbSurveyData>()
                                            where p.tbSurveyItem.tbSurveyGroup.Id == vm.SurveyGroupId
                                            && p.tbOrg != null
                                            && p.tbClass == null
                                            && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                            select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataText = tbSurveryDataText.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                var tbSurveryDataTextList = (from p in tbSurveryDataText
                                             select new
                                            {
                                             orgId = p.tbOrg.Id,
                                             orgName = p.tbOrg.OrgName,
                                             courseId = p.tbOrg.tbCourse.Id,
                                             courseName = p.tbOrg.tbCourse.CourseName,
                                             teacherId = p.tbTeacher.Id,
                                             teacherName = p.tbTeacher.TeacherName,
                                             surveryItemId = p.tbSurveyItem.Id,
                                             surveryItemName = p.tbSurveyItem.SurveyItemName
                                         }).ToList();


                vm.SurveySubjectReportTextList = (from p in tbSurveryDataTextList
                                                  where teacherIds.Contains(p.teacherId)
                                                  group p by new
                                                  {
                                                      orgId = p.orgId,
                                                      orgName = p.orgName,
                                                      courseId = p.courseId,
                                                      courseName = p.courseName,
                                                      teacherId = p.teacherId,
                                                      teacherName = p.teacherName,
                                                      surveryItemId = p.surveryItemId,
                                                      surveryItemName = p.surveryItemName
                                                  } into g
                                                  select new Dto.SurveyReport.SubjectCourseListHor
                                                  {
                                                      OrgId = g.Key.orgId,
                                                      OrgName = g.Key.orgName,
                                                      CourseId = g.Key.courseId,
                                                      CourseName = g.Key.courseName,
                                                      SurveyItemId = g.Key.surveryItemId,
                                                      SurveyItemName = g.Key.surveryItemName,
                                                      TeacherId = g.Key.teacherId,
                                                      TeacherName = g.Key.teacherName,
                                                      SurveyOptionCount = g.Count()
                                                  }).ToList();
                #endregion

                vm.SurveyItemInfoList = (from p in db.Table<Entity.tbSurveyItem>()
                                         where p.tbSurveyGroup.Id == vm.SurveyGroupId
                                         orderby p.tbSurveyGroup.No, p.No
                                         select new Dto.SurveyItem.Info
                                         {
                                             Id = p.Id,
                                             SurveyItemName = p.SurveyItemName,
                                             SurveyItemType = p.SurveyItemType,
                                             No = p.No
                                         }).Distinct().ToList();

                vm.SurveyOptionInfoList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where p.tbSurveyItem.tbSurveyGroup.Id == vm.SurveyGroupId
                                           orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               OptionValue = p.OptionValue,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).Distinct().ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubjectCourseListHor(Models.SurveyReport.SubjectCourseListHor vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SubjectCourseListHor", new
            {
                SearchText = vm.SearchText,
                SurveyId = vm.SurveyId,
                SurveyGroupId = vm.SurveyGroupId,
                SurveyCourseId = vm.SurveyCourseId
            }));
        }
        public ActionResult SubjectCourseListHorExport()
        {
            var vm = new Models.SurveyReport.SubjectCourseListHor();
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 准备数据
                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                      where p.tbSurvey.Id == vm.SurveyId
                                      && p.tbSurvey.IsDeleted == false
                                      && p.IsOrg
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.SurveyGroupName,
                                          Value = p.Id.ToString()
                                      }).ToList();

                if (vm.SurveyGroupId == 0 && vm.SurveyGroupList.Count > 0)
                {
                    vm.SurveyGroupId = vm.SurveyGroupList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                       where p.tbSurveyGroup.Id == vm.SurveyGroupId
                                       && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                       && p.tbSurveyGroup.IsDeleted == false
                                       && p.tbSurveyGroup.IsOrg
                                       && p.tbCourse.IsDeleted == false
                                       orderby p.tbCourse.tbSubject.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.tbCourse.CourseName,
                                           Value = p.tbCourse.Id.ToString()
                                       }).Distinct().ToList();

                var surveyCourseIds = new List<int>();
                if (vm.SurveyCourseId == 0)
                {
                    surveyCourseIds = vm.SurveyCourseList.Select(d => d.Value.ConvertToInt()).ToList();
                }
                else
                {
                    surveyCourseIds.Add((int)vm.SurveyCourseId);
                }

                vm.SurveyTeacherList = (from p in db.Table<Areas.Course.Entity.tbOrgTeacher>()
                                        join m in db.Table<Entity.tbSurveyCourse>()
                                        on new { yearId = p.tbOrg.tbYear.Id, courseId = p.tbOrg.tbCourse.Id }
                                        equals new { yearId = m.tbSurveyGroup.tbSurvey.tbYear.Id, courseId = m.tbCourse.Id }
                                        where p.tbOrg.IsDeleted == false
                                        orderby p.tbTeacher.TeacherCode, p.tbTeacher.TeacherName
                                        select new Dto.SurveyReport.SubjectCourseListHor
                                        {
                                            CourseNo = p.tbOrg.tbCourse.No,
                                            CourseId = p.tbOrg.tbCourse.Id,
                                            CourseName = p.tbOrg.tbCourse.CourseName,
                                            TeacherId = p.tbTeacher.Id,
                                            TeacherName = p.tbTeacher.TeacherName,
                                            OrgId = p.tbOrg.Id,
                                            OrgName = p.tbOrg.OrgName,
                                            ClassId = p.tbOrg.IsClass ? (p.tbOrg.tbClass == null ? 0 : p.tbOrg.tbClass.Id) : 0,
                                            ClassName = p.tbOrg.IsClass ? (p.tbOrg.tbClass == null ? "" : p.tbOrg.tbClass.ClassName) : ""
                                        }).Distinct().ToList();

                vm.SurveyTeacherList = vm.SurveyTeacherList.Where(d => surveyCourseIds.Contains(d.CourseId)).ToList();

                var teacherIds = vm.SurveyTeacherList.Select(d => d.TeacherId).Distinct().ToList();

                #region 单选分数
                var tbSurveryData = from p in db.Table<Entity.tbSurveyData>()
                                    where p.tbSurveyItem.tbSurveyGroup.Id == vm.SurveyGroupId
                                    && p.tbOrg != null
                                    && p.tbClass == null
                                    && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                    select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryData = tbSurveryData.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                var tbSurveryDataList = (from p in tbSurveryData
                                         select new
                                         {
                                             orgId = p.tbOrg.Id,
                                             orgName = p.tbOrg.OrgName,
                                             courseId = p.tbOrg.tbCourse.Id,
                                             courseName = p.tbOrg.tbCourse.CourseName,
                                             teacherId = p.tbTeacher.Id,
                                             teacherName = p.tbTeacher.TeacherName,
                                             surveryItemId = p.tbSurveyItem.Id,
                                             surveryItemName = p.tbSurveyItem.SurveyItemName,
                                             surverOptionId = p.tbSurveyOption.Id,
                                             surverOptionName = p.tbSurveyOption.OptionName,
                                             surverOptionValue = p.tbSurveyOption.OptionValue
                                         }).ToList();

                vm.SurveySubjectReportList = (from p in tbSurveryDataList
                                              where teacherIds.Contains(p.teacherId)
                                              group p by new
                                              {
                                                  orgId = p.orgId,
                                                  orgName = p.orgName,
                                                  courseId = p.courseId,
                                                  courseName = p.courseName,
                                                  teacherId = p.teacherId,
                                                  teacherName = p.teacherName,
                                                  surveryItemId = p.surveryItemId,
                                                  surveryItemName = p.surveryItemName,
                                                  surverOptionId = p.surverOptionId,
                                                  surverOptionName = p.surverOptionName
                                              } into g
                                              select new Dto.SurveyReport.SubjectCourseListHor
                                              {
                                                  OrgId = g.Key.orgId,
                                                  OrgName = g.Key.orgName,
                                                  CourseId = g.Key.courseId,
                                                  CourseName = g.Key.courseName,
                                                  SurveyItemId = g.Key.surveryItemId,
                                                  SurveyItemName = g.Key.surveryItemName,
                                                  SurveyOptionId = g.Key.surverOptionId,
                                                  SurveyOptionName = g.Key.surverOptionName,
                                                  TeacherId = g.Key.teacherId,
                                                  TeacherName = g.Key.teacherName,
                                                  SurveyOptionCount = g.Count(),
                                                  SurveyOptionSum = g.Select(d => d.surverOptionValue).Sum()
                                              }).ToList();
                #endregion

                #region 问答模式
                var tbSurveryDataText = from p in db.Table<Entity.tbSurveyData>()
                                        where p.tbSurveyItem.tbSurveyGroup.Id == vm.SurveyGroupId
                                        && p.tbOrg != null
                                        && p.tbClass == null
                                        && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                        select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbSurveryDataText = tbSurveryDataText.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText) || d.tbOrg.OrgName.Contains(vm.SearchText)));
                }

                var tbSurveryDataTextList = (from p in tbSurveryDataText
                                             select new
                                             {
                                                 orgId = p.tbOrg.Id,
                                                 orgName = p.tbOrg.OrgName,
                                                 courseId = p.tbOrg.tbCourse.Id,
                                                 courseName = p.tbOrg.tbCourse.CourseName,
                                                 teacherId = p.tbTeacher.Id,
                                                 teacherName = p.tbTeacher.TeacherName,
                                                 surveryItemId = p.tbSurveyItem.Id,
                                                 surveryItemName = p.tbSurveyItem.SurveyItemName
                                             }).ToList();


                vm.SurveySubjectReportTextList = (from p in tbSurveryDataTextList
                                                  where teacherIds.Contains(p.teacherId)
                                                  group p by new
                                                  {
                                                      orgId = p.orgId,
                                                      orgName = p.orgName,
                                                      courseId = p.courseId,
                                                      courseName = p.courseName,
                                                      teacherId = p.teacherId,
                                                      teacherName = p.teacherName,
                                                      surveryItemId = p.surveryItemId,
                                                      surveryItemName = p.surveryItemName
                                                  } into g
                                                  select new Dto.SurveyReport.SubjectCourseListHor
                                                  {
                                                      OrgId = g.Key.orgId,
                                                      OrgName = g.Key.orgName,
                                                      CourseId = g.Key.courseId,
                                                      CourseName = g.Key.courseName,
                                                      SurveyItemId = g.Key.surveryItemId,
                                                      SurveyItemName = g.Key.surveryItemName,
                                                      TeacherId = g.Key.teacherId,
                                                      TeacherName = g.Key.teacherName,
                                                      SurveyOptionCount = g.Count()
                                                  }).ToList();
                #endregion

                vm.SurveyItemInfoList = (from p in db.Table<Entity.tbSurveyItem>()
                                         where p.tbSurveyGroup.Id == vm.SurveyGroupId
                                         orderby p.tbSurveyGroup.No, p.No
                                         select new Dto.SurveyItem.Info
                                         {
                                             Id = p.Id,
                                             SurveyItemName = p.SurveyItemName,
                                             SurveyItemType = p.SurveyItemType,
                                             No = p.No
                                         }).Distinct().ToList();

                vm.SurveyOptionInfoList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where p.tbSurveyItem.tbSurveyGroup.Id == vm.SurveyGroupId
                                           orderby p.tbSurveyItem.tbSurveyGroup.No, p.tbSurveyItem.No, p.No
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               OptionValue = p.OptionValue,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).Distinct().ToList();
                #endregion
                var file = System.IO.Path.GetTempFileName();
                var sheetList = new List<Code.NpoiHelper.DataTableToExcelPram>();
                var surveyOptionCount = 0;
                var optionList = vm.SurveyOptionInfoList.Select(d => d.OptionName.Substring(0, 1)).Distinct().ToList();
                var itemList = vm.SurveyItemInfoList.Where(d => d.SurveyItemType != XkSystem.Code.EnumHelper.SurveyItemType.TextBox).OrderBy(d => d.No).ToList();
                surveyOptionCount = optionList.Count() * 2;
                var allColumnLength = surveyOptionCount + 2;
                var arrColumns = new string[allColumnLength];

                for (int i = 0; i < arrColumns.Length; i++)
                {
                    arrColumns[i] = (i + 1).ToString();
                }
                foreach (var teacher in vm.SurveyTeacherList.GroupBy(d => new { d.TeacherId, d.TeacherName }).Select(g => new { g.Key.TeacherId, g.Key.TeacherName }).ToList())
                {
                    var sheet = new Code.NpoiHelper.DataTableToExcelPram();
                    sheet.isColumnWritten = false;
                    sheet.isWriteHeader = true;
                    sheet.strHeaderText = teacher.TeacherName;
                    //开始表格
                    var dt = Code.Common.ArrayToDataTable(arrColumns);
                    var regions = new List<NPOI.SS.Util.CellRangeAddress>();
                    #region 处理DataList
                    var indexL = 1;
                    foreach (var teacherOrg in vm.SurveyTeacherList.Where(d => d.TeacherId == teacher.TeacherId)
            .GroupBy(d => new { d.TeacherId, d.TeacherName, d.CourseId, d.CourseName, d.CourseNo, d.OrgId, d.OrgName, d.ClassId, d.ClassName, d.ClassNo })
            .Select(g => new { g.Key.TeacherId, g.Key.TeacherName, g.Key.CourseId, g.Key.CourseName, g.Key.CourseNo, g.Key.OrgId, g.Key.OrgName, g.Key.ClassId, g.Key.ClassName, g.Key.ClassNo }).OrderBy(g => g.TeacherId))
                    {
                        var arrSurveyItemID = new string[allColumnLength];
                        var arrSurveyItemName = new string[allColumnLength];
                        var arrSurveyOptionID = new string[allColumnLength];
                        var arrSurveyOptionName = new string[allColumnLength];
                        arrSurveyItemID[0] = "任课教师";
                        arrSurveyItemName[0] = "任课教师";
                        arrSurveyOptionID[0] = "任课教师";
                        arrSurveyOptionName[0] = "任课教师";
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(indexL, indexL + 1, 0, 0));
                        arrSurveyItemID[1] = "评价内容";
                        arrSurveyItemName[1] = "评价内容";
                        arrSurveyOptionID[1] = "评价内容";
                        arrSurveyOptionName[1] = "评价内容";
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(indexL, indexL + 1, 1, 1));
                        var index = 1;
                        for (int i = 0; i < 1; i++)
                        {
                            for (var j = 0; j < optionList.Count(); j++)
                            {
                                for (var n = 0; n < 2; n++)
                                {
                                    index++;
                                    arrSurveyItemID[index] = "选项";
                                    arrSurveyItemName[index] = "选项";
                                    arrSurveyOptionID[index] = "选择" + optionList[j].ToString() + "(人数/所占比例)";
                                    arrSurveyOptionName[index] = "选择" + optionList[j].ToString() + "(人数/所占比例)";
                                }
                                regions.Add(new NPOI.SS.Util.CellRangeAddress(indexL + 1, indexL + 1, index - 1, index));
                            }
                            regions.Add(new NPOI.SS.Util.CellRangeAddress(indexL, indexL, index - optionList.Count() * 2 + 1, index));
                        }
                        dt.Rows.Add(arrSurveyItemName);
                        indexL++;
                        dt.Rows.Add(arrSurveyOptionName);
                        indexL++;
                        var rowIndex = 1;
                        foreach (var item in itemList)
                        {
                            if(rowIndex==1)
                            {
                                string[] arrSurveyData = new string[allColumnLength];
                                arrSurveyData[0] = teacherOrg.TeacherName + "\r\n" + teacherOrg.CourseName + "\r\n" + teacherOrg.OrgName + "\r\n" + teacherOrg.ClassName;
                                arrSurveyData[1] = rowIndex.ToString() + "、" + item.SurveyItemName;
                                decimal itemCount = vm.SurveySubjectReportList
                                        .Where(d => d.TeacherId == teacherOrg.TeacherId && d.CourseId == teacherOrg.CourseId && d.OrgId == teacherOrg.OrgId && d.SurveyItemId == item.Id)
                                        .Sum(d => d.SurveyOptionCount);
                                var indexR = 1;
                                foreach (var option in optionList)
                                {
                                    indexR++;
                                    var optionCount = vm.SurveySubjectReportList
                                        .Where(d => d.TeacherId == teacherOrg.TeacherId && d.CourseId == teacherOrg.CourseId && d.OrgId == teacherOrg.OrgId && d.SurveyItemId == item.Id && d.SurveyOptionName.Substring(0, 1) == option)
                                        .Sum(d => d.SurveyOptionCount);
                                    if (optionCount > decimal.Zero)
                                    {
                                        arrSurveyData[indexR] = optionCount.ToString();
                                    }
                                    indexR++;
                                    if (itemCount > decimal.Zero)
                                    {
                                        var optionRate = Decimal.Round(optionCount / itemCount * 100, 2);
                                        if (optionRate > decimal.Zero)
                                        {
                                            arrSurveyData[indexR] = optionRate.ToString() + "%";
                                        }
                                    }
                                }
                                dt.Rows.Add(arrSurveyData);
                                regions.Add(new NPOI.SS.Util.CellRangeAddress(indexL, indexL + itemList.Count()-1, 0, 0));
                            }
                            else
                            {
                                string[] arrSurveyData = new string[allColumnLength];
                                arrSurveyData[1] = rowIndex.ToString() + "、" + item.SurveyItemName;
                                decimal itemCount = vm.SurveySubjectReportList
                                        .Where(d => d.TeacherId == teacherOrg.TeacherId && d.CourseId == teacherOrg.CourseId && d.OrgId == teacherOrg.OrgId && d.SurveyItemId == item.Id)
                                        .Sum(d => d.SurveyOptionCount);
                                var indexR = 1;
                                foreach (var option in optionList)
                                {
                                    indexR++;
                                    var optionCount = vm.SurveySubjectReportList
                                        .Where(d => d.TeacherId == teacherOrg.TeacherId && d.CourseId == teacherOrg.CourseId && d.OrgId == teacherOrg.OrgId && d.SurveyItemId == item.Id && d.SurveyOptionName.Substring(0, 1) == option)
                                        .Sum(d => d.SurveyOptionCount);
                                    if (optionCount > decimal.Zero)
                                    {
                                        arrSurveyData[indexR] = optionCount.ToString();
                                    }
                                    indexR++;
                                    if (itemCount > decimal.Zero)
                                    {
                                        var optionRate = Decimal.Round(optionCount / itemCount * 100, 2);
                                        if (optionRate > decimal.Zero)
                                        {
                                            arrSurveyData[indexR] = optionRate.ToString() + "%";
                                        }
                                    }
                                }
                                dt.Rows.Add(arrSurveyData);
                            }                            
                            rowIndex++;
                            indexL++;
                        }

                        string[] arrSurveyDataLast = new string[allColumnLength];
                        dt.Rows.Add(arrSurveyDataLast);
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(indexL, indexL, 0, allColumnLength-1));
                        indexL++;
                        dt.Rows.Add(arrSurveyDataLast);
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(indexL, indexL, 0, allColumnLength-1));
                        indexL++;
                        

                    }
                    #endregion
                    sheet.data = dt;
                    sheet.regions = regions;
                    sheetList.Add(sheet);
                }
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