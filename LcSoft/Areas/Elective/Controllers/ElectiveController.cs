using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveController : Controller
    {
        public ActionResult List()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Teacher)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Elective.List();
                var tb = from p in db.Table<Entity.tbElective>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ElectiveName.Contains(vm.SearchText));
                }

                vm.ElectiveList = (from p in tb
                                   orderby p.No descending
                                   select new Dto.Elective.List
                                   {
                                       Id = p.Id,
                                       No = p.No,
                                       ElectiveName = p.ElectiveName,
                                       ElectiveTypeName = p.tbElectiveType.ElectiveTypeName,
                                       IsPop = p.IsPop,
                                       FromDate = p.FromDate,
                                       IsDisable = p.IsDisable,
                                       Remark = p.Remark,
                                       ToDate = p.ToDate
                                   }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Elective.List vm)
        {  
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                pageIndex=vm.Page.PageIndex,
                pageSize=vm.Page.PageSize
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElective>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var electiveGroupList = (from p in db.Table<Entity.tbElectiveGroup>()
                                            .Include(d => d.tbElective)
                                         where ids.Contains(p.tbElective.Id)
                                         select p).ToList();

                var electiveSectionList = (from p in db.Table<Entity.tbElectiveSection>()
                                            .Include(d => d.tbElective)
                                           where ids.Contains(p.tbElective.Id)
                                           select p).ToList();

                var electiveOrgList = (from p in db.Table<Entity.tbElectiveOrg>()
                                        .Include(d => d.tbElective)
                                       where ids.Contains(p.tbElective.Id)
                                       select p).ToList();

                var electiveClassList = (from p in db.Table<Entity.tbElectiveClass>()
                                            .Include(d => d.tbElective)
                                         where ids.Contains(p.tbElective.Id)
                                         select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    foreach (var group in electiveGroupList.Where(d => d.tbElective.Id == a.Id))
                    {
                        group.IsDeleted = true;
                    }

                    foreach (var section in electiveSectionList.Where(d => d.tbElective.Id == a.Id))
                    {
                        section.IsDeleted = true;
                    }

                    foreach (var org in electiveOrgList.Where(d => d.tbElective.Id == a.Id))
                    {
                        org.IsDeleted = true;
                    }

                    foreach (var cla in electiveClassList.Where(d => d.tbElective.Id == a.Id))
                    {
                        cla.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了选课设置");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Teacher)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Elective.Edit();
                vm.ElectiveTypeList = Areas.Elective.Controllers.ElectiveTypeController.SelectList();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                
                if (vm.ElectiveTypeList.Count > 0)
                {
                    vm.ElectiveEdit.ElectiveTypeId = vm.ElectiveTypeList.FirstOrDefault().Value.ConvertToInt();
                }

                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbElective>()
                              where p.Id == id
                              select new Dto.Elective.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  ElectiveName = p.ElectiveName,
                                  ElectiveTypeId = p.tbElectiveType.Id,
                                  IsPop = p.IsPop,
                                  FromDate = p.FromDate,
                                  ToDate = p.ToDate,
                                  IsDisable = p.IsDisable,
                                  Remark = p.Remark
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ElectiveEdit = tb;
                        vm.FromDate = tb.FromDate.ToString(Code.Common.StringToDateTime);
                        vm.ToDate = tb.ToDate.ToString(Code.Common.StringToDateTime);
                        vm.ApplyFromDate = tb.ApplyFromDate.ToString(Code.Common.StringToDateTime);
                        vm.ApplyToDate = tb.ApplyToDate.ToString(Code.Common.StringToDateTime);
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Models.Elective.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                int electiveId = 0;

                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var isExists = db.Table<Entity.tbElective>().Count(p => p.ElectiveName == vm.ElectiveEdit.ElectiveName && p.Id != vm.ElectiveEdit.Id) > 0;
                    if (isExists)
                    {
                        error.AddError("系统中已存在相同名字的选课记录!");
                    }
                    else
                    {
                        if (vm.ElectiveEdit.Id == 0)
                        {
                            var tb = new Entity.tbElective();
                            tb.No = vm.ElectiveEdit.No == null ? db.Table<Entity.tbElective>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ElectiveEdit.No;
                            tb.ElectiveName = vm.ElectiveEdit.ElectiveName;
                            tb.tbElectiveType = db.Set<Entity.tbElectiveType>().Find(vm.ElectiveEdit.ElectiveTypeId);
                            tb.IsPop = vm.ElectiveEdit.IsPop;
                            tb.IsDisable = vm.ElectiveEdit.IsDisable;
                            tb.Remark = vm.ElectiveEdit.Remark;
                            tb.FromDate = DateTime.Parse(vm.FromDate);
                            tb.ToDate = DateTime.Parse(vm.ToDate);
                            tb.ApplyFromDate = DateTime.Parse(vm.ApplyFromDate);
                            tb.ApplyToDate = DateTime.Parse(vm.ApplyToDate);

                            db.Set<Entity.tbElective>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了选课设置");
                                ElectiveSectionController.InsertDefault(db, tb.Id);
                                ElectiveGroupController.InsertDefault(db, tb.Id);
                            }

                            electiveId = tb.Id;
                        }
                        else
                        {
                            var tb = (from p in db.Table<Entity.tbElective>()
                                      where p.Id == vm.ElectiveEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                electiveId = tb.Id;
                                tb.No = vm.ElectiveEdit.No == null ? db.Table<Entity.tbElective>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ElectiveEdit.No;
                                tb.ElectiveName = vm.ElectiveEdit.ElectiveName;
                                tb.tbElectiveType = db.Set<Entity.tbElectiveType>().Find(vm.ElectiveEdit.ElectiveTypeId);
                                tb.IsPop = vm.ElectiveEdit.IsPop;
                                tb.FromDate = DateTime.Parse(vm.FromDate);
                                tb.ToDate = DateTime.Parse(vm.ToDate);
                                tb.ApplyFromDate = DateTime.Parse(vm.ApplyFromDate);
                                tb.ApplyToDate = DateTime.Parse(vm.ApplyToDate);
                                tb.IsDisable = vm.ElectiveEdit.IsDisable;
                                tb.Remark = vm.ElectiveEdit.Remark;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了选课设置");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                if (Request["Step"] != null)
                {
                    return Code.MvcHelper.Post(error, Url.Action("List", "ElectiveClass", new { ElectiveId = electiveId }));
                }
                else
                {
                    return Code.MvcHelper.Post(error, Url.Action("List"));
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDisable(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Entity.tbElective>().Find(id);
                if (tb != null)
                {
                    tb.IsDisable = !tb.IsDisable;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("更新了选课设置");
                }

                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }


        [HttpGet]
        public JsonResult IsExists()
        {
            var result = false;
            var name = Request.QueryString[0];
            var id = Request.QueryString[1].ConvertToInt();
            using (var db = new XkSystem.Models.DbContext())
            {
                if (id > 0)
                {
                    result = db.Table<Entity.tbElective>().Count(p => p.ElectiveName.Equals(name) && p.Id != id) == 0;
                }
                else
                {
                    result = db.Table<Entity.tbElective>().Count(p => p.ElectiveName.Equals(name))==0;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElective>()
                          orderby p.No descending
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ElectiveName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [HttpPost]
        public JsonResult GetElectiveType(int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return Json((from p in db.Table<Entity.tbElective>() where p.Id == electiveId select p.tbElectiveType.ElectiveTypeCode).FirstOrDefault());
            }
        }
    }
}