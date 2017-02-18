using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class DictEducationController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictEducation.List();
                var tb = from p in db.Table<Dict.Entity.tbDictEducation>().Include(d => d.tbDictDegree)
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.EducationName.Contains(vm.SearchText));
                }

                vm.DataList = (from p in tb
                               orderby p.No
                               select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DictEducation.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictEducation>()
                          where ids.Contains(p.Id)
                          select p).ToList();
                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了教育程度");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictEducation.Edit();

                if (id != 0)
                {
                    vm.DataEdit = db.Table<Dict.Entity.tbDictEducation>().Include(d => d.tbDictDegree)
                        .Where(d => d.Id == id).FirstOrDefault();
                }
                vm.DictDegreeList = DictDegreeController.SelectList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DictEducation.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (db.Table<Dict.Entity.tbDictEducation>().Where(d => d.EducationName == vm.DataEdit.EducationName && d.Id != vm.DataEdit.Id).Any())
                    {
                        error.AddError("该教育程度已存在!");
                        return Code.MvcHelper.Post(error);
                    }
                    else
                    {
                        if (vm.DataEdit.Id == 0)
                        {
                            var tb = new Dict.Entity.tbDictEducation();
                            tb.No = vm.DataEdit.No == null ? db.Table<Dict.Entity.tbDictEducation>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No;
                            tb.EducationName = vm.DataEdit.EducationName;
                            tb.tbDictDegree = db.Set<Entity.tbDictDegree>().Find(vm.DataEdit.tbDictDegree.Id);
                            db.Set<Dict.Entity.tbDictEducation>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了教育程度");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Dict.Entity.tbDictEducation>()
                                      where p.Id == vm.DataEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.DataEdit.No == null ? db.Table<Dict.Entity.tbDictEducation>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No;
                                tb.EducationName = vm.DataEdit.EducationName;
                                tb.tbDictDegree = db.Set<Entity.tbDictDegree>().Find(vm.DataEdit.tbDictDegree.Id);
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了教育程度");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }
            }

            return Code.MvcHelper.Post(error);
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return (from p in db.Table<Dict.Entity.tbDictEducation>()
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = p.EducationName,
                            Value = p.Id.ToString()
                        }).ToList();
            }
        }

    }
}