using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveReportController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveReport.List();
                vm.ElectiveList = ElectiveController.SelectList();

                if (vm.ElectiveId == 0 && vm.ElectiveList.Count() > 0)
                {
                    vm.ElectiveId = vm.ElectiveList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.IsWeekPeriod = (from p in db.Table<Entity.tbElective>()
                                   where p.Id == vm.ElectiveId
                                   select p.tbElectiveType.ElectiveTypeCode).FirstOrDefault() == Code.EnumHelper.ElectiveType.WeekPeriod;

                var tb = from p in db.Table<Entity.tbElectiveOrg>()
                         where p.tbElective.Id == vm.ElectiveId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.OrgName.Contains(vm.SearchText));
                }

                vm.ElectiveOrgList = (from p in tb
                                      orderby p.tbElectiveSection.No, p.tbElectiveGroup.No, p.OrgName
                                      select new Dto.ElectiveOrg.List()
                                      {
                                          Id = p.Id,
                                          OrgName = p.OrgName,
                                          CourseName = p.tbCourse.CourseName,
                                          MaxCount = p.MaxCount,
                                          RemainCount = p.RemainCount,
                                          TeacherName = p.tbTeacher.TeacherName,
                                          RoomName = p.tbRoom.RoomName,
                                          ElectiveGroupId = p.tbElectiveGroup.Id,
                                          ElectiveGroupName = p.tbElectiveGroup.ElectiveGroupName,
                                          ElectiveGroupMaxElective = p.tbElectiveGroup.MaxElective,
                                          ElectiveSectionId = p.tbElectiveSection.Id,
                                          ElectiveSectionName = p.tbElectiveSection.ElectiveSectionName,
                                          ElectiveSectionMaxElective = p.tbElectiveSection.MaxElective
                                      }).ToList();

                var orgScheduleList = (from p in db.Table<Entity.tbElectiveOrgSchedule>()
                                       where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                       select new
                                       {
                                           OrgId = p.tbElectiveOrg.Id,
                                           p.tbWeek.WeekName,
                                           p.tbPeriod.PeriodName
                                       }).ToList();

                var dataList = (from p in db.Table<Entity.tbElectiveData>()
                                where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                    && p.tbStudent.IsDeleted == false
                                group p by p.tbElectiveOrg.Id into g
                                select new
                                {
                                    OrgId = g.Key,
                                    ElectiveCount = g.Count()
                                }).ToList();
                foreach (var a in vm.ElectiveOrgList)
                {
                    a.ElectiveCount = dataList.Where(d => d.OrgId == a.Id).Select(d => d.ElectiveCount).DefaultIfEmpty().FirstOrDefault();
                    a.WeekPeriod = string.Join(",", orgScheduleList.Where(o => o.OrgId == a.Id).Select(o => o.WeekName + o.PeriodName).Distinct().ToList());
                }

                if (vm.IsWeekPeriod)
                {
                    vm.IsHiddenSection = true;
                    vm.IsHiddenGroup = true;
                }
                else
                {
                    vm.IsHiddenSection = vm.ElectiveOrgList.Select(d => d.ElectiveSectionName).Distinct().Count() <= 1;
                    vm.IsHiddenGroup = vm.ElectiveOrgList.Select(d => d.ElectiveGroupName).Distinct().Count() <= 1;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { ElectiveId = vm.ElectiveId, SearchText = vm.SearchText }));
        }

        #region 未选学生_任课教师
        public ActionResult Teacher()
        {
            Models.ElectiveReport.List vm = new Models.ElectiveReport.List();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.ElectiveList = ElectiveController.SelectList();
                if (vm.ElectiveId == 0 && vm.ElectiveList.Count() > 0)
                {
                    vm.ElectiveId = vm.ElectiveList.FirstOrDefault().Value.ConvertToInt();
                }

                //任课教师
                vm.OrgList = ElectiveOrgController.SelectList(vm.ElectiveId, Code.Common.UserId);
                vm.OrgList.Insert(0, new SelectListItem() { Text = "全部班级", Value = "" });

                var tb = from p in db.Table<Entity.tbElectiveOrg>()
                         where p.tbElective.Id == vm.ElectiveId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.OrgName.Contains(vm.SearchText));
                }

                if (vm.OrgId != 0)
                {
                    tb = tb.Where(d => d.Id == vm.OrgId);
                }

                vm.ElectiveOrgList = (from p in tb
                                      join d in db.Table<Entity.tbElectiveData>() on p.Id equals d.tbElectiveOrg.Id
                                      join c in db.Table<Basis.Entity.tbClassStudent>() on d.tbStudent.Id equals c.tbStudent.Id
                                      join tc in db.Table<Basis.Entity.tbClass>() on c.tbClass.Id equals tc.Id
                                      join t in db.Table<Basis.Entity.tbClassTeacher>() on tc.Id equals t.tbClass.Id into result
                                      from x in result.DefaultIfEmpty()
                                      where p.tbElective.Id == vm.ElectiveId
                                         && p.tbCourse.IsDeleted == false
                                         && p.tbElectiveGroup.IsDeleted == false
                                         && p.tbElectiveSection.IsDeleted == false
                                         && d.IsDeleted == false
                                         && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                      orderby p.tbElectiveSection.Id, p.tbElectiveGroup.Id
                                      select new Dto.ElectiveOrg.List()
                                      {
                                          OrgName = p.OrgName,
                                          MaxCount = p.MaxCount,
                                          CourseId = p.tbCourse.Id,
                                          ClassName = tc.ClassName,
                                          IsPreElective = d.IsPreElective,
                                          RemainCount = p.RemainCount,
                                          RoomName = p.tbRoom.RoomName,
                                          CourseName = p.tbCourse.CourseName,
                                          StudentCode = d.tbStudent.StudentCode,
                                          StudentName = d.tbStudent.StudentName,
                                          ElectiveGroupName = p.tbElectiveGroup.ElectiveGroupName,
                                          HeadTeacherName = x == null ? "" : x.tbTeacher.TeacherName,
                                          ElectiveSectionName = p.tbElectiveSection.ElectiveSectionName
                                      }).ToList();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Teacher(Models.ElectiveReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("Teacher", new { ElectiveId = vm.ElectiveId, OrgId = vm.OrgId, SearchText = vm.SearchText }));
        }

        public ActionResult TeacherExport(Models.ElectiveReport.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                string file = System.IO.Path.GetTempFileName();

                var tb = from p in db.Table<Entity.tbElectiveOrg>()
                         where p.tbElective.Id == vm.ElectiveId
                         select p;

                var list = (from p in db.Table<Entity.tbElectiveOrg>()
                            join d in db.Table<Entity.tbElectiveData>() on p.Id equals d.tbElectiveOrg.Id
                            join c in db.Table<Basis.Entity.tbClassStudent>() on d.tbStudent.Id equals c.tbStudent.Id
                            join tc in db.Table<Basis.Entity.tbClass>() on c.tbClass.Id equals tc.Id
                            join t in db.Table<Basis.Entity.tbClassTeacher>() on tc.Id equals t.tbClass.Id into result
                            from x in result.DefaultIfEmpty()
                            where p.tbElective.Id == vm.ElectiveId
                               && p.tbCourse.IsDeleted == false
                               && p.tbElectiveGroup.IsDeleted == false
                               && p.tbElectiveSection.IsDeleted == false
                               && d.IsDeleted == false
                               && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                            orderby p.tbElectiveSection.Id, p.tbElectiveGroup.Id
                            select new Dto.ElectiveOrg.List()
                            {
                                OrgName = p.OrgName,
                                MaxCount = p.MaxCount,
                                CourseId = p.tbCourse.Id,
                                ClassName = tc.ClassName,
                                IsPreElective = d.IsPreElective,
                                RemainCount = p.RemainCount,
                                RoomName = p.tbRoom.RoomName,
                                CourseName = p.tbCourse.CourseName,
                                StudentCode = d.tbStudent.StudentCode,
                                StudentName = d.tbStudent.StudentName,
                                ElectiveGroupName = p.tbElectiveGroup.ElectiveGroupName,
                                HeadTeacherName = x == null ? "" : x.tbTeacher.TeacherName,
                                ElectiveSectionName = p.tbElectiveSection.ElectiveSectionName
                            }).ToList();



                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(
                    new string[] { "分段", "分组", "班级", "课程", "学号", "学生", "行政班级", "班主任", "是否预选" }.Select(p => new System.Data.DataColumn(p)).ToArray()
                    );
                foreach (var item in list)
                {
                    var dr = dt.NewRow();
                    dr["分段"] = item.ElectiveSectionName;
                    dr["分组"] = item.ElectiveGroupName;
                    dr["班级"] = item.OrgName;
                    dr["课程"] = item.CourseName;
                    dr["学号"] = item.StudentCode;
                    dr["学生"] = item.StudentName;
                    dr["行政班级"] = item.ClassName;
                    dr["班主任"] = item.HeadTeacherName;
                    dr["是否预选"] = item.IsPreElective ? "是" : "否";
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
        #endregion


        #region 未选学生_班主任
        public ActionResult HeaderTeacher()
        {
            Models.ElectiveReport.List vm = new Models.ElectiveReport.List();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.ElectiveList = ElectiveController.SelectList();
                if (vm.ElectiveId == 0 && vm.ElectiveList.Count() > 0)
                {
                    vm.ElectiveId = vm.ElectiveList.FirstOrDefault().Value.ConvertToInt();
                }
                vm.OrgList = ElectiveOrgController.SelectList(vm.ElectiveId);
                vm.OrgList.Insert(0, new SelectListItem() { Text = "全部班级", Value = "" });

                var tb = from p in db.Table<Entity.tbElectiveOrg>()
                         where p.tbElective.Id == vm.ElectiveId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.OrgName.Contains(vm.SearchText));
                }

                if (vm.OrgId != 0)
                {
                    tb = tb.Where(d => d.Id == vm.OrgId);
                }

                vm.ElectiveOrgList = (from p in tb
                                      join d in db.Table<Entity.tbElectiveData>() on p.Id equals d.tbElectiveOrg.Id
                                      join c in db.Table<Basis.Entity.tbClassStudent>() on d.tbStudent.Id equals c.tbStudent.Id
                                      join tc in db.Table<Basis.Entity.tbClass>() on c.tbClass.Id equals tc.Id
                                      join t in db.Table<Basis.Entity.tbClassTeacher>() on tc.Id equals t.tbClass.Id //into result
                                      //from x in result.DefaultIfEmpty()
                                      where p.tbElective.Id == vm.ElectiveId
                                         && t.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                         && p.tbCourse.IsDeleted == false
                                         && p.tbElectiveGroup.IsDeleted == false
                                         && p.tbElectiveSection.IsDeleted == false
                                         && d.IsDeleted == false
                                      orderby d.tbStudent.StudentCode
                                      select new Dto.ElectiveOrg.List()
                                      {
                                          OrgName = p.OrgName,
                                          MaxCount = p.MaxCount,
                                          CourseId = p.tbCourse.Id,
                                          ClassName = tc.ClassName,
                                          IsPreElective = d.IsPreElective,
                                          RemainCount = p.RemainCount,
                                          RoomName = p.tbRoom.RoomName,
                                          CourseName = p.tbCourse.CourseName,
                                          StudentCode = d.tbStudent.StudentCode,
                                          StudentName = d.tbStudent.StudentName,
                                          ElectiveGroupName = p.tbElectiveGroup.ElectiveGroupName,
                                          HeadTeacherName = t.tbTeacher.TeacherName,
                                          ElectiveSectionName = p.tbElectiveSection.ElectiveSectionName
                                      }).ToList();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HeaderTeacher(Models.ElectiveReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("HeaderTeacher", new { ElectiveId = vm.ElectiveId, OrgId = vm.OrgId, SearchText = vm.SearchText }));
        }

        public ActionResult HeaderTeacherExport(Models.ElectiveReport.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                string file = System.IO.Path.GetTempFileName();

                var tb = from p in db.Table<Entity.tbElectiveOrg>()
                         where p.tbElective.Id == vm.ElectiveId
                         select p;

                var list = (from p in tb
                            join d in db.Table<Entity.tbElectiveData>() on p.Id equals d.tbElectiveOrg.Id
                            join c in db.Table<Basis.Entity.tbClassStudent>() on d.tbStudent.Id equals c.tbStudent.Id
                            join tc in db.Table<Basis.Entity.tbClass>() on c.tbClass.Id equals tc.Id
                            join t in db.Table<Basis.Entity.tbClassTeacher>() on tc.Id equals t.tbClass.Id //into result
                                                                                                           //from x in result.DefaultIfEmpty()
                            where p.tbElective.Id == vm.ElectiveId
                               && t.tbTeacher.tbSysUser.Id == Code.Common.UserId
                               && p.tbCourse.IsDeleted == false
                               && p.tbElectiveGroup.IsDeleted == false
                               && p.tbElectiveSection.IsDeleted == false
                               && d.IsDeleted == false
                            orderby d.tbStudent.StudentCode
                            select new Dto.ElectiveOrg.List()
                            {
                                OrgName = p.OrgName,
                                MaxCount = p.MaxCount,
                                CourseId = p.tbCourse.Id,
                                ClassName = tc.ClassName,
                                IsPreElective = d.IsPreElective,
                                RemainCount = p.RemainCount,
                                RoomName = p.tbRoom.RoomName,
                                CourseName = p.tbCourse.CourseName,
                                StudentCode = d.tbStudent.StudentCode,
                                StudentName = d.tbStudent.StudentName,
                                ElectiveGroupName = p.tbElectiveGroup.ElectiveGroupName,
                                HeadTeacherName = t.tbTeacher.TeacherName,
                                ElectiveSectionName = p.tbElectiveSection.ElectiveSectionName
                            }).ToList();



                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(
                    new string[] { "学号", "学生", "行政班级", "班主任", "班级", "分段", "分组", "课程", "是否预选" }.Select(p => new System.Data.DataColumn(p)).ToArray()
                    );
                foreach (var item in list)
                {
                    var index = 0;
                    var dr = dt.NewRow();
                    dr[index++] = item.StudentCode;
                    dr[index++] = item.StudentName;
                    dr[index++] = item.ClassName;
                    dr[index++] = item.HeadTeacherName;
                    dr[index++] = item.OrgName;
                    dr[index++] = item.ElectiveSectionName;
                    dr[index++] = item.ElectiveGroupName;
                    dr[index++] = item.CourseName;
                    dr[index++] = item.IsPreElective ? "是" : "否";
                    dt.Rows.Add(dr);
                    index = 0;
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

        #endregion



        public ActionResult ListExport(Models.ElectiveReport.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                string file = System.IO.Path.GetTempFileName();

                var tb = from p in db.Table<Entity.tbElectiveOrg>()
                         where p.tbElective.Id == vm.ElectiveId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.OrgName.Contains(vm.SearchText));
                }

                var yearId = Basis.Controllers.YearController.GetDefaultYearId(db);
                var list = (from p in tb
                            join d in db.Table<Entity.tbElectiveData>() on p.Id equals d.tbElectiveOrg.Id
                            join o in db.Table<Entity.tbElectiveOrgSchedule>() on d.tbElectiveOrg.Id equals o.tbElectiveOrg.Id into schedule
                            from s in schedule.DefaultIfEmpty()
                            join c in db.Table<Basis.Entity.tbClassStudent>() on d.tbStudent.Id equals c.tbStudent.Id
                            join tc in db.Table<Basis.Entity.tbClass>() on c.tbClass.Id equals tc.Id
                            join t in db.Table<Basis.Entity.tbClassTeacher>() on tc.Id equals t.tbClass.Id into result
                            from x in result.DefaultIfEmpty()
                            where c.tbClass.tbYear.Id== yearId
                            orderby d.InputDate descending
                            select new
                            {
                                Id = p.Id,
                                StudentCode = d.tbStudent.StudentCode,
                                StudentName = d.tbStudent.StudentName,
                                ClassName = c.tbClass.ClassName,
                                HeadTeacherName = x == null ? "-" : x.tbTeacher.TeacherName,
                                WeekName = s == null ? "-" : s.tbWeek.WeekName,
                                PeriodName = s == null ? "-" : s.tbPeriod.PeriodName,
                                OrgName = p.OrgName,
                                CourseName = p.tbCourse.CourseName,
                                MaxCount = p.MaxCount,
                                RemainCount = p.RemainCount,
                                TeacherName = p.tbTeacher.TeacherName,
                                ElectiveGroupName = p.tbElectiveGroup.ElectiveGroupName,
                                ElectiveSectionId = p.tbElectiveSection.Id,
                                ElectiveSectionName = p.tbElectiveSection.ElectiveSectionName,
                                RoomName = p.tbRoom.RoomName
                            }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(
                    new string[] { "学号", "姓名", "行政班", "班主任", "分段", "分组", "班级", "课程", "教室", "星期", "节次", "教师", "已选人数", "剩余人数", "总名额" }.Select(p => new System.Data.DataColumn(p)).ToArray()
                    );

                var dataList = (from p in db.Table<Entity.tbElectiveData>()
                                where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                    && p.tbStudent.IsDeleted == false
                                group p by p.tbElectiveOrg.Id into g
                                select new
                                {
                                    OrgId = g.Key,
                                    ElectiveCount = g.Count()
                                }).ToList();

                foreach (var item in list)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = item.StudentCode;
                    dr["姓名"] = item.StudentName;
                    dr["行政班"] = item.ClassName;
                    dr["班主任"] = item.HeadTeacherName;
                    dr["分段"] = item.ElectiveSectionName;
                    dr["分组"] = item.ElectiveGroupName;
                    dr["班级"] = item.OrgName;
                    dr["课程"] = item.CourseName;
                    dr["教室"] = item.RoomName;
                    dr["星期"] = item.WeekName;
                    dr["节次"] = item.PeriodName;
                    dr["教师"] = item.TeacherName;
                    dr["已选人数"] = dataList.Where(d => d.OrgId == item.Id).Select(d => d.ElectiveCount).DefaultIfEmpty().FirstOrDefault();
                    dr["剩余人数"] = item.RemainCount;
                    dr["总名额"] = item.MaxCount;
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

        /// <summary>
        /// 明细，所有选课学员列表
        /// </summary>
        /// <returns></returns>
        public ActionResult FullList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveReport.List();
                vm.ElectiveList = ElectiveController.SelectList();
                if (vm.ElectiveId == 0 && vm.ElectiveList.Count() > 0)
                {
                    vm.ElectiveId = vm.ElectiveList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.IsWeekPeriod = (from p in db.Table<Entity.tbElective>()
                                   where p.Id == vm.ElectiveId
                                   select p.tbElectiveType.ElectiveTypeCode).FirstOrDefault() == Code.EnumHelper.ElectiveType.WeekPeriod;

                //var classList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                //                 where p.tbClass.tbYear.IsDefault
                //                    && p.tbClass.IsDeleted == false
                //                    && p.tbClass.tbYear.IsDeleted == false
                //                 select new
                //                 {
                //                     StudentId = p.tbStudent.Id,
                //                     p.tbClass.ClassName
                //                 }).ToList();

                var yearId = Basis.Controllers.YearController.GetDefaultYearId(db);
                var tb = from p in db.Table<Entity.tbElectiveData>()
                         join c in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals c.tbStudent.Id
                         where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                            && c.tbClass.tbYear.Id==yearId
                            && p.tbElectiveOrg.tbCourse.IsDeleted == false
                            && p.tbElectiveOrg.tbElectiveGroup.IsDeleted == false
                            && p.tbElectiveOrg.tbElectiveSection.IsDeleted == false
                         select new { tbElectiveData = p, tbClass = c.tbClass };

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbElectiveData.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbElectiveData.tbStudent.StudentName.Contains(vm.SearchText));
                }


                vm.ElectiveOrgList = (from p in tb
                                      orderby p.tbElectiveData.InputDate descending
                                      select new Dto.ElectiveOrg.List()
                                      {
                                          Id = p.tbElectiveData.tbElectiveOrg.Id,
                                          OrgName = p.tbElectiveData.tbElectiveOrg.OrgName,
                                          CourseId = p.tbElectiveData.tbElectiveOrg.tbCourse.Id,
                                          CourseName = p.tbElectiveData.tbElectiveOrg.tbCourse.CourseName,
                                          TeacherName = p.tbElectiveData.tbElectiveOrg.tbTeacher.TeacherName,
                                          RoomName = p.tbElectiveData.tbElectiveOrg.tbRoom.RoomName,
                                          RemainCount = p.tbElectiveData.tbElectiveOrg.RemainCount,
                                          ElectiveSectionName = p.tbElectiveData.tbElectiveOrg.tbElectiveSection.ElectiveSectionName,
                                          ElectiveGroupName = p.tbElectiveData.tbElectiveOrg.tbElectiveGroup.ElectiveGroupName,
                                          InputDate = p.tbElectiveData.InputDate,
                                          StudentId = p.tbElectiveData.tbStudent.Id,
                                          StudentCode = p.tbElectiveData.tbStudent.StudentCode,
                                          StudentName = p.tbElectiveData.tbStudent.StudentName,
                                          IsPreElective = p.tbElectiveData.IsPreElective,
                                          MaxCount = p.tbElectiveData.tbElectiveOrg.MaxCount,
                                          ClassName = p.tbClass.ClassName
                                      }).ToPageList(vm.Page);

                if (vm.IsWeekPeriod)
                {
                    var orgIds = vm.ElectiveOrgList.Select(p => p.Id);
                    var electiveOrgSchedule = (from p in db.Table<Entity.tbElectiveOrgSchedule>().Include(p => p.tbElectiveOrg).Include(p => p.tbWeek).Include(p => p.tbPeriod) where orgIds.Contains(p.tbElectiveOrg.Id) select p).ToList();

                    vm.ElectiveOrgList.ForEach(p =>
                    {
                        var weekPeriodName = new List<string>();
                        electiveOrgSchedule.ForEach(c =>
                        {
                            if (p.Id == c.tbElectiveOrg.Id)
                            {
                                weekPeriodName.Add($"{c.tbWeek?.WeekName}{c.tbPeriod?.PeriodName}");
                            }
                        });
                        p.WeekPeriod = string.Join(",", weekPeriodName);
                    });
                }


                //foreach (var a in vm.ElectiveOrgList)
                //{
                //    a.ClassName = classList.Where(c => c.StudentId == a.StudentId).Select(c => c.ClassName).FirstOrDefault();
                //}

                if (vm.IsWeekPeriod)
                {
                    vm.IsHiddenSection = true;
                    vm.IsHiddenGroup = true;
                }
                else
                {
                    vm.IsHiddenSection = vm.ElectiveOrgList.Select(d => d.ElectiveSectionName).Distinct().Count() <= 1;
                    vm.IsHiddenGroup = vm.ElectiveOrgList.Select(d => d.ElectiveGroupName).Distinct().Count() <= 1;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FullList(Models.ElectiveReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("FullList", new { ElectiveId = vm.ElectiveId, SearchText = vm.SearchText, pageIndex = vm.Page.PageIndex, pageSize = vm.Page.PageSize }));
        }

        public ActionResult FullListExport(int ElectiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveReport.List();

                string file = System.IO.Path.GetTempFileName();

                vm.ElectiveList = ElectiveController.SelectList();

                if (vm.ElectiveId == 0 && vm.ElectiveList.Count > 0)
                {
                    vm.ElectiveId = vm.ElectiveList.FirstOrDefault().Value.ConvertToInt();
                }

                var classList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                 where p.tbClass.tbYear.IsDefault
                                    && p.tbClass.IsDeleted == false
                                    && p.tbClass.tbYear.IsDeleted == false
                                 select new
                                 {
                                     StudentId = p.tbStudent.Id,
                                     p.tbClass.ClassName
                                 }).ToList();

                var tb = from p in db.Table<Entity.tbElectiveData>()
                         where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                            && p.tbElectiveOrg.tbCourse.IsDeleted == false
                            && p.tbElectiveOrg.tbElectiveGroup.IsDeleted == false
                            && p.tbElectiveOrg.tbElectiveSection.IsDeleted == false
                         select p;

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }

                vm.ElectiveOrgList = (from p in tb
                                      orderby p.InputDate descending
                                      select new Dto.ElectiveOrg.List()
                                      {
                                          OrgName = p.tbElectiveOrg.OrgName,
                                          CourseId = p.tbElectiveOrg.tbCourse.Id,
                                          CourseName = p.tbElectiveOrg.tbCourse.CourseName,
                                          TeacherName = p.tbElectiveOrg.tbTeacher.TeacherName,
                                          RoomName = p.tbElectiveOrg.tbRoom.RoomName,
                                          RemainCount = p.tbElectiveOrg.RemainCount,
                                          ElectiveSectionName = p.tbElectiveOrg.tbElectiveSection.ElectiveSectionName,
                                          ElectiveGroupName = p.tbElectiveOrg.tbElectiveGroup.ElectiveGroupName,
                                          InputDate = p.InputDate,
                                          StudentId = p.tbStudent.Id,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName,
                                          IsPreElective = p.IsPreElective,
                                          MaxCount = p.tbElectiveOrg.MaxCount
                                      }).ToList();

                foreach (var a in vm.ElectiveOrgList)
                {
                    a.ClassName = classList.Where(c => c.StudentId == a.StudentId).Select(c => c.ClassName).FirstOrDefault();
                }

                var dt = new System.Data.DataTable();

                dt.Columns.AddRange(
                    new string[] { "分段", "分组", "选课班级", "课程", "学号", "学生", "行政班", "教师", "上课教室", "选课时间", "是否预选" }.Select(p => new System.Data.DataColumn(p)).ToArray()
                    );

                foreach (var a in vm.ElectiveOrgList)
                {
                    var dr = dt.NewRow();
                    dr["分段"] = a.ElectiveSectionName;
                    dr["分组"] = a.ElectiveGroupName;
                    dr["选课班级"] = a.OrgName;
                    dr["课程"] = a.CourseName;
                    dr["学号"] = a.StudentCode;
                    dr["学生"] = a.StudentName;
                    dr["行政班"] = a.ClassName;
                    dr["教师"] = string.IsNullOrWhiteSpace(a.TeacherName) ? "-" : a.TeacherName;
                    dr["上课教室"] = string.IsNullOrWhiteSpace(a.RoomName) ? "-" : a.RoomName;
                    dr["选课时间"] = a.InputDate;
                    dr["是否预选"] = a.IsPreElective ? "是" : "否";
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

        public ActionResult UnElective()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveReport.List();
                vm.UeType = vm.UeType == 0 ? 1 : vm.UeType;
                vm.UeType = (vm.UeType != 1 && vm.UeType != 2) ? 1 : vm.UeType;

                vm.ElectiveList = ElectiveController.SelectList();
                if (vm.ElectiveId == 0 && vm.ElectiveList.Count() > 0)
                {
                    vm.ElectiveId = vm.ElectiveList.FirstOrDefault().Value.ConvertToInt();
                }

                //班级
                var tbClassIds = (from p in db.Table<Entity.tbElectiveClass>()
                                  where p.tbElective.Id == vm.ElectiveId
                                  select p.tbClass.Id).ToList();
                if (vm.UeType == 2)
                {//详细列表
                    var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                             where p.tbStudent.IsDeleted == false
                             select p;
                    if (string.IsNullOrEmpty(vm.SearchText) == false)
                    {
                        tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                    }
                    //班级学生列表
                    var tbClassStudents = (from p in tb
                                           where tbClassIds.Contains(p.tbClass.Id)
                                           select new
                                           {
                                               No = p.No,
                                               GradeNo = p.tbClass.tbGrade.No,
                                               ClassNo = p.tbClass.No,
                                               ClassName = p.tbClass.ClassName,
                                               StudentId = p.tbStudent.Id,
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName,
                                               SexName = p.tbStudent.tbSysUser.tbSex.SexName
                                           }).ToList();

                    //已选学生
                    var tbElectiveDataStudentIds = (from p in db.Table<Entity.tbElectiveData>()
                                                    where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                                    select p.tbStudent.Id).Distinct().ToList();

                    vm.ElectiveStudentList = (from p in tbClassStudents
                                              where tbElectiveDataStudentIds.Contains(p.StudentId) == false
                                              orderby p.GradeNo, p.ClassNo, p.ClassName, p.StudentCode
                                              select new Areas.Basis.Dto.ClassStudent.List()
                                              {
                                                  No = p.No,
                                                  ClassName = p.ClassName,
                                                  StudentId = p.StudentId,
                                                  SexName = p.SexName,
                                                  StudentCode = p.StudentCode,
                                                  StudentName = p.StudentName
                                              }).ToList();
                }
                else
                {//汇总列表
                    var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                                .Include(p => p.tbElective)
                              where p.tbElective.Id == vm.ElectiveId
                              select p);
                    if (string.IsNullOrEmpty(vm.SearchText) == false)
                    {
                        tb = tb.Where(d => d.OrgName.Contains(vm.SearchText) || d.OrgName.Contains(vm.SearchText));
                    }

                    var tbElectiveClass = (from p in tb
                                           select new
                                           {
                                               ElectiveId = p.tbElective.Id,
                                               ElectiveOrgId = p.Id,
                                               OrgName = p.OrgName,
                                               TeacherName = p.tbTeacher.TeacherName,
                                               IsPermitClass = p.IsPermitClass
                                           }).ToList();

                    var limitClass = (from p in db.Table<Entity.tbElectiveOrgClass>().Include(p => p.tbElectiveOrg)
                                      where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                      select new
                                      {
                                          Id = p.tbElectiveOrg.Id,  //ElectiveOrgId
                                          ClassId = p.tbClass.Id,
                                          ClassName = p.tbClass.ClassName
                                      }).ToList();

                    var electiveClass = (from p in db.Table<Entity.tbElectiveClass>()
                                         where p.tbElective.Id == vm.ElectiveId
                                         select new
                                         {
                                             Id = p.tbElective.Id,      //ElectiveId
                                             ClassId = p.tbClass.Id,
                                             ClassName = p.tbClass.ClassName
                                         }).ToList();

                    var listClassIds = limitClass.Union(electiveClass).Select(p => p.ClassId).ToList();

                    var allClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                        where listClassIds.Contains(p.Id)
                                        join s in db.Table<Basis.Entity.tbClassStudent>() on p.Id equals s.tbClass.Id into student
                                        from r in student.DefaultIfEmpty()
                                        group r by new { p.Id, p.ClassName } into result
                                        select new
                                        {
                                            ClassId = result.Key.Id,
                                            ClassName = result.Key.ClassName,
                                            StudentCount = result.Count(r => r.tbClass != null)
                                        }).ToList();

                    //限定选课班级
                    var listPermitClass = (from p in tbElectiveClass.Where(p => p.IsPermitClass)
                                           join p1 in limitClass on p.ElectiveOrgId equals p1.Id    //ElectiveOrgId
                                           join p2 in allClassList on p1.ClassId equals p2.ClassId
                                           select new Dto.ElectiveReport.UnElectiveList()
                                           {
                                               OrgId = p.ElectiveOrgId,
                                               OrgName = p.OrgName,
                                               TeacherName = p.TeacherName,
                                               ClassId = p2.ClassId,
                                               ClassName = p2.ClassName,
                                               StudentNum = p2.StudentCount
                                           }).ToList();

                    //不限定选课班级
                    var listUnPermitClass = (from p in tbElectiveClass.Where(p => !p.IsPermitClass)
                                             join p1 in electiveClass on p.ElectiveId equals p1.Id  //ElectiveId
                                             join p2 in allClassList on p1.ClassId equals p2.ClassId
                                             select new Dto.ElectiveReport.UnElectiveList()
                                             {
                                                 OrgId = p.ElectiveOrgId,
                                                 OrgName = p.OrgName,
                                                 TeacherName = p.TeacherName,
                                                 ClassId = p2.ClassId,
                                                 ClassName = p2.ClassName,
                                                 StudentNum = p2.StudentCount
                                             }).ToList();

                    var listAll = listPermitClass.Union(listUnPermitClass).ToList();

                    //已选学生
                    var tbElectiveDataStudents = (from p in db.Table<Entity.tbElectiveData>()
                                                  join s in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals s.tbStudent.Id
                                                  where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                                  select new
                                                  {
                                                      ClassId = s.tbClass.Id,
                                                      OrgId = p.tbElectiveOrg.Id,
                                                      StudentId = p.tbStudent.Id
                                                  }).Distinct().ToList();
                    listAll.ForEach(p =>
                    {
                        p.ExistsStudentNum = p.StudentNum - tbElectiveDataStudents.Count(s => s.ClassId == p.ClassId && s.OrgId == p.OrgId);
                    });

                    vm.UnElectiveList = listAll;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnElective(Models.ElectiveReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("UnElective", new { ElectiveId = vm.ElectiveId, SearchText = vm.SearchText, UeType = vm.UeType }));
        }

        public ActionResult UnElectiveExport(int ElectiveId, int ClassId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveReport.List();
                var file = System.IO.Path.GetTempFileName();

                vm.ElectiveList = ElectiveController.SelectList();

                if (vm.ElectiveId == 0 && vm.ElectiveList.Count() > 0)
                {
                    vm.ElectiveId = vm.ElectiveList.FirstOrDefault().Value.ConvertToInt();
                }
                var tbClassIds = new List<int>();
                if (ClassId > 0)
                {
                    tbClassIds.Add(ClassId);
                }
                else
                {
                    tbClassIds = (from p in db.Table<Entity.tbElectiveClass>()
                                  where p.tbElective.Id == vm.ElectiveId && p.IsDeleted == false
                                  select p.tbClass.Id).ToList();
                }

                var tbClassStudents = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                       where tbClassIds.Contains(p.tbClass.Id) && p.IsDeleted == false
                                       select new
                                       {
                                           No = p.No,
                                           ClassName = p.tbClass.ClassName,
                                           StudentId = p.tbStudent.Id,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName,
                                           SexName = p.tbStudent.tbSysUser.tbSex.SexName
                                       }).ToList();

                var tbElectiveDataStudentIds = (from p in db.Table<Entity.tbElectiveData>()
                                                where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId && p.IsDeleted == false

                                                select p.tbStudent.Id).Distinct().ToList();


                vm.ElectiveStudentList = (from p in tbClassStudents
                                          where tbElectiveDataStudentIds.Contains(p.StudentId) == false
                                          select new Areas.Basis.Dto.ClassStudent.List()
                                          {
                                              No = p.No,
                                              ClassName = p.ClassName,
                                              StudentId = p.StudentId,
                                              SexName = p.SexName,
                                              StudentCode = p.StudentCode,
                                              StudentName = p.StudentName
                                          }).ToList();

                var dt = new System.Data.DataTable();

                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("座位号"),
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("班级"),
                        new System.Data.DataColumn("状态")
                    });

                foreach (var a in vm.ElectiveStudentList)
                {
                    var dr = dt.NewRow();
                    dr["座位号"] = a.No;
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["性别"] = a.SexName;
                    dr["班级"] = a.ClassName;
                    dr["状态"] = "未选课";
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt, "未选名单");

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

        /// <summary>
        /// 已选学员名单
        /// </summary>
        /// <param name="electiveOrgId"></param>
        /// <returns></returns>
        public ActionResult StudentList(int electiveOrgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveReport.Student();

                //var classList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                //                 where p.tbClass.tbYear.IsDefault
                //                    && p.tbClass.IsDeleted == false
                //                    && p.tbClass.tbYear.IsDeleted == false
                //                 select new
                //                 {
                //                     StudentId = p.tbStudent.Id,
                //                     p.tbClass.ClassName
                //                 }).ToList();

                vm.StudentList = (from p in db.Table<Entity.tbElectiveData>()
                                  join c in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals c.tbStudent.Id
                                  where p.tbElectiveOrg.Id == electiveOrgId
                                  orderby p.InputDate descending
                                  select new Dto.ElectiveReport.Student()
                                  {
                                      StudentId = p.tbStudent.Id,
                                      StudentName = p.tbStudent.StudentName,
                                      ClassName = c.tbClass.ClassName,
                                      StudentCode = p.tbStudent.StudentCode,
                                      InputDate = p.InputDate
                                  }).ToList();
                //foreach (var a in vm.StudentList)
                //{
                //    a.ClassName = classList.Where(c => c.StudentId == a.StudentId).Select(c => c.ClassName).FirstOrDefault();
                //}

                return View(vm);
            }
        }


        /// <summary>
        ///未选学员人数（根据行政班限制数计算）
        /// </summary>
        /// <param name="electiveOrgId"></param>
        /// <returns></returns>
        public ActionResult UnOrgClassList(int electiveOrgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveReport.UnOrgClass();
                vm.UnOrgClassList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                     where p.tbElectiveOrg.Id == electiveOrgId
                                     select new Dto.ElectiveReport.UnOrgClass()
                                     {
                                         MaxLimit = p.MaxLimit,
                                         RemainCount = p.RemainCount,
                                         ClassName = p.tbClass.ClassName
                                     }).ToList();
                return View(vm);
            }
        }


        /// <summary>
        /// 未选学员名单
        /// </summary>
        /// <param name="electiveOrgId"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        public ActionResult UnStudentList(int electiveOrgId, int classId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveReport.Student();

                var classList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                 where p.tbClass.tbYear.IsDefault
                                    && p.tbClass.IsDeleted == false
                                    && p.tbClass.tbYear.IsDeleted == false
                                    && p.tbStudent.IsDeleted == false
                                 select new
                                 {
                                     StudentId = p.tbStudent.Id,
                                     p.tbClass.ClassName
                                 }).ToList();

                //已选
                var exStudentId = (from p in db.Table<Entity.tbElectiveData>()
                                   where p.tbElectiveOrg.Id == electiveOrgId
                                   select p.tbStudent.Id).ToList();


                var tbOrg = db.Set<Entity.tbElectiveOrg>().Find(electiveOrgId);
                if (tbOrg.IsPermitClass)
                {
                    var studentList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                       join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                       where p.Id == electiveOrgId
                                       select new
                                       {
                                           OrgId = p.tbElectiveOrg.Id,
                                           ClassId = p.tbClass.Id,
                                           ClassName = p.tbClass.ClassName,
                                           StudentId = q.tbStudent.Id,
                                           StudentCode = q.tbStudent.StudentCode,
                                           StudentName = q.tbStudent.StudentName
                                       }).ToList();
                    if (classId > 0)
                    {
                        studentList = studentList.Where(p => p.ClassId == classId).ToList();
                    }

                    studentList.RemoveAll(p => exStudentId.Contains(p.StudentId));
                    vm.StudentList = studentList.Select(p => new Dto.ElectiveReport.Student()
                    {
                        ClassName = p.ClassName,
                        StudentCode = p.StudentCode,
                        StudentName = p.StudentName
                    }).ToList();
                    return View(vm);
                }
                else
                {
                    var studentList = (from p in db.Table<Entity.tbElectiveOrg>()
                                       join ec in db.Table<Entity.tbElectiveClass>() on p.tbElective.Id equals ec.tbElective.Id
                                       join s in db.Table<Basis.Entity.tbClassStudent>() on ec.tbClass.Id equals s.tbClass.Id
                                       where p.Id == electiveOrgId
                                       select new
                                       {
                                           OrgId = p.Id,
                                           ClassId = s.tbClass.Id,
                                           ClassName = s.tbClass.ClassName,
                                           StudentId = s.tbStudent.Id,
                                           StudentCode = s.tbStudent.StudentCode,
                                           StudentName = s.tbStudent.StudentName
                                       }).ToList();
                    if (classId > 0)
                    {
                        studentList = studentList.Where(p => p.ClassId == classId).ToList();
                    }
                    studentList.RemoveAll(p => exStudentId.Contains(p.StudentId));
                    vm.StudentList = studentList.Select(p => new Dto.ElectiveReport.Student()
                    {
                        ClassName = p.ClassName,
                        StudentCode = p.StudentCode,
                        StudentName = p.StudentName
                    }).ToList();
                    return View(vm);
                }
            }
        }

        /// <summary>
        /// 未选学生列表（按行政班汇总）
        /// </summary>
        /// <returns></returns>
        public ActionResult UnElectiveClass()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveReport.UnElectiveClassList();

                vm.ElectiveList = ElectiveController.SelectList();
                if (vm.ElectiveList == null || vm.ElectiveList.Count == 0)
                {
                    vm.NoMoralData = true;
                    return View(vm);
                }
                if (vm.ElectiveId <= 0)
                {
                    vm.ElectiveId = vm.ElectiveList.FirstOrDefault().Value.ConvertToInt();
                }

                //var tb1 = (from p in db.Table<Entity.tbElectiveOrg>()
                //           join ec in db.Table<Entity.tbElectiveClass>() on p.tbElective.Id equals ec.tbElective.Id
                //           join eo in db.Table<Entity.tbElectiveOrgClass>() on p.Id equals eo.tbElectiveOrg.Id
                //           join c in db.Table<Entity.tbElectiveData>() on p.Id equals c.tbElectiveOrg.Id into data
                //           from d in data.DefaultIfEmpty()
                //           select new
                //           {
                //               tbElectiveOrg = p,
                //               tbElectiveData = d
                //           }
                //        );

                var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>()
                    .Include(d => d.tbTeacher).Include(d => d.tbClass).ToList();
                var classStudentList = db.Table<Basis.Entity.tbClassStudent>()
                    .Include(d => d.tbStudent)
                    .Include(d => d.tbClass).ToList();

                var electiveDataList = (from p in db.Table<Entity.tbElectiveData>()
                                        join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals q.tbStudent.Id
                                        where p.tbStudent.IsDeleted == false
                                            && q.tbClass.IsDeleted == false
                                        group p by new { StudentId = p.tbStudent.Id, ClassId = q.tbClass.Id } into g
                                        select new
                                        {
                                            StudentId = g.Key.StudentId,
                                            ClassId = g.Key.ClassId
                                        }).ToList();

                var tb = from p in db.Table<Entity.tbElectiveClass>()
                         where p.tbClass.IsDeleted == false
                            && p.tbElective.Id == vm.ElectiveId
                         select p;
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbElective.ElectiveName.Contains(vm.SearchText));
                }

                vm.List = (from p in tb
                           orderby p.tbClass.No
                           group p by new { p.tbClass.Id, p.tbClass.No, p.tbClass.ClassName, GradeNo = p.tbClass.tbGrade.No } into g
                           orderby g.Key.GradeNo, g.Key.No
                           select new Dto.ElectiveReport.UnElectiveClassList()
                           {
                               Id = g.Key.Id,
                               No = g.Key.No,
                               ClassName = g.Key.ClassName
                           }).ToList();

                foreach (var v in vm.List)
                {
                    v.Num = classStudentList.Where(d => d.tbClass.Id == v.Id).Count();
                    v.UnNum = v.Num - electiveDataList.Where(d => d.ClassId == v.Id).Count();
                    if (classTeacherList.Where(d => d.tbClass.Id == v.Id).Any())
                    {
                        v.TeacherName = classTeacherList.Where(d => d.tbClass.Id == v.Id).FirstOrDefault().tbTeacher.TeacherName;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnElectiveClass(Models.ElectiveReport.UnElectiveClassList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("UnElectiveClass", new { SearchText = vm.SearchText, ElectiveId = vm.ElectiveId }));
        }

        public ActionResult UnElectiveClassStudent(int electiveId, int classId)
        {
            var vm = new Models.ElectiveReport.UnElectiveClassStudentList();
            vm.ClassId = classId;

            using (var db = new XkSystem.Models.DbContext())
            {
                var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>()
                    .Include(d => d.tbClass)
                   .Include(d => d.tbTeacher).ToList();
                var classStudentList = db.Table<Basis.Entity.tbClassStudent>()
                    .Include(d => d.tbStudent)
                    .Include(d => d.tbClass).Where(d => d.tbClass.Id == vm.ClassId);

                var electiveDataStudentIds = db.Table<Entity.tbElectiveData>().Where(d => d.tbElectiveOrg.tbElective.Id == electiveId)
                    .Select(d => d.tbStudent.Id).ToList();

                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    classStudentList = classStudentList.Where(p => p.tbStudent.StudentName.Contains(vm.SearchText) || p.tbStudent.StudentCode.Contains(vm.SearchText));
                }

                vm.List = (from p in classStudentList
                           where !electiveDataStudentIds.Contains(p.tbStudent.Id)
                           orderby p.No
                           select new Dto.ElectiveReport.UnElectiveClassStudentList()
                           {
                               Id = p.Id,
                               No = p.No,
                               ClassId = p.tbClass.Id,
                               ClassName = p.tbClass.ClassName,
                               StudentCode = p.tbStudent.StudentCode,
                               StudentName = p.tbStudent.StudentName,
                               StudentSex = p.tbStudent.tbSysUser.tbSex.SexName
                           }).ToPageList(vm.Page);

                foreach (var v in vm.List)
                {
                    if (classTeacherList.Where(d => d.tbClass.Id == v.ClassId).Any())
                    {
                        v.TeacherName = classTeacherList.Where(d => d.tbClass.Id == v.ClassId).FirstOrDefault().tbTeacher.TeacherName;
                    }
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnElectiveClassStudent(Models.ElectiveReport.UnElectiveClassStudentList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("UnElectiveClassStudent", new
            {
                SearchText = vm.SearchText,
                ClassId = vm.ClassId,
                ElectiveId = vm.ElectiveId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult UnElectiveStudent()
        {
            var vm = new Models.ElectiveReport.UnElectiveStudentList();

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.ElectiveList = ElectiveController.SelectList();
                if (vm.ElectiveId <= 0 && vm.ElectiveList.Count > 0)
                {
                    vm.ElectiveId = vm.ElectiveList.FirstOrDefault().Value.ConvertToInt();
                }

                var classIds = db.Table<Entity.tbElectiveClass>()
                    .Where(d => d.tbElective.Id == vm.ElectiveId)
                    .Select(d => d.tbClass.Id).ToList();
                var studentIds = db.Table<Entity.tbElectiveData>()
                    .Select(d => d.tbStudent.Id).ToList();
                var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>()
                    .Include(d => d.tbTeacher)
                    .Include(d => d.tbClass).ToList();


                var tb = (from p in db.Table<Basis.Entity.tbClassStudent>() select p);

                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(p => p.tbStudent.StudentName.Contains(vm.SearchText) || p.tbStudent.StudentCode.Contains(vm.SearchText));
                }


                vm.List = (from p in tb
                           where classIds.Contains(p.tbClass.Id) && !studentIds.Contains(p.tbStudent.Id)
                           orderby p.No
                           select new Dto.ElectiveReport.UnElectiveStudentList()
                           {
                               Id = p.Id,
                               No = p.No,
                               ClassId = p.tbClass.Id,
                               ClassName = p.tbClass.ClassName,
                               StudentCode = p.tbStudent.StudentCode,
                               StudentName = p.tbStudent.StudentName,
                               StudentSex = p.tbStudent.tbSysUser.tbSex.SexName
                           }).ToPageList(vm.Page);
                foreach (var v in vm.List)
                {
                    v.TeacherName = (from p in classTeacherList
                                     where p.tbClass.Id == v.ClassId
                                     select p.tbTeacher.TeacherName).FirstOrDefault();
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnElectiveStudent(Models.ElectiveReport.UnElectiveStudentList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("UnElectiveStudent", new { SearchText = vm.SearchText, ElectiveId = vm.ElectiveId, pageIndex = vm.Page.PageIndex, pageSize = vm.Page.PageSize }));
        }


        public ActionResult SolutionList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveReport.SolutionList();
                vm.ElectiveList = ElectiveController.SelectList();
                if (vm.ElectiveId == 0 && vm.ElectiveList.Count > 0)
                {
                    vm.ElectiveId = vm.ElectiveList.FirstOrDefault().Value.ConvertToInt();
                }

                var tb = (from p in db.Table<Entity.tbElectiveData>()
                          where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                            && p.tbElectiveOrg.IsDeleted == false
                            && p.tbStudent.IsDeleted == false
                          select new Models.ElectiveReport.SolutionList.mySolution
                          {
                              StudentId = p.tbStudent.Id,
                              OrgId = p.tbElectiveOrg.Id
                          }).ToList();
                var tc = (from p in tb
                          select new
                          {
                              p.StudentId
                          }).Distinct();
                var list = new List<Models.ElectiveReport.SolutionList.MyStudentGroup>();
                foreach (var r in tc)
                {
                    var model = new Models.ElectiveReport.SolutionList.MyStudentGroup
                    {
                        StudentId = r.StudentId
                    };
                    var tv = from y in tb.Where(c => c.StudentId == r.StudentId)
                             orderby y.OrgId
                             select y;
                    var strvalue = String.Empty;
                    foreach (var v in tv)
                    {
                        strvalue += v.OrgId + ",";
                    }

                    model.StrValue = strvalue;
                    list.Add(model);
                }

                vm.MySolutionList = (from l in list
                                     select new Models.ElectiveReport.SolutionList.MyStudentGroup
                                     {
                                         StudentId = l.StudentId,
                                         StrValue = l.StrValue
                                     }

                              into d
                                     group d by new
                                     {
                                         d.StrValue
                                     }

                                  into q
                                     select new Models.ElectiveReport.SolutionList.Solution
                                     {
                                         StrValue = Code.Common.DESEnCode(q.Key.StrValue),
                                         StudentId = (from j in q
                                                      where j.StrValue == q.Key.StrValue
                                                      select j.StudentId).FirstOrDefault(),
                                         StudentCount = (from c in q
                                                         where c.StrValue == q.Key.StrValue
                                                         select c.StudentId).Count()
                                     }).OrderByDescending(d => d.StudentCount).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SolutionList(Models.ElectiveReport.SolutionList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SolutionList", new { electiveId = vm.ElectiveId }));
        }

        public ActionResult SolutionStudent(int electiveId, string electiveOrgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveReport.SolutionStudent();
                electiveOrgId = Code.Common.DESDeCode(electiveOrgId);

                var tb = (from p in db.Table<Entity.tbElectiveData>()
                          where p.tbElectiveOrg.tbElective.Id == electiveId
                            && p.tbElectiveOrg.IsDeleted == false
                            && p.tbStudent.IsDeleted == false
                          select new Models.ElectiveReport.SolutionList.mySolution
                          {
                              StudentId = p.tbStudent.Id,
                              OrgId = p.tbElectiveOrg.Id
                          }).ToList();
                var tc = (from p in tb
                          select new
                          {
                              p.StudentId
                          }).Distinct();
                var list = new List<Models.ElectiveReport.SolutionList.MyStudentGroup>();
                foreach (var r in tc)
                {
                    var model = new Models.ElectiveReport.SolutionList.MyStudentGroup
                    {
                        StudentId = r.StudentId
                    };
                    var tv = from y in tb.Where(c => c.StudentId == r.StudentId)
                             orderby y.OrgId
                             select y;
                    var strvalue = String.Empty;
                    foreach (var v in tv)
                    {
                        strvalue += v.OrgId + ",";
                    }

                    model.StrValue = strvalue;
                    list.Add(model);
                }

                var studentList = list.Where(d => d.StrValue == electiveOrgId).Select(d => d.StudentId).ToList();

                var classList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                 where p.tbClass.tbYear.IsDefault
                                    && p.tbClass.IsDeleted == false
                                    && p.tbClass.tbYear.IsDeleted == false
                                 select new
                                 {
                                     StudentId = p.tbStudent.Id,
                                     p.tbClass.ClassName
                                 }).ToList();

                vm.StudentList = (from p in db.Table<Student.Entity.tbStudent>()
                                  where studentList.Contains(p.Id)
                                  orderby p.StudentCode
                                  select new Dto.ElectiveReport.Student()
                                  {
                                      StudentId = p.Id,
                                      StudentName = p.StudentName,
                                      StudentCode = p.StudentCode,
                                  }).ToList();
                foreach (var a in vm.StudentList)
                {
                    a.ClassName = classList.Where(c => c.StudentId == a.StudentId).Select(c => c.ClassName).FirstOrDefault();
                }

                return View(vm);
            }
        }
    }
}