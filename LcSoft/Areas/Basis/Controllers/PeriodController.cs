using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class PeriodController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Period.List();
                var tb = from p in db.Table<Basis.Entity.tbPeriod>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.PeriodName.Contains(vm.SearchText));
                }

                vm.PeriodList = (from p in tb
                                 orderby p.No
                                 select new Dto.Period.List
                                 {
                                     Id = p.Id,
                                     No = p.No,
                                     PeriodName = p.PeriodName
                                 }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Period.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbPeriod>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了节次");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Period.Edit();
                vm.PeriodTypeList = PeriodTypeController.SelectList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbPeriod>()
                              where p.Id == id
                              select new Dto.Period.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  PeriodTypeId = p.tbPeriodType.Id,
                                  PeriodName = p.PeriodName
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.PeriodEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Period.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Basis.Entity.tbPeriod>().Where(d => d.PeriodName == vm.PeriodEdit.PeriodName && d.Id != vm.PeriodEdit.Id).Any())
                    {
                        error.AddError("该节次已存在!");
                    }
                    else
                    {
                        if (vm.PeriodEdit.Id == 0)
                        {
                            var tb = new Basis.Entity.tbPeriod();
                            tb.No = vm.PeriodEdit.No == null ? db.Table<Basis.Entity.tbPeriod>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.PeriodEdit.No;
                            tb.PeriodName = vm.PeriodEdit.PeriodName;
                            tb.tbPeriodType = db.Set<Entity.tbPeriodType>().Find(vm.PeriodEdit.PeriodTypeId);
                            db.Set<Basis.Entity.tbPeriod>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了节次");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Basis.Entity.tbPeriod>()
                                      where p.Id == vm.PeriodEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.PeriodEdit.No == null ? db.Table<Basis.Entity.tbPeriod>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.PeriodEdit.No;
                                tb.PeriodName = vm.PeriodEdit.PeriodName;
                                tb.tbPeriodType = db.Set<Entity.tbPeriodType>().Find(vm.PeriodEdit.PeriodTypeId);
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了节次");
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

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbPeriod>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.PeriodName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        /// <summary>
        /// 首页显示课表时用到颜色
        /// </summary>
        [NonAction]
        public static List<XkSystem.Areas.Basis.Dto.Period.List> SelectListForColor()
        {
            var list = new List<XkSystem.Areas.Basis.Dto.Period.List>();
            using (var db = new XkSystem.Models.DbContext())
            {
                list = (from p in db.Table<Basis.Entity.tbPeriod>()
                        orderby p.No
                        select new Dto.Period.List()
                        {
                            Id = p.Id,
                            No = p.No,
                            Color = p.tbPeriodType.Color,
                            PeriodName = p.PeriodName
                        }).ToList();
            }
            return list;
        }

        public static List<System.Web.Mvc.SelectListItem> SelectScheduleList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbPeriod>()
                          where p.PeriodName != "午"
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.PeriodName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}