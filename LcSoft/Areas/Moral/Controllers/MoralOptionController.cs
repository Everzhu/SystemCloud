using XkSystem.Areas.Sys.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralOptionController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralOption.List();
                var tb = (from p in db.Table<Moral.Entity.tbMoralOption>() where p.tbMoralItem.Id == vm.MoralItemId select p);
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(p => p.MoralOptionName.Contains(vm.SearchText));
                }

                vm.MoralOptionList = (from p in tb
                                      orderby p.No
                                      select new Dto.MoralOption.List()
                                      {
                                          Id=p.Id,
                                          No=p.No,
                                          MoralItemId=p.tbMoralItem.Id,
                                          MoralItemName=p.tbMoralItem.MoralItemName,
                                          MoralOptionName = p.MoralOptionName,
                                          MoralOptionValue=p.MoralOptionValue
                                      }).ToPageList(vm.Page);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.MoralOption.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                pageIndex=vm.Page.PageIndex,
                pageSize=vm.Page.PageSize
            }));
        }

        public ActionResult Edit(int id=0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralOption.Edit();
                if (id > 0)
                {
                    var tb = (from p in db.Table<Moral.Entity.tbMoralOption>()
                              where p.Id == id
                              select new Dto.MoralOption.Edit()
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  MoralOptionName = p.MoralOptionName,
                                  MoralOptionValue = p.MoralOptionValue,
                                  tbMoralItemId = p.tbMoralItem.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.MoralOptionEdit = tb;
                    }
                }
                else
                {
                    vm.MoralOptionEdit.No = db.Table<Moral.Entity.tbMoralOption>().Where(p => p.tbMoralItem.Id == vm.MoralItemId).Select(p => p.No).DefaultIfEmpty(0).Max() + 1;
                }
                return View(vm);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.MoralOption.Edit vm)
        {
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                var isExists = db.Table<Moral.Entity.tbMoralOption>().Count(p => p.MoralOptionName.Equals(vm.MoralOptionEdit.MoralOptionName) && p.Id != vm.MoralOptionEdit.Id) > 0;
                if (isExists)
                {
                    error.AddError("系统中已存在相同名称的记录！");
                    return Code.MvcHelper.Post(error);
                }

                var tbMoralItem = db.Set<Moral.Entity.tbMoralItem>().Find(vm.MoralItemId);
 
                if (vm.MoralOptionEdit.Id == 0)
                {
                    var tb = new Moral.Entity.tbMoralOption()
                    {
                        No=vm.MoralOptionEdit.No.HasValue?vm.MoralOptionEdit.No.Value: db.Table<Moral.Entity.tbMoralOption>().Where(p=>p.tbMoralItem.Id == vm.MoralItemId).Select(p=>p.No).DefaultIfEmpty(0).Max() + 1,
                        MoralOptionName=vm.MoralOptionEdit.MoralOptionName,
                        MoralOptionValue=vm.MoralOptionEdit.MoralOptionValue,
                        tbMoralItem=tbMoralItem
                    };
                    db.Set<Moral.Entity.tbMoralOption>().Add(tb);
                    if (db.SaveChanges() > 0)
                    {
                        SysUserLogController.Insert("添加了德育项目选项！");
                    }
                }
                else
                {
                    var tb = (from p in db.Table<Moral.Entity.tbMoralOption>() where p.Id == vm.MoralOptionEdit.Id select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.No = vm.MoralOptionEdit.No.HasValue ? vm.MoralOptionEdit.No.Value : db.Table<Moral.Entity.tbMoralOption>().Where(p => p.tbMoralItem.Id == vm.MoralItemId).Select(p => p.No).DefaultIfEmpty(0).Max() + 1;
                        tb.MoralOptionName = vm.MoralOptionEdit.MoralOptionName;
                        tb.MoralOptionValue = vm.MoralOptionEdit.MoralOptionValue;
                        tb.tbMoralItem = tbMoralItem;

                        if (db.SaveChanges() > 0)
                        {
                            SysUserLogController.Insert("修改了德育项目选项！");
                        }
                    }
                    else
                    {
                        error.AddError(Resources.LocalizedText.MsgNotFound);
                    }
                    
                }

                return Code.MvcHelper.Post();
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Moral.Entity.tbMoralOption>().Find(id);
                if (tb != null) {
                    tb.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    Sys.Controllers.SysUserLogController.Insert("删除了德育选项！");
                }
                return Code.MvcHelper.Post();
            }
        }


        [NonAction]
        internal static List<Dto.MoralOption.List> SelectList(int moralItemId=0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Moral.Entity.tbMoralOption>() select p);
                if (moralItemId > 0)
                {
                    tb = tb.Where(p => p.tbMoralItem.Id == moralItemId);
                }
                return (from p in tb select new Dto.MoralOption.List()
                {
                    Id=p.Id,
                    No=p.No,
                    MoralItemId=p.tbMoralItem.Id,
                    MoralItemName = p.tbMoralItem.MoralItemName,
                    MoralOptionName =p.MoralOptionName,
                    MoralOptionValue=p.MoralOptionValue
                }).ToList();
            }
        }

    }
}