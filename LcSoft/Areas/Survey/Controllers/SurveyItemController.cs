using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Data;

namespace XkSystem.Areas.Survey.Controllers
{
    public class SurveyItemController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyItem.List();
                vm.SurveyName = db.Table<Entity.tbSurvey>().FirstOrDefault(d => d.Id == vm.SurveyId).SurveyName;
                vm.SurveyGroupList = Controllers.SurveyGroupController.SelectList(vm.SurveyId);

                var tb = from p in db.Table<Entity.tbSurveyItem>()
                         where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                         select p;
                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.SurveyItemName.Contains(vm.SearchText));
                }

                vm.SurveyItemList = (from p in tb
                                     orderby p.tbSurveyGroup.No, p.No
                                     select new Dto.SurveyItem.List
                                     {
                                         Id = p.Id,
                                         No = p.No,
                                         SurveyGroupId = p.tbSurveyGroup.Id,
                                         SurveyItemName = p.SurveyItemName,
                                         IsVertical = p.IsVertical,
                                         SurveyItemType = p.SurveyItemType,
                                         TextMaxLength = p.TextMaxLength
                                     }).ToList();

                var optionList = (from p in db.Table<Entity.tbSurveyOption>()
                                  where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                  group p by p.tbSurveyItem.Id into g
                                  select new
                                  {
                                      ItemId = g.Key,
                                      OptionCount = g.Count()
                                  }).ToList();
                foreach (var item in vm.SurveyItemList)
                {
                    item.OptionCount = optionList.Where(d => d.ItemId == item.Id).Select(d => d.OptionCount).FirstOrDefault();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SurveyItem.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                surveyId = vm.SurveyId,
            }));
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyItem.Edit();
                vm.SurveyItemTypeList = typeof(Code.EnumHelper.SurveyItemType).ToItemList();
                vm.SurveyGroupList = Controllers.SurveyGroupController.SelectList(vm.SurveyId);

                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbSurveyItem>()
                              where p.Id == id
                              select new Dto.SurveyItem.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  IsVertical = p.IsVertical,
                                  SurveyGroupId = p.tbSurveyGroup.Id,
                                  SurveyItemType = p.SurveyItemType,
                                  SurveyItemName = p.SurveyItemName,
                                  TextMaxLength = p.TextMaxLength
                              }).FirstOrDefault();

                    if (tb != null)
                    {
                        vm.SurveyItemEdit = tb;
                        vm.SurveyOptionList = (from s in db.Table<Entity.tbSurveyOption>()
                                               where s.tbSurveyItem.Id == id
                                               select new Dto.SurveyOption.List
                                               {
                                                   Id = s.Id,
                                                   No = s.No,
                                                   OptionName = s.OptionName,
                                                   OptionValue = s.OptionValue,
                                               }).ToList();
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.SurveyItem.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.SurveyItemEdit.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox)
                    {
                        var arrystr = new string[] { };
                        var txtOptionId = Request["txtOptionId"] != null ? Request["txtOptionId"].Split(',') : arrystr;
                        var txtOptionNo = Request["txtOptionNo"] != null ? Request["txtOptionNo"].Split(',') : arrystr;
                        var txtOptionName = Request["txtOptionName"] != null ? Request["txtOptionName"].Split(',') : arrystr;
                        var txtOptionValue = Request["txtOptionValue"] != null ? Request["txtOptionValue"].Split(',') : arrystr;

                        if (txtOptionId.Count() > decimal.One || txtOptionNo.Count() > decimal.One || txtOptionName.Count() > decimal.One)
                        {
                            var errorMsg = new { Status = decimal.Zero, Message = "问答题，只允许保留一个选择项目，请删除多余项目" };
                            return Json(errorMsg);
                        }
                    }
                    #region 保存项目
                    if (vm.SurveyItemEdit.Id == 0)//新增项目
                    {
                        var tb = new Entity.tbSurveyItem();
                        tb.No = vm.SurveyItemEdit.No == null ? db.Table<Entity.tbSurveyItem>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SurveyItemEdit.No;
                        tb.SurveyItemName = vm.SurveyItemEdit.SurveyItemName;
                        tb.tbSurveyGroup = db.Set<Entity.tbSurveyGroup>().Find(vm.SurveyItemEdit.SurveyGroupId);
                        tb.IsVertical = vm.SurveyItemEdit.IsVertical;
                        tb.SurveyItemType = vm.SurveyItemEdit.SurveyItemType;
                        tb.TextMaxLength = vm.SurveyItemEdit.TextMaxLength;
                        db.Set<Entity.tbSurveyItem>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价项目");
                        }
                        vm.SurveyItemEdit.Id = tb.Id;
                    }
                    else//修改
                    {
                        var tb = (from p in db.Table<Entity.tbSurveyItem>()
                                  where p.Id == vm.SurveyItemEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.SurveyItemEdit.No == null ? db.Table<Entity.tbSurveyItem>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SurveyItemEdit.No;
                            tb.SurveyItemName = vm.SurveyItemEdit.SurveyItemName;
                            tb.tbSurveyGroup = db.Set<Entity.tbSurveyGroup>().Find(vm.SurveyItemEdit.SurveyGroupId);
                            tb.IsVertical = vm.SurveyItemEdit.IsVertical;
                            tb.SurveyItemType = vm.SurveyItemEdit.SurveyItemType;
                            tb.TextMaxLength = vm.SurveyItemEdit.TextMaxLength;
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
                    #endregion
                    if (vm.SurveyItemEdit.Id != 0)
                    {
                        var arrystr = new string[] { };
                        var txtOptionId = Request["txtOptionId"] != null ? Request["txtOptionId"].Split(',') : arrystr;
                        var txtOptionNo = Request["txtOptionNo"] != null ? Request["txtOptionNo"].Split(',') : arrystr;
                        var txtOptionName = Request["txtOptionName"] != null ? Request["txtOptionName"].Split(',') : arrystr;
                        var txtOptionValue = Request["txtOptionValue"] != null ? Request["txtOptionValue"].Split(',') : arrystr;

                        if (txtOptionId.Count() == decimal.Zero || txtOptionNo.Count() == decimal.Zero || txtOptionName.Count() == decimal.Zero)
                        {
                            return Code.MvcHelper.Post(error);
                        }

                        var list = (from p in db.Table<Entity.tbSurveyOption>()
                                    where p.tbSurveyItem.Id == vm.SurveyItemEdit.Id
                                    select p).ToList();
                        //删除无效
                        foreach (var a in list.Where(d => txtOptionId.Contains(d.Id.ToString()) == false))
                        {
                            a.IsDeleted = true;
                        }
                        for (var i = 0; i < txtOptionId.Count(); i++)
                        {
                            if (string.IsNullOrEmpty(txtOptionName[i]))
                            {
                                //输入内容为空,判断是否存在Id
                                if (string.IsNullOrEmpty(txtOptionId[i]) == false)
                                {
                                    //如果是有id的，那就是数据库中记录的，应该做删除
                                    var tb = list.Where(d => d.Id == txtOptionId[i].ConvertToInt()).FirstOrDefault();
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价内容");
                                    tb.IsDeleted = true;
                                }
                            }
                            else
                            {
                                //输入内容不为空，判断是否存在id并执行对应的操作
                                if (string.IsNullOrEmpty(txtOptionId[i]) == false)
                                {
                                    //如果有id的，执行更新操作
                                    var tb = list.Where(d => d.Id == txtOptionId[i].ConvertToInt()).FirstOrDefault();
                                    tb.No = txtOptionNo[i].ConvertToInt();
                                    tb.OptionName = txtOptionName[i];
                                    tb.OptionValue = txtOptionValue[i].ConvertToDecimal();
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价内容");
                                }
                                else
                                {
                                    //没有id的，执行插入操作
                                    var tb = new Entity.tbSurveyOption();
                                    tb.No = txtOptionNo[i].ConvertToInt();
                                    tb.OptionName = txtOptionName[i];
                                    tb.OptionValue = txtOptionValue[i].ConvertToDecimal();
                                    tb.tbSurveyItem = db.Set<Entity.tbSurveyItem>().Find(vm.SurveyItemEdit.Id);
                                    db.Set<Entity.tbSurveyOption>().Add(tb);
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价内容");
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSurveyItem>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var surveyOptionList = (from p in db.Table<Entity.tbSurveyOption>()
                                            .Include(d => d.tbSurveyItem)
                                        where ids.Contains(p.tbSurveyItem.Id)
                                        select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    foreach (var option in surveyOptionList.Where(d => d.tbSurveyItem.Id == a.Id))
                    {
                        option.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价项目");
                }

                return Code.MvcHelper.Post();
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int groupId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (groupId != 0)
                {
                    var tb = (from p in db.Table<Entity.tbSurveyItem>()
                              where p.tbSurveyGroup.Id == groupId
                              select new System.Web.Mvc.SelectListItem
                              {
                                  Text = p.SurveyItemName,
                                  Value = p.Id.ToString()
                              }).ToList();
                    return tb;
                }
                else
                {
                    return new List<SelectListItem>();
                }
            }
        }

        [NonAction]
        public static List<Dto.SurveyItem.List> SelectItemList(int groupId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (groupId != 0)
                {
                    var tb = (from p in db.Table<Entity.tbSurveyItem>()
                              where p.tbSurveyGroup.Id == groupId
                              select new Dto.SurveyItem.List
                              {
                                  Id = p.Id,
                                  SurveyItemName = p.SurveyItemName,
                                  SurveyItemType = p.SurveyItemType
                              }).ToList();
                    return tb;
                }
                else
                {
                    return new List<Dto.SurveyItem.List>();
                }
            }
        }


        public ActionResult Import()
        {
            var vm = new Models.SurveyItem.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Survey/Views/SurveyItem/SurveyItemTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.SurveyItem.Import vm)
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
                    var tbList = new List<string>() { "排序", "评价内容", "评价分组", "试题类型", "是否纵向" };
                    var dtOptionColumns = new List<string>();
                    foreach (var columns in dt.Columns)
                    {
                        if (!dtOptionColumns.Contains(columns.ToString()) && !tbList.Contains(columns.ToString()))
                        {
                            dtOptionColumns.Add(columns.ToString());
                        }
                    }
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
                        var dtoTest = new Dto.SurveyItem.Import()
                        {
                            No = Convert.ToString(dr["排序"]),
                            SurveyItemName = Convert.ToString(dr["评价内容"]),
                            SurveyGroupName = Convert.ToString(dr["评价分组"]),
                            SurveyItemType = Convert.ToString(dr["试题类型"]),
                            IsVertical = Convert.ToString(dr["是否纵向"]),
                            ImportOptionList = GetImportOptionList(dr, dtOptionColumns)
                        };
                        if (vm.ImportList.Where(d => d.No == dtoTest.No
                                                && d.SurveyItemName == dtoTest.SurveyItemName
                                                && d.SurveyGroupName == dtoTest.SurveyGroupName
                                                && d.SurveyItemType == dtoTest.SurveyItemType
                                                && d.IsVertical == dtoTest.IsVertical).Count() == 0)
                        {
                            vm.ImportList.Add(dtoTest);
                        }
                    }
                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.No) &&
                        string.IsNullOrEmpty(d.SurveyItemName) &&
                        string.IsNullOrEmpty(d.SurveyGroupName) &&
                        string.IsNullOrEmpty(d.SurveyItemType) &&
                        string.IsNullOrEmpty(d.IsVertical)
                    );
                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "Excel未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    #region 3、Excel格式校验
                    //评价分组
                    var SurveyGroup = (from p in db.Table<Entity.tbSurveyGroup>()
                                       .Include(d => d.tbSurvey)
                                       where p.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurvey.IsDeleted == false
                                       select p).ToList();
                    //评价内容
                    var SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                          .Include(d => d.tbSurveyGroup)
                                          where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                          && p.tbSurveyGroup.IsDeleted == false
                                          && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                          select p).ToList();
                    //评价选项
                    var SurveyOptionList = (from p in db.Table<Entity.tbSurveyOption>()
                                            .Include(d => d.tbSurveyItem)
                                            .Include(d => d.tbSurveyItem.tbSurveyGroup)
                                            where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                            && p.tbSurveyItem.IsDeleted == false
                                            && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                            && p.tbSurveyItem.tbSurveyGroup.tbSurvey.IsDeleted == false
                                            select p).ToList();
                    //试题类型
                    var SurveyItemTypeList = typeof(Code.EnumHelper.SurveyItemType).ToItemList();

                    //是否纵向
                    var IsVerticalList = new List<string>() { "是", "否" };

                    foreach (var item in vm.ImportList)
                    {
                        int No = 0;
                        if (int.TryParse(item.No, out No) == false || No <= 0)
                        {
                            item.Error = item.Error + "【排序】必须是正整数!";
                            continue;
                        }
                        if (string.IsNullOrEmpty(item.SurveyItemName))
                        {
                            item.Error = item.Error + "【评价内容】不能为空!";
                            continue;
                        }
                        if (string.IsNullOrEmpty(item.SurveyGroupName))
                        {
                            item.Error = item.Error + "【评价分组】不能为空!";
                            continue;
                        }
                        else
                        {
                            if (SurveyGroup.Where(d => d.SurveyGroupName == item.SurveyGroupName).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "【评价分组】不存在数据库!";
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(item.SurveyItemType))
                        {
                            item.SurveyItemType = "单选题";
                        }
                        else
                        {
                            if (SurveyItemTypeList.Where(d => d.Text == item.SurveyItemType).Count() == decimal.Zero)
                            {
                                item.Error += "试题类型必须是：单选题、多选题、问答题!";
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(item.IsVertical))
                        {
                            item.IsVertical = "否";
                        }
                        else
                        {
                            if (IsVerticalList.Where(d => d == item.IsVertical).Count() == decimal.Zero)
                            {
                                item.Error += "是否纵向必须是：【是】或者【否】!";
                                continue;
                            }
                        }
                        if (item.ImportOptionList.Count > decimal.Zero)
                        {
                            var errorStr = "";
                            foreach (var a in item.ImportOptionList)
                            {
                                if (string.IsNullOrEmpty(a.SurveyOptionValue))
                                {
                                    a.SurveyOptionValue = "0";
                                }
                                else
                                {
                                    try
                                    {
                                        var value = Convert.ToDecimal(a.SurveyOptionValue);
                                    }
                                    catch
                                    {
                                        errorStr = errorStr + "【" + a.SurveyColumnName + "】的分数必须是正确的数字格式!";
                                        continue;
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(errorStr) == false)
                            {
                                item.Error += errorStr;
                                continue;
                            }
                        }
                        if (vm.IsUpdate)
                        {
                            if (SurveyItemList.Where(d => d.tbSurveyGroup.SurveyGroupName == item.SurveyGroupName && d.SurveyItemName == item.SurveyItemName).Count() > 1)
                            {
                                item.Error += "系统中该评价内容数据存在重复，无法确认需要更新的记录!";
                                continue;
                            }
                        }
                        else
                        {
                            if (SurveyItemList.Where(d => d.tbSurveyGroup.SurveyGroupName == item.SurveyGroupName && d.SurveyItemName == item.SurveyItemName).Count() > 0)
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
                    var addSurveyItemList = new List<Entity.tbSurveyItem>();
                    var addSurveyOptionList = new List<Entity.tbSurveyOption>();
                    foreach (var item in vm.ImportList)
                    {
                        Entity.tbSurveyItem tb = null;
                        if (SurveyItemList.Where(d => d.tbSurveyGroup.SurveyGroupName == item.SurveyGroupName && d.SurveyItemName == item.SurveyItemName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                tb = SurveyItemList.Where(d => d.tbSurveyGroup.SurveyGroupName == item.SurveyGroupName && d.SurveyItemName == item.SurveyItemName).FirstOrDefault();
                                tb.No = item.No.ConvertToInt();
                                tb.SurveyItemName = item.SurveyItemName;
                                #region 添加外键
                                if (!string.IsNullOrEmpty(item.SurveyGroupName))
                                {
                                    tb.tbSurveyGroup = SurveyGroup.Where(d => d.SurveyGroupName == item.SurveyGroupName).FirstOrDefault();
                                }
                                #endregion
                                tb.IsVertical = item.IsVertical == "是" ? true : false;
                                if (item.SurveyItemType == "单选题")
                                {
                                    tb.SurveyItemType = Code.EnumHelper.SurveyItemType.Radio;
                                }
                                else if (item.SurveyItemType == "多选题")
                                {
                                    tb.SurveyItemType = Code.EnumHelper.SurveyItemType.CheckBox;
                                }
                                else if (item.SurveyItemType == "问答题")
                                {
                                    tb.SurveyItemType = Code.EnumHelper.SurveyItemType.TextBox;
                                }
                                else
                                {
                                    tb.SurveyItemType = Code.EnumHelper.SurveyItemType.Radio;
                                }
                            }
                        }
                        else
                        {
                            tb = new Entity.tbSurveyItem();
                            tb.No = item.No.ConvertToInt();
                            tb.SurveyItemName = item.SurveyItemName;
                            if (!string.IsNullOrEmpty(item.SurveyGroupName))
                            {
                                tb.tbSurveyGroup = SurveyGroup.Where(d => d.SurveyGroupName == item.SurveyGroupName).FirstOrDefault();
                            }
                            tb.IsVertical = item.IsVertical == "是" ? true : false;
                            if (item.SurveyItemType == "单选题")
                            {
                                tb.SurveyItemType = Code.EnumHelper.SurveyItemType.Radio;
                            }
                            else if (item.SurveyItemType == "多选题")
                            {
                                tb.SurveyItemType = Code.EnumHelper.SurveyItemType.CheckBox;
                            }
                            else if (item.SurveyItemType == "问答题")
                            {
                                tb.SurveyItemType = Code.EnumHelper.SurveyItemType.TextBox;
                            }
                            else
                            {
                                tb.SurveyItemType = Code.EnumHelper.SurveyItemType.Radio;
                            }
                            addSurveyItemList.Add(tb);
                        }
                        var index = 0;
                        foreach (var option in item.ImportOptionList)
                        {
                            if (item.SurveyItemType == "问答" && index == 1)
                            {
                                break;
                            }
                            if (string.IsNullOrEmpty(option.SurveyOptionName) == false)
                            {
                                if (SurveyOptionList.Where(d => d.tbSurveyItem.tbSurveyGroup.SurveyGroupName == item.SurveyGroupName && d.tbSurveyItem.SurveyItemName == item.SurveyItemName && d.OptionName == option.SurveyOptionName).Count() == decimal.Zero)
                                {
                                    index++;
                                    var tbOption = new Entity.tbSurveyOption();
                                    tbOption.OptionName = option.SurveyOptionName;
                                    tbOption.tbSurveyItem = tb;
                                    tbOption.OptionValue = option.SurveyOptionValue.ConvertToDecimal();
                                    tbOption.No = index;
                                    addSurveyOptionList.Add(tbOption);
                                }
                            }
                        }
                    }
                    db.Set<Entity.tbSurveyItem>().AddRange(addSurveyItemList);
                    db.Set<Entity.tbSurveyOption>().AddRange(addSurveyOptionList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了评价内容");
                        vm.Status = true;
                    }
                    #endregion
                }
            }

            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        private static List<Dto.SurveyItem.ImportOption> GetImportOptionList(DataRow dr, List<string> listColum)
        {
            var ImportOptionList = new List<Dto.SurveyItem.ImportOption>();
            foreach (var item in listColum.Where(d => d.StartsWith("项目")))
            {
                var ImportOption = new Dto.SurveyItem.ImportOption();
                ImportOption.SurveyColumnName = item;
                ImportOption.SurveyOptionName = dr[item].ToString();
                var valueStr = item.Replace("项目", "分数");
                try
                {
                    ImportOption.SurveyOptionValue = dr[valueStr].ToString();
                }
                catch
                {
                    ImportOption.SurveyOptionValue = decimal.Zero.ToString();
                }
                if (!ImportOptionList.Contains(ImportOption) && string.IsNullOrEmpty(ImportOption.SurveyOptionName.ConvertToString()) == false)
                {
                    ImportOptionList.Add(ImportOption);
                }
            }

            return ImportOptionList;
        }

        public ActionResult Export(int surveyId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                var tb = (from p in db.Table<Entity.tbSurveyItem>()
                          where p.tbSurveyGroup.tbSurvey.Id == surveyId
                              && p.tbSurveyGroup.IsDeleted == false
                              && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                          orderby p.tbSurveyGroup.No, p.No
                          select new
                          {
                              p.Id,
                              p.No,
                              p.SurveyItemName,
                              p.tbSurveyGroup.SurveyGroupName,
                              p.IsVertical,
                              p.SurveyItemType,
                              p.TextMaxLength
                          }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("排序"),
                        new System.Data.DataColumn("评教分组"),
                        new System.Data.DataColumn("评价内容"),
                        new System.Data.DataColumn("选项布局"),
                        new System.Data.DataColumn("试题类型"),
                        new System.Data.DataColumn("字数限制")
                    });
                foreach (var a in tb)
                {
                    var dr = dt.NewRow();
                    dr["排序"] = a.No;
                    dr["评价内容"] = a.SurveyItemName;
                    dr["评教分组"] = a.SurveyGroupName;
                    dr["选项布局"] = a.IsVertical ? "纵向" : "横向";
                    dr["试题类型"] = a.SurveyItemType.GetDescription();
                    dr["字数限制"] = a.TextMaxLength.ToString();
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