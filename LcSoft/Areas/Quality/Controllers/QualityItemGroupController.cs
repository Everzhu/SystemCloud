using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace XkSystem.Areas.Quality.Controllers
{
    public class QualityItemGroupController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityItemGroup.List();

                var tb = from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                         .Include(d=>d.tbQuality)
                         where p.tbQuality.Id == vm.QualityId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.QualityItemGroupName.Contains(vm.SearchText));
                }

                vm.QualityItemGroupList = (from p in tb
                                           orderby p.No
                                           select new Dto.QualityItemGroup.List
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               QualityItemGroupName = p.QualityItemGroupName,
                                               QualityName=p.tbQuality.QualityName,
                                           }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.QualityItemGroup.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, QualityId = vm.QualityId }));
        }

        public ActionResult Edit(int id = 0, int qualityId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityItemGroup.Edit();

                if (id != 0 && qualityId != 0)
                {
                    var tb = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                              where p.Id == id
                              && p.tbQuality.Id == qualityId
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.QualityItemGroupEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.QualityItemGroup.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                int qualityItemGroupId = 0;

                if (error.Count == decimal.Zero)
                {
                    var tb = new Quality.Entity.tbQualityItemGroup();

                    if (vm.QualityItemGroupEdit.Id == 0)
                    {
                        tb.No = vm.QualityItemGroupEdit.No == null ? db.Table<Quality.Entity.tbQualityItemGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualityItemGroupEdit.No;
                        tb.QualityItemGroupName = vm.QualityItemGroupEdit.QualityItemGroupName;
                        tb.tbQuality = db.Set<Quality.Entity.tbQuality>().Find(vm.QualityId);
                        db.Set<Quality.Entity.tbQualityItemGroup>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价分组");
                        }
                       
                    }
                    else
                    {
                        tb = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                                  where p.Id == vm.QualityItemGroupEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            qualityItemGroupId = tb.Id;

                            tb.No = vm.QualityItemGroupEdit.No == null ? db.Table<Quality.Entity.tbQualityItemGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualityItemGroupEdit.No;
                            tb.QualityItemGroupName = vm.QualityItemGroupEdit.QualityItemGroupName;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价分组");
                            }
                           
                        }
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价分组");
                }

                return Code.MvcHelper.Post();
            }
        }
    }
}