using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralDataReasonController : Controller
    {
        // GET: Moral/MoralDataReason
        public ActionResult List()
        {
            var vm = new Models.MoralDataReason.List();
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbMoralDataReason>() select p);
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(p => p.Reason.Contains(vm.SearchText));
                }
                vm.MoralDataReasonList = (from p in tb select new Dto.MoralDataReason.List()
                    {
                        Id = p.Id,
                        No = p.No,
                        Reason = p.Reason,
                        tbMoralItemName=p.tbMoralItem.MoralItemName,
                        tbMoralName=p.tbMoralItem.tbMoralGroup.tbMoral.MoralName
                    }).ToList();
            }
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.MoralDataReason.List vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("List", new { SearchText = vm.SearchText }));
        }



        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.MoralDataReason.Edit();
            using (var db = new XkSystem.Models.DbContext())
            {
                if (id > 0)
                {
                    var tb = (from p in db.Table<Entity.tbMoralDataReason>()
                              where p.Id == id
                              select new Dto.MoralDataReason.Edit()
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  Reason = p.Reason,
                                  tbMoralItemId=p.tbMoralItem.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.MoralDataReasonEdit = tb;
                    }
                }
                else
                {
                    var maxNo = (from p in db.Table<Entity.tbMoralDataReason>() select p.No).DefaultIfEmpty().Max();
                    vm.MoralDataReasonEdit.No= (maxNo.HasValue?maxNo.Value:0)+ 1;
                }
                vm.MoralItemList = MoralItemController.SelectListByMoralId(vm.MoralId).Select(p => new SelectListItem() { Text=p.MoralItemName,Value=p.Id.ToString() }).ToList();
            }
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.MoralDataReason.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tbMoralItem = db.Set<Entity.tbMoralItem>().Find(vm.MoralDataReasonEdit.tbMoralItemId);
                if (vm.MoralDataReasonEdit.Id > 0)
                {
                    var tb = (from p in db.Table<Entity.tbMoralDataReason>() where p.Id == vm.MoralDataReasonEdit.Id select p).FirstOrDefault();
                    if (vm.MoralDataReasonEdit.No.HasValue && vm.MoralDataReasonEdit.No > 0)
                    {
                        tb.No = vm.MoralDataReasonEdit.No;
                    }
                    tb.Reason = vm.MoralDataReasonEdit.Reason;
                    tb.tbMoralItem = tbMoralItem;
                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("修改了德育评分原因设置项.");
                    }
                }
                else
                {
                    db.Set<Entity.tbMoralDataReason>().Add(new Entity.tbMoralDataReason()
                    {
                        No = vm.MoralDataReasonEdit.No.HasValue && vm.MoralDataReasonEdit.No > 0 ? vm.MoralDataReasonEdit.No : (from p in db.Table<Entity.tbMoralDataReason>() select p.No).DefaultIfEmpty().Max() + 1,
                        Reason = vm.MoralDataReasonEdit.Reason,
                        tbMoralItem=tbMoralItem
                    });
                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("添加了德育评分原因设置项.");
                    }
                }
            }
            return Code.MvcHelper.Post();
        }


        /// <summary>
        /// 根据德育选项Id获取对应的德育评分原因，并且根据id选中传入项（如果id>0的情况）
        /// </summary>
        /// <param name="moralItemId">德育选项Id</param>
        /// <param name="id">德育评分原因Id</param>
        /// <returns>德育评分原因列表</returns>
        [NonAction]
        internal static List<SelectListItem> SelectList(int moralItemId,int id=0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return (from p in db.Table<Entity.tbMoralDataReason>() where p.tbMoralItem.Id==moralItemId select new SelectListItem()
                {
                    Text=p.Reason,
                    Value=p.Id.ToString(),
                    Selected=p.Id==id
                }).ToList();
            }
        }
    }

}