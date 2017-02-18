using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class ClassController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Class.List();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.Where(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                vm.ClassTypeList = Basis.Controllers.ClassTypeController.SelectList();

                var tb = from p in db.Table<Basis.Entity.tbClass>()
                         where p.tbYear.Id == vm.YearId
                         select p;
                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ClassName.Contains(vm.SearchText));
                }

                if (vm.GradeId != 0)
                {
                    tb = tb.Where(d => d.tbGrade.Id == vm.GradeId);
                }

                if (vm.ClassTypeId != 0)
                {
                    tb = tb.Where(d => d.tbClassType.Id == vm.ClassTypeId);
                }

                vm.ClassList = (from p in tb
                                orderby p.tbGrade.No, p.tbGrade.GradeName, p.No, p.ClassName
                                select new Dto.Class.List
                                {
                                    Id = p.Id,
                                    No = p.No,
                                    ClassName = p.ClassName,
                                    ClassTypeName = p.tbClassType.ClassTypeName,
                                    GradeName = p.tbGrade.GradeName,
                                    RoomName = p.tbRoom != null ? p.tbRoom.RoomName : string.Empty,
                                    StudentCount = db.Set<Basis.Entity.tbClassStudent>().Where(d => d.IsDeleted == false && d.tbStudent.IsDeleted == false && d.tbClass.Id == p.Id).Count()
                                }).ToList();

                var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                        where p.tbClass.tbYear.Id == vm.YearId
                                            && p.tbTeacher.IsDeleted == false
                                            && (p.tbClass.tbGrade.Id == vm.GradeId || vm.GradeId == 0)
                                            && (p.tbClass.tbClassType.Id == vm.ClassTypeId || vm.ClassTypeId == 0)
                                        select new
                                        {
                                            ClassId = p.tbClass.Id,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).ToList();
                foreach (var a in vm.ClassList)
                {
                    a.TeacherName = string.Join(",", classTeacherList.Where(d => d.ClassId == a.Id).Select(d => d.TeacherName).Distinct());
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Class.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, classTypeId = vm.ClassTypeId, gradeId = vm.GradeId, yearId = vm.YearId }));
        }

        public ActionResult OrgStudentByClassList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Class.OrgStudentByClassList();

                var teacher = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.tbSysUser.Id == XkSystem.Code.Common.UserId).FirstOrDefault();
                if (teacher == null)
                {
                    return Content("<script>alert('你不是班主任！');</script>");
                }

                //vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                //if (vm.YearId == 0 && vm.YearList.Count > 0)
                //{
                //    vm.YearId = vm.YearList.Where(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                //}
                vm.ClassList = ClassController.SelectClassByTeacherList(0, teacher.Id);
                vm.SubjectList = Course.Controllers.SubjectController.SelectList();

                var classIds = vm.ClassList.Select(d => d.Value.ConvertToInt()).ToList();
                var tb = db.Table<Entity.tbClassStudent>();
                if (vm.ClassId > 0)
                {
                    tb = tb.Where(d => d.tbClass.Id == vm.ClassId);
                }
                else
                {
                    tb = tb.Where(d => classIds.Contains(d.tbClass.Id));
                }
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentName.Contains(vm.SearchText)
                          || d.tbStudent.StudentCode.Contains(vm.SearchText));
                }
                //行政班模式
                var tempList = tb.Select(d => new
                {
                    StudentId = d.tbStudent.Id,
                    ClassId = d.tbClass.Id,
                    StudentCode = d.tbStudent.StudentCode,
                    StudentName = d.tbStudent.StudentName
                }).ToList();

                var classOrgList = db.Table<Course.Entity.tbOrg>()
                    .Include(d => d.tbClass).Include(d => d.tbRoom).Include(d => d.tbCourse.tbSubject)
                    .Where(d => classIds.Contains(d.tbClass.Id)).ToList();

                foreach (var v in tempList)
                {
                    foreach (var vv in classOrgList.Where(d => d.tbClass.Id == v.ClassId))
                    {
                        vm.DataList.Add(new Dto.Class.OrgStudentByClassList()
                        {
                            OrgName = vv.OrgName,
                            OrgId = vv.Id,
                            RoomName = vv.tbRoom != null ? vv.tbRoom.RoomName : "",
                            StudentId = v.StudentId,
                            StudentCode = v.StudentCode,
                            StudentName = v.StudentName,
                            SubjectId = vv.tbCourse.tbSubject != null ? vv.tbCourse.tbSubject.Id : 0,
                            SubjectName = vv.tbCourse.tbSubject != null ? vv.tbCourse.tbSubject.SubjectName : ""
                        });
                    }
                }

                //班级学生
                var classStudentIds = tempList.Select(d => d.StudentId).ToList();

                //走班模式
                var tempDataList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                    where p.tbOrg.IsClass == false
                                    && classStudentIds.Contains(p.tbStudent.Id)
                                    orderby p.No
                                    select new Dto.Class.OrgStudentByClassList()
                                    {
                                        OrgName = p.tbOrg.OrgName,
                                        OrgId = p.tbOrg.Id,
                                        RoomName = p.tbOrg.tbRoom != null ? p.tbOrg.tbRoom.RoomName : "",
                                        StudentId = p.tbStudent.Id,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        SubjectId = p.tbOrg.tbCourse.tbSubject != null ? p.tbOrg.tbCourse.tbSubject.Id : 0,
                                        SubjectName = p.tbOrg.tbCourse.tbSubject != null ? p.tbOrg.tbCourse.tbSubject.SubjectName : ""
                                    }).ToList();
                vm.DataList.AddRange(tempDataList);

                var orgIds = vm.DataList.Select(c => c.OrgId).Distinct().ToList();

                var orgTeacherList = db.Table<Course.Entity.tbOrgTeacher>()
                    .Include(d => d.tbOrg).Include(d => d.tbTeacher)
                    .Where(d => orgIds.Contains(d.tbOrg.Id)).ToList();

                var orgScheduleList = db.Table<Course.Entity.tbOrgSchedule>()
                    .Include(d => d.tbOrg).Include(d => d.tbPeriod).Include(d => d.tbWeek)
                    .OrderBy(d => d.tbWeek.No)
                    .Where(d => orgIds.Contains(d.tbOrg.Id)).ToList();

                foreach (var v in vm.DataList)
                {
                    v.TeacherCode = string.Join(",", orgTeacherList.Where(d => d.tbOrg.Id == v.OrgId).Select(d => d.tbTeacher.TeacherCode).Distinct());
                    v.TeacherName = string.Join(",", orgTeacherList.Where(d => d.tbOrg.Id == v.OrgId).Select(d => d.tbTeacher.TeacherName).Distinct());
                    v.WeekPeriod = string.Join(",", orgScheduleList.Where(d => d.tbOrg.Id == v.OrgId).Select(d => "[" + d.tbWeek.WeekName + "]" + d.tbPeriod.PeriodName));
                }

                if (vm.SubjectId > 0)
                {
                    vm.DataList = vm.DataList.Where(d => d.SubjectId == vm.SubjectId).ToList();
                }

                vm.DataList.OrderBy(d => d.StudentCode);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrgStudentByClassList(Models.Class.OrgStudentByClassList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("OrgStudentByClassList", new
            {
                searchText = vm.SearchText,
                SubjectId = vm.SubjectId,
                ClassId = vm.ClassId,
                yearId = vm.YearId
            }));
        }

        public ActionResult ExportOrgStudentByClassList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Class.OrgStudentByClassList();

                var teacher = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.tbSysUser.Id == XkSystem.Code.Common.UserId).FirstOrDefault();
                if (teacher == null)
                {
                    return Content("<script>alert('你不是班主任！');</script>");
                }
                var classIds = SelectClassByTeacherList(0, teacher.Id).Select(d => d.Value.ConvertToInt()).ToList();
                var tb = db.Table<Entity.tbClassStudent>();
                if (vm.ClassId > 0)
                {
                    tb = tb.Where(d => d.tbClass.Id == vm.ClassId);
                }
                else
                {
                    tb = tb.Where(d => classIds.Contains(d.tbClass.Id));
                }
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentName.Contains(vm.SearchText)
                          || d.tbStudent.StudentCode.Contains(vm.SearchText));
                }
                //行政班模式
                var tempList = tb.Select(d => new
                {
                    StudentId = d.tbStudent.Id,
                    ClassId = d.tbClass.Id,
                    StudentCode = d.tbStudent.StudentCode,
                    StudentName = d.tbStudent.StudentName
                }).ToList();

                var classOrgList = db.Table<Course.Entity.tbOrg>()
                    .Include(d => d.tbClass).Include(d => d.tbRoom).Include(d => d.tbCourse.tbSubject)
                    .Where(d => classIds.Contains(d.tbClass.Id)).ToList();

                foreach (var v in tempList)
                {
                    foreach (var vv in classOrgList.Where(d => d.tbClass.Id == v.ClassId))
                    {
                        vm.DataList.Add(new Dto.Class.OrgStudentByClassList()
                        {
                            OrgName = vv.OrgName,
                            OrgId = vv.Id,
                            RoomName = vv.tbRoom != null ? vv.tbRoom.RoomName : "",
                            StudentId = v.StudentId,
                            StudentCode = v.StudentCode,
                            StudentName = v.StudentName,
                            SubjectId = vv.tbCourse.tbSubject != null ? vv.tbCourse.tbSubject.Id : 0,
                            SubjectName = vv.tbCourse.tbSubject != null ? vv.tbCourse.tbSubject.SubjectName : ""
                        });
                    }
                }

                //班级学生
                var classStudentIds = tempList.Select(d => d.StudentId).ToList();

                //走班模式
                var tempDataList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                    where p.tbOrg.IsClass == false
                                    && classStudentIds.Contains(p.tbStudent.Id)
                                    orderby p.No
                                    select new Dto.Class.OrgStudentByClassList()
                                    {
                                        OrgName = p.tbOrg.OrgName,
                                        OrgId = p.tbOrg.Id,
                                        RoomName = p.tbOrg.tbRoom != null ? p.tbOrg.tbRoom.RoomName : "",
                                        StudentId = p.tbStudent.Id,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        SubjectId = p.tbOrg.tbCourse.tbSubject != null ? p.tbOrg.tbCourse.tbSubject.Id : 0,
                                        SubjectName = p.tbOrg.tbCourse.tbSubject != null ? p.tbOrg.tbCourse.tbSubject.SubjectName : ""
                                    }).ToList();
                vm.DataList.AddRange(tempDataList);

                var orgIds = vm.DataList.Select(c => c.OrgId).Distinct().ToList();

                var orgTeacherList = db.Table<Course.Entity.tbOrgTeacher>()
                    .Include(d => d.tbOrg).Include(d => d.tbTeacher)
                    .Where(d => orgIds.Contains(d.tbOrg.Id)).ToList();

                var orgScheduleList = db.Table<Course.Entity.tbOrgSchedule>()
                    .Include(d => d.tbOrg).Include(d => d.tbPeriod).Include(d => d.tbWeek)
                    .OrderBy(d => d.tbWeek.No)
                    .Where(d => orgIds.Contains(d.tbOrg.Id)).ToList();

                foreach (var v in vm.DataList)
                {
                    v.TeacherCode = string.Join(",", orgTeacherList.Where(d => d.tbOrg.Id == v.OrgId).Select(d => d.tbTeacher.TeacherCode).Distinct());
                    v.TeacherName = string.Join(",", orgTeacherList.Where(d => d.tbOrg.Id == v.OrgId).Select(d => d.tbTeacher.TeacherName).Distinct());
                    v.WeekPeriod = string.Join(",", orgScheduleList.Where(d => d.tbOrg.Id == v.OrgId).Select(d => "[" + d.tbWeek.WeekName + "]" + d.tbPeriod.PeriodName));
                }

                if (vm.SubjectId > 0)
                {
                    vm.DataList = vm.DataList.Where(d => d.SubjectId == vm.SubjectId).ToList();
                }

                vm.DataList.OrderBy(d => d.StudentCode);

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("科目"),
                        new System.Data.DataColumn("教学班"),
                        new System.Data.DataColumn("任课教师"),
                        new System.Data.DataColumn("任课教师编号"),
                        new System.Data.DataColumn("教室"),
                        new System.Data.DataColumn("星期节次")
                    });
                foreach (var a in vm.DataList)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["科目"] = a.SubjectName;
                    dr["教学班"] = a.OrgName;
                    dr["任课教师"] = a.TeacherName;
                    dr["任课教师编号"] = a.TeacherCode;
                    dr["教室"] = a.RoomName;
                    dr["星期节次"] = a.WeekPeriod;
                    dt.Rows.Add(dr);
                }

                var file = System.IO.Path.GetTempFileName();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbClass>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                            .Include(d => d.tbClass)
                                        where ids.Contains(p.tbClass.Id)
                                        select p).ToList();

                var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                            .Include(d => d.tbClass)
                                        where ids.Contains(p.tbClass.Id)
                                        select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    foreach (var student in classStudentList.Where(d => d.tbClass.Id == a.Id))
                    {
                        student.IsDeleted = true;
                    }

                    foreach (var teacher in classTeacherList.Where(d => d.tbClass.Id == a.Id))
                    {
                        teacher.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了行政班");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Class.Edit();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
                if (vm.ClassEdit.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.ClassEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                vm.ClassTypeList = Basis.Controllers.ClassTypeController.SelectList();
                vm.RoomList = Basis.Controllers.RoomController.SelectList(0, 0, true);
                vm.TeacherList = Teacher.Controllers.TeacherController.SelectList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbClass>()
                              where p.Id == id
                              select new Dto.Class.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  ClassName = p.ClassName,
                                  ClassTypeId = p.tbClassType.Id,
                                  GradeId = p.tbGrade.Id,
                                  YearId = p.tbYear.Id,
                                  RoomId = p.tbRoom.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ClassEdit = tb;
                        tb.TeacherId = db.Table<Basis.Entity.tbClassTeacher>().Where(d => d.tbClass.Id == tb.Id && d.tbTeacher.IsDeleted == false).Select(d => d.tbTeacher.Id).FirstOrDefault();
                    }
                }
                else
                {
                    vm.ClassEdit.No = (from p in db.Table<Entity.tbClass>() select p.No).DefaultIfEmpty(0).Max() + 1;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Class.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Basis.Entity.tbClass>().Where(d => d.ClassName == vm.ClassEdit.ClassName && d.tbYear.Id == vm.ClassEdit.YearId && d.Id != vm.ClassEdit.Id).Any())
                    {
                        error.AddError("该班级已存在!");
                    }
                    else
                    {
                        if (vm.ClassEdit.Id == 0)
                        {
                            var tb = new Basis.Entity.tbClass();
                            tb.No = vm.ClassEdit.No > 0 ? vm.ClassEdit.No : (from p in db.Table<Entity.tbClass>() select p.No).DefaultIfEmpty(0).Max() + 1;
                            tb.ClassName = vm.ClassEdit.ClassName;
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.ClassEdit.YearId);
                            tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.ClassEdit.GradeId);
                            tb.tbClassType = db.Set<Basis.Entity.tbClassType>().Find(vm.ClassEdit.ClassTypeId);
                            tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.ClassEdit.RoomId);
                            db.Set<Basis.Entity.tbClass>().Add(tb);

                            if (vm.ClassEdit.TeacherId != null)
                            {
                                var classTeacher = new Basis.Entity.tbClassTeacher();
                                classTeacher.tbClass = tb;
                                classTeacher.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.ClassEdit.TeacherId);
                                db.Set<Basis.Entity.tbClassTeacher>().Add(classTeacher);

                                #region tbSysUserRole:新增角色成员
                                var tbUserRole = new Sys.Entity.tbSysUserRole();
                                tbUserRole.tbSysRole = db.Table<Sys.Entity.tbSysRole>().Where(d => d.RoleCode == Code.EnumHelper.SysRoleCode.ClassTeacher).FirstOrDefault();
                                var teacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.ClassEdit.TeacherId);
                                tbUserRole.tbSysUser = db.Table<Sys.Entity.tbSysUser>().Where(d => d.UserCode == teacher.TeacherCode).FirstOrDefault();
                                db.Set<Sys.Entity.tbSysUserRole>().Add(tbUserRole);
                                #endregion
                            }

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了班主任");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Basis.Entity.tbClass>()
                                      where p.Id == vm.ClassEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.ClassEdit.No > 0 ? vm.ClassEdit.No : (from p in db.Table<Entity.tbClass>() select p.No).DefaultIfEmpty(0).Max() + 1;
                                tb.ClassName = vm.ClassEdit.ClassName;
                                tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.ClassEdit.YearId);
                                tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.ClassEdit.GradeId);
                                tb.tbClassType = db.Set<Basis.Entity.tbClassType>().Find(vm.ClassEdit.ClassTypeId);
                                tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.ClassEdit.RoomId);

                                if (vm.ClassEdit.TeacherId != null)
                                {
                                    var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                                                .Include(d => d.tbTeacher)
                                                            where p.tbClass.Id == tb.Id
                                                            select p).ToList();
                                    if (classTeacherList.Where(d => d.tbTeacher.Id == vm.ClassEdit.TeacherId).Count() == 0)
                                    {
                                        foreach (var a in classTeacherList)
                                        {
                                            a.IsDeleted = true;
                                        }

                                        var classTeacher = new Basis.Entity.tbClassTeacher();
                                        classTeacher.tbClass = tb;
                                        classTeacher.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.ClassEdit.TeacherId);
                                        db.Set<Basis.Entity.tbClassTeacher>().Add(classTeacher);

                                        #region tbSysUserRole:新增角色成员
                                        var teacherCode = db.Set<Teacher.Entity.tbTeacher>().Find(vm.ClassEdit.TeacherId).TeacherCode;

                                        var deleteUserRoleTb = (from p in db.Table<Sys.Entity.tbSysUserRole>()
                                                                where p.tbSysRole.RoleCode == Code.EnumHelper.SysRoleCode.ClassTeacher && p.tbSysUser.UserCode == teacherCode
                                                                select p).ToList();

                                        foreach (var a in deleteUserRoleTb)
                                        {
                                            a.IsDeleted = true;
                                        }

                                        var tbUserRole = new Sys.Entity.tbSysUserRole();
                                        tbUserRole.tbSysRole = db.Table<Sys.Entity.tbSysRole>().Where(d => d.RoleCode == Code.EnumHelper.SysRoleCode.ClassTeacher).FirstOrDefault();
                                        tbUserRole.tbSysUser = db.Table<Sys.Entity.tbSysUser>().Where(d => d.UserCode == teacherCode).FirstOrDefault();
                                        db.Set<Sys.Entity.tbSysUserRole>().Add(tbUserRole);
                                        #endregion
                                    }
                                }
                                else
                                {
                                    var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                                                .Include(d => d.tbTeacher)
                                                            where p.tbClass.Id == tb.Id
                                                            select p).ToList();
                                    if (classTeacherList.Where(d => d.tbTeacher.Id == vm.ClassEdit.TeacherId).Count() == 0)
                                    {
                                        foreach (var a in classTeacherList)
                                        {
                                            a.IsDeleted = true;
                                        }
                                    }
                                }

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了班主任");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }


        /// <summary>
        /// 根据学段类型、Id及年级获取对应学年下的所有班级
        /// </summary>
        /// <param name="yearType">类型[学年、学期、学段]</param>
        /// <param name="id"></param>
        /// <param name="gradeId"></param>
        /// <returns></returns>
        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectListByYearType(Code.EnumHelper.YearType yearType, int id = 0, int gradeId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {

                int yearId = 0;
                var tbYear = (from y in db.Table<Basis.Entity.tbYear>() select y);

                switch (yearType)
                {
                    case Code.EnumHelper.YearType.Year:
                        yearId = id;
                        break;
                    case Code.EnumHelper.YearType.Term:
                        yearId = (from p in tbYear where p.Id == id select p.tbYearParent.Id).FirstOrDefault();
                        break;
                    case Code.EnumHelper.YearType.Section:
                        yearId = (from p in tbYear where p.Id == id select p.tbYearParent.tbYearParent.Id).FirstOrDefault();
                        break;
                }

                var list = (from p in db.Table<Basis.Entity.tbClass>()
                            where p.tbYear.Id == yearId
                                && (p.tbGrade.Id == gradeId || gradeId == 0)
                            orderby p.tbGrade.No, p.No
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.ClassName + "[" + p.tbClassType.ClassTypeName + "]",
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }


        [NonAction]
        public static List<Dto.Class.Info> SelectInfoListByYearType(Code.EnumHelper.YearType yearType, int yearId = 0)
        {
            int _yearId = 0;
            using (var db = new XkSystem.Models.DbContext())
            {
                if (yearId == 0)
                {
                    var year = (from p in db.Table<Basis.Entity.tbYear>()
                                orderby p.IsDefault descending
                                select new
                                {
                                    p.tbYearParent.tbYearParent.Id
                                }).FirstOrDefault();
                    if (year != null)
                    {
                        _yearId = year.Id;
                    }
                }
                else
                {
                    var tbYear = (from y in db.Table<Basis.Entity.tbYear>() select y);
                    switch (yearType)
                    {
                        case Code.EnumHelper.YearType.Year:
                            _yearId = yearId;
                            break;
                        case Code.EnumHelper.YearType.Term:
                            _yearId = (from p in tbYear where p.Id == yearId select p.tbYearParent.Id).FirstOrDefault();
                            break;
                        case Code.EnumHelper.YearType.Section:
                            _yearId = (from p in tbYear where p.Id == yearId select p.tbYearParent.tbYearParent.Id).FirstOrDefault();
                            break;
                    }
                }

                var list = (from p in db.Table<Basis.Entity.tbClass>()
                            where p.tbYear.Id == _yearId
                            orderby p.tbGrade.No, p.No
                            select new Dto.Class.Info
                            {
                                Id = p.Id,
                                No = p.No,
                                ClassName = p.ClassName
                            }).ToList();
                return list;
            }
        }


        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int yearId = 0, int? gradeId = 0, int id = 0, string SearchText = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (yearId == 0)
                {
                    yearId = YearController.GetDefaultYearId(db);
                }

                var list = (from p in db.Table<Basis.Entity.tbClass>()
                            where p.tbYear.Id == yearId
                                && (p.tbGrade.Id == gradeId || gradeId == 0)
                                && (p.Id == id || id == 0)
                                && (p.ClassName.Contains(SearchText) || string.IsNullOrEmpty(SearchText))
                            orderby p.tbGrade.No, p.No
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.ClassName + "[" + p.tbClassType.ClassTypeName + "]",
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectClassList(int yearId = 0, int? gradeId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (yearId == 0)
                {
                    var year = (from p in db.Table<Basis.Entity.tbYear>()
                                orderby p.IsDefault descending
                                select new
                                {
                                    p.tbYearParent.tbYearParent.Id
                                }).FirstOrDefault();
                    if (year != null)
                    {
                        yearId = year.Id;
                    }
                }

                var list = (from p in db.Table<Basis.Entity.tbClass>()
                            where p.tbYear.Id == yearId
                                && (p.tbGrade.Id == gradeId || gradeId == 0)
                            orderby p.tbGrade.No, p.No
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.ClassName,
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectClassList(int yearId = 0, int teacherId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (yearId == 0)
                {
                    var year = (from p in db.Table<Basis.Entity.tbYear>()
                                orderby p.IsDefault descending
                                select new
                                {
                                    p.tbYearParent.tbYearParent.Id
                                }).FirstOrDefault();
                    if (year != null)
                    {
                        yearId = year.Id;
                    }
                }

                var list = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                            where p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                && (p.tbTeacher.Id == teacherId || teacherId == 0)
                            orderby p.tbClass.tbGrade.No, p.tbClass.No
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.tbClass.ClassName + "[" + p.tbClass.tbClassType.ClassTypeName + "]",
                                Value = p.tbClass.Id.ToString()
                            }).ToList();
                return list;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectClassByTeacherList(int yearId = 0, int teacherId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (yearId == 0)
                {
                    var year = (from p in db.Table<Basis.Entity.tbYear>()
                                orderby p.IsDefault descending
                                select new
                                {
                                    p.tbYearParent.tbYearParent.Id
                                }).FirstOrDefault();
                    if (year != null)
                    {
                        yearId = year.Id;
                    }
                }

                var list = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                            where p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                && (p.tbTeacher.Id == teacherId)
                            orderby p.tbClass.tbGrade.No, p.tbClass.No
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.tbClass.ClassName + "[" + p.tbClass.tbClassType.ClassTypeName + "]",
                                Value = p.tbClass.Id.ToString()
                            }).ToList();
                return list;
            }
        }

        [NonAction]
        public static List<Dto.Class.Info> SelectInfoList(int yearId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (yearId == 0)
                {
                    var year = (from p in db.Table<Basis.Entity.tbYear>()
                                orderby p.IsDefault descending
                                select new
                                {
                                    p.tbYearParent.tbYearParent.Id
                                }).FirstOrDefault();
                    if (year != null)
                    {
                        yearId = year.Id;
                    }
                }

                var list = (from p in db.Table<Basis.Entity.tbClass>()
                            where p.tbYear.Id == yearId
                            orderby p.tbGrade.No, p.No
                            select new Dto.Class.Info
                            {
                                Id = p.Id,
                                No = p.No,
                                ClassName = p.ClassName
                            }).ToList();
                return list;
            }
        }

        [NonAction]
        public static Dto.Class.Info SelectInfoByStudentUserId(int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                int yearId = 0;
                var year = (from p in db.Table<Basis.Entity.tbYear>()
                            orderby p.IsDefault descending
                            select new
                            {
                                p.tbYearParent.tbYearParent.Id
                            }).FirstOrDefault();
                if (year != null)
                {
                    yearId = year.Id;
                }

                var tb = (from p in db.Table<Basis.Entity.tbClassStudent>()
                          where p.tbStudent.tbSysUser.Id == userId
                              && p.tbClass.tbYear.Id == yearId
                          orderby p.tbClass.No, p.No
                          select new Dto.Class.Info
                          {
                              Id = p.tbClass.Id,
                              No = p.tbClass.No,
                              ClassName = p.tbClass.ClassName + "[" + p.tbClass.tbClassType.ClassTypeName + "]",
                          }).FirstOrDefault();

                return tb;
            }
        }

        public ActionResult Import()
        {
            var vm = new Models.Class.Import();
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.Class.Import vm)
        {
            vm.ImportClassList = new List<Dto.Class.ImportClass>();
            vm.ImportStudentList = new List<Dto.Class.ImportStudent>();
            vm.ImportClassTypeList = new List<Dto.Class.ImportClassType>();
            vm.ImportGradeList = new List<Dto.Class.ImportGrade>();
            vm.ImportTeacherList = new List<Dto.Class.ImportTeacher>();
            vm.ImportRoomList = new List<Dto.Class.ImportRoom>();
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
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
                    var tbList = new List<string>() { "学生学号", "学生姓名", "座位号", "排序", "班级名称", "年级", "班级类型", "班主任", "班级教室" };

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
                        var dtoClass = new Dto.Class.ImportClass()
                        {
                            //No = Convert.ToString(dr["排序"]),
                            ClassName = Convert.ToString(dr["班级名称"]),
                            GradeName = Convert.ToString(dr["年级"]),
                            ClassTypeName = Convert.ToString(dr["班级类型"]),
                            TeacherName = Convert.ToString(dr["班主任"]),
                            RoomName = Convert.ToString(dr["班级教室"])
                        };
                        int no = 0;
                        if (int.TryParse(dr["排序"].ConvertToString(), out no) == false)
                        {
                            dtoClass.Error += "排序必须是正整数!";
                        }
                        else
                        {
                            dtoClass.No = no;
                        }
                        var dtoStudent = new Dto.Class.ImportStudent()
                        {
                            StudentCode = Convert.ToString(dr["学生学号"]),
                            StudentName = Convert.ToString(dr["学生姓名"]),
                            No = Convert.ToString(dr["座位号"]),
                            ClassName = Convert.ToString(dr["班级名称"])
                        };
                        var dtoClassType = new Dto.Class.ImportClassType()
                        {
                            ClassTypeName = Convert.ToString(dr["班级类型"])
                        };
                        var dtoGrade = new Dto.Class.ImportGrade()
                        {
                            GradeName = Convert.ToString(dr["年级"])
                        };
                        var dtoTeacher = new Dto.Class.ImportTeacher()
                        {
                            TeacherCode = Convert.ToString(dr["班主任教职工号"]),
                            TeacherName = Convert.ToString(dr["班主任"])
                        };
                        var dtoRoom = new Dto.Class.ImportRoom()
                        {
                            RoomName = Convert.ToString(dr["班级教室"])
                        };
                        if (vm.ImportClassList.Where(d => d.No == dtoClass.No
                                                        && d.ClassName == dtoClass.ClassName
                                                        && d.GradeName == dtoClass.GradeName
                                                        && d.ClassTypeName == dtoClass.ClassTypeName
                                                        && d.TeacherName == dtoClass.TeacherName
                                                        && d.RoomName == dtoClass.RoomName).Count() == 0)
                        {
                            vm.ImportClassList.Add(dtoClass);
                        }
                        if (vm.ImportStudentList.Where(d => d.StudentCode == dtoStudent.StudentCode
                                                        && d.StudentName == dtoStudent.StudentName
                                                        && d.No == dtoStudent.No
                                                        && d.ClassName == dtoStudent.ClassName).Count() == 0)
                        {
                            vm.ImportStudentList.Add(dtoStudent);
                        }
                        if (vm.ImportClassTypeList.Where(d => d.ClassTypeName == dtoClassType.ClassTypeName).Count() == 0)
                        {
                            vm.ImportClassTypeList.Add(dtoClassType);
                        }
                        if (vm.ImportGradeList.Where(d => d.GradeName == dtoGrade.GradeName).Count() == 0)
                        {
                            vm.ImportGradeList.Add(dtoGrade);
                        }
                        if (vm.ImportTeacherList.Where(d => d.TeacherCode == dtoTeacher.TeacherCode
                                                         && d.TeacherName == dtoTeacher.TeacherName).Count() == 0)
                        {
                            vm.ImportTeacherList.Add(dtoTeacher);
                        }
                        if (vm.ImportRoomList.Where(d => d.RoomName == dtoRoom.RoomName).Count() == 0)
                        {
                            vm.ImportRoomList.Add(dtoRoom);
                        }
                    }
                    vm.ImportClassList.RemoveAll(d =>
                        d.No == 0 &&
                        string.IsNullOrEmpty(d.ClassName) &&
                        string.IsNullOrEmpty(d.GradeName) &&
                        string.IsNullOrEmpty(d.ClassTypeName) &&
                        string.IsNullOrEmpty(d.TeacherName) &&
                        string.IsNullOrEmpty(d.RoomName)
                    );
                    vm.ImportStudentList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.StudentCode) &&
                        string.IsNullOrEmpty(d.StudentName) &&
                        string.IsNullOrEmpty(d.No)
                    );
                    vm.ImportClassTypeList.RemoveAll(d => string.IsNullOrEmpty(d.ClassTypeName));
                    vm.ImportGradeList.RemoveAll(d => string.IsNullOrEmpty(d.GradeName));
                    vm.ImportTeacherList.RemoveAll(d => string.IsNullOrEmpty(d.TeacherCode)
                                                    && string.IsNullOrEmpty(d.TeacherName));
                    vm.ImportRoomList.RemoveAll(d => string.IsNullOrEmpty(d.RoomName));
                    #endregion

                    if (vm.ImportClassList.Count == 0 && vm.ImportStudentList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }

                    var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                         .Include(d => d.tbYear)
                                         .Include(d => d.tbGrade)
                                         .Include(d => d.tbClassType)
                                         .Include(d => d.tbRoom)
                                     where p.tbYear.Id == vm.YearId
                                     select p).ToList();
                    var gradeList = db.Table<Basis.Entity.tbGrade>().ToList();
                    var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>().ToList();
                    var classStudentList = db.Table<Basis.Entity.tbClassStudent>().Where(d => d.tbStudent.IsDeleted == false).ToList();
                    var classTypeList = db.Table<Basis.Entity.tbClassType>().ToList();
                    var yearList = db.Table<Basis.Entity.tbYear>().ToList();
                    var roomList = db.Table<Basis.Entity.tbRoom>().ToList();
                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();
                    var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                       select p).ToList();

                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportClassList)
                    {
                        if (string.IsNullOrEmpty(item.ClassName))
                        {
                            item.Error = item.Error + "班级名称不能为空!";
                        }
                        if (string.IsNullOrEmpty(item.ClassTypeName))
                        {
                            item.Error = item.Error + "班级类型不能为空!";
                        }
                        if (string.IsNullOrEmpty(item.GradeName))
                        {
                            item.Error = item.Error + "年级名称不能为空!";
                        }
                        if (vm.IsUpdate)
                        {
                            if (classList.Where(d => d.ClassName == item.ClassName).Count() > 1)
                            {
                                item.Error += "系统中该班级数据存在重复，无法确认需要更新的记录!";
                            }
                            if (classList.Where(d => d.ClassName == item.ClassName
                            && (d.tbClassType.ClassTypeName != item.ClassTypeName || d.tbGrade.GradeName != item.GradeName)).Count() > 0)
                            {
                                item.Error += "系统中存在该班级名，并且班级类型和年级不相同，请确认！";
                            }
                        }
                        else
                        {
                            if (classList.Where(d => d.ClassName == item.ClassName).Count() > 0)
                            {
                                item.Error += "系统中已存在该记录!";
                            }
                        }
                    }
                    foreach (var item in vm.ImportStudentList)
                    {
                        if (string.IsNullOrEmpty(item.StudentName))
                        {
                            item.Error += "学生姓名不能为空！";
                        }

                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error += "学号不能为空！";
                        }

                        if (studentList.Where(d => d.StudentCode == item.StudentCode).Count() == 0)
                        {
                            item.Error += "学生不存在!";
                        }

                        if (studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Count() == 0)
                        {
                            item.Error += "学号和学生姓名不匹配！";
                        }

                        if (classStudentList.Where(d => d.tbStudent.StudentCode == item.StudentCode && d.tbClass.ClassName != item.ClassName).Count() > 0)
                        {
                            item.Error += "学生已存在其他行政班内！";
                        }

                        if (studentList.Where(d => d.StudentCode == item.StudentCode).Count() > 1)
                        {
                            item.Error += "学生学号对应多条学生记录，无法确认需要添加的记录!";
                        }
                    }
                    if (vm.ImportClassList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0
                        || vm.ImportStudentList.Where(d => !string.IsNullOrEmpty(d.Error)).Count() > 0)
                    {
                        vm.ImportClassList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        vm.ImportStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        vm.ImportClassTypeList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        vm.ImportGradeList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        vm.ImportRoomList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        vm.ImportTeacherList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 数据导入，判断关键字，存在的数据做更新，不存在的做新增
                    foreach (var item in vm.ImportClassList)
                    {
                        Basis.Entity.tbClass tb = null;
                        if (classList.Where(d => d.ClassName == item.ClassName && d.tbClassType.ClassTypeName == item.ClassTypeName && d.tbGrade.GradeName == item.GradeName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                #region 修改行政班
                                tb = classList.Where(d => d.ClassName == item.ClassName && d.tbClassType.ClassTypeName == item.ClassTypeName && d.tbGrade.GradeName == item.GradeName).FirstOrDefault();

                                tb.No = item.No;
                                tb.tbYear = yearList.Where(d => d.Id == vm.YearId).FirstOrDefault();
                                tb.ClassName = item.ClassName;
                                tb.tbGrade = gradeList.Where(d => d.GradeName.Equals(item.GradeName)).FirstOrDefault();
                                tb.tbClassType = classTypeList.Where(d => d.ClassTypeName.Equals(item.ClassTypeName)).FirstOrDefault();
                                if (string.IsNullOrEmpty(item.RoomName) == false)
                                {
                                    tb.tbRoom = roomList.Where(d => d.RoomName == item.RoomName).FirstOrDefault();
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            #region 新增行政班
                            tb = new Basis.Entity.tbClass();
                            tb.No = item.No;
                            tb.tbYear = yearList.Where(d => d.Id == vm.YearId).FirstOrDefault();
                            tb.ClassName = item.ClassName;
                            tb.tbGrade = gradeList.Where(d => d.GradeName.Equals(item.GradeName)).FirstOrDefault();
                            tb.tbClassType = classTypeList.Where(d => d.ClassTypeName.Equals(item.ClassTypeName)).FirstOrDefault();
                            if (string.IsNullOrEmpty(item.RoomName) == false)
                            {
                                tb.tbRoom = roomList.Where(d => d.RoomName == item.RoomName).FirstOrDefault();
                            }

                            db.Set<Basis.Entity.tbClass>().Add(tb);
                            #endregion
                        }

                        if (!string.IsNullOrEmpty(item.TeacherName))
                        {
                            #region 添加班主任
                            if (teacherList.Where(d => d.TeacherName == item.TeacherName).Count() > 0)
                            {
                                if (classTeacherList.Where(d => d.tbClass.ClassName == item.ClassName && d.tbTeacher.TeacherCode == item.TeacherName).Count() == 0)
                                {
                                    var classTeacher = new Basis.Entity.tbClassTeacher()
                                    {
                                        tbClass = tb,
                                        tbTeacher = teacherList.Where(d => d.TeacherName == item.TeacherName).FirstOrDefault(),
                                    };
                                    db.Set<Basis.Entity.tbClassTeacher>().Add(classTeacher);
                                }
                            }
                            else
                            {
                                item.Error = "系统中无此老师信息，请核对添加!";
                                vm.ImportClassList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                vm.ImportStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                vm.ImportClassTypeList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                vm.ImportGradeList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                vm.ImportRoomList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                vm.ImportTeacherList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                return View(vm);
                            }
                            #endregion
                        }
                        if (vm.ImportStudentList.Where(d => d.ClassName == item.ClassName).Count() > 0)
                        {
                            #region 添加班级学生
                            foreach (var itemStudent in vm.ImportStudentList.Where(d => d.ClassName == item.ClassName))
                            {
                                if (classStudentList.Where(d => d.tbClass.ClassName == itemStudent.ClassName && d.tbStudent.StudentCode == itemStudent.StudentCode).Count() == 0)
                                {
                                    var classStudent = new Basis.Entity.tbClassStudent()
                                    {
                                        tbClass = tb,
                                        tbStudent = studentList.Where(d => d.StudentCode == itemStudent.StudentCode).FirstOrDefault(),
                                        No = string.IsNullOrEmpty(itemStudent.No) ? 0 : Convert.ToInt32(itemStudent.No),
                                    };
                                    db.Set<Basis.Entity.tbClassStudent>().Add(classStudent);
                                }
                                else
                                {
                                    var classStudent = classStudentList.Where(d => d.tbClass.ClassName == itemStudent.ClassName && d.tbStudent.StudentCode == itemStudent.StudentCode).FirstOrDefault();
                                    classStudent.No = itemStudent.No.ConvertToInt();
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了行政班");
                        vm.Status = true;
                    }
                }
            }
            vm.ImportClassList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            vm.ImportStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Basis/Views/Class/ClassTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult Export()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                 select new
                                 {
                                     p.ClassName,
                                     p.tbGrade.GradeName,
                                     p.tbClassType.ClassTypeName,
                                 }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("年级名称"),
                        new System.Data.DataColumn("班级名称"),
                        new System.Data.DataColumn("班级类型"),
                    });
                foreach (var a in classList)
                {
                    var dr = dt.NewRow();
                    dr["年级名称"] = a.ClassName;
                    dr["班级名称"] = a.GradeName;
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


        public ActionResult ImportClass()
        {
            var vm = new Models.Class.ImportClass();
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
            return View(vm);
        }

        public ActionResult ImportClassTemplate()
        {
            var file = Server.MapPath("~/Areas/Basis/Views/Class/ClassTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportClass(Models.Class.ImportClass vm)
        {
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
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
                    var tbList = new List<string>() { "班序", "班级名称", "年级", "班级类型", "班主任", "班级教室" };

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
                        var dtoClass = new Dto.Class.ImportClass()
                        {
                            //No = Convert.ToString(dr["班序"]),
                            ClassName = Convert.ToString(dr["班级名称"]),
                            GradeName = Convert.ToString(dr["年级"]),
                            ClassTypeName = Convert.ToString(dr["班级类型"]),
                            TeacherName = Convert.ToString(dr["班主任"]),
                            RoomName = Convert.ToString(dr["班级教室"])
                        };
                        int no = 0;
                        if (int.TryParse(dr["班序"].ConvertToString(), out no) == false)
                        {
                            dtoClass.Error += "班序必须是正整数!";
                        }
                        else
                        {
                            dtoClass.No = no;
                        }

                        if (vm.ImportClassList.Where(d => d.No == dtoClass.No
                                                        && d.ClassName == dtoClass.ClassName
                                                        && d.GradeName == dtoClass.GradeName
                                                        && d.ClassTypeName == dtoClass.ClassTypeName
                                                        && d.TeacherName == dtoClass.TeacherName
                                                        && d.RoomName == dtoClass.RoomName).Count() == 0)
                        {
                            vm.ImportClassList.Add(dtoClass);
                        }
                    }
                    vm.ImportClassList.RemoveAll(d =>
                        d.No == 0 &&
                        string.IsNullOrEmpty(d.ClassName) &&
                        string.IsNullOrEmpty(d.GradeName) &&
                        string.IsNullOrEmpty(d.ClassTypeName) &&
                        string.IsNullOrEmpty(d.TeacherName) &&
                        string.IsNullOrEmpty(d.RoomName)
                    );
                    if (vm.ImportClassList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    if (vm.ImportClassList.Select(d => d.No).Distinct().Count() < vm.ImportClassList.Count)
                    {
                        ModelState.AddModelError("", "班序不能重复!");
                        return View(vm);
                    }
                    #endregion

                    var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                         .Include(d => d.tbYear)
                                         .Include(d => d.tbGrade)
                                         .Include(d => d.tbClassType)
                                         .Include(d => d.tbRoom)
                                     where p.tbYear.Id == vm.YearId
                                     select p).ToList();
                    var gradeList = db.Table<Basis.Entity.tbGrade>().ToList();
                    var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>().ToList();
                    var classTypeList = db.Table<Basis.Entity.tbClassType>().ToList();
                    var yearList = db.Table<Basis.Entity.tbYear>().ToList();
                    var roomList = db.Table<Basis.Entity.tbRoom>().ToList();
                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();

                    #region 验证数据格式是否正确

                    foreach (var item in vm.ImportClassList)
                    {
                        if (classList.Where(d => d.No == item.No).Any())
                        {
                            item.Error += "班序不能重复！";
                        }
                        if (string.IsNullOrEmpty(item.ClassName))
                        {
                            item.Error = item.Error + "班级名称不能为空!";
                        }

                        if (string.IsNullOrEmpty(item.ClassTypeName))
                        {
                            item.Error = item.Error + "班级类型不能为空!";
                        }
                        else
                        {
                            if (classTypeList.Where(d => d.ClassTypeName == item.ClassTypeName).Any() == false)
                            {
                                item.Error = item.Error + "班级类型不存在!";
                            }
                        }

                        if (string.IsNullOrEmpty(item.GradeName))
                        {
                            item.Error = item.Error + "年级名称不能为空!";
                        }
                        else
                        {
                            if (gradeList.Where(d => d.GradeName == item.GradeName).Any() == false)
                            {
                                item.Error = item.Error + "年级不存在!";
                            }
                        }

                        if (!string.IsNullOrEmpty(item.TeacherName) && teacherList.Where(d => d.TeacherName == item.TeacherName).Count() == 0)
                        {
                            item.Error += "教师不存在；";
                        }
                        if (vm.IsUpdate)
                        {
                            if (classList.Where(d => d.ClassName == item.ClassName).Count() > 1)
                            {
                                item.Error += "系统中该班级数据存在重复，无法确认需要更新的记录!";
                            }

                            if (classList.Where(d => d.ClassName == item.ClassName
                            && (d.tbClassType.ClassTypeName != item.ClassTypeName || d.tbGrade.GradeName != item.GradeName)).Count() > 0)
                            {
                                item.Error += "系统中存在该班级名，并且班级类型和年级不相同，请确认！";
                            }
                        }
                        else
                        {
                            if (classList.Where(d => d.ClassName == item.ClassName).Count() > 0)
                            {
                                item.Error += "系统中已存在该记录!";
                            }
                        }
                    }

                    if (vm.ImportClassList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportClassList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 数据导入，判断关键字，存在的数据做更新，不存在的做新增
                    var addClassList = new List<Basis.Entity.tbClass>();
                    var addClassTeacherList = new List<Basis.Entity.tbClassTeacher>();
                    foreach (var item in vm.ImportClassList)
                    {
                        Basis.Entity.tbClass tb = null;
                        if (classList.Where(d => d.ClassName == item.ClassName && d.tbClassType.ClassTypeName == item.ClassTypeName && d.tbGrade.GradeName == item.GradeName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                #region 修改行政班
                                tb = classList.Where(d => d.ClassName == item.ClassName && d.tbClassType.ClassTypeName == item.ClassTypeName && d.tbGrade.GradeName == item.GradeName).FirstOrDefault();

                                tb.No = item.No;
                                tb.tbYear = yearList.Where(d => d.Id == vm.YearId).FirstOrDefault();
                                tb.ClassName = item.ClassName;

                                #region 添加外键
                                if (!string.IsNullOrEmpty(item.GradeName))
                                {
                                    tb.tbGrade = gradeList.Where(d => d.GradeName.Equals(item.GradeName)).FirstOrDefault();
                                }
                                if (!string.IsNullOrEmpty(item.ClassTypeName))
                                {
                                    tb.tbClassType = classTypeList.Where(d => d.ClassTypeName.Equals(item.ClassTypeName)).FirstOrDefault();
                                }
                                if (!string.IsNullOrEmpty(item.RoomName))
                                {
                                    tb.tbRoom = roomList.Where(d => d.RoomName == item.RoomName).FirstOrDefault();
                                }
                                #endregion

                                #endregion
                            }
                        }
                        else
                        {
                            #region 新增行政班
                            tb = new Basis.Entity.tbClass();
                            tb.No = item.No;
                            tb.tbYear = yearList.Where(d => d.Id == vm.YearId).FirstOrDefault();
                            tb.ClassName = item.ClassName;

                            #region 添加外键
                            if (!string.IsNullOrEmpty(item.GradeName))
                            {
                                tb.tbGrade = gradeList.Where(d => d.GradeName.Equals(item.GradeName)).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.ClassTypeName))
                            {
                                tb.tbClassType = classTypeList.Where(d => d.ClassTypeName.Equals(item.ClassTypeName)).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.RoomName))
                            {
                                tb.tbRoom = roomList.Where(d => d.RoomName == item.RoomName).FirstOrDefault();
                            }
                            #endregion

                            addClassList.Add(tb);
                            #endregion
                        }

                        if (!string.IsNullOrEmpty(item.TeacherName))
                        {
                            if (classTeacherList.Where(d => d.tbClass.ClassName == item.ClassName && d.tbTeacher.TeacherName == item.TeacherName).Count() == 0)
                            {
                                var classTeacher = new Basis.Entity.tbClassTeacher();
                                classTeacher.tbClass = tb;
                                classTeacher.tbTeacher = teacherList.Where(d => d.TeacherName == item.TeacherName).FirstOrDefault();
                                addClassTeacherList.Add(classTeacher);
                            }
                        }
                    }
                    #endregion

                    db.Set<Basis.Entity.tbClass>().AddRange(addClassList);
                    db.Set<Basis.Entity.tbClassTeacher>().AddRange(addClassTeacherList);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了行政班");
                        vm.Status = true;
                    }
                }
            }

            vm.ImportClassList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public ActionResult ExportClass()
        {
            var vm = new Models.Class.ExportClass();

            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tb = db.Table<Basis.Entity.tbClass>();
                if (vm.ClassTypeId != 0)
                {
                    tb = tb.Where(d => d.tbClassType.Id == vm.ClassTypeId);
                }
                if (vm.GradeId != 0)
                {
                    tb = tb.Where(d => d.tbGrade.Id == vm.GradeId);
                }
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.ClassName.Contains(vm.SearchText));
                }
                if (vm.YearId != 0)
                {
                    tb = tb.Where(d => d.tbYear.Id == vm.YearId);
                }

                vm.ExportClassList = (from p in tb
                                      select new Dto.Class.ExportClass
                                      {
                                          Id = p.Id,
                                          ClassName = p.ClassName,
                                          No = p.No,
                                          ClassTypeName = p.tbClassType.ClassTypeName,
                                          GradeName = p.tbGrade.GradeName,
                                          RoomName = p.tbRoom.RoomName,
                                          StudentCount = db.Set<Basis.Entity.tbClassStudent>().Where(d => d.IsDeleted == false && d.tbStudent.IsDeleted == false && d.tbClass.Id == p.Id).Count()
                                      }).ToList();

                var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                        where p.tbTeacher.IsDeleted == false
                                        select new
                                        {
                                            ClassId = p.tbClass.Id,
                                            p.tbTeacher.TeacherName
                                        }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("排序"),
                        new System.Data.DataColumn("班级名称"),
                        new System.Data.DataColumn("年级名称"),
                        new System.Data.DataColumn("班级类型"),
                        new System.Data.DataColumn("班主任"),
                        new System.Data.DataColumn("班级教室"),
                        new System.Data.DataColumn("学生人数"),
                    });
                foreach (var a in vm.ExportClassList)
                {
                    var dr = dt.NewRow();
                    dr["排序"] = a.No;
                    dr["班级名称"] = a.ClassName;
                    dr["年级名称"] = a.GradeName;
                    dr["班级类型"] = a.ClassTypeName;
                    dr["班主任"] = string.Join(",", classTeacherList.Where(d => d.ClassId == a.Id).Select(d => d.TeacherName).ToList());
                    dr["班级教室"] = a.RoomName;
                    dr["学生人数"] = a.StudentCount;
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

        public ActionResult ImportStudent()
        {
            var vm = new Models.Class.ImportStudent();
            return View(vm);
        }

        public ActionResult ImportStudentTemplate()
        {
            var file = Server.MapPath("~/Areas/Basis/Views/Class/StudentTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportStudent(Models.Class.ImportStudent vm)
        {
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
                    var tbList = new List<string>() { "学生学号", "学生姓名", "座位号", "班级名称", "小组名称" };

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
                        var dtoStudent = new Dto.Class.ImportStudent()
                        {
                            StudentCode = dr["学生学号"].ToString(),
                            StudentName = dr["学生姓名"].ToString(),
                            No = dr["座位号"].ToString(),
                            ClassGroupName = dr["小组名称"].ConvertToString(),
                            ClassName = dr["班级名称"].ToString()
                        };
                        if (vm.ImportStudentList.Where(d => d.StudentCode == dtoStudent.StudentCode
                                                        && d.StudentName == dtoStudent.StudentName
                                                        && d.No == dtoStudent.No
                                                        && d.ClassGroupName == dtoStudent.ClassGroupName
                                                        && d.ClassName == dtoStudent.ClassName).Count() == 0)
                        {
                            vm.ImportStudentList.Add(dtoStudent);
                        }
                    }
                    vm.ImportStudentList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.StudentCode) &&
                        string.IsNullOrEmpty(d.StudentName) &&
                        string.IsNullOrEmpty(d.No) &&
                        string.IsNullOrEmpty(d.ClassGroupName) &&
                        string.IsNullOrEmpty(d.ClassName)
                    );
                    if (vm.ImportStudentList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                         .Include(d => d.tbYear)
                                         .Include(d => d.tbGrade)
                                         .Include(d => d.tbClassType)
                                         .Include(d => d.tbRoom)
                                     where p.tbYear.Id == vm.YearId
                                     select p).ToList();
                    //var gradeList = db.Table<Basis.Entity.tbGrade>().ToList();
                    //var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>().ToList();
                    var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                .Include(d => d.tbClass)
                                            where p.tbClass.tbYear.Id == vm.YearId
                                                && p.tbStudent.IsDeleted == false
                                                && p.tbClass.IsDeleted == false
                                            select p).ToList();
                    //var classTypeList = db.Table<Basis.Entity.tbClassType>().ToList();
                    //var roomList = db.Table<Basis.Entity.tbRoom>().ToList();
                    //var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();
                    var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                       select p).ToList();
                    var classGroupList = db.Table<Basis.Entity.tbClassGroup>().Where(d => d.tbClass.tbYear.Id == vm.YearId).Include(d => d.tbClass).ToList();
                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();

                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportStudentList)
                    {
                        if (!vm.IsAddClass && classList.Where(d => d.ClassName == item.ClassName).Count() == 0)
                        {
                            item.Error += "班级不存在；";
                        }

                        if (string.IsNullOrEmpty(item.StudentName))
                        {
                            item.Error += "学生姓名不能为空！";
                        }

                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error += "学号不能为空！";
                        }

                        if (studentList.Where(d => d.StudentCode == item.StudentCode).Count() == 0)
                        {
                            item.Error += "学生不存在!";
                        }

                        if (studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Any() == false)
                        {
                            item.Error += "学号和学生姓名不匹配！";
                        }

                        if (classStudentList.Where(d => d.tbStudent.StudentCode == item.StudentCode && d.tbClass.ClassName != item.ClassName).Count() > 0)
                        {
                            item.Error += "学生已存在其他行政班内！";
                        }

                        if (studentList.Where(d => d.StudentCode == item.StudentCode).Count() > 1)
                        {
                            item.Error += "学生学号对应多条学生记录，无法确认需要添加的记录!";
                        }
                    }

                    if (vm.ImportStudentList.Where(d => !string.IsNullOrEmpty(d.Error)).Count() > 0)
                    {
                        vm.ImportStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }

                    #endregion

                    #region 自动添加班级、学生、行政班小组
                    var editClassList = new List<Dto.Class.Edit>();
                    foreach (var v in vm.ImportStudentList)
                    {
                        if (!string.IsNullOrEmpty(v.ClassName) && classList.Where(d => d.ClassName == v.ClassName).Count() == 0)
                        {
                            var editClass = new Dto.Class.Edit()
                            {
                                ClassName = v.ClassName
                            };
                            if (editClassList.Where(d => d.ClassName == editClass.ClassName).Count() == 0)
                            {
                                editClassList.Add(editClass);
                            }
                        }
                    }

                    var addClassList = new List<Basis.Entity.tbClass>();
                    if (editClassList.Count > 0)
                    {
                        addClassList = BuildList(db, editClassList);
                    }

                    #endregion

                    #region 添加班级学生
                    var addClassStudent = new List<Basis.Entity.tbClassStudent>();
                    foreach (var itemStudent in vm.ImportStudentList)
                    {
                        if (classStudentList.Where(d => d.tbClass.ClassName == itemStudent.ClassName && d.tbStudent.StudentCode == itemStudent.StudentCode).Count() == 0)
                        {
                            var classStudent = new Basis.Entity.tbClassStudent()
                            {
                                No = string.IsNullOrEmpty(itemStudent.No) ? 0 : Convert.ToInt32(itemStudent.No),
                            };

                            #region 添加外键

                            if (!string.IsNullOrEmpty(itemStudent.StudentCode) && !string.IsNullOrEmpty(itemStudent.StudentName))
                            {
                                classStudent.tbStudent = studentList.Where(d => d.StudentCode == itemStudent.StudentCode && d.StudentName == itemStudent.StudentName).FirstOrDefault();
                            }

                            if (!string.IsNullOrEmpty(itemStudent.ClassName))
                            {
                                if (vm.IsAddClass && classList.Where(d => d.ClassName == itemStudent.ClassName).Count() == 0)
                                {
                                    classStudent.tbClass = addClassList.Where(d => d.ClassName == itemStudent.ClassName).FirstOrDefault();
                                }
                                else
                                {
                                    classStudent.tbClass = classList.Where(d => d.ClassName == itemStudent.ClassName).FirstOrDefault();
                                }
                            }

                            if (!string.IsNullOrEmpty(itemStudent.ClassGroupName))
                            {
                                classStudent.tbClassGroup = classGroupList.Where(d => d.ClassGroupName == itemStudent.ClassGroupName && d.tbClass.ClassName == itemStudent.ClassName).FirstOrDefault();
                            }

                            #endregion

                            addClassStudent.Add(classStudent);
                        }
                        else
                        {
                            var classStudent = classStudentList.Where(d => d.tbStudent.StudentCode == itemStudent.StudentCode).FirstOrDefault();
                            classStudent.No = itemStudent.No.ConvertToInt();

                            #region 添加外键
                            if (!string.IsNullOrEmpty(itemStudent.ClassName))
                            {
                                if (vm.IsAddClass && classList.Where(d => d.ClassName == itemStudent.ClassName).Count() == 0)
                                {
                                    classStudent.tbClass = addClassList.Where(d => d.ClassName == itemStudent.ClassName).FirstOrDefault();
                                }
                                else
                                {
                                    classStudent.tbClass = classList.Where(d => d.ClassName == itemStudent.ClassName).FirstOrDefault();
                                }
                            }
                            if (!string.IsNullOrEmpty(itemStudent.ClassGroupName))
                            {
                                classStudent.tbClassGroup = classGroupList.Where(d => d.ClassGroupName == itemStudent.ClassGroupName && d.tbClass.ClassName == itemStudent.ClassName).FirstOrDefault();
                            }
                            #endregion
                        }
                    }

                    db.Set<Basis.Entity.tbClassStudent>().AddRange(addClassStudent);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了行政班学生");
                        vm.Status = true;
                    }
                    #endregion
                }
            }
            vm.ImportStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public ActionResult ExportStudent()
        {
            var vm = new Models.Class.ExportStudent();
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tb = db.Table<Basis.Entity.tbClassStudent>()
                    .Include(d => d.tbClass)
                    .Include(d => d.tbStudent)
                    .Include(d => d.tbStudent.tbSysUser.tbSex);
                if (vm.ClassTypeId != 0)
                {
                    tb = tb.Where(d => d.tbClass.tbClassType.Id == vm.ClassTypeId);
                }
                if (vm.GradeId != 0)
                {
                    tb = tb.Where(d => d.tbClass.tbGrade.Id == vm.GradeId);
                }
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbClass.ClassName.Contains(vm.SearchText));
                }
                if (vm.YearId != 0)
                {
                    tb = tb.Where(d => d.tbClass.tbYear.Id == vm.YearId);
                }

                vm.ExportStudentList = (from p in tb
                                        select new Dto.Class.ExportStudent()
                                        {
                                            No = p.No,
                                            StudentCode = p.tbStudent.StudentCode,
                                            StudentName = p.tbStudent.StudentName,
                                            ClassName = p.tbClass.ClassName,
                                            Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                            Id = p.Id
                                        }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学生学号"),
                        new System.Data.DataColumn("学生姓名"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("座位号"),
                        new System.Data.DataColumn("班级名称")
                    });
                foreach (var a in vm.ExportStudentList)
                {
                    var dr = dt.NewRow();
                    dr["学生学号"] = a.StudentCode;
                    dr["学生姓名"] = a.StudentName;
                    dr["性别"] = a.Sex;
                    dr["座位号"] = a.No;
                    dr["班级名称"] = a.ClassName;
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


        public ActionResult ImportClassAndStudent()
        {
            var vm = new Models.Class.ImportClassAndStudent();
            vm.YearList = YearController.SelectList(Code.EnumHelper.YearType.Year);
            return View(vm);
        }

        public ActionResult ImportClassAndStudentTemplate()
        {
            var file = Server.MapPath("~/Areas/Basis/Views/Class/ClassAndStudentTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportClassAndStudent(Models.Class.ImportClassAndStudent vm)
        {
            vm.YearList = YearController.SelectList(Code.EnumHelper.YearType.Year);
            if (ModelState.IsValid)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
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
                    var tbList = new List<string>() { "学生学号", "学生姓名", "排序", "班级名称", "年级", "班级类型", "班主任", "班级教室" };

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
                        var dto = new Dto.Class.ImportClassAndStudent()
                        {
                            StudentCode = dr["学生学号"].ToString(),
                            StudentName = dr["学生姓名"].ToString(),
                            No = dr["排序"].ToString(),
                            ClassName = dr["班级名称"].ToString(),
                            ClassTypeName = dr["班级类型"].ToString(),
                            GradeName = dr["年级"].ToString(),
                            RoomName = dr["班级教室"].ToString(),
                            TeacherName = dr["班主任"].ToString(),
                        };
                        if (vm.ImportClassList.Where(d => d.StudentCode == dto.StudentCode
                                                        && d.StudentName == dto.StudentName
                                                        && d.No == dto.No
                                                        && d.ClassTypeName == dto.ClassTypeName
                                                        && d.GradeName == dto.GradeName
                                                        && d.RoomName == dto.RoomName
                                                        && d.TeacherName == dto.TeacherName
                                                        && d.ClassName == dto.ClassName).Count() == 0)
                        {
                            vm.ImportClassList.Add(dto);
                        }
                    }
                    vm.ImportClassList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.StudentCode) &&
                        string.IsNullOrEmpty(d.StudentName) &&
                        string.IsNullOrEmpty(d.No) &&
                        string.IsNullOrEmpty(d.ClassTypeName) &&
                        string.IsNullOrEmpty(d.GradeName) &&
                        string.IsNullOrEmpty(d.RoomName) &&
                        string.IsNullOrEmpty(d.TeacherName) &&
                        string.IsNullOrEmpty(d.ClassName)
                    );
                    if (vm.ImportClassList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                         .Include(d => d.tbYear)
                                         .Include(d => d.tbGrade)
                                         .Include(d => d.tbClassType)
                                         .Include(d => d.tbRoom)
                                     where p.tbYear.Id == vm.YearId
                                     select p).ToList();
                    var gradeList = db.Table<Basis.Entity.tbGrade>().ToList();
                    var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>()
                        .Include(d => d.tbClass).Include(d => d.tbTeacher).ToList();
                    var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                .Include(d => d.tbClass)
                                                .Include(d => d.tbStudent)
                                            where p.tbClass.tbYear.Id == vm.YearId
                                                && p.tbStudent.IsDeleted == false
                                                && p.tbClass.IsDeleted == false
                                            select p).ToList();
                    var classTypeList = db.Table<Basis.Entity.tbClassType>().ToList();
                    var roomList = db.Table<Basis.Entity.tbRoom>().ToList();
                    var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                       select p).ToList();
                    var classGroupList = db.Table<Basis.Entity.tbClassGroup>().Where(d => d.tbClass.tbYear.Id == vm.YearId).Include(d => d.tbClass).ToList();
                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();
                    var yearList = db.Table<Basis.Entity.tbYear>().ToList();

                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportClassList)
                    {
                        if (vm.ImportClassList.Where(d => d.ClassName == item.ClassName && d.No != item.No).Any())
                        {
                            item.Error += "此班级有多个不同的排序号！";
                        }
                        if (string.IsNullOrEmpty(item.StudentName))
                        {
                            item.Error += "学生姓名不能为空！";
                        }
                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error += "学号不能为空！";
                        }
                        if (studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Count() == 0)
                        {
                            item.Error += "学生不存在!";
                        }
                        if (!vm.IsUpdate && classStudentList.Where(d => d.tbStudent.StudentCode == item.StudentCode && d.tbClass.ClassName != item.ClassName).Count() > 0)
                        {
                            item.Error += "学生已存在其他行政班内！";
                        }
                        if (!string.IsNullOrEmpty(item.No))
                        {
                            int No = 0;
                            if (!int.TryParse(item.No, out No))
                            {
                                item.Error += "排序不是数字；";
                            }
                        }
                        if (string.IsNullOrEmpty(item.ClassName))
                        {
                            item.Error += "班级名称不能为空；";
                        }
                        else
                        {
                            if (classList.Where(d => d.ClassName == item.ClassName).Any())
                            {
                                item.Error += "班级已存在；";
                            }
                        }
                        if (string.IsNullOrEmpty(item.GradeName))
                        {
                            item.Error += "年级不能为空；";
                        }
                        else
                        {
                            if (!gradeList.Where(d => d.GradeName == item.GradeName).Any())
                            {
                                item.Error += "年级不存在；";
                            }
                        }
                        if (string.IsNullOrEmpty(item.ClassTypeName))
                        {
                            item.Error += "班级类型不能为空；";
                        }
                        else
                        {
                            if (!classTypeList.Where(d => d.ClassTypeName == item.ClassTypeName).Any())
                            {
                                item.Error += "班级类型不存在；";
                            }
                        }
                        if (!string.IsNullOrEmpty(item.TeacherName))
                        {
                            if (teacherList.Where(d => d.TeacherName == item.TeacherName).Count() == 0)
                            {
                                item.Error += "班主任不存在；";
                            }
                        }
                        if (!string.IsNullOrEmpty(item.RoomName))
                        {
                            if (roomList.Where(d => d.RoomName == item.RoomName).Any() == false)
                            {
                                item.Error += "班级教室不存在；";
                            }
                        }

                        if (studentList.Where(d => d.StudentCode == item.StudentCode).Count() > 1)
                        {
                            item.Error += "学生学号对应多条学生记录，无法确认需要添加的记录!";
                        }
                    }

                    if (vm.ImportClassList.Where(d => !string.IsNullOrEmpty(d.Error)).Count() > 0)
                    {
                        vm.ImportClassList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }

                    #endregion

                    #region 自动添加班级、学生、行政班小组
                    var tbClassList = new List<Basis.Entity.tbClass>();
                    var tbClassTeacherList = new List<Basis.Entity.tbClassTeacher>();
                    var tbClassStudentList = new List<Basis.Entity.tbClassStudent>();

                    #region 添加/修改班级
                    foreach (var v in vm.ImportClassList)
                    {
                        if (classList.Where(d => d.ClassName == v.ClassName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                var classTemp = classList.Where(d => d.ClassName == v.ClassName).FirstOrDefault();
                                classTemp.No = v.No.ConvertToInt();
                                classTemp.tbClassType = classTypeList.Where(d => d.ClassTypeName == v.ClassTypeName).FirstOrDefault();
                                classTemp.tbGrade = gradeList.Where(d => d.GradeName == v.GradeName).FirstOrDefault();
                                classTemp.tbRoom = roomList.Where(d => d.RoomName == v.RoomName).FirstOrDefault();
                            }
                        }
                        else
                        {
                            if (tbClassList.Where(d => d.ClassName == v.ClassName).Count() == 0)
                            {
                                var classTemp = new Basis.Entity.tbClass()
                                {
                                    No = v.No.ConvertToInt(),
                                    ClassName = v.ClassName,
                                    tbClassType = classTypeList.Where(d => d.ClassTypeName == v.ClassTypeName).FirstOrDefault(),
                                    tbGrade = gradeList.Where(d => d.GradeName == v.GradeName).FirstOrDefault(),
                                    tbRoom = roomList.Where(d => d.RoomName == v.RoomName).FirstOrDefault(),
                                    tbYear = yearList.Where(d => d.Id == vm.YearId).FirstOrDefault()
                                };
                                tbClassList.Add(classTemp);
                            }
                        }
                    }
                    #endregion

                    #region 添加/修改班主任
                    foreach (var v in vm.ImportClassList)
                    {
                        if (!string.IsNullOrEmpty(v.TeacherName))
                        {
                            var classTeacherTemp = classTeacherList.Where(d => d.tbClass.ClassName == v.ClassName).FirstOrDefault();
                            if (classTeacherTemp != null)
                            {
                                classTeacherTemp.tbTeacher = teacherList.Where(d => d.TeacherName == v.TeacherName).FirstOrDefault();
                            }
                            else
                            {
                                if (tbClassTeacherList.Where(d => d.tbTeacher.TeacherName == v.TeacherName && d.tbClass.ClassName == v.ClassName).Count() == 0)
                                {
                                    classTeacherTemp = new Basis.Entity.tbClassTeacher()
                                    {
                                        tbTeacher = teacherList.Where(d => d.TeacherName == v.TeacherName).FirstOrDefault()
                                    };
                                    if (classList.Where(d => d.ClassName == v.ClassName).Any())
                                    {
                                        classTeacherTemp.tbClass = classList.Where(d => d.ClassName == v.ClassName).FirstOrDefault();
                                    }
                                    else
                                    {
                                        classTeacherTemp.tbClass = tbClassList.Where(d => d.ClassName == v.ClassName).FirstOrDefault();
                                    }
                                    tbClassTeacherList.Add(classTeacherTemp);
                                }
                            }
                        }
                    }
                    #endregion

                    #region 删除原有班级学生
                    var classNameList = new List<string>();
                    foreach (var v in vm.ImportClassList)
                    {
                        if (classNameList.Contains(v.ClassName) == false)
                        {
                            classNameList.Add(v.ClassName);
                        }
                    }
                    foreach (var v in classStudentList)
                    {
                        if (classNameList.Contains(v.tbClass.ClassName))
                        {
                            v.IsDeleted = true;
                        }
                    }
                    #endregion

                    #region 添加/修改班级学生
                    foreach (var v in vm.ImportClassList)
                    {
                        var classStudentTemp = new Basis.Entity.tbClassStudent()
                        {
                            tbStudent = studentList.Where(d => d.StudentCode == v.StudentCode).FirstOrDefault()
                        };
                        if (classList.Where(d => d.ClassName == v.ClassName).Any())
                        {
                            classStudentTemp.tbClass = classList.Where(d => d.ClassName == v.ClassName).FirstOrDefault();
                        }
                        else
                        {
                            classStudentTemp.tbClass = tbClassList.Where(d => d.ClassName == v.ClassName).FirstOrDefault();
                        }
                        tbClassStudentList.Add(classStudentTemp);
                    }
                    #endregion

                    db.Set<Basis.Entity.tbClass>().AddRange(tbClassList);
                    db.Set<Basis.Entity.tbClassTeacher>().AddRange(tbClassTeacherList);
                    db.Set<Basis.Entity.tbClassStudent>().AddRange(tbClassStudentList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了行政班+学生");
                        vm.ImportClassList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        vm.Status = true;
                    }

                    #endregion
                }
            }
            return View(vm);
        }


        [NonAction]
        public static List<Basis.Entity.tbClass> BuildList(XkSystem.Models.DbContext db, List<Dto.Class.Edit> editList)
        {
            List<Basis.Entity.tbClass> list = new List<Basis.Entity.tbClass>();

            foreach (var v in editList)
            {
                var classTemp = new Basis.Entity.tbClass()
                {
                    No = 0,
                    ClassName = v.ClassName,
                    tbClassType = db.Table<Basis.Entity.tbClassType>().ToList()[0],
                    tbGrade = db.Table<Basis.Entity.tbGrade>().ToList()[0],
                    tbYear = db.Table<Basis.Entity.tbYear>().Where(d => d.IsDefault == true).FirstOrDefault(),
                };
                list.Add(classTemp);
            }

            return list;
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetClassList(int yearId = 0, int gradeId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (yearId == 0)
                {
                    var year = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.YearType == Code.EnumHelper.YearType.Section
                                orderby p.IsDefault descending
                                select new
                                {
                                    p.tbYearParent.tbYearParent.Id
                                }).FirstOrDefault();
                    if (year != null)
                    {
                        yearId = year.Id;
                    }
                }

                var list = (from p in db.Table<Basis.Entity.tbClass>()
                            where p.tbYear.Id == yearId
                                && (p.tbGrade.Id == gradeId || gradeId == 0)
                            orderby p.tbGrade.No, p.No
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.ClassName,
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }
    }
}