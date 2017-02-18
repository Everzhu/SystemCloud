using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sms.Controllers
{
    public class SmsController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Sms.List();

                var tb = from p in db.Table<Entity.tbSms>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.SmsTitle.Contains(vm.SearchText));
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

                vm.SmsList = (from p in tb
                              where p.PlanDate >= fromDate && p.PlanDate < toDate
                              orderby p.PlanDate, p.InputDate
                              select new Dto.Sms.List
                              {
                                  Id = p.Id,
                                  SmsTitle = p.SmsTitle,
                                  PlanDate = p.PlanDate,
                                  InputDate = p.InputDate,
                                  SysUserName = p.tbSysUser.UserName
                              }).ToPageList(vm.Page);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Sms.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                dateSearchFrom = vm.DateSearchFrom,
                dateSearchTo = vm.DateSearchTo,
                pageSize = vm.Page.PageSize,
                pageCount = vm.Page.PageCount,
                pageIndex = vm.Page.PageIndex
            }));
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Sms.Edit();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbSms>()
                              where p.Id == id
                              select new Dto.Sms.Edit
                              {
                                  Id = p.Id,
                                  SmsTitle = p.SmsTitle,
                                  PlanDate = p.PlanDate,
                                  SysUserId = p.tbSysUser.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.SmsEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Models.Sms.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.SmsEdit.Id == 0)
                    {
                        var tb = new Entity.tbSms();
                        tb.InputDate = DateTime.Now;
                        tb.PlanDate = vm.SmsEdit.PlanDate;
                        tb.SmsTitle = vm.SmsEdit.SmsTitle;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Entity.tbSms>().Add(tb);
                        //循环电话
                        var mobileArr = vm.SmsEdit.Mobile.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var mobile in mobileArr)
                        {
                            var tbSmsTo = new Entity.tbSmsTo();
                            tbSmsTo.Mobile = mobile;
                            tbSmsTo.Retry = Convert.ToInt32(decimal.Zero);
                            tbSmsTo.Status = decimal.Zero;
                            tbSmsTo.SendDate = vm.SmsEdit.PlanDate;
                            tbSmsTo.tbSms = tb;
                            db.Set<Entity.tbSmsTo>().Add(tbSmsTo);
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了短信记录");
                        }
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了短信");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Entity.tbSms>()
                                  where p.Id == vm.SmsEdit.Id
                                  select p).FirstOrDefault();

                        if (tb != null)
                        {
                            tb.PlanDate = vm.SmsEdit.PlanDate;
                            tb.SmsTitle = vm.SmsEdit.SmsTitle;
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了短信");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }
                var vmList = new Models.Sms.List();
                return Code.MvcHelper.Post(error, Url.Action("List", new
                {
                    searchText = vmList.SearchText,
                    dateSearchFrom = vmList.DateSearchFrom,
                    dateSearchTo = vmList.DateSearchTo,
                    pageIndex = vmList.Page.PageIndex,
                    pageSize = vmList.Page.PageSize
                }));
            }
        }

        [NonAction]
        public static bool SendCheckCode(Models.Sms.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var smsId = 0;
                    if (vm.SmsEdit.Id == 0)
                    {
                        var tb = new Entity.tbSms();
                        tb.InputDate = DateTime.Now;
                        tb.PlanDate = vm.SmsEdit.PlanDate;
                        tb.SmsTitle = vm.SmsEdit.SmsTitle;
                        tb.SmsPIN = vm.SmsEdit.SmsPIN;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Entity.tbSms>().Add(tb);
                        //循环电话
                        var mobileArr = vm.SmsEdit.Mobile.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var mobile in mobileArr)
                        {
                            var tbSmsTo = new Entity.tbSmsTo();
                            tbSmsTo.Mobile = mobile;
                            tbSmsTo.Retry = Convert.ToInt32(decimal.Zero);
                            tbSmsTo.Status = decimal.One;
                            tbSmsTo.SendDate = vm.SmsEdit.PlanDate;
                            tbSmsTo.tbSms = tb;
                            db.Set<Entity.tbSmsTo>().Add(tbSmsTo);
                        }
                        if (db.SaveChanges() > 0)
                        {
                            smsId = tb.Id;
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了密码找回验证码短信");
                            //单独执行发短信的方法
                            return SmsSendController.SmsSendCheckCode(smsId);
                        }
                        return false;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public ActionResult EditSms(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Sms.EditSms();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbSms>()
                              where p.Id == id
                              select new Dto.Sms.EditSms
                              {
                                  Id = p.Id,
                                  SmsTitle = p.SmsTitle,
                                  PlanDate = p.PlanDate,
                                  SysUserId = p.tbSysUser.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.SmsEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditSms(Models.Sms.EditSms vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.SmsEdit.Id > 0)
                    {
                        var tb = (from p in db.Table<Entity.tbSms>()
                                  where p.Id == vm.SmsEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.InputDate = DateTime.Now;
                            tb.PlanDate = vm.SmsEdit.PlanDate;
                            tb.SmsTitle = vm.SmsEdit.SmsTitle;
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了短信");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }
                var vmList = new Models.Sms.List();
                return Code.MvcHelper.Post(error, Url.Action("List", new
                {
                    searchText = vmList.SearchText,
                    dateSearchFrom = vmList.DateSearchFrom,
                    dateSearchTo = vmList.DateSearchTo,
                    pageIndex = vmList.Page.PageIndex,
                    pageSize = vmList.Page.PageSize
                }));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSms>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                var tbSmsTo = (from p in db.Table<Entity.tbSmsTo>()
                               where ids.Contains(p.tbSms.Id)
                               select p).ToList();

                foreach (var a in tbSmsTo)
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

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Sms/Views/Sms/SmsTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        public ActionResult ImportSmsJson()
        {
            var vm = new Models.Sms.Import();
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                if (ModelState.IsValid)
                {

                    var file = Request.Files[0];
                    var fileSave = System.IO.Path.GetTempFileName();
                    file.SaveAs(fileSave);
                    using (var db = new XkSystem.Models.DbContext())
                    {
                        if (Code.Common.GetFileType(file.FileName) != Code.FileType.Excel)
                        {
                            error.AddError("上传的文件不是正确的EXCLE文件!");
                        }
                        var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), string.Empty);

                        if (dt == null)
                        {
                            error.AddError("无法读取上传的文件，请检查文件格式是否正确!");
                        }
                        var tbList = new List<string>() { "手机号", "短信内容", "计划发送时间" };
                        var Text = string.Empty;
                        foreach (var a in tbList)
                        {
                            if (!dt.Columns.Contains(a.ToString()))
                            {
                                Text += a + ",";
                            }
                        }
                        if (!string.IsNullOrEmpty(Text))
                        {
                            error.AddError("上传的EXCEL内容与预期不一致!缺少字段:" + Text);
                        }
                        var ImportList = new List<Dto.Sms.Import>();
                        //执行导入
                        var index = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            index++;
                            var import = new Dto.Sms.Import();
                            import.Mobile = dr["手机号"].ToString();
                            if (string.IsNullOrEmpty(import.Mobile) || import.Mobile.Length != 11)
                            {
                                string msgError = string.Format("第{0}行数据手机号({1})格式不正确;", index.ToString(), import.Mobile);
                                error.AddError(msgError);
                                continue;
                            }
                            import.SmsTitle = dr["短信内容"].ToString();
                            if (string.IsNullOrEmpty(import.SmsTitle))
                            {
                                string msgError = string.Format("第{0}行数据短信内容不能为空;", index.ToString(), import.SmsTitle);
                                error.AddError(msgError);
                                continue;
                            }

                            if (!string.IsNullOrEmpty(dr["计划发送时间"].ToString()))
                            {
                                DateTime timeTemp = new DateTime();
                                if (!DateTime.TryParse(dr["计划发送时间"].ToString().Trim(), out timeTemp))
                                {
                                    string msgError = string.Format("第{0}行数据计划发送时间({1})格式不正确,(例如:{2});", index.ToString(), dr["计划发送时间"].ToString(), DateTime.Now.ToString(XkSystem.Code.Common.StringToDate));
                                    error.AddError(msgError);
                                    continue;
                                }
                                import.PlanDate = Convert.ToDateTime(dr["计划发送时间"]).ToString(XkSystem.Code.Common.StringToDateTime);
                            }
                            else
                            {
                                import.PlanDate = DateTime.Now.ToString(XkSystem.Code.Common.StringToDateTime);
                            }
                            vm.SmsImportList.Add(import);
                        }
                    }
                }
            }
            var dynamic = new { Status = decimal.Zero, Message = vm.SmsImportList };
            if (error != null && error.Count > decimal.Zero)
            {
                return Json(new { Status = decimal.One, Message = error }, JsonRequestBehavior.AllowGet);
            }
            return Json(dynamic, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ImportSendSms(List<string> strJson, int ImportType = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dto.Sms.Import>>(strJson[0]);
                //发短信代码
                var listSms = new List<Entity.tbSms>();
                if (ImportType == decimal.Zero)//模式1：短信内容一致，手机号不一样
                {
                    //短信详情
                    var tb = new Entity.tbSms();
                    tb.InputDate = DateTime.Now;
                    tb.PlanDate = list.FirstOrDefault().PlanDate.ConvertToDateTime();
                    tb.SmsTitle = list.FirstOrDefault().SmsTitle;
                    tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                    db.Set<Entity.tbSms>().Add(tb);
                    listSms.Add(tb);

                    foreach (var sms in list)
                    {
                        //发送信息
                        var tbSmsTo = new Entity.tbSmsTo();
                        tbSmsTo.Mobile = sms.Mobile;
                        tbSmsTo.Retry = Convert.ToInt32(decimal.Zero);
                        tbSmsTo.SendDate = list.FirstOrDefault().PlanDate.ConvertToDateTime();
                        tbSmsTo.Status = decimal.Zero;
                        tbSmsTo.tbSms = tb;
                        db.Set<Entity.tbSmsTo>().Add(tbSmsTo);
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入了短信手机信息");
                    }
                }
                else//模式2：短信内容不一致，手机号不一样
                {
                    foreach (var sms in list)
                    {
                        //短信详情
                        var tb = new Entity.tbSms();
                        tb.InputDate = DateTime.Now;
                        tb.PlanDate = sms.PlanDate.ConvertToDateTime();
                        tb.SmsTitle = sms.SmsTitle;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Entity.tbSms>().Add(tb);
                        listSms.Add(tb);
                        //发送信息
                        var tbSmsTo = new Entity.tbSmsTo();
                        tbSmsTo.Mobile = sms.Mobile;
                        tbSmsTo.Retry = Convert.ToInt32(decimal.Zero);
                        tbSmsTo.SendDate = sms.PlanDate.ConvertToDateTime();
                        tbSmsTo.Status = decimal.Zero;
                        tbSmsTo.tbSms = tb;
                        db.Set<Entity.tbSmsTo>().Add(tbSmsTo);
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入了短信记录");
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入了短信");
                }
                return Json(new { Status = decimal.Zero, Message = "" });
            }
        }

        public ActionResult Import(bool status = true, List<string> error = null)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Sms.Import();
                if (error != null && error.Count > decimal.Zero)
                {
                    foreach (var msg in error)
                    {
                        ModelState.AddModelError("", msg);
                    }
                    status = false;
                }
                return View(vm);
            }
        }

        public ActionResult SelectUserSend(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Sms.SelectUserSend();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbSms>()
                              where p.Id == id
                              select new Dto.Sms.SelectUserSend
                              {
                                  Id = p.Id,
                                  SmsTitle = p.SmsTitle,
                                  PlanDate = p.PlanDate,
                                  SysUserId = p.tbSysUser.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.SelectUserSendEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SelectUserSend(Models.Sms.SelectUserSend vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.SelectUserSendEdit.Id == 0)
                    {
                        var tb = new Entity.tbSms();
                        tb.InputDate = DateTime.Now;
                        tb.PlanDate = vm.SelectUserSendEdit.PlanDate;
                        tb.SmsTitle = vm.SelectUserSendEdit.SmsTitle;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Entity.tbSms>().Add(tb);
                        //循环电话
                        var userIds = vm.SelectUserSendEdit.Mobile.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (userIds.Count() == decimal.Zero)
                        {
                            var errorMsg = new { Status = decimal.Zero, Message = "请选择需要接受短信的人员" };
                            return Json(errorMsg);
                        }
                        var tbSysUserList = (from p in db.Table<Sys.Entity.tbSysUser>()
                                             where userIds.Contains(p.Id.ToString())
                                             select p).ToList();
                        var addEntitySmsToList = new List<Entity.tbSmsTo>();
                        foreach (var user in tbSysUserList)
                        {
                            var tbSmsTo = new Entity.tbSmsTo();
                            tbSmsTo.Mobile = user.Mobile;
                            tbSmsTo.tbSysUser = user;
                            tbSmsTo.Retry = Convert.ToInt32(decimal.Zero);
                            tbSmsTo.SendDate = vm.SelectUserSendEdit.PlanDate;
                            tbSmsTo.Status = decimal.Zero;
                            tbSmsTo.tbSms = tb;
                            addEntitySmsToList.Add(tbSmsTo);
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了短信手机信息");
                        }
                        db.Set<Entity.tbSmsTo>().AddRange(addEntitySmsToList);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了短信");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Entity.tbSms>()
                                  where p.Id == vm.SelectUserSendEdit.Id
                                  select p).FirstOrDefault();

                        if (tb != null)
                        {
                            tb.InputDate = DateTime.Now;
                            tb.PlanDate = vm.SelectUserSendEdit.PlanDate;
                            tb.SmsTitle = vm.SelectUserSendEdit.SmsTitle;
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了短信");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }
                var vmList = new Models.Sms.List();
                return Code.MvcHelper.Post(error, Url.Action("List", new
                {
                    searchText = vmList.SearchText,
                    dateSearchFrom = vmList.DateSearchFrom,
                    dateSearchTo = vmList.DateSearchTo,
                    pageIndex = vmList.Page.PageIndex,
                    pageSize = vmList.Page.PageSize
                }));
            }
        }

        public ActionResult SelectUser()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Sms.SelectUser();
                if (Request["UserType"] != null)
                {
                    Code.EnumHelper.SysUserType userType;
                    Enum.TryParse(Request["UserType"], out userType);
                    vm.UserType = userType;
                }

                var tb = from p in db.Table<Sys.Entity.tbSysUser>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.UserCode.Contains(vm.SearchText) || d.UserName.Contains(vm.SearchText) || d.Mobile.Contains(vm.SearchText));
                }

                vm.SelectUserList = (from p in tb
                                     where p.UserType == vm.UserType
                                     orderby p.UserCode
                                     select new Dto.Sms.SelectUser
                                     {
                                         Id = p.Id,
                                         UserCode = p.UserCode,
                                         UserName = p.UserName,
                                         SexName = p.tbSex.SexName,
                                         Mobile = p.Mobile,
                                         UserType = p.UserType
                                     }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectUser(Models.Sms.SelectUser vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SelectUser",
                new
                {
                    searchText = vm.SearchText,
                    userType = vm.UserType,
                    pageIndex = vm.Page.PageIndex,
                    pageSize = vm.Page.PageSize
                }));
        }

        public ActionResult SelectUserJson(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Sms.SelectUser();
                var tb = (from p in db.Table<Sys.Entity.tbSysUser>()
                          where ids.Contains(p.Id)
                          select new Dto.Sms.SelectUser
                          {
                              Id = p.Id,
                              UserCode = p.UserCode,
                              UserName = p.UserName,
                              SexName = p.tbSex.SexName,
                              Mobile = p.Mobile,
                              UserType = p.UserType
                          }).ToList();
                return Json(tb, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
