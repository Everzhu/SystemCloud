using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class ClassAllotResultController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassAllotResult.List();
                vm.YearList = YearController.SelectList(Code.EnumHelper.YearType.Year);
                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }
                vm.ClassAllotClassList = ClassAllotClassController.SelectList(vm.YearId);

                var tb = from p in db.Table<Basis.Entity.tbClassAllotResult>()
                         where p.tbClassAllotClass.tbYear.Id == vm.YearId
                         select p;
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentName.Contains(vm.SearchText) || d.tbStudent.StudentCode.Contains(vm.SearchText));
                }

                if (vm.ClassAllotClassId > 0)
                {
                    tb = tb.Where(d => d.tbClassAllotClass.Id == vm.ClassAllotClassId);
                }

                vm.ClassAllotResultList = (from p in tb
                                           select new Dto.ClassAllotResult.List
                                           {
                                               Id = p.Id,
                                               ClassName = p.tbClassAllotClass.ClassName,
                                               ClassTypeName = p.tbClassAllotClass.tbClassType.ClassTypeName,
                                               GradeName = p.tbClassAllotClass.tbGrade.GradeName,
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName,
                                               SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                               Score = p.Score
                                           }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ClassAllotResult.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                yearId = vm.YearId,
                ClassAllotClassId = vm.ClassAllotClassId
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = from p in db.Table<Basis.Entity.tbClassAllotResult>()
                         where ids.Contains(p.Id)
                         select p;

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了排班结果");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassAllotResult.Edit();
                vm.ClassAllotClassList = ClassAllotClassController.SelectList(vm.YearId);

                vm.ClassAllotResultEdit = (from p in db.Table<Basis.Entity.tbClassAllotResult>()
                                           where p.Id == id
                                           select new Dto.ClassAllotResult.Edit
                                           {
                                               Id = p.Id,
                                               ClassAllotClassId = p.tbClassAllotClass.Id
                                           }).FirstOrDefault();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ClassAllotResult.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbClassAllotResult>()
                              where p.Id == vm.ClassAllotResultEdit.Id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.tbClassAllotClass = db.Set<Basis.Entity.tbClassAllotClass>().Find(vm.ClassAllotResultEdit.ClassAllotClassId);

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("分班结果中学生调班");
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult Export(int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassAllotResult.Export();

                var file = System.IO.Path.GetTempFileName();

                vm.ClassAllotResultExport = (from p in db.Table<Basis.Entity.tbClassAllotResult>()
                                             where p.tbClassAllotClass.tbYear.Id == yearId
                                                && p.tbClassAllotClass.IsDeleted == false
                                             orderby p.tbClassAllotClass.No, p.tbClassAllotClass.ClassName
                                             select new Dto.ClassAllotResult.Export
                                             {
                                                 ClassName = p.tbClassAllotClass.ClassName,
                                                 ClassTypeName = p.tbClassAllotClass.tbClassType.ClassTypeName,
                                                 GradeName = p.tbClassAllotClass.tbGrade.GradeName,
                                                 StudentCode = p.tbStudent.StudentCode,
                                                 StudentName = p.tbStudent.StudentName,
                                                 SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                                 Score = p.Score
                                             }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("班级"),
                        new System.Data.DataColumn("班级类型"),
                        new System.Data.DataColumn("年级"),
                        new System.Data.DataColumn("分班成绩")
                    });
                foreach (var a in vm.ClassAllotResultExport)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["性别"] = a.SexName;
                    dr["班级"] = a.ClassName;
                    dr["班级类型"] = a.ClassTypeName;
                    dr["年级"] = a.GradeName;
                    dr["分班成绩"] = a.Score;
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

        public ActionResult StartAllotClass()
        {
            var vm = new Models.ClassAllotResult.StartAllotClass();

            return View(vm);
        }

        [HttpPost]
        public ActionResult StartAllotClass(Models.ClassAllotResult.StartAllotClass vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var del = (from p in db.Table<Basis.Entity.tbClassAllotResult>()
                           where p.tbClassAllotClass.tbYear.Id == vm.YearId
                           select p).ToList();
                foreach (var a in del)
                {
                    a.IsDeleted = true;
                }

                var classAllotStudentList = (from p in db.Table<Basis.Entity.tbClassAllotStudent>()
                                             orderby p.Score descending
                                             select new
                                             {
                                                 p.tbStudent,
                                                 ClassTypeId = p.tbClassType.Id,
                                                 GradeId = p.tbGrade.Id,
                                                 SexId = p.tbStudent.tbSysUser.tbSex != null ? p.tbStudent.tbSysUser.tbSex.Id : 0,
                                                 Score = p.Score
                                             }).ToList();
                if (vm.IsScore)
                {
                    classAllotStudentList = (from p in db.Table<Basis.Entity.tbClassAllotStudent>()
                                             orderby p.tbStudent.EntranceScore descending
                                             select new
                                             {
                                                 p.tbStudent,
                                                 ClassTypeId = p.tbClassType.Id,
                                                 GradeId = p.tbGrade.Id,
                                                 SexId = p.tbStudent.tbSysUser.tbSex != null ? p.tbStudent.tbSysUser.tbSex.Id : 0,
                                                 Score = p.tbStudent.EntranceScore
                                             }).ToList();
                }

                var classAllotClassList = (from p in db.Table<Basis.Entity.tbClassAllotClass>()
                                           where p.tbYear.Id == vm.YearId
                                           select new
                                           {
                                               p,
                                               GradeId = p.tbGrade.Id,
                                               ClassTypeId = p.tbClassType.Id
                                           }).ToList();

                var classTypeList = (from p in db.Table<Basis.Entity.tbClassType>()
                                     orderby p.No
                                     select p.Id).ToList();
                var sexList = (from p in db.Table<Dict.Entity.tbDictSex>()
                               orderby p.No
                               select p.Id).ToList();
                sexList.Add(0);
                var list = new List<Basis.Entity.tbClassAllotResult>();

                foreach (var grade in classAllotClassList.Select(d => d.GradeId).Distinct())
                {
                    foreach (var classType in classTypeList)
                    {
                        foreach (var sex in sexList)
                        {
                            var studentListTemp = classAllotStudentList.Where(d => d.GradeId == grade && d.ClassTypeId == classType && d.SexId == sex).ToList();
                            var classListTemp = classAllotClassList.Where(d => d.GradeId == grade && d.ClassTypeId == classType).ToList();

                            if (vm.IsOrder)
                            {
                                for (int i = 0; i < studentListTemp.Count; i++)
                                {
                                    var classAllotResult = new Basis.Entity.tbClassAllotResult()
                                    {
                                        tbStudent = studentListTemp[i].tbStudent,
                                        tbClassAllotClass = classListTemp[i % classListTemp.Count].p,
                                        Score = studentListTemp[i].Score,
                                    };
                                    list.Add(classAllotResult);
                                }
                            }
                            else
                            {
                                bool b = false;
                                for (int i = 0; i < studentListTemp.Count; i++)
                                {
                                    if (i % classListTemp.Count == 0)
                                    {
                                        b = !b;
                                    }

                                    var classAllotResult = new Basis.Entity.tbClassAllotResult()
                                    {
                                        tbStudent = studentListTemp[i].tbStudent,
                                        Score = studentListTemp[i].Score
                                    };

                                    if (b)
                                    {
                                        classAllotResult.tbClassAllotClass = classListTemp[i % classListTemp.Count].p;
                                    }
                                    else
                                    {
                                        var num = (classListTemp.Count - 1) - i % classListTemp.Count;
                                        classAllotResult.tbClassAllotClass = classListTemp[num].p;
                                    }

                                    list.Add(classAllotResult);
                                }
                            }
                        }
                    }
                }

                db.Set<Basis.Entity.tbClassAllotResult>().AddRange(list);

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("分班");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        public ActionResult PushToClass(int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                    .Include(d => d.tbGrade)
                                 where p.tbYear.Id == yearId
                                 select p).ToList();


                var classAllotClassList = db.Table<Basis.Entity.tbClassAllotClass>()
                                                .Where(d => d.tbYear.Id == yearId)
                                                .Include(d => d.tbClassType)
                                                .Include(d => d.tbGrade)
                                                .Include(d => d.tbYear).ToList();

                foreach (var a in classAllotClassList.Select(d => d.tbGrade.Id).Distinct())
                {
                    foreach (var b in classList.Where(d => d.tbGrade.Id == a))
                    {
                        b.IsDeleted = true;
                    }
                }

                var classAllotResultList = db.Table<Basis.Entity.tbClassAllotResult>()
                                            .Include(d => d.tbClassAllotClass)
                                            .Include(d => d.tbStudent).ToList();

                // 添加新分班数据
                var listClass = new List<Basis.Entity.tbClass>();
                var listClassStudent = new List<Basis.Entity.tbClassStudent>();

                foreach (var a in classAllotClassList)
                {
                    var classTemp = new Basis.Entity.tbClass()
                    {
                        No = a.No,
                        ClassName = a.ClassName,
                        tbClassType = a.tbClassType,
                        tbGrade = a.tbGrade,
                        tbYear = a.tbYear
                    };
                    listClass.Add(classTemp);
                }

                foreach (var v in classAllotResultList)
                {
                    var classStudentTemp = new Basis.Entity.tbClassStudent()
                    {
                        No = v.No,
                        tbClass = listClass.Where(d => d.ClassName == v.tbClassAllotClass.ClassName).FirstOrDefault(),
                        tbStudent = v.tbStudent
                    };

                    listClassStudent.Add(classStudentTemp);
                }

                db.Set<Basis.Entity.tbClass>().AddRange(listClass);
                db.Set<Basis.Entity.tbClassStudent>().AddRange(listClassStudent);

                if (db.SaveChanges() > 0)
                {
                    ClearAllot();
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("从分班数据更新班级");
                }

                return Code.MvcHelper.Post(null, Url.Action("List", "Class"), "操作成功!");
            }
        }

        public int ClearAllot()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var classList = db.Table<Entity.tbClassAllotClass>();
                var studentList = db.Table<Entity.tbClassAllotStudent>();
                var resultList = db.Table<Entity.tbClassAllotResult>();
                if (classList.Count() == 0 && studentList.Count() == 0 && resultList.Count() == 0)
                {
                    return 1;
                }
                foreach (var v in classList)
                {
                    v.IsDeleted = true;
                }
                foreach (var v in studentList)
                {
                    v.IsDeleted = true;
                }
                foreach (var v in resultList)
                {
                    v.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}