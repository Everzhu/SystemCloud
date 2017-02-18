using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyRoomStudentController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyRoomStudent.List();

                vm.RoomName = db.Set<Basis.Entity.tbRoom>().Find(vm.RoomId).RoomName;

                var tb = from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                         where p.tbStudy.Id == vm.StudyId
                         && p.tbRoom.Id == vm.RoomId
                         && p.tbStudy.IsDeleted == false
                         && p.tbRoom.IsDeleted == false
                         && p.tbStudent.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }

                vm.StudyRoomStudentList = (from p in tb
                                           orderby p.tbStudent.StudentCode
                                           select new Dto.StudyRoomStudent.List
                                           {
                                               Id = p.Id,
                                               StudentId = p.tbStudent.Id,
                                               StudyId = p.tbStudy.Id,
                                               RoomId = p.tbRoom.Id,
                                               RoomName = p.tbRoom.RoomName,
                                               StudentCode = p.tbStudent.StudentCode,
                                               SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                               StudentName = p.tbStudent.StudentName
                                           }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudyRoomStudent.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                studyId = vm.StudyId,
                roomId = vm.RoomId,
                searchText = vm.SearchText
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了晚自习班级学生");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(List<int> ids, int studyId, int roomId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var StudyRoomStudentList = (from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                                            where p.tbStudy.Id == studyId
                                            && p.tbRoom.Id == roomId
                                            && p.tbStudy.IsDeleted == false
                                            && p.tbRoom.IsDeleted == false
                                            && p.tbStudent.IsDeleted == false
                                            select p.tbStudent.Id).ToList();

                var check = (from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                             where p.tbStudy.IsDeleted == false
                             && p.tbRoom.IsDeleted == false
                             && p.tbStudent.IsDeleted == false
                             && p.tbStudy.Id == studyId
                             && p.tbRoom.Id != roomId
                             && ids.Contains(p.tbStudent.Id)
                             select new
                             {
                                 p.tbStudent.StudentCode,
                                 p.tbStudent.StudentName,
                                 p.tbRoom.RoomName
                             }).ToList();

                if (check.Count > 0)
                {
                    error.AddError(string.Join("\r\n", check.Select(d => d.StudentCode + "(" + d.StudentName + ")已在" + d.RoomName).ToList()));
                }
                else
                {
                    var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                       where ids.Contains(p.Id) && StudyRoomStudentList.Contains(p.Id) == false
                                       select p).ToList();

                    foreach (var student in studentList)
                    {
                        var tb = new Study.Entity.tbStudyRoomStudent();
                        tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(roomId);
                        tb.tbStudent = student;
                        tb.tbStudy = db.Set<Study.Entity.tbStudy>().Find(studyId);
                        db.Set<Study.Entity.tbStudyRoomStudent>().Add(tb);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了晚自习班级学生");
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult Export()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyRoomStudent.List();
                var file = System.IO.Path.GetTempFileName();
                var tb = from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                         where p.tbStudy.Id == vm.StudyId
                         && p.tbRoom.Id == vm.RoomId
                         && p.tbStudy.IsDeleted == false
                         && p.tbRoom.IsDeleted == false
                         && p.tbStudent.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }

                vm.StudyRoomStudentList = (from p in tb
                                           orderby p.tbStudent.StudentCode
                                           select new Dto.StudyRoomStudent.List
                                           {
                                               Id = p.Id,
                                               StudentId = p.tbStudent.Id,
                                               StudyId = p.tbStudy.Id,
                                               RoomId = p.tbRoom.Id,
                                               RoomName = p.tbRoom.RoomName,
                                               StudentCode = p.tbStudent.StudentCode,
                                               SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                               StudentName = p.tbStudent.StudentName
                                           }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("序号"),
                        new System.Data.DataColumn("教室"),
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("性别"),
                    });

                var index = 0;
                foreach (var a in vm.StudyRoomStudentList)
                {
                    index++;
                    var dr = dt.NewRow();
                    dr["序号"] = index.ToString();
                    dr["教室"] = a.RoomName;
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["性别"] = a.SexName;
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

        public ActionResult Import()
        {
            var vm = new Models.StudyRoomStudent.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Study/Views/StudyRoomStudent/StudyRoomStudentTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.StudyRoomStudent.Import vm)
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
                    var tbList = new List<string>() { "晚自习名称", "教室", "学号", "姓名" };

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
                        ModelState.AddModelError("", "上传的EXCEL晚自习内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }
                    #endregion

                    #region 2、Excel数据读取
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dtoStudyRoomStudent = new Dto.StudyRoomStudent.Import()
                        {
                            StudyName = Convert.ToString(dr["晚自习名称"]),
                            RoomName = Convert.ToString(dr["教室"]),
                            StudentCode = Convert.ToString(dr["学号"]),
                            StudentName = Convert.ToString(dr["姓名"])
                        };
                        if (vm.ImportList.Where(d => d.StudyName == dtoStudyRoomStudent.StudyName
                                                && d.RoomName == dtoStudyRoomStudent.RoomName
                                                && d.StudentCode == dtoStudyRoomStudent.StudentCode
                                                && d.StudentName == dtoStudyRoomStudent.StudentName).Count() == 0)
                        {
                            vm.ImportList.Add(dtoStudyRoomStudent);
                        }
                    }
                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.StudyName) &&
                        string.IsNullOrEmpty(d.RoomName) &&
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
                    //学生列表
                    var StudyRoomStudentList = (from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                                                .Include(d => d.tbStudent)
                                                .Include(d => d.tbStudy)
                                                .Include(d => d.tbRoom)
                                                select p).ToList();
                    //学生列表
                    var StudentList = (from p in db.Table<Student.Entity.tbStudent>() select p).ToList();

                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.StudyName))
                        {
                            item.Error = item.Error + "晚自习名称不能为空!";
                            continue;
                        }
                        if (string.IsNullOrEmpty(item.RoomName))
                        {
                            item.Error = item.Error + "教室不能为空!";
                            continue;
                        }
                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error = item.Error + "学号不能为空!";
                            continue;
                        }
                        else
                        {
                            if (StudentList.Where(d => d.StudentCode == item.StudentCode).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "学生不存在数据库!";
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(item.StudentName))
                        {
                            item.Error = item.Error + "姓名不能为空!";
                            continue;
                        }
                        else
                        {
                            if (StudentList.Where(d => d.StudentName == item.StudentName).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "学生不存在数据库!";
                                continue;
                            }
                        }
                        if (StudyRoomStudentList.Where(d => d.tbStudy.Id == vm.StudyId && d.tbRoom.Id == vm.RoomId && d.tbStudent.StudentCode == item.StudentCode).Count() > 0)
                        {
                            item.Error += "系统中已存在该记录!";
                            continue;
                        }
                    }
                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 4、Excel执行导入
                    var addStudyRoomStudentList = new List<Study.Entity.tbStudyRoomStudent>();
                    foreach (var item in vm.ImportList)
                    {
                        Study.Entity.tbStudyRoomStudent tb = null;
                        if (StudyRoomStudentList.Where(d => d.tbStudy.Id == vm.StudyId && d.tbRoom.Id == vm.RoomId && d.tbStudent.StudentCode == item.StudentCode).Count() == decimal.Zero)
                        {
                            tb = new Study.Entity.tbStudyRoomStudent();
                            tb.tbStudy = db.Set<Study.Entity.tbStudy>().Find(vm.StudyId);
                            tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.RoomId);
                            tb.tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == item.StudentCode).FirstOrDefault();
                            addStudyRoomStudentList.Add(tb);
                        }
                    }
                    db.Set<Study.Entity.tbStudyRoomStudent>().AddRange(addStudyRoomStudentList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了晚自习教室学生");
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