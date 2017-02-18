using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;
//using XkSystem.Areas.Open.Controllers;

namespace XkSystem.Areas.Wechat.Controllers
{
    public class OpenController : Controller
    {
        #region Wechat
        /// <summary>
        /// Wechat
        /// </summary>
        /// <returns></returns>
        //public ActionResult Search()
        //{
        //    using (var db = new XkSystem.Models.DbContext())
        //    {
        //        var vm = new XkSystem.Areas.Open.Models.Open.List();
        //        var rooms = (from p in db.Table<Basis.Entity.tbRoom>()
        //                     select new Code.MuiJsonDataBind
        //                     {
        //                         text = p.RoomName,
        //                         value = p.Id.ToString(),
        //                     }).ToList();
        //        vm.RoomListJson = JsonConvert.SerializeObject(rooms);
        //        return View("m_Search", vm);
        //    }
        //}

        /// <summary>
        /// Wechat
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Search(XkSystem.Areas.Open.Models.Open.List vm)
        //{
        //    return Code.MvcHelper.Post(null, Url.Action("MList", new
        //    {
        //        OpenName = vm.OpenName,
        //        TeacherName = vm.TeacherName,
        //        RoomId = vm.RoomId,
        //        ScheduleDate = vm.ScheduleDate,
        //        PeriodNames = vm.PeriodNames,
        //        pageIndex = vm.Page.PageIndex,
        //        pageSize = vm.Page.PageSize
        //    }));
        //}

        /// <summary>
        /// Wechat
        /// </summary>
        /// <returns></returns>
        //public ActionResult MList(Open.Models.Open.List vm)
        //{
        //    using (var db = new XkSystem.Models.DbContext())
        //    {
        //        var tb = (from p in db.Table<Open.Entity.tbOpen>()
        //                 .Where(d => d.CheckStatus == Code.EnumHelper.CheckStatus.Success)
        //                  select p).Include(r => r.tbRoom).Include(r => r.tbTeacher).Include(r => r.tbTeacher.tbSysUser);
        //        if (string.IsNullOrEmpty(vm.OpenName) == false)
        //        {
        //            tb = tb.Where(d => d.OpenName.Contains(vm.OpenName));
        //        }
        //        if (string.IsNullOrEmpty(vm.TeacherName) == false)
        //        {
        //            tb = tb.Where(d => d.tbTeacher.TeacherName.Contains(vm.TeacherName));
        //        }
        //        if (string.IsNullOrEmpty(vm.RoomId) == false)
        //        {
        //            tb = tb.Where(d => d.tbRoom.Id.ToString().Equals(vm.RoomId));
        //        }
        //        if (!string.IsNullOrEmpty(vm.ScheduleDate))
        //        {
        //            var ScheduleDate = Convert.ToDateTime(vm.ScheduleDate);
        //            tb = tb.Where(d => d.ScheduleDate >= ScheduleDate);
        //        }
        //        else
        //        {
        //            var ScheduleDate = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd"));
        //            tb = tb.Where(d => d.ScheduleDate >= ScheduleDate);
        //        }

        //        vm.OpenList = (from p in tb
        //                            select new XkSystem.Areas.Open.Dto.Open.List
        //                            {
        //                                Id = p.Id,
        //                                OpenName = p.OpenName,
        //                                CheckStatusT = p.CheckStatus,
        //                                RoomName = p.tbRoom.RoomName,
        //                                ScheduleDate = p.ScheduleDate,
        //                                TeacherName = p.tbTeacher.TeacherName,
        //                                UserName = p.tbSysUser.UserName,
        //                                Count = p.Count,
        //                                UserId = p.tbTeacher.tbSysUser.Id,
        //                                InputTime = p.InputDate
        //                            }).OrderByDescending(d => d.InputTime).ToPageList(vm.Page);

        //        var tbOpenUserList = (from p in db.Table<Open.Entity.tbOpenUser>()
        //                                      select p).Include(o => o.tbOpen).Include(o => o.tbSysUser).ToList();

