using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sms.Controllers
{
    public class SmsConfigController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SmsConfig.List();

                var tb = from p in db.Table<Entity.tbSmsConfig>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.SmsServer.Contains(vm.SearchText));
                }

                vm.SmsConfigList = (from p in tb
                                    orderby p.No
                                    select new Dto.SmsConfig.List
                                    {
                                        Id = p.Id,
                                        No = p.No,
                                        SmsServer = p.SmsServer,
                                        SmsAccount = p.SmsAccount,
                                        SmsPassword = p.SmsPassword,
                                        SmsUrl = p.SmsUrl,
                                        SmsServerType = p.SmsServerType,
                                        SmsFreeSignName = p.SmsFreeSignName,
                                        SmsTemplateCode = p.SmsTemplateCode,
                                        Status = p.Status,
                                        IsDefault = p.IsDefault
                                    }).ToPageList(vm.Page);

                foreach (var a in vm.SmsConfigList)
                {
                    a.SmsAccount = Code.Common.DESDeCode(a.SmsAccount);
                    a.SmsPassword = Code.Common.DESDeCode(a.SmsPassword);
                    a.SmsUrl = Code.Common.DESDeCode(a.SmsUrl);
                    a.SmsFreeSignName = Code.Common.DESDeCode(a.SmsFreeSignName);
                    a.SmsTemplateCode = Code.Common.DESDeCode(a.SmsTemplateCode);
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SmsConfig.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                pageSize = vm.Page.PageSize,
                pageCount = vm.Page.PageCount,
                pageIndex = vm.Page.PageIndex
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSmsConfig>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了短信服务配置");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SmsConfig.Edit();
                vm.SmsServerTypeList = typeof(XkSystem.Code.EnumHelper.SmsServerType).ToItemList();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbSmsConfig>()
                              where p.Id == id
                              select new Dto.SmsConfig.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  SmsServer = p.SmsServer,
                                  SmsAccount = p.SmsAccount,
                                  SmsPassword = p.SmsPassword,
                                  SmsUrl = p.SmsUrl,
                                  SmsServerType = p.SmsServerType,
                                  SmsFreeSignName = p.SmsFreeSignName,
                                  SmsTemplateCode = p.SmsTemplateCode,
                                  Status = p.Status,
                                  IsDefault = p.IsDefault
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.SmsConfigEdit = tb;
                        vm.SmsConfigEdit.SmsAccount = Code.Common.DESDeCode(vm.SmsConfigEdit.SmsAccount);
                        vm.SmsConfigEdit.SmsPassword = Code.Common.DESDeCode(vm.SmsConfigEdit.SmsPassword);
                        vm.SmsConfigEdit.SmsUrl = Code.Common.DESDeCode(vm.SmsConfigEdit.SmsUrl);
                        vm.SmsConfigEdit.SmsFreeSignName = Code.Common.DESDeCode(vm.SmsConfigEdit.SmsFreeSignName);
                        vm.SmsConfigEdit.SmsTemplateCode = Code.Common.DESDeCode(vm.SmsConfigEdit.SmsTemplateCode);
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.SmsConfig.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Entity.tbSmsConfig>().Where(d => d.SmsServer == vm.SmsConfigEdit.SmsServer && d.Id != vm.SmsConfigEdit.Id).Any())
                    {
                        error.AddError("该短信配置已经存在!");
                        return Code.MvcHelper.Post(error);
                    }

                    if (vm.SmsConfigEdit.IsDefault)
                    {
                        var tb = from p in db.Table<Entity.tbSmsConfig>()
                                 select p;

                        foreach (var section in tb)
                        {
                            section.IsDefault = false;
                        }

                        db.SaveChanges();
                    }

                    if (vm.SmsConfigEdit.Id == 0)
                    {
                        var tb = new Entity.tbSmsConfig();
                        tb.No = vm.SmsConfigEdit.No == null ? db.Table<Entity.tbSmsConfig>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SmsConfigEdit.No;
                        tb.SmsServer = vm.SmsConfigEdit.SmsServer;
                        tb.SmsServerType = vm.SmsConfigEdit.SmsServerType;
                        tb.SmsAccount = Code.Common.DESEnCode(vm.SmsConfigEdit.SmsAccount);
                        tb.SmsPassword = Code.Common.DESEnCode(vm.SmsConfigEdit.SmsPassword);
                        tb.SmsUrl = Code.Common.DESEnCode(string.IsNullOrEmpty(vm.SmsConfigEdit.SmsUrl) ? "" : vm.SmsConfigEdit.SmsUrl);
                        tb.SmsFreeSignName = Code.Common.DESEnCode(string.IsNullOrEmpty(vm.SmsConfigEdit.SmsFreeSignName) ? "" : vm.SmsConfigEdit.SmsFreeSignName);
                        tb.SmsTemplateCode = Code.Common.DESEnCode(string.IsNullOrEmpty(vm.SmsConfigEdit.SmsTemplateCode) ? "" : vm.SmsConfigEdit.SmsTemplateCode);
                        tb.Status = vm.SmsConfigEdit.Status;
                        tb.IsDefault = vm.SmsConfigEdit.IsDefault;
                        db.Set<Entity.tbSmsConfig>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了短信服务配置");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Entity.tbSmsConfig>()
                                  where p.Id == vm.SmsConfigEdit.Id
                                  select p).FirstOrDefault();

                        if (tb != null)
                        {
                            tb.No = vm.SmsConfigEdit.No == null ? db.Table<Entity.tbSmsConfig>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SmsConfigEdit.No;
                            tb.SmsServer = vm.SmsConfigEdit.SmsServer;
                            tb.SmsServerType = vm.SmsConfigEdit.SmsServerType;
                            tb.SmsAccount = Code.Common.DESEnCode(vm.SmsConfigEdit.SmsAccount);
                            tb.SmsPassword = Code.Common.DESEnCode(vm.SmsConfigEdit.SmsPassword);
                            tb.SmsUrl = Code.Common.DESEnCode(string.IsNullOrEmpty(vm.SmsConfigEdit.SmsUrl) ? "" : vm.SmsConfigEdit.SmsUrl);
                            tb.SmsFreeSignName = Code.Common.DESEnCode(string.IsNullOrEmpty(vm.SmsConfigEdit.SmsFreeSignName) ? "" : vm.SmsConfigEdit.SmsFreeSignName);
                            tb.SmsTemplateCode = Code.Common.DESEnCode(string.IsNullOrEmpty(vm.SmsConfigEdit.SmsTemplateCode) ? "" : vm.SmsConfigEdit.SmsTemplateCode);
                            tb.Status = vm.SmsConfigEdit.Status;
                            tb.IsDefault = vm.SmsConfigEdit.IsDefault;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了短信服务配置");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDisable(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Entity.tbSmsConfig>().Find(id);
                if (tb != null)
                {
                    tb.Status = !tb.Status;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了短信服务配置字段【Status】");
                    }
                }

                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDefault(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Entity.tbSmsConfig>().Find(id);
                if (tb != null)
                {
                    var list = from p in db.Table<Entity.tbSmsConfig>()
                               select p;

                    foreach (var a in list)
                    {
                        a.IsDefault = false;
                    }

                    tb.IsDefault = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了短信服务配置字段【IsDefault】");
                }

                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }

        public ActionResult Export()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var vm = new Models.SmsConfig.List();

                var tb = from p in db.Table<Entity.tbSmsConfig>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.SmsServer.Contains(vm.SearchText));
                }

                vm.SmsConfigList = (from p in tb
                                    orderby p.No
                                    select new Dto.SmsConfig.List
                                    {
                                        Id = p.Id,
                                        No = p.No,
                                        SmsServer = p.SmsServer,
                                        SmsAccount = p.SmsAccount,
                                        SmsPassword = p.SmsPassword,
                                        SmsUrl = p.SmsUrl,
                                        SmsServerType = p.SmsServerType,
                                        SmsFreeSignName = p.SmsFreeSignName,
                                        SmsTemplateCode = p.SmsTemplateCode,
                                        Status = p.Status,
                                        IsDefault = p.IsDefault
                                    }).ToPageList(vm.Page);

                foreach (var a in vm.SmsConfigList)
                {
                    a.SmsAccount = Code.Common.DESDeCode(a.SmsAccount);
                    a.SmsPassword = Code.Common.DESDeCode(a.SmsPassword);
                    a.SmsUrl = Code.Common.DESDeCode(a.SmsUrl);
                    a.SmsFreeSignName = Code.Common.DESDeCode(a.SmsFreeSignName);
                    a.SmsTemplateCode = Code.Common.DESDeCode(a.SmsTemplateCode);
                }

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("排序"),
                        new System.Data.DataColumn("短信服务"),
                        new System.Data.DataColumn("服务类型"),
                        new System.Data.DataColumn("短信账户"),
                        new System.Data.DataColumn("短信密码"),
                        new System.Data.DataColumn("短信地址(阿里)"),
                        new System.Data.DataColumn("短信签名(阿里)"),
                        new System.Data.DataColumn("短信模版(阿里)"),
                        new System.Data.DataColumn("状态"),
                        new System.Data.DataColumn("是否默认"),
                    });
                foreach (var a in vm.SmsConfigList)
                {
                    var dr = dt.NewRow();
                    dr["排序"] = a.No;
                    dr["短信服务"] = a.SmsServer;
                    dr["服务类型"] = a.SmsServerTypeName;
                    dr["短信账户"] = a.SmsAccount;
                    dr["短信密码"] = a.SmsPassword;
                    dr["短信地址(阿里)"] = a.SmsUrl;
                    dr["短信签名(阿里)"] = a.SmsFreeSignName;
                    dr["短信模版(阿里)"] = a.SmsTemplateCode;
                    dr["状态"] = a.Status ? "已启用" : "未启用";
                    dr["是否默认"] = a.IsDefault ? "已激活" : "未激活";
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

        public ActionResult Import()
        {
            var vm = new Models.SmsConfig.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Sms/Views/SmsConfig/SmsConfigTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.SmsConfig.Import vm)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);
                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 1、Excel模版校验
                    if (Code.Common.GetFileType(file.FileName) != Code.FileType.Excel)
                    {
                        ModelState.AddModelError("", "上传的文件不是正确的EXCLE文件!");
                        return View(vm);
                    }

                    var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), string.Empty);
                    if (dt == null)
                    {
                        ModelState.AddModelError("", "无法读取上传的文件，请检查文件格式是否正确!");
                        return View(vm);
                    }
                    var tbList = new List<string>() { "排序", "短信服务", "服务类型", "短信账户", "短信密码", "短信地址(阿里)", "短信签名(阿里)", "短信模版(阿里)", "状态" };
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
                        ModelState.AddModelError("", "上传的EXCEL晚自习内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }
                    #endregion

                    #region 2、Excel数据读取
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dtoKpi = new Dto.SmsConfig.Import()
                        {
                            No = Convert.ToString(dr["排序"]),
                            SmsServer = Convert.ToString(dr["短信服务"]),
                            SmsServerType = Convert.ToString(dr["服务类型"]),
                            SmsAccount = Convert.ToString(dr["短信账户"]),
                            SmsPassword = Convert.ToString(dr["短信密码"]),
                            SmsUrl = Convert.ToString(dr["短信地址(阿里)"]),
                            SmsFreeSignName = Convert.ToString(dr["短信签名(阿里)"]),
                            SmsTemplateCode = Convert.ToString(dr["短信模版(阿里)"]),
                            Status = Convert.ToString(dr["状态"])
                        };

                        if (vm.ImportList.Where(d => d.No == dtoKpi.No
                                                && d.SmsServer == dtoKpi.SmsServer
                                                && d.SmsServerType == dtoKpi.SmsServerType
                                                && d.SmsAccount == dtoKpi.SmsAccount
                                                && d.SmsPassword == dtoKpi.SmsPassword
                                                && d.SmsUrl == dtoKpi.SmsUrl
                                                && d.SmsFreeSignName == dtoKpi.SmsFreeSignName
                                                && d.SmsTemplateCode == dtoKpi.SmsTemplateCode
                                                && d.Status == dtoKpi.Status).Count() == 0)
                        {
                            vm.ImportList.Add(dtoKpi);
                        }
                    }
                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.No) &&
                        string.IsNullOrEmpty(d.SmsServer) &&
                        string.IsNullOrEmpty(d.SmsServerType) &&
                        string.IsNullOrEmpty(d.SmsAccount) &&
                        string.IsNullOrEmpty(d.SmsPassword) &&
                        string.IsNullOrEmpty(d.SmsUrl) &&
                        string.IsNullOrEmpty(d.SmsFreeSignName) &&
                        string.IsNullOrEmpty(d.SmsTemplateCode) &&
                        string.IsNullOrEmpty(d.Status));

                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "Excel未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    #region 3、Excel格式校验
                    //评价小组
                    var SmsConfigList = (from p in db.Table<Entity.tbSmsConfig>()
                                         select p).ToList();

                    var SmsConfigTypeList = typeof(Code.EnumHelper.SmsServerType).ToItemList();

                    var IsStatus = new List<string>() { "已启用", "未启用" };

                    foreach (var item in vm.ImportList)
                    {
                        #region 排序
                        int No = 0;
                        if (int.TryParse(item.No, out No) == false || No <= 0)
                        {
                            item.Error = item.Error + "【排序】必须是正整数!";
                            continue;
                        }
                        #endregion                        
                        #region 短信服务
                        if (string.IsNullOrEmpty(item.SmsServer))
                        {
                            item.Error = item.Error + "【短信服务】不能为空!";
                            continue;
                        }
                        #endregion
                        #region 服务类型
                        if (string.IsNullOrEmpty(item.SmsServerType))
                        {
                            item.SmsServerType = "未知";
                        }
                        else
                        {
                            if (SmsConfigTypeList.Where(d => d.Text == item.SmsServerType).Count() == decimal.Zero)
                            {
                                item.Error += "【服务类型】必须是：未知、开维教育、阿里大鱼!";
                                continue;
                            }
                        }
                        #endregion                        
                        #region 短信账户
                        if (string.IsNullOrEmpty(item.SmsAccount))
                        {
                            item.Error = item.Error + "【短信账户】不能为空!";
                            continue;
                        }
                        #endregion
                        #region 短信密码
                        if (string.IsNullOrEmpty(item.SmsPassword))
                        {
                            item.Error = item.Error + "【短信密码】不能为空!";
                            continue;
                        }
                        #endregion
                        #region 短信地址(阿里)
                        if (string.IsNullOrEmpty(item.SmsUrl) && item.SmsServerType == "阿里大鱼")
                        {
                            item.Error = item.Error + "【短信地址(阿里)】不能为空!";
                            continue;
                        }
                        #endregion
                        #region 短信签名(阿里)
                        if (string.IsNullOrEmpty(item.SmsFreeSignName) && item.SmsServerType == "阿里大鱼")
                        {
                            item.Error = item.Error + "【短信签名(阿里)】不能为空!";
                            continue;
                        }
                        #endregion
                        #region 短信模板(阿里)
                        if (string.IsNullOrEmpty(item.SmsTemplateCode) && item.SmsServerType == "阿里大鱼")
                        {
                            item.Error = item.Error + "【短信模板(阿里)】不能为空!";
                            continue;
                        }
                        #endregion
                        #region 状态
                        if (string.IsNullOrEmpty(item.Status))
                        {
                            item.Status = "未启用";
                        }
                        else
                        {
                            if (IsStatus.Where(d => d == item.Status).Count() == decimal.Zero)
                            {
                                item.Error += "【状态】必须是：已启用、未启用!";
                                continue;
                            }
                        }
                        #endregion                                                
                        #region 是否更新
                        if (vm.IsUpdate)
                        {
                            if (SmsConfigList.Where(d => d.SmsServer == item.SmsServer).Count() > decimal.One)
                            {
                                item.Error += "系统中该短信配置数据存在重复，无法确认需要更新的记录!";
                                continue;
                            }
                        }
                        else
                        {
                            if (SmsConfigList.Where(d => d.SmsServer == item.SmsServer).Count() > decimal.Zero)
                            {
                                item.Error += "系统中已存在该记录!";
                                continue;
                            }
                        }
                        #endregion
                    }
                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 5、Excel执行导入
                    var addKpiItemList = new List<Entity.tbSmsConfig>();
                    foreach (var item in vm.ImportList)
                    {
                        Entity.tbSmsConfig tb = null;
                        if (SmsConfigList.Where(d => d.SmsServer == item.SmsServer).Count() > decimal.Zero)
                        {
                            if (vm.IsUpdate)
                            {
                                tb = SmsConfigList.Where(d => d.SmsServer == item.SmsServer).FirstOrDefault();
                                tb.No = item.No.ConvertToInt();
                                tb.SmsServer = item.SmsServer;
                                tb.SmsAccount = Code.Common.DESEnCode(item.SmsAccount);
                                tb.SmsPassword = Code.Common.DESEnCode(item.SmsPassword);
                                tb.SmsUrl = Code.Common.DESEnCode(item.SmsUrl);
                                tb.SmsFreeSignName = Code.Common.DESEnCode(item.SmsFreeSignName);
                                tb.SmsTemplateCode = Code.Common.DESEnCode(item.SmsTemplateCode);
                                tb.Status = item.Status == "已启用" ? true : false;
                                #region 短信类型
                                if (item.SmsServerType == "未知")
                                {
                                    tb.SmsServerType = Code.EnumHelper.SmsServerType.None;
                                }
                                else if (item.SmsServerType == "开维教育")
                                {
                                    tb.SmsServerType = Code.EnumHelper.SmsServerType.KaiWei;
                                }
                                else if (item.SmsServerType == "阿里大鱼")
                                {
                                    tb.SmsServerType = Code.EnumHelper.SmsServerType.Aali;
                                }
                                else
                                {
                                    tb.SmsServerType = Code.EnumHelper.SmsServerType.None;
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            if (addKpiItemList.Where(d => d.SmsServer == item.SmsServer).Count() == decimal.Zero)
                            {
                                tb = new Entity.tbSmsConfig();
                                tb.No = item.No.ConvertToInt();
                                tb.SmsServer = item.SmsServer;
                                tb.SmsAccount = Code.Common.DESEnCode(item.SmsAccount);
                                tb.SmsPassword = Code.Common.DESEnCode(item.SmsPassword);
                                tb.SmsUrl = Code.Common.DESEnCode(item.SmsUrl);
                                tb.SmsFreeSignName = Code.Common.DESEnCode(item.SmsFreeSignName);
                                tb.SmsTemplateCode = Code.Common.DESEnCode(item.SmsTemplateCode);
                                tb.Status = item.Status == "已启用" ? true : false;
                                tb.IsDefault = false;
                                #region 短信类型
                                if (item.SmsServerType == "未知")
                                {
                                    tb.SmsServerType = Code.EnumHelper.SmsServerType.None;
                                }
                                else if (item.SmsServerType == "开维教育")
                                {
                                    tb.SmsServerType = Code.EnumHelper.SmsServerType.KaiWei;
                                }
                                else if (item.SmsServerType == "阿里大鱼")
                                {
                                    tb.SmsServerType = Code.EnumHelper.SmsServerType.Aali;
                                }
                                else
                                {
                                    tb.SmsServerType = Code.EnumHelper.SmsServerType.None;
                                }
                                #endregion
                                addKpiItemList.Add(tb);
                            }
                            else
                            {
                                tb = addKpiItemList.Where(d => d.SmsServer == item.SmsServer).FirstOrDefault();
                            }
                        }
                    }
                    db.Set<Entity.tbSmsConfig>().AddRange(addKpiItemList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了短信配置");
                        vm.Status = true;
                    }
                    #endregion
                }
            }
            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public static Dto.SmsConfig.Info SelectDefaultInfo()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = from p in db.TableRoot<Entity.tbSmsConfig>()
                         join t in db.Set<Admin.Entity.tbTenant>() on p.tbTenant.Id equals t.Id
                         where p.IsDeleted == false && t.IsDefault && t.IsDeleted == false
                          && p.Status && p.IsDefault
                         select p;

                var smsConfigInfo = (from p in tb
                                     orderby p.No
                                     select new Dto.SmsConfig.Info
                                     {
                                         Id = p.Id,
                                         No = p.No,
                                         SmsServer = p.SmsServer,
                                         SmsAccount = p.SmsAccount,
                                         SmsPassword = p.SmsPassword,
                                         SmsUrl = p.SmsUrl,
                                         SmsServerType = p.SmsServerType,
                                         SmsFreeSignName = p.SmsFreeSignName,
                                         SmsTemplateCode = p.SmsTemplateCode,
                                         Status = p.Status,
                                         IsDefault = p.IsDefault
                                     }).FirstOrDefault();

                if (smsConfigInfo != null)
                {
                    smsConfigInfo.SmsAccount = Code.Common.DESDeCode(smsConfigInfo.SmsAccount);
                    smsConfigInfo.SmsPassword = Code.Common.DESDeCode(smsConfigInfo.SmsPassword);
                    smsConfigInfo.SmsUrl = Code.Common.DESDeCode(smsConfigInfo.SmsUrl);
                    smsConfigInfo.SmsFreeSignName = Code.Common.DESDeCode(smsConfigInfo.SmsFreeSignName);
                    smsConfigInfo.SmsTemplateCode = Code.Common.DESDeCode(smsConfigInfo.SmsTemplateCode);
                }
                else
                {
                    return null;
                }
                return smsConfigInfo;
            }
        }
    }
}