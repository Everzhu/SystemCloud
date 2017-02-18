using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Student.Controllers
{
    public class StudentBestController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentBest.List();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                var classStudentList = db.Table<Basis.Entity.tbClassStudent>()
                    .Include(d => d.tbStudent)
                    .Include(d => d.tbClass).ToList();
                var studentBestList = db.Table<Student.Entity.tbStudentBest>()
                    .Include(d => d.tbStudent).ToList();
                var tb = db.Table<Basis.Entity.tbClass>();

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.ClassName.Contains(vm.SearchText));
                }
                if (vm.GradeId > 0)
                {
                    tb = tb.Where(d => d.tbGrade.Id == vm.GradeId);
                }

                vm.DataList = (from p in tb
                               orderby p.No
                               select new Dto.StudentBest.List()
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   ClassName = p.ClassName,
                                   ClassTypeName = p.tbClassType.ClassTypeName,
                               }).ToPageList(vm.Page);

                foreach (var v in vm.DataList)
                {
                    v.BestStudentCount = studentBestList.Where(d => classStudentList.Where(e => e.tbClass.Id == v.Id).Select(e => e.tbStudent.Id).ToList().Contains(d.tbStudent.Id)).Count();
                    v.StudentCount = classStudentList.Where(d => d.tbClass.Id == v.Id).Count();
                    if (v.BestStudentCount > 0 && v.StudentCount > 0)
                    {
                        v.PercentAge = Math.Round((decimal)v.BestStudentCount / v.StudentCount * 100, 2) + "%";
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudentBest.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                GradeId = vm.GradeId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult ClassStudentList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentBest.ClassStudentList();
                var studentBestIds = db.Table<Student.Entity.tbStudentBest>()
                    .Select(d => d.tbStudent.Id).ToList();
                vm.DataList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                               where p.tbClass.Id == vm.ClassId
                               && (p.tbStudent.StudentName.Contains(vm.SearchText) || p.tbStudent.StudentCode.Contains(vm.SearchText))
                               orderby p.No
                               select new Dto.StudentBest.ClassStudentList()
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   StudentId = p.tbStudent.Id,
                                   StudentCode = p.tbStudent.StudentCode,
                                   StudentName = p.tbStudent.StudentName,
                                   SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                   IsBest = studentBestIds.Contains(p.tbStudent.Id) ? true : false
                               }).ToList();

                //foreach(var v in )
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassStudentList(Models.StudentBest.ClassStudentList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassStudentList", new
            {
                SearchText = vm.SearchText,
                ClassId = vm.ClassId
            }));
        }

        public ActionResult SetBest(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Student.Entity.tbStudentBest>().Where(d => d.tbStudent.Id == id).FirstOrDefault();
                if (tb != null)
                {
                    tb.IsDeleted = true;
                }
                else
                {
                    tb = new Student.Entity.tbStudentBest()
                    {
                        tbStudent = db.Set<Student.Entity.tbStudent>().Find(id),
                        tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                        InputDate = DateTime.Now
                    };
                    db.Set<Student.Entity.tbStudentBest>().Add(tb);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("设置了优秀学生");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult GradeStudentList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentBest.GradeStudentList();
                var bestStudentIds = db.Table<Student.Entity.tbStudentBest>()
                    .Select(d => d.tbStudent.Id).ToList();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId <= 0)
                {
                    vm.GradeList.FirstOrDefault().Selected = true;
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                var tb = db.Table<Basis.Entity.tbClassStudent>();

                if (vm.GradeId > 0)
                {
                    tb = tb.Where(d => d.tbClass.tbGrade.Id == vm.GradeId);
                }
                if (string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbClass.ClassName.Contains(vm.SearchText)
                          || d.tbStudent.StudentName.Contains(vm.SearchText)
                          || d.tbStudent.StudentCode.Contains(vm.SearchText));
                }

                vm.DataList = (from p in tb
                               orderby p.No
                               select new Dto.StudentBest.GradeStudentList()
                               {
                                   ClassName = p.tbClass.ClassName,
                                   StudentId = p.tbStudent.Id,
                                   StudentCode = p.tbStudent.StudentCode,
                                   StudentName = p.tbStudent.StudentName,
                                   SexName = p.tbStudent.tbSysUser.tbSex.SexName
                               }).ToPageList(vm.Page);

                foreach (var v in vm.DataList)
                {
                    v.IsBest = bestStudentIds.Contains(v.StudentId) ? true : false;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GradeStudentList(Models.StudentBest.GradeStudentList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("GradeStudentList", new
            {
                SearchText = vm.SearchText,
                GradeId = vm.GradeId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult ClearGradeBestStudent(int GradeId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var classStudentIds = db.Table<Basis.Entity.tbClassStudent>()
                    .Where(d => d.tbClass.tbGrade.Id == GradeId).Select(d => d.tbStudent.Id).ToList();
                var studentBestList = db.Table<Student.Entity.tbStudentBest>()
                    .Where(d => classStudentIds.Contains(d.tbStudent.Id)).ToList();

                foreach (var v in studentBestList)
                {
                    v.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除年级优秀学生");
                }

                return Content("<script>window.location.href='" + Url.Action("GradeStudentList", new { GradeId = GradeId }) + "'</script>");
            }
        }

        public ActionResult Import()
        {
            var vm = new Models.StudentBest.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Student/Views/StudentBest/BestStudent.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.StudentBest.Import vm)
        {
            if (ModelState.IsValid)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 上传文件
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
                    var tbList = new List<string>() { "学生学号", "学生姓名" };

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

                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dtoTemp = new Dto.StudentBest.Import()
                        {
                            StudentCode = dr["学生学号"].ConvertToString(),
                            StudentName = dr["学生姓名"].ConvertToString()
                        };
                        if (vm.ImportList.Where(d => d.StudentCode == dtoTemp.StudentCode
                             && d.StudentName == dtoTemp.StudentName).Any() == false)
                        {
                            vm.ImportList.Add(dtoTemp);
                        }
                    }

                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.StudentCode) &&
                        string.IsNullOrEmpty(d.StudentName));

                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    #region 验证数据
                    var studentList = db.Table<Student.Entity.tbStudent>().ToList();

                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.StudentName))
                        {
                            item.Error += "学生姓名不能为空;";
                        }
                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error += "学生学号不能为空;";
                        }
                        if (studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Any() == false)
                        {
                            item.Error += "系统中不存在该学生;";
                        }
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 保存数据
                    var studentBestList = db.Table<Student.Entity.tbStudentBest>()
                        .Include(d => d.tbStudent).ToList();
                    var tbStudentBestList = new List<Student.Entity.tbStudentBest>();
                    foreach (var v in vm.ImportList)
                    {
                        if (tbStudentBestList.Where(d => d.tbStudent.StudentCode == v.StudentCode && d.tbStudent.StudentName == v.StudentName).Any() == false
                            && studentBestList.Where(d => d.tbStudent.StudentCode == v.StudentCode && d.tbStudent.StudentName == v.StudentName).Any() == false)
                        {
                            var studentBest = new Student.Entity.tbStudentBest()
                            {
                                tbStudent = studentList.Where(d => d.StudentName == v.StudentName && d.StudentCode == v.StudentCode).FirstOrDefault(),
                                tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                                InputDate = DateTime.Now
                            };
                            tbStudentBestList.Add(studentBest);
                        }
                    }

                    db.Set<Student.Entity.tbStudentBest>().AddRange(tbStudentBestList);

                    if (db.SaveChanges() > 0)
                    {
                        vm.Status = true;
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量添加优秀学生");
                    }
                    #endregion
                }
            }
            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }


        //[HttpPost]
        //public ActionResult Delete(List<int> ids)
        //{
        //    var error = new List<string>();

        //    using (var db = new XkSystem.Models.DbContext())
        //    {
        //        var tb = db.Table<Student.Entity.tbStudentHonorLevel>().Where(d => ids.Contains(d.Id)).ToList();
        //        foreach (var v in tb)
        //        {
        //            v.IsDeleted = true;
        //        }

        //        if (db.SaveChanges() > 0)
        //        {
        //            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除学生获奖级别");
        //        }
        //    }

        //    return Code.MvcHelper.Post(error);
        //}

        //public ActionResult Edit(int id = 0)
        //{
        //    var vm = new Models.StudentHonorLevel.Edit();

        //    if (id != 0)
        //    {
        //        using (var db = new XkSystem.Models.DbContext())
        //        {
        //            var tb = db.Set<Student.Entity.tbStudentHonorLevel>().Find(id);
        //            vm.StudentHonorLevelEdit = new Dto.StudentHonorLevel.Edit()
        //            {
        //                Id = tb.Id,
        //                No = tb.No,
        //                StudentHonorLevelName = tb.StudentHonorLevelName
        //            };
        //        }
        //    }

        //    return View(vm);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Models.StudentHonorLevel.Edit vm)
        //{
        //    var error = new List<string>();
        //    using (var db = new XkSystem.Models.DbContext())
        //    {
        //        if (vm.StudentHonorLevelEdit.Id != 0)
        //        {
        //            var tb = db.Set<Student.Entity.tbStudentHonorLevel>().Find(vm.StudentHonorLevelEdit.Id);
        //            tb.No = vm.StudentHonorLevelEdit.No;
        //            tb.StudentHonorLevelName = vm.StudentHonorLevelEdit.StudentHonorLevelName;
        //        }
        //        else
        //        {
        //            var tb = new Student.Entity.tbStudentHonorLevel()
        //            {
        //                No = vm.StudentHonorLevelEdit.No,
        //                StudentHonorLevelName = vm.StudentHonorLevelEdit.StudentHonorLevelName
        //            };
        //            db.Set<Student.Entity.tbStudentHonorLevel>().Add(tb);
        //        }

        //        if (db.SaveChanges() > 0)
        //        {
        //            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加/修改学生获奖级别");
        //        }
        //    }

        //    return Code.MvcHelper.Post(error);
        //}
    }
}