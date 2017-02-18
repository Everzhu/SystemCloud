using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveOrgStudentController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveOrgStudent.List();
                vm.ElectiveOrgName = db.Set<Entity.tbElectiveOrg>().Find(vm.ElectiveOrgId).OrgName;

                var tb = from p in db.Table<Entity.tbElectiveOrgStudent>()
                         where p.tbElectiveOrg.Id == vm.ElectiveOrgId && p.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }

                vm.ElectiveOrgStudentList = (from p in tb
                                             orderby p.tbElectiveOrg.OrgName
                                             select new Dto.ElectiveOrgStudent.List
                                             {
                                                 Id = p.Id,
                                                 ElectiveOrgName = p.tbElectiveOrg.OrgName,
                                                 StudentCode = p.tbStudent.StudentCode,
                                                 StudentName = p.tbStudent.StudentName,
                                                 IsFixed = p.IsFixed,
                                                 IsChecked = p.IsChecked
                                             }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveOrgStudent.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, electiveId = vm.ElectiveId, electiveOrgId = vm.ElectiveOrgId }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids, int electiveId, int electiveOrgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {

                var tb = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                          where ids.Contains(p.Id)
                          select new
                          {
                              p,
                              p.tbElectiveOrg,
                              p.tbStudent
                          }).ToList();

                foreach (var a in tb)
                {
                    a.p.IsDeleted = true;
                }

                var orgIds = tb.Select(p => p.tbElectiveOrg.Id).ToList();
                var studentIds = tb.Select(p => p.tbStudent.Id).ToList();

                //var tbElectiveDataList = (from p in db.Table<Entity.tbElectiveData>()
                //                          where orgIds.Contains(p.tbElectiveOrg.Id) && studentIds.Contains(p.tbStudent.Id)
                //                          select p);

                var tbElectiveDataList = (from p in db.Table<Entity.tbElectiveData>()
                                          join c in db.Table< Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals c.tbStudent.Id
                                          where orgIds.Contains(p.tbElectiveOrg.Id) && studentIds.Contains(p.tbStudent.Id)
                                          select new {
                                              tbElectiveData=p,
                                              tbClass=c.tbClass
                                            });

                var limitOrgList = (from p in db.Table<Entity.tbElectiveOrgClass>().Include(p => p.tbClass).Include(p => p.tbElectiveOrg)
                                    where p.tbElectiveOrg.Id == electiveOrgId
                                      && p.tbElectiveOrg.IsDeleted == false
                                    select p).ToList();

                foreach (var item in tbElectiveDataList)
                {
                    item.tbElectiveData.IsDeleted = true;
                    item.tbElectiveData.tbElectiveOrg.RemainCount++;

                    //删除预选学生时，恢复行政班剩余人数（如果有行政班人数限制的话）
                    var limitOrg = limitOrgList.Where(p => p.tbElectiveOrg.Id == item.tbElectiveData.tbElectiveOrg.Id && p.tbClass.Id == item.tbClass.Id).FirstOrDefault();
                    if (limitOrg != null)
                    {
                        limitOrg.RemainCount++;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了预选学生");
                    //SaveToElectiveData(db, electiveId);
                }
            }
            return Code.MvcHelper.Post(null, Url.Action("List", new { electiveId = electiveId, electiveOrgId = electiveOrgId }));

        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveOrgStudent.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                              where p.Id == id
                              select new Dto.ElectiveOrgStudent.Edit
                              {
                                  Id = p.Id,
                                  ElectiveOrgId = p.tbElectiveOrg.Id,
                                  ClassId = p.tbStudent.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ElectiveOrgStudentEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ElectiveOrgStudent.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ElectiveOrgStudentEdit.Id == 0)
                    {
                        var tb = new Entity.tbElectiveOrgStudent();
                        tb.tbElectiveOrg = db.Set<Entity.tbElectiveOrg>().Find(vm.ElectiveOrgStudentEdit.ElectiveOrgId);
                        //tb.tbStudent = db.Set<Basis.Entity.tbClass>().Find(vm.ElectiveOrgStudentEdit.ClassId);
                        db.Set<Entity.tbElectiveOrgStudent>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了选课班级");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                                  where p.Id == vm.ElectiveOrgStudentEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了选课班级");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }


        /// <summary>
        /// 预选学生
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="orgId"></param>
        /// <param name="electiveId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(List<int> ids, int orgId, int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                var electiveClassIdList = db.Table<Entity.tbElectiveClass>().Where(p => p.tbElective.Id == electiveId && p.IsDeleted == false).Select(p => p.tbClass.Id).ToList();

                if (electiveClassIdList != null && !electiveClassIdList.Any())
                {
                    error.AddError("当前选课设置不完整，尚未设置适用班级！");
                    return Code.MvcHelper.Post(error);
                }

                //已预选学生列表
                var orgStudentList = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                                      where p.tbElectiveOrg.Id == orgId
                                      select p.tbStudent.Id).ToList();

                var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                   join c in db.Table< Basis.Entity.tbClassStudent>() on p.Id equals c.tbStudent.Id
                                   where ids.Contains(p.Id) && orgStudentList.Contains(p.Id) == false
                                   select new {
                                       tbStudent=p,
                                       tbClass=c.tbClass
                                   }).ToList();

                //移除班级不符合条件的学生
                //studentList.RemoveAll(p => !db.Table<Basis.Entity.tbClassStudent>().Where(c => c.IsDeleted == false && electiveClassIdList.Contains(c.tbClass.Id)).Select(c => c.tbStudent.Id).Contains(p.tbStudent.Id));
                studentList.RemoveAll(p => !electiveClassIdList.Contains(p.tbClass.Id));

                //移除已经选过该课程的学生
                studentList.RemoveAll(p => db.Table<Entity.tbElectiveData>().Any(e => e.tbStudent.Id == p.tbStudent.Id && e.tbElectiveOrg.Id == orgId && e.IsDeleted == false));

                if (!studentList.Any())
                {
                    error.AddError("所选学生列表中有数据不符合当前选课的要求！");
                    return Code.MvcHelper.Post(error);
                }

                var electiveOrg = db.Set<Entity.tbElectiveOrg>().Where(p => p.Id == orgId && p.IsDeleted == false).FirstOrDefault();
                if (electiveOrg.RemainCount < studentList.Count)
                {
                    error.AddError("课程剩余人数不足！");
                    return Code.MvcHelper.Post(error);
                }


                var studentCountByClass = (from p in studentList
                                           group p by new { p.tbClass.Id, p.tbClass.ClassName } into g
                          select new
                          {
                              ClassId = g.Key.Id,
                              ClassName=g.Key.ClassName,
                              StudentCount = g.Count()
                          });

                var limitOrgList = (from p in db.Table<Entity.tbElectiveOrgClass>().Include(p=>p.tbClass).Include(p=>p.tbElectiveOrg)
                                    where p.tbElectiveOrg.Id == orgId
                                      && p.tbElectiveOrg.IsDeleted == false
                                    select p).ToList();

                foreach (var item in studentCountByClass)
                {
                    if (limitOrgList.Exists(p => p.tbClass.Id == item.ClassId && p.RemainCount < item.StudentCount))
                    {
                        error.AddError($"针对当前行政班级{item.ClassName}的人数限制不足！");
                        return Code.MvcHelper.Post(error);
                    }
                }


                foreach (var student in studentList)
                {
                    var tb = new Entity.tbElectiveOrgStudent()
                    {
                        tbElectiveOrg = db.Set<Entity.tbElectiveOrg>().Find(orgId),
                        tbStudent = student.tbStudent,
                        IsFixed = true,
                        IsChecked = true,
                    };
                    db.Set<Entity.tbElectiveOrgStudent>().Add(tb);

                    var tbElectiveData = new Entity.tbElectiveData()
                    {
                        tbElectiveOrg = tb.tbElectiveOrg,
                        tbStudent = tb.tbStudent,
                        IsPreElective = true,
                        IsFixed = tb.IsFixed,
                        InputDate = DateTime.Now
                    };
                    db.Set<Entity.tbElectiveData>().Add(tbElectiveData);

                    //预选，减少对应的选课行政班人数
                    var limitClass = limitOrgList.Where(p => p.tbElectiveOrg.Id== orgId && p.tbClass.Id==student.tbClass.Id).FirstOrDefault();
                    if (limitClass != null)
                    {
                        limitClass.RemainCount--;
                    }
                }
                electiveOrg.RemainCount -= studentList.Count;       

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了选课预选学生!");
                }
                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchFixed(List<int> ids, int electiveId, int electiveOrgId, bool isFixed)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                            .Include(d => d.tbElectiveOrg)
                            .Include(d => d.tbStudent)
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsFixed = isFixed;

                    var dataList = db.Table<Entity.tbElectiveData>().Where(d => d.tbElectiveOrg.Id == a.tbElectiveOrg.Id && d.tbStudent.Id == a.tbStudent.Id).ToList();
                    foreach (var data in dataList)
                    {
                        data.IsFixed = isFixed;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("切换了预选学生是否允许修改!");
                }

                return Code.MvcHelper.Post(null, Url.Action("List", new { electiveId = electiveId, electiveOrgId = electiveOrgId }), "操作成功!");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchChecked(List<int> ids, int electiveId, int electiveOrgId, bool isChecked)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                            .Include(d => d.tbElectiveOrg)
                            .Include(d => d.tbStudent)
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsChecked = isChecked;

                    if (isChecked)
                    {
                        if (db.Table<Entity.tbElectiveData>().Where(d => d.tbElectiveOrg.Id == a.tbElectiveOrg.Id && d.tbStudent.Id == a.tbStudent.Id).Count() == 0)
                        {
                            var tbElectiveData = new Entity.tbElectiveData()
                            {
                                tbElectiveOrg = a.tbElectiveOrg,
                                tbStudent = a.tbStudent,
                                IsPreElective = true,
                                IsFixed = a.IsFixed,
                                InputDate = DateTime.Now
                            };
                            db.Set<Entity.tbElectiveData>().Add(tbElectiveData);
                            a.tbElectiveOrg.RemainCount--;
                        }
                    }
                    else
                    {
                        var dataList = db.Table<Entity.tbElectiveData>().Where(d => d.tbElectiveOrg.Id == a.tbElectiveOrg.Id && d.tbStudent.Id == a.tbStudent.Id).ToList();
                        foreach (var data in dataList)
                        {
                            data.IsDeleted = true;
                            a.tbElectiveOrg.RemainCount++;
                        }
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("切换了预选学生是否选中!");
                }

                return Code.MvcHelper.Post(null, Url.Action("List", new { electiveId = electiveId, electiveOrgId = electiveOrgId }), "操作成功!");
            }
        }

        public ActionResult Import()
        {
            var vm = new Models.ElectiveOrgStudent.Import();
            return View(vm);
        }

        [HttpPost]
        public ActionResult Import(Models.ElectiveOrgStudent.Import vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

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
                        var tbList = new List<string>() { "选课班级", "学号", "姓名", "允许学生修改", "默认选中" };
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

                        var electiveOrgList = (from p in db.Table<Entity.tbElectiveOrg>()
                                                .Include(d => d.tbElective)
                                               where p.tbElective.Id == vm.ElectiveId
                                                && p.tbCourse.IsDeleted == false
                                                && p.tbElectiveGroup.IsDeleted == false
                                                && p.tbElectiveSection.IsDeleted == false
                                               select p).ToList();

                        var studentList = (from p in db.Table<Student.Entity.tbStudent>()
                                           join s in db.Table<Basis.Entity.tbClassStudent>() 
                                           on p.Id equals s.tbStudent.Id
                                           select new
                                           {
                                               p.StudentCode,
                                               p.StudentName,
                                               StudentId = p.Id,
                                               ClassId = s.tbClass.Id
                                           }).ToList();

                        var electiveClassIds = db.Set<Entity.tbElectiveClass>().Where(p => p.tbElective.Id == vm.ElectiveId && p.IsDeleted == false).Select(p => p.tbClass.Id).ToList();

                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var dr = dt.Rows[i];
                            var import = new Dto.ElectiveOrgStudent.Import()
                            {
                                OrgName = dr["选课班级"].ToString().Trim(),
                                StudentCode = dr["学号"].ToString().Trim(),
                                StudentName = dr["姓名"].ToString().Trim(),
                                IsFixed = dr["允许学生修改"].ToString().Trim(),
                                IsChecked = dr["默认选中"].ToString().Trim()
                            };

                            if (string.IsNullOrEmpty(import.OrgName))
                            {
                                import.Error += "选课班级不能为空!";
                            }

                            if (string.IsNullOrEmpty(import.StudentCode))
                            {
                                import.Error += "学号不能为空!";
                            }

                            if (string.IsNullOrEmpty(import.StudentName))
                            {
                                import.Error += "姓名不能为空!";
                            }

                            var student = studentList.Where(p => p.StudentCode == import.StudentCode).FirstOrDefault();
                            if (student == null)
                            {
                                import.Error += "学号不存在，或者学生尚未加入班级!";
                            }
                            else if (student.StudentName != import.StudentName)
                            {
                                import.Error += "学号与姓名不匹配!";
                            }

                            if (string.IsNullOrEmpty(import.IsFixed))
                            {
                                import.Error += "允许学生修改不能为空!";
                            }
                            else
                            {
                                if ((new string[] { "是", "否" }).Contains(import.IsFixed) == false)
                                {
                                    import.Error += "允许学生修改只能填写是或否!";
                                }
                            }

                            if (string.IsNullOrEmpty(import.IsChecked))
                            {
                                import.Error += "默认选中不能为空!";
                            }
                            else
                            {
                                if ((new string[] { "是", "否" }).Contains(import.IsChecked) == false)
                                {
                                    import.Error += "默认选中只能填写是或否!";
                                }
                            }

                            if (!electiveOrgList.Any(p => p.OrgName == import.OrgName))
                            {
                                import.Error += "找不到对应的选课班级!";
                            }

                            if (student != null)
                            {
                                var count = db.Table<Entity.tbElectiveData>().Where(p => p.tbElectiveOrg.OrgName == import.OrgName && p.tbElectiveOrg.IsDeleted == false && p.tbElectiveOrg.tbElective.Id == vm.ElectiveId && p.tbStudent.Id == student.StudentId).Count();
                                if (count > 0)
                                {
                                    import.Error += "系统已存在该学生的选课记录!";
                                }


                                var classLimit = db.Table<Entity.tbElectiveOrgClass>().Where(p => p.tbElectiveOrg.OrgName == import.OrgName && p.tbElectiveOrg.IsDeleted == false && p.tbElectiveOrg.tbElective.Id == vm.ElectiveId).Select(p => p.tbClass.Id).ToList();
                                if (classLimit != null && !classLimit.Contains(student.ClassId))
                                {
                                    import.Error += "学生所在班级被限制预选该课程!";
                                }
                                
                                if (electiveClassIds != null && !electiveClassIds.Contains(student.ClassId))
                                {
                                    import.Error += "学生所在班级不能预选该课程!";
                                }
                            }
                            vm.ImportList.Add(import);
                        }

                        vm.ImportList.RemoveAll(p => string.IsNullOrEmpty(p.OrgName + p.StudentCode + p.StudentName + p.IsFixed));


                        if (vm.ImportList.GroupBy(p => new { p.StudentCode, p.OrgName }).Select(p => p.Count()).First() > 1)
                        {
                            vm.ImportList.ForEach(p =>
                            {
                                vm.ImportList.ForEach(p1 => p1.Error += p1.StudentCode + "|" + p1.OrgName == p.StudentCode + "|" + p.OrgName ? "该条数据重复!" : "");
                            });
                        }

                        if (vm.ImportList.Count(p => string.IsNullOrEmpty(p.Error) == false) > 0)
                        {
                            vm.ImportList.RemoveAll(p => string.IsNullOrEmpty(p.Error));
                            return View(vm);
                        }



                        var limitElectiveOrgList = (from p in db.Table<Entity.tbElectiveOrgClass>().Include(p => p.tbClass).Include(p => p.tbElectiveOrg)
                                            where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                              && p.tbElectiveOrg.IsDeleted == false
                                            select p).ToList();

                        /*
                        foreach (var item in studentCountByClass)
                        {
                            if (limitOrgList.Exists(p => p.tbClass.Id == item.ClassId && p.RemainCount < item.StudentCount))
                            {
                                error.AddError($"针对当前行政班级{item.ClassName}的人数限制不足！");
                                return Code.MvcHelper.Post(error);
                            }
                        }
                        */

                        vm.ImportList.ForEach(p => {
                            p.ClassId = studentList.Where(s => s.StudentCode.Equals(p.StudentCode)).Select(s => s.ClassId).FirstOrDefault();
                        });

                        foreach (var org in vm.ImportList)
                        {
                          
                            var electiveOrg =electiveOrgList.Where(p => p.OrgName.Equals(org.OrgName) && p.tbElective.Id == vm.ElectiveId && p.IsDeleted == false)
                                .Select(p => new
                                {
                                    p.MaxCount,
                                    p.RemainCount
                                }).FirstOrDefault();

                            //本次excel导入数据中选择同一班级的人数
                            var studentCount = vm.ImportList.Count(p => p.OrgName.Equals(org.OrgName) && p.IsChecked == "是");
                            if (studentCount > electiveOrg.RemainCount)
                            {
                                org.Error += "课程剩余人数不足！";
                            }

                            //判断选课班级对学生当前班级有没有限制
                            var limitInfo = (from p in limitElectiveOrgList where p.tbElectiveOrg.OrgName.Equals(org.OrgName) && p.tbClass.Id == org.ClassId select p).FirstOrDefault();
                            if (limitInfo != null)
                            {
                                var classStudentCount = vm.ImportList.Count(p => p.ClassId == org.ClassId);
                                //判断选课班级对当前学生所在班级的限制人数
                                if (classStudentCount>limitInfo.RemainCount)
                                {
                                    org.Error += "针对当前行政班级的人数限制不足！";
                                }
                            }

                          
                        }

                        if (vm.ImportList.Count(p => string.IsNullOrEmpty(p.Error) == false) > 0)
                        {
                            vm.ImportList.RemoveAll(p => string.IsNullOrEmpty(p.Error));
                            return View(vm);
                        }                     
                        
                        //分组分段

                        foreach (var item in vm.ImportList)
                        {
                            var tbElectiveOrg = electiveOrgList.Where(d => d.OrgName == item.OrgName).FirstOrDefault();
                            var tb = new Entity.tbElectiveOrgStudent
                            {
                                tbElectiveOrg = tbElectiveOrg,
                                tbStudent = db.Set<Student.Entity.tbStudent>().Find(studentList.Where(p => p.StudentCode == item.StudentCode && p.StudentName == item.StudentName).First().StudentId),
                                IsFixed = item.IsFixed == "是" ? false : true,
                                IsChecked = item.IsChecked == "是" ? true : false
                            };
                            
                            db.Set<Entity.tbElectiveOrgStudent>().Add(tb);

                            if (item.IsChecked == "是")
                            {
                                var tbElectiveData = new Entity.tbElectiveData()
                                {
                                    tbElectiveOrg = tb.tbElectiveOrg,
                                    tbStudent = tb.tbStudent,
                                    IsPreElective = true,
                                    IsFixed = tb.IsFixed,
                                    InputDate = DateTime.Now
                                };
                                db.Set<Entity.tbElectiveData>().Add(tbElectiveData);
                                tbElectiveOrg.RemainCount--;

                                //更新行政班剩余人数
                                var limit = (from p in limitElectiveOrgList where p.tbElectiveOrg.Id == tbElectiveOrg.Id && p.tbClass.Id == item.ClassId select p).FirstOrDefault();
                                if (limit != null)
                                {
                                    limit.RemainCount--;
                                }

                            }
                        }

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入了预选学生!");
                            vm.ImportList.RemoveAll(p => string.IsNullOrEmpty(p.Error));
                            vm.Status = true;
                        }

                    }
                }
            }

            return View(vm);
        }


        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Elective/Views/ElectiveOrgStudent/OrgStudentTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult Export(Models.ElectiveOrg.Import vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                var listElectiveOrg = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                                       where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbElectiveOrg.IsDeleted == false
                                        && p.tbElectiveOrg.tbCourse.IsDeleted == false
                                        && p.tbElectiveOrg.tbElectiveGroup.IsDeleted == false
                                        && p.tbElectiveOrg.tbElectiveSection.IsDeleted == false
                                       select new Dto.ElectiveOrgStudent.List
                                       {
                                           ElectiveOrgName = p.tbElectiveOrg.OrgName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName,
                                           IsFixed = false
                                       }).ToList();
                var dataTable = new System.Data.DataTable();
                var listColumnNames = new List<string> { "选课班级", "学号", "姓名", "允许学生修改" };
                dataTable.Columns.AddRange(listColumnNames.Select(p => new System.Data.DataColumn(p)).ToArray());

                listElectiveOrg.ForEach(p =>
                {
                    var dataRow = dataTable.NewRow();
                    dataRow["选课班级"] = p.ElectiveOrgName;
                    dataRow["学号"] = p.StudentCode;
                    dataRow["姓名"] = p.StudentName;
                    dataRow["允许学生修改"] = p.IsFixed;
                    dataTable.Rows.Add(dataRow);
                });

                Code.NpoiHelper.DataTableToExcel(file, dataTable);
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

        public static string SaveToElectiveData(XkSystem.Models.DbContext db, int electiveId)
        {

            //预选记录
            var dataList = (from p in db.Table<Entity.tbElectiveData>()
                                .Include(d => d.tbElectiveOrg)
                                .Include(d => d.tbStudent)
                            where p.tbElectiveOrg.tbElective.Id == electiveId
                                && p.IsPreElective == true
                            select p).ToList();

            var orgStudent = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                                .Include(d => d.tbElectiveOrg)
                                .Include(d => d.tbStudent)
                              where p.tbElectiveOrg.tbElective.Id == electiveId && p.IsDeleted == false
                              select p).ToList();

            //删除已经不在此次导入预选学生列表的选课记录
            foreach (var a in dataList.Where(d => orgStudent.Where(o => o.tbElectiveOrg.Id == d.tbElectiveOrg.Id && o.tbStudent.Id == d.tbStudent.Id).Count() == 0))
            {
                a.IsDeleted = true;
            }

            //修改在预选学生名单的选课记录
            foreach (var a in dataList.Where(d => orgStudent.Where(o => o.tbElectiveOrg.Id == d.tbElectiveOrg.Id && o.tbStudent.Id == d.tbStudent.Id).Count() > 0))
            {
                a.IsPreElective = a.IsPreElective;
                a.IsFixed = orgStudent.Where(d => d.tbElectiveOrg.Id == a.tbElectiveOrg.Id && d.tbStudent.Id == d.tbStudent.Id).FirstOrDefault().IsFixed;
            }

            //增加没有添加记录的预选学生名单
            var list = new List<Entity.tbElectiveData>();
            foreach (var a in orgStudent.Where(d => dataList.Where(o => o.tbElectiveOrg.Id == d.tbElectiveOrg.Id && o.tbStudent.Id == d.tbStudent.Id).Count() == 0))
            {
                var data = new Entity.tbElectiveData();
                data.tbElectiveOrg = a.tbElectiveOrg;
                data.tbStudent = a.tbStudent;
                data.IsPreElective = true;
                data.IsFixed = a.IsFixed;
                data.InputDate = DateTime.Now;
                list.Add(data);
            }
            db.Set<Entity.tbElectiveData>().AddRange(list);

            db.SaveChanges();

            //重新计算剩余名额
            var dataCountList = (from p in db.Table<Entity.tbElectiveData>()
                                 where p.tbElectiveOrg.tbElective.Id == electiveId
                                     && p.tbStudent.IsDeleted == false
                                 group p by p.tbElectiveOrg.Id into g
                                 select new
                                 {
                                     OrgId = g.Key,
                                     iCount = g.Count()
                                 }).ToList();

            var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                      where p.tbElective.Id == electiveId
                      select p).ToList();
            foreach (var a in tb)
            {
                a.RemainCount = a.MaxCount - dataCountList.Where(d => d.OrgId == a.Id).Select(d => d.iCount).DefaultIfEmpty().FirstOrDefault();
            }

            db.SaveChanges();

            return string.Empty;
        }
    }
}