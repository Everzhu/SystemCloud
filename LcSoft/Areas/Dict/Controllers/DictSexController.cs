using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class DictSexController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Sex.List();
                var tb = from p in db.Table<Dict.Entity.tbDictSex>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.SexName.Contains(vm.SearchText));
                }

                vm.SexList = (from p in tb
                              orderby p.No
                              select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Sex.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictSex>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了性别");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Sex.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Dict.Entity.tbDictSex>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.SexEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Sex.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Dict.Entity.tbDictSex>().Where(d => d.SexName == vm.SexEdit.SexName && d.Id != vm.SexEdit.Id).Any())
                    {
                        error.AddError("该性别已存在!");
                    }
                    else
                    {
                        if (vm.SexEdit.Id == 0)
                        {
                            var tb = new Dict.Entity.tbDictSex();
                            tb.No = vm.SexEdit.No == null ? db.Table<Dict.Entity.tbDictSex>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SexEdit.No;
                            tb.SexName = vm.SexEdit.SexName;
                            db.Set<Dict.Entity.tbDictSex>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了性别");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Dict.Entity.tbDictSex>()
                                      where p.Id == vm.SexEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.SexEdit.No == null ? db.Table<Dict.Entity.tbDictSex>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SexEdit.No;
                                tb.SexName = vm.SexEdit.SexName;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改性别");
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
        public static List<System.Web.Mvc.SelectListItem> SelectList(int id = 0)
        {
            var tb = new List<System.Web.Mvc.SelectListItem>();
            using (var db = new XkSystem.Models.DbContext())
            {
                tb = (from p in db.Table<Dict.Entity.tbDictSex>()
                      orderby p.No
                      select new System.Web.Mvc.SelectListItem
                      {
                          Text = p.SexName,
                          Value = p.Id.ToString()
                      }).ToList();

                if (id > 0)
                {
                    tb.Where(d => d.Value == id.ConvertToString()).FirstOrDefault().Selected = true;
                }
            }
            return tb;
        }
    }
}