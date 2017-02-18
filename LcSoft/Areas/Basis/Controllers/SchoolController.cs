using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class SchoolController : Controller
    {
        public ActionResult Edit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.School.Edit();
                var tb = (from p in db.Table<Basis.Entity.tbSchool>()
                          where p.tbTenant.Id == Code.Common.TenantId
                          select p).FirstOrDefault();
                if (tb != null)
                {
                    vm.DataEdit = tb;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.School.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (vm.DataEdit.Id > 0)
                    {
                        var tb = db.Set<Basis.Entity.tbSchool>().Find(vm.DataEdit.Id);
                        tb.Address = vm.DataEdit.Address;
                        tb.Email = vm.DataEdit.Email;
                        tb.Phone = vm.DataEdit.Phone;
                        tb.PostCode = vm.DataEdit.PostCode;
                        tb.Remark = vm.DataEdit.Remark;
                        tb.SchoolMaster = vm.DataEdit.SchoolMaster;
                        tb.SchoolName = vm.DataEdit.SchoolName;
                        tb.SchoolNameEn = vm.DataEdit.SchoolNameEn;
                        tb.SchoolType = vm.DataEdit.SchoolType;
                    }
                    else
                    {
                        var tb = new Basis.Entity.tbSchool()
                        {
                            Address = vm.DataEdit.Address,
                            Email = vm.DataEdit.Email,
                            Phone = vm.DataEdit.Phone,
                            PostCode = vm.DataEdit.PostCode,
                            Remark = vm.DataEdit.Remark,
                            SchoolMaster = vm.DataEdit.SchoolMaster,
                            SchoolName = vm.DataEdit.SchoolName,
                            SchoolNameEn = vm.DataEdit.SchoolNameEn,
                            SchoolType = vm.DataEdit.SchoolType
                        };
                        db.Set<Basis.Entity.tbSchool>().Add(tb);
                    }

                    var tbTenant = (from p in db.TableRoot<Admin.Entity.tbTenant>() where p.IsDefault select p).FirstOrDefault();
                    if (tbTenant != null)
                    {
                        tbTenant.TenantName = vm.DataEdit.SchoolName;
                        tbTenant.Title = vm.DataEdit.SchoolName;
                    }

                    if (db.SaveChanges() > 0)
                    {
                        Code.Common.AppTitle = vm.DataEdit.SchoolName;
                        HttpContext.Cache["AppTitle"] = vm.DataEdit.SchoolName;
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增/修改了学校信息");
                    }
                }
            }

            return Code.MvcHelper.Post(error, null, "提交成功!");
        }
    }
}