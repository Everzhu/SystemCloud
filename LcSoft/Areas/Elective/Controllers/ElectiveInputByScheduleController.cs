using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveInputByScheduleController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveInputBySchedule.List();

                var electiveModel = db.Set<Entity.tbElective>().Find(vm.ElectiveId);
                vm.IsOpen = electiveModel != null && electiveModel.FromDate < DateTime.Now;
                vm.IsEnd = electiveModel != null && electiveModel.ToDate < DateTime.Now;
                vm.ElectiveName = electiveModel.ElectiveName;

                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectList();

                var limitClassList = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                      join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                      where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId &&
                                            p.tbElectiveOrg.IsDeleted == false &&
                                            q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                      select p.tbElectiveOrg.Id).ToList();

                var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                          join s in db.Table<Entity.tbElectiveOrgSchedule>() on p.Id equals s.tbElectiveOrg.Id
                          where p.tbElective.Id == vm.ElectiveId &&
                                p.tbCourse.IsDeleted == false &&
                                p.tbElectiveGroup.IsDeleted == false &&
                                p.tbElectiveSection.IsDeleted == false &&
                                (p.IsPermitClass == false || limitClassList.Contains(p.Id))
                          orderby p.tbElectiveGroup.Id
                          select new
                          {
                              OrgId = p.Id,
                              p.OrgName,
                              p.tbTeacher.TeacherName,
                              p.tbRoom.RoomName,
                              WeekId = s.tbWeek.Id,
                              PeriodId = s.tbPeriod.Id
                          }).ToList();

                var electiveDataList = (from p in db.Table<Entity.tbElectiveData>()
                                        join s in db.Table<Entity.tbElectiveOrgSchedule>() on  p.tbElectiveOrg.Id equals s.tbElectiveOrg.Id 
                                        where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                         && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        select new
                                        {
                                            p.tbElectiveOrg.Id,
                                            p.IsFixed,
                                            WeekId = s.tbWeek.Id,
                                            PeriodId = s.tbPeriod.Id
                                        }).ToList();

                vm.ElectiveOrgList = (from p in tb
                                      select new Dto.ElectiveInputBySchedule.List()
                                      {
                                          Id = p.OrgId,
                                          OrgName = p.OrgName,
                                          TeacherName = p.TeacherName,
                                          RoomName = p.RoomName,
                                          WeekId = p.WeekId,
                                          PeriodId = p.PeriodId,
                                          IsChecked = electiveDataList.Where(d => d.WeekId == p.WeekId && d.PeriodId == p.PeriodId).Select(d => d.Id).Contains(p.OrgId) ? true : false,
                                          IsFixed = electiveDataList.Where(d => d.Id == p.OrgId).Select(d => d.IsFixed).DefaultIfEmpty().FirstOrDefault(),
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


        public ActionResult History()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveInputBySchedule.List();
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectList();

                var elective = (from p in db.Table<Entity.tbElective>()
                                where p.Id == vm.ElectiveId
                                select new
                                {
                                    p.ElectiveName,
                                    p.FromDate,
                                    p.ToDate
                                }).FirstOrDefault();
                if (elective != null)
                {
                    vm.ElectiveName = elective.ElectiveName;
                    vm.IsOpen = elective.FromDate < DateTime.Now;
                    vm.IsEnd = elective.ToDate <= DateTime.Now;
                }

                vm.ElectiveOrgList = (from p in db.Table<Entity.tbElectiveData>()
                                      join s in db.Table<Entity.tbElectiveOrgSchedule>() on p.tbElectiveOrg.Id equals s.tbElectiveOrg.Id
                                      where p.tbElectiveOrg.tbElective.Id == vm.ElectiveId
                                       && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                      select new Dto.ElectiveInputBySchedule.List()
                                      {
                                          OrgName = p.tbElectiveOrg.OrgName,
                                          TeacherName = p.tbElectiveOrg.tbTeacher.TeacherName,
                                          RoomName = p.tbElectiveOrg.tbRoom.RoomName,
                                          WeekId = s.tbWeek.Id,
                                          PeriodId = s.tbPeriod.Id
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
    }
}