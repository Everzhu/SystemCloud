using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveGroupController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveGroup.List();
                vm.ElectiveName = db.Table<Entity.tbElective>().FirstOrDefault(d => d.Id == vm.ElectiveId).ElectiveName;

                var tb = from p in db.Table<Entity.tbElectiveGroup>()
                         where p.tbElective.Id == vm.ElectiveId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ElectiveGroupName.Contains(vm.SearchText));
                }

                vm.ElectiveGroupList = (from p in tb
                                        orderby p.No
                                        select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveGroup.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, electiveId = vm.ElectiveId }));
        }

        public JsonResult Delete(List<int> ids, int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var count = db.Table<Entity.tbElectiveGroup>().Count(d => d.tbElective.Id == electiveId);
                if (count == decimal.One)
                {
                    error.Add("至少应该保留一个分组数据");
                }

                var tb = (from p in db.Table<Entity.tbElectiveGroup>()
                          where ids.Contains(p.Id)
                          select p).FirstOrDefault();
                if (tb != null)
                {
                    tb.IsDeleted = true;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除选课分组");
                    }
                }

                return Code.MvcHelper.Post(error, Url.Action("List", new { electiveId = electiveId }));
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveGroup.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbElectiveGroup>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ElectiveGroupEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ElectiveGroup.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                if (vm.ElectiveGroupEdit.MaxElective < vm.ElectiveGroupEdit.MinElective)
                {
                    error.AddError("最大选课数和最小选课数设置错误，无法比较大小!");
                }

                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Entity.tbElectiveGroup>().Where(d=>d.ElectiveGroupName == vm.ElectiveGroupEdit.ElectiveGroupName && d.tbElective.Id == vm.ElectiveId && d.Id != vm.ElectiveGroupEdit.Id).Any())
                    {
                        error.AddError("该选课组已存在!");
                    }
                    else
                    {
                        if (vm.ElectiveGroupEdit.Id == 0)
                        {
                            var tb = new Entity.tbElectiveGroup();
                            tb.No = vm.ElectiveGroupEdit.No == null ? db.Table<Entity.tbElectiveGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ElectiveGroupEdit.No;
                            tb.tbElective = db.Set<Entity.tbElective>().Find(vm.ElectiveId);
                            tb.ElectiveGroupName = vm.ElectiveGroupEdit.ElectiveGroupName;
                            tb.MaxElective = vm.ElectiveGroupEdit.MaxElective;
                            tb.MinElective = vm.ElectiveGroupEdit.MinElective;
                            db.Set<Entity.tbElectiveGroup>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了选课时组");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Entity.tbElectiveGroup>()
                                      where p.Id == vm.ElectiveGroupEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.ElectiveGroupEdit.No == null ? db.Table<Entity.tbElectiveGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ElectiveGroupEdit.No;
                                tb.ElectiveGroupName = vm.ElectiveGroupEdit.ElectiveGroupName;
                                tb.MaxElective = vm.ElectiveGroupEdit.MaxElective;
                                tb.MinElective = vm.ElectiveGroupEdit.MinElective;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了选课时组");
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
            var tb = new Entity.tbElectiveGroup();
            tb.No = 1;
            tb.tbElective = db.Set<Entity.tbElective>().Find(electiveId);
            tb.ElectiveGroupName = "默认分组";
            tb.MaxElective = 99;
            tb.MinElective = 0;
            db.Set<Entity.tbElectiveGroup>().Add(tb);
            
            if (db.SaveChanges() > 0)
            {
                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了选课分组");
            }

            return string.Empty;
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveGroup>()
                          where p.tbElective.Id == electiveId
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ElectiveGroupName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<Entity.tbElectiveGroup> SelectInfoList(int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveGroup>()
                          where p.tbElective.Id == electiveId
                          orderby p.No
                          select p).ToList();
                return tb;
            }
        }
    }
}