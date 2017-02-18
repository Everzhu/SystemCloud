using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyOptionController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyOption.List();
                var tb = from p in db.Table<Study.Entity.tbStudyOption>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.StudyOptionName.Contains(vm.SearchText));
                }

                vm.StudyOptionList = (from p in tb
                                      orderby p.No
                                      select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudyOption.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Study.Entity.tbStudyOption>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除晚自习表现");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyOption.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Study.Entity.tbStudyOption>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.StudyOptionEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudyOption.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.StudyOptionEdit.Id == 0)
                    {
                        var tb = new Study.Entity.tbStudyOption();
                        tb.No = vm.StudyOptionEdit.No == null ? db.Table<Study.Entity.tbStudyOption>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudyOptionEdit.No;
                        tb.StudyOptionName = vm.StudyOptionEdit.StudyOptionName;
                        tb.StudyOptionValue = vm.StudyOptionEdit.StudyOptionValue;
                        db.Set<Study.Entity.tbStudyOption>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加晚自习表现");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Study.Entity.tbStudyOption>()
                                  where p.Id == vm.StudyOptionEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.StudyOptionEdit.No == null ? db.Table<Study.Entity.tbStudyOption>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudyOptionEdit.No;
                            tb.StudyOptionName = vm.StudyOptionEdit.StudyOptionName;
                            tb.StudyOptionValue = vm.StudyOptionEdit.StudyOptionValue;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习表现");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
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
                var tb = (from p in db.Table<Study.Entity.tbStudyOption>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.StudyOptionName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}