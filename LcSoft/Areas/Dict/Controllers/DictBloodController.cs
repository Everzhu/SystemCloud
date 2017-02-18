using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class DictBloodController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Blood.List();

                var tb = db.Table<Dict.Entity.tbDictBlood>();
                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.BloodName.Contains(vm.SearchText));
                }

                vm.tbDictBloodList = tb.ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Blood.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictBlood>()
                          where ids.Contains(p.Id)
                          select p).ToList();
                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了血型");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Blood.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Dict.Entity.tbDictBlood>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.tbDictBlood = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Blood.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Dict.Entity.tbDictBlood>().Where(d => d.BloodName == vm.tbDictBlood.BloodName && d.Id != vm.tbDictBlood.Id).Any())
                    {
                        error.AddError("该血型已存在!");
                    }
                    else
                    {
                        if (vm.tbDictBlood.Id == 0)
                        {
                            var tb = new Dict.Entity.tbDictBlood();
                            //tb.No = vm.tbDictBlood.No == null ? db.Table<Dict.Entity.tbDictBlood>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.tbDictBlood.No;
                            tb.BloodName = vm.tbDictBlood.BloodName;
                            db.Set<Dict.Entity.tbDictBlood>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了血型");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Dict.Entity.tbDictBlood>()
                                      where p.Id == vm.tbDictBlood.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.tbDictBlood.No == null ? db.Table<Dict.Entity.tbDictBlood>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.tbDictBlood.No;
                                tb.BloodName = vm.tbDictBlood.BloodName;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了血型");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictBlood>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.BloodName,
                              Value = p.Id.ToString()
                          }).ToList();

                return tb;
            }
        }
    }
}