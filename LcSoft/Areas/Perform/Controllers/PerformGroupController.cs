using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Perform.Controllers
{
    public class PerformGroupController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformGroup.List();
                vm.PerformName = db.Table<Perform.Entity.tbPerform>().FirstOrDefault(d => d.Id == vm.PerformId).PerformName;
                var tb = from p in db.Table<Perform.Entity.tbPerformGroup>()
                         where p.tbPerform.Id == vm.PerformId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.PerformGroupName.Contains(vm.SearchText));
                }
                vm.PerformGroupList = (from p in tb
                                       orderby p.No
                                       select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.PerformGroup.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, PerformId = vm.PerformId }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Perform.Entity.tbPerformGroup>()
                          where ids.Contains(p.Id)
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

                    foreach (var course in performCourseList.Where(d => d.tbPerformGroup.Id == a.Id))
                    {
                        course.IsDeleted = true;
                    }

                    foreach (var item in performItemList.Where(d => d.tbPerformGroup.Id == a.Id))
                    {
                        item.IsDeleted = true;

                        foreach (var data in performDataList.Where(d => d.tbPerformItem.Id == item.Id))
                        {
                            data.IsDeleted = true;
                        }
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价项分组");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformGroup.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Perform.Entity.tbPerformGroup>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.PerformGroupEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult Edit(Models.PerformGroup.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var fail = Json(new { Status = decimal.Zero, Message = "操作失败！" });

                if (error.Count != decimal.Zero)
                    return fail;

                var tb = new Perform.Entity.tbPerformGroup();

                #region 【新增或修改数据】
                //新增
                if (vm.PerformGroupEdit.Id == 0)
                {
                    tb.No = vm.PerformGroupEdit.No == null ? db.Table<Perform.Entity.tbPerformGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.PerformGroupEdit.No;
                    tb.PerformGroupName = vm.PerformGroupEdit.PerformGroupName;
                    tb.tbPerform = db.Set<Perform.Entity.tbPerform>().Find(vm.PerformId);
                    tb.MaxScore = vm.PerformGroupEdit.MaxScore;
                    tb.MinScore = vm.PerformGroupEdit.MinScore;
                    db.Set<Perform.Entity.tbPerformGroup>().Add(tb);
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价项分组");
                }
                else//修改
                {
                    tb = (from p in db.Table<Perform.Entity.tbPerformGroup>()
                          where p.Id == vm.PerformGroupEdit.Id
                          select p).FirstOrDefault();

                    if (tb == null)
                    {
                        error.AddError(Resources.LocalizedText.MsgNotFound);
                        return fail;
                    }
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价项分组");

                    tb.No = vm.PerformGroupEdit.No == null ? db.Table<Perform.Entity.tbPerformGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.PerformGroupEdit.No;
                    tb.PerformGroupName = vm.PerformGroupEdit.PerformGroupName;
                    tb.MinScore = vm.PerformGroupEdit.MinScore;
                    tb.MaxScore = vm.PerformGroupEdit.MaxScore;
                }
                #endregion

                #region 【事务提交】
                using (TransactionScope scope = new TransactionScope())
                {
                    var result = new PerformCourseController().SaveCourseNode(db, Request["txtSubjectIds"], tb, vm.PerformId);

                    if (result.Status == decimal.Zero)
                    {
                        return Json(result);
                    }

                    db.SaveChanges();
                    scope.Complete();

                    return Code.MvcHelper.Post(null, Url.Action("List", new { performId = vm.PerformId }));
                }
                #endregion
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int performId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Perform.Entity.tbPerformGroup>()
                          where p.tbPerform.Id == performId
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.PerformGroupName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}