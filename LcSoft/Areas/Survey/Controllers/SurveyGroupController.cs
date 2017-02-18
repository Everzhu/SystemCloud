using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Survey.Controllers
{
    public class SurveyGroupController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyGroup.List();
                vm.SurveyName = db.Table<Entity.tbSurvey>().FirstOrDefault(d => d.Id == vm.SurveyId).SurveyName;

                var tb = from p in db.Table<Entity.tbSurveyGroup>()
                         where p.tbSurvey.Id == vm.SurveyId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.SurveyGroupName.Contains(vm.SearchText));
                }

                vm.SurveyGroupList = (from p in tb
                                      orderby p.No
                                      select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SurveyGroup.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, SurveyId = vm.SurveyId }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSurveyGroup>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var surveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                        .Include(d => d.tbSurveyGroup)
                                      where ids.Contains(p.tbSurveyGroup.Id)
                                      select p).ToList();

                var surveyDataList = (from p in db.Table<Entity.tbSurveyData>()
                        .Include(d => d.tbSurveyItem)
                                      where ids.Contains(p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id)
                                        && p.tbSurveyItem.IsDeleted == false
                                        && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                      select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    foreach (var item in surveyItemList.Where(d => d.tbSurveyGroup.Id == a.Id))
                    {
                        item.IsDeleted = true;

                        foreach (var data in surveyDataList.Where(d => d.tbSurveyItem.Id == item.Id))
                        {
                            item.IsDeleted = true;
                        }
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价分组");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyGroup.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbSurveyGroup>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.SurveyGroupEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult Edit(Models.SurveyGroup.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var tb = new Entity.tbSurveyGroup();

                    if (vm.SurveyGroupEdit.Id == 0)
                    {
                        tb.No = vm.SurveyGroupEdit.No == null ? db.Table<Entity.tbSurveyGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SurveyGroupEdit.No;
                        tb.SurveyGroupName = vm.SurveyGroupEdit.SurveyGroupName;
                        tb.tbSurvey = db.Set<Entity.tbSurvey>().Find(vm.SurveyId);
                        tb.IsOrg = vm.SurveyGroupEdit.IsOrg;
                        db.Set<Entity.tbSurveyGroup>().Add(tb);
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价分组");
                    }
                    else
                    {
                        tb = (from p in db.Table<Entity.tbSurveyGroup>()
                              where p.Id == vm.SurveyGroupEdit.Id
                              select p).FirstOrDefault();
                        if (tb != null)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价分组");
                            tb.No = vm.SurveyGroupEdit.No == null ? db.Table<Entity.tbSurveyGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SurveyGroupEdit.No;
                            tb.SurveyGroupName = vm.SurveyGroupEdit.SurveyGroupName;
                            tb.IsOrg = vm.SurveyGroupEdit.IsOrg;
                        }
                    }
                    //新增：班主任评价，不保存课程
                    if (vm.SurveyGroupEdit.IsOrg == false)
                    {
                        if (vm.SurveyGroupEdit.Id > decimal.Zero)
                        {
                            var tbSurveyCourse = (from p in db.Table<Entity.tbSurveyCourse>()
                                                  where p.tbSurveyGroup.Id == vm.SurveyGroupEdit.Id
                                                  select p).ToList();

                            foreach (var a in tbSurveyCourse)
                            {
                                //删除数据
                                a.IsDeleted = true;
                                a.UpdateTime = DateTime.Now;
                            }
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        var errorSubject = new SurveyCourseController().SaveSubjectNode(db, Request["txtSubjectIds"], tb, vm.SurveyId);
                        if (errorSubject.Status == decimal.Zero)
                        {
                            return Json(errorSubject);//出错抛出
                        }
                        else
                        {
                            db.SaveChanges();
                        }
                    }
                }
                return Code.MvcHelper.Post(error, null);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int surveyId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSurveyGroup>()
                          where p.tbSurvey.Id == surveyId
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.SurveyGroupName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<Dto.SurveyGroup.Info> SelectInfoList(int surveyId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSurveyGroup>()
                          where p.tbSurvey.Id == surveyId
                          select new Dto.SurveyGroup.Info
                          {
                              Id = p.Id,
                              No = p.No,
                              SurveyGroupName = p.SurveyGroupName,
                              IsOrg = p.IsOrg
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static Dto.SurveyGroup.Info SelectInfo(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSurveyGroup>()
                          where p.Id == id
                          select new Dto.SurveyGroup.Info
                          {
                              Id = p.Id,
                              No = p.No,
                              SurveyGroupName = p.SurveyGroupName,
                              IsOrg = p.IsOrg
                          }).FirstOrDefault();
                return tb;
            }
        }
    }
}