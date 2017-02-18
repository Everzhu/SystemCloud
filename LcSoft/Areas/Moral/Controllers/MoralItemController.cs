using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralItemController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralItem.List();
                var moral = db.Table<Moral.Entity.tbMoral>().FirstOrDefault(d => d.Id == vm.MoralId);
                if (moral == null)
                {
                    moral = (from p in db.Table<Moral.Entity.tbMoral>() select p).FirstOrDefault();
                }
                if (moral == null)
                {
                    return RedirectToAction("List", "Moral");
                }
                vm.MoralId = moral.Id;
                vm.MoralName = moral.MoralName;
                vm.MoralType = moral.MoralType;

                var tb = from p in db.Table<Moral.Entity.tbMoralItem>()
                         where p.tbMoralGroup.tbMoral.Id == vm.MoralId
                         select p;
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(p => p.MoralItemName.Contains(vm.SearchText));
                }
                if (vm.MoralGroupId.HasValue && vm.MoralGroupId.Value > 0)
                {
                    tb = tb.Where(p => p.tbMoralGroup.Id == vm.MoralGroupId.Value);
                }

                if (vm.MoralKindId.HasValue && vm.MoralKindId.Value > -1)
                {
                    tb = tb.Where(p => (int)p.MoralItemKind == vm.MoralKindId);
                }

                vm.MoralItemList = (from p in tb
                                    orderby p.tbMoralGroup.Id, p.MoralItemName
                                    select new Dto.MoralItem.List()
                                    {
                                        Id = p.Id,
                                        No = p.No,
                                        MoralItemName = p.MoralItemName,
                                        InitScore = p.InitScore,
                                        MaxScore = p.MaxScore,
                                        MinScore = p.MinScore,
                                        DefaultValue = p.DefaultValue,
                                        MoralExpress = p.MoralExpress,
                                        MoralItemKind = p.MoralItemKind,
                                        MoralGroupName = p.tbMoralGroup.MoralGroupName,
                                        MoralItemOperateType = p.MoralItemOperateType,
                                        MoralItemType = p.MoralItemType,
                                        AutoCheck = p.AutoCheck
                                    }).ToList();
                vm.MoralGroupList = MoralGroupController.SelectList(vm.MoralId);

                vm.MoralKindList = typeof(Code.EnumHelper.MoralItemKind).ToItemList();
                vm.MoralKindList.Insert(0, new SelectListItem() { Text = "评价对象", Value = "-1" });
                return View(vm);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.MoralItem.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                moralId = vm.MoralId,
                moralGroupId = vm.MoralGroupId,
                searchText = vm.SearchText,
                moralKindId = vm.MoralKindId
            }));
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.MoralItem.Edit();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.MoralType = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId).MoralType;

                if (id > 0)
                {
                    var tb = (from p in db.Table<Moral.Entity.tbMoralItem>()
                              where p.Id == id
                              select new Dto.MoralItem.Edit()
                              {
                                  Id = p.Id,
                                  InitScore = p.InitScore,
                                  MaxScore = p.MaxScore,
                                  MinScore = p.MinScore,
                                  DefaultValue = p.DefaultValue,
                                  MoralExpress = p.MoralExpress,
                                  MoralItemName = p.MoralItemName,
                                  MoralItemType = p.MoralItemType,
                                  MoralItemKind = p.MoralItemKind,
                                  MoralItemOperateType = p.MoralItemOperateType,
                                  No = p.No,
                                  MoralGroupId = p.tbMoralGroup.Id,
                                  AutoCheck = p.AutoCheck
                              }).FirstOrDefault();

                    if (tb != null)
                    {
                        vm.MoralItemEdit = tb;
                        if (vm.MoralType != Code.EnumHelper.MoralType.Many)
                        {
                            vm.MoralOptionList = MoralOptionController.SelectList(vm.MoralItemEdit.Id);
                        }
                    }
                }
                else
                {
                    vm.MoralItemEdit.No = db.Table<Moral.Entity.tbMoralItem>().Where(p => p.tbMoralGroup.tbMoral.Id == vm.MoralId).Select(p => p.No).DefaultIfEmpty(0).Max() + 1;
                    vm.MoralItemEdit.MoralItemType = Code.EnumHelper.MoralItemType.Select;
                    vm.MoralItemEdit.MoralItemOperateType = Code.EnumHelper.MoralItemOperateType.Score;
                }

            }
            vm.MoralGroupList = MoralGroupController.SelectList(vm.MoralId);
            vm.MoralItemKindList = typeof(Code.EnumHelper.MoralItemKind).ToItemList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.MoralItem.Edit vm)
        {
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                var tbMoralGroup = (from p in db.Set<Moral.Entity.tbMoralGroup>().Include(p => p.tbMoral) where p.Id == vm.MoralItemEdit.MoralGroupId select new { p, p.tbMoral }).FirstOrDefault();
                if (vm.MoralItemEdit.MoralItemOperateType == Code.EnumHelper.MoralItemOperateType.Score)
                {
                    if (vm.MoralItemEdit.MaxScore <= vm.MoralItemEdit.MinScore)
                    {
                        error.AddError("最大分值必须大于最小分值！");
                        return Code.MvcHelper.Post(error);
                    }

                    //var tbMoralGroup = (from p in db.Set<Moral.Entity.tbMoralGroup>().Include(p => p.tbMoral) where p.Id == vm.MoralItemEdit.MoralGroupId select new { p, p.tbMoral }).FirstOrDefault();

                    if (tbMoralGroup.tbMoral.MoralType != Code.EnumHelper.MoralType.Many)
                    {
                        if (vm.MoralItemEdit.InitScore < vm.MoralItemEdit.MinScore || vm.MoralItemEdit.InitScore > vm.MoralItemEdit.MaxScore)
                        {
                            error.AddError("初始分分必须介于最小分和最大分之间！");
                            return Code.MvcHelper.Post(error);
                        }
                    }

                    if (tbMoralGroup.tbMoral.MoralType == Code.EnumHelper.MoralType.Many)
                    {
                        if (vm.MoralItemEdit.DefaultValue > vm.MoralItemEdit.MaxScore || vm.MoralItemEdit.DefaultValue < vm.MoralItemEdit.MinScore)
                        {
                            error.AddError("基础分必须介于最小分和最大分之间！");
                            return Code.MvcHelper.Post(error);
                        }
                    }
                }

                var isExists = (db.Table<Moral.Entity.tbMoralItem>()
                    .Where(t => t.MoralItemName == vm.MoralItemEdit.MoralItemName
                        && t.tbMoralGroup.tbMoral.Id == vm.MoralId
                        && t.tbMoralGroup.Id == vm.MoralItemEdit.MoralGroupId
                        && t.Id != vm.MoralItemEdit.Id)
                    ).Any();
                if (isExists)
                {
                    error.AddError("系统中已存在相同项目名称的记录！");
                    return Code.MvcHelper.Post(error);
                }

                var emptyString = new string[] { };
                var ids = Request["txtId"]?.Split(',') ?? emptyString;
                var nos = Request["txtNo"]?.Split(',') ?? emptyString;
                var names = Request["txtMoralOptionName"]?.Split(',') ?? emptyString;
                var values = Request["txtMoralOptionValue"]?.Split(',') ?? emptyString;

                //单次模式，如果选项类型是下拉框，判断是否有设置选项
                if (tbMoralGroup.tbMoral.MoralType != Code.EnumHelper.MoralType.Many && vm.MoralItemEdit.MoralItemType == Code.EnumHelper.MoralItemType.Select)
                {
                    if (names.Count() == decimal.Zero || names.Count(p => !string.IsNullOrWhiteSpace(p)) == 0)
                    {
                        error.AddError("至少应添加一个 [选项名称] 不为空的选项数据！");
                        return Code.MvcHelper.Post(error);
                    }
                }

                Moral.Entity.tbMoralItem tbMoralItem = null;
                if (vm.MoralItemEdit.Id > 0)
                {
                    tbMoralItem = db.Set<Moral.Entity.tbMoralItem>().Find(vm.MoralItemEdit.Id);
                    if (tbMoralItem == null)
                    {
                        error.AddError(Resources.LocalizedText.MsgNotFound);
                        return Code.MvcHelper.Post(error);
                    }
                }
                else
                {
                    tbMoralItem = new Moral.Entity.tbMoralItem();
                }

                tbMoralItem.InitScore = vm.MoralItemEdit.InitScore;
                tbMoralItem.MaxScore = vm.MoralItemEdit.MaxScore;
                tbMoralItem.MinScore = vm.MoralItemEdit.MinScore;
                tbMoralItem.DefaultValue = vm.MoralItemEdit.DefaultValue;
                tbMoralItem.MoralExpress = vm.MoralItemEdit.MoralExpress;
                tbMoralItem.MoralItemName = vm.MoralItemEdit.MoralItemName;
                tbMoralItem.MoralItemType = vm.MoralItemEdit.MoralItemType;
                tbMoralItem.MoralItemKind = vm.MoralItemEdit.MoralItemKind;
                tbMoralItem.MoralItemOperateType = vm.MoralItemEdit.MoralItemOperateType;
                tbMoralItem.tbMoralGroup = tbMoralGroup.p;
                tbMoralItem.AutoCheck = vm.MoralItemEdit.AutoCheck;
                tbMoralItem.No = (vm.MoralItemEdit.No ?? 0) == 0 ? db.Table<Moral.Entity.tbMoralItem>().Where(p => p.tbMoralGroup.tbMoral.Id == vm.MoralId).Select(p => p.No).DefaultIfEmpty(0).Max() + 1 : vm.MoralItemEdit.No.Value;

                if (vm.MoralItemEdit.Id == 0)
                {
                    db.Set<Moral.Entity.tbMoralItem>().Add(tbMoralItem);
                }


                //编辑的时候，改变操作方式时，删除原有方式下的德育数据
                if (vm.MoralItemEdit.Id > 0)
                {
                    var tbMoralData = (from p in db.Table<Entity.tbMoralData>() where p.tbMoralItem.Id == vm.MoralItemEdit.Id && p.MoralItemOperateType != vm.MoralItemEdit.MoralItemOperateType select p);
                    foreach (var item in tbMoralData)
                    {
                        item.IsDeleted = true;
                    }
                }

                #region tbMoralOption

                if (tbMoralGroup.tbMoral.MoralType != Code.EnumHelper.MoralType.Many)
                {
                    var oldIds = ids.Where(p => !string.IsNullOrWhiteSpace(p)).Select(int.Parse);
                    //var optionList = (from p in db.Table<Moral.Entity.tbMoralOption>() where  p.tbMoralItem.Id == tbMoralItem.Id && !oldIds.Contains(p.Id) select p);
                    var optionList = (from p in db.Table<Moral.Entity.tbMoralOption>() where p.tbMoralItem.Id == tbMoralItem.Id select p);

                    if (vm.MoralItemEdit.MoralItemType == Code.EnumHelper.MoralItemType.Text)
                    {
                        foreach (var option in optionList)
                        {
                            option.IsDeleted = true;
                        }
                    }
                    else
                    {
                        for (var i = 0; i < ids.Length; i++)
                        {
                            var _OptionId = ids[i].ConvertToInt();
                            var _No = nos[i].ConvertToInt();
                            var _Name = names[i];
                            if (string.IsNullOrWhiteSpace(_Name))
                            {
                                continue;
                            }
                            var _Value = values[i];

                            if (_Value.ConvertToDecimal() > tbMoralItem.MaxScore)
                            {
                                error.Add("选项的分数不能超过项目的最大分数！");
                                break;
                            }
                            if (_Value.ConvertToDecimal() <= decimal.Zero)
                            {
                                error.Add("选项的分数不能为0！");
                                break;
                            }

                            if (_OptionId > 0)
                            {
                                //var option = db.Set<Moral.Entity.tbMoralOption>().Find(_OptionId.ConvertToInt());
                                var option = optionList.First(p => p.Id == _OptionId);
                                option.No = _No == 0 ? db.Table<Moral.Entity.tbMoralOption>().Where(p => p.tbMoralItem.Id == tbMoralItem.Id).Select(p => p.No).DefaultIfEmpty(0).Max() + 1 : _No;
                                option.MoralOptionName = _Name;
                                option.MoralOptionValue = _Value.ConvertToDecimal();
                            }
                            else
                            {
                                var option = new Moral.Entity.tbMoralOption()
                                {
                                    No = _No == 0 ? db.Table<Moral.Entity.tbMoralOption>().Where(p => p.tbMoralItem.Id == tbMoralItem.Id).Select(p => p.No).DefaultIfEmpty(0).Max() + 1 : _No,
                                    MoralOptionName = _Name,
                                    MoralOptionValue = _Value.ConvertToDecimal(),
                                    tbMoralItem = tbMoralItem
                                };
                                db.Set<Moral.Entity.tbMoralOption>().Add(option);
                            }
                        }
                    }
                }
                #endregion

                if (error != null && error.Any())
                {
                    return Code.MvcHelper.Post(error);
                }
                else
                {
                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert($"{(vm.MoralItemEdit.Id == 0 ? "添加" : "修改")}了德育项目！");
                    }
                }
                return Code.MvcHelper.Post();
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Moral.Entity.tbMoralItem>() where ids.Contains(p.Id) select p);
                foreach (var item in tb)
                {
                    item.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    Sys.Controllers.SysUserLogController.Insert("删除了德育项目！");
                }
                return Code.MvcHelper.Post();
            }
        }


        public ActionResult Import()
        {
            var vm = new Models.MoralItem.Import();
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.MoralItem.Import vm)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);
                using (var db = new XkSystem.Models.DbContext())
                {
                    var ExList = new List<string>() { ".xlsx" };
                    if (!ExList.Contains(System.IO.Path.GetExtension(file.FileName)))
                    {
                        ModelState.AddModelError(string.Empty, "上传的文件不是正确的excel文件!");
                        return View(vm);
                    }
                    else
                    {
                        var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), string.Empty);
                        if (dt == null)
                        {
                            ModelState.AddModelError(string.Empty, "无法读取上传的文件，请检查文件格式是否正确!");
                            return View(vm);
                        }
                        else
                        {
                            var tbList = new List<string>() { "项目名称", "德育分组", "最小分值", "最大分值", "初始分值", "选项类型", "表达式", "德育选项" };
                            foreach (var name in tbList)
                            {
                                var text = string.Empty;
                                text += !dt.Columns.Contains(name) ? name + "," : "";
                                if (!string.IsNullOrWhiteSpace(text))
                                {
                                    ModelState.AddModelError(string.Empty, "上传的excel文件内容与预期不一致!错误详细:" + text);
                                    return View(vm);
                                }
                            }
                            if (dt.Rows.Count == 0)
                            {
                                ModelState.AddModelError(string.Empty, "上传的文件不包含任何有效数据!");
                                return View(vm);
                            }

                            var listMoralGroup = (from p in db.Table<Moral.Entity.tbMoralGroup>().Include(p => p.tbMoral) where p.tbMoral.Id == vm.MoralId select p).ToList();
                            var listMoralItem = (from p in db.Table<Moral.Entity.tbMoralItem>().Include(p => p.tbMoralGroup) where p.tbMoralGroup.tbMoral.Id == vm.MoralId select p).ToList();
                            var moralExpressList = typeof(Code.EnumHelper.MoralExpress).ToItemList();
                            var moralItemTypeList = typeof(Code.EnumHelper.MoralItemType).ToItemList();

                            for (var i = 0; i < dt.Rows.Count; i++)
                            {
                                var dr = dt.Rows[i];
                                var importModel = new Dto.MoralItem.Import()
                                {
                                    MoralItemName = dr["项目名称"].ConvertToString(),
                                    MoralGroupName = dr["德育分组"].ConvertToString(),
                                    MinScore = dr["最小分值"].ConvertToString(),
                                    MaxScore = dr["最大分值"].ConvertToString(),
                                    InitScore = dr["初始分值"].ConvertToString(),
                                    MoralExpress = dr["表达式"].ConvertToString(),
                                    MoralItemType = dr["选项类型"].ConvertToString(),
                                    MoralOptionString = dr["德育选项"].ConvertToString(),
                                };

                                if (string.IsNullOrWhiteSpace(importModel.MoralItemName))
                                {
                                    importModel.ImportError += "项目名称为空;";
                                }
                                else
                                {
                                    if (listMoralItem.Count(p => p.tbMoralGroup.tbMoral.Id == vm.MoralId && p.MoralItemName.Equals(importModel.MoralItemName)) > 0)
                                    {
                                        importModel.ImportError += "系统中已存在相同名称的记录;";
                                    }
                                }
                                if (string.IsNullOrWhiteSpace(importModel.MoralGroupName))
                                {
                                    importModel.ImportError += "德育分组为空;";
                                }
                                else
                                {
                                    if (listMoralGroup.Count(p => p.tbMoral.Id == vm.MoralId && p.MoralGroupName.Equals(importModel.MoralGroupName)) == 0)
                                    {
                                        importModel.ImportError += "找不到相应的德育分组;";
                                    }
                                }

                                if (importModel.MinScore.ConvertToDecimal() <= decimal.Zero)
                                {
                                    importModel.ImportError += "最小分值不合法或小于等于0;";
                                }
                                if (importModel.MaxScore.ConvertToDecimal() <= decimal.Zero)
                                {
                                    importModel.ImportError += "最大分值不合法或小于等于0;";
                                }

                                if (importModel.InitScore.ConvertToDecimal() < importModel.MinScore.ConvertToDecimal() || importModel.InitScore.ConvertToDecimal() > importModel.MaxScore.ConvertToDecimal())
                                {
                                    importModel.ImportError += "初始分值必须介于最大最小分值之间;";
                                }


                                if (string.IsNullOrWhiteSpace(importModel.MoralExpress) || !moralExpressList.Exists(p => p.Text.Equals(importModel.MoralExpress)))
                                {
                                    importModel.ImportError += $"表达式值只能是 [{string.Join("、", moralExpressList.Select(p => p.Text).ToList())}]中的一种;";
                                }


                                if (string.IsNullOrWhiteSpace(importModel.MoralItemType) || !moralItemTypeList.Exists(p => p.Text.Equals(importModel.MoralItemType)))
                                {
                                    importModel.ImportError += $"选项类型值只能是[{string.Join("、", moralItemTypeList.Select(p => p.Text).ToList())}]中的一种;";
                                }
                                else
                                {
                                    Code.EnumHelper.MoralItemType _moralItemType = (Code.EnumHelper.MoralItemType)System.Enum.Parse(typeof(Code.EnumHelper.MoralItemType), moralItemTypeList.Where(p => p.Text.Equals(importModel.MoralItemType)).Select(p => p.Value).First());
                                    if (_moralItemType == Code.EnumHelper.MoralItemType.Select)
                                    {
                                        if (string.IsNullOrWhiteSpace(importModel.MoralOptionString))
                                        {
                                            importModel.ImportError += $"选项类型为{_moralItemType.GetDescription()}时德育选项不能为空;";
                                        }
                                        else
                                        {
                                            var optionList = importModel.MoralOptionString.Split('|').ToList();
                                            foreach (var p in optionList)
                                            {
                                                var option = p.Split(',');
                                                if (option == null || !option.Any() || option.Length != 2)
                                                {
                                                    importModel.ImportError += "德育选项值不正确;";
                                                    break;
                                                }
                                                else if (option[1].ConvertToDecimal() < importModel.MinScore.ConvertToDecimal() || option[1].ConvertToDecimal() > importModel.MaxScore.ConvertToDecimal())
                                                {
                                                    importModel.ImportError += "德育选项中的值必须介于最小分值和最大分值之间;";
                                                    break;
                                                }
                                            }


                                        }
                                    }
                                }

                                vm.ImportList.Add(importModel);
                            }

                            if (vm.ImportList.Count(p => !string.IsNullOrWhiteSpace(p.ImportError)) > 0)
                            {
                                vm.ImportList.RemoveAll(p => string.IsNullOrWhiteSpace(p.ImportError));
                                return View(vm);
                            }


                            var groupByItemName = vm.ImportList.GroupBy(p => p.MoralItemName).Where(p => p.Count() > 1).Select(p => p.Key).ToList();
                            if (groupByItemName != null && groupByItemName.Any())
                            {
                                vm.ImportList.Where(p => groupByItemName.Contains(p.MoralItemName)).ToList().ForEach(p =>
                                {
                                    p.ImportError += "项目名称重复;";
                                });
                            }

                            if (vm.ImportList.Count(p => !string.IsNullOrWhiteSpace(p.ImportError)) > 0)
                            {
                                vm.ImportList.RemoveAll(p => string.IsNullOrWhiteSpace(p.ImportError));
                                return View(vm);
                            }


                            var list = new List<Moral.Entity.tbMoralItem>();
                            list.AddRange(vm.ImportList.Select(p => new Moral.Entity.tbMoralItem()
                            {
                                MoralItemName = p.MoralItemName,
                                MoralItemType = (Code.EnumHelper.MoralItemType)System.Enum.Parse(typeof(Code.EnumHelper.MoralItemType), moralItemTypeList.Where(t => t.Text.Equals(p.MoralItemType)).Select(t => t.Value).First()),
                                MoralExpress = (Code.EnumHelper.MoralExpress)System.Enum.Parse(typeof(Code.EnumHelper.MoralExpress), moralExpressList.Where(e => e.Text.Equals(p.MoralExpress)).Select(e => e.Value).First()),
                                InitScore = p.InitScore.ConvertToDecimal(),
                                MaxScore = p.MaxScore.ConvertToDecimal(),
                                MinScore = p.MinScore.ConvertToDecimal(),
                                tbMoralGroup = listMoralGroup.FirstOrDefault(g => g.MoralGroupName.Equals(p.MoralGroupName)),
                                No = db.Table<Moral.Entity.tbMoralItem>().Where(i => i.tbMoralGroup.tbMoral.Id == vm.MoralId && i.tbMoralGroup.MoralGroupName.Equals(p.MoralGroupName)).Select(g => g.No).DefaultIfEmpty(0).Max() + 1,

                                MoralItemKind = Code.EnumHelper.MoralItemKind.Student,
                                DefaultValue = decimal.Zero
                            }));

                            db.Set<Moral.Entity.tbMoralItem>().AddRange(list);

                            var listMoralOption = new List<Moral.Entity.tbMoralOption>();
                            list.Where(p => p.MoralItemType == Code.EnumHelper.MoralItemType.Select).ToList().ForEach(p =>
                            {
                                var optionList = vm.ImportList.FirstOrDefault(l => l.MoralItemName.Equals(p.MoralItemName)).MoralOptionString.Split('|');
                                listMoralOption.AddRange(optionList.Select(o => new Moral.Entity.tbMoralOption()
                                {
                                    No = db.Table<Moral.Entity.tbMoralOption>().Where(i => i.tbMoralItem.MoralItemName.Equals(p.MoralItemName)).Select(g => g.No).DefaultIfEmpty(0).Max() + 1,
                                    MoralOptionName = o.Split(',')[0],
                                    MoralOptionValue = o.Split(',')[1].ConvertToDecimal(),
                                    tbMoralItem = p
                                }).ToList());
                            });

                            db.Set<Moral.Entity.tbMoralOption>().AddRange(listMoralOption);

                            if (db.SaveChanges() > 0)
                            {
                                Sys.Controllers.SysUserLogController.Insert("批量导入了德育项目！");
                                vm.ImportList.RemoveAll(p => string.IsNullOrWhiteSpace(p.ImportError));
                                vm.Status = true;
                            }
                        }
                    }
                }
            }
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Moral/Views/MoralItem/Import.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }


        [NonAction]
        internal static List<Dto.MoralItem.List> SelectList(int moralGroupId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Moral.Entity.tbMoralItem>() select p);
                if (moralGroupId > 0)
                {
                    tb = tb.Where(p => p.tbMoralGroup.Id == moralGroupId);
                }
                return (from p in tb
                        select new Dto.MoralItem.List()
                        {
                            Id = p.Id,
                            No = p.No,
                            MoralItemName = p.MoralItemName,
                            InitScore = p.InitScore,
                            MaxScore = p.MaxScore,
                            MinScore = p.MinScore,
                            DefaultValue = p.DefaultValue,
                            MoralExpress = p.MoralExpress,
                            MoralGroupId = p.tbMoralGroup.Id,
                            MoralGroupName = p.tbMoralGroup.MoralGroupName,
                            MoralItemType = p.MoralItemType
                        }).ToList();
            }
        }


        [NonAction]
        internal static List<Dto.MoralItem.Info> SelectListByMoralId(int moralId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return (from p in db.Table<Moral.Entity.tbMoralItem>()
                        where p.tbMoralGroup.tbMoral.Id == moralId
                        select new Dto.MoralItem.Info()
                        {
                            Id = p.Id,
                            MoralItemName = p.MoralItemName,
                            MoralItemKind = p.MoralItemKind,
                            MoralGroupId = p.tbMoralGroup.Id,
                            DefaultValue = p.DefaultValue,
                            MaxScore = p.MaxScore,
                            MinScore = p.MinScore,
                            InitScore = p.InitScore
                        }).ToList();
            }
        }

        [NonAction]
        internal static List<Dto.MoralItem.Info> SelectListByMoralIdAndKind(int moralId, Code.EnumHelper.MoralItemKind moralItemKind)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return (from p in db.Table<Moral.Entity.tbMoralItem>()
                        where p.tbMoralGroup.tbMoral.Id == moralId && p.MoralItemKind == moralItemKind
                        select new Dto.MoralItem.Info()
                        {
                            Id = p.Id,
                            MoralItemName = p.MoralItemName,
                            MoralGroupId = p.tbMoralGroup.Id,
                            DefaultValue = p.DefaultValue,
                            MaxScore = p.MaxScore,
                            MinScore = p.MinScore,
                            InitScore = p.InitScore
                        }).ToList();
            }
        }

        [NonAction]
        internal static IEnumerable<Dto.MoralItem.List> SelectList(int groupId, Code.EnumHelper.MoralItemKind moralItemKind)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return (from p in db.Table<Moral.Entity.tbMoralItem>()
                        where p.tbMoralGroup.Id == groupId && p.MoralItemKind == moralItemKind
                        select new Dto.MoralItem.List()
                        {
                            Id = p.Id,
                            MoralItemOperateType = p.MoralItemOperateType,
                            MoralGroupId = p.tbMoralGroup.Id,
                            DefaultValue = p.DefaultValue,
                            MoralItemName = p.MoralItemName,
                        }).ToList();
            }
        }
    }

}