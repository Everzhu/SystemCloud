using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Admin.Controllers
{
    public class TeanntProgramController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeanntProgram.List();
                var tb = from p in db.Table<Admin.Entity.tbTenantProgram>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbProgram.ProgramName.Contains(vm.SearchText));
                }

                vm.TeanntProgramList = (from p in tb
                                        orderby p.No
                                        select new Dto.TeanntProgram.List
                                        {
                                            Id = p.Id,
                                            No = p.No,
                                            TeanntProgramName = p.tbProgram.ProgramName
                                        }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.TeanntProgram.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Admin.Entity.tbTenantProgram>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了程序类型");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeanntProgram.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Admin.Entity.tbTenantProgram>()
                              where p.Id == id
                              select new Dto.TeanntProgram.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  TeanntProgramName = p.tbProgram.ProgramName
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.TeanntProgramEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.TeanntProgram.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.TeanntProgramEdit.Id == 0)
                    {
                        var tb = new Admin.Entity.tbTenantProgram();
                        tb.No = vm.TeanntProgramEdit.No;
                        tb.tbProgram.ProgramName = vm.TeanntProgramEdit.TeanntProgramName;
                        db.Set<Admin.Entity.tbTenantProgram>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了程序类型");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Admin.Entity.tbTenantProgram>()
                                  where p.Id == vm.TeanntProgramEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.TeanntProgramEdit.No;
                            tb.tbProgram.ProgramName = vm.TeanntProgramEdit.TeanntProgramName;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了程序类型");
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
                var tb = (from p in db.Table<Admin.Entity.tbTenantProgram>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbProgram.ProgramName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectListByTenant(int tenantId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Set<Admin.Entity.tbTenantProgram>()
                          where p.tbTenant.Id == tenantId && p.IsDeleted == false
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbProgram.ProgramName,
                              Value = p.tbProgram.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}