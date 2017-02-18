using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyCostController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyCost.List();
                var tb =(from p in db.Table<Study.Entity.tbStudyCost>()
                         where p.tbTeacher.IsDeleted == false
                         && (!string.IsNullOrEmpty(vm.SearchText)?(p.tbTeacher.TeacherName.Contains(vm.SearchText)|| p.tbTeacher.TeacherCode.Contains(vm.SearchText)):true)
                        select p);


                vm.StudyCostList = (from p in tb
                                orderby p.tbTeacher.No,p.tbTeacher.TeacherCode
                                select new Dto.StudyCost.List
                                {
                                    Id = p.Id,
                                    No = p.No,
                                    TeacherName=p.tbTeacher.TeacherName,
                                    Cost=p.Cost
                                }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudyCost.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Study.Entity.tbStudyCost>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除教师节次费用设置");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyCost.Edit();
                vm.TeacherList =Teacher.Controllers.TeacherController.SelectList();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Study.Entity.tbStudyCost>()
                              where p.Id == id
                              select new Dto.StudyCost.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  TeacherId=p.tbTeacher.Id,
                                  Cost=p.Cost
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.StudyCostEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudyCost.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.StudyCostEdit.Id == 0)
                    {
                        var check = (from p in db.Table<Study.Entity.tbStudyCost>()
                                     where p.tbTeacher.Id == vm.StudyCostEdit.TeacherId
                                     select p).FirstOrDefault();
                        if (check== null)
                        {
                            var tb = new Study.Entity.tbStudyCost();
                            tb.No = vm.StudyCostEdit.No == null ? db.Table<Study.Entity.tbStudyCost>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudyCostEdit.No;
                            tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.StudyCostEdit.TeacherId);
                            tb.Cost = Convert.ToDecimal(vm.StudyCostEdit.Cost);
                            db.Set<Study.Entity.tbStudyCost>().Add(tb);
                        }
                        else
                        {
                            check.Cost = Convert.ToDecimal(vm.StudyCostEdit.Cost);
                        }
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加费用设置");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Study.Entity.tbStudyCost>()
                                  where p.Id == vm.StudyCostEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.StudyCostEdit.No == null ? db.Table<Study.Entity.tbStudyCost>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudyCostEdit.No;
                            tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.StudyCostEdit.TeacherId);
                            tb.Cost = Convert.ToDecimal(vm.StudyCostEdit.Cost);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改费用设置");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }
                return Code.MvcHelper.Post(error, Url.Action("List"));
            }
        }

        public ActionResult Export(string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tb = (from p in db.Table<Study.Entity.tbStudyCost>()
                          where p.tbTeacher.IsDeleted == false
                          && (!string.IsNullOrEmpty(searchText) ? (p.tbTeacher.TeacherName.Contains(searchText) || p.tbTeacher.TeacherCode.Contains(searchText)) : true)
                          select p);

                var ExportList = (from p in tb
                                 orderby p.Id descending
                                 select new
                                 {
                                     Id = p.Id,
                                     p.tbTeacher.TeacherCode,
                                     p.tbTeacher.TeacherName,
                                     p.Cost
                                 }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("职工号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("节次费用"),
                    });
                foreach (var a in ExportList)
                {
                    var dr = dt.NewRow();
                    dr["职工号"] = a.TeacherCode;
                    dr["姓名"] = a.TeacherName;
                    dr["节次费用"] = a.Cost;
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
            var vm = new Models.StudyCost.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Study/Views/StudyCost/PeriodMoneyImport.xls");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.StudyCost.Import vm)
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
                    var tbList = new List<string>() { "职工号", "姓名", "节次费用"};

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
                        var dtoStudy = new Dto.StudyCost.Import()
                        {
                            TeacherCode = Convert.ToString(dr["职工号"]),
                            TeacherName = Convert.ToString(dr["姓名"]),
                            Cost = Convert.ToString(dr["节次费用"]),
                        };
                        if (vm.ImportList.Where(d => d.TeacherCode == dtoStudy.TeacherCode
                                                && d.TeacherName == dtoStudy.TeacherName
                                                ).Count() == 0)
                        {
                            vm.ImportList.Add(dtoStudy);
                        }
                    }
                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.TeacherCode) &&
                        string.IsNullOrEmpty(d.TeacherName)
                    );
                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "Excel未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    #region 3、Excel格式校验
                    var teacherList=(from p in db.Table<Teacher.Entity.tbTeacher>()
                                               select p).ToList();
                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.TeacherCode))
                        {
                            item.Error = item.Error + "职工号不能为空!";
                            continue;
                        }
                        if (string.IsNullOrEmpty(item.TeacherName))
                        {
                            item.Error = item.Error + "姓名不能为空!";
                            continue;
                        }
                        else
                        {
                            if (teacherList.Where(d => d.TeacherCode == item.TeacherCode).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "职工号不存在数据库!";
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
                    var studyCostList = (from p in db.Table<Study.Entity.tbStudyCost>()
                                       select p).ToList();

                    var addStudyList = new List<Study.Entity.tbStudyCost>();
                    foreach (var item in vm.ImportList)
                    {
                        Study.Entity.tbStudyCost tb = null;
                        tb = studyCostList.Where(d => d.tbTeacher.TeacherCode == item.TeacherCode).Select(d => d).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.Cost =item.Cost.ConvertToDecimal();
                        }
                        else
                        {
                            tb = new Study.Entity.tbStudyCost();
                            tb.tbTeacher =(from p in db.Table<Teacher.Entity.tbTeacher>()
                                           where p.TeacherCode==item.TeacherCode
                                           select p).FirstOrDefault();
                            tb.Cost = item.Cost.ConvertToDecimal();
                            addStudyList.Add(tb);
                        }
                    }
                    db.Set<Study.Entity.tbStudyCost>().AddRange(addStudyList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入节次费用");
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