using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Data;

namespace XkSystem.Areas.Course.Controllers
{
    public class OrgStudentController : Controller
    {
        public ActionResult List(int orgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgStudent.List();
                vm.OrgId = orgId;
                var org = (from p in db.Table<Course.Entity.tbOrg>()
                            .Include(d => d.tbClass)
                            .Include(d => d.tbYear)
                            .Include(d => d.tbYear.tbYearParent)
                            .Include(d => d.tbYear.tbYearParent.tbYearParent)
                           where p.Id == orgId
                           select p).FirstOrDefault();
                if (org != null)
                {
                    vm.IsClass = org.IsClass;
                    vm.OrgName = org.OrgName;

                    if (org.IsClass)
                    {
                        if (org.tbClass != null)
                        {
                            var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                                     where p.tbClass.Id == org.tbClass.Id && p.tbStudent.IsDeleted == false
                                     select p;
                            if (string.IsNullOrEmpty(vm.SearchText) == false)
                            {
                                tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                            }

                            vm.OrgStudentList = (from p in tb
                                                 orderby p.No, p.tbStudent.StudentCode
                                                 select new Dto.OrgStudent.List
                                                 {
                                                     Id = p.Id,
                                                     No = p.No,
                                                     StudentName = p.tbStudent.StudentName,
                                                     StudentCode = p.tbStudent.StudentCode,
                                                     SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                                     ClassName = p.tbClass.ClassName
                                                 }).ToList();
                            var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                                    where p.tbTeacher.IsDeleted == false
                                                        && p.tbClass.Id == org.tbClass.Id
                                                    select new
                                                    {
                                                        ClassId = p.tbClass.Id,
                                                        TeacherName = p.tbTeacher.TeacherName
                                                    }).ToList();
                            foreach (var a in vm.OrgStudentList)
                            {
                                a.TeacherName = string.Join(",", classTeacherList.Select(d => d.TeacherName));
                            }
                        }
                    }
                    else
                    {
                        var tb = from p in db.Table<Course.Entity.tbOrgStudent>()
                                 where p.tbOrg.Id == orgId
                                 select p;

                        if (string.IsNullOrEmpty(vm.SearchText) == false)
                        {
                            tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                        }

                        vm.OrgStudentList = (from p in tb
                                             orderby p.No, p.tbStudent.StudentCode
                                             select new Dto.OrgStudent.List
                                             {
                                                 Id = p.Id,
                                                 OrgName = p.tbOrg.OrgName,
                                                 No = p.No,
                                                 StudentId = p.tbStudent.Id,
                                                 StudentName = p.tbStudent.StudentName,
                                                 StudentCode = p.tbStudent.StudentCode,
                                                 SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                                 ClassName = "",
                                                 TeacherName = "",
                                             }).ToList();

                        var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                                join q in db.Table<Basis.Entity.tbClassStudent>()
                                                on p.tbClass.Id equals q.tbClass.Id
                                                where p.tbTeacher.IsDeleted == false
                                                    && p.tbClass.tbYear.Id == org.tbYear.tbYearParent.tbYearParent.Id
                                                select new
                                                {
                                                    StudentId = q.tbStudent.Id,
                                                    ClassName = q.tbClass.ClassName,
                                                    TeacherName = p.tbTeacher.TeacherName
                                                }).ToList();

                        foreach (var a in vm.OrgStudentList)
                        {
                            a.ClassName = string.Join(",", classTeacherList.Where(d => d.StudentId == a.StudentId).Select(d => d.ClassName));
                            a.TeacherName = string.Join(",", classTeacherList.Where(d => d.StudentId == a.StudentId).Select(d => d.TeacherName));
                        }
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.OrgStudent.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { orgId = vm.OrgId, searchText = vm.SearchText }));
        }

        public ActionResult ListByOrg(int orgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgStudent.List();
                vm.OrgId = orgId;
                var org = (from p in db.Table<Course.Entity.tbOrg>()
                            .Include(d => d.tbClass)
                            .Include(d => d.tbYear)
                           where p.Id == orgId
                           select p).FirstOrDefault();
                if (org != null)
                {
                    vm.IsClass = org.IsClass;
                    vm.OrgName = org.OrgName;

                    if (org.IsClass)
                    {
                        if (org.tbClass != null)
                        {
                            var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                                     where p.tbClass.Id == org.tbClass.Id && p.tbStudent.IsDeleted == false
                                     select p;
                            if (string.IsNullOrEmpty(vm.SearchText) == false)
                            {
                                tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                            }

                            vm.OrgStudentList = (from p in tb
                                                 orderby p.No, p.tbStudent.StudentCode
                                                 select new Dto.OrgStudent.List
                                                 {
                                                     Id = p.Id,
                                                     No = p.No,
                                                     StudentName = p.tbStudent.StudentName,
                                                     StudentCode = p.tbStudent.StudentCode,
                                                     SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                                     ClassName = p.tbClass.ClassName
                                                 }).ToList();
                            var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                                    where p.tbTeacher.IsDeleted == false
                                                        && p.tbClass.Id == org.tbClass.Id
                                                    select new
                                                    {
                                                        ClassId = p.tbClass.Id,
                                                        TeacherName = p.tbTeacher.TeacherName
                                                    }).ToList();
                            foreach (var a in vm.OrgStudentList)
                            {
                                a.TeacherName = string.Join(",", classTeacherList.Select(d => d.TeacherName));
                            }
                        }
                    }
                    else
                    {
                        var tb = from p in db.Table<Course.Entity.tbOrgStudent>()
                                 where p.tbOrg.Id == orgId
                                 select p;

                        if (string.IsNullOrEmpty(vm.SearchText) == false)
                        {
                            tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                        }

                        vm.OrgStudentList = (from p in tb
                                             orderby p.No, p.tbStudent.StudentCode
                                             select new Dto.OrgStudent.List
                                             {
                                                 Id = p.Id,
                                                 OrgName = p.tbOrg.OrgName,
                                                 No = p.No,
                                                 StudentId = p.tbStudent.Id,
                                                 StudentName = p.tbStudent.StudentName,
                                                 StudentCode = p.tbStudent.StudentCode,
                                                 SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                                 ClassName = p.tbOrg.tbClass.ClassName,
                                                 TeacherName = "",
                                             }).ToList();
                        var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                                join q in db.Table<Basis.Entity.tbClassStudent>()
                                                on p.tbClass.Id equals q.tbClass.Id
                                                where p.tbTeacher.IsDeleted == false
                                                    && p.tbClass.tbYear.Id == org.tbYear.Id
                                                select new
                                                {
                                                    StudentId = q.tbStudent.Id,
                                                    TeacherName = p.tbTeacher.TeacherName
                                                }).ToList();
                        foreach (var a in vm.OrgStudentList)
                        {
                            a.TeacherName = string.Join(",", classTeacherList.Where(d => d.StudentId == a.StudentId).Select(d => d.TeacherName));
                        }
                    }
                }

                return View(vm);
            }
        }

        public ActionResult ExportListByOrg(int orgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgStudent.ExportListByOrg();
                var org = (from p in db.Table<Course.Entity.tbOrg>()
                           .Include(d => d.tbClass)
                           .Include(d => d.tbYear)
                           where p.Id == orgId
                           select p).FirstOrDefault();

                if (org != null)
                {
                    if (org.IsClass)
                    {
                        if (org.tbClass != null)
                        {
                            vm.DataList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                           where p.tbClass.Id == org.tbClass.Id
                                           && p.tbStudent.IsDeleted == false
                                           orderby p.No, p.tbStudent.StudentCode
                                           select new Dto.OrgStudent.ExportListByOrg
                                           {
                                               No = p.No,
                                               StudentName = p.tbStudent.StudentName,
                                               StudentCode = p.tbStudent.StudentCode,
                                               SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                               ClassName = p.tbClass.ClassName
                                           }).ToList();

                            var classTeacher = db.Table<Basis.Entity.tbClassTeacher>()
                                .Include(d => d.tbTeacher).Include(d => d.tbClass)
                                .Where(d => d.tbClass.Id == org.tbClass.Id).FirstOrDefault();

                            foreach (var a in vm.DataList)
                            {
                                a.TeacherName = classTeacher.tbTeacher.TeacherName;
                            }
                        }
                    }
                    else
                    {
                        vm.DataList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                       where p.tbOrg.Id == orgId
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Dto.OrgStudent.ExportListByOrg
                                       {
                                           No = p.No,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                           ClassName = p.tbOrg.tbClass.ClassName
                                       }).ToList();
                    }
                }

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("座位号"),
                        new System.Data.DataColumn("行政班"),
                        new System.Data.DataColumn("班主任")
                    });
                foreach (var v in vm.DataList)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = v.StudentCode;
                    dr["姓名"] = v.StudentName;
                    dr["性别"] = v.SexName;
                    dr["座位号"] = v.No;
                    dr["行政班"] = v.ClassName;
                    dr["班主任"] = v.TeacherName;
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

        public ActionResult OrgStudentList()
        {
            var vm = new Models.OrgStudent.OrgStudentList();
            vm.GradeList = Basis.Controllers.GradeController.SelectList();
            vm.SubjectList = SubjectController.SelectList();
            vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Course.Entity.tbOrgStudent>();
                var orgTeacherList = db.Table<Course.Entity.tbOrgTeacher>()
                    .Include(d => d.tbOrg).Include(d => d.tbTeacher).ToList();

                if (vm.GradeId > 0)
                {
                    tb = tb.Where(d => d.tbOrg.tbGrade.Id == vm.GradeId);
                }
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }
                if (vm.SubjectId > 0)
                {
                    tb = tb.Where(d => d.tbOrg.tbCourse.tbSubject.Id == vm.SubjectId);
                }
                if (vm.YearId > 0)
                {
                    tb = tb.Where(d => d.tbOrg.tbYear.Id == vm.YearId);
                }

                vm.List = (from p in tb
                           orderby p.tbStudent.StudentCode
                           select new Dto.OrgStudent.OrgStudentList()
                           {
                               Id = p.Id,
                               No = p.No,
                               IsClass = p.tbOrg.IsClass,
                               ClassName = p.tbOrg.IsClass ? p.tbOrg.tbClass.ClassName : "",
                               OrgName = p.tbOrg.OrgName,
                               OrgId = p.tbOrg.Id,
                               SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                               StudentCode = p.tbStudent.StudentCode,
                               StudentName = p.tbStudent.StudentName
                           }).ToPageList(vm.Page);
                if (orgTeacherList.Count > 0)
                {
                    foreach (var v in vm.List)
                    {
                        v.TeacherName = orgTeacherList.Where(d => d.tbOrg.OrgName == v.OrgName).Select(d => d.tbTeacher.TeacherName).FirstOrDefault();
                    }
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrgStudentList(Models.OrgStudent.OrgStudentList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("OrgStudentList", new
            {
                yearId = vm.YearId,
                GradeId = vm.GradeId,
                SubjectId = vm.SubjectId,
                searchText = vm.SearchText,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrgStudent>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了教学班学生");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgStudent.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Course.Entity.tbOrgStudent>()
                              where p.Id == id
                              select new Dto.OrgStudent.Edit
                              {
                                  Id = p.Id,
                                  StudentCode = p.tbStudent.StudentCode + "(" + p.tbStudent.StudentName + ")",
                                  OrgId = p.tbOrg.Id,
                                  No = p.No,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.OrgStudentEdit = tb;
                    }
                }

                vm.OrgList = OrgController.SelectList(vm.OrgStudentEdit.OrgId);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.OrgStudent.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var studentCode = vm.OrgStudentEdit.StudentCode.Split('(')[0];
                    if (db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == studentCode).Count() == 0)
                    {
                        error.Add("学生不存在!");
                        return Code.MvcHelper.Post(error);
                    }
                    if (db.Table<Course.Entity.tbOrgStudent>().Where(d => d.tbStudent.StudentCode == studentCode).Count() > 0)
                    {
                        error.Add("学生已分班!");
                        return Code.MvcHelper.Post(error);
                    }

                    if (vm.OrgStudentEdit.Id > 0)
                    {
                        var tb = db.Set<Course.Entity.tbOrgStudent>().Find(vm.OrgStudentEdit.Id);
                        tb.No = vm.OrgStudentEdit.No;
                        tb.tbOrg = db.Set<Course.Entity.tbOrg>().Find(vm.OrgStudentEdit.OrgId);
                        tb.tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == studentCode).FirstOrDefault();
                    }
                    else
                    {
                        var tb = new Course.Entity.tbOrgStudent()
                        {
                            No = vm.OrgStudentEdit.No,
                            tbOrg = db.Set<Course.Entity.tbOrg>().Find(vm.OrgStudentEdit.OrgId),
                            tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == studentCode).FirstOrDefault()
                        };
                        db.Set<Course.Entity.tbOrgStudent>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改/添加了教学班学生");
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult SetSeatNo(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgStudent.Edit();

                vm.OrgStudentEdit = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                     where p.Id == id
                                     select new Dto.OrgStudent.Edit
                                     {
                                         Id = p.Id,
                                         OrgId = p.tbOrg.Id,
                                         No = p.No,
                                     }).FirstOrDefault();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetSeatNo(Models.OrgStudent.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.OrgStudentEdit.No > 0 == false)
                    {
                        error.Add("座位号必须为正整数！");
                        return Code.MvcHelper.Post(error);
                    }
                    if (db.Table<Entity.tbOrgStudent>().Where(d => d.tbOrg.Id == vm.OrgStudentEdit.OrgId && d.No == vm.OrgStudentEdit.No).Any())
                    {
                        error.Add("座位号重复！");
                        return Code.MvcHelper.Post(error);
                    }
                    var tb = db.Set<Course.Entity.tbOrgStudent>().Find(vm.OrgStudentEdit.Id);
                    tb.No = vm.OrgStudentEdit.No;
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改/添加了教学班学生座位号");
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(List<int> ids, int orgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgStudent.Edit();

                var classStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                        where p.tbOrg.Id == orgId
                                        select p.tbStudent.Id).ToList();
                var maxNo = db.Table<Course.Entity.tbOrgStudent>().Where(d => d.tbOrg.Id == orgId).Select(d => d.No).DefaultIfEmpty(0).Max();
                var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                   where ids.Contains(p.Id) && classStudentList.Contains(p.Id) == false
                                   select p).ToList();
                foreach (var student in studentList)
                {
                    maxNo = maxNo + 1;

                    var tb = new Course.Entity.tbOrgStudent();
                    tb.tbOrg = db.Set<Course.Entity.tbOrg>().Find(orgId);
                    tb.tbStudent = student;
                    tb.No = maxNo;
                    db.Set<Course.Entity.tbOrgStudent>().Add(tb);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了教学班学生");
                }

                return Json(new { Status = decimal.One, Message = "操作成功！" });
            }
        }

        public ActionResult Import(int orgId)
        {
            var vm = new Models.OrgStudent.Import();
            vm.OrgId = orgId;
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Course/Views/OrgStudent/OrgStudentTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.OrgStudent.Import vm)
        {
            if (ModelState.IsValid)
            {
                vm.ImportList = new List<Dto.OrgStudent.Import>();
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
                    var tbList = new List<string>() { "座位号", "学生姓名", "学生学号" };

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
                        ModelState.AddModelError("", "上传的教学班内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }
                    var org = (from p in db.Table<Course.Entity.tbOrg>()
                               where p.Id == vm.OrgId
                               select p).FirstOrDefault();
                    var studentList = db.Table<Student.Entity.tbStudent>().ToList();
                    var orgStudentList = db.Table<Course.Entity.tbOrgStudent>().Where(d => d.tbOrg.Id == vm.OrgId)
                                            .Include(d => d.tbStudent).ToList();

                    foreach (DataRow dr in dt.Rows)
                    {
                        var import = new Dto.OrgStudent.Import()
                        {
                            No = dr["座位号"].ToString().Trim(),
                            StudentCode = dr["学生学号"].ToString().Trim(),
                            StudentName = dr["学生姓名"].ToString().Trim()
                        };
                        vm.ImportList.Add(import);
                    }

                    //验证
                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.No))
                        {
                            item.Error += "座位号不能为空！";
                        }
                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error += "学生学号不能为空！";
                        }
                        if (string.IsNullOrEmpty(item.StudentName))
                        {
                            item.Error += "学生姓名不能为空！";
                        }
                        if (studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Count() == 0)
                        {
                            item.Error += "学生不存在！";
                        }
                        if (vm.ImportList.Where(d => d.StudentCode == item.StudentCode).Count() > 1)
                        {
                            item.Error += "学生重复！";
                        }
                        int no = 0;
                        if (!int.TryParse(item.No, out no))
                        {
                            item.Error += "座位号必须为正整数！";
                        }
                    }
                    if (vm.ImportList.Where(d => !string.IsNullOrEmpty(d.Error)).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }

                    foreach (var v in vm.ImportList)
                    {
                        if (orgStudentList.Where(d => d.tbStudent.StudentCode == v.StudentCode).Count() > 0)
                        {
                            v.Error += "本教学班中已经存在此学生信息！";
                            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                            return View(vm);
                        }
                        var tb = new Course.Entity.tbOrgStudent();
                        tb.tbOrg = org;
                        tb.tbStudent = studentList.Where(d => d.StudentName == v.StudentName && d.StudentCode == v.StudentCode).FirstOrDefault();
                        db.Set<Course.Entity.tbOrgStudent>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入了教学班学生");
                        vm.Status = true;
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                    }
                }
            }
            return View(vm);
        }

        public ActionResult Export(int orgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var org = (from p in db.Table<Course.Entity.tbOrg>()
                           where p.Id == orgId
                           select new
                           {
                               p.IsClass,
                               ClassId = p.IsClass ? p.tbClass.Id : 0,
                               OrgId = p.Id,
                               p.OrgName
                           }).FirstOrDefault();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("教学班"),
                         new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("座位号"),
                    });
                if (org != null && org.IsClass == true)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbClassStudent>()
                              where p.tbClass.Id == org.ClassId
                                && p.tbStudent.IsDeleted == false
                              select new
                              {
                                  p.tbStudent.StudentCode,
                                  p.tbStudent.StudentName,
                                  p.tbStudent.tbSysUser.tbSex.SexName,
                                  p.No
                              }).ToList();
                    foreach (var a in tb)
                    {
                        var dr = dt.NewRow();
                        dr["教学班"] = org.OrgName;
                        dr["学号"] = a.StudentCode;
                        dr["姓名"] = a.StudentName;
                        dr["性别"] = a.SexName;
                        dr["座位号"] = a.No;
                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    var classList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                     where p.tbStudent.IsDeleted == false
                                     && p.tbOrg.Id == org.OrgId
                                     select new
                                     {
                                         p.tbStudent.StudentCode,
                                         p.tbStudent.StudentName,
                                         p.tbStudent.tbSysUser.tbSex.SexName,
                                         p.No
                                     }).ToList();
                    foreach (var a in classList)
                    {
                        var dr = dt.NewRow();
                        dr["教学班"] = org.OrgName;
                        dr["学号"] = a.StudentCode;
                        dr["姓名"] = a.StudentName;
                        dr["性别"] = a.SexName;
                        dr["座位号"] = a.No;
                        dt.Rows.Add(dr);
                    }
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

        public ActionResult ListByTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgStudent.ListByTeacher();
                vm.OrgList = OrgTeacherController.GetOrgListByOrgTeacher();
                if (vm.OrgId == 0 && vm.OrgList.Count > 0)
                {
                    vm.OrgId = vm.OrgList.FirstOrDefault().Value.ConvertToInt();
                }

                var org = (from p in db.Table<Course.Entity.tbOrg>()
                            .Include(d => d.tbClass)
                            .Include(d => d.tbYear)
                           where p.Id == vm.OrgId
                           select p).FirstOrDefault();
                if (org != null)
                {
                    if (org.IsClass)
                    {
                        if (org.tbClass != null)
                        {
                            var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                                     where p.tbClass.Id == org.tbClass.Id && p.tbStudent.IsDeleted == false
                                     select p;
                            if (string.IsNullOrEmpty(vm.SearchText) == false)
                            {
                                tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                            }

                            vm.OrgStudentList = (from p in tb
                                                 orderby p.No, p.tbStudent.StudentCode
                                                 select new Dto.OrgStudent.List
                                                 {
                                                     Id = p.Id,
                                                     No = p.No,
                                                     StudentName = p.tbStudent.StudentName,
                                                     StudentCode = p.tbStudent.StudentCode,
                                                     SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                                     ClassName = p.tbClass.ClassName,
                                                 }).ToList();
                            var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                                    where p.tbTeacher.IsDeleted == false
                                                        && p.tbClass.Id == org.tbClass.Id
                                                    select new
                                                    {
                                                        ClassId = p.tbClass.Id,
                                                        TeacherName = p.tbTeacher.TeacherName
                                                    }).ToList();
                            foreach (var a in vm.OrgStudentList)
                            {
                                a.TeacherName = string.Join(",", classTeacherList.Select(d => d.TeacherName));
                            }
                        }
                    }
                    else
                    {
                        var tb = from p in db.Table<Course.Entity.tbOrgStudent>()
                                 where p.tbOrg.Id == vm.OrgId
                                 select p;

                        if (string.IsNullOrEmpty(vm.SearchText) == false)
                        {
                            tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                        }

                        vm.OrgStudentList = (from p in tb
                                             orderby p.No, p.tbStudent.StudentCode
                                             select new Dto.OrgStudent.List
                                             {
                                                 Id = p.Id,
                                                 OrgName = p.tbOrg.OrgName,
                                                 No = p.No,
                                                 StudentId = p.tbStudent.Id,
                                                 StudentName = p.tbStudent.StudentName,
                                                 StudentCode = p.tbStudent.StudentCode,
                                                 SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                                 ClassName = p.tbOrg.tbClass.ClassName,
                                                 TeacherName = "",
                                             }).ToList();
                        var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                                join q in db.Table<Basis.Entity.tbClassStudent>()
                                                on p.tbClass.Id equals q.tbClass.Id
                                                where p.tbTeacher.IsDeleted == false
                                                    && p.tbClass.tbYear.Id == org.tbYear.Id
                                                select new
                                                {
                                                    StudentId = q.tbStudent.Id,
                                                    TeacherName = p.tbTeacher.TeacherName
                                                }).ToList();
                        foreach (var a in vm.OrgStudentList)
                        {
                            a.TeacherName = string.Join(",", classTeacherList.Where(d => d.StudentId == a.StudentId).Select(d => d.TeacherName));
                        }
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListByTeacher(Models.OrgStudent.ListByTeacher vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ListByTeacher", new { orgId = vm.OrgId, searchText = vm.SearchText }));
        }

        public ActionResult SelectOrgStudent()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgStudent.SelectOrgStudent();
                return View(vm);
            }
        }

        public ActionResult GetOrgStudentTree()
        {
            using (var db = new XkSystem.Models.DbContext())
            {

                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                             .Include(d => d.tbClass)
                             orderby p.No ascending
                             select p).Distinct().ToList();

                var studengList = new List<Dto.OrgStudent.SelectOrgStudent>();
                foreach (var org in tbOrg)
                {
                    if (org.IsClass)
                    {
                        var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                                 where p.tbClass.Id == org.tbClass.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Dto.OrgStudent.SelectOrgStudent
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           SysUserId = p.tbStudent.tbSysUser.Id,
                                           OrgId = org.Id
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            studengList.AddRange(student);
                        }
                    }
                    else
                    {
                        var tb = from p in db.Table<Course.Entity.tbOrgStudent>()
                                 where p.tbOrg.Id == org.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbOrg.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Dto.OrgStudent.SelectOrgStudent
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           SysUserId = p.tbStudent.tbSysUser.Id,
                                           OrgId = org.Id
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            studengList.AddRange(student);
                        }
                    }
                }

                List<Code.TreeHelper> result = new List<Code.TreeHelper>() {
                    new Code.TreeHelper() {
                        Id=-1,
                        name="教学班",
                        open=true,
                        children=new List<Code.TreeHelper>()
                    }
                };
                result = GetCourseTree(result, tbOrg, studengList);
                return Json(result);
            }
        }

        public List<Code.TreeHelper> GetCourseTree(List<Code.TreeHelper> result, List<Course.Entity.tbOrg> tbClass, List<Dto.OrgStudent.SelectOrgStudent> tbClassStudent)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                foreach (var item in tbClass)
                {
                    var node = new Code.TreeHelper();
                    node.name = item.OrgName;
                    node.Id = item.Id;
                    node.open = false;
                    node.isChecked = false;
                    node.chkDisabled = false;
                    var classStudent = (from p in tbClassStudent.Where(d => d.OrgId == item.Id)
                                        select p).ToList();
                    node.children = GetCourseTreeSub(classStudent);
                    result[0].children.Add(node);
                }
                return result;
            }
        }

        private List<Code.TreeHelper> GetCourseTreeSub(List<Dto.OrgStudent.SelectOrgStudent> courseList)
        {
            var result = new List<Code.TreeHelper>();
            foreach (var item in courseList)
            {
                var node = new Code.TreeHelper();
                node.name = item.StudentName;
                node.Id = item.SysUserId;
                node.open = true;
                node.isChecked = false;
                node.children = null;
                result.Add(node);
            }
            return result;
        }

        public static IQueryable<Student.Entity.tbStudent> GetOrgStudent(XkSystem.Models.DbContext db, int orgId)
        {
            var org = (from p in db.Table<Course.Entity.tbOrg>()
                       where p.Id == orgId
                       select new
                       {
                           p.Id,
                           p.IsClass,
                           ClassId = p.tbClass != null ? p.tbClass.Id : 0
                       }).FirstOrDefault();
            if (org != null)
            {
                if (org.IsClass)
                {
                    return from p in db.Table<Basis.Entity.tbClassStudent>()
                           where p.tbClass.Id == org.ClassId
                               && p.tbStudent.IsDeleted == false
                           select p.tbStudent;

                }
                else
                {
                    return from p in db.Table<Course.Entity.tbOrgStudent>()
                           where p.tbOrg.Id == org.Id
                             && p.tbStudent.IsDeleted == false
                           select p.tbStudent;
                }
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<Student.Entity.tbStudent> GetOrgGradeStudent(XkSystem.Models.DbContext db, int orgId, int gradeId)
        {
            var org = (from p in db.Table<Course.Entity.tbOrg>()
                       where p.Id == orgId
                       select new
                       {
                           p.Id,
                           p.IsClass,
                           ClassId = p.tbClass != null ? p.tbClass.Id : 0
                       }).FirstOrDefault();
            if (org != null)
            {
                if (org.IsClass)
                {
                    return from p in db.Table<Basis.Entity.tbClassStudent>()
                           where p.tbClass.Id == org.ClassId
                               && p.tbStudent.IsDeleted == false
                               && p.tbClass.tbGrade.Id == gradeId
                           select p.tbStudent;

                }
                else
                {
                    return from p in db.Table<Course.Entity.tbOrgStudent>()
                           where p.tbOrg.Id == org.Id
                             && p.tbStudent.IsDeleted == false
                             && p.tbOrg.tbGrade.Id == gradeId
                           select p.tbStudent;
                }
            }
            else
            {
                return null;
            }
        }

        public static List<Student.Entity.tbStudent> GetOrgStudent(List<int> orgIdList)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var orgList = (from p in db.Table<Course.Entity.tbOrg>()
                               where orgIdList.Contains(p.Id)
                               select new
                               {
                                   p.Id,
                                   p.IsClass,
                                   ClassId = p.tbClass.Id
                               }).ToList();

                var orgIds = orgList.Where(d => d.IsClass == false).Select(d => d.Id).ToList();
                var classIds = orgList.Where(d => d.IsClass).Select(d => d.ClassId).ToList();
                var orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                      where orgIds.Contains(p.tbOrg.Id)
                                        && p.tbStudent.IsDeleted == false
                                      select p.tbStudent).ToList();
                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                        where classIds.Contains(p.tbClass.Id)
                                            && p.tbStudent.IsDeleted == false
                                        select p.tbStudent).ToList();
                return orgStudentList.Union(classStudentList).ToList();
            }
        }

        public static IQueryable<Student.Entity.tbStudent> GetOrgGradeStudent(XkSystem.Models.DbContext db, int orgId)
        {
            var org = (from p in db.Table<Course.Entity.tbOrg>()
                       where p.Id == orgId
                       select new
                       {
                           p.Id,
                           p.IsClass,
                           ClassId = p.tbClass != null ? p.tbClass.Id : 0
                       }).FirstOrDefault();
            if (org != null)
            {
                if (org.IsClass)
                {
                    return from p in db.Table<Basis.Entity.tbClassStudent>()
                           where p.tbClass.Id == org.ClassId
                               && p.tbStudent.IsDeleted == false
                           select p.tbStudent;

                }
                else
                {
                    return from p in db.Table<Course.Entity.tbOrgStudent>()
                           where p.tbOrg.Id == org.Id
                             && p.tbStudent.IsDeleted == false
                           select p.tbStudent;
                }
            }
            else
            {
                return null;
            }
        }
    }
}