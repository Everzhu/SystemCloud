using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace XkSystem.Areas.Quality.Controllers
{
    public class QualityController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Quality.List();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }
                var tb = from p in db.Table<Quality.Entity.tbQuality>()
                         where p.IsDeleted==false
                         && p.tbYear.Id==vm.YearId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.QualityName.Contains(vm.SearchText));
                }

                vm.QualityList = (from p in tb
                                  orderby p.No
                                  select new Dto.Quality.List
                                  {
                                      Id = p.Id,
                                      No = p.No,
                                      QualityName = p.QualityName,
                                      FromDate = p.FromDate,
                                      IsOpen = p.IsOpen,
                                      ToDate = p.ToDate,
                                      YearName = p.tbYear.YearName,
                                      IsActive=p.IsActive,
                                  }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Quality.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { yearId=vm.YearId,searchText = vm.SearchText }));
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Quality.Edit();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.QualityEdit.YearId == 0)
                {
                    vm.QualityEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }
                vm.QualityList = Areas.Quality.Controllers.QualityController.SelectList();
                vm.QualityList.Insert(0, new SelectListItem { Text = "请选择", Value = "0" });
                vm.CreateWay = "全新创建";

                if (id != 0)
                {
                    var tb = (from p in db.Table<Quality.Entity.tbQuality>()
                              where p.Id == id
                              select new Dto.Quality.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  QualityName = p.QualityName,
                                  FromDate = p.FromDate,
                                  IsOpen = p.IsOpen,
                                  ToDate = p.ToDate,
                                  YearId = p.tbYear.Id,
                                  IsActive=p.IsActive,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.QualityEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Quality.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (vm.QualityEdit.ToDate < vm.QualityEdit.FromDate)
                {
                    error.AddError("开始时间不能大于结束时间！");
                }
                int qualityId = 0;

                if (error.Count == decimal.Zero)
                {
                    if (vm.QualityEdit.Id == 0)
                    {
                        if (vm.CreateWay == "复制历史" && vm.CopyQualityId == 0)
                        {
                            error.AddError("请选择需要复制的历史评教设置！");
                        }
                        else
                        {
                            var tb = new Quality.Entity.tbQuality();
                            tb.No = vm.QualityEdit.No == null ? db.Table<Quality.Entity.tbQuality>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualityEdit.No;
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.QualityEdit.YearId);
                            tb.QualityName = vm.QualityEdit.QualityName;
                            tb.FromDate = vm.QualityEdit.FromDate;
                            tb.ToDate = vm.QualityEdit.ToDate;
                            tb.IsOpen = vm.QualityEdit.IsOpen;
                            tb.IsActive = vm.QualityEdit.IsActive;
                            db.Set<Quality.Entity.tbQuality>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价");
                            }

                            qualityId = tb.Id;
                            if (tb.IsActive == true)
                            {
                                var list = (from p in db.Table<Quality.Entity.tbQuality>()
                                            where p.Id != tb.Id
                                            && p.IsDeleted == false
                                            select p).ToList();
                                foreach (var model in list)
                                {
                                    model.IsActive = false;
                                    model.UpdateTime = DateTime.Now;
                                }
                            }

                            #region 复制历史
                            if (vm.CreateWay == "复制历史" && vm.CopyQualityId != 0)
                            {
                                var copyQuality = (from p in db.Table<Quality.Entity.tbQuality>()
                                                   where p.Id == vm.CopyQualityId
                                                   && p.IsDeleted==false
                                                   select p).FirstOrDefault();

                                #region tbQualityItemGroup
                                //获取需要复制的评价分组
                                var copyQualityItemGroupList = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                                                                where p.tbQuality.Id == vm.CopyQualityId
                                                                 && p.IsDeleted == false
                                                                 && p.tbQuality.IsDeleted == false
                                                                select p).ToList();
                                foreach (var copyQualityItemGroup in copyQualityItemGroupList)
                                {
                                    var group = new Quality.Entity.tbQualityItemGroup();
                                    group.QualityItemGroupName = copyQualityItemGroup.QualityItemGroupName;
                                    group.No = copyQualityItemGroup.No;
                                    group.tbQuality = tb;
                                    db.Set<Quality.Entity.tbQualityItemGroup>().Add(group);

                                    //获取需要复制的评价内容
                                    var copyQualityItemList = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                                          .Include(d => d.tbQualityItemGroup)
                                                               where p.tbQualityItemGroup.Id == copyQualityItemGroup.Id
                                                                && p.IsDeleted == false
                                                                && p.tbQualityItemGroup.IsDeleted == false
                                                           && p.tbQualityItemGroup.tbQuality.IsDeleted == false
                                                               select p).ToList();
                                    foreach (var copyQualityItem in copyQualityItemList)
                                    {
                                        var item = new Quality.Entity.tbQualityItem();
                                        item.QualityItemName = copyQualityItem.QualityItemName;
                                        item.No = copyQualityItem.No;
                                        item.QualityItemType = copyQualityItem.QualityItemType;
                                        item.tbQualityItemGroup = group;
                                        db.Set<Quality.Entity.tbQualityItem>().Add(item);

                                        //获取需要复制的评价选项
                                        var copyQualityOptionList = (from p in db.Table<Quality.Entity.tbQualityOption>()
                                                            .Include(d => d.tbQualityItem)
                                                                     where p.tbQualityItem.Id == copyQualityItem.Id
                                                                     && p.IsDeleted == false
                                                                     && p.tbQualityItem.IsDeleted == false
                                                                && p.tbQualityItem.tbQualityItemGroup.IsDeleted == false
                                                           && p.tbQualityItem.tbQualityItemGroup.tbQuality.IsDeleted == false
                                                                     select p).ToList();
                                        foreach (var copyQualityOption in copyQualityOptionList)
                                        {
                                            var option = new Quality.Entity.tbQualityOption();
                                            option.OptionName = copyQualityOption.OptionName;
                                            option.No = copyQualityOption.No;
                                            option.OptionValue = copyQualityOption.OptionValue;
                                            option.tbQualityItem = item;
                                            db.Set<Quality.Entity.tbQualityOption>().Add(option);
                                        }
                                    }
                                }
                                db.SaveChanges();
                                #endregion
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        var tb = (from p in db.Table<Quality.Entity.tbQuality>()
                                  where p.Id == vm.QualityEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            qualityId = tb.Id;

                            tb.No = vm.QualityEdit.No == null ? db.Table<Quality.Entity.tbQuality>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualityEdit.No;
                            tb.QualityName = vm.QualityEdit.QualityName;
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.QualityEdit.YearId);
                            tb.FromDate = vm.QualityEdit.FromDate;
                            tb.ToDate = vm.QualityEdit.ToDate;
                            tb.IsOpen = vm.QualityEdit.IsOpen;
                            tb.IsActive = vm.QualityEdit.IsActive;
                            if (tb.IsActive == true)
                            {
                                var list = (from p in db.Table<Quality.Entity.tbQuality>()
                                            where p.Id != tb.Id
                                            && p.IsDeleted == false
                                            select p).ToList();
                                foreach (var model in list)
                                {
                                    model.IsActive = false;
                                    model.UpdateTime = DateTime.Now;
                                }
                            }
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价");
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
                    return Code.MvcHelper.Post(error, Url.Action("List", "QualityItemGroup", new { qualityId = qualityId }));
                }
                else
                {
                    return Code.MvcHelper.Post(error, Url.Action("List"));
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Quality.Entity.tbQuality>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetOpen(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Quality.Entity.tbQuality>().Find(id);
                if (tb != null)
                {
                    tb.IsOpen = !tb.IsOpen;
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价");
                    }
                }

                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetActive(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Quality.Entity.tbQuality>().Find(id);
                if (tb != null)
                {
                    tb.IsActive = !tb.IsActive;
                    if (tb.IsActive == true)
                    {
                        var list = (from p in db.Table<Quality.Entity.tbQuality>()
                                    where p.Id != id
                                    && p.IsDeleted == false
                                    select p).ToList();
                        foreach (var model in list)
                        {
                            model.IsActive = false;
                            model.UpdateTime = DateTime.Now;
                        }
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价");
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
                var tb = (from p in db.Table<Quality.Entity.tbQuality>()
                          where p.IsDeleted==false
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.QualityName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}