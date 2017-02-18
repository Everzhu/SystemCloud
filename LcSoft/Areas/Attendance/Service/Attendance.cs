using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace XkSystem.Areas.Attendance.Service
{
    public class Attendance
    {
        /// <summary>
        /// 添加考勤原始数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool AddAttendance(List<Entity.tbAttendanceLog> list, string TenantName)
        {
            try
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (list != null && list.Count > 0)
                    {
                        var Tenant = db.TableRoot<Admin.Entity.tbTenant>().Where(d => d.TenantName == TenantName).FirstOrDefault();
                        foreach (var item in list)
                        {
                            item.tbTenant = Tenant;
                        }
                        db.Set<Entity.tbAttendanceLog>().AddRange(list);
                        if (db.SaveChanges() > 0)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }

        public static bool DealWithAttendanceLog(string TenantName)
        {
            int VailTime = -30;//提前打卡有效时间内
            int LastTime = 5;//最晚打卡时间后(迟到)
            int AbsentTime = 15;//最晚打卡时间后(旷课)
            var date = DateTime.Now;
            try
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var Tenant = db.TableRoot<Admin.Entity.tbTenant>().Where(d => d.TenantName == TenantName).FirstOrDefault();
                    var SysUser = db.TableRoot<Sys.Entity.tbSysUser>().Where(d => d.UserCode == "System" && d.tbTenant.TenantName == TenantName).FirstOrDefault();
                    if (SysUser == null)
                    {
                        var admin = new Sys.Entity.tbSysUser()
                        {
                            tbTenant = Tenant,
                            UserCode = "System",
                            UserName = "System",
                            UserType = Code.EnumHelper.SysUserType.Other,
                            Password = "system&123456",
                            PasswordMd5 = Code.Common.CreateMD5Hash("system&123456"),
                        };
                        db.Set<Sys.Entity.tbSysUser>().Add(admin);
                        db.SaveChanges();
                    }

                    int weekIndex = (int)DateTime.Now.DayOfWeek;
                    weekIndex = weekIndex == 0 ? 7 : weekIndex;

                    var OrgScheduleList = (from p in db.TableRoot<Course.Entity.tbOrgSchedule>().Where(d => d.tbTenant.TenantName == TenantName && d.tbWeek.No == weekIndex)
                                           select p).Include(d => d.tbPeriod).Include(d => d.tbOrg).ToList();
                    List<Entity.tbAttendance> ADlist = new List<Entity.tbAttendance>();
                    foreach (var ScheItem in OrgScheduleList)
                    {
                        var PeriodModel = (from p in db.TableRoot<Basis.Entity.tbPeriod>().Where(d => d.tbTenant.TenantName == TenantName && d.Id == ScheItem.tbPeriod.Id)
                                           select p).FirstOrDefault();

                        var PrevPeriodToDate = new DateTime();
                        if (PeriodModel.No > 1)
                        {
                            var PrevNo = PeriodModel.No - 1;
                            var PrevPeriodModel = (from p in db.TableRoot<Basis.Entity.tbPeriod>().Where(d => d.tbTenant.TenantName == TenantName && d.No == PrevNo)
                                                   select p).FirstOrDefault();
                            PrevPeriodToDate = Convert.ToDateTime(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " " + PrevPeriodModel.ToDate);
                        }

                        var PeriodFromDate = Convert.ToDateTime(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " " + PeriodModel.FromDate);
                        var PeriodToDate = Convert.ToDateTime(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " " + PeriodModel.ToDate);
                        var KkDate = PeriodFromDate.AddMinutes(AbsentTime);//旷课时间点
                        var LDate = PeriodFromDate.AddMinutes(LastTime);//迟到时间点
                        var VDate = PeriodFromDate.AddMinutes(VailTime);//有效打卡时间点

                        var OrgModel = (from p in db.TableRoot<Course.Entity.tbOrg>().Where(d => d.tbTenant.TenantName == TenantName && d.Id == ScheItem.tbOrg.Id)
                                        select p).Include(d => d.tbClass).FirstOrDefault();
                        if (OrgModel.tbClass != null)//tbClassStudent
                        {
                            var ClassStudentList = (from p in db.TableRoot<Basis.Entity.tbClassStudent>().Where(d => d.tbTenant.TenantName == TenantName && d.tbClass.Id == OrgModel.tbClass.Id)
                                                    select p).Include(d => d.tbStudent).Include(d => d.tbStudent.tbSysUser).ToList();

                            var StudentUserIds = ClassStudentList.Where(d => d.tbStudent.tbSysUser.CardNo != null).Select(d => d.tbStudent.tbSysUser.CardNo).ToList();

                            var LCardNoIds = new List<string>();

                            var LogList = new List<Entity.tbAttendanceLog>();
                            //迟到
                            if (PeriodModel.No == 1)
                            {
                                LogList = (from p in db.TableRoot<Entity.tbAttendanceLog>().Where(d => d.tbTenant.TenantName == TenantName && d.Status == false && StudentUserIds.Contains(d.CardNumber) && d.AttendanceDate <= PeriodToDate)
                                           select p).ToList();//第一节课
                            }
                            else
                            {
                                LogList = (from p in db.TableRoot<Entity.tbAttendanceLog>().Where(d => d.tbTenant.TenantName == TenantName && d.Status == false && StudentUserIds.Contains(d.CardNumber) && d.AttendanceDate > PrevPeriodToDate && d.AttendanceDate <= PeriodToDate)
                                           select p).ToList();//其他节课
                            }


                            foreach (var LogItem in LogList)
                            {
                                LogItem.Status = true;
                                if (LogItem.AttendanceDate > LDate && LogItem.AttendanceDate <= KkDate)//迟到
                                {
                                    var model = new Entity.tbAttendance();
                                    model.AttendanceDate = LogItem.AttendanceDate;
                                    model.No = 0;
                                    model.tbTenant = Tenant;
                                    model.InputDate = DateTime.Now;
                                    model.tbOrg = OrgModel;
                                    model.tbAttendanceType = db.TableRoot<Entity.tbAttendanceType>().Where(d => d.AttendanceTypeName == "迟到").FirstOrDefault();
                                    model.tbPeriod = PeriodModel;
                                    model.tbStudent = db.TableRoot<Student.Entity.tbStudent>().Where(d => d.CardNo == LogItem.CardNumber).FirstOrDefault();
                                    model.tbSysUser = db.TableRoot<Sys.Entity.tbSysUser>().Where(d => d.UserCode == "System" && d.tbTenant.TenantName == TenantName).FirstOrDefault();
                                    ADlist.Add(model);
                                    LCardNoIds.Add(LogItem.CardNumber);
                                }
                                else if (LogItem.AttendanceDate >= VDate && LogItem.AttendanceDate <= LDate)//正常
                                {
                                    LCardNoIds.Add(LogItem.CardNumber);
                                }
                                else if (LogItem.AttendanceDate > KkDate)
                                {
                                    LCardNoIds.Add(LogItem.CardNumber);
                                }
                            }
                            //旷课
                            if (LCardNoIds != null && LCardNoIds.Count > 0 && LCardNoIds.Count >= (ClassStudentList.Count / 2))
                            {
                                var KkStudentList = ClassStudentList.Where(d => !LCardNoIds.Contains(d.tbStudent.tbSysUser.CardNo));
                                foreach (var KsItem in KkStudentList)
                                {
                                    var model = new Entity.tbAttendance();
                                    model.AttendanceDate = DateTime.Today;
                                    model.tbTenant = Tenant;
                                    model.InputDate = DateTime.Now;
                                    model.tbOrg = OrgModel;
                                    model.tbAttendanceType = db.TableRoot<Entity.tbAttendanceType>().Where(d => d.AttendanceTypeName == "缺席").FirstOrDefault();
                                    model.tbPeriod = PeriodModel;
                                    model.tbStudent = db.TableRoot<Student.Entity.tbStudent>().Where(d => d.CardNo == KsItem.tbStudent.tbSysUser.CardNo).FirstOrDefault();
                                    model.tbSysUser = db.TableRoot<Sys.Entity.tbSysUser>().Where(d => d.UserCode == "System" && d.tbTenant.TenantName == TenantName).FirstOrDefault();
                                    ADlist.Add(model);
                                }
                            }
                            db.Set<Entity.tbAttendance>().AddRange(ADlist);
                        }
                        else//tbOrgStudent
                        {
                            var OrgStudentList = (from p in db.TableRoot<Course.Entity.tbOrgStudent>().Where(d => d.tbTenant.TenantName == TenantName && d.tbOrg.Id == ScheItem.tbOrg.Id)
                                                  select p).Include(d => d.tbStudent).Include(d => d.tbStudent.tbSysUser).ToList();

                            var StudentUserIds = OrgStudentList.Where(d => d.tbStudent.tbSysUser.CardNo != null).Select(d => d.tbStudent.tbSysUser.CardNo).ToList();

                            var LCardNoIds = new List<string>();

                            var LogList = new List<Entity.tbAttendanceLog>();
                            //迟到
                            if (PeriodModel.No == 1)
                            {
                                LogList = (from p in db.TableRoot<Entity.tbAttendanceLog>().Where(d => d.tbTenant.TenantName == TenantName && d.Status == false && StudentUserIds.Contains(d.CardNumber) && d.AttendanceDate <= PeriodToDate)
                                           select p).ToList();//第一节课
                            }
                            else
                            {
                                LogList = (from p in db.TableRoot<Entity.tbAttendanceLog>().Where(d => d.tbTenant.TenantName == TenantName && d.Status == false && StudentUserIds.Contains(d.CardNumber) && d.AttendanceDate > PrevPeriodToDate && d.AttendanceDate <= PeriodToDate)
                                           select p).ToList();//其他节课
                            }

                            //List<Perform.Entity.tbAttendance> ADlist = new List<Perform.Entity.tbAttendance>();
                            foreach (var LogItem in LogList)
                            {
                                LogItem.Status = true;
                                if (LogItem.AttendanceDate > LDate && LogItem.AttendanceDate <= KkDate)//迟到
                                {
                                    var model = new Entity.tbAttendance();
                                    model.AttendanceDate = LogItem.AttendanceDate;
                                    model.No = 0;
                                    model.tbTenant = Tenant;
                                    model.InputDate = DateTime.Now;
                                    model.tbOrg = OrgModel;
                                    model.tbAttendanceType = db.TableRoot<Entity.tbAttendanceType>().Where(d => d.AttendanceTypeName == "迟到").FirstOrDefault();
                                    model.tbPeriod = PeriodModel;
                                    model.tbStudent = db.TableRoot<Student.Entity.tbStudent>().Where(d => d.CardNo == LogItem.CardNumber).FirstOrDefault();
                                    model.tbSysUser = db.TableRoot<Sys.Entity.tbSysUser>().Where(d => d.UserCode == "SysUser" && d.tbTenant.TenantName == TenantName).FirstOrDefault();
                                    ADlist.Add(model);
                                    LCardNoIds.Add(LogItem.CardNumber);
                                }
                                else if (LogItem.AttendanceDate >= VDate && LogItem.AttendanceDate <= LDate)//正常
                                {
                                    LCardNoIds.Add(LogItem.CardNumber);
                                }
                                else if (LogItem.AttendanceDate > KkDate)
                                {
                                    LCardNoIds.Add(LogItem.CardNumber);
                                }
                            }
                            //旷课
                            if (LCardNoIds != null && LCardNoIds.Count > 0 && LCardNoIds.Count >= (OrgStudentList.Count / 2))
                            {
                                var KkStudentList = OrgStudentList.Where(d => !LCardNoIds.Contains(d.tbStudent.tbSysUser.CardNo));
                                foreach (var KsItem in KkStudentList)
                                {
                                    var model = new Entity.tbAttendance();
                                    model.AttendanceDate = DateTime.Today;
                                    model.No = 0;
                                    model.tbTenant = Tenant;
                                    model.InputDate = DateTime.Now;
                                    model.tbOrg = OrgModel;
                                    model.tbAttendanceType = db.TableRoot<Entity.tbAttendanceType>().Where(d => d.AttendanceTypeName == "缺席").FirstOrDefault();
                                    model.tbPeriod = PeriodModel;
                                    model.tbStudent = db.TableRoot<Student.Entity.tbStudent>().Where(d => d.CardNo == KsItem.tbStudent.tbSysUser.CardNo).FirstOrDefault();
                                    model.tbSysUser = db.TableRoot<Sys.Entity.tbSysUser>().Where(d => d.UserCode == "SysUser" && d.tbTenant.TenantName == TenantName).FirstOrDefault();
                                    ADlist.Add(model);
                                }
                            }
                            db.Set<Entity.tbAttendance>().AddRange(ADlist);
                        }
                    }
                    db.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}