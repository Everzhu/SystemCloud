using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class BuildController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Build.List();
                var tb = from p in db.Table<Basis.Entity.tbBuild>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.BuildName.Contains(vm.SearchText));
                }

                vm.BuildList = (from p in tb
                                    orderby p.No, p.BuildName
                                    select new Dto.Build.List
                                    {
                                        Id = p.Id,
                                        No = p.No,
                                        BuildName = p.BuildName,
                                        BuildTypeName = p.tbBuildType.BuildTypeName
                                    }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Build.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbBuild>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var roomList = (from p in db.Table<Basis.Entity.tbRoom>()
                                    .Include(d => d.tbBuild)
                                where ids.Contains(p.tbBuild.Id)
                                select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    foreach (var room in roomList.Where(d=>d.tbBuild.Id == a.Id))
                    {
                        room.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了教学楼");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Build.Edit();
                vm.BuildTypeList = BuildTypeController.SelectList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbBuild>()
                              where p.Id == id
                              select new Dto.Build.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  BuildName = p.BuildName,
                                  BuildTypeId = p.tbBuildType.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.BuildEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Build.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Basis.Entity.tbBuild>().Where(d=>d.BuildName == vm.BuildEdit.BuildName && d.Id != vm.BuildEdit.Id).Any())
                    {
                        error.AddError("该教学楼已存在!");
                    }
                    else
                    {
                        if (vm.BuildEdit.Id == 0)
                        {
                            var tb = new Basis.Entity.tbBuild();
                            tb.No = vm.BuildEdit.No == null ? db.Table<Basis.Entity.tbBuild>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.BuildEdit.No;
                            tb.BuildName = vm.BuildEdit.BuildName;
                            tb.tbBuildType = db.Set<Basis.Entity.tbBuildType>().Find(vm.BuildEdit.BuildTypeId);
                            db.Set<Basis.Entity.tbBuild>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了教学楼");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Basis.Entity.tbBuild>()
                                      where p.Id == vm.BuildEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.BuildEdit.No == null ? db.Table<Basis.Entity.tbBuild>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.BuildEdit.No;
                                tb.BuildName = vm.BuildEdit.BuildName;
                                tb.tbBuildType = db.Set<Basis.Entity.tbBuildType>().Find(vm.BuildEdit.BuildTypeId);

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了教学楼");
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
        public static List<System.Web.Mvc.SelectListItem> SelectList(int buildId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var list = (from p in db.Table<Basis.Entity.tbBuild>()
                            orderby p.No, p.BuildName
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.BuildName,
                                Value = p.Id.ToString(),
                                Selected = p.Id == buildId
                            }).ToList();
                return list;
            }
        }
    }
}