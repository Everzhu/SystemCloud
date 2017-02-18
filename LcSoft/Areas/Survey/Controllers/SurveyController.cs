using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace XkSystem.Areas.Survey.Controllers
{
    public class SurveyController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Survey.List();

                var tb = from p in db.Table<Entity.tbSurvey>()
                         select p;
                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.SurveyName.Contains(vm.SearchText));
                }

                vm.SurveyList = (from p in tb
                                 orderby p.No descending
                                 select new Dto.Survey.List
                                 {
                                     Id = p.Id,
                                     No = p.No,
                                     SurveyName = p.SurveyName,
                                     FromDate = p.FromDate,
                                     ToDate = p.ToDate,
                                     IsOpen = p.IsOpen,
                                     YearSectionName = p.tbYear.YearName,
                                 }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Survey.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                pageSize = vm.Page.PageSize,
                pageCount = vm.Page.PageCount,
                pageIndex = vm.Page.PageIndex
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSurvey>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var surveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                        .Include(d => d.tbSurvey)
                                       where ids.Contains(p.tbSurvey.Id)
                                       select p).ToList();

                var surveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                        .Include(d => d.tbSurveyGroup)
                                      where ids.Contains(p.tbSurveyGroup.tbSurvey.Id)
                                        && p.tbSurveyGroup.IsDeleted == false
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

                    foreach (var group in surveyGroupList.Where(d => d.tbSurvey.Id == a.Id))
                    {
                        group.IsDeleted = true;

                        foreach (var item in surveyItemList.Where(d => d.tbSurveyGroup.Id == group.Id))
                        {
                            item.IsDeleted = true;

                            foreach (var data in surveyDataList.Where(d => d.tbSurveyItem.Id == item.Id))
                            {
                                item.IsDeleted = true;
                            }
                        }
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除教师评价");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Survey.Edit();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.SurveyEdit.YearId == 0)
                {
                    vm.SurveyEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                vm.SurveyList = Areas.Survey.Controllers.SurveyController.SelectList();
                vm.SurveyList.Insert(0, new SelectListItem { Text = "请选择", Value = "0" });
                vm.CreateWay = "全新创建";

                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbSurvey>()
                              where p.Id == id
                              select new Dto.Survey.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  SurveyName = p.SurveyName,
                                  YearId = p.tbYear.Id,
                                  FromDate = p.FromDate,
                                  ToDate = p.ToDate,
                                  IsOpen = p.IsOpen,
                                  Remark = p.Remark
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.SurveyEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Models.Survey.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                int surveyId = 0;

                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.SurveyEdit.Id == 0)
                    {
                        if (vm.CreateWay == "复制历史" && vm.CopySurveyId == 0)
                        {
                            error.AddError("请选择需要复制的历史评教设置！");
                        }

                        var tb = new Entity.tbSurvey();
                        tb.No = vm.SurveyEdit.No == null ? db.Table<Entity.tbSurvey>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SurveyEdit.No;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.SurveyEdit.YearId);
                        tb.SurveyName = vm.SurveyEdit.SurveyName;
                        tb.FromDate = vm.SurveyEdit.FromDate;
                        tb.ToDate = vm.SurveyEdit.ToDate;
                        tb.IsOpen = vm.SurveyEdit.IsOpen;
                        tb.Remark = vm.SurveyEdit.Remark;
                        db.Set<Entity.tbSurvey>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加教师评价");
                        }

                        surveyId = tb.Id;

                        #region 复制历史
                        if (vm.CreateWay == "复制历史" && vm.CopySurveyId != 0)
                        {
                            var copySurvey = (from p in db.Table<Entity.tbSurvey>()
                                              where p.Id == vm.CopySurveyId
                                              select p).FirstOrDefault();

                            #region tbSurveyClass
                            var copySurveyClassList = (from p in db.Table<Entity.tbSurveyClass>()
                                                       .Include(p => p.tbClass)
                                                       where p.tbSurvey.Id == vm.CopySurveyId
                                                       select p).ToList();
                            foreach (var surveyClass in copySurveyClassList)
                            {
                                var sc = new Entity.tbSurveyClass();
                                sc.tbSurvey = tb;
                                sc.tbClass = surveyClass.tbClass;
                                db.Set<Entity.tbSurveyClass>().Add(sc);
                            }
                            db.SaveChanges();
                            #endregion

                            #region tbSurveyGroup
                            var copySurveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                                       where p.tbSurvey.Id == vm.CopySurveyId
                                                       select p).ToList();
                            var dbSurveyCourseList = (from p in db.Table<Entity.tbSurveyCourse>()
                                                      .Include(p => p.tbCourse)
                                                      where p.tbSurveyGroup.tbSurvey.Id == vm.CopySurveyId
                                                      select p).ToList();
                            var dbSurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                                    where p.tbSurveyGroup.tbSurvey.Id == vm.CopySurveyId
                                                    select p).ToList();
                            var dbSurveyOptionList = (from p in db.Table<Entity.tbSurveyOption>()
                                                      where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.CopySurveyId
                                                      select p).ToList();

                            foreach (var surveyGroup in copySurveyGroupList)
                            {
                                int copySurveyGroupId = surveyGroup.Id;

                                var sg = new Entity.tbSurveyGroup();
                                sg.No = surveyGroup.No;
                                sg.SurveyGroupName = surveyGroup.SurveyGroupName;
                                sg.IsOrg = surveyGroup.IsOrg;
                                sg.tbSurvey = tb;
                                db.Set<Entity.tbSurveyGroup>().Add(sg);

                                #region tbSurveyCourse
                                var copySurveyCourseList = dbSurveyCourseList.Where(p => p.tbSurveyGroup.Id == copySurveyGroupId).ToList();
                                foreach (var surveyCourse in copySurveyCourseList)
                                {
                                    var sc = new Entity.tbSurveyCourse();
                                    sc.tbCourse = surveyCourse.tbCourse;
                                    sc.tbSurveyGroup = sg;
                                    db.Set<Entity.tbSurveyCourse>().Add(sc);
                                }
                                #endregion

                                #region tbSurveyItem
                                var copySurveyItemList = dbSurveyItemList.Where(p => p.tbSurveyGroup.Id == copySurveyGroupId).ToList();
                                foreach (var surveyItem in copySurveyItemList)
                                {
                                    int copySurveyItemId = surveyItem.Id;
                                    var si = new Entity.tbSurveyItem();
                                    si.No = surveyItem.No;
                                    si.SurveyItemName = surveyItem.SurveyItemName;
                                    si.TextMaxLength = surveyItem.TextMaxLength;
                                    si.SurveyItemType = surveyItem.SurveyItemType;
                                    si.IsVertical = surveyItem.IsVertical;
                                    si.tbSurveyGroup = sg;
                                    db.Set<Entity.tbSurveyItem>().Add(si);

                                    #region tbSurveyOption
                                    var copySurveyOptionList = dbSurveyOptionList.Where(p => p.tbSurveyItem.Id == copySurveyItemId).ToList();
                                    foreach (var surveyOption in copySurveyOptionList)
                                    {
                                        var so = new Entity.tbSurveyOption();
                                        so.No = surveyOption.No;
                                        so.OptionName = surveyOption.OptionName;
                                        so.OptionValue = surveyOption.OptionValue;
                                        so.tbSurveyItem = si;
                                        db.Set<Entity.tbSurveyOption>().Add(so);
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion

                            db.SaveChanges();
                        }
                        #endregion
                    }
                    else
                    {
                        var tb = (from p in db.Table<Entity.tbSurvey>()
                                  where p.Id == vm.SurveyEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            surveyId = tb.Id;

                            tb.No = vm.SurveyEdit.No == null ? db.Table<Entity.tbSurvey>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SurveyEdit.No;
                            tb.SurveyName = vm.SurveyEdit.SurveyName;
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.SurveyEdit.YearId);
                            tb.FromDate = vm.SurveyEdit.FromDate;
                            tb.ToDate = vm.SurveyEdit.ToDate;
                            tb.IsOpen = vm.SurveyEdit.IsOpen;
                            tb.Remark = vm.SurveyEdit.Remark;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改教师评价");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                if (Request["Step"] != null)
                {
                    return Code.MvcHelper.Post(error, Url.Action("List", "SurveyClass", new { surveyId = surveyId }));
                }
                else
                {
                    return Code.MvcHelper.Post(error, Url.Action("List"));
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetOpen(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Entity.tbSurvey>().Find(id);
                if (tb != null)
                {
                    tb.IsOpen = !tb.IsOpen;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改教师评价");
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
                var tb = (from p in db.Table<Entity.tbSurvey>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.SurveyName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}