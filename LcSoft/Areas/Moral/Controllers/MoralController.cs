using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Moral.List();

                var tb = (from p in db.Table<Moral.Entity.tbMoral>() select p);
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(p => p.MoralName.Contains(vm.SearchText));
                }

                vm.MoralList = (from p in tb
                                orderby p.No descending
                                select new Dto.Moral.List()
                                {
                                    Id = p.Id,
                                    FromDate = p.FromDate,
                                    ToDate = p.ToDate,
                                    MoralName = p.MoralName,
                                    IsOpen = p.IsOpen,
                                    MoralType = p.MoralType,
                                    YearName = p.tbYear.YearName
                                }).ToPageList(vm.Page);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Moral.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                SearchText = vm.SearchText,
                PageIndex = vm.Page.PageIndex,
                PageSize = vm.Page.PageSize
            }));
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Moral.Edit();
                vm.MoralEdit.IsOpen = true;
                if (id > 0)
                {
                    var tb = (from p in db.Table<Moral.Entity.tbMoral>()
                              where p.Id == id
                              select new Dto.Moral.Edit()
                              {
                                  Id = p.Id,
                                  FromDate = p.FromDate,
                                  ToDate = p.ToDate,
                                  MoralName = p.MoralName,
                                  IsOpen = p.IsOpen,
                                  MoralType = p.MoralType,
                                  tbYearId = p.tbYear.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.MoralEdit = tb;
                    }
                }
                if (vm.MoralEdit == null || vm.MoralEdit.Id == 0)
                {
                    vm.MoralEdit.FromDate = DateTime.Now;
                    vm.MoralEdit.ToDate = Code.Common.DateMonthLast;
                }
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                vm.MoralList = SelectList();
                vm.MoralList.Insert(0, new SelectListItem { Text = "请选择", Value = "0" });
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Moral.Edit vm)
        {
            var moralId = 0;
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                var isExists = db.Table<Moral.Entity.tbMoral>().Count(p => p.MoralName.Equals(vm.MoralEdit.MoralName) && p.Id != vm.MoralEdit.Id) > 0;
                if (isExists)
                {
                    error.AddError("系统中已存在相同名字的德育设置记录！");
                }
                else
                {
                    if (vm.MoralEdit.ToDate <= vm.MoralEdit.FromDate)
                    {
                        error.AddError("结束时间必须大于开始时间！");
                    }
                    else
                    {
                        if (vm.MoralEdit.Id == 0)
                        {
                            Entity.tbMoral copyMoral = null;
                            if (vm.CreateWay == "复制历史" && vm.CopyMoralId == 0)
                            {
                                error.Add("请选择复制对象！");
                                return Code.MvcHelper.Post(error);
                            }
                            else
                            {
                                copyMoral = db.Set<Entity.tbMoral>().Find(vm.CopyMoralId);
                            }
                            var tb = new Moral.Entity.tbMoral()
                            {
                                No = vm.MoralEdit.No == null ? db.Table<Moral.Entity.tbMoral>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : vm.MoralEdit.No.Value,
                                MoralName = vm.MoralEdit.MoralName,
                                tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.MoralEdit.tbYearId),
                                FromDate = vm.MoralEdit.FromDate,
                                IsOpen = vm.MoralEdit.IsOpen,
                                ToDate = vm.MoralEdit.ToDate,
                                MoralType = copyMoral?.MoralType ?? vm.MoralEdit.MoralType
                            };

                            db.Set<Moral.Entity.tbMoral>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                Sys.Controllers.SysUserLogController.Insert("添加了德育设置！");
                                if (copyMoral == null)
                                {
                                    MoralGroupController.InsertDefault(db, tb.Id);
                                }
                            }
                            moralId = tb.Id;

                            #region 复制
                            if (vm.CreateWay == "复制历史")
                            {
                                //德育班级
                                var moralClass = (from p in db.Table<Entity.tbMoralClass>().Include(p => p.tbClass) where p.tbMoral.Id == vm.CopyMoralId select p).ToList();
                                db.Set<Entity.tbMoralClass>().AddRange(moralClass.Select(p => new Entity.tbMoralClass()
                                {
                                    No = p.No,
                                    tbClass = p.tbClass,
                                    tbMoral = tb
                                }));


                                //德育分组
                                var moralGroup = (from p in db.Table<Entity.tbMoralGroup>() where p.tbMoral.Id == vm.CopyMoralId select p).ToList();

                                //德育项目
                                var moralItem = (from p in db.Table<Entity.tbMoralItem>().Include(p => p.tbMoralGroup) where p.tbMoralGroup.tbMoral.Id == vm.CopyMoralId select p).ToList();

                                List<Entity.tbMoralOption> moralOption = new List<Entity.tbMoralOption>();
                                //德育选项 
                                if (copyMoral.MoralType == Code.EnumHelper.MoralType.Once)
                                {
                                    moralOption = (from p in db.Table<Entity.tbMoralOption>() where p.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.CopyMoralId select p).ToList();
                                }

                                moralGroup.ForEach(g =>
                                {
                                    var newGroup = new Entity.tbMoralGroup()
                                    {
                                        No = g.No,
                                        MoralGroupName = g.MoralGroupName,
                                        tbMoral = tb
                                    };
                                    db.Set<Entity.tbMoralGroup>().Add(newGroup);

                                    moralItem.Where(p => p.tbMoralGroup.Id == g.Id).ToList().ForEach(i =>
                                    {
                                        var newItem = new Entity.tbMoralItem()
                                        {
                                            No = i.No,
                                            MinScore = i.MinScore,
                                            MaxScore = i.MaxScore,
                                            InitScore = i.InitScore,
                                            DefaultValue = i.DefaultValue,
                                            MoralExpress = i.MoralExpress,
                                            MoralItemKind = i.MoralItemKind,
                                            MoralItemType = i.MoralItemType,
                                            MoralItemName = i.MoralItemName,
                                            tbMoralGroup = newGroup
                                        };
                                        db.Set<Entity.tbMoralItem>().Add(newItem);

                                        //德育选项 
                                        if (moralOption != null && moralOption.Any())
                                        {
                                            moralOption.Where(o=>o.tbMoralItem.Id==i.Id).ToList().ForEach(o =>
                                            {
                                                var newMoralOption = new Entity.tbMoralOption()
                                                {
                                                    No =o.No,
                                                    MoralOptionName=o.MoralOptionName,
                                                    MoralOptionValue=o.MoralOptionValue,
                                                    tbMoralItem=newItem
                                                };
                                                db.Set<Entity.tbMoralOption>().Add(newMoralOption);
                                            });
                                        }
                                    });
                                });

                                //德育评价人员不复制，因为日期不一样
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("复制了德育设置！");
                                }
                            }
                            #endregion

                        }
                        else
                        {
                            var tb = (from p in db.Table<Moral.Entity.tbMoral>()
                                      where p.Id == vm.MoralEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                moralId = tb.Id;
                                tb.No = vm.MoralEdit.No == null ? db.Table<Moral.Entity.tbMoral>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MoralEdit.No;
                                tb.MoralName = vm.MoralEdit.MoralName;
                                tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.MoralEdit.tbYearId);
                                tb.FromDate = vm.MoralEdit.FromDate;
                                tb.ToDate = vm.MoralEdit.ToDate;
                                tb.IsOpen = vm.MoralEdit.IsOpen;
                                tb.MoralType = vm.MoralEdit.MoralType;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了德育设置");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(Request["Step"].ConvertToString()))
                {
                    return Code.MvcHelper.Post(error, Url.Action("List"));
                }
                else
                {
                    return Code.MvcHelper.Post(error, Url.Action("List", "MoralClass", new { MoralId = moralId }));
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            if (ids != null && ids.Any())
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var tb = (from p in db.Table<Entity.tbMoral>() where ids.Contains(p.Id) select p);
                    foreach (var item in tb)
                    {
                        item.IsDeleted = true;
                    }

                    var tbMoralData = (from p in db.Table<Entity.tbMoralData>() where ids.Contains(p.tbMoralItem.tbMoralGroup.tbMoral.Id) select p);
                    foreach (var item in tb)
                    {
                        item.IsDeleted = true;
                    }

                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("删除了德育设置！");
                    }
                }
            }
            return Code.MvcHelper.Post();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDisable(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Moral.Entity.tbMoral>().Find(id);
                if (tb != null)
                {
                    tb.IsOpen = !tb.IsOpen;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改德育");
                    }
                }

                return Code.MvcHelper.Post();
            }
        }

        [NonAction]
        internal static List<SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                //return (from p in db.Table<Moral.Entity.tbMoral>()
                //        where DateTime.Now <= p.ToDate && DateTime.Now >= p.FromDate && p.IsOpen
                //        orderby p.No
                //        select new SelectListItem()
                //        {
                //            Text = p.MoralName,
                //            Value = p.Id.ToString()
                //        }).ToList();

                return (from p in db.Table<Moral.Entity.tbMoral>()
                        where p.IsOpen
                        orderby p.No
                        select new SelectListItem()
                        {
                            Text = p.MoralName,
                            Value = p.Id.ToString()
                        }).ToList();

            }
        }

        [NonAction]
        internal static List<SelectListItem> SelectList(Code.EnumHelper.MoralType moralType)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return (from p in db.Table<Moral.Entity.tbMoral>()
                        where p.MoralType == moralType && DateTime.Now <= p.ToDate && DateTime.Now >= p.FromDate && p.tbYear.IsDefault && p.IsOpen
                        orderby p.No
                        select new SelectListItem()
                        {
                            Text = p.MoralName,
                            Value = p.Id.ToString()
                        }).ToList();
            }
        }

        [NonAction]
        internal static List<Dto.Moral.List> SelectInfoList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return (from p in db.Table<Moral.Entity.tbMoral>()
                        where DateTime.Now <= p.ToDate && DateTime.Now >= p.FromDate && p.tbYear.IsDefault && p.IsOpen
                        orderby p.No
                        select new Dto.Moral.List()
                        {
                            Id = p.Id,
                            MoralName = p.MoralName,
                            FromDate = p.FromDate,
                            ToDate = p.ToDate
                        }).ToList();
            }
        }
    }
}