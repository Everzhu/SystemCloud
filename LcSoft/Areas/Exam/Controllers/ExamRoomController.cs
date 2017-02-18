using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamRoomController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamRoom.List();

                var tb = from p in db.Table<Exam.Entity.tbExamRoom>()
                         where p.tbExamCourse.IsDeleted==false
                         && p.tbExamCourse.tbCourse.IsDeleted==false
                         && p.tbRoom.IsDeleted==false
                         && p.tbExamCourse.Id==vm.ExamCourseId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ExamRoomName.Contains(vm.SearchText));
                }

                vm.ExamRoomList = (from p in tb
                               orderby p.No descending
                               select new Dto.ExamRoom.List
                               {
                                   Id = p.Id,
                                   ExamCourseName=p.tbExamCourse.tbCourse.CourseName,
                                   ExamRoomName=p.ExamRoomName,
                                   RoomName=p.tbRoom.RoomName,
                                   RowSeat=p.RowSeat
                               }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamRoom.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, ExamCourseId=vm.ExamCourseId, ScheduleId = vm.ScheduleId, ExamId = vm.ExamId }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExamRoomStudentList(Models.ExamRoom.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ExamRoomStudentList", new { searchText = vm.SearchText, ExamId = vm.ExamId }));
        }

        public ActionResult ExamRoomStudentList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamRoom.List();

                vm.ExamList = Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                var tb = from p in db.Table<Exam.Entity.tbExamRoom>()
                         where p.tbExamCourse.IsDeleted == false
                         && p.tbExamCourse.tbCourse.IsDeleted == false
                         && p.tbRoom.IsDeleted == false
                         && p.tbExamCourse.tbExamSchedule.IsDeleted==false
                         && p.tbExamCourse.tbExamSchedule.tbExam.Id == vm.ExamId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ExamRoomName.Contains(vm.SearchText));
                }
                //考场学生
                vm.StudentList= (from p in db.Table<Exam.Entity.tbExamStudent>()
                               where p.tbStudent.IsDeleted == false
                               && p.tbExamRoom.IsDeleted == false
                               select new Dto.ExamRoom.List
                               {
                                   Id=p.tbExamRoom.Id,
                                   StudentCode=p.tbStudent.StudentCode,
                                   StudentName=p.tbStudent.StudentName
                               }).Distinct().ToList();
                //监考教师
                vm.ExamTeacherList = (from p in db.Table<Exam.Entity.tbExamTeacher>()
                               where p.tbExamRoom.IsDeleted == false
                               && p.tbTeacher.IsDeleted==false
                               select new Dto.ExamRoom.List
                               {
                                   Id=p.tbExamRoom.Id,
                                   TeacherCode=p.tbTeacher.TeacherCode,
                                   TeacherName=p.tbTeacher.TeacherName
                               }).Distinct().ToList();

                vm.ExamRoomList = (from p in tb
                                   orderby p.No descending
                                   select new Dto.ExamRoom.List
                                   {
                                       Id = p.Id,
                                       ExamCourseName = p.tbExamCourse.tbCourse.CourseName,
                                       ExamRoomName = p.ExamRoomName,
                                       RoomName = p.tbRoom.RoomName,
                                       RowSeat = p.RowSeat
                                   }).ToList();
                return View(vm);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamRoom>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除考场");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamRoom.Edit();
                vm.RoomList =Basis.Controllers.RoomController.SelectList();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamRoom>()
                              where p.Id == id && p.tbRoom.IsDeleted==false
                              select new Dto.ExamRoom.Edit
                              {
                                  Id = p.Id,
                                  RoomId=p.tbRoom.Id,
                                  ExamRoomName=p.ExamRoomName,
                                  ExamCourseId=p.tbExamCourse.Id,
                                  RowSeat=p.RowSeat
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamRoomEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamRoom.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamRoomEdit.Id == 0)
                    {
                        var tb = new Exam.Entity.tbExamRoom();
                        tb.ExamRoomName = vm.ExamRoomEdit.ExamRoomName;
                        tb.tbExamCourse = db.Set<Exam.Entity.tbExamCourse>().Find(vm.ExamRoomEdit.ExamCourseId);
                        tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.ExamRoomEdit.RoomId);
                        tb.RowSeat = vm.ExamRoomEdit.RowSeat;
                        db.Set<Exam.Entity.tbExamRoom>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考场");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExamRoom>()
                                  where p.Id == vm.ExamRoomEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.ExamRoomName = vm.ExamRoomEdit.ExamRoomName;
                            tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.ExamRoomEdit.RoomId);
                            tb.RowSeat = vm.ExamRoomEdit.RowSeat;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改考场");
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

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamRoom>()
                          orderby p.No descending
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ExamRoomName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        public ActionResult Import()
        {
            var vm = new Models.ExamRoom.Import();
            return View(vm);
        }

        public ActionResult ImportStudent()
        {
            var vm = new Models.ExamRoom.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Exam/Views/ExamRoom/ExamRoomImport.xls");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult ImportTemplateStudent()
        {
            var file = Server.MapPath("~/Areas/Exam/Views/ExamRoom/ExamStudentImport.xls");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.ExamRoom.Import vm)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);
                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 1、Excel模版校验
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
                    var tbList = new List<string>() { "考场名称", "考试教室"};

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
                    #endregion

                    #region 2、Excel数据读取
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dto = new Dto.ExamRoom.Import()
                        {
                            ExamRoomName = Convert.ToString(dr["考场名称"]),
                            RoomName = Convert.ToString(dr["考试教室"])
                        };
                        if (vm.ImportList.Where(d => d.ExamRoomName == dto.ExamRoomName
                                                && d.RoomName == dto.RoomName
                                                ).Count() == 0)
                        {
                            vm.ImportList.Add(dto);
                        }
                    }
                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.ExamRoomName) &&
                        string.IsNullOrEmpty(d.RoomName)
                    );
                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "Excel未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    #region 3、Excel格式校验
                    var roomList = (from p in db.Table<Basis.Entity.tbRoom>()
                                       select p).ToList();
                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.ExamRoomName))
                        {
                            item.Error = item.Error + "考场名称不能为空!";
                            continue;
                        }
                        if (string.IsNullOrEmpty(item.RoomName))
                        {
                            item.Error = item.Error + "考试教室不能为空!";
                            continue;
                        }
                        else
                        {
                            if (roomList.Where(d => d.RoomName == item.RoomName).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "考试教室不存在数据库!";
                                continue;
                            }
                        }
                    }
                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 4、Excel执行导入
                    var examRoomList = (from p in db.Table<Exam.Entity.tbExamRoom>()
                                        where p.tbExamCourse.Id==vm.ExamCourseId
                                        select p).ToList();

                    var addList = new List<Exam.Entity.tbExamRoom>();
                    foreach (var item in vm.ImportList)
                    {
                        Exam.Entity.tbExamRoom tb = null;
                        tb = examRoomList.Where(d => d.ExamRoomName == item.ExamRoomName).Select(d => d).FirstOrDefault();
                        if (tb!= null)
                        {
                            tb.tbRoom = (from p in db.Table<Basis.Entity.tbRoom>()
                                         where p.RoomName == item.RoomName
                                         select p).FirstOrDefault();
                        }
                        else
                        {
                            tb = new Exam.Entity.tbExamRoom();
                            tb.ExamRoomName = item.ExamRoomName;
                            tb.tbRoom = (from p in db.Table<Basis.Entity.tbRoom>()
                                         where p.RoomName == item.RoomName
                                         select p).FirstOrDefault();
                            tb.tbExamCourse = db.Set<Exam.Entity.tbExamCourse>().Find(vm.ExamCourseId);
                            tb.RowSeat = 5;
                            addList.Add(tb);
                        }
                    }
                    db.Set<Exam.Entity.tbExamRoom>().AddRange(addList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入考场");
                        vm.Status = true;
                    }
                    #endregion
                }
            }
            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportStudent(Models.ExamRoom.Import vm)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);
                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 1、Excel模版校验
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
                    var tbList = new List<string>() {"考场名称", "学生学号", "学生姓名" };

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
                    #endregion

                    #region 2、Excel数据读取
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dto = new Dto.ExamRoom.Import()
                        {
                            ExamRoomName = Convert.ToString(dr["考场名称"]),
                            StudentCode = Convert.ToString(dr["学号"]),
                            StudentName = Convert.ToString(dr["学生姓名"]),
                        };
                        if (vm.ImportList.Where(d => d.ExamRoomName == dto.ExamRoomName
                                                && d.StudentCode == dto.StudentCode
                                                && d.StudentName == dto.StudentName
                                                ).Count() == 0)
                        {
                            vm.ImportList.Add(dto);
                        }
                    }
                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.ExamRoomName) &&
                        string.IsNullOrEmpty(d.StudentCode) &&
                         string.IsNullOrEmpty(d.StudentName)
                    );
                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "Excel未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    #region 3、Excel格式校验
                    var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                        select p).ToList();
                    var examRoomList = (from p in db.Table<Exam.Entity.tbExamRoom>()
                                    where p.tbExamCourse.Id==vm.ExamCourseId
                                    select p).ToList();
                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.ExamRoomName))
                        {
                            item.Error = item.Error + "考场名称不能为空!";
                            continue;
                        }
                        else
                        {
                            if (examRoomList.Where(d => d.ExamRoomName == item.ExamRoomName).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "该考试课程的考场不存在数据库!";
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(item.StudentName))
                        {
                            item.Error = item.Error + "学生姓名不能为空!";
                            continue;
                        }
                        if (studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Count() == 0)
                        {
                            item.Error = item.Error + "学生姓名和学生学号不匹配!";
                        }
                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error = item.Error + "学生学号不能为空!";
                            continue;
                        }
                        else
                        {
                            if (studentList.Where(d => d.StudentCode == item.StudentCode).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "该学号不存在数据库!";
                                continue;
                            }
                        }
                    }
                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 4、Excel执行导入
                    var examStudentList = (from p in db.Table<Exam.Entity.tbExamStudent>()
                                        where p.tbStudent.IsDeleted==false
                                        && p.tbExamRoom.IsDeleted==false
                                        select p).ToList();

                    var addList = new List<Exam.Entity.tbExamStudent>();
                    foreach (var item in vm.ImportList)
                    {
                        Exam.Entity.tbExamStudent tb = null;
                        tb = examStudentList.Where(d => d.tbStudent.StudentCode == item.StudentCode && d.tbExamRoom.ExamRoomName==item.ExamRoomName).Select(d => d).FirstOrDefault();
                        if (tb == null)
                        {
                            tb = new Exam.Entity.tbExamStudent();
                            tb.tbExamRoom = (from p in db.Table<Exam.Entity.tbExamRoom>()
                                         where p.ExamRoomName == item.ExamRoomName && p.tbExamCourse.Id==vm.ExamCourseId
                                         select p).FirstOrDefault();
                            tb.tbStudent = (from p in db.Table<Student.Entity.tbStudent>()
                                            where p.StudentCode==item.StudentCode
                                            select p).FirstOrDefault();
                            addList.Add(tb);
                        }
                    }
                    db.Set<Exam.Entity.tbExamStudent>().AddRange(addList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入考场学生");
                        vm.Status = true;
                    }
                    #endregion
                }
            }
            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }
    }
}