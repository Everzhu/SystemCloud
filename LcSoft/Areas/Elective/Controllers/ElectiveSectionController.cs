using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveSectionController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveSection.List();
                vm.ElectiveName = db.Table<Entity.tbElective>().FirstOrDefault(d => d.Id == vm.ElectiveId).ElectiveName;

                var tb = from p in db.Table<Entity.tbElectiveSection>()
                         where p.tbElective.Id == vm.ElectiveId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ElectiveSectionName.Contains(vm.SearchText));
                }

                vm.ElectiveSectionList = (from p in tb
                                          orderby p.No
                                          select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveSection.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, electiveId = vm.ElectiveId }));
        }

        public JsonResult Delete(List<int> ids, int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var count = db.Table<Entity.tbElectiveSection>().Count(d => d.tbElective.Id == electiveId);
                if (count == decimal.One)
                {
                    error.Add("至少应该保留一个分段数据");
                }

                var tb = (from p in db.Table<Entity.tbElectiveSection>()
                          where ids.Contains(p.Id)
                          select p).FirstOrDefault();
                if (tb != null)
                {
                    tb.IsDeleted = true;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除选课分段");
                    }
                }

                return Code.MvcHelper.Post(error, Url.Action("List", new { electiveId = electiveId }));
            }    
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveSection.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbElectiveSection>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ElectiveSectionEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ElectiveSection.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                if (vm.ElectiveSectionEdit.MaxElective < vm.ElectiveSectionEdit.MinElective)
                {
                    error.AddError("最大选课数和最小选课数设置错误，无法比较大小!");
                }

                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Entity.tbElectiveSection>().Where(d=>d.ElectiveSectionName == vm.ElectiveSectionEdit.ElectiveSectionName && d.tbElective.Id == vm.ElectiveId && d.Id != vm.ElectiveSectionEdit.Id).Any())
                    {
                        error.AddError("该选课分段已存在!");
                    }
                    else
                    {
                        if (vm.ElectiveSectionEdit.Id == 0)
                        {
                            var tb = new Entity.tbElectiveSection();
                            tb.No = vm.ElectiveSectionEdit.No == null ? db.Table<Entity.tbElectiveSection>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ElectiveSectionEdit.No;
                            tb.tbElective = db.Set<Entity.tbElective>().Find(vm.ElectiveId);
                            tb.ElectiveSectionName = vm.ElectiveSectionEdit.ElectiveSectionName;
                            tb.MaxElective = vm.ElectiveSectionEdit.MaxElective;
                            tb.MinElective = vm.ElectiveSectionEdit.MinElective;
                            db.Set<Entity.tbElectiveSection>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了选课时段");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Entity.tbElectiveSection>()
                                      where p.Id == vm.ElectiveSectionEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.ElectiveSectionEdit.No == null ? db.Table<Entity.tbElectiveSection>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ElectiveSectionEdit.No;
                                tb.ElectiveSectionName = vm.ElectiveSectionEdit.ElectiveSectionName;
                                tb.MaxElective = vm.ElectiveSectionEdit.MaxElective;
                                tb.MinElective = vm.ElectiveSectionEdit.MinElective;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了选课时段");
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
        public static string InsertDefault(XkSystem.Models.DbContext db, int electiveId)
        {
            var tb = new Entity.tbElectiveSection();
            tb.No = 1;
            tb.tbElective = db.Set<Entity.tbElective>().Find(electiveId);
            tb.ElectiveSectionName = "默认分段";
            tb.MaxElective = 99;
            tb.MinElective = 0;
            db.Set<Entity.tbElectiveSection>().Add(tb);

            if (db.SaveChanges() > 0)
            {
                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了选课时段");
            }

            return string.Empty;
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveSection>()
                          where p.tbElective.Id == electiveId
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ElectiveSectionName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<Dto.ElectiveSection.Info> SelectInfoList(int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveSection>()
                          where p.tbElective.Id == electiveId
                          orderby p.No
                          select new Dto.ElectiveSection.Info
                          {
                              Id = p.Id,
                              No = p.No,
                              ElectiveSectionName = p.ElectiveSectionName,
                              MaxElective = p.MaxElective,
                              MinElective = p.MinElective
                          }).ToList();
                return tb;
            }
        }
    }
}