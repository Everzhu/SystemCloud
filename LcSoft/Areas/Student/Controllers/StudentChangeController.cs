using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Student.Controllers
{
    public class StudentChangeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentChange.List();
                var tb = from p in db.Table<Student.Entity.tbStudentChange>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbStudentChangeType.StudentChangeTypeName.Contains(vm.SearchText)
                                || d.tbSysUser.UserCode.Contains(vm.SearchText)
                                || d.tbSysUser.UserName.Contains(vm.SearchText)
                                || d.tbStudent.StudentCode.Contains(vm.SearchText)
                                || d.tbStudent.StudentName.Contains(vm.SearchText)
                                || d.tbStudent.StudentNameEn.Contains(vm.SearchText));
                }

                vm.StudentChangeList = (from p in tb
                                        orderby p.InputDate descending
                                        select new Dto.StudentChange.List
                                        {
                                            Id = p.Id,
                                            StudentCode = p.tbStudent.StudentCode,
                                            StudentName = p.tbStudent.StudentName,
                                            StudentChangeTypeName = p.tbStudentChangeType.StudentChangeTypeName,
                                            InputDate = p.InputDate,
                                            UserName = p.tbSysUser.UserName,
                                            StudentChangeType = p.tbStudentChangeType.StudentChangeType
                                        }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudentChange.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, pageIndex = vm.Page.PageIndex, pageSize = vm.Page.PageSize }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Student.Entity.tbStudentChange>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除学生调动");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentChange.Edit();
                vm.StudentChangeTypeList = StudentChangeTypeController.SelectList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Student.Entity.tbStudentChange>()
                                .Include(d => d.tbStudentChangeType)
                              where p.Id == id
                              select new Dto.StudentChange.Edit
                              {
                                  Id = p.Id,
                                  StudentCode = p.tbStudent.StudentCode,
                                  StudentName = p.tbStudent.StudentName,
                                  StudentChangeTypeId = p.tbStudentChangeType.Id,
                                  Remark = p.Remark,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.StudentChangeEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudentChange.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var tb = (from p in db.Table<Student.Entity.tbStudentChange>()
                                .Include(d => d.tbStudent.tbSysUser)
                              where p.Id == vm.StudentChangeEdit.Id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.tbStudentChangeType = db.Set<Student.Entity.tbStudentChangeType>().Find(vm.StudentChangeEdit.StudentChangeTypeId);
                        tb.Remark = vm.StudentChangeEdit.Remark;

                        if (tb.tbStudentChangeType.StudentChangeTypeName == "复学")
                        {
                            tb.tbStudent.tbSysUser.IsDisable = false;
                        }
                        else
                        {
                            tb.tbStudent.tbSysUser.IsDisable = true;
                        }

                        // 如果休学、转学、开除、退学、出国、其他，删除学生
                        //StudentController.Delete(db, vm.StudentChangeEdit.UserCode);

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改学生调动");
                        }
                    }
                    else
                    {
                        var sCode = vm.StudentChangeEdit.StudentCode.Split('(')[0];
                        var studentUser = (from p in db.Table<Student.Entity.tbStudent>()
                                            .Include(d => d.tbSysUser)
                                           where p.StudentCode == sCode
                                           select p).FirstOrDefault();
                        if (studentUser == null)
                        {
                            error.AddError("学号对应学生信息不存在，请核对！");
                            return Code.MvcHelper.Post(error, "", "学号对应学生信息不存在，请核对！");
                        }

                        tb = new Student.Entity.tbStudentChange();
                        tb.tbStudent = studentUser;
                        tb.tbStudentChangeType = db.Table<Student.Entity.tbStudentChangeType>().Where(d => d.Id == vm.StudentChangeEdit.StudentChangeTypeId).FirstOrDefault();
                        tb.Remark = vm.StudentChangeEdit.Remark;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        tb.InputDate = DateTime.Now;
                        db.Set<Student.Entity.tbStudentChange>().Add(tb);

                        if (tb.tbStudentChangeType.StudentChangeTypeName == "复学")
                        {
                            tb.tbStudent.tbSysUser.IsDisable = false;
                        }
                        else
                        {
                            tb.tbStudent.tbSysUser.IsDisable = true;
                        }

                        // 如果休学、转学、开除、退学、出国、其他，删除学生
                        //StudentController.Delete(db, vm.StudentChangeEdit.UserCode);

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生调动");
                        }
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        /// <summary>
        /// 在校：调班，转入，复学
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentInSchool(int id = 0)
        {
            var vm = new Models.StudentChange.StudentInSchool();
            vm.ClassList = Basis.Controllers.ClassController.SelectList();
            vm.StudentChangeTypeList = StudentChangeTypeController.SelectList(Code.EnumHelper.StudentChangeType.InSchool);

            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.DataEdit = (from p in db.Table<Entity.tbStudentChange>()
                                   where p.Id == id
                                   select new Dto.StudentChange.StudentInSchool()
                                   {
                                       Id = p.Id,
                                       StudentId = p.tbStudent.Id,
                                       StudentChangeTypeId = p.tbStudentChangeType.Id,
                                       StudentCode = p.tbStudent.StudentCode,
                                       StudentName = p.tbStudent.StudentName,
                                       Remark = p.Remark
                                   }).FirstOrDefault();
                    if (db.Table<Basis.Entity.tbClassStudent>().Where(d => d.tbStudent.Id == vm.DataEdit.StudentId).Any())
                    {
                        vm.DataEdit.ClassId = db.Table<Basis.Entity.tbClassStudent>().Where(d => d.tbStudent.Id == vm.DataEdit.StudentId).Include(d => d.tbClass).FirstOrDefault().tbClass.Id;
                    }
                }
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentInSchool(Models.StudentChange.StudentInSchool vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var tb = new Student.Entity.tbStudentChange();
                    if (vm.DataEdit.Id > 0)
                    {

                        tb = db.Table<Entity.tbStudentChange>().Where(d => d.Id == vm.DataEdit.Id).Include(d => d.tbStudent.tbSysUser).FirstOrDefault();
                        tb.InputDate = DateTime.Now;
                        tb.Remark = vm.DataEdit.Remark;
                        tb.tbStudentChangeType = db.Set<Student.Entity.tbStudentChangeType>().Find(vm.DataEdit.StudentChangeTypeId);
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                    }
                    else
                    {
                        var studentCode = "";
                        if (vm.DataEdit.StudentCode.IndexOf('(') > 0)
                        {
                            studentCode = vm.DataEdit.StudentCode.Split('(')[0];
                        }
                        var student = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == studentCode).Include(d => d.tbSysUser).FirstOrDefault();
                        if (student == null)
                        {
                            error.Add("学生不存在！");
                            return Code.MvcHelper.Post(error);
                        }

                        tb = new Student.Entity.tbStudentChange()
                        {
                            InputDate = DateTime.Now,
                            Remark = vm.DataEdit.Remark,
                            tbStudent = student,
                            tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                            tbStudentChangeType = db.Set<Student.Entity.tbStudentChangeType>().Find(vm.DataEdit.StudentChangeTypeId)
                        };

                        db.Set<Student.Entity.tbStudentChange>().Add(tb);
                    }

                    tb.tbStudent.tbSysUser.IsDisable = false;
                    var classStudent = db.Table<Basis.Entity.tbClassStudent>().Where(d => d.tbStudent.StudentCode == vm.DataEdit.StudentCode).Include(d => d.tbClass).FirstOrDefault();
                    classStudent.tbClass = db.Set<Basis.Entity.tbClass>().Find(vm.DataEdit.ClassId);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生调动");
                    }

                }
            }
            return Code.MvcHelper.Post(error);
        }

        /// <summary>
        /// 离校：休学，转学，开除，退学，出国，其他
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentOutSchool(int id = 0)
        {
            var vm = new Models.StudentChange.StudentOutSchool();
            vm.StudentChangeTypeList = StudentChangeTypeController.SelectList(Code.EnumHelper.StudentChangeType.OutSchool);

            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.DataEdit = (from p in db.Table<Entity.tbStudentChange>()
                                   where p.Id == id
                                   select new Dto.StudentChange.StudentOutSchool()
                                   {
                                       Id = p.Id,
                                       StudentId = p.tbStudent.Id,
                                       StudentChangeTypeId = p.tbStudentChangeType.Id,
                                       StudentCode = p.tbStudent.StudentCode,
                                       StudentName = p.tbStudent.StudentName,
                                       Remark = p.Remark
                                   }).FirstOrDefault();
                }
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentOutSchool(Models.StudentChange.StudentOutSchool vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var tb = new Student.Entity.tbStudentChange();
                    if (vm.DataEdit.Id > 0)
                    {

                        tb = db.Table<Entity.tbStudentChange>().Where(d => d.Id == vm.DataEdit.Id).Include(d => d.tbStudent.tbSysUser).FirstOrDefault();
                        tb.InputDate = DateTime.Now;
                        tb.Remark = vm.DataEdit.Remark;
                        tb.tbStudentChangeType = db.Set<Student.Entity.tbStudentChangeType>().Find(vm.DataEdit.StudentChangeTypeId);
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                    }
                    else
                    {
                        var studentCode = "";
                        if (vm.DataEdit.StudentCode.IndexOf('(') > 0)
                        {
                            studentCode = vm.DataEdit.StudentCode.Split('(')[0];
                        }
                        var student = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == studentCode).Include(d => d.tbSysUser).FirstOrDefault();
                        if (student == null)
                        {
                            error.Add("学生不存在！");
                            return Code.MvcHelper.Post(error);
                        }

                        tb = new Student.Entity.tbStudentChange()
                        {
                            InputDate = DateTime.Now,
                            Remark = vm.DataEdit.Remark,
                            tbStudent = student,
                            tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                            tbStudentChangeType = db.Set<Student.Entity.tbStudentChangeType>().Find(vm.DataEdit.StudentChangeTypeId)
                        };

                        db.Set<Student.Entity.tbStudentChange>().Add(tb);
                    }

                    tb.tbStudent.IsDeleted = true;
                    tb.tbStudent.tbSysUser.IsDisable = true;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生调动");
                    }

                }
            }
            return Code.MvcHelper.Post(error);
        }

        public ActionResult StudentOut()
        {
            var vm = new Models.StudentChange.StudentOut();
            vm.StudentChangeTypeList = StudentChangeTypeController.SelectList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentOut(Models.StudentChange.StudentOut vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var studentCode = "";
                    if (vm.DataEdit.StudentCode.IndexOf('(') > 0)
                    {
                        studentCode = vm.DataEdit.StudentCode.Split('(')[0];
                    }
                    else
                    {
                        studentCode = vm.DataEdit.StudentCode;
                    }
                    var student = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == studentCode).Include(d => d.tbSysUser).FirstOrDefault();
                    if (student == null)
                    {
                        error.Add("学生不存在！");
                        return Code.MvcHelper.Post(error);
                    }
                    else
                    {
                        var tb = new Student.Entity.tbStudentChange()
                        {
                            InputDate = DateTime.Now,
                            Remark = vm.DataEdit.Remark,
                            tbStudent = student,
                            tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                            //tbStudentChangeType = db.Table<Student.Entity.tbStudentChangeType>().Where(d => d.StudentChangeTypeName == "转学").FirstOrDefault()
                            tbStudentChangeType = db.Set<Student.Entity.tbStudentChangeType>().Find(vm.DataEdit.StudentChangeTypeId)
                        };

                        if (tb.tbStudentChangeType.StudentChangeTypeName == "复学" || tb.tbStudentChangeType.StudentChangeTypeName == "转入")
                        {
                            tb.tbStudent.tbSysUser.IsDisable = false;
                        }
                        else
                        {
                            student.IsDeleted = true;
                            tb.tbStudent.tbSysUser.IsDisable = true;
                        }

                        db.Set<Student.Entity.tbStudentChange>().Add(tb);

                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生调动");
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult StudentIn()
        {
            var vm = new Models.StudentChange.StudentIn();
            vm.ClassList = Basis.Controllers.ClassController.SelectList();
            return View(vm);
            //using (var db = new XkSystem.Models.DbContext())
            //{
            //}
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentIn(Models.StudentChange.StudentIn vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var student = new Student.Entity.tbStudent()
                    {
                        StudentCode = vm.DataEdit.StudentCode,
                        StudentName = vm.DataEdit.StudentName,
                        tbSysUser = new Sys.Entity.tbSysUser()
                        {
                            UserCode = vm.DataEdit.StudentCode,
                            UserName = vm.DataEdit.StudentName,
                            Password = Code.Common.DESEnCode("123456"),
                            PasswordMd5 = Code.Common.CreateMD5Hash("123456"),
                            UserType = Code.EnumHelper.SysUserType.Student,
                            tbSex = db.Table<Dict.Entity.tbDictSex>().Where(d => d.SexName == vm.DataEdit.SexName).FirstOrDefault()
                        }
                    };

                    var tb = new Student.Entity.tbStudentChange()
                    {
                        InputDate = DateTime.Now,
                        Remark = vm.DataEdit.Remark,
                        tbStudent = student,
                        tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                        tbStudentChangeType = db.Table<Student.Entity.tbStudentChangeType>().Where(d => d.StudentChangeTypeName == "转入").FirstOrDefault()
                    };
                    db.Set<Student.Entity.tbStudentChange>().Add(tb);

                    var classStudent = new Basis.Entity.tbClassStudent()
                    {
                        tbClass = db.Set<Basis.Entity.tbClass>().Find(vm.DataEdit.ClassId),
                        tbStudent = student
                    };
                    db.Set<Basis.Entity.tbClassStudent>().Add(classStudent);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生调动");
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult StudentReset()
        {
            var vm = new Models.StudentChange.StudentReset();
            vm.ClassList = Basis.Controllers.ClassController.SelectList();
            vm.StudentSessionList = StudentSessionController.SelectList();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentReset(Models.StudentChange.StudentReset vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var student = db.Table<Entity.tbStudent>().Where(d => d.StudentCode == vm.DataEdit.StudentCode && d.StudentName == vm.DataEdit.StudentName).FirstOrDefault();
                    if (student == null)
                    {
                        error.Add("学生不存在！");
                        return Code.MvcHelper.Post(error);
                    }
                    student.tbStudentSession = db.Set<Student.Entity.tbStudentSession>().Find(vm.DataEdit.StudentSessionId);

                    var tb = new Student.Entity.tbStudentChange()
                    {
                        InputDate = DateTime.Now,
                        Remark = vm.DataEdit.Remark,
                        tbStudent = student,
                        tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                        tbStudentChangeType = db.Table<Student.Entity.tbStudentChangeType>().Where(d => d.StudentChangeTypeName == "复学").FirstOrDefault()
                    };

                    var classStudent = db.Table<Basis.Entity.tbClassStudent>().Where(d => d.tbStudent.Id == student.Id).FirstOrDefault();
                    if (classStudent != null)
                    {
                        classStudent.tbClass = db.Set<Basis.Entity.tbClass>().Find(vm.DataEdit.ClassId);
                    }
                    else
                    {
                        classStudent = new Basis.Entity.tbClassStudent()
                        {
                            tbClass = db.Set<Basis.Entity.tbClass>().Find(vm.DataEdit.ClassId),
                            tbStudent = student
                        };
                        db.Set<Basis.Entity.tbClassStudent>().Add(classStudent);
                    }


                    db.Set<Student.Entity.tbStudentChange>().Add(tb);
                    db.Set<Basis.Entity.tbClassStudent>().Add(classStudent);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生调动");
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult Export()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentChange.Export();
                var file = System.IO.Path.GetTempFileName();

                var tb = db.Table<Student.Entity.tbStudentChange>();

                #region 条件查询
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudentChangeType.StudentChangeTypeName.Contains(vm.SearchText)
                               || d.tbSysUser.UserCode.Contains(vm.SearchText)
                               || d.tbSysUser.UserName.Contains(vm.SearchText)
                               || d.tbStudent.StudentCode.Contains(vm.SearchText)
                               || d.tbStudent.StudentName.Contains(vm.SearchText)
                               || d.tbStudent.StudentNameEn.Contains(vm.SearchText));
                }
                #endregion

                vm.ExportList = (from p in tb
                                 orderby p.No
                                 select new Dto.StudentChange.Export()
                                 {
                                     Id = p.Id,
                                     No = p.No,
                                     ChangeTypeName = p.tbStudentChangeType.StudentChangeTypeName,
                                     Remark = p.Remark,
                                     StudentCode = p.tbStudent.StudentCode,
                                     StudentName = p.tbStudent.StudentName
                                 }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("排序"),
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("调动类型"),
                        new System.Data.DataColumn("备注"),
                    });
                foreach (var a in vm.ExportList)
                {
                    var dr = dt.NewRow();
                    dr["排序"] = a.No;
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["调动类型"] = a.ChangeTypeName;
                    dr["备注"] = a.Remark;
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
            var vm = new Models.StudentChange.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Student/Views/StudentChange/StudentChangeTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.StudentChange.Import vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                #region 上传excel文件,并转为DataTable
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

                var tbList = new List<string>() { "学号", "姓名", "调动类型", "备注" };

                var Text = string.Empty;
                foreach (var a in tbList)
                {
                    if (dt.Columns.Contains(a.ToString()) == false)
                    {
                        Text += a + ",";
                    }
                }

                if (string.IsNullOrEmpty(Text) == false)
                {
                    ModelState.AddModelError("", "上传的EXCEL内容与预期不一致，缺少对应的字段：" + Text);
                    return View(vm);
                }
                #endregion

                #region 将DataTable转为List
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var dto = new Dto.StudentChange.Import()
                    {
                        #region
                        StudentCode = dr["学号"].ConvertToString(),
                        StudentName = dr["姓名"].ConvertToString(),
                        ChangeTypeName = dr["调动类型"].ConvertToString(),
                        Remark = dr["备注"].ConvertToString(),
                        #endregion
                    };

                    if (vm.ImportList.Where(d => d.StudentName == dto.StudentName
                                             && d.StudentCode == dto.StudentCode
                                             && d.ChangeTypeName == dto.ChangeTypeName).Count() == 0)
                    {
                        vm.ImportList.Add(dto);
                    }
                }

                if (vm.ImportList.Count == 0)
                {
                    ModelState.AddModelError("", "未读取到任何有效数据!");
                    return View(vm);
                }
                #endregion

                var studentList = db.Table<Student.Entity.tbStudent>().ToList();
                var changeTypeList = db.Table<Student.Entity.tbStudentChangeType>().ToList();
                var studentChangeList = db.Table<Student.Entity.tbStudentChange>()
                    .Include(d => d.tbStudentChangeType)
                    .Include(d => d.tbStudent).ToList();

                #region 验证数据格式是否正确
                foreach (var v in vm.ImportList)
                {
                    if (string.IsNullOrEmpty(v.ChangeTypeName))
                    {
                        v.Error += "调动类型不能为空；";
                    }
                    if (changeTypeList.Where(d => d.StudentChangeTypeName == v.ChangeTypeName).Count() == 0)
                    {
                        v.Error += "调动类型不存在；";
                    }
                    if (string.IsNullOrEmpty(v.StudentName))
                    {
                        v.Error += "学生姓名不能为空；";
                    }
                    if (string.IsNullOrEmpty(v.StudentCode))
                    {
                        v.Error += "学生学号不能为空；";
                    }
                    if (studentList.Where(d => d.StudentCode == v.StudentCode && d.StudentName == v.StudentName).Count() == 0)
                    {
                        v.Error += "学生不存在；";
                    }
                    if (!vm.IsUpdate && studentChangeList.Where(d => d.tbStudent.StudentCode == v.StudentCode && d.tbStudentChangeType.StudentChangeTypeName == v.ChangeTypeName).Count() > 0)
                    {
                        v.Error += "系统中已存在该记录!";
                    }
                }

                if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                {
                    vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                    return View(vm);
                }
                #endregion

                #region 数据导入，判断关键字，存在的数据做更新，不存在的做新增
                var tbStudentChangeList = new List<Student.Entity.tbStudentChange>();
                foreach (var v in vm.ImportList)
                {
                    var tb = new Student.Entity.tbStudentChange();
                    if (studentChangeList.Where(d => d.tbStudent.StudentCode == v.StudentCode && d.tbStudentChangeType.StudentChangeTypeName == v.ChangeTypeName).Count() > 0)
                    {
                        #region 修改
                        if (vm.IsUpdate)
                        {
                            tb = studentChangeList.Where(d => d.tbStudent.StudentCode == v.StudentCode && d.tbStudentChangeType.StudentChangeTypeName == v.ChangeTypeName).FirstOrDefault();
                            tb.tbStudent = studentList.Where(d => d.StudentCode == v.StudentCode && d.StudentName == v.StudentName).FirstOrDefault();
                            tb.Remark = v.Remark;
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);

                            if (!string.IsNullOrEmpty(v.ChangeTypeName))
                            {
                                tb.tbStudentChangeType = changeTypeList.Where(d => d.StudentChangeTypeName == v.ChangeTypeName).FirstOrDefault();
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 新增
                        tb = new Student.Entity.tbStudentChange();
                        tb.InputDate = DateTime.Now;
                        tb.Remark = v.Remark;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        tb.tbStudent = studentList.Where(d => d.StudentCode == v.StudentCode && d.StudentName == v.StudentName).FirstOrDefault();

                        if (!string.IsNullOrEmpty(v.ChangeTypeName))
                        {
                            tb.tbStudentChangeType = changeTypeList.Where(d => d.StudentChangeTypeName == v.ChangeTypeName).FirstOrDefault();
                        }

                        tbStudentChangeList.Add(tb);
                        #endregion
                    }
                }

                db.Set<Student.Entity.tbStudentChange>().AddRange(tbStudentChangeList);

                if (db.SaveChanges() > 0)
                {
                    vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入学生异动");
                    vm.Status = true;
                }
                #endregion
            }

            return View(vm);
        }

        /// <summary>
        /// 学生异动通用方法
        /// </summary>
        public static bool StudentChange(Models.StudentChange.StudentChange sc)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var studentUser = (from p in db.Table<Sys.Entity.tbSysUser>()
                                   where p.UserType == Code.EnumHelper.SysUserType.Student
                                   && p.UserCode == sc.StudentCode
                                   select p).FirstOrDefault();
                var tb = new Student.Entity.tbStudentChange();
                tb.Remark = sc.Remark;
                tb.tbSysUser = studentUser;
                tb.InputDate = DateTime.Now;
                db.Set<Student.Entity.tbStudentChange>().Add(tb);
                db.SaveChanges();
                return true;
            }
        }
    }
}