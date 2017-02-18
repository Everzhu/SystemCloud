using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Perform.Controllers
{
    public class PerformItemController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformItem.List();
                vm.PerformName = db.Table<Perform.Entity.tbPerform>().FirstOrDefault(d => d.Id == vm.PerformId).PerformName;
                vm.PerformGroupList = Controllers.PerformGroupController.SelectList(vm.PerformId);

                var tb = from p in db.Table<Perform.Entity.tbPerformItem>()
                         where p.tbPerformGroup.tbPerform.Id == vm.PerformId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.PerformItemName.Contains(vm.SearchText));
                }

                vm.PerformItemList = (from p in tb
                                      orderby p.tbPerformGroup.No, p.No
                                      select new Dto.PerformItem.List
                                      {
                                          Id = p.Id,
                                          No = p.No,
                                          PerformGroupId = p.tbPerformGroup.Id,
                                          PerformGroupName = p.tbPerformGroup.PerformGroupName,
                                          PerformItemName = p.PerformItemName,
                                          Rate = p.Rate,
                                          ScoreMax = p.ScoreMax,
                                          IsSelect = p.IsSelect,
                                          IsMany = p.IsMany,
                                          DefaultValue = p.DefaultValue
                                      }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.PerformItem.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                PerformId = vm.PerformId
            }));
        }

        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Perform.Entity.tbPerformItem>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价项目");
                }
                return Code.MvcHelper.Post();
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int performGroupId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Perform.Entity.tbPerformItem>()
                          where p.tbPerformGroup.Id == performGroupId && p.tbPerformGroup.IsDeleted == false
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.PerformItemName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformItem.Edit();
                vm.PerformGroupList = Controllers.PerformGroupController.SelectList(vm.PerformId);
                if (id != 0)
                {
                    var tb = (from p in db.Table<Perform.Entity.tbPerformItem>()
                              where p.Id == id
                              select new Dto.PerformItem.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  PerformItemName = p.PerformItemName,
                                  Rate = p.Rate,
                                  ScoreMax = p.ScoreMax,
                                  PerformGroupId = p.tbPerformGroup.Id,
                                  IsSelect = p.IsSelect,
                                  IsMany = p.IsMany,
                                  DefaultValue = p.DefaultValue
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.PerformItemEdit = tb;
                        vm.PerformOptionList = (from s in db.Table<Entity.tbPerformOption>()
                                                where s.tbPerformItem.Id == id
                                                select new Dto.PerformOption.List
                                                {
                                                    Id = s.Id,
                                                    No = s.No,
                                                    PerformItemId = s.tbPerformItem.Id,
                                                    OptionName = s.OptionName,
                                                    OptionValue = s.OptionValue
                                                }).ToList();
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult Edit(Models.PerformItem.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var tbGroupPerformItemName = new List<string>();
                    if (vm.PerformItemEdit.Id == 0)
                    {
                        //同一组下面项目名称不能重复
                        tbGroupPerformItemName = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                                  where p.tbPerformGroup.Id == vm.PerformItemEdit.PerformGroupId
                                                  && p.tbPerformGroup.IsDeleted == false
                                                  select p.PerformItemName).ToList();
                    }
                    else
                    {
                        //同一组下面项目名称不能重复
                        tbGroupPerformItemName = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                                  where p.tbPerformGroup.Id == vm.PerformItemEdit.PerformGroupId
                                                  && p.Id != vm.PerformItemEdit.Id
                                                  && p.tbPerformGroup.IsDeleted == false
                                                  select p.PerformItemName).ToList();
                    }

                    if (tbGroupPerformItemName.Where(d => d == vm.PerformItemEdit.PerformItemName).Count() > decimal.Zero)
                    {
                        var errorMsg = new { Status = decimal.Zero, Message = "同一分组下评价项目不能重复" };
                        return Json(errorMsg);
                    }

                    if (db.Table<Perform.Entity.tbPerformItem>().Where(d => d.tbPerformGroup.Id == vm.PerformItemEdit.PerformGroupId && d.PerformItemName == vm.PerformItemEdit.PerformItemName && d.Id != vm.PerformItemEdit.Id).Count() > decimal.Zero)
                    {
                        var errorMsg = new { Status = decimal.Zero, Message = "同一评价下不同分组评价项目不能重复" };
                        return Json(errorMsg);
                    }

                    var emptyString = new string[] { };
                    var ids = Request["txtId"]?.Split(',') ?? emptyString;
                    var nos = Request["txtNo"]?.Split(',') ?? emptyString;
                    var names = Request["txtOptionName"]?.Split(',') ?? emptyString;
                    var values = Request["txtOptionValue"]?.Split(',') ?? emptyString;

                    if (vm.PerformItemEdit.IsSelect)
                    {
                        if (names.Count() == decimal.Zero || names.Count(p => !string.IsNullOrWhiteSpace(p)) == 0)
                        {
                            error.AddError("至少应添加一个 [选项名称] 不为空的选项数据！");
                            return Code.MvcHelper.Post(error);
                        }
                    }

                    //新增
                    if (vm.PerformItemEdit.Id == 0)
                    {
                        var tb = new Perform.Entity.tbPerformItem();
                        tb.No = vm.PerformItemEdit.No == null ? db.Table<Perform.Entity.tbPerformItem>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.PerformItemEdit.No;
                        tb.PerformItemName = vm.PerformItemEdit.PerformItemName;
                        tb.Rate = vm.PerformItemEdit.Rate;
                        tb.ScoreMax = vm.PerformItemEdit.ScoreMax;
                        tb.IsSelect = vm.PerformItemEdit.IsSelect;
                        tb.IsMany = vm.PerformItemEdit.IsMany;
                        tb.DefaultValue = vm.PerformItemEdit.DefaultValue;
                        tb.tbPerformGroup = db.Set<Perform.Entity.tbPerformGroup>().Find(vm.PerformItemEdit.PerformGroupId);
                        db.Set<Perform.Entity.tbPerformItem>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价项目");
                        }
                        vm.PerformItemEdit.Id = tb.Id;
                    }
                    else//修改
                    {
                        var tb = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                  where p.Id == vm.PerformItemEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.PerformItemEdit.No == null ? db.Table<Perform.Entity.tbPerformItem>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.PerformItemEdit.No;
                            tb.PerformItemName = vm.PerformItemEdit.PerformItemName;
                            tb.Rate = vm.PerformItemEdit.Rate;
                            tb.ScoreMax = vm.PerformItemEdit.ScoreMax;
                            tb.IsSelect = vm.PerformItemEdit.IsSelect;
                            tb.IsMany = vm.PerformItemEdit.IsMany;
                            tb.DefaultValue = vm.PerformItemEdit.DefaultValue;
                            tb.tbPerformGroup = db.Set<Perform.Entity.tbPerformGroup>().Find(vm.PerformItemEdit.PerformGroupId);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价项目");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }

                    #region 保存答案
                    if (vm.PerformItemEdit.IsSelect)
                    {

                        if (ids.Count() == decimal.Zero || nos.Count() == decimal.Zero || names.Count() == decimal.Zero)
                        {
                            return Code.MvcHelper.Post(error);
                        }

                        var list = (from p in db.Table<Entity.tbPerformOption>()
                                    where p.tbPerformItem.Id == vm.PerformItemEdit.Id
                                    select p).ToList();
                        //删除无效
                        foreach (var a in list.Where(d => ids.Contains(d.Id.ToString()) == false))
                        {
                            a.IsDeleted = true;
                        }
                        for (var i = 0; i < ids.Count(); i++)
                        {
                            if (string.IsNullOrEmpty(names[i]))
                            {
                                //输入内容为空,判断是否存在Id
                                if (string.IsNullOrEmpty(ids[i]) == false)
                                {
                                    //如果是有id的，那就是数据库中记录的，应该做删除
                                    var tb = list.Where(d => d.Id == ids[i].ConvertToInt()).FirstOrDefault();
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价内容");
                                    tb.IsDeleted = true;
                                }
                            }
                            else
                            {
                                //输入内容不为空，判断是否存在id并执行对应的操作
                                if (string.IsNullOrEmpty(ids[i]) == false)
                                {
                                    //如果有id的，执行更新操作
                                    var tb = list.Where(d => d.Id == ids[i].ConvertToInt()).FirstOrDefault();
                                    tb.No = nos[i].ConvertToInt();
                                    tb.OptionName = names[i];
                                    tb.OptionValue = values[i].ConvertToDecimal();
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价内容");
                                }
                                else
                                {
                                    //没有id的，执行插入操作
                                    var tb = new Entity.tbPerformOption();
                                    tb.No = nos[i].ConvertToInt();
                                    tb.OptionName = names[i];
                                    tb.tbPerformItem = db.Set<Entity.tbPerformItem>().Find(vm.PerformItemEdit.Id);
                                    tb.OptionValue = values[i].ConvertToDecimal();
                                    db.Set<Entity.tbPerformOption>().Add(tb);
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价内容");
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                    #endregion
                }

                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult Import()
        {
            var vm = new Models.PerformItem.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Perform/Views/PerformItem/PerformItemTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.PerformItem.Import vm)
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
                    var tbList = new List<string>() { "排序", "评价项目", "评价分组", "满分值", "折算比例", "是否多次", "是否选项", "默认分数", "选项排序", "选项名称", "选项分值" };
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
                        var dtoTest = new Dto.PerformItem.Import()
                        {
                            No = Convert.ToString(dr["排序"]),
                            PerformItemName = Convert.ToString(dr["评价项目"]),
                            PerformGroupName = Convert.ToString(dr["评价分组"]),
                            ScoreMax = Convert.ToString(dr["满分值"]),
                            Rate = Convert.ToString(dr["折算比例"]),
                            IsMany = Convert.ToString(dr["是否多次"]),
                            IsSelect = Convert.ToString(dr["是否选项"]),
                            DefaultValue = Convert.ToString(dr["默认分数"]),
                            OptionNo = Convert.ToString(dr["选项排序"]),
                            OptionName = Convert.ToString(dr["选项名称"]),
                            OptionValue = Convert.ToString(dr["选项分值"])
                        };
                        if (vm.ImportList.Where(d => d.No == dtoTest.No
                                                && d.No == dtoTest.No
                                                && d.PerformItemName == dtoTest.PerformItemName
                                                && d.PerformGroupName == dtoTest.PerformGroupName
                                                && d.ScoreMax == dtoTest.ScoreMax
                                                && d.Rate == dtoTest.Rate
                                                && d.IsMany == dtoTest.IsMany
                                                && d.IsSelect == dtoTest.IsSelect
                                                && d.DefaultValue == dtoTest.DefaultValue
                                                && d.OptionNo == dtoTest.OptionNo
                                                && d.OptionName == dtoTest.OptionName
                                                && d.OptionValue == dtoTest.OptionValue).Count() == 0)
                        {
                            vm.ImportList.Add(dtoTest);
                        }
                    }
                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.No) &&
                        string.IsNullOrEmpty(d.PerformItemName) &&
                        string.IsNullOrEmpty(d.PerformGroupName) &&
                        string.IsNullOrEmpty(d.ScoreMax) &&
                        string.IsNullOrEmpty(d.Rate) &&
                        string.IsNullOrEmpty(d.IsMany) &&
                        string.IsNullOrEmpty(d.IsSelect) &&
                        string.IsNullOrEmpty(d.DefaultValue) &&
                        string.IsNullOrEmpty(d.OptionNo) &&
                        string.IsNullOrEmpty(d.OptionName) &&
                        string.IsNullOrEmpty(d.OptionValue)
                    );
                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "Excel未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    #region 3、Excel格式校验
                    //评价分组
                    var PerformGroup = (from p in db.Table<Perform.Entity.tbPerformGroup>()
                                       .Include(d => d.tbPerform)
                                        where p.tbPerform.Id == vm.PerformId
                                        && p.tbPerform.IsDeleted == false
                                        select p).ToList();
                    //评价内容
                    var PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                          .Include(d => d.tbPerformGroup)
                                           where p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                           && p.tbPerformGroup.IsDeleted == false
                                           && p.tbPerformGroup.tbPerform.IsDeleted == false
                                           select p).ToList();

                    //评价选项
                    var PerformOptionList = (from p in db.Table<Entity.tbPerformOption>()
                                          .Include(d => d.tbPerformItem)
                                             where p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                                             && p.tbPerformItem.IsDeleted == false
                                             && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                             && p.tbPerformItem.tbPerformGroup.tbPerform.IsDeleted == false
                                             select p).ToList();
                    //是否多次打分
                    var IsManyList = new List<string>() { "是", "否" };
                    //是否可选
                    var IsSelectList = new List<string>() { "文本框", "下拉框" };

                    foreach (var item in vm.ImportList)
                    {
                        int No = 0;
                        if (int.TryParse(item.No, out No) == false || No <= 0)
                        {
                            item.Error = item.Error + "【排序】必须是正整数!";
                            continue;
                        }
                        if (string.IsNullOrEmpty(item.PerformItemName))
                        {
                            item.Error = item.Error + "【评价项目】不能为空!";
                            continue;
                        }
                        if (string.IsNullOrEmpty(item.PerformGroupName))
                        {
                            item.Error = item.Error + "【评价分组】不能为空!";
                            continue;
                        }
                        else
                        {
                            if (PerformGroup.Where(d => d.PerformGroupName == item.PerformGroupName).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "【评价分组】不存在数据库!";
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(item.ScoreMax))
                        {
                            item.ScoreMax = "100";
                        }
                        else
                        {
                            try
                            {
                                var ScoreMax = Convert.ToDecimal(item.ScoreMax);
                            }
                            catch
                            {
                                item.Error = item.Error + "【满分值】必须是正确的数字格式!";
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(item.Rate))
                        {
                            item.Rate = "100";
                        }
                        else
                        {
                            try
                            {
                                var rate = Convert.ToDecimal(item.Rate);
                            }
                            catch
                            {
                                item.Error = item.Error + "【折算比例】必须是正确的数字格式!";
                                continue;
                            }
                        }

                        if (string.IsNullOrEmpty(item.DefaultValue))
                        {
                            item.DefaultValue = "0";
                        }
                        else
                        {
                            try
                            {
                                var DefaultValue = Convert.ToDecimal(item.DefaultValue);
                            }
                            catch
                            {
                                item.Error = item.Error + "【默认分数】必须是正确的数字格式!";
                                continue;
                            }
                        }

                        #region 是否多次
                        if (string.IsNullOrEmpty(item.IsMany))
                        {
                            item.Error = item.Error + "【是否多次】不能为空!";
                            continue;
                        }
                        else
                        {
                            if (IsManyList.Where(d => d == item.IsMany).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "【是否多次】必须是：是、否!";
                                continue;
                            }
                            else
                            {
                                if (item.IsMany == "是")
                                {
                                    #region 是否选项
                                    if (string.IsNullOrEmpty(item.IsSelect))
                                    {
                                        item.Error = item.Error + "【是否选项】不能为空!";
                                        continue;
                                    }
                                    else
                                    {
                                        if (IsSelectList.Where(d => d == item.IsSelect).Count() == decimal.Zero)
                                        {
                                            item.Error = item.Error + "【是否选项】必须是：文本框、下拉框!";
                                            continue;
                                        }
                                        else
                                        {
                                            if (item.IsSelect == "下拉框")
                                            {
                                                #region 选项排序
                                                int KpiNo = 0;
                                                if (int.TryParse(item.OptionNo, out KpiNo) == false || KpiNo <= 0)
                                                {
                                                    item.Error = item.Error + "【选项排序】必须是正整数!";
                                                    continue;
                                                }
                                                #endregion
                                                #region 选项名称
                                                if (string.IsNullOrEmpty(item.OptionName))
                                                {
                                                    item.Error = item.Error + "【选项名称】不能为空!";
                                                    continue;
                                                }
                                                #endregion
                                                #region 选项分值
                                                if (string.IsNullOrEmpty(item.OptionValue))
                                                {
                                                    item.Error = item.Error + "【选项分值】不能为空!";
                                                    continue;
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        var KpiOptionValue = Convert.ToDecimal(item.OptionValue);
                                                    }
                                                    catch
                                                    {
                                                        item.Error = item.Error + "【选项分值】必须是正确的数字格式!";
                                                        continue;
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 是否选项
                                    if (string.IsNullOrEmpty(item.IsSelect))
                                    {
                                        item.Error = item.Error + "【是否选项】不能为空!";
                                        continue;
                                    }
                                    else
                                    {
                                        if (item.IsSelect != "文本框")
                                        {
                                            item.Error = item.Error + "【是否选项】必须是：文本框!";
                                            continue;
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        #endregion
                        if (vm.IsUpdate)
                        {
                            if (PerformItemList.Where(d => d.PerformItemName == item.PerformItemName && d.tbPerformGroup.PerformGroupName == item.PerformGroupName).Count() > 1)
                            {
                                item.Error += "系统中该评价内容数据存在重复，无法确认需要更新的记录!";
                                continue;
                            }
                        }
                        else
                        {
                            if (PerformItemList.Where(d => d.PerformItemName == item.PerformItemName && d.tbPerformGroup.PerformGroupName == item.PerformGroupName).Count() > 0)
                            {
                                item.Error += "系统中已存在该记录!";
                                continue;
                            }
                        }
                    }
                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 5、Excel执行导入
                    var addPerformItemList = new List<Perform.Entity.tbPerformItem>();
                    var addPerformOptionList = new List<Entity.tbPerformOption>();
                    foreach (var item in vm.ImportList)
                    {
                        Perform.Entity.tbPerformItem tb = null;
                        if (PerformItemList.Where(d => d.tbPerformGroup.PerformGroupName == item.PerformGroupName && d.PerformItemName == item.PerformItemName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                tb = PerformItemList.Where(d => d.tbPerformGroup.PerformGroupName == item.PerformGroupName && d.PerformItemName == item.PerformItemName).FirstOrDefault();
                                tb.No = item.No.ConvertToInt();
                                tb.PerformItemName = item.PerformItemName;
                                tb.IsMany = item.IsMany == "是" ? true : false;
                                tb.IsSelect = item.IsSelect == "文本框" ? false : true;
                                tb.DefaultValue = item.DefaultValue.ConvertToDecimal();
                                if (!string.IsNullOrEmpty(item.PerformGroupName))
                                {
                                    tb.tbPerformGroup = PerformGroup.Where(d => d.PerformGroupName == item.PerformGroupName).FirstOrDefault();
                                }

                                tb.ScoreMax = item.ScoreMax;
                                tb.Rate = Convert.ToDecimal(item.Rate);
                            }
                        }
                        else
                        {
                            if (addPerformItemList.Where(d => d.tbPerformGroup.PerformGroupName == item.PerformGroupName && d.PerformItemName == item.PerformItemName).Count() == decimal.Zero)
                            {
                                tb = new Perform.Entity.tbPerformItem();
                                tb.No = item.No.ConvertToInt();
                                tb.PerformItemName = item.PerformItemName;
                                tb.IsMany = item.IsMany == "是" ? true : false;
                                tb.IsSelect = item.IsSelect == "文本框" ? false : true;
                                tb.DefaultValue = item.DefaultValue.ConvertToDecimal();
                                if (!string.IsNullOrEmpty(item.PerformGroupName))
                                {
                                    tb.tbPerformGroup = PerformGroup.Where(d => d.PerformGroupName == item.PerformGroupName).FirstOrDefault();
                                }

                                tb.ScoreMax = item.ScoreMax;
                                tb.Rate = Convert.ToDecimal(item.Rate);
                                addPerformItemList.Add(tb);
                            }
                            else
                            {
                                tb = addPerformItemList.Where(d => d.tbPerformGroup.PerformGroupName == item.PerformGroupName && d.PerformItemName == item.PerformItemName).FirstOrDefault();
                            }
                        }

                        if (string.IsNullOrEmpty(item.OptionName) == false && item.IsMany == "是" && item.IsSelect == "下拉框")
                        {
                            if (PerformOptionList.Where(d => d.tbPerformItem.tbPerformGroup.PerformGroupName == item.PerformGroupName && d.tbPerformItem.PerformItemName == item.PerformItemName && d.OptionName == item.OptionName).Count() == decimal.Zero)
                            {
                                if (addPerformOptionList.Where(d => d.tbPerformItem.tbPerformGroup.PerformGroupName == item.PerformGroupName && d.tbPerformItem.PerformItemName == item.PerformItemName && d.OptionName == item.OptionName).Count() == decimal.Zero)
                                {
                                    var tbOption = new Entity.tbPerformOption();
                                    tbOption.No = item.OptionNo.ConvertToInt();
                                    tbOption.OptionName = item.OptionName;
                                    tbOption.OptionValue = item.OptionValue.ConvertToDecimal();
                                    tbOption.tbPerformItem = tb;
                                    addPerformOptionList.Add(tbOption);
                                }
                            }
                        }
                    }
                    db.Set<Entity.tbPerformItem>().AddRange(addPerformItemList);
                    db.Set<Entity.tbPerformOption>().AddRange(addPerformOptionList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了评价项目");
                        vm.Status = true;
                    }
                    #endregion
                }
            }
            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public ActionResult Export()
        {
            var vm = new Models.PerformItem.List();

            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tb = from p in db.Table<Perform.Entity.tbPerformItem>()
                         where p.tbPerformGroup.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.PerformItemName.Contains(vm.SearchText));
                }

                vm.PerformItemList = (from p in tb
                                      orderby p.tbPerformGroup.No, p.No
                                      select new Dto.PerformItem.List
                                      {
                                          Id = p.Id,
                                          No = p.No,
                                          PerformGroupName = p.tbPerformGroup.PerformGroupName,
                                          PerformItemName = p.PerformItemName,
                                          Rate = p.Rate,
                                          ScoreMax = p.ScoreMax,
                                          IsSelect = p.IsSelect,
                                          IsMany = p.IsMany,
                                          DefaultValue = p.DefaultValue
                                      }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("排序"),
                        new System.Data.DataColumn("评教分组"),
                        new System.Data.DataColumn("评价项目"),
                        new System.Data.DataColumn("满分值"),
                        new System.Data.DataColumn("折算比例"),
                        new System.Data.DataColumn("是否选项"),
                        new System.Data.DataColumn("是否多次"),
                        new System.Data.DataColumn("默认分数")
                    });
                foreach (var a in vm.PerformItemList)
                {
                    var dr = dt.NewRow();
                    dr["排序"] = a.No;
                    dr["评教分组"] = a.PerformGroupName;
                    dr["评价项目"] = a.PerformItemName;
                    dr["满分值"] = a.ScoreMax;
                    dr["折算比例"] = a.Rate;
                    dr["是否选项"] = a.IsSelect ? "是" : "否";
                    dr["是否多次"] = a.IsMany ? "是" : "否";
                    dr["默认分数"] = a.DefaultValue;
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
    }
}