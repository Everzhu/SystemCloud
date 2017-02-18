using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Student.Controllers
{
    public class StudentHonorController : Controller
    {
        /// <summary>
        /// 学生信息添加里面的荣誉列表
        /// </summary>
        public ActionResult List(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentHonor.List();
                var tb = from p in db.Table<Student.Entity.tbStudentHonor>()
                         where p.tbStudent.Id == id
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.HonorName.Contains(vm.SearchText));
                }

                vm.StudentHonorList = (from p in tb
                                       orderby p.Id
                                       select new Dto.StudentHonor.List
                                       {
                                           Id = p.Id,
                                           HonorName = p.HonorName,
                                           HonorFile = p.HonorFile,
                                           StudentHonorLevelName = p.tbstudentHonorLevel.StudentHonorLevelName,
                                           StudentHonorTypeName = p.tbStudentHonorType.StudentHonorTypeName
                                       }).ToList();

                return PartialView(vm);
            }
        }

        [HttpPost]
        public ActionResult List(Models.StudentHonor.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        public ActionResult Delete(string guid)
        {
            return Code.MvcHelper.Post();
        }

        public ActionResult Edit()
        {
            var vm = new Models.StudentHonor.Edit();
            vm.StudentHonorLevelList = StudentHonorLevelController.SelectList();
            vm.StudentHonorTypeList = StudentHonorTypeController.SelectList();
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudentHonor.Edit vm)
        {
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                var fileName = string.Empty;

                var file = Request.Files["StudentHonorEdit.HonorFile"];
                if (Code.Common.GetFileType(file.FileName) != Code.FileType.Image)
                {
                    return Content("<script >alert('图片格式必须是jpg、jpeg、png、bmp格式！');history.go(-1);</script >", "text/html");
                }

                if (error.Count == decimal.Zero)
                {
                    if (string.IsNullOrEmpty(file.FileName) == false)
                    {
                        fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                        var fileSave = Server.MapPath("~/Files/StudentHonor/");
                        file.SaveAs(fileSave + fileName);
                    }
                }

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    ID = 0,
                    HonorName = vm.StudentHonorEdit.HonorName,
                    FileName = fileName,
                    StudentHonorLevelId = vm.StudentHonorEdit.StudentHonorLevelId,
                    StudentHonorTypeId = vm.StudentHonorEdit.StudentHonorTypeId,
                    YearId = vm.StudentHonorEdit.YearId,
                    StudentHonorLevelName = db.Set<Student.Entity.tbStudentHonorLevel>().Find(vm.StudentHonorEdit.StudentHonorLevelId.ConvertToInt()).StudentHonorLevelName,
                    StudentHonorTypeName = db.Set<Student.Entity.tbStudentHonorType>().Find(vm.StudentHonorEdit.StudentHonorTypeId.ConvertToInt()).StudentHonorTypeName
                });
                return Content("<script>window.parent.HonorEditCallBack('" + json + "');</script>");
            }
        }

        public bool InsertHonor(XkSystem.Models.DbContext db, Student.Entity.tbStudent student, string json)
        {
            var error = new List<string>();
            if (json.Length > 20)
            {
                List<Dto.StudentHonor.InsertHonor> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dto.StudentHonor.InsertHonor>>(json);
                foreach (var v in list)
                {
                    if (v.ID > 0)
                    {
                        #region 修改
                        var tb = (from p in db.Table<Student.Entity.tbStudentHonor>()
                                  where p.tbStudent.StudentCode == student.StudentCode && p.Id == v.ID
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.HonorName = v.HonorName;
                            tb.HonorFile = v.FileName;
                            tb.tbstudentHonorLevel = db.Set<Student.Entity.tbStudentHonorLevel>().Find(v.StudentHonorLevelId);
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(v.YearId);
                            tb.HonorSource = Code.EnumHelper.StudentHonorSource.Teacher;
                            tb.tbStudentHonorType = db.Set<Student.Entity.tbStudentHonorType>().Find(v.StudentHonorTypeId);
                            tb.tbUserInput = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改学生荣誉");
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                            return false;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 新增
                        var tb = new Student.Entity.tbStudentHonor()
                        {
                            tbStudent = student,
                            HonorName = v.HonorName,
                            HonorFile = v.FileName,
                            tbstudentHonorLevel = db.Set<Student.Entity.tbStudentHonorLevel>().Find(v.StudentHonorLevelId),
                            tbYear = db.Set<Basis.Entity.tbYear>().Find(v.YearId),
                            HonorSource = Code.EnumHelper.StudentHonorSource.Teacher,
                            tbStudentHonorType = db.Set<Student.Entity.tbStudentHonorType>().Find(v.StudentHonorTypeId),
                            tbUserInput = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                            CheckStatus = Code.EnumHelper.CheckStatus.Success,
                            InputDate = DateTime.Now
                        };

                        db.Set<Student.Entity.tbStudentHonor>().Add(tb);
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生荣誉");
                        #endregion
                    }
                }
                #region 删除ID不属于json的全部数据
                var honorList = (from p in db.Table<Student.Entity.tbStudentHonor>()
                                 where p.tbStudent.StudentCode == student.StudentCode
                                 select p).ToList();
                foreach (var v in honorList)
                {
                    if (list.Where(d => d.ID == v.Id).Count() == 0)
                    {
                        v.IsDeleted = true;
                    }
                }
                #endregion
            }
            else
            {
                #region 删除全部数据
                var honorList = (from p in db.Table<Student.Entity.tbStudentHonor>()
                                 where p.tbStudent.StudentCode == student.StudentCode
                                 select p).ToList();
                foreach (var v in honorList)
                {
                    v.IsDeleted = true;
                }
                #endregion
            }
            return true;
        }



        /// <summary>
        /// 荣誉菜单中的列表
        /// </summary>
        public ActionResult HonorList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentHonor.HonorList();
                vm.honorLevelList = StudentHonorLevelController.SelectList();
                vm.honorTypeList = StudentHonorTypeController.SelectList();
                vm.CheckStatusList = new List<SelectListItem>() {
                    new SelectListItem() { Text="荣誉状态",Value="-2"},
                    new SelectListItem() { Text="未处理",Value="0"},
                    new SelectListItem() { Text="通过",Value="1"},
                    new SelectListItem() { Text="不通过",Value="-1"}
                };

                var tb = db.Table<Student.Entity.tbStudentHonor>();

                if (Code.Common.UserType == Code.EnumHelper.SysUserType.Student)
                {
                    tb = tb.Where(d => d.tbStudent.tbSysUser.Id == Code.Common.UserId);
                }
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.HonorName.Contains(vm.SearchText));
                }

                if (vm.honorLevelId > 0)
                {
                    tb = tb.Where(d => d.tbstudentHonorLevel.Id == vm.honorLevelId);
                }

                if (vm.honorTypeId > 0)
                {
                    tb = tb.Where(d => d.tbStudentHonorType.Id == vm.honorTypeId);
                }

                if (vm.CheckStatusId > -2)
                {
                    tb = tb.Where(d => d.CheckStatus == (Code.EnumHelper.CheckStatus)vm.CheckStatusId);
                }

                vm.honorList = (from p in tb
                                orderby p.tbStudent.StudentCode
                                select new Dto.StudentHonor.HonorList()
                                {
                                    Id = p.Id,
                                    UserId = p.tbStudent.tbSysUser.Id,
                                    HonorFile = p.HonorFile,
                                    HonorName = p.HonorName,
                                    InputUserName = p.tbUserInput.UserName,
                                    StudentCode = p.tbStudent.StudentCode,
                                    Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                    StudentName = p.tbStudent.StudentName,
                                    StudentHonorLevelName = p.tbstudentHonorLevel.StudentHonorLevelName,
                                    HonorSource = p.HonorSource,
                                    StudentHonorTypeName = p.tbStudentHonorType.StudentHonorTypeName,
                                    CheckStatus = p.CheckStatus
                                }).ToPageList(vm.Page);

                foreach (var v in vm.honorList)
                {
                    switch (v.CheckStatus)
                    {
                        case Code.EnumHelper.CheckStatus.Fail: v.CheckStatusName = "不通过"; break;
                        case Code.EnumHelper.CheckStatus.None: v.CheckStatusName = "未处理"; break;
                        default: v.CheckStatusName = "通过"; break;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult HonorList(Models.StudentHonor.HonorList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("HonorList", new
            {
                searchText = vm.SearchText,
                honorLevelId = vm.honorLevelId,
                honorTypeId = vm.honorTypeId,
                CheckStatusId = vm.CheckStatusId
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HonorDelete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Student.Entity.tbStudentHonor>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除学生");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult HonorEdit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentHonor.HonorEdit();

                vm.StudentHonorLevelList = StudentHonorLevelController.SelectList();
                vm.StudentHonorTypeList = StudentHonorTypeController.SelectList();

                if (id != 0)
                {
                    vm.honorEdit = (from p in db.Table<Student.Entity.tbStudentHonor>()
                                    where p.Id == id
                                    select new Dto.StudentHonor.HonorEdit()
                                    {
                                        Id = p.Id,
                                        HonorFile = p.HonorFile,
                                        HonorName = p.HonorName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentHonorLevelId = p.tbstudentHonorLevel.Id,
                                        StudentHonorTypeId = p.tbStudentHonorType.Id
                                    }).FirstOrDefault();

                    vm.StudentHonorLevelList.Where(d => d.Value == vm.honorEdit.StudentHonorLevelId.ConvertToString()).FirstOrDefault().Selected = true;
                    vm.StudentHonorTypeList.Where(d => d.Value == vm.honorEdit.StudentHonorTypeId.ConvertToString()).FirstOrDefault().Selected = true;
                }

                return View(vm);
            }
        }

        public ActionResult HonorInfo(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentHonor.HonorEdit();

                vm.StudentHonorLevelList = StudentHonorLevelController.SelectList();
                vm.StudentHonorTypeList = StudentHonorTypeController.SelectList();

                if (id != 0)
                {
                    vm.honorEdit = (from p in db.Table<Student.Entity.tbStudentHonor>()
                                    where p.Id == id
                                    select new Dto.StudentHonor.HonorEdit()
                                    {
                                        Id = p.Id,
                                        HonorFile = p.HonorFile,
                                        CheckRemark = p.CheckRemark,
                                        HonorName = p.HonorName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentHonorLevelId = p.tbstudentHonorLevel.Id,
                                        StudentHonorTypeId = p.tbStudentHonorType.Id
                                    }).FirstOrDefault();

                    vm.StudentHonorLevelList.Where(d => d.Value == vm.honorEdit.StudentHonorLevelId.ConvertToString()).FirstOrDefault().Selected = true;
                    vm.StudentHonorTypeList.Where(d => d.Value == vm.honorEdit.StudentHonorTypeId.ConvertToString()).FirstOrDefault().Selected = true;
                }

                return View(vm);
            }
        }

        public ActionResult HonorFileInfo(int id)
        {
            var vm = new Models.StudentHonor.HonorFileInfo();
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Student.Entity.tbStudentHonor>().Find(id);
                vm.fileInfo = new Dto.StudentHonor.HonorFileInfo()
                {
                    HonorName = tb.HonorName,
                    HonorFile = tb.HonorFile
                };

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HonorEdit(Models.StudentHonor.HonorEdit vm)
        {
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                var fileName = string.Empty;

                var file = Request.Files["HonorEdit.HonorFile"];
                if (Code.Common.GetFileType(file.FileName) != Code.FileType.Image)
                {
                    return Content("<script >alert('图片格式必须是jpg、jpeg、png、bmp格式！');history.go(-1);</script >", "text/html");
                }
                if (db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == vm.honorEdit.StudentCode).Count() == 0)
                {
                    return Content("<script >alert('学号不存在！');history.go(-1);</script >", "text/html");
                }

                if (error.Count == decimal.Zero)
                {
                    if (string.IsNullOrEmpty(file.FileName) == false)
                    {
                        fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                        var fileSave = Server.MapPath("~/Files/StudentHonor/");
                        file.SaveAs(fileSave + fileName);
                    }
                }

                #region 判断老师只能导入本班级学生荣誉
                if (Code.Common.UserType == Code.EnumHelper.SysUserType.Teacher)
                {
                    var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>()
                        .Where(d => d.tbTeacher.Id == Code.Common.UserId).Include(d => d.tbClass).ToList();
                    var ClassIdList = new List<int>();
                    foreach (var v in classTeacherList)
                    {
                        ClassIdList.Add(v.tbClass.Id);
                    }
                    var classStudentList = db.Table<Basis.Entity.tbClassStudent>()
                        .Where(d => ClassIdList.Contains(d.tbClass.Id)).Include(d => d.tbStudent).ToList();
                    if (classStudentList.Where(d => d.tbStudent.StudentCode == vm.honorEdit.StudentCode).Count() == 0)
                    {
                        return Content("<script>alert('学生不在您的班级内！');history.go(-1);</script>");
                    }
                }
                #endregion


                if (vm.honorEdit.Id > 0)
                {
                    var honor = db.Set<Student.Entity.tbStudentHonor>().Find(vm.honorEdit.Id);
                    honor.tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == vm.honorEdit.StudentCode).FirstOrDefault();
                    honor.HonorName = vm.honorEdit.HonorName;
                    honor.tbstudentHonorLevel = db.Set<Student.Entity.tbStudentHonorLevel>().Find(vm.honorEdit.StudentHonorLevelId);
                    honor.HonorSource = Code.EnumHelper.StudentHonorSource.Teacher;
                    honor.CheckStatus = Code.EnumHelper.CheckStatus.Success;
                    honor.HonorFile = fileName;
                    honor.InputDate = DateTime.Now;
                    honor.tbStudentHonorType = db.Set<Student.Entity.tbStudentHonorType>().Find(vm.honorEdit.StudentHonorTypeId);
                    honor.tbUserInput = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                }
                else
                {
                    if (db.Table<Student.Entity.tbStudentHonor>().Where(d => d.tbStudent.StudentCode == vm.honorEdit.StudentCode
                                    && d.HonorName == vm.honorEdit.HonorName
                                    && d.tbstudentHonorLevel.Id == vm.honorEdit.StudentHonorLevelId
                                    && d.tbStudentHonorType.Id == vm.honorEdit.StudentHonorTypeId).Count() == 0)
                    {
                        var honor = new Student.Entity.tbStudentHonor()
                        {
                            CheckStatus = Code.EnumHelper.CheckStatus.Success,
                            HonorFile = fileName,
                            HonorName = vm.honorEdit.HonorName,
                            InputDate = DateTime.Now,
                            tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == vm.honorEdit.StudentCode).FirstOrDefault(),
                            tbstudentHonorLevel = db.Set<Student.Entity.tbStudentHonorLevel>().Find(vm.honorEdit.StudentHonorLevelId),
                            HonorSource = Code.EnumHelper.StudentHonorSource.Teacher,
                            tbStudentHonorType = db.Set<Student.Entity.tbStudentHonorType>().Find(vm.honorEdit.StudentHonorTypeId),
                            tbUserInput = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId)
                        };
                        db.Set<Student.Entity.tbStudentHonor>().Add(honor);
                    }
                    else
                    {
                        return Content("<script >alert('该学生已有相同荣誉！');window.parent.location.reload();</script >", "text/html");
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加/修改学生荣誉");
                }

                return Content("<script >window.parent.location.reload();</script >", "text/html");
            }
        }

        public ActionResult Import()
        {
            var vm = new Models.StudentHonor.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Student/Views/StudentHonor/StudentHonorTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.StudentHonor.Import vm)
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

                    var tbList = new List<string>() { "排序", "荣誉名称", "学生学号", "学生姓名", "获奖级别", "荣誉类型" };

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

                    #region 将DataTable转为List
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dto = new Dto.StudentHonor.Import()
                        {
                            HonorLevelName = dr["获奖级别"].ConvertToString(),
                            HonorName = dr["荣誉名称"].ConvertToString(),
                            HonorTypeName = dr["荣誉类型"].ConvertToString(),
                            No = dr["排序"].ConvertToString(),
                            StudentCode = dr["学生学号"].ConvertToString(),
                            StudentName = dr["学生姓名"].ConvertToString()
                        };

                        if (vm.ImportList.Where(d => d.HonorLevelName == dto.HonorLevelName
                                                 && d.HonorName == dto.HonorName
                                                 && d.HonorTypeName == dto.HonorTypeName
                                                 && d.No == dto.No
                                                 && d.StudentCode == dto.StudentCode
                                                 && d.StudentName == dto.StudentName).Count() == 0)
                        {
                            vm.ImportList.Add(dto);
                        }
                    }

                    vm.ImportList.RemoveAll(d =>
                           string.IsNullOrEmpty(d.HonorLevelName)
                        && string.IsNullOrEmpty(d.HonorName)
                        && string.IsNullOrEmpty(d.HonorTypeName)
                        && string.IsNullOrEmpty(d.No)
                        && string.IsNullOrEmpty(d.StudentCode)
                        && string.IsNullOrEmpty(d.StudentName));

                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    var studentList = db.Table<Student.Entity.tbStudent>().ToList();
                    var honorLevelList = db.Table<Student.Entity.tbStudentHonorLevel>().ToList();
                    var honorTypeList = db.Table<Student.Entity.tbStudentHonorType>().ToList();
                    var honorList = db.Table<Student.Entity.tbStudentHonor>()
                                        .Include(d => d.tbStudent)
                                        .Include(d => d.tbstudentHonorLevel)
                                        .Include(d => d.tbStudentHonorType).ToList();

                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportList)
                    {
                        //判断老师只能导入本班级学生荣誉
                        if (Code.Common.UserType == Code.EnumHelper.SysUserType.Teacher)
                        {
                            var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>()
                                .Where(d => d.tbTeacher.Id == Code.Common.UserId).Include(d => d.tbClass).ToList();
                            var ClassIdList = new List<int>();
                            foreach (var v in classTeacherList)
                            {
                                ClassIdList.Add(v.tbClass.Id);
                            }
                            var classStudentList = db.Table<Basis.Entity.tbClassStudent>()
                                .Where(d => ClassIdList.Contains(d.tbClass.Id)).Include(d => d.tbStudent).ToList();
                            if (classStudentList.Where(d => d.tbStudent.StudentCode == item.StudentCode).Count() == 0)
                            {
                                item.Error += "学生不在您的班级内；";
                            }
                        }

                        if (string.IsNullOrEmpty(item.HonorLevelName))
                        {
                            item.Error = item.Error + "获奖级别不能为空!";
                        }
                        if (string.IsNullOrEmpty(item.HonorName))
                        {
                            item.Error = item.Error + "荣誉名称不能为空!";
                        }
                        if (string.IsNullOrEmpty(item.HonorTypeName))
                        {
                            item.Error = item.Error + "荣誉类型不能为空!";
                        }
                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error = item.Error + "学号不能为空!";
                        }
                        if (string.IsNullOrEmpty(item.StudentName))
                        {
                            item.Error = item.Error + "学生姓名不能为空!";
                        }
                        if (studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Count() == 0)
                        {
                            item.Error += "学生不存在;";
                        }
                        if (!vm.IsAddLevel && honorLevelList.Where(d => d.StudentHonorLevelName == item.HonorLevelName).Count() == 0)
                        {
                            item.Error += "获奖级别不存在；";
                        }
                        if (!vm.IsAddType && honorTypeList.Where(d => d.StudentHonorTypeName == item.HonorTypeName).Count() == 0)
                        {
                            item.Error += "荣誉类型不存在；";
                        }
                        if (!string.IsNullOrEmpty(item.No))
                        {
                            int no = 0;
                            if (!int.TryParse(item.No, out no))
                            {
                                item.Error += "排序必须为数字类型;";
                            }
                        }
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 添加外键
                    var editHonorLevelList = new List<Dto.StudentHonorLevel.Edit>();
                    var editHonorTypeList = new List<Dto.StudentHonorType.Edit>();

                    foreach (var v in vm.ImportList)
                    {
                        if (!string.IsNullOrEmpty(v.HonorLevelName)
                            && honorLevelList.Where(d => d.StudentHonorLevelName == v.HonorLevelName).Count() == 0)
                        {
                            var honorLevel = new Dto.StudentHonorLevel.Edit()
                            {
                                No = 0,
                                StudentHonorLevelName = v.HonorLevelName
                            };
                            if (editHonorLevelList.Where(d => d.StudentHonorLevelName == honorLevel.StudentHonorLevelName).Count() == 0)
                            {
                                editHonorLevelList.Add(honorLevel);
                            }
                        }
                        if (!string.IsNullOrEmpty(v.HonorTypeName)
                            && honorTypeList.Where(d => d.StudentHonorTypeName == v.HonorTypeName).Count() == 0)
                        {
                            var honorType = new Dto.StudentHonorType.Edit()
                            {
                                No = 0,
                                StudentHonorTypeName = v.HonorTypeName
                            };
                            if (editHonorTypeList.Where(d => d.StudentHonorTypeName == honorType.StudentHonorTypeName).Count() == 0)
                            {
                                editHonorTypeList.Add(honorType);
                            }
                        }
                    }
                    var addHonorLevelList = new List<Student.Entity.tbStudentHonorLevel>();
                    var addHonorTypeList = new List<Student.Entity.tbStudentHonorType>();

                    if (editHonorLevelList.Count > 0)
                    {
                        addHonorLevelList = StudentHonorLevelController.BuildList(db, editHonorLevelList);
                    }
                    if (editHonorTypeList.Count > 0)
                    {
                        addHonorTypeList = StudentHonorTypeController.BuildList(db, editHonorTypeList);
                    }
                    #endregion

                    #region 数据导入，判断关键字，存在的数据做更新，不存在的做新增

                    var addStudentHonor = new List<Student.Entity.tbStudentHonor>();

                    foreach (var item in vm.ImportList)
                    {
                        if (vm.IsUpdate && honorList.Where(d => d.HonorName == item.HonorName).Count() > 0)
                        {
                            var tb = honorList.Where(d => d.HonorName == item.HonorName).FirstOrDefault();
                            tb.CheckStatus = Code.EnumHelper.CheckStatus.Success;
                            tb.HonorName = item.HonorName;
                            tb.No = item.No.ConvertToInt();
                            tb.tbUserInput = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            tb.HonorSource = Code.EnumHelper.StudentHonorSource.Init;

                            #region 添加外键
                            if (!string.IsNullOrEmpty(item.StudentCode) && !string.IsNullOrEmpty(item.StudentName))
                            {
                                tb.tbStudent = studentList.Where(d => d.StudentCode == item.StudentCode).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.HonorLevelName))
                            {
                                if (vm.IsAddLevel && honorLevelList.Where(d => d.StudentHonorLevelName == item.HonorLevelName).Count() == 0)
                                {
                                    tb.tbstudentHonorLevel = addHonorLevelList.Where(d => d.StudentHonorLevelName == item.HonorLevelName).FirstOrDefault();
                                }
                                else
                                {
                                    tb.tbstudentHonorLevel = honorLevelList.Where(d => d.StudentHonorLevelName == item.HonorLevelName).FirstOrDefault();
                                }
                            }
                            if (!string.IsNullOrEmpty(item.HonorTypeName))
                            {
                                if (vm.IsAddType && honorTypeList.Where(d => d.StudentHonorTypeName == item.HonorTypeName).Count() == 0)
                                {
                                    tb.tbStudentHonorType = addHonorTypeList.Where(d => d.StudentHonorTypeName == item.HonorTypeName).FirstOrDefault();
                                }
                                else
                                {
                                    tb.tbStudentHonorType = honorTypeList.Where(d => d.StudentHonorTypeName == item.HonorTypeName).FirstOrDefault();
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            var tb = new Student.Entity.tbStudentHonor()
                            {
                                CheckStatus = Code.EnumHelper.CheckStatus.Success,
                                HonorName = item.HonorName,
                                InputDate = DateTime.Now,
                                No = item.No.ConvertToInt(),
                                tbUserInput = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                                HonorSource = Code.EnumHelper.StudentHonorSource.Init
                            };

                            #region 添加外键
                            if (!string.IsNullOrEmpty(item.StudentCode) && !string.IsNullOrEmpty(item.StudentName))
                            {
                                tb.tbStudent = studentList.Where(d => d.StudentCode == item.StudentCode).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.HonorLevelName))
                            {
                                if (vm.IsAddLevel && honorLevelList.Where(d => d.StudentHonorLevelName == item.HonorLevelName).Count() == 0)
                                {
                                    tb.tbstudentHonorLevel = addHonorLevelList.Where(d => d.StudentHonorLevelName == item.HonorLevelName).FirstOrDefault();
                                }
                                else
                                {
                                    tb.tbstudentHonorLevel = honorLevelList.Where(d => d.StudentHonorLevelName == item.HonorLevelName).FirstOrDefault();
                                }
                            }
                            if (!string.IsNullOrEmpty(item.HonorTypeName))
                            {
                                if (vm.IsAddType && honorTypeList.Where(d => d.StudentHonorTypeName == item.HonorTypeName).Count() == 0)
                                {
                                    tb.tbStudentHonorType = addHonorTypeList.Where(d => d.StudentHonorTypeName == item.HonorTypeName).FirstOrDefault();
                                }
                                else
                                {
                                    tb.tbStudentHonorType = honorTypeList.Where(d => d.StudentHonorTypeName == item.HonorTypeName).FirstOrDefault();
                                }
                            }
                            #endregion

                            addStudentHonor.Add(tb);
                        }
                    }
                    #endregion

                    db.Set<Student.Entity.tbStudentHonor>().AddRange(addStudentHonor);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入教师资料");
                        vm.Status = true;
                    }
                }
            }

            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public ActionResult Export()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentHonor.Export();

                var file = System.IO.Path.GetTempFileName();
                var tb = from p in db.Table<Student.Entity.tbStudentHonor>()
                         select p;
                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.HonorName.Contains(vm.SearchText));
                }
                vm.exportList = (from p in tb
                                 select new Dto.StudentHonor.Export()
                                 {
                                     HonorLevelName = p.tbstudentHonorLevel.StudentHonorLevelName,
                                     HonorName = p.HonorName,
                                     No = p.No,
                                     HonorTypeName = p.tbStudentHonorType.StudentHonorTypeName,
                                     StudentCode = p.tbStudent.StudentCode,
                                     StudentName = p.tbStudent.StudentName
                                 }).ToList();
                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("排序"),
                        new System.Data.DataColumn("荣誉名称"),
                        new System.Data.DataColumn("学生学号"),
                        new System.Data.DataColumn("学生姓名"),
                        new System.Data.DataColumn("获奖级别"),
                        new System.Data.DataColumn("荣誉类型")
                    });
                foreach (var a in vm.exportList)
                {
                    var dr = dt.NewRow();
                    dr["排序"] = a.No;
                    dr["荣誉名称"] = a.HonorName;
                    dr["学生学号"] = a.StudentCode;
                    dr["学生姓名"] = a.StudentName;
                    dr["获奖级别"] = a.HonorLevelName;
                    dr["荣誉类型"] = a.HonorTypeName;
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

        public ActionResult ApplyHonor()
        {
            var vm = new Models.StudentHonor.ApplyHonor();
            vm.StudentHonorLevelList = StudentHonorLevelController.SelectList();
            vm.StudentHonorTypeList = StudentHonorTypeController.SelectList();
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyHonor(Models.StudentHonor.ApplyHonor vm)
        {
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                var fileName = string.Empty;

                var file = Request.Files["ApplyHonorEdit.HonorFile"];
                if (Code.Common.GetFileType(file.FileName) != Code.FileType.Image)
                {
                    return Content("<script >alert('图片格式必须是jpg、jpeg、png、bmp格式！');history.go(-1);</script >", "text/html");
                }

                if (error.Count == decimal.Zero)
                {
                    if (string.IsNullOrEmpty(file.FileName) == false)
                    {
                        fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                        var fileSave = Server.MapPath("~/Files/StudentHonor/");
                        file.SaveAs(fileSave + fileName);
                    }
                }

                var user = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);

                var honor = new Student.Entity.tbStudentHonor()
                {
                    CheckStatus = Code.EnumHelper.CheckStatus.None,
                    HonorFile = fileName,
                    HonorName = vm.ApplyHonorEdit.HonorName,
                    InputDate = DateTime.Now,
                    No = 0,
                    tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.ApplyHonorEdit.YearId),
                    tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == user.UserCode).FirstOrDefault(),
                    tbstudentHonorLevel = db.Set<Student.Entity.tbStudentHonorLevel>().Find(vm.ApplyHonorEdit.StudentHonorLevelId),
                    HonorSource = Code.EnumHelper.StudentHonorSource.Apply,
                    tbStudentHonorType = db.Set<Student.Entity.tbStudentHonorType>().Find(vm.ApplyHonorEdit.StudentHonorTypeId),
                    tbUserInput = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId)
                };
                db.Set<Student.Entity.tbStudentHonor>().Add(honor);

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("学生申请荣誉");
                }

                //return Code.MvcHelper.Post(error);
                return Content("<script >alert('操作成功！');window.parent.location.reload();</script >", "text/html");
            }
        }




        public ActionResult ReViewApplyHonorList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentHonor.ReViewApplyHonorList();
                vm.honorLevelList = StudentHonorLevelController.SelectList();
                vm.honorTypeList = StudentHonorTypeController.SelectList();

                var tb = db.Table<Student.Entity.tbStudentHonor>()
                    .Where(d => d.CheckStatus == Code.EnumHelper.CheckStatus.None);

                //如果教师登录，只显示教师所在班级的学生荣誉
                if (Code.Common.UserType == Code.EnumHelper.SysUserType.Teacher)
                {
                    var studentList = Teacher.Controllers.TeacherController.GetStudentForTeacher(Code.Common.UserId);
                    var studentIdList = new List<int>();
                    foreach (var v in studentList)
                    {
                        studentIdList.Add(v.Id);
                    }
                    tb = tb.Where(d => studentIdList.Contains(d.tbStudent.Id));
                }
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.HonorName.Contains(vm.SearchText));
                }
                if (vm.honorLevelId > 0)
                {
                    tb = tb.Where(d => d.tbstudentHonorLevel.Id == vm.honorLevelId);
                }
                if (vm.honorTypeId > 0)
                {
                    tb = tb.Where(d => d.tbStudentHonorType.Id == vm.honorTypeId);
                }

                vm.reViewApplyHonorList = (from p in tb
                                           orderby p.tbStudent.StudentCode
                                           select new Dto.StudentHonor.ReViewApplyHonorList()
                                           {
                                               Id = p.Id,
                                               HonorFile = p.HonorFile,
                                               HonorName = p.HonorName,
                                               StudentCode = p.tbStudent.StudentCode,
                                               Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                               StudentName = p.tbStudent.StudentName,
                                               StudentHonorLevelName = p.tbstudentHonorLevel.StudentHonorLevelName,
                                               StudentHonorTypeName = p.tbStudentHonorType.StudentHonorTypeName,
                                               InputDate = p.InputDate,
                                               CheckRemark = ""
                                           }).ToPageList(vm.Page);

                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult ReViewApplyHonorList(Models.StudentHonor.ReViewApplyHonorList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ReViewApplyHonorList", new
            {
                searchText = vm.SearchText,
                honorLevelId = vm.honorLevelId,
                honorTypeId = vm.honorTypeId
            }));
        }

        public ActionResult ReViewApplyHonorEdit(int id, bool IsReViewYes)
        {
            var vm = new Models.StudentHonor.ReViewApplyHonorEdit();
            vm.edit.Id = id;
            vm.edit.IsReViewYes = IsReViewYes;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReViewApplyHonorEdit(Models.StudentHonor.ReViewApplyHonorEdit vm)
        {
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Student.Entity.tbStudentHonor>().Find(vm.edit.Id);
                tb.CheckRemark = vm.edit.CheckRemark;
                tb.CheckStatus = vm.edit.IsReViewYes ? Code.EnumHelper.CheckStatus.Success : Code.EnumHelper.CheckStatus.Fail;
                db.SaveChanges();
            }
            return Code.MvcHelper.Post(error);
        }

        public ActionResult StudentHonorList(int id)
        {
            var vm = new Models.StudentHonor.List();
            vm.StudentId = id;

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.StudentHonorList = (from p in db.Table<Student.Entity.tbStudentHonor>()
                                       where p.tbStudent.Id == id && p.HonorName.Contains(vm.SearchText)
                                       select new Dto.StudentHonor.List()
                                       {
                                           Id = p.Id,
                                           HonorFile = p.HonorFile,
                                           HonorName = p.HonorName,
                                           StudentHonorLevelName = p.tbstudentHonorLevel.StudentHonorLevelName,
                                           HonorSource = p.HonorSource,
                                           StudentHonorTypeName = p.tbStudentHonorType.StudentHonorTypeName
                                       }).ToList();
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult StudentHonorList(Models.StudentHonor.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StudentHonorList", new
            {
                id = vm.StudentId,
                searchText = vm.SearchText
            }));
        }

        public ActionResult EditStudentHonor(int honorId = 0, int studentId = 0)
        {
            var vm = new Models.StudentHonor.EditStudentHonor();
            vm.StudentId = studentId;

            if (honorId > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.StudentHonorEdit = (from p in db.Table<Student.Entity.tbStudentHonor>()
                                           where p.Id == honorId
                                           select new Dto.StudentHonor.EditStudentHonor()
                                           {
                                               Id = p.Id,
                                               HonorFile = p.HonorFile,
                                               HonorName = p.HonorName,
                                               StudentHonorLevelId = p.tbstudentHonorLevel.Id,
                                               StudentHonorTypeId = p.tbStudentHonorType.Id
                                           }).FirstOrDefault();
                }
            }

            vm.StudentHonorLevelList = StudentHonorLevelController.SelectList(vm.StudentHonorEdit.StudentHonorLevelId);
            vm.StudentHonorTypeList = StudentHonorTypeController.SelectList(vm.StudentHonorEdit.StudentHonorTypeId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudentHonor(Models.StudentHonor.EditStudentHonor vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var fileName = string.Empty;

                var file = Request.Files["StudentHonorEdit.HonorFile"];

                if (!string.IsNullOrEmpty(file.FileName))
                {
                    if (Code.Common.GetFileType(file.FileName) != Code.FileType.Image)
                    {
                        return Content("<script >alert('图片格式必须是jpg、jpeg、png、bmp格式！');history.go(-1);</script >", "text/html");
                    }
                }
                else
                {
                    return Content("<script >alert('荣誉证书不能为空！');history.go(-1);</script >", "text/html");
                }

                var tb = new Student.Entity.tbStudentHonor();
                if (vm.StudentHonorEdit.Id > 0)
                {
                    tb = db.Set<Student.Entity.tbStudentHonor>().Find(vm.StudentHonorEdit.Id);
                    tb.CheckStatus = Code.EnumHelper.CheckStatus.None;
                    tb.HonorName = vm.StudentHonorEdit.HonorName;
                    tb.InputDate = DateTime.Now;
                    tb.tbUserInput = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                    tb.tbstudentHonorLevel = db.Set<Student.Entity.tbStudentHonorLevel>().Find(vm.StudentHonorEdit.StudentHonorLevelId);
                    tb.HonorSource = Code.EnumHelper.StudentHonorSource.Init;
                    tb.tbStudentHonorType = db.Set<Student.Entity.tbStudentHonorType>().Find(vm.StudentHonorEdit.StudentHonorTypeId);
                }
                else
                {
                    tb = new Student.Entity.tbStudentHonor()
                    {
                        CheckStatus = Code.EnumHelper.CheckStatus.None,
                        HonorName = vm.StudentHonorEdit.HonorName,
                        InputDate = DateTime.Now,
                        tbUserInput = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                        tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId),
                        tbstudentHonorLevel = db.Set<Student.Entity.tbStudentHonorLevel>().Find(vm.StudentHonorEdit.StudentHonorLevelId),
                        HonorSource = Code.EnumHelper.StudentHonorSource.Init,
                        tbStudentHonorType = db.Set<Student.Entity.tbStudentHonorType>().Find(vm.StudentHonorEdit.StudentHonorTypeId),
                    };
                    db.Set<Student.Entity.tbStudentHonor>().Add(tb);
                }

                if (!string.IsNullOrEmpty(file.FileName))
                {
                    fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                    var fileSave = Server.MapPath("~/Files/StudentHonor/");
                    file.SaveAs(fileSave + fileName);
                    tb.HonorFile = fileName;
                }

                db.SaveChanges();
                return Content("<script >window.parent.location.reload();</script >", "text/html");
                //return Code.MvcHelper.Post();
                //return Content(Code.Common.Redirect(Url.Action("StudentHonorList", new { id = vm.StudentId })));
            }
        }

        public ActionResult DeleteStudentHonor(List<int> ids)
        {
            int studentId = Request["studentId"].ConvertToInt();
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Student.Entity.tbStudentHonor>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了学生荣誉信息");
                }
            }
            return Code.MvcHelper.Post(null, Url.Action("StudentHonorList", new { id = studentId }));
            //return Content(Code.Common.Redirect(Url.Action("StudentHonorList", new { id = studentId })));
        }
    }
}