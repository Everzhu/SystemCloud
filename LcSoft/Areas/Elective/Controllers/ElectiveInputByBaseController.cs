using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveInputByBaseController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveInputByBase.List();
                vm.Student = (from p in db.Table<Student.Entity.tbStudent>()
                              where p.tbSysUser.Id == Code.Common.UserId
                              select new Student.Dto.Student.SelectStudent()
                              {
                                  StudentCode = p.StudentCode,
                                  StudentName = p.StudentName
                              }).FirstOrDefault();

                var elective = (from p in db.Table<Entity.tbElective>()
                                where p.Id == vm.ElectiveId
                                select new
                                {
                                    p.ElectiveName,
                                    p.FromDate,
                                    p.ToDate,
                                    p.tbElectiveType.ElectiveTypeCode
                                }).FirstOrDefault();
                if (elective != null)
                {
                    vm.IsWeekPeriod = elective.ElectiveTypeCode == Code.EnumHelper.ElectiveType.WeekPeriod;
                    vm.ElectiveName = elective.ElectiveName;
                    vm.IsOpen = elective.FromDate < DateTime.Now;
                    vm.IsEnd = elective.ToDate < DateTime.Now;
                }

                var limitOrgList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                      join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                      where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                        && p.tbElectiveOrg.IsDeleted == false
                                        && q.tbStudent.IsDeleted == false
                                        && q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                      select p.tbElectiveOrg.Id).ToList();

                var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                          where p.tbElective.Id == vm.ElectiveId
                             && p.tbCourse.IsDeleted == false
                             && p.tbElectiveGroup.IsDeleted == false
                             && p.tbElectiveSection.IsDeleted == false
                             && (p.IsPermitClass == false || limitOrgList.Contains(p.Id))
                          orderby p.tbElectiveGroup.Id
                          select new
                          {
                              p.Id,
                              p.OrgName,
                              p.MaxCount,
                              p.RemainCount,
                              CourseId = p.tbCourse.Id,
                              p.tbCourse.CourseName,
                              p.tbTeacher.TeacherName,
                              p.Permit,
                              RoomName = p.tbRoom.RoomName,
                              ElectiveSectionId = p.tbElectiveSection.Id,
                              ElectiveSectionNo = p.tbElectiveSection.No,
                              ElectiveSectionName = p.tbElectiveSection.ElectiveSectionName,
                              ElectiveSectionMinElective = p.tbElectiveSection.MinElective,
                              ElectiveSectionMaxElective = p.tbElectiveSection.MaxElective,
                              ElectiveGroupId = p.tbElectiveGroup.Id,
                              ElectiveGroupNo = p.tbElectiveGroup.No,
                              ElectiveGroupName = p.tbElectiveGroup.ElectiveGroupName,
                              ElectiveGroupMinElective = p.tbElectiveGroup.MinElective,
                              ElectiveGroupMaxElective = p.tbElectiveGroup.MaxElective,
                          }).ToList();

                vm.ElectiveSectionList = (from p in tb
                                          group p by new { p.ElectiveSectionId, p.ElectiveSectionNo, p.ElectiveSectionName, p.ElectiveSectionMaxElective, p.ElectiveSectionMinElective } into g
                                          select new Dto.ElectiveSection.Info
                                          {
                                              Id = g.Key.ElectiveSectionId,
                                              No = g.Key.ElectiveSectionNo,
                                              ElectiveSectionName = g.Key.ElectiveSectionName,
                                              MaxElective = g.Key.ElectiveSectionMaxElective,
                                              MinElective = g.Key.ElectiveSectionMinElective
                                          }).OrderBy(d => d.No).ToList();

                vm.ElectiveGroupList = (from p in tb
                                        group p by new { p.ElectiveGroupId, p.ElectiveGroupNo, p.ElectiveGroupName, p.ElectiveGroupMaxElective, p.ElectiveGroupMinElective } into g
                                        select new Dto.ElectiveGroup.Info
                                        {
                                            Id = g.Key.ElectiveGroupId,
                                            No = g.Key.ElectiveGroupNo,
                                            ElectiveGroupName = g.Key.ElectiveGroupName,
                                            MaxElective = g.Key.ElectiveGroupMaxElective,
                                            MinElective = g.Key.ElectiveGroupMinElective
                                        }).OrderBy(d => d.No).ToList();

                var electiveDataList = (from p in db.Table<Entity.tbElectiveData>()
                                        where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                         && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        select new
                                        {
                                            p.tbElectiveOrg.Id,
                                            p.IsFixed
                                        }).ToList();

                vm.ElectiveOrgList = (from p in tb
                                      select new Dto.ElectiveOrg.List()
                                      {
                                          Id = p.Id,
                                          OrgName = p.OrgName,
                                          IsChecked = electiveDataList.Select(d => d.Id).Contains(p.Id) ? true : false,
                                          IsFixed = electiveDataList.Where(d => d.Id == p.Id).Select(d => d.IsFixed).DefaultIfEmpty().FirstOrDefault(),
                                          CourseId = p.CourseId,
                                          CourseName = p.CourseName,
                                          MaxCount = p.MaxCount,
                                          Permit=p.Permit,
                                          RoomName = p.RoomName,
                                          RemainCount = p.RemainCount,
                                          TeacherName = p.TeacherName,
                                          ElectiveGroupId = p.ElectiveGroupId,
                                          ElectiveGroupName = p.ElectiveGroupName,
                                          ElectiveGroupMinElective = p.ElectiveGroupMinElective,
                                          ElectiveGroupMaxElective = p.ElectiveGroupMaxElective,
                                          ElectiveSectionId = p.ElectiveSectionId,
                                          ElectiveSectionMinElective = p.ElectiveSectionMinElective,
                                          ElectiveSectionMaxElective = p.ElectiveSectionMaxElective,
                                      }).ToList();

                var orgStudent = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                                  where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                  select p.tbElectiveOrg.Id).ToList();
                //白名单
                vm.ElectiveOrgList.RemoveAll(d => d.Permit == 1 && orgStudent.Contains(d.Id) == false);
                //黑名单
                vm.ElectiveOrgList.RemoveAll(d => d.Permit == -1 && orgStudent.Contains(d.Id));

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

        public ActionResult Modal()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveInputByBase.List();
                vm.Student = (from p in db.Table<Student.Entity.tbStudent>()
                              where p.tbSysUser.Id == Code.Common.UserId
                              select new Student.Dto.Student.SelectStudent()
                              {
                                  StudentCode = p.StudentCode,
                                  StudentName = p.StudentName
                              }).FirstOrDefault();

                var elective = (from p in db.Table<Entity.tbElective>()
                                where p.Id == vm.ElectiveId
                                select new
                                {
                                    p.ElectiveName,
                                    p.FromDate,
                                    p.ToDate,
                                    p.tbElectiveType.ElectiveTypeCode
                                }).FirstOrDefault();
                if (elective != null)
                {
                    vm.IsWeekPeriod = elective.ElectiveTypeCode == Code.EnumHelper.ElectiveType.WeekPeriod;
                    vm.ElectiveName = elective.ElectiveName;
                    vm.IsOpen = elective.FromDate < DateTime.Now;
                    vm.IsEnd = elective.ToDate < DateTime.Now;
                }

                var limitOrgList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                    join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                    where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                      && p.tbElectiveOrg.IsDeleted == false
                                      && q.tbStudent.IsDeleted == false
                                      && q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                    select p.tbElectiveOrg.Id).ToList();

                var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                          where p.tbElective.Id == vm.ElectiveId
                             && p.tbCourse.IsDeleted == false
                             && p.tbElectiveGroup.IsDeleted == false
                             && p.tbElectiveSection.IsDeleted == false
                             && (p.IsPermitClass == false || limitOrgList.Contains(p.Id))
                          orderby p.tbElectiveGroup.Id
                          select new
                          {
                              //p.Id,
                              //p.OrgName,
                              //p.MaxCount,
                              //p.RemainCount,
                              //CourseId = p.tbCourse.Id,
                              //p.tbCourse.CourseName,
                              //p.tbTeacher.TeacherName,
                              //RoomName = p.tbRoom.RoomName,
                              ElectiveSectionId = p.tbElectiveSection.Id,
                              ElectiveSectionNo = p.tbElectiveSection.No,
                              ElectiveSectionName = p.tbElectiveSection.ElectiveSectionName,
                              ElectiveSectionMinElective = p.tbElectiveSection.MinElective,
                              ElectiveSectionMaxElective = p.tbElectiveSection.MaxElective,
                              ElectiveGroupId = p.tbElectiveGroup.Id,
                              ElectiveGroupNo = p.tbElectiveGroup.No,
                              ElectiveGroupName = p.tbElectiveGroup.ElectiveGroupName,
                              ElectiveGroupMinElective = p.tbElectiveGroup.MinElective,
                              ElectiveGroupMaxElective = p.tbElectiveGroup.MaxElective,
                          }).ToList();

                vm.ElectiveSectionList = (from p in tb
                                          group p by new { p.ElectiveSectionId, p.ElectiveSectionNo, p.ElectiveSectionName, p.ElectiveSectionMaxElective, p.ElectiveSectionMinElective } into g
                                          select new Dto.ElectiveSection.Info
                                          {
                                              Id = g.Key.ElectiveSectionId,
                                              No = g.Key.ElectiveSectionNo,
                                              ElectiveSectionName = g.Key.ElectiveSectionName,
                                              MaxElective = g.Key.ElectiveSectionMaxElective,
                                              MinElective = g.Key.ElectiveSectionMinElective
                                          }).OrderBy(d => d.No).ToList();

                vm.ElectiveGroupList = (from p in tb
                                        group p by new { p.ElectiveGroupId, p.ElectiveGroupNo, p.ElectiveGroupName, p.ElectiveGroupMaxElective, p.ElectiveGroupMinElective } into g
                                        select new Dto.ElectiveGroup.Info
                                        {
                                            Id = g.Key.ElectiveGroupId,
                                            No = g.Key.ElectiveGroupNo,
                                            ElectiveGroupName = g.Key.ElectiveGroupName,
                                            MaxElective = g.Key.ElectiveGroupMaxElective,
                                            MinElective = g.Key.ElectiveGroupMinElective
                                        }).OrderBy(d => d.No).ToList();

                var electiveDataList = (from p in db.Table<Entity.tbElectiveData>()
                                        where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                         && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        select new
                                        {
                                            p.tbElectiveOrg.Id,
                                            p.IsFixed,
                                            p.tbElectiveOrg.OrgName,
                                            GroupId=p.tbElectiveOrg.tbElectiveGroup.Id,
                                            SectionId=p.tbElectiveOrg.tbElectiveSection.Id,
                                            Permit=p.tbElectiveOrg.Permit,
                                            p.tbElectiveOrg.tbTeacher.TeacherName,
                                            p.tbElectiveOrg.tbRoom.RoomName,
                                            p.tbElectiveOrg.RemainCount
                                        }).ToList();

                //已选列表
                vm.ElectiveOrgList = (from p in electiveDataList
                                      select new Dto.ElectiveOrg.List()
                                      {
                                          OrgName=p.OrgName,
                                          Id=p.Id,
                                          IsFixed=p.IsFixed,
                                          IsChecked=true,
                                          ElectiveGroupId=p.GroupId,
                                          ElectiveSectionId=p.SectionId,
                                          Permit=p.Permit,
                                          TeacherName=p.TeacherName,
                                          RoomName=p.RoomName,
                                          RemainCount=p.RemainCount
                                      }).ToList();

                var orgStudent = (from p in db.Table<Entity.tbElectiveOrgStudent>()
                                  where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                  select p.tbElectiveOrg.Id).ToList();
                //白名单
                vm.ElectiveOrgList.RemoveAll(d => d.Permit == 1 && orgStudent.Contains(d.Id) == false);
                //黑名单
                vm.ElectiveOrgList.RemoveAll(d => d.Permit == -1 && orgStudent.Contains(d.Id));

                
                vm.IsHiddenSection = vm.ElectiveOrgList.Select(d => d.ElectiveSectionName).Distinct().Count() <= 1;
                vm.IsHiddenGroup = vm.ElectiveOrgList.Select(d => d.ElectiveGroupName).Distinct().Count() <= 1;
                
                return View(vm);
            }
        }


        public ActionResult History()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveInputByBase.History();
                var electiveModel = db.Set<Entity.tbElective>().Find(vm.ElectiveId);
                vm.ElectiveName = electiveModel.ElectiveName;

                vm.ElectiveOrgList = (from p in db.Table<Entity.tbElectiveOrg>()
                                      join d in db.Table<Entity.tbElectiveData>() on p.Id equals d.tbElectiveOrg.Id
                                      where p.tbElective.Id == vm.ElectiveId
                                         && p.tbCourse.IsDeleted == false
                                         && p.tbElectiveGroup.IsDeleted == false
                                         && p.tbElectiveSection.IsDeleted == false
                                         && d.IsDeleted == false
                                         && d.tbStudent.tbSysUser.Id == Code.Common.UserId
                                      orderby p.tbElectiveSection.Id, p.tbElectiveGroup.Id
                                      select new Dto.ElectiveOrg.List()
                                      {
                                          OrgName = p.OrgName,
                                          CourseId = p.tbCourse.Id,
                                          CourseName = p.tbCourse.CourseName,
                                          TeacherName = p.tbTeacher.TeacherName,
                                          RoomName = p.tbRoom.RoomName,
                                          RemainCount = p.RemainCount,
                                          ElectiveSectionName = p.tbElectiveSection.ElectiveSectionName,
                                          ElectiveGroupName = p.tbElectiveGroup.ElectiveGroupName,
                                          InputDate = d.InputDate
                                      }).ToList();

                vm.Student = (from p in db.Table<Student.Entity.tbStudent>()
                              where p.tbSysUser.Id == Code.Common.UserId
                              select new Student.Dto.Student.SelectStudent()
                              {
                                  StudentCode = p.StudentCode,
                                  StudentName = p.StudentName
                              }).FirstOrDefault();

                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult List(Models.ElectiveInputByBase.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var orgIds = new List<int>();
                if (Request["CboxOrg"] != null)
                {
                    orgIds = Request["CboxOrg"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                }

                var result = Save(db, vm.ElectiveId, orgIds);
                if (string.IsNullOrEmpty(result) == false)
                {
                    error.Add(result);
                }

                return Code.MvcHelper.Post(error, Url.Action("List", new { electiveId = vm.ElectiveId }), "提交成功!");
            }
        }

        private static string Save(XkSystem.Models.DbContext db, int electiveId, List<int> orgIds)
        {
            try
            {
                var elective = (from p in db.Table<Entity.tbElective>()
                                where p.Id == electiveId
                                select new
                                {
                                    p.FromDate,
                                    p.ToDate,
                                    p.tbElectiveType.ElectiveTypeCode
                                }).FirstOrDefault();
                if (elective != null)
                {
                    var isOpen = elective.FromDate < DateTime.Now;
                    var isEnd = elective.ToDate < DateTime.Now;

                    if (isOpen == false || isEnd)
                    {
                        return "选课未开放!";
                    }
                }

                //var limitOrgList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                //                    join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                //                    where p.tbElectiveOrg.tbElective.Id == electiveId
                //                      && p.tbElectiveOrg.IsDeleted == false
                //                      && q.tbStudent.tbSysUser.Id == Code.Common.UserId
                //                    select p.tbElectiveOrg.Id).ToList();

                var limitOrgList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                    join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                    where orgIds.Contains(p.tbElectiveOrg.Id)
                                      && p.tbElectiveOrg.IsDeleted == false
                                      && q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                    select new { p.tbElectiveOrg, tbElectiveOrgClass = p }).ToList();

                foreach (var item in limitOrgList)
                {
                    if (item.tbElectiveOrgClass.RemainCount <= 0)
                    {
                        return $"选课{item.tbElectiveOrg.OrgName}针对当前行政班级的人数限制已满！";
                    }
                }


                var litmitOrgIds = limitOrgList.Select(p => p.tbElectiveOrg.Id).ToList();

                var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                          where p.tbElective.Id == electiveId
                             && p.tbCourse.IsDeleted == false
                             && p.tbElectiveGroup.IsDeleted == false
                             && p.tbElectiveSection.IsDeleted == false
                             && (p.IsPermitClass == false || litmitOrgIds.Contains(p.Id))
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

                var orgList = (from p in tb
                               where orgIds.Contains(p.p.Id)
                               select new
                               {
                                   p,
                                   SectionId = p.SectionId,
                                   GroupId = p.GroupId
                               }).ToList();

                foreach (var group in groupList)
                {
                    if (orgList.Where(d => d.GroupId == group.GroupId).Count() < group.GroupMin)
                    {
                        return "分组[" + group.GroupName + "]要求最少选择 " + group.GroupMin + " 门!";
                    }
                    else if (orgList.Where(d => d.GroupId == group.GroupId).Count() > group.GroupMax)
                    {
                        return "超出分组[" + group.GroupName + "]的最大选课数要求，最多只能选择 " + group.GroupMax + " 门!";
                    }
                }

                foreach (var sec in sectionList)
                {
                    if (orgList.Where(d => d.SectionId == sec.SectionId).Count() < sec.SectionMin)
                    {
                        return "分段[" + sec.SectionName + "]要求最少选择 " + sec.SectionMin + " 门!";
                    }
                    else if (orgList.Where(d => d.SectionId == sec.SectionId).Count() > sec.SectionMax)
                    {
                        return "超出分段[" + sec.SectionName + "]的最大选课数要求，最多只能选择 " + sec.SectionMax + " 门!";
                    }
                }

                //已选课程
                var electiveDataList = (from p in db.Table<Entity.tbElectiveData>()
                                            .Include(d => d.tbElectiveOrg)
                                        where p.tbElectiveOrg.IsDeleted == false
                                            && p.tbElectiveOrg.tbElective.Id == electiveId
                                            && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        select p).ToList();

                //判断课程剩余名额
                foreach (var orgId in orgIds.Where(d => electiveDataList.Select(p => p.tbElectiveOrg.Id).Contains(d) == false))
                {
                    var tbOrg = orgList.Where(d => d.p.p.Id == orgId).FirstOrDefault();
                    if (tbOrg.p.p.RemainCount <= 0)
                    {
                        return "[" + tbOrg.p.p.OrgName + "]的选课人数已满";
                    }
                    else if ((tbOrg.p.p.MaxCount - ElectiveDataController.GetCount(db, tbOrg.p.p.Id)) <= 0)
                    {
                        tbOrg.p.p.RemainCount = tbOrg.p.p.MaxCount - ElectiveDataController.GetCount(db, tbOrg.p.p.Id);
                        db.SaveChanges();
                        return "[" + tbOrg.p.p.OrgName + "]的选课人数已满";
                    }
                }

                //删除已选
                var mydataIdList = electiveDataList.Select(d => d.Id).ToList();
                foreach (var electiveData in electiveDataList.Where(d => orgIds.Contains(d.tbElectiveOrg.Id) == false))
                {
                    electiveData.IsDeleted = true;
                    electiveData.tbElectiveOrg.RemainCount++;
                    var limitClass=limitOrgList.Where(p => p.tbElectiveOrgClass.tbElectiveOrg.Id == electiveData.tbElectiveOrg.Id).Select(p=>p.tbElectiveOrgClass).FirstOrDefault();
                    if (limitClass != null) {
                        limitClass.RemainCount++;
                    }
                    mydataIdList.Remove(electiveData.Id);
                }

                db.SaveChanges();
                
                var student = db.Table<Student.Entity.tbStudent>().FirstOrDefault(p => p.tbSysUser.Id == Code.Common.UserId);
                foreach (var orgId in orgIds.Where(d => electiveDataList.Select(p => p.tbElectiveOrg.Id).Contains(d) == false))
                {
                    //更新ElectiveOrg
                    var electiveOrg = orgList.Where(d => d.p.p.Id == orgId).FirstOrDefault();
                    if (electiveOrg.p.p.RemainCount > 0)
                    {
                        electiveOrg.p.p.RemainCount--;

                        var limitClass = limitOrgList.Where(p => p.tbElectiveOrgClass.tbElectiveOrg.Id == orgId).Select(p => p.tbElectiveOrgClass).FirstOrDefault();
                        if (limitClass != null)
                        {
                            limitClass.RemainCount--;
                        }

                        var data = new Entity.tbElectiveData();
                        data.tbStudent = student;
                        data.tbElectiveOrg = electiveOrg.p.p;
                        data.InputDate = DateTime.Now;
                        db.Set<Entity.tbElectiveData>().Add(data);

                        db.SaveChanges();
                        mydataIdList.Add(data.Id);
                    }
                    else
                    {
                        return "[" + electiveOrg.p.p.OrgName + "]的选课人数已满";
                    }
                }

                //db.SaveChanges();


                //删除重复的数据，防止同一节次添加多条记录(主要是解决高并发下会重复插入数据的问题)
                var invalidDataList = (from p in db.Table<Entity.tbElectiveData>()
                                       join q in db.Table<Entity.tbElectiveOrgSchedule>() on p.tbElectiveOrg.Id equals q.tbElectiveOrg.Id
                                       where p.tbElectiveOrg.IsDeleted == false
                                        && p.tbElectiveOrg.tbElective.Id == electiveId
                                        && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        && mydataIdList.Contains(p.Id) == false
                                       select p).Include(d => d.tbElectiveOrg).ToList();
                foreach (var a in invalidDataList)
                {
                    a.tbElectiveOrg.RemainCount++;

                    a.IsDeleted = true;
                }

                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                exception.Entries.Single().Reload();
                Save(db, electiveId, orgIds);
            }

            return string.Empty;
        }
    }
}