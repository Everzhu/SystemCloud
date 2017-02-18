using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveOrgScheduleController : Controller
    {
        public ActionResult List(int electiveOrgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveOrgSchedule.List();
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectList();

                vm.ElectiveOrgScheduleList = (from p in db.Table<Entity.tbElectiveOrgSchedule>()
                                              where p.tbElectiveOrg.Id== electiveOrgId
                                              orderby p.No
                                              select new Dto.ElectiveOrgSchedule.List
                                              {
                                                  Id = p.Id,
                                                  ElectiveOrgId=p.tbElectiveOrg.Id,
                                                  ElectiveOrgName = p.tbElectiveOrg.OrgName,
                                                  PeriodId=p.tbPeriod.Id,
                                                  PeriodName = p.tbPeriod.PeriodName,
                                                  WeekId=p.tbWeek.Id,
                                                  WeekName = p.tbWeek.WeekName
                                              }).ToList();
                return PartialView(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveOrgSchedule.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { electiveOrgId = vm.ElectiveOrgId }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveOrgSchedule>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了选课课表");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.ElectiveOrgSchedule.List();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ElectiveOrgSchedule.Edit vm)
        {
            var ids = Request["cBox"];
            if (string.IsNullOrWhiteSpace(ids))
            {
                var error = new List<string>();
                error.AddError("至少选择一项!");
                return Code.MvcHelper.Post(error);
            }
            using (var db = new XkSystem.Models.DbContext())
            {
                var tbElectiveOrg = db.Set<Entity.tbElectiveOrg>().Find(vm.ElectiveOrgId);
                EditOrgSchedule(db, tbElectiveOrg, ids);
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了选课开班");
                }
                return Code.MvcHelper.Post();
            }
        }

        public bool EditOrgSchedule(XkSystem.Models.DbContext db, Entity.tbElectiveOrg electiveOrg, string ids)
        {
            if (string.IsNullOrEmpty(ids) == false)
            {
                //删除之前的数据
                var delList = (from p in db.Table<Entity.tbElectiveOrgSchedule>()
                               where p.tbElectiveOrg.Id == electiveOrg.Id
                               select p).ToList();
                foreach (var a in delList)
                {
                    a.IsDeleted = true;
                }

                foreach (var v in ids.Split(','))
                {
                    var tb = new Entity.tbElectiveOrgSchedule();
                    tb.tbElectiveOrg = electiveOrg;
                    tb.tbWeek = db.Set<Basis.Entity.tbWeek>().Find(Convert.ToInt32(v.Split('_')[0]));
                    tb.tbPeriod = db.Set<Basis.Entity.tbPeriod>().Find(Convert.ToInt32(v.Split('_')[1]));
                    db.Set<Entity.tbElectiveOrgSchedule>().Add(tb);
                }
            }

            return true;
        }

    }
}