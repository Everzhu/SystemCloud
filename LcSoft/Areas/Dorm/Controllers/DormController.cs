using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dorm.Controllers
{
    public class DormController : Controller
    {
        public ActionResult List()
        {
            var vm = new Models.Dorm.List();

            using (var db = new XkSystem.Models.DbContext())
            {
                var dormApplyList = db.Table<Dorm.Entity.tbDormApply>().Include(d => d.tbStudent.tbSysUser).ToList();
                var tb = db.Table<Dorm.Entity.tbDorm>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.DormName.Contains(vm.SearchText));
                }

                vm.DormList = (from p in tb
                               orderby p.No
                               select new Dto.Dorm.List()
                               {
                                   ApplyFrom = p.ApplyFrom,
                                   ApplyTo = p.ApplyTo,
                                   DormName = p.DormName,
                                   Id = p.Id,
                                   IsApply = p.IsApply,
                                   YearName = p.tbYear.YearName
                               }).ToPageList(vm.Page);

                foreach (var v in vm.DormList)
                {
                    v.IsAlreadyApply = dormApplyList.Where(d => d.tbStudent.tbSysUser.Id == Code.Common.UserId).Count() > 0 ? true : false;
                    if (v.IsAlreadyApply)
                    {
                        v.DormApplyId = dormApplyList.Where(d => d.tbStudent.tbSysUser.Id == Code.Common.UserId).FirstOrDefault().Id;
                    }
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Dorm.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Dorm.Entity.tbDorm>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Dorm.Edit();
                vm.YearList = Areas.Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
                if (vm.DormEdit.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.DormEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                if (id > 0)
                {
                    vm.DormEdit = (from p in db.Table<Dorm.Entity.tbDorm>()
                                   where p.Id == id
                                   select new Dto.Dorm.Edit()
                                   {
                                       Id = p.Id,
                                       DormName = p.DormName,
                                       IsApply = p.IsApply,
                                       YearId = p.tbYear.Id,
                                       ApplyFrom = p.ApplyFrom,
                                       ApplyTo = p.ApplyTo
                                   }).FirstOrDefault();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Dorm.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (db.Table<Dorm.Entity.tbDorm>().Where(d => d.tbYear.Id == vm.DormEdit.YearId).Count() == 0)
                    {
                        var tb = new Dorm.Entity.tbDorm()
                        {
                            No = vm.DormEdit.No,
                            ApplyFrom = vm.DormEdit.ApplyFrom,
                            ApplyTo = vm.DormEdit.ApplyTo,
                            DormName = vm.DormEdit.DormName,
                            IsApply = vm.DormEdit.IsApply,
                            tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.DormEdit.YearId)
                        };
                        db.Set<Dorm.Entity.tbDorm>().Add(tb);
                    }
                    else
                    {
                        var tb = db.Table<Dorm.Entity.tbDorm>().Where(d => d.tbYear.Id == vm.DormEdit.YearId).FirstOrDefault();
                        tb.ApplyFrom = vm.DormEdit.ApplyFrom;
                        tb.ApplyTo = vm.DormEdit.ApplyTo;
                        tb.DormName = vm.DormEdit.DormName;
                        tb.IsApply = vm.DormEdit.IsApply;
                        tb.No = vm.DormEdit.No;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.DormEdit.YearId);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改/添加了住宿");
                    }
                }
            }
            return Code.MvcHelper.Post(error);
        }

        public static List<System.Web.Mvc.SelectListItem> SelectList(int id = 0)
        {
            var tb = new List<System.Web.Mvc.SelectListItem>();

            using (var db = new XkSystem.Models.DbContext())
            {
                tb = (from p in db.Table<Dorm.Entity.tbDorm>()
                      where p.IsApply == true
                      select new System.Web.Mvc.SelectListItem()
                      {
                          Text = p.DormName,
                          Value = p.Id.ToString()
                      }).ToList();
                if (id > 0)
                {
                    tb.Where(d => d.Value == id.ToString()).FirstOrDefault().Selected = true;
                }
            }

            return tb;
        }

        public ActionResult SetApply(int Id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Dorm.Entity.tbDorm>().Find(Id);
                tb.IsApply = !tb.IsApply;
                db.SaveChanges();
            }
            return Code.MvcHelper.Post();
        }























    }
}