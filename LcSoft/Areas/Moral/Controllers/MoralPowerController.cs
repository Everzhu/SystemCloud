using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralPowerController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralPower.List();
                var tb = (from p in db.Table<Moral.Entity.tbMoralPower>() where p.tbMoralItem.Id == vm.MoralItemId select p);
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(p => p.tbTeacher.TeacherName.Contains(vm.SearchText) || p.tbTeacher.TeacherCode.Contains(vm.SearchText));
                }

                vm.MoralPowerList = (from p in tb
                                     select new Dto.MoralPower.List()
                                     {
                                         Id = p.Id,
                                         No = p.No,
                                         MoralDate = p.MoralDate,
                                         MoralItemName = p.tbMoralItem.MoralItemName,
                                         TeacherName = p.tbTeacher.TeacherName
                                     }).ToList();
                var powerIds = vm.MoralPowerList.Select(p => p.Id);
                var morarlPowerClass = (from p in db.Table<Entity.tbMoralPowerClass>() where powerIds.Contains(p.tbMoralPower.Id) select new { p.tbMoralPower.Id, p.tbClass.ClassName });
                vm.MoralPowerList.ForEach(p => {
                    p.MoralPowerClassNames = string.Join(",", morarlPowerClass.Where(c => c.Id == p.Id).Select(c => c.ClassName));
                });
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.MoralPower.List vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("List", new
            {
                searchText=vm.SearchText,
                moralItemId = vm.MoralItemId
            }));
        }



        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.MoralPower.Edit();
            using (var db = new XkSystem.Models.DbContext())
            {
                if (id > 0)
                {
                    var tb = (from p in db.Table<Moral.Entity.tbMoralPower>()
                              where p.Id == id
                              select new Dto.MoralPower.Edit()
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  TeacherId = p.tbTeacher.Id,
                                  MoralDate = p.MoralDate,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.MoralPowerEdit = tb;
                    }

                    vm.MoralClassIds = (from p in db.Table<Entity.tbMoralPowerClass>() where p.tbMoralPower.Id == id select p.tbClass.Id).ToList();
                    if (vm.MoralClassIds != null && vm.MoralClassIds.Any())
                    {
                        vm.MoralClassId = string.Join(",", vm.MoralClassIds);
                    }
                }
                else
                {
                    vm.MoralPowerEdit.No = db.Table<Moral.Entity.tbMoralPower>().Where(p => p.tbMoralItem.Id == vm.MoralItemId).Select(p => p.No).DefaultIfEmpty(0).Max() + 1;
                }
            }
            vm.TeacherList = Teacher.Controllers.TeacherController.SelectList();
            vm.MoralClassList = MoralClassController.SelectList(vm.MoralId);
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.MoralPower.Edit vm)
        {
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Moral.Entity.tbMoralPower>() where p.tbMoralItem.Id == vm.MoralItemId && p.tbTeacher.Id==vm.MoralPowerEdit.TeacherId && p.Id != vm.MoralPowerEdit.Id select p);
                if (vm.MoralPowerEdit.MoralDate.HasValue)
                {
                    tb = tb.Where(p => p.MoralDate == vm.MoralPowerEdit.MoralDate.Value);
                }
                else
                {
                    tb = tb.Where(p => !p.MoralDate.HasValue);
                }
                var isExists = tb.Count() > 0;
                if (isExists)
                {
                    error.Add("已存在相同的记录！");
                    return Code.MvcHelper.Post(error);
                }
                if (vm.MoralPowerEdit.Id == 0)
                {
                    var tbMoralPower = new Moral.Entity.tbMoralPower()
                    {
                        No = (vm.MoralPowerEdit.No ?? 0) == 0 ? db.Table<Moral.Entity.tbMoralPower>().Where(p => p.tbMoralItem.Id == vm.MoralItemId).Select(p => p.No).DefaultIfEmpty(0).Max() + 1 : vm.MoralPowerEdit.No.Value,
                        MoralDate = vm.MoralPowerEdit.MoralDate,
                        tbMoralItem = db.Set<Moral.Entity.tbMoralItem>().Find(vm.MoralItemId),
                        tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.MoralPowerEdit.TeacherId)
                    };
                    db.Set<Moral.Entity.tbMoralPower>().Add(tbMoralPower);

                    if (!string.IsNullOrWhiteSpace(vm.MoralClassId))
                    {   
                        var classIds = vm.MoralClassId.TrimEnd(',').Split(',').Select(int.Parse).ToList();
                        var classList = (from p in db.Table<Basis.Entity.tbClass>() where classIds.Contains(p.Id) select p).ToList();

                        db.Set<Moral.Entity.tbMoralPowerClass>().AddRange(classList.Select(p => new Moral.Entity.tbMoralPowerClass()
                        {
                            tbClass =p,
                            tbMoralPower=tbMoralPower
                        }).ToList());
                    }

                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("添加了德育项目评价人员！");
                    }
                }
                else
                {
                    var tbMoralPower = (from p in db.Table<Moral.Entity.tbMoralPower>() where p.Id == vm.MoralPowerEdit.Id select p).FirstOrDefault();
                    if (tbMoralPower == null)
                    {
                        error.Add(Resources.LocalizedText.MsgNotFound);
                        return Code.MvcHelper.Post(error);
                    }

                    tbMoralPower.MoralDate = vm.MoralPowerEdit.MoralDate;
                    tbMoralPower.No = (vm.MoralPowerEdit.No ?? 0) == 0 ? db.Table<Moral.Entity.tbMoralPower>().Where(p => p.tbMoralItem.Id == vm.MoralItemId).Select(p => p.No).DefaultIfEmpty(0).Max() + 1 : vm.MoralPowerEdit.No.Value;
                    tbMoralPower.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.MoralPowerEdit.TeacherId);

                    if (!string.IsNullOrWhiteSpace(vm.MoralClassId))
                    {
                        var tbMoralPowerClass = (from p in db.Table<Moral.Entity.tbMoralPowerClass>() where p.tbMoralPower.Id == tbMoralPower.Id select p);
                        foreach (var powerClass in tbMoralPowerClass)
                        {
                            powerClass.IsDeleted = true;
                        }
                        var classIds = vm.MoralClassId.TrimEnd(',').Split(',').Select(int.Parse).ToList();
                        var classList = (from p in db.Table<Basis.Entity.tbClass>() where classIds.Contains(p.Id) select p).ToList();
                        db.Set<Moral.Entity.tbMoralPowerClass>().AddRange(classList.Select(p => new Moral.Entity.tbMoralPowerClass()
                        {
                            tbClass = p,
                            tbMoralPower = tbMoralPower
                        }).ToList());
                    }

                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("修改了德育项目评价人员！");
                    }
                }
            }
            return Code.MvcHelper.Post();
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Moral/Views/MoralPower/Import.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult Import()
        {
            return View(new Models.MoralPower.Import());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.MoralPower.Import vm)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);
                using (var db = new XkSystem.Models.DbContext())
                {
                    var ExList = new List<string>() { ".xlsx" };
                    if (!ExList.Contains(System.IO.Path.GetExtension(file.FileName)))
                    {
                        ModelState.AddModelError(string.Empty, "上传的文件不是正确的excel文件!");
                        return View(vm);
                    }
                    else
                    {
                        var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), string.Empty);
                        if (dt == null)
                        {
                            ModelState.AddModelError(string.Empty, "无法读取上传的文件，请检查文件格式是否正确!");
                            return View(vm);
                        }
                        else
                        {
                            var tbList = new List<string>() { "德育项目", "评分人员", "评分日期","评分班级" };
                            foreach (var name in tbList)
                            {
                                var text = string.Empty;
                                text += !dt.Columns.Contains(name) ? name + "," : "";
                                if (!string.IsNullOrWhiteSpace(text))
                                {
                                    ModelState.AddModelError(string.Empty, "上传的excel文件内容与预期不一致!错误详细:" + text);
                                    return View(vm);
                                }
                            }
                            if (dt.Rows.Count == 0)
                            {
                                ModelState.AddModelError(string.Empty, "上传的文件不包含任何有效数据!");
                                return View(vm);
                            }


                            var teacherList = Teacher.Controllers.TeacherController.SelectInfoList();
                            //var moralItemList = MoralItemController.SelectListByMoralId(vm.MoralId);
                            var moralItemList = db.Table<Moral.Entity.tbMoralItem>().Where(p => p.tbMoralGroup.tbMoral.Id == vm.MoralId).ToList();

                            //var moralClassList = db.Table<tbMoralClass>().Where(p => p.tbMoral.Id == vm.MoralId).ToList();
                            var moralClassList = (from p in db.Table<Entity.tbMoralClass>() where p.tbMoral.Id == vm.MoralId select p.tbClass).ToList();

                            for (var i = 0; i < dt.Rows.Count; i++)
                            {
                                var dr = dt.Rows[i];
                                var importModel = new Dto.MoralPower.Import()
                                {
                                    MoralItemName = dr["德育项目"].ConvertToString(),
                                    TeacherName = dr["评分人员"].ConvertToString(),
                                    MoralDate = dr["评分日期"].ConvertToString(),
                                    MoralClass=dr["评分班级"].ConvertToString()
                                };
                                var isExists = false;
                                if (string.IsNullOrWhiteSpace(importModel.MoralItemName))
                                {
                                    importModel.ImportError += "项目名称为空;";
                                }
                                else
                                {
                                    if (moralItemList.Count(p => p.MoralItemName.Equals(importModel.MoralItemName)) == 0)
                                    {
                                        importModel.ImportError += "找不到对应的德育项目;";
                                    }
                                    else
                                    {
                                        isExists = true;
                                    }
                                }


                                if (string.IsNullOrWhiteSpace(importModel.TeacherName))
                                {
                                    importModel.ImportError += "评分人员为空;";
                                }
                                else
                                {
                                    if (teacherList.Count(p => p.TeacherName.Equals(importModel.TeacherName)) == 0)
                                    {
                                        importModel.ImportError += "找不到相应的老师;";
                                    }
                                }

                                DateTime? date = null;
                                var isFlag = true;
                                if (!string.IsNullOrWhiteSpace(importModel.MoralDate))
                                {
                                    var _date = DateTime.MinValue;
                                    if (!DateTime.TryParse(importModel.MoralDate, out _date))
                                    {
                                        importModel.ImportError += "日期格式不正确;";
                                        isFlag = false;
                                    }
                                    else
                                    {
                                        date = _date.Date;
                                    }
                                }

                                if (isExists && isFlag)
                                {
                                    var tb = (from p in db.Table<Moral.Entity.tbMoralPower>()
                                              where p.tbTeacher.TeacherName.Equals(importModel.TeacherName) && p.tbMoralItem.MoralItemName.Equals(importModel.MoralItemName)
                                              select p);
                                    if (date.HasValue)
                                    {
                                        tb = tb.Where(p => p.MoralDate == date.Value);
                                    }
                                    if (tb.Count() > 0)
                                    {
                                        importModel.ImportError += "已存在相同的记录！";
                                    }
                                }


                                var classNames = importModel.MoralClass.Split(',').ToList();
                                if (classNames == null || !classNames.Any())
                                {
                                    importModel.ImportError += "评分班级不能为空！";
                                }
                                else
                                {
                                    classNames.RemoveAll(p => string.IsNullOrWhiteSpace(p));
                                    if (classNames == null || !classNames.Any())
                                    {
                                        importModel.ImportError += "评分班级不能为空！";
                                    }
                                    else
                                    {
                                        var unExistsName = string.Empty;
                                        classNames.ForEach(p => 
                                        {
                                            if (!moralClassList.Exists(c => c.ClassName.Equals(p)))
                                            {
                                                unExistsName += p+",";
                                            }
                                        });
                                        if (!string.IsNullOrWhiteSpace(unExistsName))
                                        {
                                            unExistsName = unExistsName.TrimEnd(',');
                                            importModel.ImportError += $"找不到如下【{unExistsName}】班级！";
                                        }
                                    }
                                }

                                vm.ImportList.Add(importModel);
                            }

                            if (vm.ImportList.Count(p => !string.IsNullOrWhiteSpace(p.ImportError)) > 0)
                            {
                                vm.ImportList.RemoveAll(p => string.IsNullOrWhiteSpace(p.ImportError));
                                return View(vm);
                            }

                            //var list = new List<Moral.Entity.tbMoralPower>();

                            vm.ImportList.ForEach(p =>
                            {
                                var tbMoralPower = new Moral.Entity.tbMoralPower()
                                {
                                    No = db.Table<Moral.Entity.tbMoralPower>().Where(m => m.tbMoralItem.MoralItemName.Equals(p.MoralItemName)).Select(g => g.No).DefaultIfEmpty(0).Max() + 1,
                                    //tbMoralItem = db.Table<Moral.Entity.tbMoralItem>().FirstOrDefault(i => i.MoralItemName.Equals(p.MoralItemName)),
                                    tbMoralItem=moralItemList.FirstOrDefault(i=>i.MoralItemName.Equals(p.MoralItemName)),
                                    tbTeacher = db.Table<Teacher.Entity.tbTeacher>().FirstOrDefault(t => t.TeacherName.Equals(p.TeacherName))
                                };
                                if (!string.IsNullOrWhiteSpace(p.MoralDate))
                                {
                                    tbMoralPower.MoralDate = DateTime.Parse(p.MoralDate).Date;
                                }
                                db.Set<Moral.Entity.tbMoralPower>().Add(tbMoralPower);


                                p.MoralClass.Split(',').ToList().ForEach(c => {
                                    if (!string.IsNullOrWhiteSpace(c))
                                    {
                                        var tbMoralPowerClass = new Entity.tbMoralPowerClass()
                                        {
                                            tbMoralPower = tbMoralPower,
                                            tbClass = moralClassList.First(mc => mc.ClassName.Equals(c))
                                        };
                                        db.Set<Entity.tbMoralPowerClass>().Add(tbMoralPowerClass);
                                    }
                                });
                                

                                //list.Add(tbMoralPower);
                            });

                            //db.Set<Moral.Entity.tbMoralPower>().AddRange(list);
                            if (db.SaveChanges() > 0)
                            {
                                Sys.Controllers.SysUserLogController.Insert("批量导入了德育项目评价人员！");
                                vm.ImportList.RemoveAll(p => string.IsNullOrWhiteSpace(p.ImportError));
                                vm.Status = true;
                            }
                        }
                    }
                }
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Moral.Entity.tbMoralPower>() where ids.Contains(p.Id) select p);
                foreach (var item in tb)
                {
                    item.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    Sys.Controllers.SysUserLogController.Insert("删除了德育项目评价人员！");
                }
            }
            return Code.MvcHelper.Post();
        }

    }
}