using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class CalendarController : Controller
    {
        private static readonly string[] TbWeek = { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };

        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Calendar.List();
                vm.WeekList = WeekController.SelectList();

                vm.YearList = YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearId > 0 && vm.YearList.Exists(p => int.Parse(p.Value) == vm.YearId.Value))
                {
                    vm.YearList.ForEach(p => {
                        p.Selected = int.Parse(p.Value) == vm.YearId;
                    });
                }
                //获取当前激活学段的起始日期
                if (vm.FromDate == Code.DateHelper.MinDate || vm.ToDate.Value == Code.DateHelper.MinDate || vm.ToDate.Value < vm.FromDate.Value)
                {
                    var yearId = vm.YearId.HasValue ? vm.YearId.Value : 0;
                    if (yearId == 0)
                    {
                        yearId = vm.YearList.Where(p => p.Selected).Select(p => p.Value).FirstOrDefault().ConvertToInt();
                    }

                    var tbYear = YearController.SelectInfo(yearId);
                    if (tbYear != null)
                    {
                        vm.FromDate = tbYear.FromDate.HasValue ? tbYear.FromDate.Value : Code.DateHelper.MonthFirstDay;
                        vm.ToDate = tbYear.ToDate.HasValue ? tbYear.ToDate.Value : DateTime.Now;
                    }
                    else
                    {
                        vm.FromDate = Code.DateHelper.MonthFirstDay;
                        vm.ToDate = DateTime.Now;
                    }
                }
                vm.ToDate = vm.ToDate.Value.AddDays(1).AddSeconds(-1);

                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }


                vm.DataList = (from p in db.Table<Entity.tbCalendar>()
                               where p.tbYear.Id == vm.YearId && p.CalendarDate >= vm.FromDate && p.CalendarDate <= vm.ToDate
                               select new Dto.Calendar.List()
                               {
                                   Id = p.Id,
                                   tbWeekId = p.tbWeek.Id,
                                   tbWeekName = p.tbWeek.WeekName,
                                   tbYear = p.tbYear.YearName,
                                   CalendarDate = p.CalendarDate,
                                   Remark = p.Remark
                               }).ToList();
                vm.DataList.ForEach(p =>
                {
                    p.tbWeekName = TbWeek[p.tbWeekId - 1];
                });
                return View(vm);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Calendar.List vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("List", new
            {
                YearId = vm.YearId,
                FromDate = vm.FromDate,
                ToDate = vm.ToDate
            }));
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.Calendar.Edit();

            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var tb = (from p in db.Table<Basis.Entity.tbCalendar>()
                              where p.tbYear.Id == vm.YearId && p.CalendarDate == vm.CalendarDate
                              select new Dto.Calendar.Edit()
                              {
                                  Id = p.Id,
                                  tbWeekId = p.tbWeek.Id,
                                  tbYearId = p.tbYear.Id,
                                  CalendarDate = p.CalendarDate,
                                  Remark = p.Remark
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.CalendarEdit = tb;
                    }
                }
            }
            else
            {
                vm.CalendarEdit.tbYearId = vm.YearId;
                vm.CalendarDate = vm.CalendarDate;
            }
            vm.WeekList = WeekController.SelectList();
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Calendar.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (vm.CalendarEdit.Id > 0)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbCalendar>() where p.Id == vm.CalendarEdit.Id select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.Remark = vm.CalendarEdit.Remark;
                        tb.tbWeek = db.Set<Basis.Entity.tbWeek>().Find(vm.CalendarEdit.tbWeekId);

                        if (db.SaveChanges() > 0)
                        {
                            Sys.Controllers.SysUserLogController.Insert("修改了校历！");
                        }
                    }
                    else
                    {
                        return Code.MvcHelper.Post(new List<string>() { Resources.LocalizedText.MsgNotFound });
                    }
                }
                else
                {
                    var tb = new Basis.Entity.tbCalendar()
                    {
                        CalendarDate = vm.CalendarDate,
                        tbWeek = db.Set<Basis.Entity.tbWeek>().Find(vm.CalendarEdit.tbWeekId),
                        tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId),
                        Remark = vm.CalendarEdit.Remark
                    };
                    db.Set<Basis.Entity.tbCalendar>().Add(tb);
                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("添加了校历！");
                    }
                }
            }
            return Code.MvcHelper.Post();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id=0)
        {
            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var tb = (from p in db.Table<Basis.Entity.tbCalendar>() where p.Id == id select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.IsDeleted = true;
                    }
                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("删除了校历！");
                    }
                }
            }
            return Code.MvcHelper.Post();
        }


    }
}