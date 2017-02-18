using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Teacher.Controllers
{
    public class TeacherHonorController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeacherHonor.List();
                //vm.TeacherName = db.Set<Teacher.Entity.tbTeacher>().Find(vm.TeacherId).TeacherName;
                vm.TeacherHonorLevelList = TeacherHonorLevelController.SelectList();
                vm.TeacherHonorTypeList = TeacherHonorTypeController.SelectList();
                var tb = db.Table<Teacher.Entity.tbTeacherHonor>().Where(d => d.tbTeacher.IsDeleted == false);

                #region 查询条件
                //tb = tb.Where(d => d.tbTeacher.Id == vm.TeacherId);
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.HonorName.Contains(vm.SearchText)
                        || d.tbTeacher.TeacherName.Contains(vm.SearchText)
                        || d.tbTeacher.TeacherCode.Contains(vm.SearchText));
                }
                if (vm.TeacherHonorLevelId > 0)
                {
                    tb = tb.Where(d => d.tbTeacherHonorLevel.Id == vm.TeacherHonorLevelId);
                }
                if (vm.TeacherHonorTypeId > 0)
                {
                    tb = tb.Where(d => d.tbTeacherHonorType.Id == vm.TeacherHonorTypeId);
                }
                #endregion

                vm.DataList = (from p in tb
                               orderby p.No
                               select new Dto.TeacherHonor.List()
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   TeacherId = p.tbTeacher.Id,
                                   TeacherCode = p.tbTeacher.TeacherCode,
                                   TeacherName = p.tbTeacher.TeacherName,
                                   HonorName = p.HonorName,
                                   HonorFile = p.HonorFile,
                                   TeacherHonorLevelName = p.tbTeacherHonorLevel.TeacherHonorLevelName,
                                   TeacherHonorTypeName = p.tbTeacherHonorType.TeacherHonorTypeName
                               }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.TeacherHonor.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                //TeacherId = vm.TeacherId,
                TeacherHonorLevelId = vm.TeacherHonorLevelId,
                TeacherHonorTypeId = vm.TeacherHonorTypeId
            }));
        }

        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Teacher.Entity.tbTeacherHonor>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了教师荣誉");
                }
            }
            return Code.MvcHelper.Post();
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeacherHonor.Edit();
                vm.TeacherHonorLevelList = TeacherHonorLevelController.SelectList();
                vm.TeacherHonorTypeList = TeacherHonorTypeController.SelectList();

                if (id > 0)
                {
                    vm.DataEdit = (from p in db.Table<Teacher.Entity.tbTeacherHonor>()
                                   where p.Id == id
                                   select new Dto.TeacherHonor.Edit()
                                   {
                                       Id = p.Id,
                                       No = p.No,
                                       TeacherId = p.tbTeacher.Id,
                                       TeacherCode = p.tbTeacher.TeacherCode,
                                       TeacherName = p.tbTeacher.TeacherName,
                                       HonorName = p.HonorName,
                                       HonorFile = p.HonorFile,
                                       TeacherHonorLevelId = p.tbTeacherHonorLevel.Id,
                                       TeacherHonorTypeId = p.tbTeacherHonorType.Id
                                   }).FirstOrDefault();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.TeacherHonor.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var fileName = string.Empty;
                    var file = Request.Files["DataEdit.HonorFile"];
                    if (Code.Common.GetFileType(file.FileName) != Code.FileType.Image)
                    {
                        return Content("<script >alert('图片格式必须是jpg、jpeg、png、bmp格式！');history.go(-1);</script >", "text/html");
                    }
                    if (string.IsNullOrEmpty(file.FileName) == false)
                    {
                        fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                        var fileSave = Server.MapPath("~/Files/TeacherHonor/");
                        file.SaveAs(fileSave + fileName);
                    }


                    if (vm.DataEdit.Id > 0)
                    {
                        var tb = db.Set<Teacher.Entity.tbTeacherHonor>().Find(vm.DataEdit.Id);
                        tb.HonorFile = fileName;
                        tb.HonorName = vm.DataEdit.HonorName;
                        tb.No = vm.DataEdit.No > 0 ? (int)vm.DataEdit.No : db.Table<Teacher.Entity.tbTeacherHonor>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1;
                        tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.DataEdit.TeacherId);
                        tb.tbTeacherHonorLevel = db.Set<Teacher.Entity.tbTeacherHonorLevel>().Find(vm.DataEdit.TeacherHonorLevelId);
                        tb.tbTeacherHonorType = db.Set<Teacher.Entity.tbTeacherHonorType>().Find(vm.DataEdit.TeacherHonorTypeId);
                    }
                    else
                    {
                        if (db.Table<Teacher.Entity.tbTeacherHonor>().Where(d => d.HonorName == vm.DataEdit.HonorName).Any())
                        {
                            return Content("<script >alert('该教师已有相同荣誉！');window.parent.location.reload();</script >", "text/html");
                        }
                        var teacherCode = vm.DataEdit.TeacherNameCode.Split('(')[0];
                        var tb = new Teacher.Entity.tbTeacherHonor()
                        {
                            HonorFile = fileName,
                            HonorName = vm.DataEdit.HonorName,
                            No = vm.DataEdit.No > 0 ? (int)vm.DataEdit.No : db.Table<Teacher.Entity.tbTeacherHonor>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1,
                            tbTeacher = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.TeacherCode == teacherCode).FirstOrDefault(),
                            tbTeacherHonorLevel = db.Set<Teacher.Entity.tbTeacherHonorLevel>().Find(vm.DataEdit.TeacherHonorLevelId),
                            tbTeacherHonorType = db.Set<Teacher.Entity.tbTeacherHonorType>().Find(vm.DataEdit.TeacherHonorTypeId)
                        };
                        db.Set<Teacher.Entity.tbTeacherHonor>().Add(tb);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加/修改教师荣誉");
                    }
                }
            }

            return Content("<script >window.parent.location.reload();</script >", "text/html");
        }

        public ActionResult HonorFileInfo(int id)
        {
            var vm = new Models.TeacherHonor.HonorFileInfo();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.Info = db.Set<Teacher.Entity.tbTeacherHonor>().Find(id);

                return View(vm);
            }
        }

        public ActionResult Import()
        {
            var vm = new Models.TeacherHonor.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Teacher/Views/TeacherHonor/TeacherHonor.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.TeacherHonor.Import vm)
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

                    var tbList = new List<string>() { "排序", "教师编号", "教师姓名", "荣誉名称", "荣誉类型", "荣誉级别" };

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

                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dto = new Dto.TeacherHonor.Import()
                        {
                            HonorName = dr["荣誉名称"].ConvertToString(),
                            TeacherCode = dr["教师编号"].ConvertToString(),
                            TeacherName = dr["教师姓名"].ConvertToString(),
                            TeacherHonorLevelName = dr["荣誉级别"].ConvertToString(),
                            TeacherHonorTypeName = dr["荣誉类型"].ConvertToString()
                        };
                        int no = 0;
                        if (int.TryParse(dr["排序"].ConvertToString(), out no))
                        {
                            dto.No = no;
                        }
                        else
                        {
                            ModelState.AddModelError("", "排序字段必须是数字！");
                            return View(vm);
                        }
                        if (vm.ImportList.Where(d => d.No == dto.No
                            && d.HonorName == dto.HonorName
                            && d.TeacherCode == dto.TeacherCode
                            && d.TeacherHonorLevelName == dto.TeacherHonorLevelName
                            && d.TeacherHonorTypeName == dto.TeacherHonorTypeName
                            && d.TeacherName == dto.TeacherName).Any() == false)
                        {
                            vm.ImportList.Add(dto);
                        }
                    }

                    vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.TeacherCode)
                        && string.IsNullOrEmpty(d.TeacherName)
                        && d.No == 0
                        && string.IsNullOrEmpty(d.HonorName)
                        && string.IsNullOrEmpty(d.TeacherHonorLevelName)
                        && string.IsNullOrEmpty(d.TeacherHonorTypeName));

                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    #region 验证数据
                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();
                    var teacherHonorTypeList = db.Table<Teacher.Entity.tbTeacherHonorType>().ToList();
                    var teacherHonorLevelList = db.Table<Teacher.Entity.tbTeacherHonorLevel>().ToList();
                    var teacherHonorList = db.Table<Teacher.Entity.tbTeacherHonor>()
                        .Include(d => d.tbTeacher)
                        .Include(d => d.tbTeacherHonorLevel)
                        .Include(d => d.tbTeacherHonorType).ToList();
                    foreach (var v in vm.ImportList)
                    {
                        if (string.IsNullOrWhiteSpace(v.TeacherCode))
                        {
                            v.Error += "教师编号不能为空；";
                        }
                        if (string.IsNullOrWhiteSpace(v.TeacherName))
                        {
                            v.Error += "教师名称不能为空；";
                        }
                        if (string.IsNullOrWhiteSpace(v.HonorName))
                        {
                            v.Error += "荣誉名称不能为空；";
                        }
                        if (string.IsNullOrWhiteSpace(v.TeacherHonorLevelName))
                        {
                            v.Error += "荣誉级别不能为空；";
                        }
                        if (string.IsNullOrWhiteSpace(v.TeacherHonorTypeName))
                        {
                            v.Error += "荣誉类型不能为空；";
                        }
                        if (teacherList.Where(d => d.TeacherCode == v.TeacherCode && d.TeacherName == v.TeacherName).Any() == false)
                        {
                            v.Error += "教师不存在；";
                        }
                        if (teacherHonorLevelList.Where(d => d.TeacherHonorLevelName == v.TeacherHonorLevelName).Any() == false)
                        {
                            v.Error += "荣誉级别不存在；";
                        }
                        if (teacherHonorTypeList.Where(d => d.TeacherHonorTypeName == v.TeacherHonorTypeName).Any() == false)
                        {
                            v.Error += "荣誉类型不存在；";
                        }
                        if (vm.IsUpdate == false && teacherHonorList.Where(d => d.HonorName == v.HonorName).Any())
                        {
                            v.Error += "系统中已存在此记录；";
                        }
                        //if (vm.IsUpdate == false && teacherHonorList.Where(d => d.HonorName == v.HonorName
                        //         && d.tbTeacher.TeacherCode == v.TeacherCode
                        //         && d.tbTeacherHonorLevel.TeacherHonorLevelName == v.TeacherHonorLevelName
                        //         && d.tbTeacherHonorType.TeacherHonorTypeName == v.TeacherHonorTypeName).Any())
                        //{
                        //    v.Error += "系统中已存在此记录；";
                        //}
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 数据导入
                    var tbTeacherHonorList = new List<Teacher.Entity.tbTeacherHonor>();
                    foreach (var v in vm.ImportList)
                    {
                        if (teacherHonorList.Where(d => d.HonorName == v.HonorName).Any())
                        {
                            var tb = teacherHonorList.Where(d => d.HonorName == v.HonorName).FirstOrDefault();
                            tb.No = v.No;
                            tb.tbTeacher = teacherList.Where(d => d.TeacherCode == v.TeacherCode).FirstOrDefault();
                            tb.tbTeacherHonorLevel = teacherHonorLevelList.Where(d => d.TeacherHonorLevelName == v.TeacherHonorLevelName).FirstOrDefault();
                            tb.tbTeacherHonorType = teacherHonorTypeList.Where(d => d.TeacherHonorTypeName == v.TeacherHonorTypeName).FirstOrDefault();
                        }
                        else
                        {
                            var tb = new Teacher.Entity.tbTeacherHonor()
                            {
                                No = v.No,
                                HonorName = v.HonorName,
                                tbTeacher = teacherList.Where(d => d.TeacherCode == v.TeacherCode).FirstOrDefault(),
                                tbTeacherHonorLevel = teacherHonorLevelList.Where(d => d.TeacherHonorLevelName == v.TeacherHonorLevelName).FirstOrDefault(),
                                tbTeacherHonorType = teacherHonorTypeList.Where(d => d.TeacherHonorTypeName == v.TeacherHonorTypeName).FirstOrDefault()
                            };
                            tbTeacherHonorList.Add(tb);
                        }
                    }
                    db.Set<Teacher.Entity.tbTeacherHonor>().AddRange(tbTeacherHonorList);

                    if (db.SaveChanges() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入教师荣誉");
                        //刷新缓存
                        System.Web.HttpContext.Current.Cache["Power"] = Sys.Controllers.SysRolePowerController.GetPower();
                        vm.Status = true;
                    }
                    #endregion
                }
            }

            return View(vm);
        }

        public ActionResult Export()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeacherHonor.Export();

                var tb = db.Table<Teacher.Entity.tbTeacherHonor>();

                #region 查询条件
                if (string.IsNullOrEmpty(vm.searchText) == false)
                {
                    tb = tb.Where(d => d.HonorName.Contains(vm.searchText));
                }
                if (vm.TeacherHonorLevelId > 0)
                {
                    tb = tb.Where(d => d.tbTeacherHonorLevel.Id == vm.TeacherHonorLevelId);
                }
                if (vm.TeacherHonorTypeId > 0)
                {
                    tb = tb.Where(d => d.tbTeacherHonorType.Id == vm.TeacherHonorTypeId);
                }
                #endregion

                vm.DataList = (from p in tb
                               orderby p.No
                               select new Dto.TeacherHonor.Export()
                               {
                                   No = p.No,
                                   HonorName = p.HonorName,
                                   TeacherCode = p.tbTeacher.TeacherCode,
                                   TeacherName = p.tbTeacher.TeacherName,
                                   TeacherHonorLevelName = p.tbTeacherHonorLevel.TeacherHonorLevelName,
                                   TeacherHonorTypeName = p.tbTeacherHonorType.TeacherHonorTypeName
                               }).ToList();


                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("排序"),
                        new System.Data.DataColumn("教师编号"),
                        new System.Data.DataColumn("教师姓名"),
                        new System.Data.DataColumn("荣誉名称"),
                        new System.Data.DataColumn("荣誉类型"),
                        new System.Data.DataColumn("荣誉级别")
                    });
                foreach (var a in vm.DataList)
                {
                    var dr = dt.NewRow();
                    dr["排序"] = a.No;
                    dr["教师编号"] = a.TeacherCode;
                    dr["教师姓名"] = a.TeacherName;
                    dr["荣誉名称"] = a.HonorName;
                    dr["荣誉类型"] = a.TeacherHonorTypeName;
                    dr["荣誉级别"] = a.TeacherHonorLevelName;
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
    }
}