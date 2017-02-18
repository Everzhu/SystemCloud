using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class YearController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Year.List();
                var tb = from p in db.Table<Basis.Entity.tbYear>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.YearName.Contains(vm.SearchText));
                }

                var result = (from p in tb
                              orderby p.No
                              select new Dto.Year.List
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  YearName = p.YearName,
                                  IsDisable = p.IsDisable,
                                  IsDefault = p.IsDefault,
                                  FromDate = p.FromDate,
                                  ToDate = p.ToDate,
                                  YearType = p.YearType,
                                  ParentId = p.tbYearParent != null ? p.tbYearParent.Id : 0
                              }).ToList();
                vm.YearList = (from p in result where p.YearType == Code.EnumHelper.YearType.Year select p).ToList();

                vm.YearList.ForEach(p =>
                {
                    p.ChildList = (from t in result where t.ParentId == p.Id select t).ToList();
                });


                vm.YearList.ForEach(p =>
                {
                    p.ChildList.ForEach(t =>
                    {
                        t.ChildList = (from s in result where s.ParentId == t.Id select s).ToList();
                    });
                });

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Year.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbYear>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    var delList = (from p in db.Table<Basis.Entity.tbYear>()
                                   where (p.tbYearParent.Id == a.Id || p.tbYearParent.tbYearParent.Id == a.Id)
                                   select p).ToList();
                    foreach (var del in delList)
                    {
                        del.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了学年学段");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Year.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbYear>()
                              where p.Id == id
                              select new Dto.Year.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  YearName = p.YearName,
                                  IsDisable = p.IsDisable,
                                  IsDefault = p.IsDefault,
                                  FromDate = p.FromDate,
                                  ToDate = p.ToDate,
                                  YearTypeCode = p.YearType
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.YearEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Year.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.YearEdit.ToDate <= vm.YearEdit.FromDate)
                    {
                        error.AddError("结束时间必须晚于开始时间！");
                        return Code.MvcHelper.Post(error);
                    }

                    if ((vm.YearEdit.ToDate.HasValue && !vm.YearEdit.FromDate.HasValue) || (!vm.YearEdit.ToDate.HasValue && vm.YearEdit.FromDate.HasValue))
                    {
                        error.AddError("开始时间和结束时间必须成对设置！");
                        return Code.MvcHelper.Post(error);
                    }

                    if (db.Table<Basis.Entity.tbYear>().Where(d => d.YearName == vm.YearEdit.YearName && d.Id != vm.YearEdit.Id).Any())
                    {
                        error.AddError("该学年已存在!");
                        return Code.MvcHelper.Post(error);
                    }

                    if (vm.YearEdit.IsDefault)
                    {
                        var tb = from p in db.Table<Basis.Entity.tbYear>()
                                 select p;

                        foreach (var section in tb)
                        {
                            section.IsDefault = false;
                        }

                        db.SaveChanges();
                    }

                    if (vm.YearEdit.Id == 0)
                    {
                        var listTbYear = new List<Entity.tbYear>();
                        var yearNum = 0;
                        if (!int.TryParse(vm.YearEdit.YearName.Substring(0, 4), out yearNum))
                        {
                            return Code.MvcHelper.Post(new List<string>() { "学年名称的前4位必须为年份数字！" });
                        }


                        var tbTenant = db.Set<Admin.Entity.tbTenant>().Find(Code.Common.TenantId);
                        var tb = new Basis.Entity.tbYear();
                        //tb.No = vm.YearEdit.No == null ? db.Table<Basis.Entity.tbYear>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.YearEdit.No;
                        tb.No = yearNum * 100;
                        tb.YearName = vm.YearEdit.YearName;
                        tb.IsDisable = vm.YearEdit.IsDisable;
                        tb.IsDefault = vm.YearEdit.IsDefault;
                        tb.FromDate = vm.YearEdit.FromDate;
                        tb.ToDate = vm.YearEdit.ToDate;
                        tb.tbTenant = tbTenant;

                        listTbYear.Add(tb);

                        #region 添加子级学期、学段

                        var term1 = new Areas.Basis.Entity.tbYear()
                        {
                            No = yearNum * 100 + 10,
                            YearName = yearNum + "-" + (yearNum + 1) + "学年上学期",
                            tbYearParent = tb,
                            tbTenant = tbTenant,                            
                            YearType = Code.EnumHelper.YearType.Term,
                            IsDisable = tb.IsDisable
                        };

                        listTbYear.Add(term1);

                        var term2 = new Areas.Basis.Entity.tbYear()
                        {
                            No = yearNum * 100 + 20,
                            YearName = yearNum + "-" + (yearNum + 1) + "学年下学期",
                            tbYearParent = tb,
                            tbTenant = tbTenant,
                            YearType = Code.EnumHelper.YearType.Term,
                            IsDisable = tb.IsDisable
                        };
                        listTbYear.Add(term2);

                        var section1 = new Areas.Basis.Entity.tbYear()
                        {
                            No = yearNum * 100 + 11,
                            YearName = yearNum + "-" + (yearNum + 1) + "学年上学期期中",
                            tbYearParent = term1,
                            tbTenant = tbTenant,
                            YearType = Code.EnumHelper.YearType.Section,
                            IsDisable = tb.IsDisable
                        };
                        listTbYear.Add(section1);

                        var section2 = new Areas.Basis.Entity.tbYear()
                        {
                            No = yearNum * 100 + 12,
                            YearName = yearNum + "-" + (yearNum + 1) + "学年上学期期末",
                            tbYearParent = term1,
                            tbTenant = tbTenant,
                            YearType = Code.EnumHelper.YearType.Section,
                            IsDisable = tb.IsDisable
                        };
                        listTbYear.Add(section2);

                        var section3 = new Areas.Basis.Entity.tbYear()
                        {
                            No = yearNum * 100 + 23,
                            YearName = yearNum + "-" + (yearNum + 1) + "学年下学期期中",
                            tbYearParent = term2,
                            tbTenant = tbTenant,
                            YearType = Code.EnumHelper.YearType.Section,
                            IsDisable = tb.IsDisable
                        };
                        listTbYear.Add(section3);
                        var section4 = new Areas.Basis.Entity.tbYear()
                        {
                            No = yearNum * 100 + 24,
                            YearName = yearNum + "-" + (yearNum + 1) + "学年下学期期末",
                            tbYearParent = term2,
                            tbTenant = tbTenant,
                            YearType = Code.EnumHelper.YearType.Section,
                            IsDisable = tb.IsDisable
                        };
                        listTbYear.Add(section4);

                        #endregion


                        db.Set<Entity.tbYear>().AddRange(listTbYear);

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了学年学段");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Basis.Entity.tbYear>().Include(p => p.tbYearParent)
                                  where p.Id == vm.YearEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            if (vm.YearEdit.YearName.Length < 9)
                            {
                                error.AddError("学年名称格式不正确，格式为：yyyy-yyyy学年，如:2015-2016学年！");
                                return Code.MvcHelper.Post(error);
                            }
                            tb.No = vm.YearEdit.No == null ? db.Table<Basis.Entity.tbYear>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.YearEdit.No;
                            tb.YearName = vm.YearEdit.YearName;
                            tb.IsDisable = vm.YearEdit.IsDisable;
                            tb.IsDefault = vm.YearEdit.IsDefault;
                            tb.FromDate = vm.YearEdit.FromDate;
                            tb.ToDate = vm.YearEdit.ToDate;


                            //同步设置所有子级
                            if (tb.YearType != Code.EnumHelper.YearType.Section)
                            {
                                var childs = GetSon(db, tb.Id);
                                if (childs != null)
                                {
                                    foreach (var item in childs)
                                    {
                                        item.IsDisable = tb.IsDisable;
                                        item.YearName = tb.YearName.Substring(0, 9) + item.YearName.Remove(0, 9);
                                    }
                                }
                            }

                            //设置为启用时，同步设置所有上级
                            if (tb.YearType != Code.EnumHelper.YearType.Year && !tb.IsDisable)
                            {
                                var parents = GetParent(db, tb.tbYearParent.Id);
                                foreach (var item in parents)
                                {
                                    item.IsDisable = tb.IsDisable;
                                }
                            }


                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了学年学段");
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


        /// <summary>
        /// 递归获取子级
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private IEnumerable<Basis.Entity.tbYear> GetSon(XkSystem.Models.DbContext db, int id)
        {
            var tb = (from p in db.Table<Basis.Entity.tbYear>() where p.tbYearParent.Id == id select p);
            return tb.ToList().Concat(tb.ToList().SelectMany(p => GetSon(db, p.Id)));
        }

        /// <summary>
        /// 递归获取父级
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private IEnumerable<Basis.Entity.tbYear> GetParent(XkSystem.Models.DbContext db, int id)
        {
            var tb = (from p in db.Table<Basis.Entity.tbYear>().Include(p => p.tbYearParent) where p.Id == id select p).ToList();
            if (tb.FirstOrDefault().tbYearParent == null)
            {
                return tb;
            }
            return tb.Concat(tb.SelectMany(p => GetParent(db, p.tbYearParent.Id)));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDisable(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                //var tb = db.Set<Basis.Entity.tbYear>().Find(id);
                //tb.IsDisable = !tb.IsDisable;
                //if (db.SaveChanges() > 0)
                //{
                //    Sys.Controllers.SysUserLogController.Insert("修改了学年学段");
                //}

                #region  同步开启(子/父级状态)
                var tb = (from p in db.Table<Basis.Entity.tbYear>().Include(p => p.tbYearParent) where p.Id == id select p).FirstOrDefault();
                if (tb != null)
                {
                    tb.IsDisable = !tb.IsDisable;

                    //同步设置所有子级
                    if (tb.YearType != Code.EnumHelper.YearType.Section)
                    {
                        var childs = GetSon(db, tb.Id);
                        if (childs != null)
                        {
                            foreach (var item in childs)
                            {
                                item.IsDisable = tb.IsDisable;
                            }
                        }
                    }

                    //设置为启用时，同步设置所有上级
                    if (tb.YearType != Code.EnumHelper.YearType.Year && !tb.IsDisable)
                    {
                        var parents = GetParent(db, tb.tbYearParent.Id);
                        foreach (var item in parents)
                        {
                            item.IsDisable = tb.IsDisable;
                        }
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了学年学段");
                    }
                }
                #endregion

                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDefault(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbYear>().Include(p => p.tbYearParent.tbYearParent) where p.Id == id select p).FirstOrDefault();
                if (tb != null)
                {
                    var list = from p in db.Table<Basis.Entity.tbYear>()
                               select p;
                    foreach (var a in list)
                    {
                        a.IsDefault = false;
                    }

                    tb.IsDefault = true;

                    //设置为激活状态时，启用本身，上级(学段)、上上级(学年)的状态为开启
                    tb.IsDisable = false;
                    if (tb.tbYearParent != null)
                    {
                        tb.tbYearParent.IsDisable = false;
                    }
                    if (tb.tbYearParent.tbYearParent != null)
                    {
                        tb.tbYearParent.tbYearParent.IsDisable = false;
                    }

                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了学年学段");
                }

                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }

        /// <summary>
        /// 根据学段id获取学年Id
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        [NonAction]
        public static int GetDefaultYearId(XkSystem.Models.DbContext db)
        {
            return  (from p in db.Table<Entity.tbYear>() where p.IsDefault select p.tbYearParent.tbYearParent.Id).FirstOrDefault();
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(Code.EnumHelper.YearType yearType)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var defaultYearId = 0;
                switch (yearType)
                {
                    case Code.EnumHelper.YearType.Section:
                        break;
                    case Code.EnumHelper.YearType.Term:
                        defaultYearId = (from p in db.Table<Entity.tbYear>() where p.IsDefault select p.tbYearParent.Id).FirstOrDefault();
                        break;
                    default:
                        defaultYearId = (from p in db.Table<Entity.tbYear>() where p.IsDefault select p.tbYearParent.tbYearParent.Id).FirstOrDefault();
                        break;
                }
                return (from p in db.Table<Entity.tbYear>()
                        where p.YearType == yearType && !p.IsDisable
                        orderby p.No
                        select new SelectListItem()
                        {
                            Text = p.YearName,
                            Value = p.Id.ToString(),
                            Selected = defaultYearId > 0 ? p.Id == defaultYearId : p.IsDefault
                        }).ToList();
            }
        }

        [NonAction]
        public static Dto.Year.Info SelectDefaultInfo()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbYear>()
                          where p.IsDisable == false
                          orderby p.IsDefault descending, p.No
                          select new Dto.Year.Info
                          {
                              Id = p.Id,
                              YearName = p.YearName
                          }).FirstOrDefault();
                return tb;
            }
        }

        [NonAction]
        public static Dto.Year.Info SelectInfo(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbYear>()
                          where p.Id == id
                          orderby p.No
                          select new Dto.Year.Info
                          {
                              Id = p.Id,
                              YearName = p.YearName,
                              FromDate = p.FromDate,
                              ToDate = p.ToDate
                          }).FirstOrDefault();
                return tb;
            }
        }
    }
}