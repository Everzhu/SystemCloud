using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Course.Controllers
{
    public class OrgController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Org.List();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                vm.GradeList = Basis.Controllers.GradeController.SelectList();

                vm.SubjectList = Course.Controllers.SubjectController.SelectList();

                var tb = from p in db.Table<Course.Entity.tbOrg>()
                         where p.tbYear.Id == vm.YearId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.OrgName.Contains(vm.SearchText));
                }

                if (vm.GradeId > 0)
                {
                    tb = tb.Where(d => d.tbGrade.Id == vm.GradeId);
                }

                if (vm.SubjectId > 0)
                {
                    tb = tb.Where(d => d.tbCourse.tbSubject.Id == vm.SubjectId);
                }

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
                                  IsClass = p.IsClass,
                                  IsAutoAttendance = p.IsAutoAttendance ? "是" : "否",
                                  ClassName = (p.tbClass != null && p.tbClass.IsDeleted == false) ? p.tbClass.ClassName : string.Empty,
                                  StudentCount = p.IsClass && p.tbClass != null ?
                                        db.Set<Basis.Entity.tbClassStudent>().Where(d => d.IsDeleted == false && d.tbStudent.IsDeleted == false && d.tbClass.Id == p.tbClass.Id).Count()
                                        : db.Set<Course.Entity.tbOrgStudent>().Where(d => d.IsDeleted == false && d.tbStudent.IsDeleted == false && d.tbOrg.Id == p.Id).Count()
                              }).ToList();

                var orgIds = vm.OrgList.Select(d => d.Id).ToList();

                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                      where p.tbTeacher.IsDeleted == false
                                        && orgIds.Contains(p.tbOrg.Id)
                                      group p by new { p.tbOrg.Id, p.tbTeacher.TeacherName } into result
                                      select new
                                      {
                                          OrgId = result.Key.Id,
                                          TeacherName = result.Key.TeacherName
                                      }).ToList();
                var scheduleList = (from p in db.Table<Entity.tbOrgSchedule>()
                                    where p.tbOrg.IsDeleted == false
                                    && p.tbPeriod.IsDeleted == false
                                    && p.tbWeek.IsDeleted == false
                                    && orgIds.Contains(p.tbOrg.Id)
                                    orderby p.tbWeek.No
                                    select new
                                    {
                                        OrgId = p.tbOrg.Id,
                                        PeriodName = p.tbPeriod.PeriodName,
                                        WeekName = p.tbWeek.WeekName
                                    }).ToList();

                foreach (var a in vm.OrgList)
                {
                    //老师
                    a.TeacherName = string.Join(",", orgTeacherList.Where(d => d.OrgId == a.Id).Select(d => d.TeacherName));

                    //节次
                    var scheduleTemp = scheduleList.Where(d => d.OrgId == a.Id).ToList();
                    a.ScheduleString = string.Join("\r\n", scheduleTemp.Select(d => d.WeekName + " " + d.PeriodName).ToList());
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Org.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                yearId = vm.YearId,
                GradeId = vm.GradeId == null ? -1 : vm.GradeId,
                SubjectId = vm.SubjectId == null ? -1 : vm.SubjectId,
                searchText = vm.SearchText
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrg>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                        .Include(d => d.tbOrg)
                                      where ids.Contains(p.tbOrg.Id)
                                      select p).ToList();

                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                        .Include(d => d.tbOrg)
                                      where ids.Contains(p.tbOrg.Id)
                                      select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    foreach (var student in orgStudentList.Where(d => d.tbOrg.Id == a.Id))
                    {
                        student.IsDeleted = true;
                    }

                    foreach (var teacher in orgTeacherList.Where(d => d.tbOrg.Id == a.Id))
                    {
                        teacher.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了教学班");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Org.Edit();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && (vm.OrgEdit.YearId == 0 || vm.OrgEdit.YearId == null))
                {
                    vm.OrgEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                vm.SubjectList = Course.Controllers.SubjectController.SelectList();
                vm.RoomList = Basis.Controllers.RoomController.SelectList(0, 0, true);
                vm.ClassList = Basis.Controllers.ClassController.SelectList(db.Table<Basis.Entity.tbYear>().Where(d => d.Id == vm.OrgEdit.YearId).Select(d => d.tbYearParent.tbYearParent.Id).FirstOrDefault());
                vm.TeacherList = Teacher.Controllers.TeacherController.SelectList();
                vm.CourseList = Course.Controllers.CourseController.SelectList();

                if (id != 0)
                {
                    var orgTeacherList = db.Table<Course.Entity.tbOrgTeacher>()
                        .Where(d => d.tbOrg.Id == id && d.tbTeacher.IsDeleted == false)
                        .Select(d => d.tbTeacher.Id).Distinct().ToList();

                    var tb = (from p in db.Table<Course.Entity.tbOrg>()
                              where p.Id == id
                              select new Dto.Org.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  CourseId = p.tbCourse.Id,
                                  GradeId = p.tbGrade.Id,
                                  OrgName = p.OrgName,
                                  RoomId = p.tbRoom.Id,
                                  //TeacherId = db.Set<Course.Entity.tbOrgTeacher>().Where(d => d.IsDeleted == false && d.tbOrg.Id == p.Id && d.tbTeacher.IsDeleted == false).Select(d => d.tbTeacher.Id).FirstOrDefault(),
                                  YearId = p.tbYear.Id,
                                  IsClass = p.IsClass,
                                  ClassId = p.tbClass.Id,
                                  IsAutoAttendance = p.IsAutoAttendance
                              }).FirstOrDefault();

                    if (tb != null)
                    {
                        vm.OrgEdit = tb;
                        vm.TeacherIds = string.Join(",", orgTeacherList);
                        vm.TeacherList = Teacher.Controllers.TeacherController.SelectListBySelected(vm.TeacherIds);
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Org.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                //string ids = Request["cbox"];
                string ids = Request["slt"];

                var error = new List<string>();
                if (vm.OrgEdit.IsClass && vm.OrgEdit.ClassId == null)
                {
                    error.Add("请绑定对应的行政班!");
                }

                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Course.Entity.tbOrg>().Where(d => d.OrgName == vm.OrgEdit.OrgName && d.tbYear.Id == vm.OrgEdit.YearId && d.Id != vm.OrgEdit.Id).Any())
                    {
                        error.AddError("该班级已存在!");
                    }
                    else
                    {
                        OrgScheduleController osc = new OrgScheduleController();

                        if (vm.OrgEdit.Id == 0)
                        {
                            var tb = new Course.Entity.tbOrg();
                            tb.No = vm.OrgEdit.No == null ? db.Table<Course.Entity.tbOrg>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.OrgEdit.No;
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.OrgEdit.YearId);
                            tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(vm.OrgEdit.ClassId);
                            tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.OrgEdit.GradeId);
                            tb.tbCourse = db.Set<Course.Entity.tbCourse>().Find(vm.OrgEdit.CourseId);
                            tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.OrgEdit.RoomId);
                            tb.IsClass = vm.OrgEdit.IsClass;
                            tb.OrgName = vm.OrgEdit.OrgName;
                            tb.IsAutoAttendance = vm.OrgEdit.IsAutoAttendance;
                            db.Set<Course.Entity.tbOrg>().Add(tb);
                            osc.EditOrgSchedule(db, tb, ids.Split(','));

                            if (string.IsNullOrEmpty(vm.TeacherIds) == false)
                            {
                                var teacherIds = new List<int>();
                                vm.TeacherIds.Split(',').ToList().ForEach(a => { teacherIds.Add(Convert.ToInt32(a)); });
                                var tbOrgTeacherList = new List<Entity.tbOrgTeacher>();
                                var teacherList = db.Table<Teacher.Entity.tbTeacher>().Where(d => teacherIds.Contains(d.Id)).ToList();
                                teacherList.ForEach(d =>
                                {
                                    tbOrgTeacherList.Add(new Entity.tbOrgTeacher()
                                    {
                                        tbOrg = tb,
                                        tbTeacher = d
                                    });
                                });
                                db.Set<Course.Entity.tbOrgTeacher>().AddRange(tbOrgTeacherList);
                            }
                            else
                            {
                                var errorMsg = new { Status = decimal.Zero, Message = "请至少选择一个教师；" };
                                return Json(errorMsg);
                            }

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了教学班");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Course.Entity.tbOrg>()
                                          .Include(d => d.tbClass)
                                      where p.Id == vm.OrgEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.OrgEdit.No == null ? db.Table<Course.Entity.tbOrg>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.OrgEdit.No;
                                tb.OrgName = vm.OrgEdit.OrgName;
                                tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.OrgEdit.YearId);
                                tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(vm.OrgEdit.ClassId);
                                tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.OrgEdit.GradeId);
                                tb.tbCourse = db.Set<Course.Entity.tbCourse>().Find(vm.OrgEdit.CourseId);
                                tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.OrgEdit.RoomId);
                                tb.IsClass = vm.OrgEdit.IsClass;
                                tb.IsAutoAttendance = vm.OrgEdit.IsAutoAttendance;

                                osc.EditOrgSchedule(db, tb, ids.Split(','));
                                //优化代码：任课教师如果没有，则需要提示
                                if (string.IsNullOrEmpty(vm.TeacherIds) == false)
                                {
                                    //删除旧数据
                                    var oldOrgTeacherList = db.Table<Entity.tbOrgTeacher>()
                                        .Where(d => d.tbOrg.Id == vm.OrgEdit.Id).ToList();
                                    oldOrgTeacherList.ForEach(d => { d.IsDeleted = true; });

                                    var teacherIds = new List<int>();
                                    vm.TeacherIds.Split(',').ToList().ForEach(a => { teacherIds.Add(Convert.ToInt32(a)); });
                                    var tbOrgTeacherList = new List<Entity.tbOrgTeacher>();
                                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().Where(d => teacherIds.Contains(d.Id)).ToList();
                                    teacherList.ForEach(d =>
                                    {
                                        tbOrgTeacherList.Add(new Entity.tbOrgTeacher()
                                        {
                                            tbOrg = tb,
                                            tbTeacher = d
                                        });
                                    });
                                    db.Set<Course.Entity.tbOrgTeacher>().AddRange(tbOrgTeacherList);
                                }
                                else
                                {
                                    var errorMsg = new { Status = decimal.Zero, Message = "请至少选择一个教师；" };
                                    return Json(errorMsg);
                                }

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了教学班信息!");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OpenAttendance(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Course.Entity.tbOrg>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsAutoAttendance = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("开启自动考勤");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CloseAttendance(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Course.Entity.tbOrg>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsAutoAttendance = false;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("关闭自动考勤");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Import()
        {
            var vm = new Models.Org.Import();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.Org.Import vm)
        {
            vm.ImportOrgList = new List<Dto.Org.ImportOrg>();
            vm.ImportOrgStudentList = new List<Dto.Org.ImportOrgStudent>();
            if (ModelState.IsValid)
            {
                if (!vm.IsAdd && !vm.IsUpdate)
                {
                    ModelState.AddModelError("", "请选择“导入选项”!");
                    return View(vm);
                }
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
                    var tbOrgList = new List<string>() { "学生学号", "学生姓名", "座位号", "班序", "教学班名称", "学年", "学期", "学段", "年级", "课程", "班级模式", "绑定班级", "任课老师", "上课教室" };
                    var Text = string.Empty;
                    foreach (var a in tbOrgList)
                    {
                        if (!dt.Columns.Contains(a.ToString()))
                        {
                            Text += a + ",";
                        }
                    }

                    if (!string.IsNullOrEmpty(Text))
                    {
                        ModelState.AddModelError("", "上传的教学班内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }

                    #region 将DataTable转为List
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dtoOrg = new Dto.Org.ImportOrg()
                        {
                            No = Convert.ToString(dr["班序"]),
                            OrgName = Convert.ToString(dr["教学班名称"]),
                            YearName = Convert.ToString(dr["学年"]),
                            GradeName = Convert.ToString(dr["年级"]),
                            CourseName = Convert.ToString(dr["课程"]),
                            IsClass = Convert.ToString(dr["班级模式"]),
                            ClassName = Convert.ToString(dr["绑定班级"]),
                            TeacherName = Convert.ToString(dr["任课老师"]).Split(',').ToList(),
                            RoomName = Convert.ToString(dr["上课教室"])
                        };
                        if (vm.ImportOrgList.Where(d => d.No == dtoOrg.No
                                                    && d.OrgName == dtoOrg.OrgName
                                                    && d.YearName == dtoOrg.YearName
                                                    && d.GradeName == dtoOrg.GradeName
                                                    && d.CourseName == dtoOrg.CourseName
                                                    && d.IsClass == dtoOrg.IsClass
                                                    && d.ClassName == dtoOrg.ClassName
                                                    //&& d.TeacherName == dtoOrg.TeacherName
                                                    && d.RoomName == dtoOrg.RoomName).Count() == 0)
                        {
                            vm.ImportOrgList.Add(dtoOrg);
                        }
                        if (!dtoOrg.IsClass.Contains("行政班模式"))
                        {
                            var dtoOrgStudent = new Dto.Org.ImportOrgStudent()
                            {
                                StudentCode = Convert.ToString(dr["学生学号"]),
                                StudentName = Convert.ToString(dr["学生姓名"]),
                                OrgName = Convert.ToString(dr["教学班名称"]),
                                No = Convert.ToString(dr["座位号"])
                            };
                            if (vm.ImportOrgStudentList.Where(d => d.StudentCode == dtoOrgStudent.StudentCode
                                                                && d.StudentName == dtoOrgStudent.StudentName
                                                                && d.OrgName == dtoOrgStudent.OrgName
                                                                && d.No == dtoOrgStudent.No).Count() == 0)
                            {
                                vm.ImportOrgStudentList.Add(dtoOrgStudent);
                            }
                        }
                    }

                    vm.ImportOrgList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.No) &&
                        string.IsNullOrEmpty(d.OrgName) &&
                        string.IsNullOrEmpty(d.YearName) &&
                        string.IsNullOrEmpty(d.GradeName) &&
                        string.IsNullOrEmpty(d.CourseName) &&
                        string.IsNullOrEmpty(d.IsClass) &&
                        string.IsNullOrEmpty(d.ClassName) &&
                        d.TeacherName.Count == 0 &&
                        string.IsNullOrEmpty(d.RoomName)
                    );
                    vm.ImportOrgStudentList.RemoveAll(d =>
                      string.IsNullOrEmpty(d.StudentCode) &&
                      string.IsNullOrEmpty(d.StudentName) &&
                      string.IsNullOrEmpty(d.OrgName)
                    );
                    #endregion

                    if (vm.ImportOrgList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }

                    var orgList = db.Table<Course.Entity.tbOrg>().ToList();
                    var gradeList = db.Table<Basis.Entity.tbGrade>().ToList();
                    var courseList = db.Table<Course.Entity.tbCourse>().ToList();
                    var roomList = db.Table<Basis.Entity.tbRoom>().ToList();
                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();
                    var yearList = db.Table<Basis.Entity.tbYear>().ToList();
                    var studentList = db.Table<Student.Entity.tbStudent>().Where(d => d.IsDeleted == false).ToList();
                    var classList = db.Table<Basis.Entity.tbClass>().ToList();

                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportOrgList)
                    {
                        if (item.TeacherName.Count > 0)
                        {
                            item.TeacherName.ForEach(d =>
                            {
                                if (teacherList.Where(e => e.TeacherName == d).Any() == false)
                                {
                                    item.Error += "教师不存在！";
                                }
                            });
                        }

                        if (string.IsNullOrEmpty(item.OrgName))
                        {
                            item.Error = item.Error + "教学班名称不能为空!";
                        }

                        if (string.IsNullOrEmpty(item.GradeName))
                        {
                            item.Error = item.Error + "年级不能为空!";
                        }

                        if (string.IsNullOrEmpty(item.CourseName))
                        {
                            item.Error = item.Error + "课程不能为空!";
                        }

                        if (string.IsNullOrEmpty(item.IsClass))
                        {
                            item.Error = item.Error + "班级模式不能为空!";
                        }

                        //if (vm.ImportOrgList.Where(d => d.OrgName == item.OrgName).Count() > 1)
                        //{
                        //    item.Error = item.Error + "该条数据重复!";
                        //}

                        if (vm.IsUpdate == false && orgList.Where(d => d.OrgName == item.OrgName).Count() > 0)
                        {
                            item.Error = item.Error + "系统中已存在该记录!";
                        }
                        int No = 0;
                        if (!string.IsNullOrEmpty(item.No) && !int.TryParse(item.No, out No))
                        {
                            item.Error += "排序号必须为正整数！";
                        }

                        if (courseList.Where(d => d.CourseName == item.CourseName).Count() == decimal.Zero)
                        {
                            item.Error += "课程不存在;";
                        }

                        if (yearList.Where(d => d.YearName == item.YearName).Count() == 0)
                        {
                            item.Error += "学年不存在！";
                        }

                        if (gradeList.Where(d => d.GradeName == item.GradeName).Count() == 0)
                        {
                            item.Error += "年级不存在！";
                        }

                        if (courseList.Where(d => d.CourseName == item.CourseName).Count() == 0)
                        {
                            item.Error += "课程不存在！";
                        }

                        if (!string.IsNullOrEmpty(item.RoomName) && roomList.Where(d => d.RoomName == item.RoomName).Count() == 0)
                        {
                            item.Error += "教室不存在！";
                        }

                        if (item.IsClass.Contains("行政班模式") && classList.Where(d => d.ClassName == item.ClassName).Count() == 0)
                        {
                            item.Error += "行政班不存在！";
                        }

                    }

                    foreach (var v in vm.ImportOrgStudentList)
                    {
                        if (orgList.Where(d => d.OrgName == v.OrgName).Count() == 0 && vm.ImportOrgList.Where(d => d.OrgName == v.OrgName).Count() == 0)
                        {
                            v.Error += "学生指定的教学班不存在;";
                        }
                        if (string.IsNullOrEmpty(v.StudentCode) || string.IsNullOrEmpty(v.StudentName))
                        {
                            v.Error += "学生姓名和学号不能为空;";
                        }
                        if (studentList.Where(d => d.StudentCode == v.StudentCode).Count() == 0)
                        {
                            v.Error += "指定学号的学生不存在;";
                        }

                        if (studentList.Where(d => d.StudentCode == v.StudentCode).FirstOrDefault().StudentName != v.StudentName)
                        {
                            v.Error += "学生学号和姓名不匹配;";
                        }

                        int no = 0;
                        if (!string.IsNullOrEmpty(v.No) && !int.TryParse(v.No, out no))
                        {
                            v.Error += "座位号必须为正整数！";
                        }
                    }

                    if (vm.ImportOrgList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0
                        || vm.ImportOrgStudentList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportOrgList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        vm.ImportOrgStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    foreach (var item in vm.ImportOrgList)
                    {
                        Course.Entity.tbOrg tb = null;
                        if (orgList.Where(d => d.OrgName == item.OrgName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                if (orgList.Where(d => d.OrgName == item.OrgName).Count() > 1)
                                {
                                    item.Error += "系统中该教学班数据存在重复，无法确认需要更新的记录!";
                                    vm.ImportOrgList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                    vm.ImportOrgStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                    return View(vm);
                                }
                                tb = orgList.Where(d => d.OrgName == item.OrgName).FirstOrDefault();
                                tb.No = string.IsNullOrEmpty(item.No) ? 0 : Convert.ToInt32(item.No);
                                tb.tbYear = yearList.Where(d => d.YearName == item.YearName).FirstOrDefault();
                                tb.tbGrade = gradeList.Where(d => d.GradeName == item.GradeName).FirstOrDefault();
                                tb.tbCourse = courseList.Where(d => d.CourseName == item.CourseName).FirstOrDefault();
                                tb.tbRoom = roomList.Where(c => c.RoomName == item.RoomName).FirstOrDefault();
                                tb.OrgName = item.OrgName;
                                tb.IsClass = item.IsClass.Contains("行政班模式") ? true : false;
                                tb.tbClass = item.IsClass.Contains("行政班模式") ? classList.Where(d => d.ClassName == item.ClassName).FirstOrDefault() : null;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (vm.IsAdd)
                            {
                                tb = new Course.Entity.tbOrg();
                                tb.No = string.IsNullOrEmpty(item.No) ? 0 : Convert.ToInt32(item.No);
                                tb.tbYear = yearList.Where(d => d.YearName == item.YearName).FirstOrDefault();
                                tb.tbGrade = gradeList.Where(d => d.GradeName == item.GradeName).FirstOrDefault();
                                tb.tbCourse = courseList.Where(d => d.CourseName == item.CourseName).FirstOrDefault();
                                tb.tbRoom = roomList.Where(c => c.RoomName == item.RoomName).FirstOrDefault();
                                tb.OrgName = item.OrgName;
                                tb.IsClass = item.IsClass.Contains("行政班模式") ? true : false;
                                tb.tbClass = item.IsClass.Contains("行政班模式") ? classList.Where(d => d.ClassName == item.ClassName).FirstOrDefault() : null;
                                db.Set<Course.Entity.tbOrg>().Add(tb);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        //添加任课老师
                        if (item.TeacherName.Count > 0)
                        {
                            item.TeacherName.ForEach(d =>
                            {
                                var tbOrgTeacher = new Entity.tbOrgTeacher()
                                {
                                    tbOrg = tb,
                                    tbTeacher = teacherList.Where(e => e.TeacherName == d).FirstOrDefault()
                                };
                                db.Set<Course.Entity.tbOrgTeacher>().Add(tbOrgTeacher);
                            });
                        }

                        if (item.IsClass.Contains("行政班模式"))
                        {
                            #region 行政班模式，把行政班的学生添加到走读班
                            var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                        .Include(d => d.tbStudent)
                                                    where p.tbClass.ClassName == item.ClassName
                                                    select p).ToList();

                            if (classStudentList.Count > 0)
                            {
                                foreach (var v in classStudentList)
                                {
                                    var orgStudent = new Course.Entity.tbOrgStudent()
                                    {
                                        tbStudent = v.tbStudent,
                                        tbOrg = tb
                                    };
                                    db.Set<Course.Entity.tbOrgStudent>().Add(orgStudent);
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region 走读班模式，判断是否一块上传学生信息
                            if (vm.ImportOrgStudentList.Count > 0 && vm.ImportOrgStudentList.Where(d => d.OrgName == tb.OrgName).Count() > 0)
                            {
                                foreach (var itemStudent in vm.ImportOrgStudentList.Where(d => d.OrgName == tb.OrgName))
                                {
                                    var student = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == itemStudent.StudentCode).FirstOrDefault();
                                    if (student == null)
                                    {
                                        item.Error += "学生不存在;";
                                    }
                                    else
                                    {
                                        var orgStudent = new Course.Entity.tbOrgStudent()
                                        {
                                            tbStudent = student,
                                            tbOrg = tb
                                        };
                                        db.Set<Course.Entity.tbOrgStudent>().Add(orgStudent);
                                    }
                                }
                                if (vm.ImportOrgStudentList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0
                                    || vm.ImportOrgList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                                {
                                    vm.ImportOrgList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                    vm.ImportOrgStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                    return View(vm);
                                }
                            }
                            #endregion 
                        }
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量添加了教学班");
                        vm.Status = true;
                    }
                }
            }
            vm.ImportOrgList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            vm.ImportOrgStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public ActionResult Export()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var vm = new Models.Org.List();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                vm.SubjectList = Course.Controllers.SubjectController.SelectList();

                vm.OrgList = (from p in db.Table<Course.Entity.tbOrg>()
                              where p.tbYear.Id == vm.YearId
                                && (p.tbGrade.Id == vm.GradeId || vm.GradeId == 0)
                                && (p.tbCourse.tbSubject.Id == vm.SubjectId || vm.SubjectId == 0)
                              orderby p.tbCourse.CourseName, p.No
                              select new Dto.Org.List
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  CourseName = p.tbCourse.CourseName,
                                  GradeName = p.tbGrade.GradeName,
                                  OrgName = p.OrgName,
                                  RoomName = p.tbRoom.RoomName,
                                  YearName = p.tbYear.YearName,
                                  ClassName = p.tbClass.ClassName,
                              }).ToList();

                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                      where p.tbTeacher.IsDeleted == false
                                        && p.tbOrg.tbYear.Id == vm.YearId
                                        && (p.tbOrg.tbGrade.Id == vm.GradeId || vm.GradeId == 0)
                                        && (p.tbOrg.tbCourse.tbSubject.Id == vm.SubjectId || vm.SubjectId == 0)
                                      select new
                                      {
                                          OrgId = p.tbOrg.Id,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();
                foreach (var a in vm.OrgList)
                {
                    a.TeacherName = string.Join(",", orgTeacherList.Where(d => d.OrgId == a.Id).Select(d => d.TeacherName));
                }

                var scheduleList = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                                    where p.tbWeek.IsDeleted == false
                                        && p.tbPeriod.IsDeleted == false
                                        && p.tbOrg.tbYear.Id == vm.YearId
                                        && (p.tbOrg.tbGrade.Id == vm.GradeId || vm.GradeId == 0)
                                        && (p.tbOrg.tbCourse.tbSubject.Id == vm.SubjectId || vm.SubjectId == 0)
                                    select new
                                    {
                                        OrgId = p.tbOrg.Id,
                                        WeekName = p.tbWeek.WeekName,
                                        PeriodName = p.tbPeriod.PeriodName
                                    });

                foreach (var a in vm.OrgList)
                {
                    a.Schedule = string.Join(",", scheduleList.Where(d => d.OrgId == a.Id).Select(d => d.WeekName + d.PeriodName));
                }

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("班序"),
                        new System.Data.DataColumn("班级名称"),
                        new System.Data.DataColumn("课程名称"),
                        new System.Data.DataColumn("年级名称"),
                        new System.Data.DataColumn("教室"),
                        new System.Data.DataColumn("任课老师"),
                        new System.Data.DataColumn("课表节次")
                    });
                foreach (var a in vm.OrgList)
                {
                    var dr = dt.NewRow();
                    dr["班序"] = a.No;
                    dr["班级名称"] = a.OrgName;
                    dr["课程名称"] = a.CourseName;
                    dr["年级名称"] = a.GradeName;
                    dr["教室"] = a.RoomName;
                    dr["任课老师"] = a.TeacherName;
                    dr["课表节次"] = a.Schedule;
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

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Course/Views/Org/OrgTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult ImportOrg()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Org.ImportOrg();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                return View(vm);
            }
        }

        public ActionResult ImportOrgTemplate()
        {
            var file = Server.MapPath("~/Areas/Course/Views/Org/OrgTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportOrg(Models.Org.ImportOrg vm)
        {
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
            if (vm.YearList.Count > 0 && vm.YearId == 0)
            {
                vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
            }

            if (ModelState.IsValid)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 上传数据

                    vm.ImportOrgList = new List<Dto.Org.ImportOrg>();

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
                    var tbOrgList = new List<string>() { "班序", "教学班名称", "学年学段", "年级", "课程", "班级模式", "绑定班级", "任课老师", "上课教室", "课表节次", "自动考勤" };
                    var Text = string.Empty;
                    foreach (var a in tbOrgList)
                    {
                        if (!dt.Columns.Contains(a.ToString()))
                        {
                            Text += a + ",";
                        }
                    }

                    if (!string.IsNullOrEmpty(Text))
                    {
                        ModelState.AddModelError("", "上传的教学班内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }

                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dtoOrg = new Dto.Org.ImportOrg()
                        {
                            No = dr["班序"].ConvertToString(),
                            OrgName = dr["教学班名称"].ConvertToString(),
                            YearName = dr["学年学段"].ConvertToString(),
                            GradeName = dr["年级"].ConvertToString(),
                            CourseName = dr["课程"].ConvertToString(),
                            IsClass = dr["班级模式"].ConvertToString(),
                            ClassName = dr["绑定班级"].ConvertToString(),
                            TeacherName = dr["任课老师"].ConvertToString().Split(',').ToList(),
                            OrgSchedule = dr["课表节次"].ConvertToString(),
                            RoomName = dr["上课教室"].ConvertToString(),
                            IsAutoAttendance = dr["自动考勤"].ConvertToString()
                        };
                        if (vm.ImportOrgList.Where(d => d.No == dtoOrg.No
                                                    && d.OrgName == dtoOrg.OrgName
                                                    && d.YearName == dtoOrg.YearName
                                                    && d.GradeName == dtoOrg.GradeName
                                                    && d.CourseName == dtoOrg.CourseName
                                                    && d.IsClass == dtoOrg.IsClass
                                                    && d.ClassName == dtoOrg.ClassName
                                                    && d.OrgSchedule == dtoOrg.OrgSchedule
                                                    && d.IsAutoAttendance == dtoOrg.IsAutoAttendance
                                                    && d.RoomName == dtoOrg.RoomName).Any() == false)
                        {
                            vm.ImportOrgList.Add(dtoOrg);
                        }
                    }

                    vm.ImportOrgList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.No) &&
                        string.IsNullOrEmpty(d.OrgName) &&
                        string.IsNullOrEmpty(d.YearName) &&
                        string.IsNullOrEmpty(d.GradeName) &&
                        string.IsNullOrEmpty(d.CourseName) &&
                        string.IsNullOrEmpty(d.IsClass) &&
                        string.IsNullOrEmpty(d.ClassName) &&
                        d.TeacherName.Count > 0 &&
                        string.IsNullOrEmpty(d.OrgSchedule) &&
                        string.IsNullOrEmpty(d.IsAutoAttendance) &&
                        string.IsNullOrEmpty(d.RoomName)
                    );

                    if (vm.ImportOrgList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }

                    #endregion

                    #region 验证数据格式是否正确

                    var orgList = db.Table<Course.Entity.tbOrg>().Where(d => d.tbYear.Id == vm.YearId).ToList();
                    var gradeList = db.Table<Basis.Entity.tbGrade>().ToList();
                    var courseList = db.Table<Course.Entity.tbCourse>().ToList();
                    var roomList = db.Table<Basis.Entity.tbRoom>().ToList();
                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();
                    var yearList = db.Table<Basis.Entity.tbYear>().ToList();
                    ////var studentList = db.Table<Student.Entity.tbStudent>().Where(d => d.IsDeleted == false).ToList();
                    var classList = db.Table<Basis.Entity.tbClass>().ToList();
                    //var subjectList = db.Table<Course.Entity.tbSubject>().ToList();
                    //var courseTypeList = db.Table<Course.Entity.tbCourseType>().ToList();
                    var orgTeacherList = db.Table<Course.Entity.tbOrgTeacher>().Where(d => d.tbOrg.IsDeleted == false).Include(d => d.tbTeacher).Include(d => d.tbOrg).ToList();
                    var weekList = db.Table<Basis.Entity.tbWeek>().ToList();
                    var periodList = db.Table<Basis.Entity.tbPeriod>().ToList();

                    foreach (var item in vm.ImportOrgList)
                    {
                        int no = 0;
                        if (!string.IsNullOrEmpty(item.No) && !int.TryParse(item.No, out no))
                        {
                            item.Error += "排序号必须为正整数；";

                        }
                        //自动考勤不填写，默认为否
                        if (string.IsNullOrEmpty(item.IsAutoAttendance))
                        {
                            item.IsAutoAttendance = "否";
                        }
                        else
                        {
                            if (!(new string[] { "是", "否" }.Contains(item.IsAutoAttendance)))
                            {
                                item.Error += "自动考勤只包括是或否；";
                            }
                        }

                        if (string.IsNullOrEmpty(item.OrgName))
                        {
                            item.Error = item.Error + "教学班名称不能为空!";
                        }
                        else
                        {
                            if (!vm.IsUpdate && orgList.Where(d => d.OrgName == item.OrgName).Count() > 0)
                            {
                                item.Error += "教学班名称已存在；";
                            }
                        }

                        if (string.IsNullOrEmpty(item.YearName))
                        {
                            item.Error += "学年不能为空！";
                        }
                        else
                        {
                            if (yearList.Where(d => d.YearName == item.YearName).Count() == 0)
                            {
                                item.Error += "学年不存在！";
                            }
                        }

                        if (string.IsNullOrEmpty(item.GradeName))
                        {
                            item.Error += "年级不能为空！";
                        }
                        else
                        {
                            if (gradeList.Where(d => d.GradeName == item.GradeName).Count() == 0)
                            {
                                item.Error += "年级不存在！";
                            }
                        }

                        if (string.IsNullOrEmpty(item.CourseName))
                        {
                            item.Error += "课程不能为空！";
                        }
                        else
                        {
                            if (courseList.Where(d => d.CourseName == item.CourseName).Any() == false)
                            {
                                item.Error += "课程不存在！";
                            }
                        }

                        if (string.IsNullOrEmpty(item.IsClass))
                        {
                            item.Error = item.Error + "班级模式不能为空!";
                        }

                        if (!string.IsNullOrEmpty(item.RoomName) && roomList.Where(d => d.RoomName == item.RoomName).Count() == 0)
                        {
                            item.Error += "教室不存在！";
                        }

                        if (item.IsClass.Contains("行政班模式") && classList.Where(d => d.ClassName == item.ClassName).Count() == 0)
                        {
                            item.Error += "行政班不存在！";
                        }
                        if (item.TeacherName.Count > 0)
                        {
                            item.TeacherName.ForEach(d =>
                            {
                                if (teacherList.Where(e => e.TeacherName == d).Any() == false)
                                {
                                    item.Error += "教师不存在！";
                                }
                            });
                        }

                        //判断星期节次是否存在
                        if (string.IsNullOrEmpty(item.OrgSchedule) == false)
                        {
                            var weekPeriodList = (from p in weekList
                                                  from q in periodList
                                                  select p.WeekName + q.PeriodName).ToList();
                            var temp = item.OrgSchedule.Split(',');
                            foreach (var t in temp)
                            {
                                if (t.Contains("["))
                                {
                                    if (!weekPeriodList.Contains(t.Split('[')[0]))
                                    {
                                        item.Error += "星期节次不存在！";
                                    }
                                }
                                else
                                {
                                    if (!weekPeriodList.Contains(t))
                                    {
                                        item.Error += "星期节次不存在！";
                                    }
                                }
                            }
                        }
                    }

                    if (vm.ImportOrgList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportOrgList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }

                    #endregion

                    var addOrgList = new List<Course.Entity.tbOrg>();
                    var addOrgTeacherList = new List<Course.Entity.tbOrgTeacher>();

                    #region 添加教学班
                    foreach (var item in vm.ImportOrgList)
                    {
                        Course.Entity.tbOrg tb = null;

                        #region 添加教学班
                        if (orgList.Where(d => d.OrgName == item.OrgName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                if (orgList.Where(d => d.OrgName == item.OrgName).Count() > 1)
                                {
                                    item.Error += "系统中该教学班数据存在重复，无法确认需要更新的记录!";
                                    vm.ImportOrgList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                    return View(vm);
                                }
                                tb = orgList.Where(d => d.OrgName == item.OrgName).FirstOrDefault();
                                tb.No = string.IsNullOrEmpty(item.No) ? 0 : Convert.ToInt32(item.No);
                                tb.OrgName = item.OrgName;
                                tb.IsAutoAttendance = item.IsAutoAttendance == "是" ? true : false;
                                tb.IsClass = item.IsClass.Contains("行政班模式") ? true : false;
                                tb.tbClass = item.IsClass.Contains("行政班模式") ? classList.Where(d => d.ClassName == item.ClassName).FirstOrDefault() : null;

                                #region 添加外键
                                if (!string.IsNullOrEmpty(item.CourseName))
                                {
                                    tb.tbCourse = courseList.Where(d => d.CourseName == item.CourseName).FirstOrDefault();
                                }
                                if (!string.IsNullOrEmpty(item.RoomName))
                                {
                                    tb.tbRoom = roomList.Where(c => c.RoomName == item.RoomName).FirstOrDefault();
                                }
                                if (!string.IsNullOrEmpty(item.GradeName))
                                {
                                    tb.tbGrade = gradeList.Where(d => d.GradeName == item.GradeName).FirstOrDefault();
                                }
                                if (!string.IsNullOrEmpty(item.YearName))
                                {
                                    tb.tbYear = yearList.Where(d => d.YearName == item.YearName).FirstOrDefault();
                                }
                                #endregion
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            tb = new Course.Entity.tbOrg();
                            tb.No = string.IsNullOrEmpty(item.No) ? 0 : Convert.ToInt32(item.No);
                            tb.tbCourse = courseList.Where(d => d.CourseName == item.CourseName).FirstOrDefault();
                            tb.OrgName = item.OrgName;
                            tb.IsAutoAttendance = item.IsAutoAttendance == "是" ? true : false;
                            tb.IsClass = item.IsClass.Contains("行政班模式") ? true : false;
                            tb.tbClass = item.IsClass.Contains("行政班模式") ? classList.Where(d => d.ClassName == item.ClassName).FirstOrDefault() : null;

                            #region 添加外键
                            if (!string.IsNullOrEmpty(item.CourseName))
                            {
                                tb.tbCourse = courseList.Where(d => d.CourseName == item.CourseName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.RoomName))
                            {
                                tb.tbRoom = roomList.Where(c => c.RoomName == item.RoomName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.GradeName))
                            {
                                tb.tbGrade = gradeList.Where(d => d.GradeName == item.GradeName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.YearName))
                            {
                                tb.tbYear = yearList.Where(d => d.YearName == item.YearName).FirstOrDefault();
                            }
                            #endregion

                            addOrgList.Add(tb);
                        }
                        #endregion

                        #region 添加任课老师
                        if (item.TeacherName.Count > 0)
                        {
                            item.TeacherName.ForEach(d =>
                            {
                                if (orgTeacherList.Where(e => e.tbOrg.OrgName == item.OrgName && e.tbTeacher.TeacherName == d).Any() == false)
                                {
                                    var orgTeacher = new Course.Entity.tbOrgTeacher()
                                    {
                                        tbOrg = tb,
                                        tbTeacher = teacherList.Where(f => f.TeacherName == d).FirstOrDefault()
                                    };
                                    addOrgTeacherList.Add(orgTeacher);
                                }
                            });
                        }
                        #endregion

                        #region 添加课表节次
                        if (!string.IsNullOrEmpty(item.OrgSchedule))
                        {
                            new OrgScheduleController().EditOrgSchedule(db, tb, item.OrgSchedule);
                        }
                        #endregion
                    }
                    #endregion

                    db.Set<Course.Entity.tbOrg>().AddRange(addOrgList);
                    db.Set<Course.Entity.tbOrgTeacher>().AddRange(addOrgTeacherList);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量添加了教学班");
                        vm.Status = true;
                    }
                }
            }
            vm.ImportOrgList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public ActionResult ExportOrg()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var vm = new Models.Org.List();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                vm.SubjectList = Course.Controllers.SubjectController.SelectList();
                vm.OrgList = (from p in db.Table<Course.Entity.tbOrg>()
                              where p.tbYear.Id == vm.YearId
                                && (p.tbGrade.Id == vm.GradeId || vm.GradeId == 0)
                                && (p.tbCourse.tbSubject.Id == vm.SubjectId || vm.SubjectId == 0)
                              orderby p.tbCourse.CourseName, p.No
                              select new Dto.Org.List
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  CourseName = p.tbCourse.CourseName,
                                  GradeName = p.tbGrade.GradeName,
                                  OrgName = p.OrgName,
                                  RoomName = p.tbRoom.RoomName,
                                  YearName = p.tbYear.YearName,
                                  ClassName = p.tbClass.ClassName,
                                  StudentCount = p.IsClass && p.tbClass != null ?
                                        db.Set<Basis.Entity.tbClassStudent>().Where(d => d.IsDeleted == false && d.tbStudent.IsDeleted == false && d.tbClass.Id == p.tbClass.Id).Count()
                                        : db.Set<Course.Entity.tbOrgStudent>().Where(d => d.IsDeleted == false && d.tbStudent.IsDeleted == false && d.tbOrg.Id == p.Id).Count()
                              }).ToList();

                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                      where p.tbTeacher.IsDeleted == false
                                        && p.tbOrg.tbYear.Id == vm.YearId
                                        && (p.tbOrg.tbGrade.Id == vm.GradeId || vm.GradeId == 0)
                                        && (p.tbOrg.tbCourse.tbSubject.Id == vm.SubjectId || vm.SubjectId == 0)
                                      select new
                                      {
                                          OrgId = p.tbOrg.Id,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();

                if (orgTeacherList.Count > 0)
                {
                    foreach (var a in vm.OrgList)
                    {
                        a.TeacherName = string.Join(",", orgTeacherList.Where(d => d.OrgId == a.Id).Select(d => d.TeacherName));
                    }
                }

                var scheduleList = (from p in db.Table<Course.Entity.tbOrgSchedule>()
                                    where p.tbWeek.IsDeleted == false
                                        && p.tbPeriod.IsDeleted == false
                                        && p.tbOrg.tbYear.Id == vm.YearId
                                        && (p.tbOrg.tbGrade.Id == vm.GradeId || vm.GradeId == 0)
                                        && (p.tbOrg.tbCourse.tbSubject.Id == vm.SubjectId || vm.SubjectId == 0)
                                    select new
                                    {
                                        OrgId = p.tbOrg.Id,
                                        WeekName = p.tbWeek.WeekName,
                                        PeriodName = p.tbPeriod.PeriodName
                                    }).ToList();

                if (scheduleList.Count > 0)
                {
                    foreach (var a in vm.OrgList)
                    {
                        a.Schedule = string.Join(",", scheduleList.Where(d => d.OrgId == a.Id).Select(d => d.WeekName + d.PeriodName));
                    }
                }

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("班序"),
                        new System.Data.DataColumn("班级名称"),
                        new System.Data.DataColumn("课程名称"),
                        new System.Data.DataColumn("年级名称"),
                        new System.Data.DataColumn("教室"),
                        new System.Data.DataColumn("任课老师"),
                        new System.Data.DataColumn("课表节次"),
                        new System.Data.DataColumn("学生人数"),
                    });
                foreach (var a in vm.OrgList)
                {
                    var dr = dt.NewRow();
                    dr["班序"] = a.No;
                    dr["班级名称"] = a.OrgName;
                    dr["课程名称"] = a.CourseName;
                    dr["年级名称"] = a.GradeName;
                    dr["教室"] = a.RoomName;
                    dr["任课老师"] = a.TeacherName;
                    dr["课表节次"] = a.Schedule;
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

        public ActionResult ImportOrgStudent()
        {
            var vm = new Models.Org.ImportOrgStudent();
            return View(vm);
        }

        public ActionResult ImportOrgStudentTemplate()
        {
            var file = Server.MapPath("~/Areas/Course/Views/Org/OrgStudentTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportOrgStudent(Models.Org.ImportOrgStudent vm)
        {
            vm.ImportOrgStudentList = new List<Dto.Org.ImportOrgStudent>();
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
                    var tbOrgList = new List<string>() { "学生学号", "学生姓名", "座位号", "教学班名称" };
                    var Text = string.Empty;
                    foreach (var a in tbOrgList)
                    {
                        if (!dt.Columns.Contains(a.ToString()))
                        {
                            Text += a + ",";
                        }
                    }

                    if (!string.IsNullOrEmpty(Text))
                    {
                        ModelState.AddModelError("", "上传的教学班内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }

                    #region 将DataTable转为List
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dtoOrgStudent = new Dto.Org.ImportOrgStudent()
                        {
                            StudentCode = Convert.ToString(dr["学生学号"]),
                            StudentName = Convert.ToString(dr["学生姓名"]),
                            OrgName = Convert.ToString(dr["教学班名称"]),
                            No = Convert.ToString(dr["座位号"])
                        };
                        if (vm.ImportOrgStudentList.Where(d => d.StudentCode == dtoOrgStudent.StudentCode
                                                            && d.StudentName == dtoOrgStudent.StudentName
                                                            && d.OrgName == dtoOrgStudent.OrgName
                                                            && d.No == dtoOrgStudent.No).Count() == 0)
                        {
                            vm.ImportOrgStudentList.Add(dtoOrgStudent);
                        }
                    }
                    vm.ImportOrgStudentList.RemoveAll(d =>
                      string.IsNullOrEmpty(d.StudentCode) &&
                      string.IsNullOrEmpty(d.StudentName) &&
                      string.IsNullOrEmpty(d.OrgName)
                    );
                    #endregion

                    if (vm.ImportOrgStudentList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }

                    var orgList = db.Table<Course.Entity.tbOrg>().ToList();
                    //var gradeList = db.Table<Basis.Entity.tbGrade>().ToList();
                    //var courseList = db.Table<Course.Entity.tbCourse>().ToList();
                    //var roomList = db.Table<Basis.Entity.tbRoom>().ToList();
                    //var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();
                    //var yearList = db.Table<Basis.Entity.tbYear>().ToList();
                    var studentList = db.Table<Student.Entity.tbStudent>().Where(d => d.IsDeleted == false).ToList();
                    //var classList = db.Table<Basis.Entity.tbClass>().ToList();
                    var orgStudentList = db.Table<Course.Entity.tbOrgStudent>().ToList();

                    #region 验证数据格式是否正确
                    foreach (var v in vm.ImportOrgStudentList)
                    {
                        if (string.IsNullOrEmpty(v.OrgName))
                        {
                            v.Error += "教学班名称不能为空；";
                        }
                        else if (orgList.Where(d => d.OrgName == v.OrgName).Count() == 0)
                        {
                            v.Error += "学生指定的教学班不存在;";
                        }

                        if (orgList.Where(d => d.OrgName == v.OrgName).Count() > 0
                            && orgList.Where(d => d.OrgName == v.OrgName).FirstOrDefault().IsClass)
                        {
                            v.Error += "行政班不能添加学生；";
                        }

                        if (string.IsNullOrEmpty(v.StudentCode) || string.IsNullOrEmpty(v.StudentName))
                        {
                            v.Error += "学生姓名和学号不能为空;";
                        }
                        else if (studentList.Where(d => d.StudentCode == v.StudentCode).Count() == 0)
                        {
                            v.Error += "指定学号的学生不存在;";
                        }

                        if (studentList.Where(d => d.StudentCode == v.StudentCode).Count() > 0
                            && studentList.Where(d => d.StudentCode == v.StudentCode && d.StudentName == v.StudentName).Count() == 0)
                        {
                            v.Error += "学号已经存在且和学生姓名不匹配！";
                        }

                        int no = 0;
                        if (!string.IsNullOrEmpty(v.No) && !int.TryParse(v.No, out no))
                        {
                            v.Error += "座位号必须为正整数！";
                        }
                    }

                    if (vm.ImportOrgStudentList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportOrgStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 走读班模式，判断上传学生信息
                    var addOrgStudentList = new List<Course.Entity.tbOrgStudent>();
                    foreach (var itemStudent in vm.ImportOrgStudentList)
                    {
                        if (orgStudentList.Where(d => d.tbOrg.OrgName == itemStudent.OrgName
                                && d.tbStudent.StudentCode == itemStudent.StudentCode
                                && d.tbStudent.StudentName == itemStudent.StudentName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                var orgStudent = orgStudentList.Where(d => d.tbOrg.OrgName == itemStudent.OrgName
                                && d.tbStudent.StudentCode == itemStudent.StudentCode
                                && d.tbStudent.StudentName == itemStudent.StudentName).FirstOrDefault();
                                orgStudent.No = itemStudent.No.ConvertToInt();

                                #region 添加外键
                                if (!string.IsNullOrEmpty(itemStudent.StudentCode) && !string.IsNullOrEmpty(itemStudent.StudentName))
                                {
                                    orgStudent.tbStudent = studentList.Where(d => d.StudentCode == itemStudent.StudentCode && d.StudentName == itemStudent.StudentName).FirstOrDefault();
                                }
                                if (!string.IsNullOrEmpty(itemStudent.OrgName))
                                {
                                    orgStudent.tbOrg = orgList.Where(d => d.OrgName == itemStudent.OrgName).FirstOrDefault();
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            var orgStudent = new Course.Entity.tbOrgStudent()
                            {
                                No = itemStudent.No.ConvertToInt()
                            };

                            #region 添加外键
                            if (!string.IsNullOrEmpty(itemStudent.StudentCode) && !string.IsNullOrEmpty(itemStudent.StudentName))
                            {
                                orgStudent.tbStudent = studentList.Where(d => d.StudentCode == itemStudent.StudentCode && d.StudentName == itemStudent.StudentName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(itemStudent.OrgName))
                            {
                                orgStudent.tbOrg = orgList.Where(d => d.OrgName == itemStudent.OrgName).FirstOrDefault();
                            }
                            #endregion

                            addOrgStudentList.Add(orgStudent);
                        }
                    }
                    //if (vm.ImportOrgStudentList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    //{
                    //    vm.ImportOrgStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                    //    return View(vm);
                    //}
                    #endregion

                    db.Set<Course.Entity.tbOrgStudent>().AddRange(addOrgStudentList);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量添加了教学班");
                        vm.Status = true;
                    }

                }
            }
            vm.ImportOrgStudentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public ActionResult ExportOrgStudent(int yearId, int gradeId, int subjectId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var orgList = (from p in db.Table<Course.Entity.tbOrg>()
                               .Include(d => d.tbClass)
                               where p.tbYear.Id == yearId
                                 && (p.tbGrade.Id == gradeId || gradeId == 0)
                                 && (p.tbCourse.tbSubject.Id == subjectId || subjectId == 0)
                               orderby p.tbCourse.CourseName, p.No
                               select p).ToList();

                var orgId = new List<int>();
                foreach (var v in orgList)
                {
                    orgId.Add(v.Id);
                }

                var orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                      where orgId.Contains(p.tbOrg.Id)
                                      select new Dto.Org.ExportOrgStudent()
                                      {
                                          OrgName = p.tbOrg.OrgName,
                                          No = p.No,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName,
                                          Sex = p.tbStudent.tbSysUser.tbSex.SexName
                                      }).ToList();

                foreach (var v in orgList.Where(d => d.IsClass))
                {
                    var classStudent = db.Table<Basis.Entity.tbClassStudent>()
                        .Include(d => d.tbStudent)
                        .Include(d => d.tbStudent.tbSysUser.tbSex)
                        .Where(d => d.tbClass.Id == v.tbClass.Id).ToList();
                    foreach (var s in classStudent)
                    {
                        var orgStudent = new Dto.Org.ExportOrgStudent()
                        {
                            OrgName = v.OrgName,
                            No = s.No,
                            StudentCode = s.tbStudent.StudentCode,
                            StudentName = s.tbStudent.StudentName,
                            Sex = s.tbStudent.tbSysUser.tbSex.SexName
                        };
                        if (!orgStudentList.Contains(orgStudent))
                        {
                            orgStudentList.Add(orgStudent);
                        }
                    }
                }

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学生学号"),
                        new System.Data.DataColumn("学生姓名"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("座位号"),
                        new System.Data.DataColumn("教学班名称")
                    });
                foreach (var a in orgStudentList)
                {
                    var dr = dt.NewRow();
                    dr["学生学号"] = a.StudentCode;
                    dr["学生姓名"] = a.StudentName;
                    dr["性别"] = a.Sex;
                    dr["座位号"] = a.No;
                    dr["教学班名称"] = a.OrgName;
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

        public ActionResult ExportOrgAndStudent()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Org.ExportOrgAndStudent();
                var tb = db.Table<Course.Entity.tbOrg>();

                #region 条件查询
                if (vm.GradeId > 0)
                {
                    tb = tb.Where(d => d.tbGrade.Id == vm.GradeId);
                }
                if (vm.SubjectId > 0)
                {
                    tb = tb.Where(d => d.tbCourse.tbSubject.Id == vm.SubjectId);
                }
                if (vm.YearId > 0)
                {
                    tb = tb.Where(d => d.tbYear.Id == vm.YearId);
                }
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(d => d.OrgName.Contains(vm.SearchText));
                }
                #endregion

                var orgIds = tb.Select(d => d.Id).ToList();
                var orgTeacherList = db.Table<Course.Entity.tbOrgTeacher>()
                    .Include(d => d.tbTeacher)
                    .Include(d => d.tbOrg)
                    .Where(d => orgIds.Contains(d.tbOrg.Id)).ToList();
                var orgScheduleList = db.Table<Course.Entity.tbOrgSchedule>()
                    .Include(d => d.tbOrg)
                    .Include(d => d.tbPeriod)
                    .Include(d => d.tbWeek)
                    .Where(d => orgIds.Contains(d.tbOrg.Id)).ToList();
                Dictionary<int, string> dic = new Dictionary<int, string>();
                foreach (var v in orgIds)
                {
                    var scheduleList = orgScheduleList.Where(d => d.tbOrg.Id == v)
                        .OrderBy(d => d.tbPeriod.No).OrderBy(d => d.tbWeek.No).ToList();
                    StringBuilder sb = new StringBuilder();
                    //星期一1[单周],星期一2,星期二3,星期二4[双周]
                    foreach (var vv in scheduleList)
                    {
                        sb.Append(vv.tbWeek.WeekName + vv.tbPeriod.PeriodName);
                        switch (vv.ScheduleType)
                        {
                            case Code.EnumHelper.CourseScheduleType.All:
                                break;
                            case Code.EnumHelper.CourseScheduleType.Dual:
                                sb.Append("[双周]");
                                break;
                            case Code.EnumHelper.CourseScheduleType.Odd:
                                sb.Append("[单周]");
                                break;
                            default: break;
                        }
                        sb.Append(",");
                    }
                    if (sb.Length > 0)
                    {
                        sb = sb.Remove(sb.Length - 1, 1);
                    }
                    dic.Add(v, sb.ToString());
                }

                vm.DataList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                               where orgIds.Contains(p.tbOrg.Id)
                               orderby p.No
                               select new Dto.Org.ExportOrgAndStudent()
                               {
                                   CourseName = p.tbOrg.tbCourse.CourseName,
                                   GradeName = p.tbOrg.tbGrade.GradeName,
                                   OrgId = p.tbOrg.Id,
                                   OrgName = p.tbOrg.OrgName,
                                   RoomName = p.tbOrg.tbRoom.RoomName,
                                   StudentCode = p.tbStudent.StudentCode,
                                   StudentName = p.tbStudent.StudentName,
                                   SexName = p.tbStudent.tbSysUser.tbSex.SexName
                               }).ToList();

                var isClassOrgList = tb.Include(d => d.tbClass)
                    .Include(d => d.tbCourse)
                    .Include(d => d.tbGrade)
                    .Include(d => d.tbRoom)
                    .Where(d => d.IsClass).ToList();
                var classIds = isClassOrgList.Select(d => d.tbClass.Id).ToList();
                var classStudentList = db.Table<Basis.Entity.tbClassStudent>()
                    .Include(d => d.tbClass)
                    .Include(d => d.tbStudent)
                    .Include(d => d.tbStudent.tbSysUser.tbSex)
                    .Where(d => classIds.Contains(d.tbClass.Id)).ToList();

                var tempList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                where classIds.Contains(p.tbClass.Id)
                                select new Dto.Org.ExportOrgAndStudent()
                                {
                                    ClassId = p.tbClass.Id,
                                    ClassName = p.tbClass.ClassName,
                                    StudentCode = p.tbStudent.StudentCode,
                                    StudentName = p.tbStudent.StudentName,
                                    SexName = p.tbStudent.tbSysUser.tbSex.SexName
                                }).ToList();
                foreach (var v in tempList)
                {
                    var org = isClassOrgList.Where(d => d.tbClass.Id == v.ClassId).FirstOrDefault();
                    if (org != null)
                    {
                        v.CourseName = org.tbCourse != null ? org.tbCourse.CourseName : "";
                        v.GradeName = org.tbGrade != null ? org.tbGrade.GradeName : "";
                        v.OrgId = org.Id;
                        v.OrgName = org.OrgName;
                        v.RoomName = org.tbRoom != null ? org.tbRoom.RoomName : "";
                        vm.DataList.Add(v);
                    }
                }

                foreach (var v in vm.DataList)
                {
                    var orgTeacher = orgTeacherList.Where(d => d.tbOrg.Id == v.OrgId).ToList();
                    if (orgTeacher != null)
                    {
                        v.TeacherCode = string.Join(",", orgTeacher.Select(d => d.tbTeacher.TeacherCode));
                        v.TeacherName = string.Join(",", orgTeacher.Select(d => d.tbTeacher.TeacherName));
                    }
                    v.OrgSchedule = dic.Where(d => d.Key == v.OrgId).FirstOrDefault().Value;
                }



                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学生学号"),
                        new System.Data.DataColumn("学生姓名"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("教学班名称"),
                        new System.Data.DataColumn("年级"),
                        new System.Data.DataColumn("班级名称"),
                        new System.Data.DataColumn("课程"),
                        new System.Data.DataColumn("教室"),
                        new System.Data.DataColumn("教师工号"),
                        new System.Data.DataColumn("教师姓名"),
                        new System.Data.DataColumn("课表节次")
                    });
                foreach (var a in vm.DataList)
                {
                    var dr = dt.NewRow();
                    dr["学生学号"] = a.StudentCode;
                    dr["学生姓名"] = a.StudentName;
                    dr["性别"] = a.SexName;
                    dr["教学班名称"] = a.OrgName;
                    dr["年级"] = a.GradeName;
                    dr["班级名称"] = a.ClassName;
                    dr["课程"] = a.CourseName;
                    dr["教室"] = a.RoomName;
                    dr["教师工号"] = a.TeacherCode;
                    dr["教师姓名"] = a.TeacherName;
                    dr["课表节次"] = a.OrgSchedule;
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

        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrg>()
                          orderby p.No, p.OrgName
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.OrgName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        public static List<System.Web.Mvc.SelectListItem> SelectList(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var list = (from p in db.Table<Course.Entity.tbOrg>()
                            orderby p.No, p.OrgName
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.OrgName,
                                Value = p.Id.ToString()
                            }).ToList();

                if (id > 0)
                {
                    list.Where(d => d.Value == id.ConvertToString()).FirstOrDefault().Selected = true;
                }

                return list;
            }
        }

        public static List<System.Web.Mvc.SelectListItem> SelectOrgList(int YearId = 0, string SearchText = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (YearId == 0)
                {
                    var year = (from p in db.Table<Basis.Entity.tbYear>()
                                orderby p.IsDefault descending
                                select new
                                {
                                    p.Id
                                }).FirstOrDefault();
                    if (year != null)
                    {
                        YearId = year.Id;
                    }
                }

                var tb = (from p in db.Table<Course.Entity.tbOrg>()
                          where p.tbYear.Id == YearId
                            && (p.OrgName.Contains(SearchText) || String.IsNullOrEmpty(SearchText))
                          orderby p.No, p.OrgName
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.OrgName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        public static List<System.Web.Mvc.SelectListItem> SelectOrgList(int SubjectId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var courseIds = (from p in db.Table<Course.Entity.tbCourse>()
                                 where (p.tbSubject.Id == SubjectId || SubjectId == 0)
                                 && p.tbSubject.IsDeleted == false
                                 orderby p.No, p.CourseName
                                 select p.Id
                                 ).ToList();

                var tb = (from p in db.Table<Course.Entity.tbOrg>()
                          where courseIds.Contains(p.tbCourse.Id)
                          && p.tbCourse.IsDeleted == false
                          orderby p.No, p.OrgName
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.OrgName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static Dto.Org.Info SelectInfo(int orgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrg>()
                          where p.Id == orgId
                          select new Dto.Org.Info
                          {
                              Id = p.Id,
                              OrgName = p.OrgName
                          }).FirstOrDefault();
                return tb;
            }
        }
    }
}