        //        var tbPeriodList = (from p in db.Table<Basis.Entity.tbPeriod>().Where(d => d.IsDeleted == false && d.PeriodName != "午")
        //                            select p).ToList();

        //        var tbOpenIds = vm.OpenList.Select(d => d.Id).ToList();
        //        var tbOpenPeriodList = OpenPeriodController.SelectOpenPeriod(tbOpenIds);

        //        foreach (var item in vm.OpenList)
        //        {
        //            string PerNames = string.Empty;
        //            foreach (var itemp in tbOpenPeriodList.Where(d => d.OpenId == item.Id))
        //            {
        //                if (!string.IsNullOrEmpty(itemp.PeriodId.ToString()))
        //                {
        //                    var premodel = tbPeriodList.Where(d => d.Id == itemp.PeriodId).FirstOrDefault();
        //                    if (premodel != null)
        //                    {
        //                        PerNames += "第" + premodel.PeriodName + "节,";
        //                    }
        //                }
        //            }
        //            item.PeriodNames = PerNames.TrimEnd(',');

        //            if (item.UserId == Code.Common.UserId)
        //            {
        //                item.ReserveStauts = 3;
        //            }
        //            else
        //            {
        //                var tbOpenUserModel = tbOpenUserList.Where(d => d.tbOpen.Id == item.Id && d.tbSysUser.Id == Code.Common.UserId).FirstOrDefault();
        //                if (tbOpenUserModel != null)
        //                {
        //                    item.ReserveStauts = 1;
        //                }
        //                else
        //                {
        //                    var YuYueCout = tbOpenUserList.Where(d => d.tbOpen.Id == item.Id).Count();
        //                    var SurplusCount = item.Count - YuYueCout;
        //                    if (SurplusCount > 0)
        //                    {
        //                        item.ReserveStauts = 0;
        //                    }
        //                    else
        //                    {
        //                        item.ReserveStauts = 2;
        //                    }

        //                }
        //            }

        //        }

        //        vm.Page.PageCount = (int)Math.Ceiling(vm.Page.TotalCount * 1.0 / vm.Page.PageSize);
        //        vm.Page.PageCount = vm.Page.PageCount == 0 ? 1 : vm.Page.PageCount;
        //        if (Request.IsAjaxRequest())
        //        {
        //            return PartialView("KList", vm);
        //        }
        //        return View("m_List", vm);
        //    }
        //}


        //[HttpPost]
        //public ActionResult ReserveOpen(int ClassId, int Count)
        //{
        //    var error = new List<string>();
        //    using (var db = new XkSystem.Models.DbContext())
        //    {
        //        var tbOpenUserList = (from p in db.Table<Open.Entity.tbOpenUser>()
        //                                      where p.tbOpen.Id == ClassId
        //                                      select p).ToList();

        //        if ((Count - tbOpenUserList.Count) > 0)
        //        {
        //            var tb = (from p in db.Table<Open.Entity.tbOpenUser>() where p.tbOpen.Id == ClassId && p.tbSysUser.Id == Code.Common.UserId select p).FirstOrDefault();
        //            if (tb == null)
        //            {
        //                var OpenUser = new Open.Entity.tbOpenUser();
        //                OpenUser.tbOpen = db.Set<Open.Entity.tbOpen>().Find(ClassId);
        //                OpenUser.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
        //                OpenUser.ReserveDate = DateTime.Now;
        //                OpenUser.MarkDate = DateTime.Now;
        //                OpenUser.tbTenant = db.Set<Admin.Entity.tbTenant>().Find(Code.Common.TenantId);
        //                db.Set<Open.Entity.tbOpenUser>().Add(OpenUser);
        //                db.SaveChanges();
        //            }
        //            else
        //            {
        //                error.AddError("你已预约了!");
        //            }
        //        }
        //        else
        //        {
        //            error.AddError("预约已满!");
        //        }
        //    }
        //    return Code.MvcHelper.Post(error);
        //}
        #endregion
    }
}