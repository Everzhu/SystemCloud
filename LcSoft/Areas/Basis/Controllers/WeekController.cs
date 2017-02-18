using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class WeekController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Week.List();
                var tb = from p in db.Table<Basis.Entity.tbWeek>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.WeekName.Contains(vm.SearchText));
                }

                vm.WeekList = (from p in tb
                               orderby p.No
                               select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Week.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbWeek>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了星期");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Week.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbWeek>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.WeekEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Week.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Basis.Entity.tbWeek>().Where(d=>d.WeekName == vm.WeekEdit.WeekName && d.Id != vm.WeekEdit.Id).Any())
                    {
                        error.AddError("该星期已存在!");
                    }
                    else
                    {
                        if (vm.WeekEdit.Id == 0)
                        {
                            var tb = new Basis.Entity.tbWeek();
                            tb.No = vm.WeekEdit.No == null ? db.Table<Basis.Entity.tbWeek>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.WeekEdit.No;
                            tb.WeekName = vm.WeekEdit.WeekName;
                            db.Set<Basis.Entity.tbWeek>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了星期");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Basis.Entity.tbWeek>()
                                      where p.Id == vm.WeekEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.WeekEdit.No == null ? db.Table<Basis.Entity.tbWeek>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.WeekEdit.No;
                                tb.WeekName = vm.WeekEdit.WeekName;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了星期");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbWeek>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.WeekName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectScheduleList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbWeek>()
                          where p.WeekName != "星期六" && p.WeekName != "星期日"
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.WeekName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static Entity.tbWeek SelectInfo(int weekNo)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbWeek>()
                          where p.No == weekNo
                          orderby p.No
                          select p).FirstOrDefault();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectInfoList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbWeek>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem()
                          {
                              Text = p.WeekName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

    }
}