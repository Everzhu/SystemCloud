using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class DictPartyController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictParty.List();
                var tb = from p in db.Table<Dict.Entity.tbDictParty>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.PartyName.Contains(vm.SearchText));
                }

                vm.PartyList = (from p in tb
                                orderby p.No
                                select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DictParty.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictParty>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了党派");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictParty.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Dict.Entity.tbDictParty>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.PartyEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DictParty.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Dict.Entity.tbDictParty>().Where(d => d.PartyName == vm.PartyEdit.PartyName && d.Id != vm.PartyEdit.Id).Any())
                    {
                        error.AddError("该党派已存在!");
                    }
                    else
                    {
                        if (vm.PartyEdit.Id == 0)
                        {
                            var tb = new Dict.Entity.tbDictParty();
                            tb.No = vm.PartyEdit.No == null ? db.Table<Dict.Entity.tbDictParty>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.PartyEdit.No;
                            tb.PartyName = vm.PartyEdit.PartyName;
                            db.Set<Dict.Entity.tbDictParty>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了党派");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Dict.Entity.tbDictParty>()
                                      where p.Id == vm.PartyEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.PartyEdit.No == null ? db.Table<Dict.Entity.tbDictParty>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.PartyEdit.No;
                                tb.PartyName = vm.PartyEdit.PartyName;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改党派");
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
                var tb = (from p in db.Table<Dict.Entity.tbDictParty>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.PartyName,
                              Value = p.Id.ToString()
                          }).ToList();

                return tb;
            }
        }
    }
}