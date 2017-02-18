using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class ClassAllotClassController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassAllotClass.List();
                vm.YearList = YearController.SelectList(Code.EnumHelper.YearType.Year);
                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                var tb = from p in db.Table<Basis.Entity.tbClassAllotClass>()
                         select p;
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.ClassName.Contains(vm.SearchText));
                }
                if (vm.YearId > 0)
                {
                    tb = tb.Where(d => d.tbYear.Id == vm.YearId);
                }

                vm.ClassAllotClassList = (from p in tb
                                          orderby p.No, p.ClassName
                                          select new Dto.ClassAllotClass.List
                                          {
                                              Id = p.Id,
                                              No = p.No,
                                              ClassName = p.ClassName,
                                              ClassTypeName = p.tbClassType.ClassTypeName,
                                              GradeName = p.tbGrade.GradeName,
                                              YearName = p.tbYear.YearName
                                          }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ClassAllotClass.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, yearId = vm.YearId }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbClassAllotClass>()
                          where ids.Contains(p.Id)
                          select p).ToList();
                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了分班班级");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassAllotClass.Edit();
                vm.GradeList = GradeController.SelectList();
                vm.ClassTypeList = ClassTypeController.SelectList();
                vm.YearList = YearController.SelectList(Code.EnumHelper.YearType.Year);
                if (vm.ClassAllotClassEdit.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.ClassAllotClassEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                if (id != 0)
                {
                    vm.ClassAllotClassEdit = (from p in db.Table<Basis.Entity.tbClassAllotClass>()
                                              where p.Id == id
                                              select new Dto.ClassAllotClass.Edit()
                                              {
                                                  Id = p.Id,
                                                  No = p.No,
                                                  ClassName = p.ClassName,
                                                  ClassTypeId = p.tbClassType.Id,
                                                  GradeId = p.tbGrade.Id,
                                                  YearId = p.tbYear.Id
                                              }).FirstOrDefault();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ClassAllotClass.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ClassAllotClassEdit.Id == 0)
                    {
                        var tb = new Basis.Entity.tbClassAllotClass();
                        tb.No = vm.ClassAllotClassEdit.No == null ? db.Table<Basis.Entity.tbClassAllotClass>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ClassAllotClassEdit.No;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.ClassAllotClassEdit.YearId);
                        tb.ClassName = vm.ClassAllotClassEdit.ClassName;
                        tb.tbClassType = db.Set<Basis.Entity.tbClassType>().Find(vm.ClassAllotClassEdit.ClassTypeId);
                        tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.ClassAllotClassEdit.GradeId);
                        db.Set<Basis.Entity.tbClassAllotClass>().Add(tb);
                    }
                    else
                    {
                        var tb = (from p in db.Table<Basis.Entity.tbClassAllotClass>()//.Where(d => d.Id == vm.ClassAllotClassEdit.Id).FirstOrDefault()
                                  where p.Id == vm.ClassAllotClassEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.ClassAllotClassEdit.No == null ? db.Table<Basis.Entity.tbClassAllotClass>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ClassAllotClassEdit.No;
                            tb.ClassName = vm.ClassAllotClassEdit.ClassName;
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.ClassAllotClassEdit.YearId);
                            tb.tbClassType = db.Set<Basis.Entity.tbClassType>().Find(vm.ClassAllotClassEdit.ClassTypeId);
                            tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.ClassAllotClassEdit.GradeId);
                        }
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加/修改了分班方式");
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult Import()
        {
            var vm = new Models.ClassAllotClass.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Basis/Views/ClassAllotClass/Template.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.ClassAllotClass.Import vm)
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
                    var tbList = new List<string>() { "排序", "班级名称", "班级类型", "年级" };

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

                    #region 将DataTable转为List
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dtoClass = new Dto.ClassAllotClass.Import()
                        {
                            No = Convert.ToString(dr["排序"]),
                            ClassName = Convert.ToString(dr["班级名称"]),
                            GradeName = Convert.ToString(dr["年级"]),
                            ClassTypeName = Convert.ToString(dr["班级类型"])
                        };
                        if (vm.ImportList.Where(d => d.No == dtoClass.No
                                            && d.ClassName == dtoClass.ClassName
                                            && d.GradeName == dtoClass.GradeName
                                            && d.ClassTypeName == dtoClass.ClassTypeName).Count() == 0)
                        {
                            vm.ImportList.Add(dtoClass);
                        }
                    }
                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.No) &&
                        string.IsNullOrEmpty(d.ClassName) &&
                        string.IsNullOrEmpty(d.GradeName) &&
                        string.IsNullOrEmpty(d.ClassTypeName)
                    );
                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    var gradeList = db.Table<Basis.Entity.tbGrade>().ToList();
                    var classTypeList = db.Table<Basis.Entity.tbClassType>().ToList();
                    var classAllotClassList = db.Table<Basis.Entity.tbClassAllotClass>().ToList();
                    var year = db.Table<Basis.Entity.tbYear>().Where(d => d.IsDefault).FirstOrDefault();

                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.ClassName))
                        {
                            item.Error = item.Error + "班级名称不能为空!";
                        }

                        int No = 0;
                        if (int.TryParse(item.No, out No) == false || No <= 0)
                        {
                            item.Error = item.Error + "排序必须是正整数!";
                        }
                        if (classTypeList.Where(d => d.ClassTypeName == item.ClassTypeName).Count() == 0)
                        {
                            item.Error += "班级类型不存在；";
                        }
                        if (gradeList.Where(d => d.GradeName == item.GradeName).Count() == 0)
                        {
                            item.Error += "年级不存在;";
                        }
                    }
                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 数据导入，判断关键字，存在的数据做更新，不存在的做新增
                    var addClassAllotClassList = new List<Basis.Entity.tbClassAllotClass>();
                    foreach (var item in vm.ImportList)
                    {
                        Basis.Entity.tbClassAllotClass tb = null;
                        if (classAllotClassList.Where(d => d.ClassName == item.ClassName && d.tbClassType.ClassTypeName == item.ClassTypeName && d.tbGrade.GradeName == item.GradeName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                #region 修改
                                tb = classAllotClassList.Where(d => d.ClassName == item.ClassName && d.tbClassType.ClassTypeName == item.ClassTypeName && d.tbGrade.GradeName == item.GradeName).FirstOrDefault();

                                tb.No = item.No.ConvertToInt();
                                tb.ClassName = item.ClassName;
                                tb.tbYear = year;
                                tb.tbGrade = gradeList.Where(d => d.GradeName.Equals(item.GradeName)).FirstOrDefault();
                                tb.tbClassType = classTypeList.Where(d => d.ClassTypeName.Equals(item.ClassTypeName)).FirstOrDefault();

                                #endregion
                            }
                        }
                        else
                        {
                            #region 新增行政班
                            tb = new Basis.Entity.tbClassAllotClass();
                            tb.No = item.No.ConvertToInt();
                            tb.ClassName = item.ClassName;
                            tb.tbYear = year;
                            tb.tbClassType = classTypeList.Where(d => d.ClassTypeName.Equals(item.ClassTypeName)).FirstOrDefault();
                            tb.tbGrade = gradeList.Where(d => d.GradeName.Equals(item.GradeName)).FirstOrDefault();
                            addClassAllotClassList.Add(tb);
                            #endregion
                        }
                    }
                    #endregion

                    db.Set<Basis.Entity.tbClassAllotClass>().AddRange(addClassAllotClassList);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了行政班");
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
                var vm = new Models.ClassAllotClass.Export();

                var file = System.IO.Path.GetTempFileName();

                var tb = db.Table<Basis.Entity.tbClassAllotClass>();

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.ClassName.Contains(vm.SearchText));
                }

                vm.ExportList = (from p in tb
                                 select new Dto.ClassAllotClass.Export()
                                 {
                                     ClassName = p.ClassName,
                                     No = p.No,
                                     ClassTypeName = p.tbClassType.ClassTypeName,
                                     GradeName = p.tbGrade.GradeName,
                                 }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("排序"),
                        new System.Data.DataColumn("班级名称"),
                        new System.Data.DataColumn("年级"),
                        new System.Data.DataColumn("班级类型")
                    });
                foreach (var a in vm.ExportList)
                {
                    var dr = dt.NewRow();
                    dr["排序"] = a.No;
                    dr["班级名称"] = a.ClassName;
                    dr["年级"] = a.GradeName;
                    dr["班级类型"] = a.ClassTypeName;
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

        public ActionResult Batch()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassAllotClass.Batch();
                vm.YearList = YearController.SelectList(Code.EnumHelper.YearType.Year);
                if (vm.BatchEdit.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.BatchEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }
                vm.GradeList = GradeController.SelectList();
                vm.ClassTypeList = ClassTypeController.SelectList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Batch(Models.ClassAllotClass.Batch vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var tb = db.Table<Basis.Entity.tbClassAllotClass>().ToList();
                    var grade = db.Set<Basis.Entity.tbGrade>().Find(vm.BatchEdit.GradeId);
                    var year = db.Set<Basis.Entity.tbYear>().Find(vm.BatchEdit.YearId);
                    var classType = db.Set<Basis.Entity.tbClassType>().Find(vm.BatchEdit.ClassTypeId);

                    var list = new List<Basis.Entity.tbClassAllotClass>();
                    for (var i = 1; i <= vm.BatchEdit.Num; i++)
                    {
                        var classAllotClass = new Basis.Entity.tbClassAllotClass()
                        {
                            ClassName = grade.GradeName + i + "班",
                            No = i,
                            tbGrade = grade,
                            tbYear = year,
                            tbClassType = classType
                        };
                        if (tb.Where(d => d.ClassName == classAllotClass.ClassName).Count() == 0)
                        {
                            list.Add(classAllotClass);
                        }
                    }

                    db.Set<Basis.Entity.tbClassAllotClass>().AddRange(list);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加/修改了分班方式");
                    }
                }
            }

            return Code.MvcHelper.Post(error);
        }

        public static List<System.Web.Mvc.SelectListItem> SelectList(int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbClassAllotClass>()
                          where p.tbYear.Id == yearId
                          select new System.Web.Mvc.SelectListItem()
                          {
                              Value = p.Id.ToString(),
                              Text = p.ClassName
                          }).ToList();
                return tb;
            }
        }
    }
}