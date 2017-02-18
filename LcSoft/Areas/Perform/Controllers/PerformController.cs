using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Perform.Controllers
{
    public class PerformController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Perform.List();
                var tb = from p in db.Table<Perform.Entity.tbPerform>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.PerformName.Contains(vm.SearchText));
                }

                vm.PerformList = (from p in tb
                                  orderby p.No descending
                                  select new Dto.Perform.List
                                  {
                                      Id = p.Id,
                                      No = p.No,
                                      IsOpen = p.IsOpen,
                                      PerformName = p.PerformName,
                                      YearName = p.tbYear.YearName,
                                      FromDate = p.FromDate,
                                      ToDate = p.ToDate
                                  }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Perform.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, PageSize = vm.Page.PageSize, PageCount = vm.Page.PageCount, PageIndex = vm.Page.PageIndex }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Perform.Entity.tbPerform>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var performGroupList = (from p in db.Table<Perform.Entity.tbPerformGroup>()
                                            .Include(d => d.tbPerform)
                                        where ids.Contains(p.tbPerform.Id)
                                        select p).ToList();

                var performCourseList = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                            .Include(d => d.tbPerformGroup)
                                         where ids.Contains(p.tbPerformGroup.tbPerform.Id)
                                         select p).ToList();

                var performItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                        .Include(d => d.tbPerformGroup)
                                       where ids.Contains(p.tbPerformGroup.tbPerform.Id)
                                       select p).ToList();

                var performDataList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                        .Include(d => d.tbPerformItem)
                                       where ids.Contains(p.tbPerformItem.tbPerformGroup.tbPerform.Id)
                                        && p.tbPerformItem.IsDeleted == false
                                        && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                       select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    foreach (var group in performGroupList.Where(d => d.tbPerform.Id == a.Id))
                    {
                        group.IsDeleted = true;

                        foreach (var course in performCourseList.Where(d => d.tbPerformGroup.Id == group.Id))
                        {
                            course.IsDeleted = true;
                        }

                        foreach (var item in performItemList.Where(d => d.tbPerformGroup.Id == group.Id))
                        {
                            item.IsDeleted = true;

                            foreach (var data in performDataList.Where(d => d.tbPerformItem.Id == item.Id))
                            {
                                data.IsDeleted = true;
                            }
                        }
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除学生评价");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Perform.Edit();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.PerformEdit.YearId == 0)
                {
                    vm.PerformEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                vm.PerformList = Areas.Perform.Controllers.PerformController.SelectList();
                vm.PerformList.Insert(0, new SelectListItem { Text = "请选择", Value = "0" });
                vm.CreateWay = "全新创建";

                if (id != 0)
                {
                    var tb = (from p in db.Table<Perform.Entity.tbPerform>()
                              where p.Id == id
                              select new Dto.Perform.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  IsOpen = p.IsOpen,
                                  PerformName = p.PerformName,
                                  YearId = p.tbYear.Id,
                                  FromDate = p.FromDate,
                                  ToDate = p.ToDate
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.PerformEdit = tb;
                    }
                }


                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Perform.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                int performId = 0;

                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.PerformEdit.Id == 0)
                    {
                        if (vm.CreateWay == "复制历史" && vm.CopyPerformId == 0)
                        {
                            error.AddError("请选择需要复制的历史评价设置！");
                        }

                        var tb = new Perform.Entity.tbPerform();
                        tb.No = vm.PerformEdit.No == null ? db.Table<Perform.Entity.tbPerform>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.PerformEdit.No;
                        tb.PerformName = vm.PerformEdit.PerformName;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.PerformEdit.YearId);
                        tb.IsOpen = vm.PerformEdit.IsOpen;
                        tb.FromDate = vm.PerformEdit.FromDate;
                        tb.ToDate = vm.PerformEdit.ToDate;
                        db.Set<Perform.Entity.tbPerform>().Add(tb);

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生评价");

                            performId = tb.Id;

                            #region 复制历史
                            if (vm.CreateWay == "复制历史" && vm.CopyPerformId != 0)
                            {
                                var copyPerform = (from p in db.Table<Perform.Entity.tbPerform>()
                                                   where p.Id == vm.CopyPerformId
                                                   select p).FirstOrDefault();

                                var copyPerformGroupList = (from p in db.Table<Perform.Entity.tbPerformGroup>()
                                                            where p.tbPerform.Id == vm.CopyPerformId
                                                            select p).ToList();
                                var dbPerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                                         where p.tbPerformGroup.tbPerform.Id == vm.CopyPerformId
                                                         select p).ToList();
                                var dbPerformCourseList = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                                           .Include(p => p.tbCourse)
                                                           where p.tbPerformGroup.tbPerform.Id == vm.CopyPerformId
                                                           select p).ToList();

                                foreach (var performGroup in copyPerformGroupList)
                                {
                                    var pg = new Perform.Entity.tbPerformGroup();
                                    pg.PerformGroupName = performGroup.PerformGroupName;
                                    pg.No = performGroup.No;
                                    pg.tbPerform = tb;
                                    db.Set<Perform.Entity.tbPerformGroup>().Add(pg);

                                    var copyPerformItemList = dbPerformItemList.Where(p => p.tbPerformGroup.Id == performGroup.Id).ToList();
                                    foreach (var copyPerformItem in copyPerformItemList)
                                    {
                                        var pi = new Perform.Entity.tbPerformItem();
                                        pi.PerformItemName = copyPerformItem.PerformItemName;
                                        pi.ScoreMax = copyPerformItem.ScoreMax;
                                        pi.Rate = copyPerformItem.Rate;
                                        pi.No = copyPerformItem.No;
                                        pi.tbPerformGroup = pg;
                                        db.Set<Perform.Entity.tbPerformItem>().Add(pi);
                                    }

                                    var copyPerformCourseList = dbPerformCourseList.Where(p => p.tbPerformGroup.Id == performGroup.Id).ToList();
                                    foreach (var copyPerformCourse in copyPerformCourseList)
                                    {
                                        var pc = new Perform.Entity.tbPerformCourse();
                                        pc.No = copyPerformCourse.No;
                                        pc.tbCourse = copyPerformCourse.tbCourse;
                                        pc.tbPerformGroup = pg;
                                        db.Set<Perform.Entity.tbPerformCourse>().Add(pc);
                                    }
                                }
                                db.SaveChanges();
                            }
                            #endregion
                        }

                    }
                    else
                    {
                        var tb = (from p in db.Table<Perform.Entity.tbPerform>()
                                  where p.Id == vm.PerformEdit.Id
                                  select p).FirstOrDefault();

                        if (tb != null)
                        {
                            performId = tb.Id;

                            tb.No = vm.PerformEdit.No == null ? db.Table<Perform.Entity.tbPerform>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.PerformEdit.No;
                            tb.PerformName = vm.PerformEdit.PerformName;
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.PerformEdit.YearId);
                            tb.IsOpen = vm.PerformEdit.IsOpen;
                            tb.FromDate = vm.PerformEdit.FromDate;
                            tb.ToDate = vm.PerformEdit.ToDate;

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改学生评价");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }
                if (Request["status"] != null)
                {
                    return Code.MvcHelper.Post(error, Url.Action("List"));
                }
                else
                {
                    return Code.MvcHelper.Post(error, Url.Action("List", "PerformGroup", new { PerformId = performId }));
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetOpen(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Perform.Entity.tbPerform>().Find(id);
                if (tb != null)
                {
                    tb.IsOpen = !tb.IsOpen;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改学生评价");
                    }
                }

                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }
        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Perform.Entity.tbPerform>()
                          where p.IsOpen == true
                          orderby p.No descending
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.PerformName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}