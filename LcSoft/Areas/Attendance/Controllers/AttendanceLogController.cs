using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Attendance.Controllers
{
    public class AttendanceLogController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.AttendanceLog.List();

                var tb = from p in db.Table<Attendance.Entity.tbAttendanceLog>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.CardNumber.Contains(vm.SearchText));
                }
                if (string.IsNullOrWhiteSpace(vm.FromTime))
                {
                    vm.FromTime = Code.DateHelper.MonthFirstDay.ToString(Code.Common.StringToDate);
                }
                if (string.IsNullOrWhiteSpace(vm.ToTime))
                {
                    vm.ToTime = Code.DateHelper.MonthLastDay.ToString(Code.Common.StringToDate);
                }
                var fromTime = vm.FromTime.ConvertToDateTime();
                var toTime = vm.ToTime.ConvertToDateTime();
                tb = tb.Where(d => d.AttendanceDate > fromTime && d.AttendanceDate < toTime);

                vm.DataList = (from p in tb
                               orderby p.No
                               select p).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.AttendanceLog.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                FromTime = vm.FromTime,
                ToTime = vm.ToTime,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult Export()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.AttendanceLog.Export();
                var tb = db.Table<Attendance.Entity.tbAttendanceLog>();

                if (string.IsNullOrWhiteSpace(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.CardNumber.Contains(vm.SearchText));
                }
                tb = tb.Where(d => d.AttendanceDate > vm.FromTime && d.AttendanceDate < vm.ToTime);

                vm.DataList = (from p in tb
                               orderby p.No
                               select new Dto.AttendanceLog.Export()
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   AttendanceDate = p.AttendanceDate,
                                   CardNumber = p.CardNumber,
                                   MachineCode = p.MachineCode,
                                   Status = p.Status,
                                   StatusName = p.Status ? "已处理" : "未处理"
                               }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("排序"),
                        new System.Data.DataColumn("考勤机编号"),
                        new System.Data.DataColumn("考勤时间"),
                        new System.Data.DataColumn("卡号"),
                        new System.Data.DataColumn("处理状态")
                    });
                foreach (var a in vm.DataList)
                {
                    var dr = dt.NewRow();
                    dr["排序"] = a.No;
                    dr["考勤机编号"] = a.MachineCode;
                    dr["考勤时间"] = a.AttendanceDate;
                    dr["卡号"] = a.CardNumber;
                    dr["处理状态"] = a.StatusName;
                    dt.Rows.Add(dr);
                }

                var file = System.IO.Path.GetTempFileName();
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
    }
}