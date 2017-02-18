using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamScheduleController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamSchedule.List();
                vm.ExamList = Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                var tb = from p in db.Table<Exam.Entity.tbExamSchedule>()
                         where p.tbExam.IsDeleted == false
                         && (vm.ExamId == 0 || p.tbExam.Id == vm.ExamId)
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ExamScheduleName.Contains(vm.SearchText));
                }

                vm.ExamScheduleList = (from p in tb
                                       orderby p.ScheduleDate, p.ScheduleNo
                                       select new Dto.ExamSchedule.List
                                       {
                                           Id = p.Id,
                                           ExamScheduleName = p.ExamScheduleName,
                                           ScheduleNo = p.ScheduleNo,
                                           ScheduleDate = p.ScheduleDate,
                                           FromDate = p.FromDate,
                                           ToDate = p.ToDate,
                                           ExamName = p.tbExam.ExamName,
                                       }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamSchedule.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, examId = vm.ExamId }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyExamRoomList(Models.ExamSchedule.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("MyExamRoomList", new { searchText = vm.SearchText, examId = vm.ExamId }));
        }

        public ActionResult MyExamRoomList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamSchedule.List();
                vm.ExamList = Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                var tb = from p in db.Table<Exam.Entity.tbExamSchedule>()
                         where p.tbExam.IsDeleted == false
                         && (vm.ExamId == 0 || p.tbExam.Id == vm.ExamId)
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ExamScheduleName.Contains(vm.SearchText));
                }

                //场次考试日期
                var lst = new List<string>();
                foreach (var o in tb.Select(d=>d.ScheduleDate).Distinct().OrderBy(d => d))
                {
                    var scheduleDate = o.ToString(Code.Common.StringToDate);
                    lst.Add(scheduleDate);
                }
                vm.columnList = lst;
                //场次课程
                var examScheduleCouseList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                             where p.tbExam.Id == vm.ExamId && p.tbCourse.IsDeleted == false
                                             && p.tbExamSchedule.IsDeleted == false
                                             select new
                                             {
                                                 ScheduleId = p.tbExamSchedule.Id,
                                                 p.tbExamSchedule.ScheduleDate,
                                                 p.tbExamSchedule.ScheduleNo,
                                                 FromDate = p.tbExamSchedule.FromDate,
                                                 ToDate = p.tbExamSchedule.ToDate,
                                                 ExamCourseId = p.Id,
                                                 p.tbCourse.CourseName
                                             }).Distinct().ToList();
                //课程考场
                var examRoomList = (from p in db.Table<Exam.Entity.tbExamRoom>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbRoom.IsDeleted == false
                                    select new
                                    {
                                        ExamCourseId = p.tbExamCourse.Id,
                                        ExamRoomId = p.Id,
                                        p.tbRoom.RoomName,
                                        p.ExamRoomName
                                    }).ToList();

                var lstExamRoom = new List<int>();
                if (Code.Common.UserType == Code.EnumHelper.SysUserType.Student)
                {
                    //考场学生
                    lstExamRoom = (from p in db.Table<Exam.Entity.tbExamStudent>()
                                   where p.tbStudent.IsDeleted == false
                                   && p.tbExamRoom.IsDeleted == false
                                   && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                   select p.tbExamRoom.Id).Distinct().ToList();
                }
                else if (Code.Common.UserType == Code.EnumHelper.SysUserType.Teacher || Code.Common.UserType == Code.EnumHelper.SysUserType.Administrator)
                {
                    //监考教师
                    lstExamRoom = (from p in db.Table<Exam.Entity.tbExamTeacher>()
                                   where p.tbExamRoom.IsDeleted == false
                                   && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                   select p.tbExamRoom.Id).Distinct().ToList();
                }

                vm.ScheduleRoomList = (from p in examScheduleCouseList
                                       join t in examRoomList on p.ExamCourseId equals t.ExamCourseId
                                       where lstExamRoom.Contains(t.ExamRoomId)
                                       select new Dto.ExamSchedule.ScheduleRoomList
                                       {
                                           ScheduleId = p.ScheduleId,
                                           ScheduleDate = p.ScheduleDate,
                                           ScheduleNo = p.ScheduleNo,
                                           ExamCourseId = p.ExamCourseId,
                                           ExamRoomId = t.ExamRoomId,
                                           FromDate = p.FromDate,
                                           ToDate = p.ToDate,
                                           CourseName = p.CourseName,
                                           ExamRoomName = t.ExamRoomName,
                                           RoomName = t.RoomName
                                       }).ToList();

                vm.ExamRoomCourseList = (from p in vm.ScheduleRoomList
                                         select new Dto.ExamSchedule.ScheduleRoomList
                                         {
                                             ExamCourseId = p.ExamCourseId,
                                             ExamRoomId = p.ExamRoomId,
                                             ScheduleNo = p.ScheduleNo
                                         }).Distinct().ToList();

                vm.ExamScheduleList = (from p in tb
                                       orderby p.ScheduleNo
                                       select new Dto.ExamSchedule.List
                                       {
                                           ScheduleNo = p.ScheduleNo,
                                       }).Distinct().ToList();
                return View(vm);
            }
        }


        public ActionResult ScheduleCourseList(int examId, int scheduleId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamSchedule.List();

                var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                          where p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExam.Id == examId
                           && p.tbExamSchedule.Id == scheduleId
                          select p);

                vm.ExamScheduleList = (from p in tb
                                       select new Dto.ExamSchedule.List
                                       {
                                           Id = p.Id,
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           CourseName = p.tbCourse.CourseName
                                       }).ToList();
                return View(vm);
            }
        }

        public ActionResult SearchCourse(int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.SearchCourse();
                vm.SubjectList = Course.Controllers.SubjectController.SelectList();
                var tb = from p in db.Table<Exam.Entity.tbExamCourse>()
                         where p.tbExam.Id == examId
                             && p.tbCourse.IsDeleted == false
                             && p.tbCourse.tbSubject.IsDeleted == false
                             && (p.tbCourse.tbSubject.Id == vm.SubjectId || vm.SubjectId == 0)
                         select p;
                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbCourse.CourseName.Contains(vm.SearchText));
                }
                vm.SubjectCourseList = (from p in tb
                                        orderby p.tbCourse.CourseName
                                        select new Areas.Course.Dto.Course.List
                                        {
                                            Id = p.Id,
                                            CourseName = p.tbCourse.CourseName,
                                            SubjectName = p.tbCourse.tbSubject.SubjectName
                                        }).Distinct().ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchCourse(Models.ExamCourse.SearchCourse vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SearchCourse", new
            {
                examId = vm.ExamId,
                SubjectId = vm.SubjectId,
                searchText = vm.SearchText
            }));
        }

        [HttpPost]
        public ActionResult UpdateExamCourse(List<int> ids, int scheduleId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                foreach (var exmCourseId in ids)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                              where p.Id == exmCourseId
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.tbExamSchedule = db.Set<Exam.Entity.tbExamSchedule>().Find(scheduleId);
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改场次考试课程");
                }

                return Code.MvcHelper.Post(null, string.Empty, "操作成功");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteExamCourse(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                foreach (var examCourseId in ids)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamCourse>().Include(d => d.tbExamSchedule)
                              where p.Id == examCourseId
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.tbExamSchedule = null;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除场次考试课程");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamSchedule>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除场次");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamSchedule.Edit();
                vm.ExamList = Exam.Controllers.ExamController.SelectList();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamSchedule>()
                              where p.Id == id
                              select new Dto.ExamSchedule.Edit
                              {
                                  Id = p.Id,
                                  ExamScheduleName = p.ExamScheduleName,
                                  ExamId = p.tbExam.Id,
                                  ScheduleNo = p.ScheduleNo,
                                  ScheduleDate = p.ScheduleDate,
                                  FromDate = p.FromDate,
                                  ToDate = p.ToDate
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamScheduleEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamSchedule.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamScheduleEdit.Id == 0)
                    {
                        var tb = new Exam.Entity.tbExamSchedule();
                        tb.tbExam = db.Set<Exam.Entity.tbExam>().Find(vm.ExamScheduleEdit.ExamId);
                        tb.ExamScheduleName = vm.ExamScheduleEdit.ExamScheduleName;
                        tb.ScheduleNo = Convert.ToInt32(vm.ExamScheduleEdit.ScheduleNo);
                        tb.ScheduleDate = vm.ExamScheduleEdit.ScheduleDate;
                        tb.FromDate = vm.ExamScheduleEdit.FromDate;
                        tb.ToDate = vm.ExamScheduleEdit.ToDate;
                        db.Set<Exam.Entity.tbExamSchedule>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加场次设置");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExamSchedule>()
                                  where p.Id == vm.ExamScheduleEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.tbExam = db.Set<Exam.Entity.tbExam>().Find(vm.ExamScheduleEdit.ExamId);
                            tb.ExamScheduleName = vm.ExamScheduleEdit.ExamScheduleName;
                            tb.ScheduleNo = Convert.ToInt32(vm.ExamScheduleEdit.ScheduleNo);
                            tb.ScheduleDate = vm.ExamScheduleEdit.ScheduleDate;
                            tb.FromDate = vm.ExamScheduleEdit.FromDate;
                            tb.ToDate = vm.ExamScheduleEdit.ToDate;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改场次设置");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        #region 导入
        public ActionResult Import()
        {
            var vm = new Models.ExamSchedule.Import();
            vm.ExamList = Exam.Controllers.ExamController.SelectList();
            return View(vm);
        }
        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Exam/Views/ExamSchedule/ExamScheduleImport.xls");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.ExamSchedule.Import vm)
        {
            vm.ExamList = Exam.Controllers.ExamController.SelectList();
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
                    var tbList = new List<string>() { "学生学号", "学生姓名", "场次名称", "考试课程", "考场名称", "考试教室", "监考教师" };

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
                        ModelState.AddModelError("", "上传的EXCEL内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }

                    #region 将DataTable转为List
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dto = new Dto.ExamSchedule.Import()
                        {
                            StudentCode = dr["学生学号"].ToString(),
                            StudentName = dr["学生姓名"].ToString(),
                            ScheduleName = dr["场次名称"].ToString(),
                            ExamCourseName = dr["考试课程"].ToString(),
                            ExamRoomName = dr["考场名称"].ToString(),
                            RoomName = dr["考试教室"].ToString(),
                            TeacherName = dr["监考教师"].ToString()
                        };
                        if (vm.ImportList.Where(d => d.StudentCode == dto.StudentCode
                                                        && d.StudentName == dto.StudentName
                                                        && d.ScheduleName == dto.ScheduleName
                                                        && d.ExamCourseName == dto.ExamCourseName
                                                        && d.ExamRoomName == dto.ExamRoomName
                                                        && d.RoomName == dto.RoomName
                                                        && d.TeacherName == dto.TeacherName).Count() == 0)
                        {
                            vm.ImportList.Add(dto);
                        }
                    }
                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.StudentCode) &&
                        string.IsNullOrEmpty(d.StudentName) &&
                        string.IsNullOrEmpty(d.ScheduleName) &&
                        string.IsNullOrEmpty(d.ExamCourseName) &&
                        string.IsNullOrEmpty(d.ExamRoomName) &&
                        string.IsNullOrEmpty(d.RoomName) &&
                        string.IsNullOrEmpty(d.TeacherName)
                    );
                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    //场次
                    var scheduleList = (from p in db.Table<Exam.Entity.tbExamSchedule>()
                                       .Include(d => d.tbExam)
                                        where p.tbExam.Id == vm.ExamId
                                        select p).ToList();
                    //考试课程
                    var examCourseList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                        .Include(d => d.tbExam)
                                        .Include(d => d.tbExamSchedule)
                                        .Include(d => d.tbCourse)
                                          where p.tbExam.Id == vm.ExamId
                                                && p.tbCourse.IsDeleted == false
                                          select p).ToList();
                    //考试地点
                    var examRoomList = (from p in db.Table<Exam.Entity.tbExamRoom>()
                                         .Include(d => d.tbExamCourse)
                                         .Include(d => d.tbExamCourse.tbExamSchedule)
                                         .Include(d => d.tbExamCourse.tbCourse)
                                         .Include(d => d.tbRoom)
                                        where p.tbExamCourse.IsDeleted == false
                              && p.tbExamCourse.tbCourse.IsDeleted == false
                              && p.tbRoom.IsDeleted == false
                              && p.tbExamCourse.tbExamSchedule.IsDeleted == false
                              && p.tbExamCourse.tbExamSchedule.tbExam.Id == vm.ExamId
                                        select p).ToList();

                    //考场学生
                    var examStudentList = (from p in db.Table<Exam.Entity.tbExamStudent>()
                                            .Include(d => d.tbExamRoom)
                                            .Include(d => d.tbExamRoom.tbExamCourse)
                                            .Include(d => d.tbExamRoom.tbExamCourse.tbExamSchedule)
                                            .Include(d => d.tbStudent)
                                           where p.tbStudent.IsDeleted == false
                                      && p.tbExamRoom.IsDeleted == false
                                      && p.tbExamRoom.tbExamCourse.IsDeleted == false
                                      && p.tbExamRoom.tbExamCourse.tbExam.Id == vm.ExamId
                                           select p).ToList();
                    //监考教师
                    var examTeacherList = (from p in db.Table<Exam.Entity.tbExamTeacher>()
                                            .Include(d => d.tbExamRoom)
                                            .Include(d => d.tbExamRoom.tbExamCourse)
                                            .Include(d => d.tbExamRoom.tbExamCourse.tbExamSchedule)
                                            .Include(d => d.tbTeacher)
                                           where p.tbExamRoom.IsDeleted == false
                                          && p.tbTeacher.IsDeleted == false
                                          && p.tbExamRoom.tbExamCourse.IsDeleted == false
                                          && p.tbExamRoom.tbExamCourse.tbExam.Id == vm.ExamId
                                           select p).ToList();

                    var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                       select p).ToList();
                    var roomList = db.Table<Basis.Entity.tbRoom>().ToList();
                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();
                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportList)
                    {
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
                        if (string.IsNullOrEmpty(item.ScheduleName))
                        {
                            item.Error += "场次名称不能为空；";
                        }
                        if (string.IsNullOrEmpty(item.ExamCourseName))
                        {
                            item.Error += "考试课程不能为空；";
                        }
                        else
                        {
                            if (examCourseList.Where(d => d.tbCourse.CourseName == item.ExamCourseName).Any() == false)
                            {
                                item.Error += "考试课程不存在；";
                            }
                        }
                        if (string.IsNullOrEmpty(item.ExamRoomName))
                        {
                            item.Error += "考场名称不能为空；";
                        }
                        if (!string.IsNullOrEmpty(item.TeacherName))
                        {
                            var teacherNames = item.TeacherName.Replace("，", ",").Split(',');
                            for (int i = 0; i < teacherNames.Count(); i++)
                            {
                                var teacher = (from p in teacherList
                                               where p.TeacherName == teacherNames[i]
                                               select p).FirstOrDefault();
                                if (teacher == null)
                                {
                                    item.Error += "监考教师名称不存在;";
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(item.RoomName))
                        {
                            item.Error += "考试教室不能为空；";
                        }
                        else
                        {
                            if (roomList.Where(d => d.RoomName == item.RoomName).Any() == false)
                            {
                                item.Error += "考试教室不存在；";
                            }
                        }
                       var tt = vm.ImportList.Where(d => d.StudentCode == item.StudentCode && d.ScheduleName == item.ScheduleName && d.ExamCourseName == item.ExamCourseName
                                                    && d.ExamRoomName == item.ExamRoomName).Count();
                        if (tt > decimal.One)
                        {
                            var strTemp = string.Format("该学生[{0}]重复出现同场次同课程同考场{1}次；", item.StudentName, vm.ImportList.Where(d => d.StudentCode == item.StudentCode && d.ScheduleName == item.ScheduleName && d.ExamCourseName == item.ExamCourseName
                                                     && d.ExamRoomName == item.ExamRoomName).Count());
                            item.Error += strTemp;
                        }
                    }

                    if (vm.ImportList.Where(d => !string.IsNullOrEmpty(d.Error)).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }

                    #endregion

                    #region 添加/修改场次
                    var tbExamScheduleList = new List<Exam.Entity.tbExamSchedule>();
                    foreach (var v in vm.ImportList)
                    {
                        if (scheduleList.Where(d => d.ExamScheduleName == v.ScheduleName).Count() == 0)
                        {
                            var Temp = new Exam.Entity.tbExamSchedule()
                            {
                                ExamScheduleName = v.ScheduleName,
                                tbExam = db.Set<Exam.Entity.tbExam>().Find(vm.ExamId),
                                ScheduleNo = 1,
                                ScheduleDate = DateTime.Now,
                                FromDate = DateTime.Now,
                                ToDate = DateTime.Now
                            };
                            tbExamScheduleList.Add(Temp);
                        }
                    }
                    #endregion

                    #region 添加/修改考试课程
                    var tbExamCourseList = new List<Exam.Entity.tbExamCourse>();
                    foreach (var v in vm.ImportList)
                    {
                        var examCourseTemp = examCourseList.Where(d => d.tbCourse.CourseName == v.ExamCourseName && d.tbExamSchedule != null && d.tbExamSchedule.ExamScheduleName == v.ScheduleName).FirstOrDefault();
                        if (examCourseTemp != null)
                        {
                            examCourseTemp.tbExamSchedule =scheduleList.Where(d => d.ExamScheduleName == v.ScheduleName).FirstOrDefault();
                        }
                        else
                        {
                            var Temp = examCourseList.Where(d => d.tbCourse.CourseName == v.ExamCourseName).FirstOrDefault();
                            if (Temp != null)
                            {
                                if (scheduleList.Where(d => d.ExamScheduleName == v.ScheduleName).Any())
                                {
                                    Temp.tbExamSchedule = scheduleList.Where(d => d.ExamScheduleName == v.ScheduleName).FirstOrDefault();
                                }
                                else
                                {
                                    Temp.tbExamSchedule = tbExamScheduleList.Where(d => d.ExamScheduleName == v.ScheduleName).FirstOrDefault();
                                }
                            }
                        }
                    }
                    #endregion

                    #region 添加/修改考场
                    var tbExamRoomList = new List<Exam.Entity.tbExamRoom>();
                    foreach (var v in vm.ImportList)
                    {
                        var examRoomTemp = examRoomList.Where(d => d.ExamRoomName == v.ExamRoomName && d.tbExamCourse.tbCourse.CourseName == v.ExamCourseName
                                                              && d.tbExamCourse.tbExamSchedule!=null && d.tbExamCourse.tbExamSchedule.ExamScheduleName==v.ScheduleName
                                                               ).FirstOrDefault();
                        if (examRoomTemp != null)
                        {
                            examRoomTemp.tbExamCourse =examCourseList.Where(d => d.tbCourse.CourseName == v.ExamCourseName).FirstOrDefault();
                            examRoomTemp.tbRoom = roomList.Where(d => d.RoomName == v.RoomName).FirstOrDefault();
                        }
                        else
                        {

                            examRoomTemp = new Exam.Entity.tbExamRoom()
                            {
                                ExamRoomName = v.ExamRoomName,
                                tbRoom = roomList.Where(d => d.RoomName == v.RoomName).FirstOrDefault(),
                                RowSeat = 5
                            };
                            if (examCourseList.Where(d => d.tbCourse.CourseName == v.ExamCourseName && d.tbExamSchedule!=null && d.tbExamSchedule.ExamScheduleName==v.ScheduleName).Any())
                            {
                                examRoomTemp.tbExamCourse = examCourseList.Where(d => d.tbCourse.CourseName == v.ExamCourseName).FirstOrDefault();
                            }
                            tbExamRoomList.Add(examRoomTemp);

                            examRoomList.Add(examRoomTemp);
                        }
                    }
                    #endregion

                    #region 添加/修改考场学生
                    var tbExamStudentList = new List<Exam.Entity.tbExamStudent>();
                    foreach (var v in vm.ImportList)
                    {
                        var classStudentTemp = examStudentList.Where(d => d.tbExamRoom.ExamRoomName == v.ExamRoomName && d.tbExamRoom.tbExamCourse.tbCourse.CourseName==v.ExamCourseName
                                                                    && d.tbExamRoom.tbExamCourse.tbExamSchedule!=null && d.tbExamRoom.tbExamCourse.tbExamSchedule.ExamScheduleName==v.ScheduleName
                                                                    && d.tbStudent.StudentCode ==v.StudentCode).FirstOrDefault();
                        if (classStudentTemp != null)
                        {
                            classStudentTemp.tbStudent = studentList.Where(d => d.StudentCode == v.StudentCode).FirstOrDefault();
                        }
                        else
                        {
                            classStudentTemp = new Exam.Entity.tbExamStudent()
                            {
                                tbStudent = studentList.Where(d => d.StudentCode == v.StudentCode).FirstOrDefault()
                            };
                            if (examRoomList.Where(d => d.ExamRoomName == v.ExamRoomName && d.tbExamCourse.tbCourse.CourseName==v.ExamCourseName
                                                   && d.tbExamCourse.tbExamSchedule!=null && d.tbExamCourse.tbExamSchedule.ExamScheduleName==v.ScheduleName).Any())
                            {
                                classStudentTemp.tbExamRoom = examRoomList.Where(d => d.ExamRoomName == v.ExamRoomName && d.tbExamCourse.tbExamSchedule!=null
                                                                                 && d.tbExamCourse.tbCourse.CourseName==v.ExamCourseName
                                                                                 && d.tbExamCourse.tbExamSchedule.ExamScheduleName==v.ScheduleName).FirstOrDefault();
                            }
                            else
                            {
                                classStudentTemp.tbExamRoom = tbExamRoomList.Where(d => d.ExamRoomName == v.ExamRoomName && d.tbExamCourse.tbExamSchedule != null
                                                                                 && d.tbExamCourse.tbCourse.CourseName == v.ExamCourseName
                                                                                 && d.tbExamCourse.tbExamSchedule.ExamScheduleName == v.ScheduleName).FirstOrDefault();
                            }
                            tbExamStudentList.Add(classStudentTemp);
                        }
                    }
                    #endregion

                    #region 添加/修改监考教师
                    var tbExamTeacherList = new List<Exam.Entity.tbExamTeacher>();
                    foreach (var v in vm.ImportList)
                    {
                        if (!string.IsNullOrEmpty(v.TeacherName))
                        {
                            var teacherNames = v.TeacherName.Replace("，", ",").Split(',');
                            for (int i = 0; i < teacherNames.Count(); i++)
                            {
                                var teacher = (from p in teacherList
                                               where p.TeacherName == teacherNames[i]
                                               select p).FirstOrDefault();
                                var teacherTemp=examTeacherList.Where(d => d.tbExamRoom.ExamRoomName == v.ExamRoomName && d.tbExamRoom.tbExamCourse.tbExamSchedule != null
                                                                                 && d.tbExamRoom.tbExamCourse.tbCourse.CourseName == v.ExamCourseName
                                                                                 && d.tbExamRoom.tbExamCourse.tbExamSchedule.ExamScheduleName == v.ScheduleName
                                                                                 && d.tbTeacher.TeacherName== teacherNames[i]).FirstOrDefault();
                                if (teacherTemp != null)
                                {
                                    teacherTemp.tbTeacher = teacher;
                                }
                                else
                                {
                                    teacherTemp = new Exam.Entity.tbExamTeacher()
                                    {
                                        tbTeacher = teacher,
                                        IsPrimary = false
                                    };
                                    if (examRoomList.Where(d => d.ExamRoomName == v.ExamRoomName && d.tbExamCourse.tbExamSchedule != null
                                                                                 && d.tbExamCourse.tbCourse.CourseName == v.ExamCourseName
                                                                                 && d.tbExamCourse.tbExamSchedule.ExamScheduleName == v.ScheduleName).Any())
                                    {
                                        teacherTemp.tbExamRoom = examRoomList.Where(d => d.ExamRoomName == v.ExamRoomName && d.tbExamCourse.tbExamSchedule != null
                                                                                 && d.tbExamCourse.tbCourse.CourseName == v.ExamCourseName
                                                                                 && d.tbExamCourse.tbExamSchedule.ExamScheduleName == v.ScheduleName).FirstOrDefault();
                                    }
                                    else
                                    {
                                        teacherTemp.tbExamRoom = tbExamRoomList.Where(d => d.ExamRoomName == v.ExamRoomName && d.tbExamCourse.tbExamSchedule != null
                                                                                 && d.tbExamCourse.tbCourse.CourseName == v.ExamCourseName
                                                                                 && d.tbExamCourse.tbExamSchedule.ExamScheduleName == v.ScheduleName).FirstOrDefault();
                                    }

                                    tbExamTeacherList.Add(teacherTemp);

                                    examTeacherList.Add(teacherTemp);
                                }
                            }
                        }
                    }
                    #endregion

                    db.Set<Exam.Entity.tbExamSchedule>().AddRange(tbExamScheduleList);
                    db.Set<Exam.Entity.tbExamRoom>().AddRange(tbExamRoomList);
                    db.Set<Exam.Entity.tbExamStudent>().AddRange(tbExamStudentList);
                    db.Set<Exam.Entity.tbExamTeacher>().AddRange(tbExamTeacherList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了考场");
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        vm.Status = true;
                    }
                    else
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        vm.Status = false;
                    }
                }
            }
            return View(vm);
        }
        #endregion
    }
}