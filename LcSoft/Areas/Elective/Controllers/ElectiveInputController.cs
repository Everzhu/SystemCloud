using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveInputController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                Models.ElectiveInput.List vm = new Models.ElectiveInput.List();

                vm.ElectiveInputList = (from p in db.Table<Entity.tbElectiveClass>()
                                        join q in db.Table<Basis.Entity.tbClassStudent>()
                                        on p.tbClass.Id equals q.tbClass.Id
                                        where p.tbElective.IsDeleted == false
                                            && p.tbElective.IsDisable == false
                                            && q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        select new Dto.ElectiveInput.List()
                                        {
                                            Id = p.tbElective.Id,
                                            No = p.tbElective.No,
                                            ElectiveName = p.tbElective.ElectiveName,
                                            FromDate = p.tbElective.FromDate,
                                            ToDate = p.tbElective.ToDate,
                                            Remark = p.tbElective.Remark,
                                            IsWeekPeriod=p.tbElective.tbElectiveType.ElectiveTypeCode== Code.EnumHelper.ElectiveType.WeekPeriod,
                                            IsPop=p.tbElective.IsPop
                                        }).Distinct().OrderByDescending(p => p.No).ToList();
                return View(vm);
            }
        }

        public ActionResult History()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveInput.History();
                vm.ElectiveHistoryList = (from p in db.Table<Entity.tbElective>()
                                          join q in db.Table<Entity.tbElectiveOrg>() on p.Id equals q.tbElective.Id
                                          join d in db.Table<Entity.tbElectiveData>() on q.Id equals d.tbElectiveOrg.Id
                                          where d.tbStudent.tbSysUser.Id == Code.Common.UserId
                                          select new Dto.ElectiveInput.List()
                                          {
                                              Id = p.Id,
                                              No = p.No,
                                              ElectiveName = p.ElectiveName,
                                              FromDate = p.FromDate,
                                              ToDate = p.ToDate,                                        
                                              IsWeekPeriod=p.tbElectiveType.ElectiveTypeCode== Code.EnumHelper.ElectiveType.WeekPeriod
                                          }).Distinct().ToList();
                return View(vm);
            }
        }
    }
}