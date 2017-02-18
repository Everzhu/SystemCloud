using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace XkSystem.Areas.Quality.Controllers
{
    public class QualityCommentController : Controller
    {
        #region 班主任评语
        public ActionResult ClassList()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Teacher)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityComment.ClassList();

                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Term);
                //获取当前激活学段
                var section = (from p in db.Table<Basis.Entity.tbYear>()
                               .Include(d => d.tbYearParent)
                               where p.IsDeleted == false
                               && p.IsDefault == true
                               && (vm.YearId == 0 ? true : p.tbYearParent.Id == vm.YearId)
                               select p).FirstOrDefault();
                if (vm.YearList.Count > 0 && vm.YearId == 0 && section != null && section.YearType == Code.EnumHelper.YearType.Section)
                {
                    vm.YearId = vm.YearList.Where(d => d.Value == section.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }

                //获取学年
                var yearId = (from p in db.Table<Basis.Entity.tbYear>()
                              where p.Id == vm.YearId
                              select p.tbYearParent.Id).FirstOrDefault();

                //获取教师所在行政班信息
                vm.YClassList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                 where p.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 && p.tbTeacher.IsDeleted == false
                                 && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                  && p.tbClass.tbYear.Id == yearId
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Text = p.tbClass.ClassName,
                                     Value = p.tbClass.Id.ToString(),
                                 }).Distinct().ToList();
                //默认取第一个评教Id去查询后面数据
                if (vm.ClassId == 0 && vm.YClassList.Count > 0)
                {
                    vm.ClassId = vm.YClassList.FirstOrDefault().Value.ConvertToInt();
                }
                else if (vm.YClassList.Count <= 0)
                {
                    vm.ClassId = 0;
                }

                //获取学生所在行政班信息
                var studentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  .Include(d => d.tbStudent)
                                  .Include(d => d.tbClass)
                                   where p.IsDeleted == false
                                  && p.tbClass.IsDeleted == false
                                  && p.tbStudent.IsDeleted == false
                                  && p.tbStudent.tbSysUser.IsDeleted == false
                                  && p.tbClass.Id == vm.ClassId
                                   select p).ToList();

                var studentIdList = studentList.Select(d => d.tbStudent.Id).ToList();

                //班主任评语
                var commentList = (from p in db.Table<Quality.Entity.tbQualityComment>()
                                .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && studentIdList.Contains(p.tbStudent.Id)
                               && p.tbYear.Id == vm.YearId
                               && p.tbSysUser.Id == Code.Common.UserId
                                   select p).ToList();

                vm.QualityCommentList = (from p in studentList
                                         select new Dto.QualityComment.ClassList
                                         {
                                             StudentId = p.tbStudent.Id,
                                             StudentCode = p.tbStudent.StudentCode,
                                             StudentName = p.tbStudent.StudentName,
                                             CommentType = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? 1 : 0),
                                             Comment = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault().Comment : ""),
                                         }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassList(Models.QualityComment.ClassList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassList", new { yearId = vm.YearId, classId = vm.ClassId, }));
        }

        public ActionResult ClassEdit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityComment.ClassEdit();
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && p.Id == vm.StudentId
                               select p).FirstOrDefault();
                if (student != null)
                {
                    vm.StudentCode = student.StudentCode;
                    vm.StudentName = student.StudentName;
                    //班主任评语
                    var tb = (from p in db.Table<Quality.Entity.tbQualityComment>()
                                .Include(d => d.tbStudent)
                              where p.IsDeleted == false
                              && p.tbYear.IsDeleted == false
                              && p.tbStudent.Id == student.Id
                              && p.tbYear.Id == vm.YearId
                              && p.tbSysUser.Id == Code.Common.UserId
                              select new Dto.QualityComment.ClassEdit
                              {
                                  Id = p.Id,
                                  Comment = p.Comment,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.CommentEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassEdit(Models.QualityComment.ClassEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                if (error.Count == decimal.Zero)
                {
                    if (vm.CommentEdit.Id == 0)
                    {
                        var tb = new Quality.Entity.tbQualityComment();
                        tb.No = vm.CommentEdit.No == null ? db.Table<Quality.Entity.tbQualityComment>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.CommentEdit.No;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                        tb.Comment = vm.CommentEdit.Comment;
                        tb.InputDate = DateTime.Now;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Quality.Entity.tbQualityComment>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评语");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Quality.Entity.tbQualityComment>()
                                  where p.Id == vm.CommentEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.CommentEdit.No == null ? db.Table<Quality.Entity.tbQualityComment>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.CommentEdit.No;
                            tb.Comment = vm.CommentEdit.Comment;
                            tb.UpdateTime = DateTime.Now;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评语");
                            }
                        }
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult ImportClassComment()
        {
            var vm = new Models.QualityComment.ImportClassComment();
            return View(vm);
        }
        public ActionResult ImportClassTemplate(int YearId, int ClassId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                #region 准备数据
                //获取学生所在行政班信息
                var studentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  .Include(d => d.tbStudent)
                                  .Include(d => d.tbClass)
                                   where p.IsDeleted == false
                                  && p.tbClass.IsDeleted == false
                                  && p.tbStudent.IsDeleted == false
                                  && p.tbStudent.tbSysUser.IsDeleted == false
                                  && p.tbClass.Id == ClassId
                                   select p).ToList();

                var studentIdList = studentList.Select(d => d.tbStudent.Id).ToList();

                //班主任评语
                var commentList = (from p in db.Table<Quality.Entity.tbQualityComment>()
                                .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && studentIdList.Contains(p.tbStudent.Id)
                               && p.tbYear.Id == YearId
                               && p.tbSysUser.Id == Code.Common.UserId
                                   select p).ToList();

                var QualityCommentList = (from p in studentList
                                          select new Dto.QualityComment.ClassList
                                          {
                                              StudentId = p.tbStudent.Id,
                                              StudentCode = p.tbStudent.StudentCode,
                                              StudentName = p.tbStudent.StudentName,
                                              CommentType = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? 1 : 0),
                                              Comment = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault().Comment : ""),
                                          }).ToList();
                #endregion

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                {
                    new System.Data.DataColumn("学号"),
                    new System.Data.DataColumn("姓名"),
                    new System.Data.DataColumn("评语内容")
                });

                foreach (var a in QualityCommentList)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["评语内容"] = a.Comment;
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);

                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, "ClassCommentTemplate.xlsx");
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportClassComment(Models.QualityComment.ImportClassComment vm)
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
                var tbList = new List<string>() { "学号", "姓名", "评语内容" };

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

                //获取学生所在行政班信息
                var studentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  .Include(d => d.tbStudent)
                                  .Include(d => d.tbClass)
                                   where p.IsDeleted == false
                                  && p.tbClass.IsDeleted == false
                                  && p.tbStudent.IsDeleted == false
                                  && p.tbStudent.tbSysUser.IsDeleted == false
                                  && p.tbClass.Id == vm.ClassId
                                   select p).ToList();

                var studentIdList = studentList.Select(d => d.tbStudent.Id).ToList();
                //获取行政班信息
                var cla = studentList.Select(d => d.tbClass).Distinct().FirstOrDefault();

                #region 将DataTable转为List
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var dtoClass = new Dto.QualityComment.ImportClassComment()
                    {
                        StudentCode = Convert.ToString(dr["学号"]),
                        StudentName = Convert.ToString(dr["姓名"]),
                        Comment = Convert.ToString(dr["评语内容"]),
                    };

                    if (vm.ImportClassCommentList.Where(d => d.StudentCode == dtoClass.StudentCode
                                                    && d.StudentName == dtoClass.StudentName
                                                    && d.Comment == dtoClass.Comment).Count() == 0)
                    {
                        vm.ImportClassCommentList.Add(dtoClass);
                    }
                }
                vm.ImportClassCommentList.RemoveAll(d =>
                    string.IsNullOrEmpty(d.StudentCode) &&
                    string.IsNullOrEmpty(d.StudentName) &&
                    string.IsNullOrEmpty(d.Comment)
                );
                if (vm.ImportClassCommentList.Count == 0)
                {
                    ModelState.AddModelError("", "未读取到任何有效数据!");
                    return View(vm);
                }
                #endregion
                var gradeList = db.Table<Basis.Entity.tbGrade>().ToList();
                var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>().ToList();
                var classTypeList = db.Table<Basis.Entity.tbClassType>().ToList();
                var yearList = db.Table<Basis.Entity.tbYear>().ToList();
                var roomList = db.Table<Basis.Entity.tbRoom>().ToList();
                var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();

                #region 验证数据格式是否正确

                foreach (var item in vm.ImportClassCommentList)
                {
                    if (string.IsNullOrEmpty(item.StudentCode))
                    {
                        item.Error = item.Error + "学号不能为空!";
                    }

                    if (string.IsNullOrEmpty(item.StudentName))
                    {
                        item.Error = item.Error + "姓名不能为空!";
                    }
                    if (studentList.Where(d => d.tbStudent.StudentCode == item.StudentCode && d.tbStudent.StudentName == item.StudentName).Count() <= 0)
                    {
                        item.Error += "学号:" + item.StudentCode + " 姓名:" + item.StudentName + " 学生不存在或不在行政班:" + cla.ClassName + "中!";
                    }
                    if (string.IsNullOrEmpty(item.Comment))
                    {
                        item.Error += "学号:" + item.StudentCode + " 姓名:" + item.StudentName + " 学生的评语内容为空，请填写完整!";
                    }
                    else
                    {
                        //字符长度显示不得超过600字符
                        if (item.Comment.Length > 600)
                        {
                            item.Error += "学号:" + item.StudentCode + " 姓名:" + item.StudentName + " 学生的评语内容超过字符数量限制，最多【600】个字符，请重新填写!";
                        }
                    }
                }

                if (vm.ImportClassCommentList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                {
                    vm.ImportClassCommentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                    return View(vm);
                }
                #endregion

                #region 
                var commentList = (from p in db.Table<Quality.Entity.tbQualityComment>()
                               .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                                   && p.tbYear.IsDeleted == false
                                   && studentIdList.Contains(p.tbStudent.Id)
                                   && p.tbYear.Id == vm.YearId
                                   && p.tbSysUser.Id == Code.Common.UserId
                                   select p).ToList();
                foreach (var item in vm.ImportClassCommentList)
                {
                    if (studentList.Where(d => d.tbStudent.StudentCode == item.StudentCode && d.tbStudent.StudentName == item.StudentName).Count() > 0)
                    {
                        var comment = commentList.Where(d => d.tbStudent.StudentCode == item.StudentCode).FirstOrDefault();
                        if (comment != null)
                        {
                            comment.Comment = item.Comment;
                            comment.UpdateTime = DateTime.Now;
                        }
                        else
                        {
                            var tb = new Quality.Entity.tbQualityComment();
                            tb.No = db.Table<Quality.Entity.tbQualityComment>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1;
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                            tb.tbStudent = studentList.Where(d => d.tbStudent.StudentCode == item.StudentCode && d.tbStudent.StudentName == item.StudentName).Select(d => d.tbStudent).FirstOrDefault();
                            tb.Comment = item.Comment;
                            tb.InputDate = DateTime.Now;
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            db.Set<Quality.Entity.tbQualityComment>().Add(tb);
                        }
                    }
                }
                #endregion
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("评语导入成功");
                    vm.Status = true;
                }
            }

            vm.ImportClassCommentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }
        #endregion

        #region 任课教师评语
        public ActionResult OrgList()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Teacher)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityComment.OrgList();

                //获取学段信息
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                //获取教师所在教学班信息
                vm.YOrgList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                               where p.IsDeleted == false
                               && p.tbOrg.IsDeleted == false
                               && p.tbTeacher.IsDeleted == false
                               && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                && p.tbOrg.tbYear.Id == vm.YearId
                                && p.tbOrg.IsDeleted == false
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.tbOrg.OrgName,
                                   Value = p.tbOrg.Id.ToString(),
                               }).Distinct().ToList();
                //默认取第一个评教Id去查询后面数据
                if (vm.OrgId == 0 && vm.YOrgList.Count > 0)
                {
                    vm.OrgId = vm.YOrgList.FirstOrDefault().Value.ConvertToInt();
                }

                //获取学生所在教学班信息
                var studentList = new List<Student.Entity.tbStudent>();
                studentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                   .Include(d => d.tbStudent)
                                   .Include(d => d.tbOrg)
                               where p.IsDeleted == false
                              && p.tbOrg.IsDeleted == false
                              && p.tbStudent.IsDeleted == false
                              && p.tbStudent.tbSysUser.IsDeleted == false
                              && p.tbOrg.Id == vm.OrgId
                               select p.tbStudent).ToList();

                if (studentList.Count() <= 0)
                {
                    //获取行政班
                    var cla = (from p in db.Table<Course.Entity.tbOrg>()
                                .Include(d => d.tbClass)
                               where p.IsDeleted == false
                               && p.Id == vm.OrgId
                               select p.tbClass).FirstOrDefault();

                    if (cla != null)
                    {
                        studentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                               .Include(d => d.tbStudent.tbSysUser)
                                       where p.IsDeleted == false
                                       && p.tbClass.IsDeleted == false
                                       //&& p.tbClass.tbYear.IsDefault == true
                                       && p.tbStudent.IsDeleted == false
                                       && p.tbClass.Id == cla.Id
                                       select p.tbStudent).ToList();
                    }
                }

                var studentIdList = studentList.Select(d => d.Id).ToList();

                //任课教师评语
                var commentList = (from p in db.Table<Quality.Entity.tbQualityRemark>()
                                .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && studentIdList.Contains(p.tbStudent.Id)
                               && p.tbOrg.tbYear.Id == vm.YearId
                               && p.tbSysUser.Id == Code.Common.UserId
                                   select p).ToList();

                vm.QualityRemarkList = (from p in studentList
                                        select new Dto.QualityComment.OrgList
                                        {
                                            StudentId = p.Id,
                                            StudentCode = p.StudentCode,
                                            StudentName = p.StudentName,
                                            CommentType = (commentList.Where(d => d.tbStudent.Id == p.Id).FirstOrDefault() != null ? 1 : 0),
                                            Comment = (commentList.Where(d => d.tbStudent.Id == p.Id).FirstOrDefault() != null ? commentList.Where(d => d.tbStudent.Id == p.Id).FirstOrDefault().Remark : ""),
                                        }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrgList(Models.QualityComment.OrgList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("OrgList", new { yearId = vm.YearId, orgId = vm.OrgId, }));
        }

        public ActionResult OrgEdit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityComment.OrgEdit();
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && p.Id == vm.StudentId
                               select p).FirstOrDefault();
                if (student != null)
                {
                    vm.StudentCode = student.StudentCode;
                    vm.StudentName = student.StudentName;
                    //任课教师评语
                    var tb = (from p in db.Table<Quality.Entity.tbQualityRemark>()
                                .Include(d => d.tbStudent)
                              where p.IsDeleted == false
                              && p.tbStudent.Id == student.Id
                              && p.tbOrg.tbYear.Id == vm.YearId
                              && p.tbOrg.IsDeleted == false
                              && p.tbSysUser.Id == Code.Common.UserId
                              select new Dto.QualityComment.OrgEdit
                              {
                                  Id = p.Id,
                                  Remark = p.Remark,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.RemarkEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrgEdit(Models.QualityComment.OrgEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                if (error.Count == decimal.Zero)
                {
                    if (vm.RemarkEdit.Id == 0)
                    {
                        var tb = new Quality.Entity.tbQualityRemark();
                        tb.No = vm.RemarkEdit.No == null ? db.Table<Quality.Entity.tbQualityRemark>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.RemarkEdit.No;
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                        tb.tbOrg = db.Set<Course.Entity.tbOrg>().Find(vm.OrgId);
                        tb.Remark = vm.RemarkEdit.Remark;
                        tb.InputDate = DateTime.Now;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Quality.Entity.tbQualityRemark>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评语");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Quality.Entity.tbQualityRemark>()
                                  where p.Id == vm.RemarkEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.RemarkEdit.No == null ? db.Table<Quality.Entity.tbQualityRemark>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.RemarkEdit.No;
                            tb.Remark = vm.RemarkEdit.Remark;
                            tb.UpdateTime = DateTime.Now;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评语");
                            }
                        }
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult ImportOrgComment()
        {
            var vm = new Models.QualityComment.ImportOrgComment();
            return View(vm);
        }

        public ActionResult ImportOrgTemplate()
        {
            var file = Server.MapPath("~/Areas/Quality/Views/QualityComment/OrgCommentTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportOrgComment(Models.QualityComment.ImportOrgComment vm)
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
                var tbList = new List<string>() { "学号", "姓名", "评语内容" };

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

                //获取学生所在教学班信息
                var studentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                    .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                                  && p.tbOrg.IsDeleted == false
                                  && p.tbStudent.IsDeleted == false
                                  && p.tbStudent.tbSysUser.IsDeleted == false
                                  && p.tbOrg.Id == vm.OrgId
                                   select new Dto.QualityComment.ImportOrgComment
                                   {
                                       OrgId = p.tbOrg.Id,
                                       StudentId = p.tbStudent.Id,
                                       StudentCode = p.tbStudent.StudentCode,
                                       StudentName = p.tbStudent.StudentName,
                                   }).ToList();

                if (studentList.Count() <= 0)
                {
                    //获取教学班
                    var cla = (from p in db.Table<Course.Entity.tbOrg>()
                                .Include(d => d.tbClass)
                               where p.IsDeleted == false
                               && p.Id == vm.OrgId
                               select p).FirstOrDefault();

                    if (cla != null)
                    {
                        studentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                               .Include(d => d.tbStudent.tbSysUser)
                                       where p.IsDeleted == false
                                       && p.tbClass.IsDeleted == false
                                       && p.tbStudent.IsDeleted == false
                                       && p.tbClass.Id == cla.tbClass.Id
                                       select new Dto.QualityComment.ImportOrgComment
                                       {
                                           OrgId = cla.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName,
                                       }).ToList();
                    }
                }

                var studentIdList = studentList.Select(d => d.StudentId).ToList();
                //获取教学班信息
                var org = (from p in db.Table<Course.Entity.tbOrg>()
                           where p.IsDeleted == false
                           && p.Id == vm.OrgId
                           select p).FirstOrDefault();

                #region 将DataTable转为List
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var dtoClass = new Dto.QualityComment.ImportOrgComment()
                    {
                        StudentCode = Convert.ToString(dr["学号"]),
                        StudentName = Convert.ToString(dr["姓名"]),
                        Comment = Convert.ToString(dr["评语内容"]),
                    };

                    if (vm.ImportOrgCommentList.Where(d => d.StudentCode == dtoClass.StudentCode
                                                    && d.StudentName == dtoClass.StudentName
                                                    && d.Comment == dtoClass.Comment).Count() == 0)
                    {
                        vm.ImportOrgCommentList.Add(dtoClass);
                    }
                }
                vm.ImportOrgCommentList.RemoveAll(d =>
                    string.IsNullOrEmpty(d.StudentCode) &&
                    string.IsNullOrEmpty(d.StudentName) &&
                    string.IsNullOrEmpty(d.Comment)
                );
                if (vm.ImportOrgCommentList.Count == 0)
                {
                    ModelState.AddModelError("", "未读取到任何有效数据!");
                    return View(vm);
                }
                #endregion
                var gradeList = db.Table<Basis.Entity.tbGrade>().ToList();
                var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>().ToList();
                var classTypeList = db.Table<Basis.Entity.tbClassType>().ToList();
                var yearList = db.Table<Basis.Entity.tbYear>().ToList();
                var roomList = db.Table<Basis.Entity.tbRoom>().ToList();
                var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();

                #region 验证数据格式是否正确

                foreach (var item in vm.ImportOrgCommentList)
                {
                    if (string.IsNullOrEmpty(item.StudentCode))
                    {
                        item.Error = item.Error + "学号不能为空!";
                    }

                    if (string.IsNullOrEmpty(item.StudentName))
                    {
                        item.Error = item.Error + "姓名不能为空!";
                    }
                    if (studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Count() <= 0)
                    {
                        item.Error += "学号:" + item.StudentCode + " 姓名:" + item.StudentName + " 学生不存在或不在教学班:" + org.OrgName + "中!";
                    }

                }

                if (vm.ImportOrgCommentList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                {
                    vm.ImportOrgCommentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                    return View(vm);
                }
                #endregion

                #region 
                var commentList = (from p in db.Table<Quality.Entity.tbQualityRemark>()
                               .Include(d => d.tbStudent)
                               .Include(d => d.tbOrg)
                                   where p.IsDeleted == false
                                   && studentIdList.Contains(p.tbStudent.Id)
                                   && p.tbOrg.tbYear.Id == vm.YearId
                                   && p.tbOrg.IsDeleted == false
                                   && p.tbSysUser.Id == Code.Common.UserId
                                   && p.tbOrg.Id == vm.OrgId
                                   select p).ToList();
                foreach (var item in vm.ImportOrgCommentList)
                {
                    if (studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Count() > 0)
                    {
                        var comment = commentList.Where(d => d.tbStudent.StudentCode == item.StudentCode && d.tbOrg.Id == vm.OrgId).FirstOrDefault();
                        if (comment != null)
                        {
                            comment.Remark = item.Comment;
                            comment.UpdateTime = DateTime.Now;
                        }
                        else
                        {
                            var tb = new Quality.Entity.tbQualityRemark();
                            tb.No = db.Table<Quality.Entity.tbQualityRemark>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1;
                            tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Select(d => d.StudentId).FirstOrDefault());
                            tb.tbOrg = db.Set<Course.Entity.tbOrg>().Find(vm.OrgId);
                            tb.Remark = item.Comment;
                            tb.InputDate = DateTime.Now;
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            db.Set<Quality.Entity.tbQualityRemark>().Add(tb);
                        }
                    }
                }
                #endregion
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("评语导入成功");
                    vm.Status = true;
                }
            }

            vm.ImportOrgCommentList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }
        #endregion

        #region 家长评语
        public ActionResult FamilyList()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Family)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityComment.FamilyList();

                //获取学期信息
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Term);
                //获取当前激活学段
                var section = (from p in db.Table<Basis.Entity.tbYear>()
                               .Include(d => d.tbYearParent)
                               where p.IsDeleted == false
                               && p.IsDefault == true
                               && (vm.YearId == 0 ? true : p.tbYearParent.Id == vm.YearId)
                               select p).FirstOrDefault();
                if (vm.YearList.Count > 0 && vm.YearId == 0 && section != null && section.YearType == Code.EnumHelper.YearType.Section)
                {
                    vm.YearId = vm.YearList.Where(d => d.Value == section.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }

                vm.YearDefault = section != null ? true : false;

                //根据家长获取学生信息
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               .Include(d => d.tbSysUser)
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && (p.tbSysUser.Id == Code.Common.UserId || p.tbSysUserFamily.Id == Code.Common.UserId)
                               select p).FirstOrDefault();
                if (student != null)
                {
                    //家长评语
                    var commentList = (from p in db.Table<Quality.Entity.tbQualityFamily>()
                                    .Include(d => d.tbStudent)
                                       where p.IsDeleted == false
                                   && p.tbYear.IsDeleted == false
                                   && p.tbStudent.Id == student.Id
                                   && p.tbYear.Id == vm.YearId
                                   && p.tbSysUser.Id == Code.Common.UserId
                                       select p).ToList();

                    var qualityFamily = new Dto.QualityComment.FamilyList();
                    qualityFamily.StudentId = student.Id;
                    qualityFamily.StudentCode = student.StudentCode;
                    qualityFamily.StudentName = student.StudentName;
                    qualityFamily.CommentType = (commentList.Where(d => d.tbStudent.Id == student.Id).FirstOrDefault() != null ? 1 : 0);
                    qualityFamily.Comment = (commentList.Where(d => d.tbStudent.Id == student.Id).FirstOrDefault() != null ? commentList.Where(d => d.tbStudent.Id == student.Id).FirstOrDefault().Content : "");
                    vm.QualityFamilyList.Add(qualityFamily);
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FamilyList(Models.QualityComment.FamilyList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("FamilyList", new { yearId = vm.YearId, }));
        }

        public ActionResult FamilyEdit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityComment.FamilyEdit();
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && p.Id == vm.StudentId
                               select p).FirstOrDefault();
                if (student != null)
                {
                    vm.StudentCode = student.StudentCode;
                    vm.StudentName = student.StudentName;
                    //家长评语
                    var tb = (from p in db.Table<Quality.Entity.tbQualityFamily>()
                                .Include(d => d.tbStudent)
                              where p.IsDeleted == false
                              && p.tbYear.IsDeleted == false
                              && p.tbStudent.Id == student.Id
                              && p.tbYear.Id == vm.YearId
                              && p.tbSysUser.Id == Code.Common.UserId
                              select new Dto.QualityComment.FamilyEdit
                              {
                                  Id = p.Id,
                                  Comment = p.Content,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.QualityFamilyEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FamilyEdit(Models.QualityComment.FamilyEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                if (error.Count == decimal.Zero)
                {
                    if (vm.QualityFamilyEdit.Id == 0)
                    {
                        var tb = new Quality.Entity.tbQualityFamily();
                        tb.No = vm.QualityFamilyEdit.No == null ? db.Table<Quality.Entity.tbQualityFamily>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualityFamilyEdit.No;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                        tb.Content = vm.QualityFamilyEdit.Comment;
                        tb.InputDate = DateTime.Now;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Quality.Entity.tbQualityFamily>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评语");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Quality.Entity.tbQualityFamily>()
                                  where p.Id == vm.QualityFamilyEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.QualityFamilyEdit.No == null ? db.Table<Quality.Entity.tbQualityFamily>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualityFamilyEdit.No;
                            tb.Content = vm.QualityFamilyEdit.Comment;
                            tb.UpdateTime = DateTime.Now;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评语");
                            }
                        }
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }
        #endregion

        #region 孩子评语
        public ActionResult ChildComment()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Family)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityComment.ChildComment();

                //获取学期信息
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Term);
                //获取当前激活学段
                var section = (from p in db.Table<Basis.Entity.tbYear>()
                               .Include(d => d.tbYearParent)
                               where p.IsDeleted == false
                               && p.IsDefault == true
                               && (vm.YearId == 0 ? true : p.tbYearParent.Id == vm.YearId)
                               select p).FirstOrDefault();
                if (vm.YearList.Count > 0 && vm.YearId == 0 && section != null && section.YearType == Code.EnumHelper.YearType.Section)
                {
                    vm.YearId = vm.YearList.Where(d => d.Value == section.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }

                //根据家长获取学生信息
                vm.Student = (from p in db.Table<Student.Entity.tbStudent>()
                               .Include(d => d.tbSysUser)
                              where p.IsDeleted == false
                              && p.tbSysUser.IsDeleted == false
                              && (p.tbSysUser.Id == Code.Common.UserId || p.tbSysUserFamily.Id == Code.Common.UserId)
                              select p).FirstOrDefault();
                if (vm.Student != null)
                {
                    //自评
                    vm.QualitySelf = (from p in db.Table<Quality.Entity.tbQualitySelf>()
                                .Include(d => d.tbStudent)
                                      where p.IsDeleted == false
                                  && p.tbYear.IsDeleted == false
                                  && p.tbStudent.Id == vm.Student.Id
                                  && p.tbYear.Id == vm.YearId
                                      select p).FirstOrDefault();

                    //学期期待
                    vm.QualityPlan = (from p in db.Table<Quality.Entity.tbQualityPlan>()
                                    .Include(d => d.tbStudent)
                                      where p.IsDeleted == false
                                  && p.tbYear.IsDeleted == false
                                   && p.tbStudent.Id == vm.Student.Id
                                  && p.tbYear.Id == vm.YearId
                                      select p).FirstOrDefault();
                    //学期总结
                    vm.QualitySummary = (from p in db.Table<Quality.Entity.tbQualitySummary>()
                                       .Include(d => d.tbStudent)
                                         where p.IsDeleted == false
                                         && p.tbYear.IsDeleted == false
                                         && p.tbStudent.Id == vm.Student.Id
                                         && p.tbYear.Id == vm.YearId
                                         select p).FirstOrDefault();
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChildComment(Models.QualityComment.ClassList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ChildComment", new { yearId = vm.YearId, }));
        }
        #endregion
    }
}