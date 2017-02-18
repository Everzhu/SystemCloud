using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sms.Controllers
{
    public class SmsToController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SmsTo.List();

                vm.SendStatus = typeof(Code.EnumHelper.SmsSendStatus).ToItemList();
                vm.SendStatus.Insert(0, new SelectListItem { Text = "==状态==", Value = "-999999" });

                var tb = from p in db.Table<Entity.tbSmsTo>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.Mobile.Contains(vm.SearchText));
                }

                if (vm.StatusId != -999999)
                {
                    tb = tb.Where(d => d.Status == vm.StatusId);
                }

                //开始日期
                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = DateTime.Now.AddDays(-7).ToString(XkSystem.Code.Common.StringToDate);
                }
                //结束日期
                if (string.IsNullOrEmpty(vm.DateSearchTo))
                {
                    vm.DateSearchTo = DateTime.Now.ToString(XkSystem.Code.Common.StringToDate);
                }

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);
                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                vm.SmsToList = (from p in tb
                                join m in db.Table<Entity.tbSms>() on p.tbSms.Id equals m.Id into n
                                from k in n.DefaultIfEmpty()
                                where p.SendDate >= fromDate && p.SendDate < toDate
                                orderby p.SendDate descending
                                select new Dto.SmsTo.List
                                {
                                    Id = p.Id,
                                    Mobile = p.Mobile,
                                    No = p.No,
                                    Remark = p.Remark,
                                    Retry = p.Retry,
                                    SmsId = p.tbSms.Id,
                                    SmsTitle = k == null ? "" : k.SmsTitle,
                                    Status = p.Status,
                                    SendDate = p.SendDate,
                                    SysUserName = p.tbSysUser.UserName
                                }).ToPageList(vm.Page);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SmsTo.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                dateSearchFrom = vm.DateSearchFrom,
                dateSearchTo = vm.DateSearchTo,
                statusId = vm.StatusId,
                pageSize = vm.Page.PageSize,
                pageCount = vm.Page.PageCount,
                pageIndex = vm.Page.PageIndex
            }));
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SmsTo.Edit();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbSmsTo>()
                              where p.Id == id
                              select new Dto.SmsTo.Edit
                              {
                                  Id = p.Id,
                                  Mobile = p.Mobile,
                                  No = p.No,
                                  Remark = p.Remark,
                                  Retry = p.Retry,
                                  SmsId = p.tbSms.Id,
                                  Status = p.Status,
                                  SysUserId = p.tbSysUser.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.SmsToEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Models.SmsTo.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.SmsToEdit.Id == 0)
                    {
                        var tb = new Entity.tbSmsTo();
                        tb.Mobile = vm.SmsToEdit.Mobile;
                        tb.Remark = vm.SmsToEdit.Remark;
                        tb.Retry = vm.SmsToEdit.Retry;
                        tb.Status = vm.SmsToEdit.Status;
                        tb.tbSms = db.Set<Entity.tbSms>().Find(vm.SmsToEdit.SmsId);
                        db.Set<Entity.tbSmsTo>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了短信接收人");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Entity.tbSmsTo>()
                                  where p.Id == vm.SmsToEdit.Id
                                  select p).FirstOrDefault();

                        if (tb != null)
                        {
                            tb.Mobile = vm.SmsToEdit.Mobile;
                            tb.Remark = vm.SmsToEdit.Remark;
                            tb.Retry = vm.SmsToEdit.Retry;
                            tb.Status = vm.SmsToEdit.Status;
                            tb.tbSms = db.Set<Entity.tbSms>().Find(vm.SmsToEdit.SmsId);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了短信接收人");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }
                return Code.MvcHelper.Post(error, Url.Action("List"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSmsTo>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除短信");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Retry(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var tb = (from p in db.Table<Entity.tbSmsTo>()
                              where ids.Contains(p.Id)
                              select p).ToList();

                    if (tb != null)
                    {
                        foreach (var smsTo in tb)
                        {
                            smsTo.Retry = smsTo.Retry + Convert.ToInt32(decimal.One);
                            smsTo.Status = decimal.One;
                        }

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("短信重新发送");
                        }
                    }
                    else
                    {
                        error.AddError(Resources.LocalizedText.MsgNotFound);
                    }
                }
                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Export()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                var vm = new Models.SmsTo.List();
                var tb = from p in db.Table<Entity.tbSmsTo>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.Mobile.Contains(vm.SearchText));
                }

                if (vm.StatusId != -999999)
                {
                    tb = tb.Where(d => d.Status == vm.StatusId);
                }

                //开始日期
                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = DateTime.Now.AddDays(-7).ToString(XkSystem.Code.Common.StringToDate);
                }
                //结束日期
                if (string.IsNullOrEmpty(vm.DateSearchTo))
                {
                    vm.DateSearchTo = DateTime.Now.ToString(XkSystem.Code.Common.StringToDate);
                }

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);
                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                vm.SmsToList = (from p in tb
                                join m in db.Table<Entity.tbSms>() on p.tbSms.Id equals m.Id into n
                                from k in n.DefaultIfEmpty()
                                where p.SendDate >= fromDate && p.SendDate < toDate
                                orderby p.SendDate descending
                                select new Dto.SmsTo.List
                                {
                                    Id = p.Id,
                                    Mobile = p.Mobile,
                                    No = p.No,
                                    Remark = p.Remark,
                                    Retry = p.Retry,
                                    SmsId = p.tbSms.Id,
                                    SmsTitle = k == null ? "" : k.SmsTitle,
                                    Status = p.Status,
                                    SendDate = p.SendDate,
                                    SysUserName = p.tbSysUser.UserName
                                }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("序号"),
                        new System.Data.DataColumn("手机号"),
                        new System.Data.DataColumn("发送时间"),
                        new System.Data.DataColumn("状态"),
                        new System.Data.DataColumn("重试次数"),
                        new System.Data.DataColumn("接收人员"),
                        new System.Data.DataColumn("短信内容"),
                        new System.Data.DataColumn("备注")
                    });
                var index = 0;
                foreach (var a in vm.SmsToList)
                {
                    index++;
                    var dr = dt.NewRow();
                    dr["序号"] = index;
                    dr["手机号"] = a.Mobile;
                    dr["接收人员"] = a.SysUserName;
                    dr["发送时间"] = a.SendDate.ToString(XkSystem.Code.Common.StringToDateTime);
                    if (a.Status == decimal.Zero)
                    {
                        dr["状态"] = "未发送";
                    }
                    else if (a.Status == decimal.One)
                    {
                        dr["状态"] = "发送成功";
                    }
                    else if (a.Status == -decimal.One)
                    {
                        dr["状态"] = "失败";
                    }
                    dr["重试次数"] = a.Retry;                    
                    dr["短信内容"] = a.SmsTitle;
                    dr["备注"] = a.Remark;
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

        public ActionResult Detail()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SmsTo.Detail();

                var tb = from p in db.Table<Entity.tbSmsTo>()
                         where p.tbSms.Id == vm.SmsId
                         select p;

                vm.SmsToDetailList = (from p in tb
                                      orderby p.SendDate descending
                                      select new Dto.SmsTo.Detail
                                      {
                                          Id = p.Id,
                                          Mobile = p.Mobile,
                                          No = p.No,
                                          Remark = p.Remark,
                                          Retry = p.Retry,
                                          SmsId = p.tbSms.Id,
                                          SmsTitle = p.tbSms.SmsTitle,
                                          Status = p.Status,
                                          SendDate = p.SendDate,
                                          SysUserName = p.tbSysUser.UserName
                                      }).ToList();

                return View(vm);
            }
        }
    }
}