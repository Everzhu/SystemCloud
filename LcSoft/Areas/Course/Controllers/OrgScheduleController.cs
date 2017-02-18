using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Course.Controllers
{
    public class OrgScheduleController : Controller
    {
        public ActionResult List(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgSchedule.List();
                vm.ScheduleTypeList = typeof(Code.EnumHelper.CourseScheduleType).ToItemList();
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectList();

                vm.OrgScheduleList = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                                      where p.tbOrg.Id == id
                                      orderby p.No
                                      select new Dto.OrgSchedule.List
                                      {
                                          Id = p.Id,
                                          ScheduleType = p.ScheduleType,
                                          OrgName = p.tbOrg.OrgName,
                                          PeriodId = p.tbPeriod.Id,
                                          WeekId = p.tbWeek.Id
                                      }).ToList();
                return PartialView(vm);
            }
        }

        [HttpPost]
        public ActionResult List(Models.OrgSchedule.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        public bool EditOrgSchedule(XkSystem.Models.DbContext db, Course.Entity.tbOrg Org, string[] ids)
        {
            //删除之前的数据
            var delList = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                           where p.tbOrg.Id == Org.Id
                           select p).ToList();
            foreach (var a in delList)
            {
                a.IsDeleted = true;
            }

            if (ids.Length > 0)
            {
                foreach (var v in ids)
                {
                    var type = v.Split('_');
                    if (type[2] != "-1")
                    {
                        var tb = new Course.Entity.tbOrgSchedule()
                        {
                            tbOrg = Org,
                            tbWeek = db.Set<Basis.Entity.tbWeek>().Find(Convert.ToInt32(v.Split('_')[0])),
                            tbPeriod = db.Set<Basis.Entity.tbPeriod>().Find(Convert.ToInt32(v.Split('_')[1])),
                            //ScheduleType = type[2] == "1" ? Code.EnumHelper.CourseScheduleType.Odd : Code.EnumHelper.CourseScheduleType.Dual
                        };
                        switch (type[2])
                        {
                            case "1": tb.ScheduleType = Code.EnumHelper.CourseScheduleType.Odd; break;
                            case "2": tb.ScheduleType = Code.EnumHelper.CourseScheduleType.Dual; break;
                            default: tb.ScheduleType = Code.EnumHelper.CourseScheduleType.All; break;
                        }

                        db.Set<Course.Entity.tbOrgSchedule>().Add(tb);
                    }
                }
            }

            return true;
        }

        public bool EditOrgSchedule(XkSystem.Models.DbContext db, Course.Entity.tbOrg Org, string orgSchedule)
        {
            //删除之前的数据
            var delList = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                           where p.tbOrg.Id == Org.Id
                           select p).ToList();
            foreach (var a in delList)
            {
                a.IsDeleted = true;
            }
            var arr = orgSchedule.Split(new char[] { ',', '，' });
            foreach (var v in arr)
            {
                var tb = new Course.Entity.tbOrgSchedule();
                if (v.Contains("["))
                {
                    var arr1 = v.Split(new char[] { '[', ']' });
                    var temp = arr1[0];
                    tb = new Course.Entity.tbOrgSchedule()
                    {
                        tbOrg = Org,
                        tbWeek = db.Table<Basis.Entity.tbWeek>().Where(d => temp.Contains(d.WeekName)).FirstOrDefault(),
                        tbPeriod = db.Table<Basis.Entity.tbPeriod>().Where(d => temp.Contains(d.PeriodName)).FirstOrDefault(),
                        ScheduleType = arr1[1].Contains("单") ? Code.EnumHelper.CourseScheduleType.Odd : Code.EnumHelper.CourseScheduleType.Dual
                    };
                }
                else
                {
                    tb = new Course.Entity.tbOrgSchedule()
                    {
                        tbOrg = Org,
                        tbWeek = db.Table<Basis.Entity.tbWeek>().Where(d => v.Contains(d.WeekName)).FirstOrDefault(),
                        tbPeriod = db.Table<Basis.Entity.tbPeriod>().Where(d => v.Contains(d.PeriodName)).FirstOrDefault(),
                        ScheduleType = Code.EnumHelper.CourseScheduleType.All
                    };
                }

                db.Set<Course.Entity.tbOrgSchedule>().Add(tb);
            }

            return true;
        }

        public static List<Dto.OrgSchedule.Info> GetAll(int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                          where p.tbOrg.tbYear.Id == yearId
                            && p.tbOrg.IsDeleted == false
                          select new Dto.OrgSchedule.Info
                          {
                              OrgId = p.tbOrg.Id,
                              RoomId = p.tbOrg.tbRoom != null ? p.tbOrg.tbRoom.Id : 0,
                              WeekId = p.tbWeek.Id,
                              ClassId = (p.tbOrg.IsClass && p.tbOrg.tbClass != null) ? p.tbOrg.tbClass.Id : 0,
                              PeriodId = p.tbPeriod.Id,
                              Subject = p.tbOrg.tbCourse.tbSubject.SubjectName,
                              SubjectId = p.tbOrg.tbCourse.tbSubject.Id,
                              CourseName = p.tbOrg.tbCourse.CourseName,
                              RoomName = p.tbOrg.tbRoom.RoomName,
                              ScheduleType = p.ScheduleType,
                          }).ToList();
                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                      where p.tbOrg.tbYear.Id == yearId
                                        && p.tbOrg.IsDeleted == false
                                      select new
                                      {
                                          OrgId = p.tbOrg.Id,
                                          TeacherId = p.tbTeacher.Id,
                                          p.tbTeacher.TeacherName
                                      }).ToList();
                foreach (var a in tb)
                {
                    var orgTeacher = orgTeacherList.Where(d=>d.OrgId == a.OrgId).FirstOrDefault();
                    if (orgTeacher != null)
                    {
                        a.TeacherName = orgTeacher.TeacherName;
                        a.TeacherId = orgTeacher.TeacherId;
                    }

                    switch (a.ScheduleType)
                    {
                        case Code.EnumHelper.CourseScheduleType.Odd: a.ScheduleTypeName = "单周"; break;
                        case Code.EnumHelper.CourseScheduleType.Dual: a.ScheduleTypeName = "双周"; break;
                        default: a.ScheduleTypeName = ""; break;
                    }
                }

                return tb;
            }
        }

        public static List<Dto.OrgSchedule.Info> GetAll(string teacherId, int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var orgTeacher = db.Table<Course.Entity.tbOrgTeacher>().Where(d => d.tbTeacher.TeacherCode == teacherId).Include(d => d.tbOrg).FirstOrDefault();
                var tb = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                          where p.tbOrg.tbYear.Id == yearId
                            && p.tbOrg.IsDeleted == false
                          select new Dto.OrgSchedule.Info
                          {
                              RoomId = p.tbOrg.tbRoom != null ? p.tbOrg.tbRoom.Id : 0,
                              WeekId = p.tbWeek.Id,
                              ClassId = (p.tbOrg.IsClass && p.tbOrg.tbClass != null) ? p.tbOrg.tbClass.Id : 0,
                              PeriodId = p.tbPeriod.Id,
                              Subject = p.tbOrg.tbCourse.tbSubject.SubjectName
                          }).ToList();
                return tb;
            }
        }

        public static List<Dto.OrgSchedule.Info> GetTeacherAll(int teacherId, int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                          join q in db.Table<Course.Entity.tbOrgTeacher>() on p.tbOrg.Id equals q.tbOrg.Id
                          where p.tbOrg.tbYear.Id == yearId
                            && p.tbOrg.IsDeleted == false
                            && q.tbTeacher.Id == teacherId
                          select new Dto.OrgSchedule.Info
                          {
                              RoomId = p.tbOrg.tbRoom != null ? p.tbOrg.tbRoom.Id : 0,
                              RoomName = p.tbOrg.tbRoom != null ? p.tbOrg.tbRoom.RoomName : String.Empty,
                              WeekId = p.tbWeek.Id,
                              ClassId = (p.tbOrg.IsClass && p.tbOrg.tbClass != null) ? p.tbOrg.tbClass.Id : 0,
                              OrgId = p.tbOrg.Id,
                              OrgName = p.tbOrg.OrgName,
                              PeriodId = p.tbPeriod.Id,
                              Subject = p.tbOrg.tbCourse.tbSubject.SubjectName,
                              CourseName = p.tbOrg.tbCourse.CourseName,
                              ScheduleType = p.ScheduleType
                          }).ToList();
                foreach (var a in tb)
                {
                    switch (a.ScheduleType)
                    {
                        case Code.EnumHelper.CourseScheduleType.Odd: a.ScheduleTypeName = "单周"; break;
                        case Code.EnumHelper.CourseScheduleType.Dual: a.ScheduleTypeName = "双周"; break;
                        default: a.ScheduleTypeName = ""; break;
                    }
                }

                return tb;
            }
        }

        public static List<Dto.OrgSchedule.Info> GetStudentAll(int studentId, int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var orgStudent = db.Table<Course.Entity.tbOrgStudent>()
                    .Where(d => d.tbStudent.Id == studentId && d.tbOrg.IsDeleted == false)
                    .Include(d => d.tbOrg).ToList();
                var classId = (from t in db.Table<Basis.Entity.tbClassStudent>()
                                .Include(t => t.tbClass)
                               where t.tbStudent.Id == studentId
                                && t.tbClass.IsDeleted == false
                               select t.tbClass.Id).FirstOrDefault();
                var list = new List<Dto.OrgSchedule.Info>();
                if (orgStudent.Count>0)
                {
                    var orgStudentOrdIds = orgStudent.Select(t => t.tbOrg.Id).ToList();
                    var tb = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                              where (p.tbOrg.tbYear.Id == yearId || (yearId == 0 && p.tbOrg.tbYear.IsDefault ))
                                && p.tbOrg.IsDeleted == false
                                && (orgStudentOrdIds.Contains(p.tbOrg.Id) || p.tbOrg.tbClass.Id == classId)
                              select new Dto.OrgSchedule.Info
                              {
                                  RoomId = p.tbOrg.tbRoom != null ? p.tbOrg.tbRoom.Id : 0,
                                  RoomName = p.tbOrg.tbRoom != null ? p.tbOrg.tbRoom.RoomName : String.Empty,
                                  WeekId = p.tbWeek.Id,
                                  ClassId = (p.tbOrg.IsClass && p.tbOrg.tbClass != null) ? p.tbOrg.tbClass.Id : 0,
                                  OrgId = p.tbOrg.Id,
                                  OrgName = p.tbOrg.OrgName,
                                  PeriodId = p.tbPeriod.Id,
                                  Subject = p.tbOrg.tbCourse.tbSubject.SubjectName,
                                  CourseName = p.tbOrg.tbCourse.CourseName,
                                  CourseId = p.tbOrg.tbCourse.Id,
                                  StudentId = studentId,
                                  ScheduleType = p.ScheduleType
                              }).ToList();

                    foreach (var a in tb)
                    {
                        switch (a.ScheduleType)
                        {
                            case Code.EnumHelper.CourseScheduleType.Odd: a.ScheduleTypeName = "单周"; break;
                            case Code.EnumHelper.CourseScheduleType.Dual: a.ScheduleTypeName = "双周"; break;
                            default: a.ScheduleTypeName = ""; break;
                        }
                    }

                    list.AddRange(tb);
                }

                if (classId != 0)
                {
                    var tb = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                              where (p.tbOrg.tbYear.Id == yearId || (yearId == 0 && p.tbOrg.tbYear.IsDefault))
                                && p.tbOrg.IsDeleted == false
                                && p.tbOrg.tbClass.Id == classId
                              select new Dto.OrgSchedule.Info
                              {
                                  RoomId = p.tbOrg.tbRoom != null ? p.tbOrg.tbRoom.Id : 0,
                                  RoomName = p.tbOrg.tbRoom != null ? p.tbOrg.tbRoom.RoomName : String.Empty,
                                  WeekId = p.tbWeek.Id,
                                  ClassId = (p.tbOrg.IsClass && p.tbOrg.tbClass != null) ? p.tbOrg.tbClass.Id : 0,
                                  OrgId = p.tbOrg.Id,
                                  OrgName = p.tbOrg.OrgName,
                                  PeriodId = p.tbPeriod.Id,
                                  Subject = p.tbOrg.tbCourse.tbSubject.SubjectName,
                                  CourseId = p.tbOrg.tbCourse.Id,
                                  StudentId = studentId,
                                  ScheduleType = p.ScheduleType
                              }).ToList();

                    foreach (var a in tb)
                    {
                        switch (a.ScheduleType)
                        {
                            case Code.EnumHelper.CourseScheduleType.Odd: a.ScheduleTypeName = "单周"; break;
                            case Code.EnumHelper.CourseScheduleType.Dual: a.ScheduleTypeName = "双周"; break;
                            default: a.ScheduleTypeName = ""; break;
                        }
                    }

                    list.AddRange(tb);
                }

                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                      where p.tbTeacher.IsDeleted == false
                                        && (p.tbOrg.tbYear.Id == yearId || (yearId == 0 && p.tbOrg.tbYear.IsDefault))
                                      select new
                                      {
                                          OrgId = p.tbOrg.Id,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();
                foreach (var a in list)
                {
                    a.TeacherName = string.Join(",", orgTeacherList.Where(d => d.OrgId == a.OrgId).Select(d => d.TeacherName));
                }

                return list;
            }
        }
    }
}