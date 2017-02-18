using XkSystem.Code;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Attendance.Controllers
{
    public class AttendanceController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Attendance.List();
                //是否班主任
                vm.IsClassTeacher = db.Table<Basis.Entity.tbClassTeacher>().Where(d => d.tbTeacher.tbSysUser.Id == Code.Common.UserId && d.tbTeacher.IsDeleted == false && d.tbClass.IsDeleted == false).Count() > decimal.Zero ? true : false;
                //今天是星期几
                var weekInt = GetWeekNum(DateTime.Now.AddDays(vm.DayWeekId));
                //当前日期
                ViewBag.DateNow = DateTime.Now.AddDays(vm.DayWeekId).ToString("D");
                ViewBag.DayId = vm.DayWeekId;
                ViewBag.DayNow = DateTime.Now.AddDays(vm.DayWeekId).Day;
                var AttendanceDate = DateTime.Now.AddDays(vm.DayWeekId).Date;
                //获取星期Id
                var weekId = db.Table<Basis.Entity.tbWeek>().Where(d => d.No == weekInt).FirstOrDefault().Id;
                //考勤类型列表
                vm.AttendanceTypeList = AttendanceTypeController.SelectList();

                //教学班Id
                var orgIdList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                 where p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                     && p.tbOrg.IsDeleted == false
                                     && p.tbTeacher.IsDeleted == false
                                 // && p.tbOrg.tbYear.IsDefault == true
                                 orderby p.tbOrg.No
                                 select p.tbOrg.Id).ToList();

                vm.OrgScheduleList = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                                      where p.tbWeek.Id == weekId
                                        && orgIdList.Contains(p.tbOrg.Id)
                                        && p.tbOrg.IsDeleted == false
                                        && p.tbPeriod.IsDeleted == false
                                        && p.tbWeek.IsDeleted == false
                                      orderby p.tbPeriod.No
                                      select new Areas.Course.Dto.OrgSchedule.List
                                      {
                                          Id = p.Id,
                                          CourseName = p.tbOrg.tbCourse.CourseName,
                                          PeriodName = p.tbPeriod.PeriodName,
                                          OrgName = p.tbOrg.OrgName,
                                          PeriodId = p.tbPeriod.Id,
                                          CourseId = p.tbOrg.tbCourse.Id,
                                          OrgId = p.tbOrg.Id
                                      }).ToList();

                if (vm.OrgId == 0 && vm.OrgScheduleList.Count > 0)
                {
                    vm.OrgId = vm.OrgScheduleList.FirstOrDefault().OrgId;
                }

                if (vm.PeriodId == 0 && vm.OrgScheduleList.Count > 0)
                {
                    vm.PeriodId = vm.OrgScheduleList.FirstOrDefault().PeriodId;
                }

                vm.OrgSelectInfo = null;

                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                                .Include(d => d.tbClass)
                                .Include(d => d.tbCourse)
                                .Include(d => d.tbYear)
                             where p.Id == vm.OrgId
                             select p).FirstOrDefault();

                if (tbOrg != null)
                {
                    //准备考勤数据
                    var tb = from p in db.Table<Attendance.Entity.tbAttendance>()
                             where p.tbStudent.IsDeleted == false
                             && p.AttendanceDate.Day == AttendanceDate.Day
                             && p.tbOrg.Id == tbOrg.Id
                             select p;
                    var orgStudentList = new List<Dto.Attendance.List>();
                    if (tbOrg.IsClass)
                    {
                        if (tbOrg.tbClass != null)
                        {
                            orgStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                              where p.tbClass.Id == tbOrg.tbClass.Id
                                                  && p.tbStudent.IsDeleted == false
                                                  && (p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText))
                                              orderby p.No, p.tbStudent.StudentCode
                                              select new Dto.Attendance.List
                                              {
                                                  No = p.No,
                                                  StudentId = p.tbStudent.Id,
                                                  StudentCode = p.tbStudent.StudentCode,
                                                  StudentName = p.tbStudent.StudentName,
                                                  SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                                  CourseName = tbOrg.tbCourse.CourseName
                                              }).ToList();
                        }
                    }
                    else
                    {
                        orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                          where p.tbOrg.Id == vm.OrgId
                                             && p.tbStudent.IsDeleted == false
                                             && (p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText))
                                          orderby p.No, p.tbStudent.StudentCode
                                          select new Dto.Attendance.List
                                          {
                                              No = p.No,
                                              StudentId = p.tbStudent.Id,
                                              StudentCode = p.tbStudent.StudentCode,
                                              StudentName = p.tbStudent.StudentName,
                                              SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                              CourseName = tbOrg.tbCourse.CourseName
                                          }).ToList();
                    }

                    //准备考勤数据
                    vm.AttendanceAllList = (from p in db.Table<Attendance.Entity.tbAttendance>()
                                            where p.tbStudent.IsDeleted == false
                                                && p.AttendanceDate.Day == AttendanceDate.Day
                                            select new Dto.Attendance.List
                                            {
                                                AttendanceDate = p.AttendanceDate,
                                                AttendanceTypeName = p.tbAttendanceType.AttendanceTypeName,
                                                AttendanceTypeId = p.tbAttendanceType.Id,
                                                OrgId = p.tbOrg.Id,
                                                PeriodId = p.tbPeriod.Id,
                                            }).ToList();


                    #region 判断班级是否录入考勤
                    if (vm.OrgScheduleList.Any())
                    {
                        var orgIds = vm.OrgScheduleList.Select(p => p.OrgId);
                        var attendanceTeachers = (from p in db.Table<Entity.tbAttendanceTeacher>()
                                                  where p.tbTeacher.tbSysUser.Id == Common.UserId && orgIds.Contains(p.tbOrg.Id) && p.AttendanceDate == AttendanceDate
                                                  select p.tbOrg.Id).ToList();
                        vm.OrgScheduleList.ForEach(p => p.IsAttendance = attendanceTeachers.Any(t => t == p.OrgId));
                    }
                    #endregion

                    vm.AttendanceList = (from p in orgStudentList
                                         select new Dto.Attendance.List
                                         {
                                             Id = tb.Where(d => d.tbStudent.Id == p.StudentId && d.tbPeriod.Id == vm.PeriodId && d.tbOrg.Id == vm.OrgId && d.AttendanceDate.Day == AttendanceDate.Day).Select(d => d.Id).FirstOrDefault(),
                                             AttendanceDate = tb.Where(d => d.tbStudent.Id == p.StudentId && d.tbPeriod.Id == vm.PeriodId && d.tbOrg.Id == vm.OrgId && d.AttendanceDate.Day == AttendanceDate.Day).Select(d => d.AttendanceDate).FirstOrDefault(),
                                             CourseName = p.CourseName,
                                             InputDate = tb.Where(d => d.tbStudent.Id == p.StudentId && d.tbPeriod.Id == vm.PeriodId && d.tbOrg.Id == vm.OrgId && d.AttendanceDate.Day == AttendanceDate.Day).Select(d => d.InputDate).FirstOrDefault(),
                                             AttendanceTypeName = tb.Where(d => d.tbStudent.Id == p.StudentId && d.tbPeriod.Id == vm.PeriodId && d.tbOrg.Id == vm.OrgId && d.AttendanceDate.Day == AttendanceDate.Day).Select(d => d.tbAttendanceType.AttendanceTypeName).FirstOrDefault() != null ? tb.Where(d => d.tbStudent.Id == p.StudentId && d.tbPeriod.Id == vm.PeriodId && d.tbOrg.Id == vm.OrgId && d.AttendanceDate.Day == AttendanceDate.Day).Select(d => d.tbAttendanceType.AttendanceTypeName).FirstOrDefault() : "正常",
                                             AttendanceTypeId = tb.Where(d => d.tbStudent.Id == p.StudentId && d.tbPeriod.Id == vm.PeriodId && d.tbOrg.Id == vm.OrgId && d.AttendanceDate.Day == AttendanceDate.Day).Select(d => d.tbAttendanceType.Id).FirstOrDefault(),
                                             StudentName = p.StudentName,
                                             SexName = p.SexName,
                                             No = p.No,
                                             StudentCode = p.StudentCode,
                                             StudentId = p.StudentId,
                                             PeriodId = vm.PeriodId,
                                             PeriodName=p.PeriodName,
                                             OrgId = p.OrgId
                                         }).ToList();
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Attendance.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                DayWeekId = vm.DayWeekId,
                PeriodId = vm.PeriodId,
                OrgId = vm.OrgId
            }));
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.Attendance.Edit();
            using (var db = new XkSystem.Models.DbContext())
            {
                if (id > 0)
                {
                    var tb = (from p in db.Table<Attendance.Entity.tbAttendance>()
                              where p.Id == id
                              select new Dto.Attendance.Edit()
                              {
                                  Id = p.Id,
                                  StudentId = p.tbStudent.Id,
                                  StudentName = p.tbStudent.StudentName,
                                  PeriodId = p.tbPeriod.Id,
                                  OrgId = p.tbOrg.Id,
                                  AttendanceDate = p.AttendanceDate,
                                  AttendanceTypeId = p.tbAttendanceType.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.AttendanceEdit = tb;
                    }
                }

                var tbMobile = (from p in db.Table<Student.Entity.tbStudent>()
                                join sf in db.Table<Student.Entity.tbStudentFamily>() on p.Id equals sf.tbStudent.Id into Family
                                from f in Family.DefaultIfEmpty()
                                where p.Id == vm.StudentId
                                select new
                                {
                                    UserId = p.tbSysUser.Id,
                                    UserName = p.StudentName,
                                    StudentMobile = p.tbSysUser.Mobile,
                                    KinshipName = f != null ? f.tbDictKinship.KinshipName : "",
                                    FamilyMobile = f != null ? f.Mobile : "",
                                }).ToList();
                vm.MobileList.Add(new Models.Attendance.SmsUserInfo()
                {
                    Mobile = tbMobile.FirstOrDefault().StudentMobile,
                    UserName = tbMobile.FirstOrDefault().UserName
                });
                vm.MobileList.AddRange(tbMobile.Where(p => !string.IsNullOrWhiteSpace(p.FamilyMobile)).Select(p => new Models.Attendance.SmsUserInfo()
                {
                    Mobile = p.FamilyMobile,
                    UserName = p.KinshipName
                }).ToList());
            }

            vm.AttendanceTypeList = AttendanceTypeController.SelectAbnormalList();
            vm.SmsTemplet = Sms.Controllers.SmsTempletController.GetTempletByType(Code.EnumHelper.SmsTempletType.Attencance);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Attendance.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var attendanceDate = DateTime.Now.AddDays(vm.DayWeekId);
                var studentName = string.Empty;
                var attendanceTypeName = string.Empty;

                if (vm.AttendanceEdit.Id > 0)
                {
                    var tb = (from p in db.Table<Attendance.Entity.tbAttendance>().Include(p => p.tbStudent) where p.Id == vm.AttendanceEdit.Id select p).FirstOrDefault();
                    studentName = tb.tbStudent.StudentName;
                    tb.tbAttendanceType = db.Set<Attendance.Entity.tbAttendanceType>().Find(vm.AttendanceEdit.AttendanceTypeId);
                    attendanceTypeName = tb.tbAttendanceType.AttendanceTypeName;
                }
                else
                {
                    var tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                    studentName = tbStudent.StudentName;
                    var tbAttendanceType = db.Set<Attendance.Entity.tbAttendanceType>().Find(vm.AttendanceEdit.AttendanceTypeId);
                    attendanceTypeName = tbAttendanceType.AttendanceTypeName;

                    var tbAttendance = new Attendance.Entity.tbAttendance()
                    {
                        tbAttendanceType = tbAttendanceType,
                        AttendanceDate = attendanceDate,
                        InputDate = DateTime.Now,
                        tbOrg = db.Set<Course.Entity.tbOrg>().Find(vm.OrgId),
                        tbPeriod = db.Set<Basis.Entity.tbPeriod>().Find(vm.PeriodId),
                        tbStudent = tbStudent,
                        tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId)
                    };
                    db.Set<Attendance.Entity.tbAttendance>().Add(tbAttendance);
                }

                var mobileList = string.IsNullOrWhiteSpace(vm.Mobiles)?new List<string>():vm.Mobiles.Split(',').Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
                if (mobileList != null && mobileList.Any())
                {
                    //短信
                    var weekId = (int)attendanceDate.DayOfWeek;
                    weekId = weekId == 0 ? 7 : weekId++;

                    var tbOrg = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                                 where p.tbOrg.Id == vm.OrgId && p.tbPeriod.Id == vm.PeriodId && p.tbWeek.Id == weekId
                                 select new
                                 {
                                     CourseName = p.tbOrg.tbCourse.CourseName,
                                     PeriodName = p.tbPeriod.PeriodName,
                                 }).FirstOrDefault();

                    //短信内容
                    var smsContext = vm.SmsTemplet.Replace("{StudentName}", studentName)
                        .Replace("{CourseName}", tbOrg.CourseName)
                        .Replace("{Date}", attendanceDate.ToString(Code.Common.StringToDate))
                        .Replace("{PeriodName}", tbOrg.PeriodName)
                        .Replace("{AttendanceTypeName}", attendanceTypeName);

                    mobileList.ForEach(p =>
                    {
                        var tbSms = new Sms.Entity.tbSms()
                        {
                            SmsTitle = smsContext,
                            tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                            InputDate=DateTime.Now,
                            PlanDate =DateTime.Now
                        };

                        db.Set<Sms.Entity.tbSms>().Add(tbSms);

                        var tbSmsTo = new Sms.Entity.tbSmsTo()
                        {
                            Remark="考勤通知短信",
                            SendDate=DateTime.Now,
                            Mobile=p,
                            tbSms=tbSms,
                            Status=0
                        };
                        db.Set<Sms.Entity.tbSmsTo>().Add(tbSmsTo);

                    });
                }

                if (db.SaveChanges() > 0)
                {
                    Sys.Controllers.SysUserLogController.Insert("录入了学生考勤！");
                }

            }
            return MvcHelper.Post();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Models.Attendance.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                string[] txtPeriodId = null;
                string[] txtOrgId = null;
                string[] txtWeekDay = null;
                try
                {
                    txtPeriodId = Request["txtPeriodId"].Split(',');
                    txtOrgId = Request["txtOrgId"].Split(',');
                    txtWeekDay = Request["txtWeekDay"].Split(',');
                }
                catch
                {
                    return Code.MvcHelper.Post(null, Url.Action("List"), "暂无数据!");
                }

                var attendanceDate = DateTime.Today.AddDays(txtWeekDay[0].ConvertToInt());
                if (db.Table<Attendance.Entity.tbAttendanceTeacher>().Where(d => d.AttendanceDate == attendanceDate && d.tbOrg.Id == vm.OrgId && d.tbTeacher.tbSysUser.Id == Code.Common.UserId).Any() == false)
                {
                    var attendanceTeacher = new Attendance.Entity.tbAttendanceTeacher();
                    attendanceTeacher.AttendanceDate = attendanceDate;
                    attendanceTeacher.tbOrg = db.Set<Course.Entity.tbOrg>().Find(vm.OrgId);
                    attendanceTeacher.tbTeacher = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.tbSysUser.Id == Code.Common.UserId).FirstOrDefault();
                    attendanceTeacher.InputDate = DateTime.Now;
                    db.Set<Attendance.Entity.tbAttendanceTeacher>().Add(attendanceTeacher);
                }
                db.SaveChanges();
                return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, DayWeekId = txtWeekDay[0].ConvertToInt(), PeriodId = txtPeriodId[0].ConvertToInt(), OrgId = txtOrgId[0].ConvertToInt() }), "提交成功!");
            }
        }

        /// <summary>
        /// 获取星期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetWeekNum(DateTime date)
        {
            string strDayOfWeek = date.DayOfWeek.ToString();
            int intWeekday = -1;
            switch (strDayOfWeek)
            {
                case "Monday":
                    intWeekday = 1;
                    break;
                case "Tuesday":
                    intWeekday = 2;
                    break;
                case "Wednesday":
                    intWeekday = 3;
                    break;
                case "Thursday":
                    intWeekday = 4;
                    break;
                case "Friday":
                    intWeekday = 5;
                    break;
                case "Saturday":
                    intWeekday = 6;
                    break;
                case "Sunday":
                    intWeekday = 7;
                    break;
                default:
                    intWeekday = -1;
                    break;
            }
            return intWeekday;
        }

        /// <summary>
        /// 考勤统计
        /// </summary>
        /// <returns></returns>
        public ActionResult AttendanceAll()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Attendance.AttendanceAll();
                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("yyyy-MM-dd");
                }

                if (string.IsNullOrEmpty(vm.DateSearchTo))
                {
                    vm.DateSearchTo = DateTime.Now.ToString("yyyy-MM-dd");
                }

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);

                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                var gList = (from p in db.Table<Attendance.Entity.tbAttendance>()
                             where p.AttendanceDate >= fromDate && p.AttendanceDate < toDate
                             && p.tbAttendanceType.AttendanceValue != 0
                             && p.tbStudent.IsDeleted == false
                             group p by p.tbAttendanceType.Id into g
                             select new
                             {
                                 TypeKey = g.Key,
                                 CountNum = g.Count()
                             }).ToList();

                var tbAttendanceType = (from p in db.Table<Attendance.Entity.tbAttendanceType>()
                                        select new 
                                        {
                                            Id = p.Id,
                                            AttendanceValue = p.AttendanceValue,
                                            AttendanceTypeName = p.AttendanceTypeName
                                        }).ToList();

                vm.AttendanceInfoList = (from p in gList
                                         orderby p.CountNum descending
                                         select new Dto.Attendance.Info
                                         {
                                             AttendanceTypeName = tbAttendanceType.Where(d => d.Id == p.TypeKey).FirstOrDefault().AttendanceTypeName,
                                             CountNum = p.CountNum
                                         }).ToList();

                vm.AttendanceTypeList = AttendanceTypeController.SelectAbnormalList();

                vm.ClassList = Areas.Course.Controllers.OrgController.SelectList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    vm.ClassList = (from p in vm.ClassList
                                    where p.Text.Contains(vm.SearchText)
                                    select p).ToList();
                }
                var tbOrgList = (from p in db.Table<Course.Entity.tbOrg>()
                                 .Include(d => d.tbClass)
                                 .Include(d => d.tbCourse)
                                 select p).ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbOrgList = (from p in tbOrgList
                                 where p.OrgName.Contains(vm.SearchText)
                                 select p).ToList();
                }


                var orgIdLists = (from p in tbOrgList
                                  select p.Id).ToList();


                var OrgScheduleList = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                                       where orgIdLists.Contains(p.tbOrg.Id)
                                         && p.tbOrg.IsDeleted == false
                                         && p.tbPeriod.IsDeleted == false
                                         && p.tbWeek.IsDeleted == false
                                       orderby p.tbPeriod.No
                                       select new Areas.Course.Dto.OrgSchedule.List
                                       {
                                           PeriodId = p.tbPeriod.Id,
                                           CourseId = p.tbOrg.tbCourse.Id,
                                           OrgId = p.tbOrg.Id
                                       }).Distinct().ToList();


                var OrgStudentList = new List<Dto.Attendance.List>();

                foreach (var org in tbOrgList)
                {
                    if (org.IsClass)
                    {
                        var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                                 where p.tbClass.Id == org.tbClass.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Dto.Attendance.List
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           ClassName = p.tbClass.ClassName,
                                           No = org.tbCourse.Id,
                                           PeriodId = 0,
                                           OrgId = org.Id
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            OrgStudentList.AddRange(student);
                        }
                    }
                    else
                    {
                        var tb = from p in db.Table<Course.Entity.tbOrgStudent>()
                                 where p.tbOrg.Id == org.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbOrg.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Dto.Attendance.List
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           ClassName = p.tbOrg.tbClass.ClassName,
                                           No = org.tbCourse.Id,
                                           PeriodId = 0,
                                           OrgId = org.Id
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            OrgStudentList.AddRange(student);
                        }
                    }
                }

                var OrgStudentListN = (from p in OrgScheduleList
                                       join m in OrgStudentList on p.OrgId equals m.OrgId
                                       select new
                                       {
                                           Id = p.Id,
                                           StudentId = m.StudentId,
                                           StudentName = m.StudentName,
                                           StudentCode = m.StudentCode,
                                           ClassName = m.ClassName,
                                           No = m.Id,
                                           PeriodId = p.PeriodId,
                                           CourseId = p.CourseId,
                                           OrgId = p.OrgId
                                       }).Distinct().ToList();

                var tbAttendance = (from p in db.Table<Attendance.Entity.tbAttendance>()
                                    where p.AttendanceDate >= fromDate && p.AttendanceDate < toDate
                                    && p.tbAttendanceType.AttendanceValue != 0
                                    && p.tbStudent.IsDeleted == false
                                    select new Dto.Attendance.Info
                                    {
                                        OrgId = p.tbOrg.Id,
                                        PeriodId = p.tbPeriod.Id,
                                        StudentId = p.tbStudent.Id,
                                        AttendanceTypeId = p.tbAttendanceType.Id
                                    }).ToList();

                var fff = new List<Dto.Attendance.Info>();
                foreach (var c in tbAttendance)
                {
                    var ff = (from p in OrgStudentListN
                              where p.StudentId == c.StudentId && p.OrgId == c.OrgId && p.PeriodId == c.PeriodId
                              select p.OrgId).ToList().FirstOrDefault();

                    var dd = new Dto.Attendance.Info();
                    dd.ClassId = ff;
                    dd.AttendanceTypeId = c.AttendanceTypeId;
                    fff.Add(dd);
                }


                var gClassList = from p in fff
                                 group p by new { p.ClassId, p.AttendanceTypeId } into g
                                 select new
                                 {
                                     TypeKey = g.Key,
                                     CountNum = g.Count()
                                 };

                vm.AttendanceInfoClassList = (from p in gClassList
                                              orderby p.CountNum descending
                                              select new Dto.Attendance.Info
                                              {
                                                  ClassId = p.TypeKey.ClassId,
                                                  AttendanceTypeId = p.TypeKey.AttendanceTypeId,
                                                  CountNum = p.CountNum
                                              }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AttendanceAll(Models.Attendance.AttendanceAll vm)
        {
            var error = new List<string>();
            return Code.MvcHelper.Post(error, Url.Action("AttendanceAll", new
            {
                searchText = vm.SearchText,
                DateSearchFrom = vm.DateSearchFrom,
                DateSearchTo = vm.DateSearchTo,
            }));
        }

        public ActionResult AttendanceAllExport(string DateSearchFrom, string DateSearchTo, int YearId, string SearchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                if (string.IsNullOrEmpty(DateSearchFrom))
                {
                    DateSearchFrom = DateTime.Now.ToString("yyyy-MM-dd");
                }

                if (string.IsNullOrEmpty(DateSearchTo))
                {
                    DateSearchTo = DateTime.Now.ToString("yyyy-MM-dd");
                }

                var fromDate = Convert.ToDateTime(DateSearchFrom);

                var toDate = Convert.ToDateTime(DateSearchTo).AddDays(1);

                var YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);

                if (YearId == 0 && YearList.Count > 0)
                {
                    YearId = YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                var AttendanceTypeList = AttendanceTypeController.SelectAbnormalList();

                var ClassList = Areas.Course.Controllers.OrgController.SelectOrgList(YearId);

                if (string.IsNullOrEmpty(SearchText) == false)
                {
                    ClassList = (from p in ClassList
                                 where p.Text.Contains(SearchText)
                                 select p).ToList();
                }
                var tbOrgList = (from p in db.Table<Course.Entity.tbOrg>()
                                 .Include(d => d.tbClass)
                                 .Include(d => d.tbCourse)
                                 where p.tbYear.Id == YearId
                                 select p).ToList();

                if (string.IsNullOrEmpty(SearchText) == false)
                {
                    tbOrgList = (from p in tbOrgList
                                 where p.OrgName.Contains(SearchText)
                                 select p).ToList();
                }

                var orgIdLists = (from p in tbOrgList
                                  select p.Id).ToList();

                var OrgScheduleList = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                                       where orgIdLists.Contains(p.tbOrg.Id)
                                         && p.tbOrg.IsDeleted == false
                                         && p.tbPeriod.IsDeleted == false
                                         && p.tbWeek.IsDeleted == false
                                       orderby p.tbPeriod.No
                                       select new Areas.Course.Dto.OrgSchedule.List
                                       {
                                           PeriodId = p.tbPeriod.Id,
                                           CourseId = p.tbOrg.tbCourse.Id,
                                           OrgId = p.tbOrg.Id
                                       }).Distinct().ToList();


                var OrgStudentList = new List<Dto.Attendance.List>();

                foreach (var org in tbOrgList)
                {
                    if (org.IsClass)
                    {
                        var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                                 where p.tbClass.Id == org.tbClass.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Dto.Attendance.List
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           ClassName = p.tbClass.ClassName,
                                           No = org.tbCourse.Id,
                                           PeriodId = 0,
                                           OrgId = org.Id
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            OrgStudentList.AddRange(student);
                        }
                    }
                    else
                    {
                        var tb = from p in db.Table<Course.Entity.tbOrgStudent>()
                                 where p.tbOrg.Id == org.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbOrg.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Dto.Attendance.List
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           ClassName = p.tbOrg.tbClass.ClassName,
                                           No = org.tbCourse.Id,
                                           PeriodId = 0,
                                           OrgId = org.Id
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            OrgStudentList.AddRange(student);
                        }
                    }
                }

                var OrgStudentListN = (from p in OrgScheduleList
                                       join m in OrgStudentList on p.OrgId equals m.OrgId
                                       select new
                                       {
                                           Id = p.Id,
                                           StudentId = m.StudentId,
                                           StudentName = m.StudentName,
                                           StudentCode = m.StudentCode,
                                           ClassName = m.ClassName,
                                           No = m.Id,
                                           PeriodId = p.PeriodId,
                                           CourseId = p.CourseId,
                                           OrgId = p.OrgId
                                       }).Distinct().ToList();

                var tbAttendance = (from p in db.Table<Attendance.Entity.tbAttendance>()
                                    where p.AttendanceDate >= fromDate && p.AttendanceDate < toDate
                                    && p.tbAttendanceType.AttendanceValue != 0
                                    && p.tbStudent.IsDeleted == false
                                    select new Dto.Attendance.Info
                                    {
                                        OrgId = p.tbOrg.Id,
                                        PeriodId = p.tbPeriod.Id,
                                        StudentId = p.tbStudent.Id,
                                        AttendanceTypeId = p.tbAttendanceType.Id
                                    }).ToList();

                var fff = new List<Dto.Attendance.Info>();
                foreach (var c in tbAttendance)
                {
                    var ff = (from p in OrgStudentListN
                              where p.StudentId == c.StudentId && p.OrgId == c.OrgId && p.PeriodId == c.PeriodId
                              select p.OrgId).ToList().FirstOrDefault();

                    var dd = new Dto.Attendance.Info();
                    dd.ClassId = ff;
                    dd.AttendanceTypeId = c.AttendanceTypeId;
                    fff.Add(dd);
                }

                var gClassList = from p in fff
                                 group p by new { p.ClassId, p.AttendanceTypeId } into g
                                 select new
                                 {
                                     TypeKey = g.Key,
                                     CountNum = g.Count()
                                 };

                var AttendanceInfoClassList = (from p in gClassList
                                               orderby p.CountNum descending
                                               select new Dto.Attendance.Info
                                               {
                                                   ClassId = p.TypeKey.ClassId,
                                                   AttendanceTypeId = p.TypeKey.AttendanceTypeId,
                                                   CountNum = p.CountNum
                                               }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("班级名称")
                    });

                foreach (var a in AttendanceTypeList)
                {
                    dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn(a.Text)
                    });
                }

                foreach (var a in ClassList)
                {
                    var dr = dt.NewRow();
                    dr["班级名称"] = a.Text;
                    foreach (var item in AttendanceTypeList)
                    {
                        var schedule = AttendanceInfoClassList.Where(d => d.ClassId.ToString() == a.Value && d.AttendanceTypeId.ToString() == item.Value);
                        if (schedule.Count() > 0)
                        {
                            dr[item.Text] = schedule.FirstOrDefault().CountNum.ToString();
                        }
                        else
                        {
                            dr[item.Text] = "";
                        }
                    }
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
        /// 考勤统计
        /// </summary>
        /// <returns></returns>
        public ActionResult AttendanceDetail()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Attendance.AttendanceDetail();
                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString(XkSystem.Code.Common.StringToDate);
                }

                if (string.IsNullOrEmpty(vm.DateSearchTo))
                {
                    vm.DateSearchTo = DateTime.Today.ToString(XkSystem.Code.Common.StringToDate);
                }

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);
                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);

                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                var tbOrgList = (from p in db.Table<Course.Entity.tbOrg>()
                                 .Include(d => d.tbClass)
                                 .Include(d => d.tbCourse)
                                 where p.tbYear.Id == vm.YearId
                                 select p).ToList();

                var OrgStudentList = new List<Course.Dto.OrgStudent.List>();

                foreach (var org in tbOrgList)
                {
                    if (org.IsClass)
                    {
                        var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                                 where p.tbClass.Id == org.tbClass.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Course.Dto.OrgStudent.List
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           ClassName = p.tbClass.ClassName,
                                           No = org.tbCourse.Id,
                                           OrgName = org.OrgName
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            OrgStudentList.AddRange(student);
                        }
                    }
                    else
                    {
                        var tb = from p in db.Table<Course.Entity.tbOrgStudent>()
                                 where p.tbOrg.Id == org.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbOrg.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Course.Dto.OrgStudent.List
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           ClassName = p.tbOrg.tbClass.ClassName,
                                           No = org.tbCourse.Id,
                                           OrgName = org.OrgName
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            OrgStudentList.AddRange(student);
                        }
                    }
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    OrgStudentList = (from p in OrgStudentList
                                      where p.StudentCode.Contains(vm.SearchText) || p.StudentName.Contains(vm.SearchText)
                                      select p).ToList();
                }

                var AttendanceList = (from p in db.Table<Attendance.Entity.tbAttendance>()
                                      where p.AttendanceDate >= fromDate && p.AttendanceDate < toDate
                                      && p.tbAttendanceType.AttendanceValue != 0
                                      && p.tbStudent.IsDeleted == false
                                      orderby p.AttendanceDate.Day descending, p.tbPeriod.No ascending, p.tbStudent.StudentCode ascending
                                      select new Dto.Attendance.Detail
                                      {
                                          AttendanceTypeName = p.tbAttendanceType.AttendanceTypeName,
                                          AttendanceDate = p.AttendanceDate,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName,
                                          ClassName = "",
                                          OrgId = p.tbStudent.Id,
                                          OrgName = p.tbOrg.OrgName,
                                          PeriodName = p.tbPeriod.PeriodName
                                      }).ToList();
                var performList = new List<Dto.Attendance.Detail>();
                foreach (var p in AttendanceList)
                {
                    var item = (from m in OrgStudentList
                                where m.StudentId == p.OrgId
                                select m).ToList().FirstOrDefault();
                    if (item != null)
                    {
                        var perform = new Dto.Attendance.Detail();
                        perform.AttendanceTypeName = p.AttendanceTypeName;
                        perform.AttendanceDate = p.AttendanceDate;
                        perform.StudentCode = p.StudentCode;
                        perform.StudentName = p.StudentName;
                        perform.ClassName = item.ClassName;
                        perform.OrgId = item.Id;
                        perform.OrgName = item.OrgName;
                        perform.PeriodName = p.PeriodName;
                        performList.Add(perform);
                    }
                }
                vm.AttendanceDetailList = performList;
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AttendanceDetail(Models.Attendance.AttendanceAll vm)
        {
            var error = new List<string>();
            return Code.MvcHelper.Post(error, Url.Action("AttendanceDetail", new
            {
                searchText = vm.SearchText,
                DateSearchFrom = vm.DateSearchFrom,
                DateSearchTo = vm.DateSearchTo,
            }));
        }

        public ActionResult AttendanceDetailExport(string DateSearchFrom, string DateSearchTo, int YearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                if (string.IsNullOrEmpty(DateSearchFrom))
                {
                    DateSearchFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString(XkSystem.Code.Common.StringToDate);
                }

                if (string.IsNullOrEmpty(DateSearchTo))
                {
                    DateSearchTo = DateTime.Today.ToString(XkSystem.Code.Common.StringToDate);
                }

                var fromDate = Convert.ToDateTime(DateSearchFrom);
                var toDate = Convert.ToDateTime(DateSearchTo).AddDays(1);

                var YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);

                if (YearId == 0 && YearList.Count > 0)
                {
                    YearId = YearList.FirstOrDefault().Value.ConvertToInt();
                }

                var tbOrgList = (from p in db.Table<Course.Entity.tbOrg>()
                                 .Include(d => d.tbClass)
                                 .Include(d => d.tbCourse)
                                 where p.tbYear.Id == YearId
                                 select p).ToList();

                var OrgStudentList = new List<Course.Dto.OrgStudent.List>();

                foreach (var org in tbOrgList)
                {
                    if (org.IsClass)
                    {
                        var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                                 where p.tbClass.Id == org.tbClass.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Course.Dto.OrgStudent.List
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           ClassName = p.tbClass.ClassName,
                                           No = org.tbCourse.Id,
                                           OrgName = org.OrgName
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            OrgStudentList.AddRange(student);
                        }
                    }
                    else
                    {
                        var tb = from p in db.Table<Course.Entity.tbOrgStudent>()
                                 where p.tbOrg.Id == org.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbOrg.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Course.Dto.OrgStudent.List
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           ClassName = p.tbOrg.tbClass.ClassName,
                                           No = org.tbCourse.Id,
                                           OrgName = org.OrgName
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            OrgStudentList.AddRange(student);
                        }
                    }
                }

                var AttendanceList = (from p in db.Table<Attendance.Entity.tbAttendance>()
                                      where p.AttendanceDate >= fromDate && p.AttendanceDate < toDate
                                      && p.tbAttendanceType.AttendanceValue != 0
                                      && p.tbStudent.IsDeleted == false
                                      orderby p.AttendanceDate.Day descending, p.tbPeriod.No ascending, p.tbStudent.StudentCode ascending
                                      select new Dto.Attendance.Detail
                                      {
                                          AttendanceTypeName = p.tbAttendanceType.AttendanceTypeName,
                                          AttendanceDate = p.AttendanceDate,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName,
                                          ClassName = "",
                                          OrgId = p.tbStudent.Id,
                                          OrgName = p.tbOrg.OrgName,
                                          PeriodName = p.tbPeriod.PeriodName
                                      }).ToList();
                var performList = new List<Dto.Attendance.Detail>();
                foreach (var p in AttendanceList)
                {
                    var item = (from m in OrgStudentList
                                where m.StudentId == p.OrgId
                                select m).ToList().FirstOrDefault();
                    var perform = new Dto.Attendance.Detail();
                    perform.AttendanceTypeName = p.AttendanceTypeName;
                    perform.AttendanceDate = p.AttendanceDate;
                    perform.StudentCode = p.StudentCode;
                    perform.StudentName = p.StudentName;
                    perform.ClassName = item.ClassName;
                    perform.OrgId = item.Id;
                    perform.OrgName = item.OrgName;
                    perform.PeriodName = p.PeriodName;
                    performList.Add(perform);
                }

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("行政班"),
                        new System.Data.DataColumn("教学班"),
                        new System.Data.DataColumn("考勤日期"),
                        new System.Data.DataColumn("节次"),
                        new System.Data.DataColumn("考勤类型")
                    });

                foreach (var a in performList)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["行政班"] = a.ClassName;
                    dr["教学班"] = a.OrgName;
                    dr["考勤日期"] = a.AttendanceDate.ToString("D");
                    dr["节次"] = a.PeriodName;
                    dr["考勤类型"] = a.AttendanceTypeName;
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
        /// 班主任考
        /// </summary>
        /// <returns></returns>
        public ActionResult TeacherAll()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Attendance.TeacherAll();
                var teacherId = (from p in db.Table<Teacher.Entity.tbTeacher>()
                                 where p.tbSysUser.Id == Code.Common.UserId
                                 select p.Id).FirstOrDefault();
                vm.ClassList = Areas.Basis.Controllers.ClassController.SelectClassByTeacherList(0, teacherId);
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.StudentInfoList = Areas.Student.Controllers.StudentController.SelectStudentInfoList(vm.ClassId, vm.SearchText);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeacherAll(Models.Attendance.TeacherAll vm)
        {
            var error = new List<string>();
            return Code.MvcHelper.Post(error, Url.Action("TeacherAll", "Attendance", new
            {
                searchText = vm.SearchText,
                ClassId = vm.ClassId
            }));
        }

        /// <summary>
        /// 班主任考勤
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentDetail()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Attendance.StudentDetail();
                if (string.IsNullOrEmpty(vm.DayNow) || vm.DayWeekId == 0)
                {
                    vm.DayNow = DateTime.Now.ToString("D");
                }
                var DateNow = Convert.ToDateTime(vm.DayNow).AddDays(vm.DayWeekId);
                vm.DateNow = DateTime.Now.ToString("D");
                vm.DayNow = DateNow.ToString("D");
                vm.DayOfWeek = XkSystem.Code.Common.GetDayOfWeek(DateNow);
                var DayOfWeek = DateNow.DayOfWeek.ToString("D");
                if (DayOfWeek.ConvertToInt() == 0)
                {
                    DayOfWeek = 7.ToString();
                }
                var Monday = DateNow.AddDays(-DayOfWeek.ConvertToInt() + 1);
                var SunDay = Monday.AddDays(8);
                var WeekDayList = new List<System.Web.Mvc.SelectListItem>();
                for (var i = 1; i <= 7; i++)
                {
                    var weekDay = new System.Web.Mvc.SelectListItem();
                    weekDay.Value = GetDayNameOfWeek(i.ToString());
                    weekDay.Text = Monday.AddDays(i - 1).ToString("D");
                    WeekDayList.Add(weekDay);
                }
                vm.WeekDayList = WeekDayList;
                vm.StudentInfoList = Areas.Student.Controllers.StudentController.GetStudentById(vm.StudentId);
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectList().Where(x => x.Text != "午休").ToList();
                vm.AttendanceTypeList = AttendanceTypeController.SelectList();
                vm.OrgScheduleList = Areas.Course.Controllers.OrgScheduleController.GetStudentAll(vm.StudentId, vm.YearId).ToList();
                vm.AttendanceInfoList = (from p in db.Table<Attendance.Entity.tbAttendance>()
                                         where p.tbStudent.Id == vm.StudentId
                                         && p.tbStudent.IsDeleted == false
                                         && p.tbPeriod.IsDeleted == false
                                         && p.AttendanceDate >= Monday && p.AttendanceDate < SunDay
                                         select new Dto.Attendance.Info
                                         {
                                             Id = p.Id,
                                             AttendanceDate = p.AttendanceDate,
                                             StudentId = p.tbStudent.Id,
                                             OrgId = p.tbOrg.Id,
                                             PeriodId = p.tbPeriod.Id,
                                             AttendanceTypeId = p.tbAttendanceType.Id
                                         }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentDetail(Models.Attendance.StudentDetail vm)
        {
            var error = new List<string>();
            return Code.MvcHelper.Post(error, Url.Action("StudentDetail", "Attendance", new
            {
                searchText = vm.SearchText,
                YearId = vm.YearId,
                StudentId = vm.StudentId,
                DayNow = vm.DayNow,
                DayWeekId = vm.DayWeekId
            }));
        }

        public ActionResult StudentSetDetail(int keyId, int orgId, string day, int periodId, int studentId, int typeId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var list = (from p in db.Table<Attendance.Entity.tbAttendance>()
                            select p).ToList();
                if (string.IsNullOrEmpty(studentId.ToString()))
                {
                    //输入内容为空,判断是否存在Id
                    if (string.IsNullOrEmpty(keyId.ToString()) == false)
                    {
                        //如果是有id的，那就是数据库中记录的，应该做删除
                        var tf = list.Where(d => d.Id == keyId).FirstOrDefault();
                        tf.IsDeleted = true;
                    }
                }
                else
                {
                    //输入内容不为空，判断是否存在id并执行对应的操作
                    if (string.IsNullOrEmpty(keyId.ToString()) == false && keyId != 0)
                    {
                        //如果有id的，执行更新操作
                        var tf = list.Where(d => d.Id == keyId).FirstOrDefault();
                        tf.tbAttendanceType = db.Set<Attendance.Entity.tbAttendanceType>().Find(typeId);
                        tf.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        if (tf.tbAttendanceType.AttendanceValue == decimal.Zero)
                        {
                            tf.IsDeleted = true;
                        }
                    }
                    else
                    {
                        //没有id的，执行插入操作
                        var tf = new Attendance.Entity.tbAttendance();
                        tf.tbStudent = db.Set<Student.Entity.tbStudent>().Find(studentId);
                        tf.tbOrg = db.Set<Course.Entity.tbOrg>().Find(orgId);
                        tf.tbPeriod = db.Set<Basis.Entity.tbPeriod>().Find(periodId);
                        tf.InputDate = DateTime.Now;
                        tf.AttendanceDate = Convert.ToDateTime(day);
                        tf.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        tf.tbAttendanceType = db.Set<Attendance.Entity.tbAttendanceType>().Find(typeId);
                        if (tf.tbAttendanceType.AttendanceValue == decimal.Zero)//正常的考勤不保存
                        {
                            return Json("ok", JsonRequestBehavior.AllowGet);
                        }

                        db.Set<Attendance.Entity.tbAttendance>().Add(tf);
                    }
                }
                db.SaveChanges();
                return Json("ok", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 考勤室考勤
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentAllAttendance()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Attendance.StudentAllAttendance();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                vm.ClassList = Areas.Basis.Controllers.ClassController.SelectClassList(Convert.ToInt32(vm.YearId), 0);
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.StudentInfoList = Areas.Student.Controllers.StudentController.SelectStudentList(vm.YearId, vm.ClassId, vm.SearchText);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentAllAttendance(Models.Attendance.StudentAllAttendance vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StudentAllAttendance", "Attendance", new { searchText = vm.SearchText, YearId = vm.YearId, ClassId = vm.ClassId }));
        }

        /// <summary>
        /// 考勤室考勤
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentAllAttendanceEdit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Attendance.StudentAllAttendanceEdit();
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectList().Where(x => x.Text != "午休").ToList();
                vm.AttendanceTypeList = AttendanceTypeController.SelectAbnormalList();
                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult StudentAllAttendanceEdit(Models.Attendance.StudentAllAttendanceEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                var studentId = Request["studentId"].ConvertToInt();
                var fromDate = vm.StudentAttendanceEdit.FromDate;
                var toDate = vm.StudentAttendanceEdit.ToDate;
                var typeId = vm.StudentAttendanceEdit.AttendanceTypeId;
                var optionForm = Request["txtOptionForm"];
                var optionTo = Request["txtOptionTo"];
                if (fromDate > toDate)
                {
                    error.Add("开始时间不得大于结束时间");
                    return Code.MvcHelper.Post(error);
                }
                else
                {
                    if (string.IsNullOrEmpty(optionForm))
                    {
                        error.Add("开始节次至少勾选一个");
                        return Code.MvcHelper.Post(error);
                    }
                    else
                    {
                        var WeekList = Basis.Controllers.WeekController.SelectList();
                        var PeriodList = Basis.Controllers.PeriodController.SelectList().Where(x => x.Text != "午休").ToList();
                        var OrgSchedule = Areas.Course.Controllers.OrgScheduleController.GetStudentAll(vm.StudentId, vm.YearId).ToList();
                        if (toDate - fromDate == TimeSpan.Zero)
                        {
                            var StartDate = fromDate;
                            var EndDate = fromDate.AddDays(1);
                            SaveAttenAttendance(StartDate, EndDate, db, vm, optionForm, WeekList, OrgSchedule, studentId, typeId);
                        }
                        else
                        {
                            var dayCount = (toDate - fromDate).Days;
                            for (var i = 0; i <= dayCount; i++)
                            {
                                if (i == 0)
                                {
                                    var StartDate = fromDate;
                                    var EndDate = fromDate.AddDays(1);
                                    SaveAttenAttendance(StartDate, EndDate, db, vm, optionForm, WeekList, OrgSchedule, studentId, typeId);
                                }
                                else if (i == dayCount)
                                {
                                    if (string.IsNullOrEmpty(optionForm))
                                    {
                                        error.Add("结束节次至少勾选一个");
                                        return Code.MvcHelper.Post(error);
                                    }
                                    var StartDate = toDate;
                                    var EndDate = toDate.AddDays(1);
                                    SaveAttenAttendance(StartDate, EndDate, db, vm, optionTo, WeekList, OrgSchedule, studentId, typeId);
                                }
                                else
                                {
                                    var StartDate = fromDate.AddDays(i);
                                    var EndDate = fromDate.AddDays(i + 1);
                                    var optionToArr = (from p in PeriodList
                                                       select p.Value).ToArray();
                                    var optionToStr = String.Join(",", optionToArr);
                                    SaveAttenAttendance(StartDate, EndDate, db, vm, optionToStr, WeekList, OrgSchedule, studentId, typeId);
                                }
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post(null, Url.Action("StudentAllAttendance", "Attendance"), "操作完成！");
            }
        }

        private static void SaveAttenAttendance(DateTime startTime, DateTime endTime, XkSystem.Models.DbContext db, Models.Attendance.StudentAllAttendanceEdit vm, string optionForm, List<System.Web.Mvc.SelectListItem> WeekList, List<Areas.Course.Dto.OrgSchedule.Info> OrgSchedule, int StudentId, int TypeId)
        {
            #region 开始日期
            var StartDate = startTime;
            var EndDate = endTime;
            var list = (from p in db.Table<Attendance.Entity.tbAttendance>()
                        .Include(d => d.tbOrg)
                        .Include(d => d.tbAttendanceType)
                        .Include(d => d.tbPeriod)
                        .Include(d => d.tbStudent)
                        where p.tbStudent.Id == vm.StudentId
                                && p.tbStudent.IsDeleted == false
                                && p.tbOrg.IsDeleted == false
                                && p.tbPeriod.IsDeleted == false
                                && p.AttendanceDate >= StartDate && p.AttendanceDate < EndDate
                        select p).ToList();
            var strOptionForm = optionForm.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var periodId in strOptionForm)
            {
                var weekName = XkSystem.Code.Common.GetDayOfWeek(StartDate).Trim();
                var weekId = WeekList.Where(d => d.Text == weekName).FirstOrDefault().Value.ConvertToInt();
                var orgScheduleOne = OrgSchedule.Where(d => d.StudentId == StudentId && d.PeriodId == periodId.ConvertToInt() && d.WeekId == weekId);
                if (orgScheduleOne.Count() > 0)
                {
                    var orgId = orgScheduleOne.FirstOrDefault().OrgId;
                    var tfOld = list.Where(d => d.AttendanceDate.Day == StartDate.Day && d.tbOrg.Id == orgId && d.tbPeriod.Id == periodId.ConvertToInt() && d.tbStudent.Id == StudentId).FirstOrDefault();
                    if (tfOld != null)
                    {
                        tfOld.tbAttendanceType = db.Set<Attendance.Entity.tbAttendanceType>().Find(TypeId);
                        tfOld.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        if (tfOld.tbAttendanceType.AttendanceValue == decimal.Zero)
                        {
                            tfOld.IsDeleted = true;
                        }
                    }
                    else
                    {
                        var tfNew = new Attendance.Entity.tbAttendance();
                        tfNew.tbStudent = db.Set<Student.Entity.tbStudent>().Find(StudentId);
                        tfNew.tbOrg = db.Set<Course.Entity.tbOrg>().Find(orgId);
                        tfNew.tbPeriod = db.Set<Basis.Entity.tbPeriod>().Find(periodId.ConvertToInt());
                        tfNew.InputDate = DateTime.Now;
                        tfNew.AttendanceDate = Convert.ToDateTime(StartDate);
                        tfNew.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        tfNew.tbAttendanceType = db.Set<Attendance.Entity.tbAttendanceType>().Find(TypeId);
                        if (tfNew.tbAttendanceType.AttendanceValue == decimal.Zero)//正常的考勤不保存
                        {
                            continue;
                        }
                        db.Set<Attendance.Entity.tbAttendance>().Add(tfNew);
                    }
                }
            }
            db.SaveChanges();
            #endregion
        }

        public static string GetDayOfWeek(string week)
        {
            switch (week)
            {
                case "1":
                    return "周一";
                case "2":
                    return "周二";
                case "3":
                    return "周三";
                case "4":
                    return "周四";
                case "5":
                    return "周五";
                case "6":
                    return "周六";
                case "7":
                    return "周日";
                default:
                    return "未知";
            }
        }

        public static string GetDayNameOfWeek(string week)
        {
            switch (week)
            {
                case "1":
                    return "星期一";
                case "2":
                    return "星期二";
                case "3":
                    return "星期三";
                case "4":
                    return "星期四";
                case "5":
                    return "星期五";
                case "6":
                    return "星期六";
                case "7":
                    return "星期日";
                default:
                    return "未知";
            }
        }

        public ActionResult ReportClass()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Attendance.ReportClass();
                vm.ClassList = Basis.Controllers.ClassTeacherController.GetClassByClassTeacher();
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.DateSearchFrom == Code.DateHelper.MinDate)
                {
                    vm.DateSearchFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                }
                if (vm.DateSearchTo == Code.DateHelper.MinDate)
                {
                    vm.DateSearchTo = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);
                }

                var todate = vm.DateSearchTo.AddDays(1);
                vm.MyAttendanceList = (from p in db.Table<Attendance.Entity.tbAttendance>()
                                       join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals q.tbStudent.Id
                                       where p.AttendanceDate >= vm.DateSearchFrom
                                         && p.AttendanceDate < todate
                                         && p.tbStudent.IsDeleted == false
                                         && q.tbClass.Id == vm.ClassId
                                       orderby p.AttendanceDate descending
                                       select new Dto.Attendance.My
                                       {
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName,
                                           OrgName = p.tbOrg.OrgName,
                                           AttendanceDate = p.AttendanceDate,
                                           AttendanceOption = p.tbAttendanceType.AttendanceTypeName,
                                           PeriodName = p.tbPeriod.PeriodName,
                                           InputDate = p.InputDate,
                                           InputUser = p.tbSysUser.UserName
                                       }).ToList();
                /*
                vm.CourseList = (from p in vm.MyAttendanceList
                                 group p by new { p.CourseId, p.CourseName } into g
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Text = g.Key.CourseName,
                                     Value = g.Key.CourseId.ToString()
                                 }).ToList();
                if (vm.CourseId != 0)
                {
                    vm.MyAttendanceList = vm.MyAttendanceList.Where(d => d.CourseId == vm.CourseId).ToList();
                }
                */

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportClass(Models.Attendance.ReportClass vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ReportClass", new { classId = vm.ClassId, courseId = vm.CourseId, dateSearchFrom = vm.DateSearchFrom, dateSearchTo = vm.DateSearchTo }));
        }

        public ActionResult My()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Attendance.My();
                if (vm.DateSearchFrom == Code.DateHelper.MinDate)
                {
                    vm.DateSearchFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                }
                if (vm.DateSearchTo == Code.DateHelper.MinDate)
                {
                    vm.DateSearchTo = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);
                }

                var todate = vm.DateSearchTo.AddDays(1);
                vm.MyAttendanceList = (from p in db.Table<Attendance.Entity.tbAttendance>()
                                       where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                         && p.AttendanceDate >= vm.DateSearchFrom
                                         && p.AttendanceDate < todate
                                       orderby p.AttendanceDate descending
                                       select new Dto.Attendance.My
                                       {
                                           OrgName = p.tbOrg.OrgName,
                                           AttendanceDate = p.AttendanceDate,
                                           AttendanceOption = p.tbAttendanceType.AttendanceTypeName,
                                           PeriodName = p.tbPeriod.PeriodName,
                                           InputDate = p.InputDate,
                                           InputUser = p.tbSysUser.UserName
                                       }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult My(Models.Attendance.My vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("My", new { dateSearchFrom = vm.DateSearchFrom, dateSearchTo = vm.DateSearchTo }));
        }

        public ActionResult UnAttendance()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Attendance.UnAttendance();
                if (vm.DateSearchFrom == Code.DateHelper.MinDate)
                {
                    vm.DateSearchFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                }
                if (vm.DateSearchTo == Code.DateHelper.MinDate)
                {
                    vm.DateSearchTo = DateTime.Today;
                }

                //生成tbOrgCalendar
                var weekList = (from p in db.Table<Basis.Entity.tbWeek>()
                                select p).ToList();

                var calendarList = (from p in db.Table<Basis.Entity.tbCalendar>()
                                    where p.CalendarDate >= vm.DateSearchFrom && p.CalendarDate <= vm.DateSearchTo
                                    select p).ToList();
                var orgSchedule = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                                   join q in db.Table<Course.Entity.tbOrgTeacher>() on p.tbOrg.Id equals q.tbOrg.Id
                                   where p.tbOrg.tbYear.IsDefault
                                    && p.tbOrg.IsDeleted == false
                                    && q.tbTeacher.IsDeleted == false
                                   select new
                                   {
                                       p.tbOrg,
                                       p.tbWeek,
                                       p.tbPeriod,
                                       q.tbTeacher
                                   }).ToList();
                var del = (from p in db.Table<Course.Entity.tbOrgCalendar>()
                           where p.CalendarDate >= vm.DateSearchFrom
                            && p.CalendarDate <= vm.DateSearchTo
                           select p).ToList();
                foreach (var a in del)
                {
                    a.IsDeleted = true;
                }

                db.SaveChanges();

                var list = new List<Course.Entity.tbOrgCalendar>();
                for (var i = 0; i < (vm.DateSearchTo - vm.DateSearchFrom).Days; i++)
                {
                    var calendar = calendarList.Where(d => d.CalendarDate == vm.DateSearchFrom.AddDays(i)).FirstOrDefault();
                    var week = (int)vm.DateSearchFrom.AddDays(i).DayOfWeek;
                    if (calendar != null)
                    {
                        week = calendar.tbWeek.WeekCode;
                    }

                    foreach (var a in orgSchedule.Where(d => d.tbWeek.WeekCode == week).ToList())
                    {
                        list.Add(new Course.Entity.tbOrgCalendar()
                        {
                            tbOrg = a.tbOrg,
                            CalendarDate = vm.DateSearchFrom.AddDays(i),
                            tbPeriod = a.tbPeriod,
                            tbTeacher = a.tbTeacher
                        });
                    }
                }
                db.Set<Course.Entity.tbOrgCalendar>().AddRange(list);
                db.SaveChanges();

                var toDate = vm.DateSearchTo.AddDays(1);
                vm.UnAttendanceList = (from p in db.Table<Course.Entity.tbOrgCalendar>()
                                       join q in db.Table<Course.Entity.tbOrgTeacher>() on p.tbOrg.Id equals q.tbOrg.Id
                                       where p.CalendarDate >= vm.DateSearchFrom && p.CalendarDate <= toDate
                                         && p.tbOrg.IsDeleted == false
                                         && p.tbTeacher.IsDeleted == false
                                       orderby p.CalendarDate descending, p.tbPeriod.No
                                       select new Dto.Attendance.UnAttendance
                                       {
                                           TeacherId = q.tbTeacher.Id,
                                           TeacherCode = q.tbTeacher.TeacherCode,
                                           TeacherName = q.tbTeacher.TeacherName,
                                           OrgId = p.tbOrg.Id,
                                           OrgName = p.tbOrg.OrgName,
                                           CalendarDate = p.CalendarDate,
                                           PeriodName = p.tbPeriod.PeriodName
                                       }).ToList();

                var data = (from p in db.Table<Attendance.Entity.tbAttendanceTeacher>()
                            where p.AttendanceDate >= vm.DateSearchFrom && p.AttendanceDate <= toDate
                            select new
                            {
                                TeacherId = p.tbTeacher.Id,
                                OrgId = p.tbOrg.Id,
                                AttendanceDate = p.AttendanceDate
                            }).ToList();

                vm.UnAttendanceList.RemoveAll(d => data.Where(o => o.TeacherId == d.TeacherId && o.OrgId == d.OrgId && o.AttendanceDate == d.CalendarDate).Any());

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnAttendance(Models.Attendance.UnAttendance vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("UnAttendance", new { dateSearchFrom = vm.DateSearchFrom, dateSearchTo = vm.DateSearchTo }));
        }

        /// <summary>
        /// 考勤统计
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentChange()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Attendance.StudentChange();
                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = DateTime.Now.ToString(Code.Common.StringToDate);
                }
                if (string.IsNullOrEmpty(vm.DateSearchTo))
                {
                    vm.DateSearchTo = DateTime.Now.ToString(Code.Common.StringToDate);
                }
                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);
                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);
                vm.AttendanceTypeList = AttendanceTypeController.SelectList();

                var tb = from p in db.Table<Attendance.Entity.tbAttendance>()
                         where p.AttendanceDate >= fromDate && p.AttendanceDate < toDate
                              && p.tbAttendanceType.AttendanceValue != 0
                              && p.tbStudent.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }
                vm.StudentChangeList = (from p in tb
                                        orderby p.AttendanceDate descending, p.tbStudent.StudentCode
                                        select new Dto.Attendance.StudentChange
                                        {
                                            Id = p.Id,
                                            AttendanceDate = p.AttendanceDate,
                                            OrgId = p.tbOrg.Id,
                                            AttendanceTypeId = p.tbAttendanceType.Id,
                                            AttendanceTypeName = p.tbAttendanceType.AttendanceTypeName,
                                            PeriodId = p.tbPeriod.Id,
                                            PeriodName = p.tbPeriod.PeriodName,
                                            StudentId = p.tbStudent.Id,
                                            StudentCode = p.tbStudent.StudentCode,
                                            SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                            StudentName = p.tbStudent.StudentName
                                        }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentChange(Models.Attendance.StudentChange vm)
        {
            var error = new List<string>();
            return Code.MvcHelper.Post(error, Url.Action("StudentChange", new
            {
                searchText = vm.SearchText,
                DateSearchFrom = vm.DateSearchFrom,
                DateSearchTo = vm.DateSearchTo
            }));
        }

        [HttpPost]
        public ActionResult SaveChange(Models.Attendance.StudentChange vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                string[] txtId = null;
                string[] txtAttendanceTypeId = null;
                try
                {
                    txtId = Request["txtId"].Split(',');
                    txtAttendanceTypeId = Request["AttendanceTypeId"].Split(',');
                }
                catch
                {
                    return Json(new { Status = decimal.One, Message = "操作成功！" });
                }

                var list = (from p in db.Table<Attendance.Entity.tbAttendance>()
                            select p).ToList();

                for (var i = 0; i < txtId.Count(); i++)
                {
                    if (string.IsNullOrEmpty(txtId[i]) == false && txtId[i].ConvertToInt() != 0)
                    {
                        var tf = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                        tf.tbAttendanceType = db.Set<Attendance.Entity.tbAttendanceType>().Find(txtAttendanceTypeId[i].ConvertToInt());
                        tf.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        if (tf.tbAttendanceType.AttendanceValue == decimal.Zero)//正常的考勤不保存
                        {
                            tf.IsDeleted = true;
                        }
                    }
                }
                db.SaveChanges();

                return Code.MvcHelper.Post(null, Url.Action("StudentChange", new { searchText = vm.SearchText, DateSearchFrom = vm.DateSearchFrom, DateSearchTo = vm.DateSearchTo, }), "提交成功!");
            }
        }

        /// <summary>
        /// CU考勤查询接口
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public static List<Dto.Attendance.Detail> AttendanceList(string TenantName, string UserCode, string FromDate, string ToDate)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var TenantModel = db.Set<Admin.Entity.tbTenant>().Where(m => m.TenantName == TenantName).FirstOrDefault();
                var tb = (from p in db.Table<Attendance.Entity.tbAttendance>(TenantModel.Id).Include(m => m.tbAttendanceType).Include(m => m.tbStudent).Include(m => m.tbOrg).Include(m => m.tbPeriod)
                          where p.tbStudent.IsDeleted == false && (p.tbStudent.tbSysUser.UserCode == UserCode && p.tbStudent.tbSysUser.tbTenant.Id == TenantModel.Id)
                          orderby p.AttendanceDate descending
                          select p).ToList();
                if (!string.IsNullOrEmpty(FromDate))
                {
                    var Fdate = Convert.ToDateTime(FromDate);
                    tb = tb.Where(d => d.AttendanceDate >= Fdate).ToList();
                }
                if (!string.IsNullOrEmpty(ToDate))
                {
                    var Tdate = Convert.ToDateTime(ToDate);
                    tb = tb.Where(d => d.AttendanceDate < Tdate).ToList();
                }
                var AttendanceList = (from p in tb
                                      select new Dto.Attendance.Detail
                                      {
                                          AttendanceTypeName = p.tbAttendanceType.AttendanceTypeName,
                                          AttendanceDate = p.AttendanceDate,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName,
                                          OrgName = p.tbOrg.OrgName,
                                          PeriodName = p.tbPeriod.PeriodName
                                      }).ToList();
                return AttendanceList;
            }
        }

        /// <summary>
        /// 考勤记录写入接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddAttendanceList(Request.AttendanceRequestModel model)
        {
            List<Attendance.Entity.tbAttendanceLog> Addlist = new List<Attendance.Entity.tbAttendanceLog>();
            if (model.AttendanceRequest != null && model.AttendanceRequest.Count > 0)
            {
                foreach (var item in model.AttendanceRequest)
                {
                    var Pmodel = new Attendance.Entity.tbAttendanceLog();
                    Pmodel.AttendanceDate = new DateTime(item.idwYear, item.idwMonth, item.idwDay, item.idwHour, item.idwMinute, item.idwSecond);
                    Pmodel.CardNumber = item.sdwEnrollNumber.ToString();
                    Pmodel.MachineCode = item.iMachineNumber.ToString();
                    Pmodel.Status = false;
                    Pmodel.No = 0;
                    Addlist.Add(Pmodel);
                }
                var b = Service.Attendance.AddAttendance(Addlist, model.TenantName);
                return new ExJsonResult(new { IsSuccess = b });
            }
            return new ExJsonResult(new { IsSuccess = false });
        }


        /// <summary>
        /// 考勤处理接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult DealWithAttendance(Request.DealAttendanceRequestModel model)
        {
            var b = Service.Attendance.DealWithAttendanceLog(model.TenantName);
            return new ExJsonResult(new { IsSuccess = b });
        }
    }
}