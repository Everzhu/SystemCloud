using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralGroupController : Controller
    {
        public ActionResult List()
        {
            var vm = new Models.MoralGroup.List();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.MoralName = db.Table<Moral.Entity.tbMoral>().FirstOrDefault(d => d.Id == vm.MoralId).MoralName;
                var tb = from p in db.Table<Moral.Entity.tbMoralGroup>() where p.tbMoral.Id == vm.MoralId select p;
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(p => p.MoralGroupName.Contains(vm.SearchText));
                }
                vm.MoralGroupList = (from p in tb
                                     orderby p.No
                                     select new Dto.MoralGroup.List()
                                     {
                                         Id = p.Id,
                                         No = p.No,
                                         MoralGroupName = p.MoralGroupName,
                                         MoralName = p.tbMoral.MoralName
                                     }).ToList();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.MoralGroup.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, moralId = vm.MoralId }));
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralGroup.Edit();
                if (id > 0)
                {

                    var tb = (from p in db.Table<Moral.Entity.tbMoralGroup>()
                              where p.Id == id
                              select new Dto.MoralGroup.Edit()
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  MoralGroupName = p.MoralGroupName,
                                  MoralId = p.tbMoral.Id
                              }).FirstOrDefault();

                    if (tb != null)
                    {
                        vm.MoralGroupEdit = tb;
                    }
                }
                else
                {
                    vm.MoralGroupEdit.No = db.Table<Moral.Entity.tbMoralGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1;
                }

                return View(vm);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.MoralGroup.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {

                var isExists = db.Table<Moral.Entity.tbMoralGroup>().Count(p => p.MoralGroupName.Equals(vm.MoralGroupEdit.MoralGroupName) && p.tbMoral.Id==vm.MoralId && p.Id != vm.MoralGroupEdit.Id) > 0;
                if (isExists)
                {
                    var error = new List<string>() { "系统中已存在相同名称的记录！" };
                    return Code.MvcHelper.Post(error);
                }

                if (vm.MoralGroupEdit.Id == 0)
                {
                    var tb = new Moral.Entity.tbMoralGroup()
                    {
                        No = vm.MoralGroupEdit.No.HasValue ? vm.MoralGroupEdit.No.Value : db.Table<Moral.Entity.tbMoralGroup>().Where(p => p.tbMoral.Id == vm.MoralId).Select(d => d.No).DefaultIfEmpty(0).Max() + 1,
                        MoralGroupName = vm.MoralGroupEdit.MoralGroupName,
                        tbMoral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId)
                    };
                    db.Set<Moral.Entity.tbMoralGroup>().Add(tb);
                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("添加了德育分组!");
                    }
                }
                else
                {
                    var tb = db.Set<Moral.Entity.tbMoralGroup>().Find(vm.MoralGroupEdit.Id);
                    tb.No = vm.MoralGroupEdit.No.HasValue ? vm.MoralGroupEdit.No.Value : db.Table<Moral.Entity.tbMoralGroup>().Where(p=>p.tbMoral.Id==vm.MoralId).Select(d => d.No).DefaultIfEmpty(0).Max() + 1;
                    tb.MoralGroupName = vm.MoralGroupEdit.MoralGroupName;
                    tb.tbMoral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("修改了德育分组!");
                    }
                }
            }
            return Code.MvcHelper.Post();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var list = (from p in db.Table<Moral.Entity.tbMoralGroup>() where ids.Contains(p.Id) select p);
                foreach (var item in list)
                {
                    item.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    Sys.Controllers.SysUserLogController.Insert("删除了德育分组！");
                }
            }
            return Code.MvcHelper.Post();
        }



        [NonAction]
        internal static List<SelectListItem> SelectList(int moralId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return (from p in db.Table<Moral.Entity.tbMoralGroup>() where p.tbMoral.Id==moralId
                        select new SelectListItem()
                        {
                            Value = p.Id.ToString(),
                            Text = p.MoralGroupName
                        }).ToList();
            }
        }

        [NonAction]
        internal static List<Dto.MoralGroup.Info> GetMoralGroupInfoList(int moralId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Moral.Entity.tbMoralGroup>()
                          select p);
                if (moralId > 0)
                {
                    tb = tb.Where(p => p.tbMoral.Id == moralId);
                }

                var list=  (from p in tb
                          join i in db.Table<Moral.Entity.tbMoralItem>() on p.Id equals i.tbMoralGroup.Id into item
                          from r in item.DefaultIfEmpty()
                          group r by new { p.Id, p.MoralGroupName } into result
                          select new Dto.MoralGroup.Info()
                          {
                              Id = result.Key.Id,
                              MoralGroupName = result.Key.MoralGroupName,
                              MoralItemCount = result.Count(r=>r.tbMoralGroup!=null)
                          }).ToList();

                return list.Where(p => p.MoralItemCount > 0).ToList();
            }
        }


        [NonAction]
        internal static List<Dto.MoralGroup.Info> GetMoralGroupInfoList(int moralId, Code.EnumHelper.MoralItemKind kind)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Moral.Entity.tbMoralGroup>() where p.tbMoral.Id == moralId
                          select p);               

                return (from p in tb
                        join i in db.Table<Moral.Entity.tbMoralItem>() on p.Id equals i.tbMoralGroup.Id into item
                        from r in item.DefaultIfEmpty()
                        where r.MoralItemKind==kind
                        group r by new { p.Id, p.MoralGroupName } into result
                        select new Dto.MoralGroup.Info()
                        {
                            Id = result.Key.Id,
                            MoralGroupName = result.Key.MoralGroupName,
                            MoralItemCount = result.Count(r => r.tbMoralGroup != null)
                        }).ToList();
            }
        }


        [NonAction]
        internal static void InsertDefault(XkSystem.Models.DbContext db, int moralId)
        {

            db.Set<Moral.Entity.tbMoralGroup>().Add(new Moral.Entity.tbMoralGroup()
            {
                No = 1,
                tbMoral = db.Set<Moral.Entity.tbMoral>().Find(moralId),
                MoralGroupName = "默认分组"
            });

            if (db.SaveChanges() > 0)
            {
                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了德育分组！");
            }
        }
    }
}