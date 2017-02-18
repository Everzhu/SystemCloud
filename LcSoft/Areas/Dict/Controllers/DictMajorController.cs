using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class DictMajorController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictMajor.List();
                var tb = from p in db.Table<Dict.Entity.tbDictMajor>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.MajorName.Contains(vm.SearchText)
                                || d.MajorCode.Contains(vm.SearchText));
                }

                vm.DataList = (from p in tb 
                               orderby p.No
                               select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DictMajor.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictMajor>()
                          where ids.Contains(p.Id)
                          select p).ToList();
                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了专业");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictMajor.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Dict.Entity.tbDictMajor>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.DataEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DictMajor.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Dict.Entity.tbDictMajor>().Where(d => d.MajorName == vm.DataEdit.MajorName && d.Id != vm.DataEdit.Id).Any())
                    {
                        error.AddError("该专业已存在!");
                    }
                    else
                    {
                        if (vm.DataEdit.Id == 0)
                        {
                            var tb = new Dict.Entity.tbDictMajor();
                            tb.No = vm.DataEdit.No == null ? db.Table<Dict.Entity.tbDictMajor>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No;
                            tb.MajorName = vm.DataEdit.MajorName;
                            tb.MajorCode = vm.DataEdit.MajorCode;
                            db.Set<Dict.Entity.tbDictMajor>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了专业");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Dict.Entity.tbDictMajor>()
                                      where p.Id == vm.DataEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.DataEdit.No == null ? db.Table<Dict.Entity.tbDictMajor>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No;
                                tb.MajorName = vm.DataEdit.MajorName;
                                tb.MajorCode = vm.DataEdit.MajorCode;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了专业");
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
                tb = (from p in db.Table<Dict.Entity.tbDictMajor>()
                      orderby p.No
                      select new System.Web.Mvc.SelectListItem
                      {
                          Text = p.MajorName,
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