using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sys.Controllers
{
    public class SysUserLogController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysUserLog.List();
                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = Code.Common.DateMonthFirst.ToString(Code.Common.StringToDate);
                    vm.DateSearchTo = Code.Common.DateMonthLast.ToString(Code.Common.StringToDate);
                }

                var fromDate = DateTime.Parse(vm.DateSearchFrom);
                var toDate = DateTime.Parse(vm.DateSearchTo).AddDays(1);

                var tb = from p in db.Table<Sys.Entity.tbSysUserLog>()
                            .Include(d => d.tbSysUser)
                         where p.ActionDate >= fromDate
                            && p.ActionDate < toDate
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ActionContent.Contains(vm.SearchText) || d.tbSysUser.UserCode.Contains(vm.SearchText) || d.tbSysUser.UserName.Contains(vm.SearchText));
                }

                if (string.IsNullOrEmpty(vm.Page.PageOrderBy))
                {
                    //默认排序
                    tb = tb.OrderByDescending(d => d.ActionDate);
                }
                else
                {
                    tb = Code.LinqSort.OrderBy(tb, vm.Page.PageOrderBy + " " + vm.Page.PageOrderDesc);
                }

                vm.UserLogList = (from p in tb
                                  select p).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SysUserLog.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                dateSearchFrom = vm.DateSearchFrom,
                dateSearchTo = vm.DateSearchTo,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize,
                pageOrderBy = vm.Page.PageOrderBy,
                pageOrderDesc = vm.Page.PageOrderDesc
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysUserLog>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除用户日志");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Export(string fromDate, string toDate, string searchText = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                var fromTime = DateTime.Parse(fromDate);
                var toTime = DateTime.Parse(toDate).AddDays(1);
                var tb = (from p in db.Table<Sys.Entity.tbSysUserLog>()
                            .Include(d => d.tbSysUser)
                          where (p.ActionContent.Contains(searchText) || p.tbSysUser.UserCode.Contains(searchText) || p.tbSysUser.UserName.Contains(searchText))
                            && p.ActionDate >= fromTime
                            && p.ActionDate < toTime
                          orderby p.ActionDate descending
                          select p).ToList();
                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("操作内容"),
                        new System.Data.DataColumn("操作人员"),
                        new System.Data.DataColumn("操作Ip"),
                        new System.Data.DataColumn("操作时间")
                    });
                foreach (var a in tb)
                {
                    var dr = dt.NewRow();
                    dr["操作内容"] = a.ActionContent;
                    dr["操作人员"] = a.tbSysUser.UserName;
                    dr["操作Ip"] = a.ActionIp;
                    dr["操作时间"] = a.ActionDate.ToString(Code.Common.StringToDateTime);
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);

                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Clear()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Sys.Entity.tbSysUserLog>();
                db.Set<Sys.Entity.tbSysUserLog>().RemoveRange(tb);

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("清空了用户日志");
                }

                return Code.MvcHelper.Post(null, "List");
            }
        }

        public static string Insert(string content, int userId = 0, int schoolId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = new Sys.Entity.tbSysUserLog();
                if (schoolId == 0)
                {
                    tb.tbTenant = db.Set<Admin.Entity.tbTenant>().Find(Code.Common.TenantId);
                }
                else
                {
                    tb.tbTenant = db.Set<Admin.Entity.tbTenant>().Find(schoolId);
                }
                if (userId == 0)
                {
                    tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                }
                else
                {
                    tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(userId);
                }
                tb.ActionContent = content;
                tb.ActionDate = DateTime.Now;
                tb.ActionIp = Code.Common.GetIp();
                db.Set<Sys.Entity.tbSysUserLog>().Add(tb);
                db.SaveChanges();
            }

            return string.Empty;
        }
    }
}