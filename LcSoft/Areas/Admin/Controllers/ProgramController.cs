using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Admin.Controllers
{
    [Description("控制器描述")]
    public class ProgramController : Controller
    {
        [Description("Action描述")]
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Program.List();
                var tb = from p in db.TableRoot<Admin.Entity.tbProgram>()
                         select p;
                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ProgramName.Contains(vm.SearchText));
                }

                vm.ProgramList = (from p in tb
                                  orderby p.No
                                  select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Program.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.TableRoot<Admin.Entity.tbProgram>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了程序");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Program.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.TableRoot<Admin.Entity.tbProgram>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ProgramEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Program.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ProgramEdit.Id == 0)
                    {
                        var tb = new Admin.Entity.tbProgram();
                        tb.No = vm.ProgramEdit.No;
                        tb.ProgramName = vm.ProgramEdit.ProgramName;
                        tb.IsDefault = vm.ProgramEdit.IsDefault;
                        tb.ProgramTitle = vm.ProgramEdit.ProgramTitle;
                        tb.IsWide = vm.ProgramEdit.IsWide;
                        tb.Startup = vm.ProgramEdit.Startup;
                        db.Set<Admin.Entity.tbProgram>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了程序信息");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.TableRoot<Admin.Entity.tbProgram>()
                                  where p.Id == vm.ProgramEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.ProgramEdit.No;
                            tb.ProgramName = vm.ProgramEdit.ProgramName;
                            tb.IsDefault = vm.ProgramEdit.IsDefault;
                            tb.ProgramTitle = vm.ProgramEdit.ProgramTitle;
                            tb.IsWide = vm.ProgramEdit.IsWide;
                            tb.Startup = vm.ProgramEdit.Startup;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了程序信息");
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
                var tb = (from p in db.TableRoot<Admin.Entity.tbProgram>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ProgramName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<Dto.Program.Info> InfoList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.TableRoot<Admin.Entity.tbProgram>()
                          orderby p.No
                          select new Dto.Program.Info
                          {
                              Id = p.Id,
                              No = p.No,
                              ProgramName = p.ProgramName,
                              IsDefault = p.IsDefault,
                              Startup = p.Startup,
                              BgIcon = p.BgIcon,
                              BgColor = p.BgColor,
                              Remark = p.Remark
                          }).ToList();
                return tb;
            }
        }
    }
}