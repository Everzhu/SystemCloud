using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveChangeController : Controller
    {
        public ActionResult List()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Teacher)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveChange.List();
                vm.ElectiveList = ElectiveController.SelectList();
                if (vm.ElectiveId == 0 && vm.ElectiveList.Count() > 0)
                {
                    vm.ElectiveId = vm.ElectiveList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.IsWeekPeriod = (from p in db.Table<Entity.tbElective>()
                                   where p.Id == vm.ElectiveId
                                   select p.tbElectiveType.ElectiveTypeCode).FirstOrDefault() == Code.EnumHelper.ElectiveType.WeekPeriod;

                //var classList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                //                 where p.tbClass.tbYear.IsDefault
                //                    && p.tbClass.IsDeleted == false
                //                    && p.tbClass.tbYear.IsDeleted == false
                //                 select new
                //                 {
                //                     StudentId = p.tbStudent.Id,
                //                     p.tbClass.ClassName
                //                 }).ToList();

                var tb = from p in db.Table<Entity.tbElectiveData>()
                         join c in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals c.tbStudent.Id
                         where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                            && c.tbClass.tbYear.IsDefault
                            && p.tbElectiveOrg.IsDeleted == false
                            && p.tbElectiveOrg.tbCourse.IsDeleted == false
                            && p.tbElectiveOrg.tbElectiveGroup.IsDeleted == false
                            && p.tbElectiveOrg.tbElectiveSection.IsDeleted == false
                            && p.tbStudent.IsDeleted == false
                         select new { tbElectiveData = p, tbClass = c.tbClass };

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbElectiveData.tbElectiveOrg.OrgName.Contains(vm.SearchText) || d.tbElectiveData.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbElectiveData.tbStudent.StudentName.Contains(vm.SearchText));
                }

                vm.ElectiveChangeList = (from p in tb
                                         orderby p.tbElectiveData.InputDate descending
                                         select new Dto.ElectiveChange.List()
                                         {
                                             ElectiveDataId = p.tbElectiveData.Id,
                                             ElectiveOrgId = p.tbElectiveData.tbElectiveOrg.Id, 
                                             OrgName = p.tbElectiveData.tbElectiveOrg.OrgName,
                                             TeacherName = p.tbElectiveData.tbElectiveOrg.tbTeacher.TeacherName,
                                             RoomName = p.tbElectiveData.tbElectiveOrg.tbRoom.RoomName,
                                             RemainCount = p.tbElectiveData.tbElectiveOrg.RemainCount,
                                             ElectiveSectionName = p.tbElectiveData.tbElectiveOrg.tbElectiveSection.ElectiveSectionName,
                                             ElectiveGroupName = p.tbElectiveData.tbElectiveOrg.tbElectiveGroup.ElectiveGroupName,
                                             InputDate = p.tbElectiveData.InputDate,
                                             StudentId = p.tbElectiveData.tbStudent.Id,
                                             StudentUserId = p.tbElectiveData.tbStudent.tbSysUser.Id,
                                             StudentCode = p.tbElectiveData.tbStudent.StudentCode,
                                             StudentName = p.tbElectiveData.tbStudent.StudentName,
                                             IsPreElective = p.tbElectiveData.IsPreElective,
                                             MaxCount = p.tbElectiveData.tbElectiveOrg.MaxCount,
                                             ClassName=p.tbClass.ClassName
                                         }).ToPageList(vm.Page);

                //foreach (var a in vm.ElectiveChangeList)
                //{
                //    a.ClassName = classList.Where(c => c.StudentId == a.StudentId).Select(c => c.ClassName).FirstOrDefault();
                //}

                if (vm.IsWeekPeriod)
                {
                    vm.IsHiddenSection = true;
                    vm.IsHiddenGroup = true;

                    if (vm.IsWeekPeriod)
                    {
                        var orgIds = vm.ElectiveChangeList.Select(p => p.ElectiveOrgId);
                        var electiveOrgSchedule = (from p in db.Table<Entity.tbElectiveOrgSchedule>().Include(p => p.tbElectiveOrg).Include(p => p.tbWeek).Include(p => p.tbPeriod) where orgIds.Contains(p.tbElectiveOrg.Id) select p).ToList();

                        vm.ElectiveChangeList.ForEach(p =>
                        {
                            var weekPeriodName = new List<string>();
                            electiveOrgSchedule.ForEach(c =>
                            {
                                if (p.ElectiveOrgId == c.tbElectiveOrg.Id)
                                {
                                    weekPeriodName.Add($"{c.tbWeek?.WeekName}{c.tbPeriod?.PeriodName}");
                                }
                            });
                            p.WeekPeriod = string.Join(",", weekPeriodName);
                        });
                    }
                }
                else
                {
                    vm.IsHiddenSection = vm.ElectiveChangeList.Select(d => d.ElectiveSectionName).Distinct().Count() <= 1;
                    vm.IsHiddenGroup = vm.ElectiveChangeList.Select(d => d.ElectiveGroupName).Distinct().Count() <= 1;
                }

                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult List(Models.ElectiveChange.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                electiveId = vm.ElectiveId,
                searchText = vm.SearchText,
                pageSize = vm.Page.PageSize,
                pageIndex = vm.Page.PageIndex
            }));
        }

        public ActionResult Select()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveChange.Select();
                vm.IsWeekPeriod = (from p in db.Set<Entity.tbElective>()
                                   where p.Id == vm.ElectiveId
                                   select p.tbElectiveType.ElectiveTypeCode).FirstOrDefault() == Code.EnumHelper.ElectiveType.WeekPeriod;

                var limitClassList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                      join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                      where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId &&
                                          p.tbElectiveOrg.IsDeleted == false &&
                                          q.tbStudent.tbSysUser.Id == vm.UserId
                                      select p.tbElectiveOrg.Id).ToList();


                var electiveType = db.Table<Entity.tbElective>().Select(p => p.tbElectiveType.ElectiveTypeCode).FirstOrDefault();

                if (electiveType == Code.EnumHelper.ElectiveType.WeekPeriod)
                {
                    //星期课表选课
                    vm.ElectiveOrgList=(from p in db.Table<Entity.tbElectiveOrg>()
                                        join s in db.Table<Entity.tbElectiveOrgSchedule>() on p.Id equals s.tbElectiveOrg.Id
                                        join d in db.Table<Entity.tbElectiveData>() on p.Id equals d.tbElectiveOrg.Id into electiveData
                                        from ed in electiveData.DefaultIfEmpty()                                       
                                        where p.tbElective.Id==vm.ElectiveId && (!p.IsPermitClass || limitClassList.Contains(p.Id))
                                        orderby p.OrgName
                                        select new Dto.ElectiveChange.Select()
                                        {
                                            Id = p.Id,
                                            IsChecked = p.Id == vm.ElectiveOrgId || (ed!=null && ed.tbElectiveOrg.Id==p.Id),
                                            OrgName = p.OrgName,
                                            RemainCount = p.RemainCount,
                                            TeacherName = p.tbTeacher.TeacherName,
                                            RoomName = p.tbRoom.RoomName,
                                            ElectiveSectionName = p.tbElectiveSection.ElectiveSectionName,
                                            ElectiveGroupName = p.tbElectiveGroup.ElectiveGroupName
                                        }).Distinct().ToList();
                }
                else
                {
                    //列表选课
                    vm.ElectiveOrgList = (from p in db.Table<Entity.tbElectiveOrg>()
                                          join d in db.Table<Entity.tbElectiveData>() on p.Id equals d.tbElectiveOrg.Id into electiveData
                                          from ed in electiveData.DefaultIfEmpty()
                                          where p.tbElective.Id == vm.ElectiveId
                                            && (p.IsPermitClass == false || limitClassList.Contains(p.Id))
                                          orderby p.OrgName
                                          select new Dto.ElectiveChange.Select()
                                          {
                                              Id = p.Id,
                                              IsChecked = p.Id == vm.ElectiveOrgId || (ed != null && ed.tbElectiveOrg.Id == p.Id),
                                              OrgName = p.OrgName,
                                              RemainCount = p.RemainCount,
                                              TeacherName = p.tbTeacher.TeacherName,
                                              RoomName = p.tbRoom.RoomName,
                                              ElectiveSectionName = p.tbElectiveSection.ElectiveSectionName,
                                              ElectiveGroupName = p.tbElectiveGroup.ElectiveGroupName
                                          }).OrderByDescending(p => p.IsChecked).ToList();
                }
                
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
                var ids = Request.Form["rdoId"];
                if (string.IsNullOrWhiteSpace(ids))
                {
                    error.AddError("请选择一项再提交!");
                    return Code.MvcHelper.Post(error);
                }
                var success = false;
                var idArray = ids.Split(',').Select(int.Parse).ToArray();
                var newOrgId = idArray[0];
                var oldOrgId = idArray[1];
                var studentId = idArray[2];
                do
                {
                    try
                    {
                        //var newElectiveOrg = (from p in db.Table<Entity.tbElectiveOrg>().Include(p=>p.tbElective.tbElectiveType) where p.Id==newOrgId select p).FirstOrDefault();

                        var newElectiveOrg = db.Set<Entity.tbElectiveOrg>().Find(newOrgId);
                        if (newElectiveOrg == null || newElectiveOrg.RemainCount == 0)
                        {
                            error.AddError("班级人数已满");
                            return Code.MvcHelper.Post(error);
                        }

                        var electiveOrgCourseId = (from p in db.Table<Entity.tbElectiveOrg>()
                                                   where p.Id == newOrgId
                                                   select p.tbCourse.Id).FirstOrDefault();
                        var checkCourse = (from p in db.Table<Entity.tbElectiveData>()
                                           where p.tbElectiveOrg.Id != newOrgId
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


                        #region 判断新课程的人数限制

                        //当前选择的课程所属分段
                        var electiveOrgInfo = (from p in db.Table<Entity.tbElectiveOrg>() where p.Id == newOrgId
                                               select new
                                               {
                                                   tbElectiveSection =p.tbElectiveSection,
                                                   tbElectiveGroup =p.tbElectiveGroup
                                               }).FirstOrDefault();

                        var electiveSection = electiveOrgInfo.tbElectiveSection;
                        //当前选择课程所属分段下已选课程数
                        var exNum = (from p in db.Table<Entity.tbElectiveData>()
                                     where p.tbElectiveOrg.tbElectiveSection.Id == electiveSection.Id && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                     select p).Count();

                        if (exNum + 1 > electiveSection.MaxElective)
                        {
                            error.AddError($"已选人数[{exNum + 1}]超出分段[{electiveSection.ElectiveSectionName}]的最大人数[{electiveSection.MaxElective}]");
                            return Code.MvcHelper.Post(error);
                        }

                        //当前选择的课程所属分组
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

                        #endregion


                        //删除旧数据
                        var oldElectiveData = (from p in db.Set<Entity.tbElectiveData>()
                                               where p.IsDeleted == false && p.tbElectiveOrg.Id == oldOrgId && p.tbStudent.Id == studentId
                                               select new
                                               {
                                                   p,
                                                   p.tbStudent,
                                                   p.tbElectiveOrg
                                               }).FirstOrDefault();
                        oldElectiveData.p.IsDeleted = true;
                        oldElectiveData.tbElectiveOrg.RemainCount++;

                        if (oldElectiveData.p.IsPreElective)
                        {
                            var tbElectiveOrgStudent = (from p in db.Set<Entity.tbElectiveOrgStudent>()
                                                        where p.tbElectiveOrg.Id == oldElectiveData.p.Id && p.tbStudent.Id == oldElectiveData.tbStudent.Id && p.IsDeleted == false
                                                        select p).FirstOrDefault();
                            if (tbElectiveOrgStudent != null)
                            {
                                tbElectiveOrgStudent.IsDeleted = true;
                            }
                        }

                        var electiveData = new Entity.tbElectiveData()
                        {
                            //IsPreElective = true,
                            tbElectiveOrg = newElectiveOrg,
                            tbStudent = db.Set<Student.Entity.tbStudent>().Find(oldElectiveData.tbStudent.Id),
                            InputDate = DateTime.Now
                        };
                        //添加新数据
                        db.Set<Entity.tbElectiveData>().Add(electiveData);

                        db.Set<Entity.tbElectiveOrgStudent>().Add(new Entity.tbElectiveOrgStudent()
                        {
                            tbElectiveOrg = newElectiveOrg,
                            tbStudent = electiveData.tbStudent,
                        });
                        newElectiveOrg.RemainCount--;
                        db.SaveChanges();
                        success = true;
                    }
                    catch (DbUpdateConcurrencyException exception)
                    {
                        exception.Entries.Single().Reload();
                    }
                } while (!success);
                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tbElectiveData = (from p in db.Set<Entity.tbElectiveData>()
                                        .Include(d => d.tbElectiveOrg)
                                        .Include(d => d.tbStudent)
                                      where ids.Contains(p.Id)
                                      select p).ToList();
                foreach (var item in tbElectiveData)
                {
                    item.IsDeleted = true;
                    item.tbElectiveOrg.RemainCount++;

                    var tbElectiveOrgStudent = (from p in db.Set<Entity.tbElectiveOrgStudent>()
                                                where p.tbElectiveOrg.Id == item.tbElectiveOrg.Id
                                                    && p.tbStudent.Id == item.tbStudent.Id
                                                select p).FirstOrDefault();
                    if (tbElectiveOrgStudent != null) 
                    {
                        tbElectiveOrgStudent.IsDeleted = true;
                    }
                    //if (item.IsPreElective)
                    //{
                    //    var tbElectiveOrgStudent = (from p in db.Set<Entity.tbElectiveOrgStudent>()
                    //                                where p.tbElectiveOrg.Id == item.tbElectiveOrg.Id 
                    //                                    && p.tbStudent.Id == item.tbStudent.Id
                    //                                select p).FirstOrDefault();
                    //    if (tbElectiveOrgStudent != null)
                    //    {
                    //        tbElectiveOrgStudent.IsDeleted = true;
                    //    }
                    //}
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("调整了学生选课信息");
                }
            }

            return Code.MvcHelper.Post();
        }
    }
}