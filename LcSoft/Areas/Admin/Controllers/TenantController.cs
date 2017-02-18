using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;

namespace XkSystem.Areas.Admin.Controllers
{
    public class TenantController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Tenant.List();
                var tb = from p in db.TableRoot<Admin.Entity.tbTenant>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.TenantName.Contains(vm.SearchText));
                }

                vm.TenantList = (from p in tb
                                 orderby p.TenantName
                                 select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Tenant.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.TableRoot<Admin.Entity.tbTenant>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了学校");
                    var cache = new Areas.Sys.Controllers.SysIndexController();
                    cache.RestartCache();
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Tenant.Edit();
                vm.ProgramList = Areas.Admin.Controllers.ProgramController.SelectList();

                if (id != 0)
                {
                    var tb = (from p in db.TableRoot<Admin.Entity.tbTenant>()
                              where p.Id == id
                              select new Dto.Tenant.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  TenantName = p.TenantName,
                                  Title = p.Title,
                                  Address = p.Address,
                                  Cdkey = p.Cdkey,
                                  Deadine = p.Deadine,
                                  Host = p.Host,
                                  IsDefault = p.IsDefault,
                                  IsVip = p.IsVip,
                                  Phone = p.Phone,
                                  Power = p.Power,
                                  Remark = p.Remark,
                                  Logo = p.Logo
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.TenantEdit = tb;
                        vm.TenantEdit.AdminLoginCode = "admin";
                        vm.TenantEdit.AdminPassword = "admin";
                        vm.TenantEdit.PasswordConfirm = "admin";
                        vm.TenantProgramList = Areas.Admin.Controllers.TeanntProgramController.SelectListByTenant(id);
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Tenant.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var file = Request.Files["TenantEdit.Logo"];

                    if (file.ContentLength > 0 && Code.Common.GetFileType(file.FileName) != Code.FileType.Image)
                    {
                        return Content("<script >alert('图片格式必须是jpg、jpeg、png、bmp格式！');history.go(-1);</script >", "text/html");
                    }

                    var programIds = new List<int>();
                    if (Request["CboxProgram"] != null)
                    {
                        programIds = Request["CboxProgram"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                    }

                    if (vm.TenantEdit.IsDefault)
                    {
                        var list = from p in db.TableRoot<Admin.Entity.tbTenant>()
                                   select p;

                        foreach (Admin.Entity.tbTenant tenant in list)
                        {
                            tenant.IsDefault = false;
                        }

                        db.SaveChanges();
                    }

                    var tb = new Admin.Entity.tbTenant();
                    if (vm.TenantEdit.Id == 0)
                    {
                        #region
                        tb.No = vm.TenantEdit.No == null ? db.TableRoot<Admin.Entity.tbTenant>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.TenantEdit.No;
                        tb.TenantName = vm.TenantEdit.TenantName;
                        tb.Title = vm.TenantEdit.Title;
                        tb.IsDefault = vm.TenantEdit.IsDefault;
                        tb.Host = vm.TenantEdit.Host;
                        tb.Cdkey = vm.TenantEdit.Cdkey;
                        tb.Power = vm.TenantEdit.Power;
                        tb.IsVip = vm.TenantEdit.IsVip;
                        tb.Power = vm.TenantEdit.Power;
                        tb.Address = vm.TenantEdit.Address;
                        tb.Deadine = vm.TenantEdit.Deadine;
                        tb.Remark = vm.TenantEdit.Remark;
                        db.Set<Admin.Entity.tbTenant>().Add(tb);

                        if (file.ContentLength > 0)
                        {
                            var fileName = "Logo.png";
                            var filePath = Server.MapPath("~/Content/Images/") + fileName; // DateTime.Now.ToString("yyyyMMddHHmmssfff") + r.Next(10000, 99999).ToString() + "." + file.FileName.Split('.').Last();
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                            file.SaveAs(filePath);
                            tb.Logo = fileName;
                        }

                        if (db.SaveChanges() > 0)
                        {
                            Migrations.Configuration.Seed(db, db.Set<Admin.Entity.tbTenant>().Find(tb.Id), vm.TenantEdit.AdminLoginCode, vm.TenantEdit.AdminPassword);

                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了学校");
                        }

                        #region 系统权限
                        programIds.ForEach(programId =>
                        {
                            var program = (from p in db.TableRoot<Admin.Entity.tbProgram>()
                                           where p.Id == programId
                                           select p).FirstOrDefault();
                            var tbTenantProgram = new Admin.Entity.tbTenantProgram();
                            tbTenantProgram.No = 0;
                            tbTenantProgram.tbTenant = tb;
                            tbTenantProgram.tbProgram = program;
                            db.Set<Admin.Entity.tbTenantProgram>().Add(tbTenantProgram);
                        });
                        db.SaveChanges();
                        #endregion
                        #endregion
                    }
                    else
                    {
                        #region
                        tb = (from p in db.TableRoot<Admin.Entity.tbTenant>()
                              where p.Id == vm.TenantEdit.Id
                              select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.TenantName = vm.TenantEdit.TenantName;
                            tb.No = vm.TenantEdit.No == null ? db.TableRoot<Admin.Entity.tbTenant>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.TenantEdit.No;
                            tb.Title = vm.TenantEdit.Title;
                            tb.IsDefault = vm.TenantEdit.IsDefault;
                            tb.Host = vm.TenantEdit.Host;
                            tb.Cdkey = vm.TenantEdit.Cdkey;
                            tb.Power = vm.TenantEdit.Power;
                            tb.IsVip = vm.TenantEdit.IsVip;
                            tb.Power = vm.TenantEdit.Power;
                            tb.Address = vm.TenantEdit.Address;
                            tb.Deadine = vm.TenantEdit.Deadine;
                            tb.Remark = vm.TenantEdit.Remark;

                            if (file.ContentLength > 0)
                            {
                                var fileName = "Logo.png";
                                var filePath = Server.MapPath("~/Content/Images/") + fileName; // DateTime.Now.ToString("yyyyMMddHHmmssfff") + r.Next(10000, 99999).ToString() + "." + file.FileName.Split('.').Last();
                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath);
                                }
                                file.SaveAs(filePath);
                                tb.Logo = fileName;
                            }

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了学校");
                            }

                            #region 系统权限
                            var tenantPrograms = (from p in db.Set<Admin.Entity.tbTenantProgram>().Where(d => d.IsDeleted == false)
                                                  where p.tbTenant.Id == tb.Id
                                                  select p).ToList();
                            tenantPrograms.ForEach(tenantProgram =>
                            {
                                tenantProgram.IsDeleted = true;
                            });
                            programIds.ForEach(programId =>
                            {
                                var program = (from p in db.TableRoot<Admin.Entity.tbProgram>()
                                               where p.Id == programId
                                               select p).FirstOrDefault();
                                var tbTenantProgram = new Admin.Entity.tbTenantProgram();
                                tbTenantProgram.No = 0;
                                tbTenantProgram.tbTenant = tb;
                                tbTenantProgram.tbProgram = program;
                                db.Set<Admin.Entity.tbTenantProgram>().Add(tbTenantProgram);
                            });
                            db.SaveChanges();
                            #endregion
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                        #endregion
                    }

                    var cache = new Areas.Sys.Controllers.SysIndexController();
                    cache.RestartCache();
                }

                return Content("<script>window.parent.location.reload();</script>"); //Code.MvcHelper.Post(error);
            }
        }

        [AllowAnonymous]
        public ActionResult GetTenant(string q)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                q = q.ConvertToString();
                var tb = (from p in db.TableRoot<Admin.Entity.tbTenant>()
                          where p.TenantName.Contains(q)
                          orderby p.TenantName
                          select p.TenantName).Take(10).ToList();

                return Json(tb, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDefault(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Admin.Entity.tbTenant>().Find(id);
                if (tb != null)
                {
                    var list = from p in db.TableRoot<Admin.Entity.tbTenant>()
                               select p;
                    foreach (var a in list)
                    {
                        a.IsDefault = false;
                    }

                    tb.IsDefault = true;

                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了租户");
                }

                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }
    }
}