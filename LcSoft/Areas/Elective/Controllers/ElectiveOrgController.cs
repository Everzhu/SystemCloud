using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveOrgController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveOrg.List();
                vm.ElectiveName = db.Table<Entity.tbElective>().FirstOrDefault(d => d.Id == vm.ElectiveId).ElectiveName;
                vm.ElectiveSectionList = Controllers.ElectiveSectionController.SelectList(vm.ElectiveId);
                vm.ElectiveGroupList = Controllers.ElectiveGroupController.SelectList(vm.ElectiveId);

                vm.IsWeekPeriod = (from p in db.Set<Entity.tbElective>()
                                   where p.Id == vm.ElectiveId
                                   select p.tbElectiveType.ElectiveTypeCode).FirstOrDefault() == Code.EnumHelper.ElectiveType.WeekPeriod;

                var tb = from p in db.Table<Entity.tbElectiveOrg>()
                         where p.tbElective.Id == vm.ElectiveId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.OrgName.Contains(vm.SearchText));
                }

                if (vm.ElectiveSectionId != 0)
                {
                    tb = tb.Where(d => d.tbElectiveSection.Id == vm.ElectiveSectionId);
                }

                if (vm.ElectiveGroupId != 0)
                {
                    tb = tb.Where(d => d.tbElectiveGroup.Id == vm.ElectiveGroupId);
                }

                vm.ElectiveOrgList = (from p in tb
                                      orderby p.No, p.OrgName
                                      select new Dto.ElectiveOrg.List
                                      {
                                          Id = p.Id,
                                          CourseName = p.tbCourse.CourseName,
                                          ElectiveSectionName = p.tbElectiveSection.ElectiveSectionName,
                                          ElectiveGroupName = p.tbElectiveGroup.ElectiveGroupName,
                                          MaxCount = p.MaxCount,
                                          RemainCount = p.RemainCount,
                                          No = p.No,
                                          OrgName = p.OrgName,
                                          RoomName = p.tbRoom.RoomName,
                                          TeacherName = p.tbTeacher.TeacherName,
                                          Permit = p.Permit,
                                          IsPermitClass = p.IsPermitClass
                                      }).ToList();
                var orgStudentList = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                                      where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                      group p by p.tbElectiveOrg.Id into g
                                      select new
                                      {
                                          OrgId = g.Key,
                                          StudentCount = g.Count()
                                      }).ToList();

                var limitClassList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                      join q in db.Table<Entity.tbElectiveClass>() on p.tbClass.Id equals q.tbClass.Id
                                      where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                        && p.tbClass.tbGrade.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbElectiveOrg.IsDeleted == false
                                        && p.tbElectiveOrg.tbElective.Id == q.tbElective.Id
                                      select new
                                      {
                                          OrgId = p.tbElectiveOrg.Id,
                                          GradeNo = p.tbClass.tbGrade.No,
                                          ClassNo = p.tbClass.No,
                                          p.tbClass.ClassName,
                                          ClassId = p.tbClass.Id
                                      }).ToList();

                var orgScheduleList = (from p in db.Table<Entity.tbElectiveOrgSchedule>()
                                       where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                       select new
                                       {
                                           OrgId = p.tbElectiveOrg.Id,
                                           p.tbWeek.WeekName,
                                           p.tbPeriod.PeriodName
                                       }).ToList();

                vm.ElectiveOrgList.ForEach(s =>
                {
                    s.WeekPeriod = string.Join(",", orgScheduleList.Where(o => o.OrgId == s.Id).Select(o => o.WeekName + o.PeriodName).ToList());
                    s.OrgStudentCount = orgStudentList.Where(d => d.OrgId == s.Id).Select(d => d.StudentCount).DefaultIfEmpty().FirstOrDefault();

                    /*
                        "全部"分两种情况，
                            1、IsPremitClass=false;
                            2、IsPremitClass=true，但限制班级数和当前选课班级数一致，即所有班级都做人数限制（数量可能有变更，不一定全是默认的999）
                    */

                    if (s.IsPermitClass == false)
                    {
                        s.LimitClassName = "全部";
                    }
                    else
                    {
                        var myClassList = limitClassList.Where(d => d.OrgId == s.Id).OrderBy(o => o.GradeNo).ThenBy(d => d.ClassNo).ThenBy(d => d.ClassName).Select(c => c.ClassName).ToList();
                        var electiveClassCount = (from p in db.Table<Entity.tbElectiveClass>() where p.tbElective.Id == vm.ElectiveId select p.Id).Count();
                        if (electiveClassCount == myClassList.Count)
                        {
                            s.LimitClassCount = electiveClassCount;
                            s.LimitClassName = "全部";
                        }
                        else
                        {
                            s.LimitClassCount = myClassList.Count;
                            s.LimitClassName = string.Join("<br/>", myClassList);
                        }
                    }
                    //s.LimitClassCount = myClassList.Count;
                    //s.LimitClassName = string.Join("<br/>", myClassList);
                    //}
                });

                if (vm.IsWeekPeriod)
                {
                    vm.IsHiddenSection = true;
                    vm.IsHiddenGroup = true;
                }
                else
                {
                    vm.IsHiddenSection = vm.ElectiveOrgList.Select(d => d.ElectiveSectionName).Distinct().Count() <= 1;
                    vm.IsHiddenGroup = vm.ElectiveOrgList.Select(d => d.ElectiveGroupName).Distinct().Count() <= 1;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveOrg.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, electiveSectionId = vm.ElectiveSectionId, electiveGroupId = vm.ElectiveGroupId, electiveId = vm.ElectiveId }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var electiveOrgClassList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                                .Include(d => d.tbElectiveOrg)
                                            where ids.Contains(p.tbElectiveOrg.Id)
                                            select p).ToList();

                var electiveOrgStudentList = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                                                .Include(d => d.tbElectiveOrg)
                                              where ids.Contains(p.tbElectiveOrg.Id)
                                              select p).ToList();

                var electiveOrgScheduleList = (from p in db.Table<Entity.tbElectiveOrgSchedule>()
                                                .Include(d => d.tbElectiveOrg)
                                               where ids.Contains(p.tbElectiveOrg.Id)
                                               select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    foreach (var cla in electiveOrgClassList.Where(d => d.tbElectiveOrg.Id == a.Id))
                    {
                        cla.IsDeleted = true;
                    }

                    foreach (var student in electiveOrgStudentList.Where(d => d.tbElectiveOrg.Id == a.Id))
                    {
                        student.IsDeleted = true;
                    }

                    foreach (var schedule in electiveOrgScheduleList.Where(d => d.tbElectiveOrg.Id == a.Id))
                    {
                        schedule.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了选课开班");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveOrg.Edit();
                vm.TeacherList = Teacher.Controllers.TeacherController.SelectList();
                vm.RoomList = Basis.Controllers.RoomController.SelectList();
                vm.ElectiveGroupList = Elective.Controllers.ElectiveGroupController.SelectList(vm.ElectiveId);
                vm.ElectiveSectionList = Elective.Controllers.ElectiveSectionController.SelectList(vm.ElectiveId);
                vm.SubjectList = ElectiveSubjectController.SelectSubjectList(vm.ElectiveId);

                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                              where p.Id == id
                              select new Dto.ElectiveOrg.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  CourseId = p.tbCourse.Id,
                                  SubjectId = p.tbCourse.tbSubject.Id,
                                  ElectiveId = p.tbElective.Id,
                                  MaxCount = p.MaxCount,
                                  OrgName = p.OrgName,
                                  RoomId = p.tbRoom.Id,
                                  TeacherId = p.tbTeacher.Id,
                                  Permit = p.Permit,
                                  ElectiveGroupId = p.tbElectiveGroup.Id,
                                  ElectiveSectionId = p.tbElectiveSection.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ElectiveOrgEdit = tb;
                        //vm.CourseList = Course.Controllers.CourseController.SelectListForElectiveOrg(vm.ElectiveId);
                        vm.CourseList = Course.Controllers.CourseController.SelectList(vm.ElectiveOrgEdit.SubjectId);
                    }
                }

                if (vm.ElectiveOrgEdit != null && !vm.ElectiveOrgEdit.No.HasValue)
                {
                    vm.ElectiveOrgEdit.No = (from p in db.Table<Entity.tbElectiveOrg>() where p.tbElective.Id == vm.ElectiveId select p.No.HasValue ? p.No.Value : 0).DefaultIfEmpty().Max() + 1;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ElectiveOrg.Edit vm)
        {
            var ids = Request["cBox"];
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var isExists = db.Table<Entity.tbElectiveOrg>().Count(p => p.OrgName == vm.ElectiveOrgEdit.OrgName && p.tbElective.Id == vm.ElectiveId && p.Id != vm.ElectiveOrgEdit.Id) > 0;
                    if (isExists)
                    {
                        error.AddError("已存在相同名字的选课开班记录!");
                    }
                    else
                    {
                        if (vm.ElectiveOrgEdit.Id == 0)
                        {
                            var tb = new Entity.tbElectiveOrg();
                            tb.No = vm.ElectiveOrgEdit.No.HasValue ? vm.ElectiveOrgEdit.No.Value : db.Table<Entity.tbElectiveOrg>().Select(d => d.No.HasValue ? d.No.Value : 0).DefaultIfEmpty().Max() + 1;
                            tb.MaxCount = vm.ElectiveOrgEdit.MaxCount; //< preStudentIds.Count ? preStudentIds.Count : vm.ElectiveOrgEdit.MaxCount,  //如果预选人数超过最大人数，将最大人数改成预选人数
                            tb.OrgName = vm.ElectiveOrgEdit.OrgName;
                            tb.RemainCount = vm.ElectiveOrgEdit.MaxCount;  //< preStudentIds.Count ? preStudentIds.Count : vm.ElectiveOrgEdit.MaxCount;
                            tb.tbCourse = db.Set<Course.Entity.tbCourse>().Find(vm.ElectiveOrgEdit.CourseId);
                            tb.tbElectiveGroup = db.Set<Entity.tbElectiveGroup>().Find(vm.ElectiveOrgEdit.ElectiveGroupId);
                            tb.tbElectiveSection = db.Set<Entity.tbElectiveSection>().Find(vm.ElectiveOrgEdit.ElectiveSectionId);
                            tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.ElectiveOrgEdit.TeacherId);
                            tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.ElectiveOrgEdit.RoomId);
                            tb.Permit = vm.ElectiveOrgEdit.Permit;
                            tb.tbElective = db.Set<Entity.tbElective>().Find(vm.ElectiveId);
                            db.Set<Entity.tbElectiveOrg>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了选课开班");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                                      where p.Id == vm.ElectiveOrgEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.ElectiveOrgEdit.No.HasValue ? vm.ElectiveOrgEdit.No.Value : db.Table<Entity.tbElectiveOrg>().Select(d => d.No.HasValue ? d.No.Value : 0).DefaultIfEmpty().Max() + 1;
                                tb.OrgName = vm.ElectiveOrgEdit.OrgName;
                                tb.tbElectiveGroup = db.Set<Entity.tbElectiveGroup>().Find(vm.ElectiveOrgEdit.ElectiveGroupId);
                                tb.tbElectiveSection = db.Set<Entity.tbElectiveSection>().Find(vm.ElectiveOrgEdit.ElectiveSectionId);
                                tb.tbCourse = db.Set<Course.Entity.tbCourse>().Find(vm.ElectiveOrgEdit.CourseId);
                                tb.MaxCount = vm.ElectiveOrgEdit.MaxCount;
                                tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.ElectiveOrgEdit.TeacherId);
                                tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.ElectiveOrgEdit.RoomId);
                                tb.Permit = vm.ElectiveOrgEdit.Permit;

                                //已选人数
                                var count = db.Table<Entity.tbElectiveData>().Count(p => p.tbElectiveOrg.Id == vm.ElectiveOrgEdit.Id && p.IsDeleted == false);
                                tb.RemainCount = vm.ElectiveOrgEdit.MaxCount - count;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了选课开班");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post(error, Url.Action("List", new { ElectiveId = vm.ElectiveId }));
            }
        }

        public ActionResult Import()
        {
            Models.ElectiveOrg.Import vm = new Models.ElectiveOrg.Import();
            return View(vm);
        }

        [HttpPost]
        public ActionResult Import(Models.ElectiveOrg.Import vm)
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
                            var tbList = new List<string>() { "班级名称", "课程名称", "选课分段名称", "选课分组名称", "总人数", "授权模式", "任课教师", "教室", "星期节次", "选课对象" };
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

                            var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();
                            var electiveGroupList = db.Table<Entity.tbElectiveGroup>().Where(d => d.tbElective.Id == vm.ElectiveId).ToList();
                            var electiveSectionList = db.Table<Entity.tbElectiveSection>().Where(d => d.tbElective.Id == vm.ElectiveId).ToList();
                            var electiveOrgClassList = db.Table<Entity.tbElectiveOrgClass>().Where(d => d.tbElectiveOrg.tbElective.Id == vm.ElectiveId).Include(d => d.tbClass).ToList();
                            var courseList = db.Table<Course.Entity.tbCourse>().ToList();
                            var roomList = db.Table<Basis.Entity.tbRoom>().ToList();
                            var weekList = db.Table<Basis.Entity.tbWeek>().ToList();
                            var periodList = db.Table<Basis.Entity.tbPeriod>().ToList();

                            var weekPeriodList = new List<string>();
                            weekList.ForEach(w =>
                            {
                                periodList.ForEach(p =>
                                {
                                    weekPeriodList.Add(w.WeekName + p.PeriodName);
                                });
                            });

                            for (var i = 0; i < dt.Rows.Count; i++)
                            {
                                var dr = dt.Rows[i];
                                var importModel = new Dto.ElectiveOrg.Import()
                                {
                                    OrgName = dr["班级名称"].ToString().Trim(),
                                    CourseName = dr["课程名称"].ToString().Trim(),
                                    ElectiveGroupName = dr["选课分组名称"].ToString().Trim(),
                                    ElectiveSectionName = dr["选课分段名称"].ToString().Trim(),
                                    MaxCount = dr["总人数"].ToString().Trim(),
                                    TeacherName = dr["任课教师"].ToString().Trim(),
                                    RoomName = dr["教室"].ToString().Trim(),
                                    Permit = dr["授权模式"].ToString().Trim(),
                                    OrgSchedule = dr["星期节次"].ToString().Trim(),
                                    LimitClass = dr["选课对象"].ToString().Trim()
                                };

                                if (string.IsNullOrEmpty(importModel.MaxCount))
                                {
                                    importModel.ImportError += "总人数不能为空!";
                                }
                                else
                                {
                                    if (importModel.MaxCount.ConvertToInt() < 0)
                                    {
                                        importModel.ImportError += "总人数必须为正整数。";
                                    }
                                }

                                if (string.IsNullOrEmpty(importModel.OrgName))
                                {
                                    importModel.ImportError += "班级不能为空!";
                                }

                                if (string.IsNullOrEmpty(importModel.CourseName))
                                {
                                    importModel.ImportError += "课程不能为空。";
                                }
                                else if (courseList.Count(p => p.CourseName.Equals(importModel.CourseName)) == 0)
                                {
                                    importModel.ImportError += "系统中找不到对应的课程。";
                                }

                                if (string.IsNullOrEmpty(importModel.ElectiveSectionName))
                                {
                                    importModel.ImportError += "分段不能为空。";
                                }
                                else if (electiveSectionList.Where(d => d.ElectiveSectionName == importModel.ElectiveSectionName).Count() == 0)
                                {
                                    importModel.ImportError += "系统中找不到对应的分段。";
                                }

                                if (string.IsNullOrEmpty(importModel.ElectiveGroupName))
                                {
                                    importModel.ImportError += "分组不能为空。";
                                }
                                else if (electiveGroupList.Count(p => p.ElectiveGroupName.Equals(importModel.ElectiveGroupName)) == 0)
                                {
                                    importModel.ImportError += "系统中找不到对应的分组。";
                                }

                                if (!string.IsNullOrWhiteSpace(importModel.TeacherName) && teacherList.Count(p => p.TeacherName.Equals(importModel.TeacherName)) == 0)
                                {
                                    importModel.ImportError += "系统中找不到对应的教师。";
                                }

                                if (!string.IsNullOrWhiteSpace(importModel.OrgName) && db.Table<Entity.tbElectiveOrg>().Count(p => p.OrgName == importModel.OrgName && p.tbElective.Id == vm.ElectiveId) > 0)
                                {
                                    importModel.IsExists = true;
                                    if (!vm.IsUpdate)
                                    {
                                        importModel.ImportError += "已存在相同班级。";
                                    }
                                }

                                if (string.IsNullOrEmpty(importModel.Permit))
                                {
                                    importModel.ImportError += "授权模式不能为空。";
                                }
                                else if (new string[] { "不限制", "白名单", "黑名单" }.Contains(importModel.Permit) == false)
                                {
                                    importModel.ImportError += "授权模式输入格式不正确。";
                                }

                                if (string.IsNullOrEmpty(importModel.RoomName) == false)
                                {
                                    if (roomList.Where(d => d.RoomName == importModel.RoomName).Count() == 0)
                                    {
                                        importModel.ImportError += "教室不存在。";
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(importModel.OrgSchedule))
                                {
                                    var scheduleS = importModel.OrgSchedule.Split(',').ToList();
                                    foreach (var item in scheduleS)
                                    {
                                        if (!weekPeriodList.Contains(item))
                                        {
                                            importModel.ImportError += "星期节次不正确";
                                            break;
                                        }
                                    }
                                }

                                vm.ImportList.Add(importModel);
                            }

                            vm.ImportList.RemoveAll(p => string.IsNullOrWhiteSpace(p.CourseName + p.ElectiveGroupName + p.ElectiveGroupName + p.MaxCount + p.OrgName + p.TeacherName));

                            if (vm.ImportList.GroupBy(p => p.OrgName).Select(p => p.Count()).First() > 1)
                            {
                                vm.ImportList.ForEach(p =>
                                {
                                    vm.ImportList.ForEach(p1 =>
                                    {
                                        if (p.OrgName.Equals(p1.OrgName))
                                        {
                                            p.ImportError += "班级名称重复!";
                                        }
                                    });
                                });
                            }

                            if (vm.ImportList.Count(p => !string.IsNullOrWhiteSpace(p.ImportError)) > 0)
                            {
                                vm.ImportList.RemoveAll(p => string.IsNullOrWhiteSpace(p.ImportError));
                                return View(vm);
                            }

                            var electiveOrgList = new List<Entity.tbElectiveOrg>();
                            var electiveOrgScheduleList = new List<Entity.tbElectiveOrgSchedule>();
                            var tbElectiveOrgClassList = new List<Entity.tbElectiveOrgClass>();
                            var index = (from p in db.Table<Entity.tbElectiveOrg>() where p.tbElective.Id == vm.ElectiveId select p.No.HasValue ? p.No.Value : 0).DefaultIfEmpty().Max() + 1;
                            foreach (var item in vm.ImportList)
                            {
                                #region tbElectiveOrg
                                Entity.tbElectiveOrg tbElectiveOrg = null;
                                if (item.IsExists)
                                {
                                    //修改
                                    tbElectiveOrg = db.Table<Entity.tbElectiveOrg>().FirstOrDefault(p => p.OrgName.Equals(item.OrgName));

                                    if (tbElectiveOrg.MaxCount != item.MaxCount.ConvertToInt())
                                    {
                                        //重新计算最大和剩余人数
                                        var existsNum = db.Table<Entity.tbElectiveData>().Count(p => p.tbElectiveOrg.Id == tbElectiveOrg.Id);
                                        if (item.MaxCount.ConvertToInt() < existsNum)
                                        {
                                            tbElectiveOrg.MaxCount = existsNum;
                                            tbElectiveOrg.RemainCount = 0;
                                        }
                                        else
                                        {
                                            tbElectiveOrg.MaxCount = item.MaxCount.ConvertToInt();
                                            tbElectiveOrg.RemainCount = item.MaxCount.ConvertToInt() - existsNum;
                                        }
                                    }
                                    if (!tbElectiveOrg.No.HasValue)
                                    {
                                        tbElectiveOrg.No = (from p in db.Table<Entity.tbElectiveOrg>() where p.tbElective.Id == vm.ElectiveId select p.No.HasValue ? p.No.Value : 0).DefaultIfEmpty().Max() + 1;
                                    }
                                }
                                else
                                {
                                    //新建
                                    tbElectiveOrg = new Entity.tbElectiveOrg();
                                    tbElectiveOrg.OrgName = item.OrgName;
                                    tbElectiveOrg.tbElective = db.Set<Entity.tbElective>().Find(vm.ElectiveId);
                                    tbElectiveOrg.MaxCount = item.MaxCount.ConvertToInt();
                                    tbElectiveOrg.RemainCount = item.MaxCount.ConvertToInt();
                                    tbElectiveOrg.No = index;
                                    index++;
                                }
                                tbElectiveOrg.tbCourse = courseList.Where(d => d.CourseName == item.CourseName).FirstOrDefault();
                                tbElectiveOrg.tbElectiveSection = electiveSectionList.Where(d => d.ElectiveSectionName == item.ElectiveSectionName).FirstOrDefault();
                                tbElectiveOrg.tbElectiveGroup = electiveGroupList.Where(d => d.ElectiveGroupName == item.ElectiveGroupName).FirstOrDefault();
                                tbElectiveOrg.tbTeacher = teacherList.Where(d => d.TeacherName == item.TeacherName).FirstOrDefault();
                                tbElectiveOrg.tbRoom = roomList.Where(d => d.RoomName == item.RoomName).FirstOrDefault();
                                tbElectiveOrg.Permit = item.Permit == "白名单" ? 1 : (item.Permit == "黑名单" ? -1 : 0);

                                if (!item.IsExists)
                                {
                                    db.Set<Entity.tbElectiveOrg>().Add(tbElectiveOrg);
                                }
                                #endregion

                                #region tbElectiveOrgSchedule
                                if (!string.IsNullOrWhiteSpace(item.OrgSchedule))
                                {
                                    var orgList = item.OrgSchedule.Split(',').ToList();
                                    if (item.IsExists)
                                    {
                                        //已存在，删除原来的数据
                                        var orgScheduleList = (from p in db.Table<Entity.tbElectiveOrgSchedule>()
                                                               where p.tbElectiveOrg.Id == tbElectiveOrg.Id &&
                                                                   orgList.Contains(p.tbWeek.WeekName + p.tbPeriod.PeriodName)
                                                               select p);
                                        foreach (var orgSchedule in orgScheduleList)
                                        {
                                            orgSchedule.IsDeleted = true;
                                        }
                                    }

                                    foreach (var org in orgList)
                                    {
                                        var tbElectiveOrgSchedule = new Entity.tbElectiveOrgSchedule()
                                        {
                                            tbElectiveOrg = tbElectiveOrg,
                                            tbWeek = weekList.Where(d => org.Contains(d.WeekName)).FirstOrDefault(),
                                            tbPeriod = periodList.Where(d => org.Contains(d.PeriodName)).FirstOrDefault()
                                        };
                                        db.Set<Entity.tbElectiveOrgSchedule>().Add(tbElectiveOrgSchedule);
                                    }
                                }
                                #endregion

                                #region tbEelectiveOrgClass
                                if (string.IsNullOrEmpty(item.LimitClass) == false)
                                {
                                    if (item.LimitClass.Contains("全部"))
                                    {
                                        tbElectiveOrg.IsPermitClass = false;
                                    }
                                    else
                                    {
                                        tbElectiveOrg.IsPermitClass = true;
                                    }

                                    //设置选课对象(可以是全部年级、高一年级或班级，多个以逗号隔开）
                                    var classIds = new List<int>();
                                    foreach (var name in item.LimitClass.Split(',').ToList())
                                    {
                                        var classIdList = (from p in db.Table<Basis.Entity.tbClass>()
                                                           where p.tbGrade.IsDeleted == false
                                                            && (p.tbGrade.GradeName == name || p.ClassName == name)
                                                           select p.Id).ToList();
                                        classIds.AddRange(classIdList);
                                    }

                                    classIds = classIds.Distinct().ToList();

                                    //选课对应的所有可选班级
                                    var tbElectiveClassIds = (from p in db.Table<Entity.tbElectiveClass>()
                                                              where p.tbElective.Id == vm.ElectiveId
                                                              select p.tbClass.Id).ToList();

                                    //删除不在范围内的班级
                                    classIds.RemoveAll(d => tbElectiveClassIds.Contains(d) == false);

                                    //删除原来已存在的不在本次范围内的班级
                                    foreach (var a in electiveOrgClassList.Where(d => classIds.Contains(d.tbClass.Id) == false))
                                    {
                                        a.IsDeleted = true;
                                    }

                                    //新增不在限制的班级
                                    var ids = classIds.Where(d => tbElectiveClassIds.Contains(d));
                                    if ((ids == null || !ids.Any()))
                                    {
                                        tbElectiveOrg.IsPermitClass = electiveOrgClassList.Count(p => !p.IsDeleted) > 0;
                                    }
                                    else
                                    {
                                        foreach (var a in ids)
                                        {
                                            var limit = new Entity.tbElectiveOrgClass();
                                            limit.tbElectiveOrg = tbElectiveOrg;
                                            limit.tbClass = db.Set<Basis.Entity.tbClass>().Find(a);
                                            limit.MaxLimit = 999;
                                            tbElectiveOrgClassList.Add(limit);
                                        }
                                    }
                                }

                                #endregion
                            }

                            db.Set<Entity.tbElectiveOrgClass>().AddRange(tbElectiveOrgClassList);

                            var rowNum = db.SaveChanges();
                            if (rowNum > 0 || vm.IsUpdate && rowNum >= 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量添加了课程");
                                vm.ImportList.RemoveAll(p => string.IsNullOrWhiteSpace(p.ImportError));
                                vm.Status = true;
                            }
                        }
                    }
                }
            }

            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Elective/Views/ElectiveOrg/Import.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult Export(Models.ElectiveOrg.Import vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                var orgList = (from p in db.Table<Entity.tbElectiveOrg>()
                               where p.tbElective.Id == vm.ElectiveId
                               select new
                               {
                                   p.Id,
                                   p.OrgName,
                                   p.tbCourse.CourseName,
                                   p.tbElectiveSection.ElectiveSectionName,
                                   p.tbElectiveGroup.ElectiveGroupName,
                                   p.MaxCount,
                                   p.Permit,
                                   p.tbTeacher.TeacherName,
                                   p.tbRoom.RoomName,
                                   p.RemainCount,
                                   p.IsPermitClass
                               }).ToList();

                var scheduleList = (from p in db.Table<Entity.tbElectiveOrgSchedule>()
                                    where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                    select new
                                    {
                                        OrgId = p.tbElectiveOrg.Id,
                                        p.tbWeek.WeekName,
                                        p.tbPeriod.PeriodName
                                    }).ToList();

                var dataTable = new System.Data.DataTable();
                var listColumnNames = new List<string> { "班级名称", "课程名称", "选课分段名称", "选课分组名称", "总人数", "授权状态", "任课教师", "教室", "剩余人数", "星期节次", "选课对象" };
                dataTable.Columns.AddRange(listColumnNames.Select(p => new System.Data.DataColumn(p)).ToArray());

                var limitClassList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                      join q in db.Table<Entity.tbElectiveClass>() on p.tbClass.Id equals q.tbClass.Id
                                      where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                        && p.tbClass.IsDeleted == false
                                        && p.tbClass.tbGrade.IsDeleted == false
                                        && p.tbElectiveOrg.IsDeleted == false
                                        && p.tbElectiveOrg.tbElective.Id == q.tbElective.Id
                                      select new
                                      {
                                          OrgId = p.tbElectiveOrg.Id,
                                          ClassId = p.tbClass.Id,
                                          GradeNo = p.tbClass.tbGrade.No,
                                          ClassNo = p.tbClass.No,
                                          p.tbClass.ClassName
                                      }).ToList();

                orgList.ForEach(s =>
                {
                    var dr = dataTable.NewRow();
                    dr["班级名称"] = s.OrgName;
                    dr["课程名称"] = s.CourseName;
                    dr["选课分段名称"] = s.ElectiveSectionName;
                    dr["选课分组名称"] = s.ElectiveGroupName;
                    dr["总人数"] = s.MaxCount;
                    dr["授权状态"] = s.Permit == 1 ? "白名单" : (s.Permit == -1 ? "黑名单" : "不限制");
                    dr["任课教师"] = s.TeacherName;
                    dr["教室"] = s.RoomName;
                    dr["剩余人数"] = s.RemainCount;
                    dr["星期节次"] = string.Join(",", scheduleList.Where(d => d.OrgId == s.Id).Select(d => d.WeekName + d.PeriodName).ToList());

                    if (s.IsPermitClass == false)
                    {
                        dr["选课对象"] = "全部";
                    }
                    else
                    {

                        var myClassList = limitClassList.Where(d => d.OrgId == s.Id).OrderBy(o => o.GradeNo).ThenBy(d => d.ClassNo).ThenBy(d => d.ClassName).Select(c => c.ClassName).ToList();
                        dr["选课对象"] = string.Join(",", myClassList);
                    }

                    dataTable.Rows.Add(dr);
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

        public ActionResult Select()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveOrg.Select();

                var elective = (from p in db.Table<Entity.tbElective>()
                                where p.Id == vm.ElectiveId
                                select new
                                {
                                    p.FromDate,
                                    p.ToDate
                                }).FirstOrDefault();
                if (elective != null)
                {
                    vm.IsOpen = elective.FromDate < DateTime.Now;
                    vm.IsEnd = elective.ToDate < DateTime.Now;
                }

                var limitOrgList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                    join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                    where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId &&
                                          p.tbElectiveOrg.IsDeleted == false &&
                                          q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                    select p.tbElectiveOrg.Id).ToList();

                //星期节次的选中课程
                var electiveDataList = (from p in db.Table<Entity.tbElectiveData>()
                                        join q in db.Table<Entity.tbElectiveOrgSchedule>() on p.tbElectiveOrg.Id equals q.tbElectiveOrg.Id
                                        where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                           && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                           && q.tbWeek.Id == vm.WeekId
                                           && q.tbPeriod.Id == vm.PeriodId
                                        select new
                                        {
                                            OrgId = p.tbElectiveOrg.Id,
                                            p.IsFixed
                                        }).ToList();

                var orgScheduleList = (from p in db.Table<Entity.tbElectiveOrg>()
                                       join q in db.Table<Entity.tbElectiveOrgSchedule>() on p.Id equals q.tbElectiveOrg.Id
                                       where p.tbElective.Id == vm.ElectiveId
                                        && p.tbCourse.IsDeleted == false
                                       select new
                                       {
                                           OrgId = p.Id,
                                           p.OrgName,
                                           p.RemainCount,
                                           CourseId = p.tbCourse.Id,
                                           WeekId = q.tbWeek.Id,
                                           q.tbWeek.WeekName,
                                           PeriodId = q.tbPeriod.Id,
                                           q.tbPeriod.PeriodName,
                                           p.tbTeacher.TeacherName,
                                           p.tbRoom.RoomName,
                                           p.Permit,
                                           p.IsPermitClass
                                       }).ToList();

                vm.ElectiveOrgList = (from p in orgScheduleList
                                      where p.WeekId == vm.WeekId &&
                                            p.PeriodId == vm.PeriodId &&
                                            (p.IsPermitClass == false || limitOrgList.Contains(p.OrgId))
                                      orderby p.OrgName
                                      select new Dto.ElectiveOrg.Select()
                                      {
                                          Id = p.OrgId,
                                          CourseId = p.CourseId,
                                          OrgName = p.OrgName,
                                          WeekId = p.WeekId,
                                          PeriodId = p.PeriodId,
                                          RemainCount = p.RemainCount,
                                          TeacherName = p.TeacherName,
                                          RoomName = p.RoomName,
                                          Permit = p.Permit,
                                          IsChecked = electiveDataList.Where(d => d.OrgId == p.OrgId).Count() > 0 ? true : false,
                                          IsFixed = electiveDataList.Where(d => d.OrgId == p.OrgId).Select(d => d.IsFixed).DefaultIfEmpty().FirstOrDefault(),
                                      }).ToList();

                var orgStudent = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                                  where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                  select p.tbElectiveOrg.Id).ToList();

                vm.ElectiveOrgList.ForEach(s =>
                {
                    s.WeekPeriod = string.Join(",", orgScheduleList.Where(o => o.OrgId == s.Id).Select(o => o.WeekName + o.PeriodName).ToList());
                });

                //白名单
                vm.ElectiveOrgList.RemoveAll(d => d.Permit == 1 && orgStudent.Contains(d.Id) == false);
                //黑名单
                vm.ElectiveOrgList.RemoveAll(d => d.Permit == -1 && orgStudent.Contains(d.Id));

                vm.ElectiveOrgList.Insert(0, new Dto.ElectiveOrg.Select() { Id = -1, OrgName = "<font color=red><b>不选课程，清空已选课程</b></font>", RemainCount = 999 });

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Select(Models.ElectiveOrg.Select vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                var orgId = Request.Form["rdoId"].ConvertToInt();

                var elective = (from p in db.Table<Entity.tbElective>()
                                where p.Id == vm.ElectiveId
                                select new
                                {
                                    p.FromDate,
                                    p.ToDate,
                                    p.tbElectiveType.ElectiveTypeCode
                                }).FirstOrDefault();
                if (elective != null)
                {
                    vm.IsOpen = elective.FromDate < DateTime.Now;
                    vm.IsEnd = elective.ToDate < DateTime.Now;

                    if (vm.IsOpen == false || vm.IsEnd)
                    {
                        error.AddError("选课未开放!");
                        return Code.MvcHelper.Post(error);
                    }
                }

                //判断当前星期节次是否存在已选中的课程
                var myElectiveOrg = (from p in db.Table<Entity.tbElectiveData>()
                                     join q in db.Table<Entity.tbElectiveOrgSchedule>() on p.tbElectiveOrg.Id equals q.tbElectiveOrg.Id
                                     where p.tbElectiveOrg.IsDeleted == false
                                      && p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                        && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        && q.tbWeek.Id == vm.WeekId
                                        && q.tbPeriod.Id == vm.PeriodId
                                     select p).Include(d => d.tbElectiveOrg).ToList();

                var limitOrgList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                    join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                    where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                      && p.tbElectiveOrg.IsDeleted == false
                                      && q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                    select new { p.tbElectiveOrg, tbElectiveOrgClass = p }).ToList();


                //若不选择课程（或者无更新的提交操作）
                if (orgId == 0)
                {
                    if (myElectiveOrg.Count == 0)
                    {
                        error.AddError("请选择一项再提交!");
                        return Code.MvcHelper.Post(error);
                    }
                    else
                    {
                        return Code.MvcHelper.Post();
                    }
                }
                else if (orgId == -1)
                {
                    //选择了清空该节次课程，则删除所选课程
                    foreach (var a in myElectiveOrg)
                    {
                        a.tbElectiveOrg.RemainCount++;
                        a.IsDeleted = true;

                        var limitClass = limitOrgList.Where(p => p.tbElectiveOrgClass.tbElectiveOrg.Id == a.tbElectiveOrg.Id).Select(p => p.tbElectiveOrgClass).FirstOrDefault();
                        if (limitClass != null)
                        {
                            limitClass.RemainCount++;
                        }
                    }

                    db.SaveChanges();
                }
                else
                {
                    var item = limitOrgList.Where(p => p.tbElectiveOrg.Id == orgId).FirstOrDefault();
                    if (item != null && item.tbElectiveOrgClass != null && item.tbElectiveOrgClass.RemainCount <= 0)
                    {
                        error.AddError($"选课{item.tbElectiveOrg.OrgName}针对当前行政班级的人数限制已满！");
                        return Code.MvcHelper.Post(error);
                    }


                    //选课课程重复判断
                    var electiveOrgCourseId = (from p in db.Table<Entity.tbElectiveOrg>()
                                               where p.Id == orgId
                                               select p.tbCourse.Id).FirstOrDefault();

                    var checkCourse = (from p in db.Table<Entity.tbElectiveData>()
                                       where p.tbElectiveOrg.Id != orgId
                                        && p.tbElectiveOrg.IsDeleted == false
                                        && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        && p.tbElectiveOrg.tbCourse.Id == electiveOrgCourseId
                                        && p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                       select 1).Count();

                    if (checkCourse > 0)
                    {
                        error.AddError("已选择该课程的另外节次，同一课程无法重复选择!");
                        return Code.MvcHelper.Post(error);
                    }

                    #region
                    //判断是否超出本段的最大选课数（判断还是有些问题的）
                    //var electiveSection = (from p in db.Table<Entity.tbElectiveData>()
                    //                       where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                    //                        && p.tbElectiveOrg.IsDeleted == false
                    //                        && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                    //                        && p.tbElectiveOrg.tbElectiveSection.IsDeleted == false
                    //                       group p by new { p.tbElectiveOrg.tbElectiveSection.Id, p.tbElectiveOrg.tbElectiveSection.ElectiveSectionName, p.tbElectiveOrg.tbElectiveSection.MaxElective } into k
                    //                       select new
                    //                       {
                    //                           k.Key.Id,
                    //                           k.Key.ElectiveSectionName,
                    //                           k.Key.MaxElective,
                    //                           iCount = k.Count()
                    //                       }).ToList();
                    //if (electiveSection.Where(d => d.MaxElective < d.iCount + 1).Count() > 0)
                    //{
                    //    error.AddError("超出该段最大选课数!");
                    //    return Code.MvcHelper.Post(error);
                    //}                    

                    //判断是否超出本组的最大选课数（判断还是有些问题的）
                    //var electiveGroup = (from p in db.Table<Entity.tbElectiveGroup>()
                    //                     join q in db.Table<Entity.tbElectiveData>() on p.Id equals q.tbElectiveOrg.tbElectiveGroup.Id
                    //                     where p.tbElective.Id == vm.ElectiveId
                    //                        && q.tbElectiveOrg.IsDeleted == false
                    //                        && q.tbStudent.tbSysUser.Id == Code.Common.UserId
                    //                     group p by new { p.Id, p.ElectiveGroupName, p.MaxElective } into k
                    //                     select new
                    //                     {
                    //                         k.Key.Id,
                    //                         k.Key.ElectiveGroupName,
                    //                         k.Key.MaxElective,
                    //                         iCount = k.Count()
                    //                     }).ToList();
                    //if (electiveGroup.Where(d => d.MaxElective < d.iCount + 1).Count() > 0)
                    //{
                    //    error.AddError("超出该组最大选课数!");
                    //    return Code.MvcHelper.Post(error);
                    //}
                    #endregion

                    //当前选择的课程所属分段
                    var electiveOrgInfo = (from p in db.Table<Entity.tbElectiveOrg>()
                                           where p.Id == orgId
                                           select new
                                           {
                                               tbElectiveSection = p.tbElectiveSection,
                                               tbElectiveGroup = p.tbElectiveGroup
                                           }).FirstOrDefault();

                    //当前选择课程所属分段下已选课程数

                    var electiveSection = electiveOrgInfo.tbElectiveSection;
                    var exNum = (from p in db.Table<Entity.tbElectiveData>()
                                 where p.tbElectiveOrg.tbElectiveSection.Id == electiveSection.Id && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                 select p).Count();

                    if (exNum + 1 > electiveSection.MaxElective)
                    {
                        error.AddError($"已选人数[{exNum + 1}]超出分段[{electiveSection.ElectiveSectionName}]的最大人数[{electiveSection.MaxElective}]");
                        return Code.MvcHelper.Post(error);
                    }

                    //当前选择的课程所属分组
                    //var electiveGroup = (from p in db.Table<Entity.tbElectiveGroup>() where p.tbElective.Id == vm.ElectiveId select p).FirstOrDefault();
                    var electiveGroup = electiveOrgInfo.tbElectiveGroup;

                    //当前选择课程所属分组下已选课程数
                    var existsGroupNum = (from p in db.Table<Entity.tbElectiveData>()
                                          where p.tbElectiveOrg.tbElectiveGroup.Id == electiveGroup.Id && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                          select p).Count();

                    if (exNum + 1 > electiveGroup.MaxElective)
                    {
                        error.AddError($"已选人数[{exNum + 1}]超出分组[{electiveGroup.ElectiveGroupName}]的最大人数[{electiveGroup.MaxElective}]");
                        return Code.MvcHelper.Post(error);
                    }



                    //判断选课是否星期节次模式,星期节次模式的时候，需要核对对应Schedule中的所有位置是空才可以选课!
                    //列表选课判断已选课程
                    if (elective.ElectiveTypeCode == Code.EnumHelper.ElectiveType.WeekPeriod)
                    {
                        var orgSchedeule = (from p in db.Table<Entity.tbElectiveOrgSchedule>()
                                            where p.tbElectiveOrg.Id == orgId
                                            select p.tbWeek.Id + "," + p.tbPeriod.Id
                                            ).ToList();

                        var dataSchedule = (from p in db.Table<Entity.tbElectiveData>()
                                            join q in db.Table<Entity.tbElectiveOrgSchedule>()
                                            on p.tbElectiveOrg.Id equals q.tbElectiveOrg.Id
                                            where p.tbElectiveOrg.Id != orgId && p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                                && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                && (q.tbWeek.Id + "," + q.tbPeriod.Id) != (vm.WeekId + "," + vm.PeriodId)
                                                && orgSchedeule.Contains(q.tbWeek.Id + "," + q.tbPeriod.Id)
                                            select new
                                            {
                                                WeekPeriod = q.tbWeek.WeekName + q.tbPeriod.PeriodName
                                            }).ToList();
                        if (dataSchedule.Count > 0)
                        {
                            error.AddError("所选课程需包含节次：" + string.Join(",", dataSchedule.Select(d => d.WeekPeriod).ToList()) + "，该位置已有选课，要选择该课程请先清空这些节次!");
                            return Code.MvcHelper.Post(error);
                        }

                    }

                    var success = false;
                    do
                    {
                        try
                        {
                            var tbOrg = (from p in db.Table<Entity.tbElectiveOrg>()
                                            .Include(d => d.tbElectiveSection)
                                            .Include(d => d.tbElectiveGroup)
                                         where p.Id == orgId
                                         select p).FirstOrDefault();
                            if (tbOrg == null)
                            {
                                return Code.MvcHelper.Post();
                            }

                            if (tbOrg.RemainCount <= 0)
                            {
                                error.AddError("人数已满!");
                                return Code.MvcHelper.Post(error);
                            }
                            else if ((tbOrg.MaxCount - ElectiveDataController.GetCount(db, tbOrg.Id)) <= 0)
                            {
                                tbOrg.RemainCount = tbOrg.MaxCount - ElectiveDataController.GetCount(db, tbOrg.Id);
                                db.SaveChanges();
                                error.AddError("人数已满!");
                                return Code.MvcHelper.Post(error);
                            }

                            if (elective.ElectiveTypeCode != Code.EnumHelper.ElectiveType.WeekPeriod)
                            {
                                var exData = db.Table<Entity.tbElectiveData>().Where(p => p.tbStudent.tbSysUser.Id == Code.Common.UserId && p.tbElectiveOrg.tbElective.Id == tbOrg.Id)
                                    .Select(p => new
                                    {
                                        GroupId = p.tbElectiveOrg.tbElectiveGroup.Id,
                                        GroupMax = p.tbElectiveOrg.tbElectiveGroup.MaxElective,
                                        SectionId = p.tbElectiveOrg.tbElectiveSection.Id,
                                        SectionMax = p.tbElectiveOrg.tbElectiveSection.MaxElective
                                    }).ToList();
                                if (exData != null && exData.Any())
                                {
                                    var limitClassList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                                          join q in db.Table<Basis.Entity.tbClassStudent>()
                                                          on p.tbClass.Id equals q.tbClass.Id
                                                          where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                                            && p.tbElectiveOrg.IsDeleted == false
                                                            && q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                          select p.tbElectiveOrg.Id).ToList();

                                    var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                                              where p.tbElective.Id == vm.ElectiveId
                                                 && p.tbCourse.IsDeleted == false
                                                 && p.tbElectiveGroup.IsDeleted == false
                                                 && p.tbElectiveSection.IsDeleted == false
                                                 && (p.IsPermitClass == false || limitClassList.Contains(p.Id))
                                              orderby p.tbElectiveGroup.Id
                                              select new
                                              {
                                                  p,
                                                  SectionId = p.tbElectiveSection.Id,
                                                  SectionNo = p.tbElectiveSection.No,
                                                  SectionName = p.tbElectiveSection.ElectiveSectionName,
                                                  SectionMin = p.tbElectiveSection.MinElective,
                                                  SectionMax = p.tbElectiveSection.MaxElective,
                                                  GroupId = p.tbElectiveGroup.Id,
                                                  GroupNo = p.tbElectiveGroup.No,
                                                  GroupName = p.tbElectiveGroup.ElectiveGroupName,
                                                  GroupMin = p.tbElectiveGroup.MinElective,
                                                  GroupMax = p.tbElectiveGroup.MaxElective,
                                              }).ToList();

                                    var sectionList = (from p in tb
                                                       group p by new { p.SectionId, p.SectionNo, p.SectionName, p.SectionMax, p.SectionMin } into g
                                                       select new
                                                       {
                                                           g.Key.SectionId,
                                                           g.Key.SectionNo,
                                                           g.Key.SectionName,
                                                           g.Key.SectionMax,
                                                           g.Key.SectionMin
                                                       }).OrderBy(d => d.SectionNo).ToList();

                                    var groupList = (from p in tb
                                                     group p by new { p.GroupId, p.GroupNo, p.GroupName, p.GroupMax, p.GroupMin } into g
                                                     select new
                                                     {
                                                         g.Key.GroupId,
                                                         g.Key.GroupNo,
                                                         g.Key.GroupName,
                                                         g.Key.GroupMax,
                                                         g.Key.GroupMin
                                                     }).OrderBy(d => d.GroupNo).ToList();

                                    foreach (var group in groupList)
                                    {
                                        var subtract = tbOrg.tbElectiveGroup.Id == group.GroupId ? 1 : 0;
                                        if (exData.Count(p => p.GroupId == group.GroupId) > (group.GroupMax - subtract))
                                        {
                                            error.AddError("已选课程数超过分组[" + group.GroupName + "]最大课程数！");
                                            return Code.MvcHelper.Post(error);
                                        }
                                    }

                                    foreach (var section in sectionList)
                                    {
                                        var subtract = tbOrg.tbElectiveSection.Id == section.SectionId ? 1 : 0;
                                        if (exData.Count(p => p.SectionId == section.SectionId) > (section.SectionMax - subtract))
                                        {
                                            error.AddError("已选课程数超过分段[" + section.SectionName + "]最大课程数！");
                                            return Code.MvcHelper.Post(error);
                                        }
                                    }
                                }
                            }
                            tbOrg.RemainCount--;

                            //更新行政班人数限制（如果有的话）
                            var limitClass = limitOrgList.Where(p => p.tbElectiveOrgClass.tbElectiveOrg.Id == orgId).Select(p => p.tbElectiveOrgClass).FirstOrDefault();
                            if (limitClass != null)
                            {
                                limitClass.RemainCount--;
                            }

                            //添加新选课数据
                            var tbElectiveData = new Entity.tbElectiveData()
                            {
                                tbElectiveOrg = tbOrg,
                                tbStudent = db.Set<Student.Entity.tbStudent>().Where(p => p.tbSysUser.Id == Code.Common.UserId).FirstOrDefault(),
                                IsPreElective = false,
                                InputDate = DateTime.Now
                            };
                            db.Set<Entity.tbElectiveData>().Add(tbElectiveData);

                            //删除本节次之前的选课记录选课数据
                            foreach (var a in myElectiveOrg)
                            {
                                a.tbElectiveOrg.RemainCount++;
                                a.IsDeleted = true;
                                var _limitClass = limitOrgList.Where(p => p.tbElectiveOrgClass.tbElectiveOrg.Id == a.tbElectiveOrg.Id).Select(p => p.tbElectiveOrgClass).FirstOrDefault();
                                if (_limitClass != null)
                                {
                                    _limitClass.RemainCount++;
                                }

                            }

                            db.SaveChanges();

                            //删除重复的数据，防止同一节次添加多条记录(主要是解决高并发下会重复插入数据的问题)
                            var invalidDataList = (from p in db.Table<Entity.tbElectiveData>()
                                                   join q in db.Table<Entity.tbElectiveOrgSchedule>() on p.tbElectiveOrg.Id equals q.tbElectiveOrg.Id
                                                   where p.tbElectiveOrg.IsDeleted == false
                                                    && p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                                      && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                      && q.tbWeek.Id == vm.WeekId
                                                      && q.tbPeriod.Id == vm.PeriodId
                                                      && p.Id != tbElectiveData.Id
                                                   select p).Include(d => d.tbElectiveOrg).ToList();
                            foreach (var a in invalidDataList)
                            {
                                a.tbElectiveOrg.RemainCount++;
                                a.IsDeleted = true;
                                var _limitClass = limitOrgList.Where(p => p.tbElectiveOrgClass.tbElectiveOrg.Id == a.tbElectiveOrg.Id).Select(p => p.tbElectiveOrgClass).FirstOrDefault();
                                if (_limitClass != null)
                                {
                                    _limitClass.RemainCount++;
                                }
                            }

                            db.SaveChanges();

                            success = true;
                        }
                        catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException exception)
                        {
                            exception.Entries.Single().Reload();
                        }
                    } while (!success);
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult SelectForBase()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveOrg.Select();

                var elective = (from p in db.Table<Entity.tbElective>()
                                where p.Id == vm.ElectiveId
                                select new
                                {
                                    p.FromDate,
                                    p.ToDate
                                }).FirstOrDefault();
                if (elective != null)
                {
                    vm.IsOpen = elective.FromDate < DateTime.Now;
                    vm.IsEnd = elective.ToDate < DateTime.Now;
                }

                var limitOrgList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                    join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                    where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId &&
                                          p.tbElectiveOrg.IsDeleted == false &&
                                          q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                    select p.tbElectiveOrg.Id).ToList();

                //选中课程
                var electiveDataList = (from p in db.Table<Entity.tbElectiveData>()
                                        where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                            && p.tbElectiveOrg.tbElectiveGroup.Id == vm.GroupId
                                            && p.tbElectiveOrg.tbElectiveSection.Id == vm.SectionId
                                            && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        select new
                                        {
                                            OrgId = p.tbElectiveOrg.Id,
                                            IsFixed = p.IsFixed
                                        }).ToList();

                var orgList = (from p in db.Table<Entity.tbElectiveOrg>()
                               where p.tbElective.Id == vm.ElectiveId
                                 && p.tbElectiveGroup.Id == vm.GroupId
                                 && p.tbElectiveSection.Id == vm.SectionId
                                 && (p.IsPermitClass == false || limitOrgList.Contains(p.Id))
                                 && p.RemainCount > 0
                               orderby p.OrgName
                               select new Dto.ElectiveOrg.Select()
                               {
                                   Id = p.Id,
                                   CourseId = p.tbCourse.Id,
                                   OrgName = p.OrgName,
                                   RemainCount = p.RemainCount,
                                   TeacherName = p.tbTeacher.TeacherName,
                                   RoomName = p.tbRoom.RoomName,
                                   Permit = p.Permit
                               }).ToList();

                vm.ElectiveOrgList = (from p in orgList
                                      orderby p.OrgName
                                      select new Dto.ElectiveOrg.Select()
                                      {
                                          Id = p.Id,
                                          CourseId = p.CourseId,
                                          OrgName = p.OrgName,
                                          RemainCount = p.RemainCount,
                                          TeacherName = p.TeacherName,
                                          RoomName = p.RoomName,
                                          Permit = p.Permit,
                                          IsChecked = electiveDataList.Where(d => d.OrgId == p.Id).Count() > 0 ? true : false,
                                          IsFixed = electiveDataList.Where(d => d.OrgId == p.Id).Select(d => d.IsFixed).DefaultIfEmpty().FirstOrDefault()
                                      }).ToList();

                var orgStudent = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                                  where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                  select p.tbElectiveOrg.Id).ToList();

                //白名单
                vm.ElectiveOrgList.RemoveAll(d => d.Permit == 1 && orgStudent.Contains(d.Id) == false);
                //黑名单
                vm.ElectiveOrgList.RemoveAll(d => d.Permit == -1 && orgStudent.Contains(d.Id));

                vm.ElectiveOrgList.Insert(0, new Dto.ElectiveOrg.Select() { Id = -1, OrgName = "<font color=red><b>不选课程，清空已选课程</b></font>", RemainCount = 999 });

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectForBase(Models.ElectiveOrg.Select vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                var orgId = Request.Form["rdoId"].ConvertToInt();
                var electiveOrgId = vm.ElectiveOrgId;

                var elective = (from p in db.Table<Entity.tbElective>()
                                where p.Id == vm.ElectiveId
                                select new
                                {
                                    p.FromDate,
                                    p.ToDate,
                                    p.tbElectiveType.ElectiveTypeCode
                                }).FirstOrDefault();
                if (elective != null)
                {
                    vm.IsOpen = elective.FromDate < DateTime.Now;
                    vm.IsEnd = elective.ToDate < DateTime.Now;

                    if (vm.IsOpen == false || vm.IsEnd)
                    {
                        error.AddError("选课未开放!");
                        return Code.MvcHelper.Post(error);
                    }
                }

                if (orgId == 0)
                {
                    error.AddError("请选择一项再提交!");
                    return Code.MvcHelper.Post(error);

                }
                else if (orgId == -1)
                {
                    //删除所选课程
                    var data = (from p in db.Table<Entity.tbElectiveData>()
                                    .Include(d => d.tbElectiveOrg)
                                where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                    && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                select p).ToList();
                    foreach (var a in data)
                    {
                        a.tbElectiveOrg.RemainCount++;
                        a.IsDeleted = true;
                    }
                    db.SaveChanges();
                }
                else
                {
                    var success = false;
                    do
                    {
                        try
                        {
                            var tbOrg = (from p in db.Table<Entity.tbElectiveOrg>()
                                            .Include(d => d.tbElectiveSection)
                                            .Include(d => d.tbElectiveGroup)
                                         where p.Id == orgId
                                         select p).FirstOrDefault();
                            if (tbOrg == null)
                            {
                                return Code.MvcHelper.Post();
                            }

                            if (tbOrg.RemainCount <= 0)
                            {
                                error.AddError("人数已满!");
                                return Code.MvcHelper.Post(error);
                            }
                            else if ((tbOrg.MaxCount - ElectiveDataController.GetCount(db, tbOrg.Id)) <= 0)
                            {
                                tbOrg.RemainCount = tbOrg.MaxCount - ElectiveDataController.GetCount(db, tbOrg.Id);
                                db.SaveChanges();
                                error.AddError("人数已满!");
                                return Code.MvcHelper.Post(error);
                            }

                            var exData = db.Table<Entity.tbElectiveData>().Where(p => p.tbStudent.tbSysUser.Id == Code.Common.UserId && p.tbElectiveOrg.tbElective.Id == tbOrg.Id)
                                .Select(p => new
                                {
                                    GroupId = p.tbElectiveOrg.tbElectiveGroup.Id,
                                    GroupMax = p.tbElectiveOrg.tbElectiveGroup.MaxElective,
                                    SectionId = p.tbElectiveOrg.tbElectiveSection.Id,
                                    SectionMax = p.tbElectiveOrg.tbElectiveSection.MaxElective
                                }).ToList();
                            if (exData != null && exData.Any())
                            {
                                var limitClassList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                                      join q in db.Table<Basis.Entity.tbClassStudent>()
                                                      on p.tbClass.Id equals q.tbClass.Id
                                                      where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                                        && p.tbElectiveOrg.IsDeleted == false
                                                        && q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                      select p.tbElectiveOrg.Id).ToList();

                                var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                                          where p.tbElective.Id == vm.ElectiveId
                                             && p.tbCourse.IsDeleted == false
                                             && p.tbElectiveGroup.IsDeleted == false
                                             && p.tbElectiveSection.IsDeleted == false
                                             && (p.IsPermitClass == false || limitClassList.Contains(p.Id))
                                          orderby p.tbElectiveGroup.Id
                                          select new
                                          {
                                              p,
                                              SectionId = p.tbElectiveSection.Id,
                                              SectionNo = p.tbElectiveSection.No,
                                              SectionName = p.tbElectiveSection.ElectiveSectionName,
                                              SectionMin = p.tbElectiveSection.MinElective,
                                              SectionMax = p.tbElectiveSection.MaxElective,
                                              GroupId = p.tbElectiveGroup.Id,
                                              GroupNo = p.tbElectiveGroup.No,
                                              GroupName = p.tbElectiveGroup.ElectiveGroupName,
                                              GroupMin = p.tbElectiveGroup.MinElective,
                                              GroupMax = p.tbElectiveGroup.MaxElective,
                                          }).ToList();

                                var sectionList = (from p in tb
                                                   group p by new { p.SectionId, p.SectionNo, p.SectionName, p.SectionMax, p.SectionMin } into g
                                                   select new
                                                   {
                                                       g.Key.SectionId,
                                                       g.Key.SectionNo,
                                                       g.Key.SectionName,
                                                       g.Key.SectionMax,
                                                       g.Key.SectionMin
                                                   }).OrderBy(d => d.SectionNo).ToList();

                                var groupList = (from p in tb
                                                 group p by new { p.GroupId, p.GroupNo, p.GroupName, p.GroupMax, p.GroupMin } into g
                                                 select new
                                                 {
                                                     g.Key.GroupId,
                                                     g.Key.GroupNo,
                                                     g.Key.GroupName,
                                                     g.Key.GroupMax,
                                                     g.Key.GroupMin
                                                 }).OrderBy(d => d.GroupNo).ToList();

                                foreach (var group in groupList)
                                {
                                    var subtract = tbOrg.tbElectiveGroup.Id == group.GroupId ? 1 : 0;
                                    if (exData.Count(p => p.GroupId == group.GroupId) > (group.GroupMax - subtract))
                                    {
                                        error.AddError("已选课程数超过分组[" + group.GroupName + "]最大课程数！");
                                        return Code.MvcHelper.Post(error);
                                    }
                                }

                                foreach (var section in sectionList)
                                {
                                    var subtract = tbOrg.tbElectiveSection.Id == section.SectionId ? 1 : 0;
                                    if (exData.Count(p => p.SectionId == section.SectionId) > (section.SectionMax - subtract))
                                    {
                                        error.AddError("已选课程数超过分段[" + section.SectionName + "]最大课程数！");
                                        return Code.MvcHelper.Post(error);
                                    }
                                }

                            }

                            tbOrg.RemainCount--;
                            //添加新选课数据
                            var tbElectiveData = new Entity.tbElectiveData()
                            {
                                tbElectiveOrg = tbOrg,
                                tbStudent = db.Set<Student.Entity.tbStudent>().Where(p => p.tbSysUser.Id == Code.Common.UserId).FirstOrDefault(),
                                IsPreElective = false,
                                InputDate = DateTime.Now
                            };
                            db.Set<Entity.tbElectiveData>().Add(tbElectiveData);

                            //删除老选课数据
                            if (electiveOrgId != 0)
                            {
                                var tbOldOrg = db.Set<Entity.tbElectiveOrg>().Find(electiveOrgId);
                                tbOldOrg.RemainCount++;

                                var tbOldData = db.Table<Entity.tbElectiveData>().Where(p => p.tbElectiveOrg.Id == electiveOrgId && p.tbStudent.tbSysUser.Id == Code.Common.UserId).FirstOrDefault();
                                tbOldData.IsDeleted = true;
                            }

                            db.SaveChanges();
                            success = true;
                        }
                        catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException exception)
                        {
                            exception.Entries.Single().Reload();
                        }
                    } while (!success);
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpGet]
        public JsonResult Info(int orgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var classList = (from p in db.Table<Entity.tbElectiveOrg>()
                                 join e in db.Table<Entity.tbElective>() on p.tbElective.Id equals e.Id
                                 join c in db.Table<Entity.tbElectiveClass>() on e.Id equals c.tbElective.Id
                                 where p.Id == orgId
                                 select new
                                 {
                                     p.tbRoom.RoomName,
                                     GradeName = c.tbClass.tbGrade.GradeName
                                 }).Distinct().ToList();


                var limitClassList = (from p in db.Table<Entity.tbElectiveOrg>()
                                      join l in db.Table<Entity.tbElectiveOrgClass>() on p.Id equals l.tbElectiveOrg.Id
                                      where p.Id == orgId
                                      select new
                                      {
                                          p.tbRoom.RoomName,
                                          GradeName = l.tbClass.tbGrade.GradeName
                                      }).Distinct().ToList();

                if (limitClassList != null && limitClassList.Any())
                {
                    classList.RemoveAll(p => limitClassList.Exists(l => l.GradeName.Equals(p.GradeName)));
                }


                return Json(classList, JsonRequestBehavior.AllowGet);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                          where p.tbElective.Id == electiveId
                            && p.tbCourse.IsDeleted == false
                            && p.tbElectiveGroup.IsDeleted == false
                            && p.tbElectiveSection.IsDeleted == false
                          orderby p.OrgName
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.OrgName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int electiveId, int teacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                          where p.tbElective.Id == electiveId
                            && p.tbCourse.IsDeleted == false
                            && p.tbElectiveGroup.IsDeleted == false
                            && p.tbElectiveSection.IsDeleted == false
                            && p.tbTeacher.tbSysUser.Id == teacherId
                          orderby p.OrgName
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.OrgName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

    }
}
