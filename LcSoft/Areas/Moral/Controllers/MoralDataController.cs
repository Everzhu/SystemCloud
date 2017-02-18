using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralDataController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralData.List();

                var tb = (from p in db.Table<Moral.Entity.tbMoralData>() where p.CheckStatus == Code.EnumHelper.CheckStatus.Success select p);
                if (vm.MoralId.HasValue && vm.MoralId.Value > 0)
                {
                    tb = tb.Where(p => p.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.MoralId.Value);
                }
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(p => p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText));
                }

                if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator)
                {
                    tb = tb.Where(p => p.tbSysUser.Id == Code.Common.UserId);
                }


                vm.MoralDataList = (from p in tb
                                    orderby p.InputDate descending
                                    select new Dto.MoralData.List()
                                    {
                                        Id = p.Id,
                                        No = p.No,
                                        DataText = p.DataText,
                                        InputDate = p.InputDate,
                                        MoralDate = p.MoralDate,
                                        MoralItemId = p.tbMoralItem.Id,
                                        MoralId = p.tbMoralItem.tbMoralGroup.tbMoral.Id,
                                        MoralItemName = p.tbMoralItem.MoralItemName,
                                        MoralOptionId = p.tbMoralOption != null ? p.tbMoralOption.Id : 0,
                                        MoralOptionName = p.tbMoralOption != null ? p.tbMoralOption.MoralOptionName : "-",
                                        MoralOptionScore = p.tbMoralOption != null ? p.tbMoralOption.MoralOptionValue : Decimal.Zero,
                                        StudentGroupName = p.tbClassGroup != null ? p.tbClassGroup.ClassGroupName : "-",
                                        ClassName = p.tbClass != null ? p.tbClass.ClassName : "-",
                                        StudentName = p.tbStudent != null ? p.tbStudent.StudentName : "-",
                                        SysUserName = p.tbSysUser.UserName
                                    }).ToPageList(vm.Page);
                //vm.MoralList = MoralController.SelectList(Code.EnumHelper.MoralType.Once);

                vm.MoralList = MoralController.SelectList();



                //vm.MoralClassList
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.MoralData.List vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("List", new
            {
                searchText = vm.SearchText,
                moralId = vm.MoralId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult Check()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralData.List();

                var tb = (from p in db.Table<Moral.Entity.tbMoralData>() /*where p.tbMoralItem.tbMoralGroup.tbMoral.MoralType == Code.EnumHelper.MoralType.Once*/ select p);
                if (vm.MoralId.HasValue && vm.MoralId.Value > 0)
                {
                    tb = tb.Where(p => p.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.MoralId.Value);
                }
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(p => p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText));
                }

                if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator)
                {
                    tb = tb.Where(p => p.tbSysUser.Id == Code.Common.UserId);
                }


                vm.MoralDataList = (from p in tb
                                    orderby p.InputDate descending
                                    select new Dto.MoralData.List()
                                    {
                                        Id = p.Id,
                                        No = p.No,
                                        DataText = p.DataText,
                                        MoralName = p.tbMoralItem.tbMoralGroup.tbMoral.MoralName,
                                        InputDate = p.InputDate,
                                        MoralDate = p.MoralDate,
                                        MoralItemId = p.tbMoralItem.Id,
                                        MoralId = p.tbMoralItem.tbMoralGroup.tbMoral.Id,
                                        MoralItemName = p.tbMoralItem.MoralItemName,
                                        MoralOptionId = p.tbMoralOption != null ? p.tbMoralOption.Id : 0,
                                        MoralOptionName = p.tbMoralOption != null ? p.tbMoralOption.MoralOptionName : "-",
                                        MoralOptionScore = p.tbMoralOption != null ? p.tbMoralOption.MoralOptionValue : Decimal.Zero,
                                        StudentGroupName = p.tbClassGroup != null ? p.tbClassGroup.ClassGroupName : "-",
                                        ClassName = p.tbClass != null ? p.tbClass.ClassName : "-",
                                        StudentName = p.tbStudent != null ? p.tbStudent.StudentName : "-",
                                        SysUserName = p.tbSysUser.UserName,
                                        CheckStatus = p.CheckStatus
                                    }).ToPageList(vm.Page);
                //vm.MoralList = MoralController.SelectList(Code.EnumHelper.MoralType.Once);

                vm.MoralList = MoralController.SelectList();



                //vm.MoralClassList
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetCheck(List<int> ids, int operate)
        {
            if (ids == null || !ids.Any() || !Enum.IsDefined(typeof(Code.EnumHelper.CheckStatus), operate))
            {
                return Code.MvcHelper.Post();
            }
            using (var db = new XkSystem.Models.DbContext())
            {
                var tbMoralData = (from p in db.Table<Entity.tbMoralData>() where ids.Contains(p.Id) select p);
                foreach (var item in tbMoralData)
                {
                    item.CheckStatus = (Code.EnumHelper.CheckStatus)Enum.Parse(typeof(Code.EnumHelper.CheckStatus), operate.ToString());
                }
                if (db.SaveChanges() > 0)
                {
                    Sys.Controllers.SysUserLogController.Insert("审核了德育记录！");
                }
            }
            return Code.MvcHelper.Post();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Check(Models.MoralData.List vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("Check", new
            {
                searchText = vm.SearchText,
                moralId = vm.MoralId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }





        #region 单次模式
        public ActionResult OnceEdit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralData.OnceEdit();
                var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);

                if (moral == null)
                {
                    moral = (from p in db.Table<Moral.Entity.tbMoral>() where DateTime.Now <= p.ToDate && DateTime.Now >= p.FromDate select p).FirstOrDefault();
                }

                if (moral == null)
                {
                    vm.DataIsNull = true;
                    return View(vm);
                    //return RedirectToAction("List", "Moral");
                }

                switch (moral.MoralType)
                {
                    case Code.EnumHelper.MoralType.Many:
                        return RedirectToAction("Edit", new { MoralId = vm.MoralId });
                    case Code.EnumHelper.MoralType.Days:
                        return RedirectToAction("DayEdit", new { MoralId = vm.MoralId });
                }

                vm.MoralId = moral.Id;
                vm.MoralClassList = MoralClassController.SelectList(vm.MoralId);
                vm.MoralGroupList = MoralGroupController.GetMoralGroupInfoList(vm.MoralId);
                vm.MoralList = MoralController.SelectList();
                vm.MoralType = moral.MoralType;

                foreach (var group in vm.MoralGroupList)
                {
                    vm.MoralItemList.AddRange(MoralItemController.SelectList(group.Id));
                }
                foreach (var item in vm.MoralItemList)
                {
                    vm.MoralOptionList.AddRange(MoralOptionController.SelectList(item.Id));
                }
                var moralClassIds = (from p in db.Table<Moral.Entity.tbMoralClass>() where p.tbMoral.Id == vm.MoralId select p.tbClass.Id).ToList();
                vm.StudentList = Student.Controllers.StudentController.GetStudentInfoListByClassIds(moralClassIds);

                var moralItemIds = vm.MoralItemList.Select(p => p.Id);
                vm.MoralDataList = (from p in db.Table<Moral.Entity.tbMoralData>()
                                    where
                                        p.CheckStatus == Code.EnumHelper.CheckStatus.Success &&
                                        moralItemIds.Contains(p.tbMoralItem.Id)  /*&& vm.MoralDate == p.MoralDate*/
                                    orderby p.No
                                    select new Dto.MoralData.OnceList()
                                    {
                                        Id = p.Id,
                                        DataText = p.DataText,
                                        MoralItemId = p.tbMoralItem.Id,
                                        MoralOptionId = p.tbMoralOption != null ? p.tbMoralOption.Id : 0,
                                        StudentId = p.tbStudent.Id,
                                        StudentName = p.tbStudent.StudentName,
                                        SysUserName = p.tbSysUser.UserName
                                    }).ToList();


                return View(vm);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnceEdit(Models.MoralData.OnceEdit vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("OnceEdit", new
            {
                moralId = vm.MoralId
            }));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Models.MoralData.OnceEdit vm)
        {
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                var moralId = vm.MoralId;

                var moralItemIds = db.Table<Moral.Entity.tbMoralItem>().Where(p => p.tbMoralGroup.tbMoral.Id == vm.MoralId).Select(p => p.Id).ToList();
                var itemCount = moralItemIds.Count;

                //列数
                var arrLength = itemCount * 2 + 3;          // 4=__RequestVerificationToken+MoralId+MoralDate+StudentIds
                var list = new List<string>();
                for (var i = 2; i < arrLength; i++)
                {
                    list.Add(Request.Form[i]);
                }
                var listData = new List<Moral.Entity.tbMoralData>();
                var studentIds = list[0].Split(',').Select(int.Parse).ToList();

                for (var i = 0; i < studentIds.Count; i++)
                {
                    Moral.Entity.tbMoralData tb = null;
                    for (var j = 1; j < list.Count; j += 2)
                    {
                        tb = new Moral.Entity.tbMoralData();
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(studentIds[i]);

                        //偶数，MoralItem
                        var items = list[j].Split(',').Select(int.Parse).ToList();
                        tb.tbMoralItem = db.Set<Moral.Entity.tbMoralItem>().Find(items[i]);

                        //奇数，MoralOption或者MoralDataText
                        var options = list[j + 1].Split(',');
                        var option = options[i];

                        //值包含option前缀的为MoralOption
                        if (option.Contains("option_"))      // option_i
                        {
                            var optionId = option.Split('_')[1].ConvertToInt();
                            tb.tbMoralOption = db.Set<Moral.Entity.tbMoralOption>().Find(optionId);
                        }
                        else
                        {
                            //判断dataText是否在MinScore和MaxScore之间
                            var optionValue = option.ConvertToDecimal();
                            if (optionValue < tb.tbMoralItem.MinScore || optionValue > tb.tbMoralItem.MaxScore)
                            {
                                error.Add($"第{i + 1}行{tb.tbMoralItem.MoralItemName}的分数必须在{tb.tbMoralItem.MinScore}-{tb.tbMoralItem.MaxScore}之间！");
                            }
                            tb.DataText = option.ConvertToDecimal();
                        }
                        listData.Add(tb);
                    }
                }
                if (!error.Any())
                {
                    var tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                    listData.ForEach(p =>
                    {
                        p.InputDate = DateTime.Now;
                        //p.MoralDate = vm.MoralDate;
                        p.MoralDate = DateTime.Now;
                        p.tbSysUser = tbSysUser;
                        p.Comment = string.Empty;       //单次模式评语默认为空
                    });

                    var oldData = (from p in db.Table<Moral.Entity.tbMoralData>()
                                   where studentIds.Contains(p.tbStudent.Id) && moralItemIds.Contains(p.tbMoralItem.Id)
                                   //&& vm.MoralDate == p.MoralDate
                                   select p);
                    foreach (var item in oldData)
                    {
                        item.IsDeleted = true;
                    }

                    db.Set<Moral.Entity.tbMoralData>().AddRange(listData);

                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("录入了德育数据！");
                    }
                }

            }
            return Code.MvcHelper.Post(error, message: "保存成功！");
        }

        #endregion

        #region 每天模式
        public ActionResult DayEdit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralData.DayEdit();
                var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);

                if (moral == null)
                {
                    moral = (from p in db.Table<Moral.Entity.tbMoral>() where DateTime.Now <= p.ToDate && DateTime.Now >= p.FromDate select p).FirstOrDefault();
                }

                if (moral == null)
                {
                    vm.DataIsNull = true;
                    return View(vm);
                    //return RedirectToAction("List", "Moral");
                }

                switch (moral.MoralType)
                {
                    case Code.EnumHelper.MoralType.Many:
                        return RedirectToAction("Edit", new { MoralId = vm.MoralId });
                    case Code.EnumHelper.MoralType.Once:
                        return RedirectToAction("OnceEdit", new { MoralId = vm.MoralId });
                }

                if (vm.MoralDate == Code.DateHelper.MinDate)
                {
                    vm.MoralDate = moral.FromDate;
                }

                vm.MoralId = moral.Id;
                vm.MoralClassList = MoralClassController.SelectList(vm.MoralId);
                vm.MoralGroupList = MoralGroupController.GetMoralGroupInfoList(vm.MoralId);
                vm.MoralList = MoralController.SelectList();

                vm.MoralType = moral.MoralType;

                vm.FromDate = moral.FromDate.ToString(Code.Common.StringToDate);
                vm.ToDate = moral.ToDate.ToString(Code.Common.StringToDate);

                foreach (var group in vm.MoralGroupList)
                {
                    vm.MoralItemList.AddRange(MoralItemController.SelectList(group.Id));
                }
                foreach (var item in vm.MoralItemList)
                {
                    vm.MoralOptionList.AddRange(MoralOptionController.SelectList(item.Id));
                }
                var moralClassIds = (from p in db.Table<Moral.Entity.tbMoralClass>() where p.tbMoral.Id == vm.MoralId select p.tbClass.Id).ToList();
                vm.StudentList = Student.Controllers.StudentController.GetStudentInfoListByClassIds(moralClassIds);

                var moralItemIds = vm.MoralItemList.Select(p => p.Id);
                vm.MoralDataList = (from p in db.Table<Moral.Entity.tbMoralData>()
                                    where
                                        p.CheckStatus == Code.EnumHelper.CheckStatus.Success &&
                                        moralItemIds.Contains(p.tbMoralItem.Id) &&
                                        vm.MoralDate == p.MoralDate
                                    orderby p.No
                                    select new Dto.MoralData.OnceList()
                                    {
                                        Id = p.Id,
                                        DataText = p.DataText,
                                        MoralItemId = p.tbMoralItem.Id,
                                        MoralOptionId = p.tbMoralOption != null ? p.tbMoralOption.Id : 0,
                                        StudentId = p.tbStudent.Id,
                                        StudentName = p.tbStudent.StudentName,
                                        SysUserName = p.tbSysUser.UserName
                                    }).ToList();


                return View(vm);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveForDay(Models.MoralData.DayEdit vm)
        {
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                var moralId = vm.MoralId;

                var moralItemIds = db.Table<Moral.Entity.tbMoralItem>().Where(p => p.tbMoralGroup.tbMoral.Id == vm.MoralId).Select(p => p.Id).ToList();
                var itemCount = moralItemIds.Count;

                //列数
                var arrLength = itemCount * 2 + 4;          // 4=__RequestVerificationToken+MoralId+MoralDate+StudentIds
                var list = new List<string>();
                for (var i = 3; i < arrLength; i++)
                {
                    list.Add(Request.Form[i]);
                }
                var listData = new List<Moral.Entity.tbMoralData>();
                var studentIds = list[0].Split(',').Select(int.Parse).ToList();

                for (var i = 0; i < studentIds.Count; i++)
                {
                    Moral.Entity.tbMoralData tb = null;
                    for (var j = 1; j < list.Count; j += 2)
                    {
                        tb = new Moral.Entity.tbMoralData();
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(studentIds[i]);

                        //偶数，MoralItem
                        var items = list[j].Split(',').Select(int.Parse).ToList();
                        tb.tbMoralItem = db.Set<Moral.Entity.tbMoralItem>().Find(items[i]);

                        //奇数，MoralOption或者MoralDataText
                        var options = list[j + 1].Split(',');
                        var option = options[i];

                        //值包含option前缀的为MoralOption
                        if (option.Contains("option_"))      // option_i
                        {
                            var optionId = option.Split('_')[1].ConvertToInt();
                            tb.tbMoralOption = db.Set<Moral.Entity.tbMoralOption>().Find(optionId);
                        }
                        else
                        {
                            //判断dataText是否在MinScore和MaxScore之间
                            var optionValue = option.ConvertToDecimal();
                            if (optionValue < tb.tbMoralItem.MinScore || optionValue > tb.tbMoralItem.MaxScore)
                            {
                                error.Add($"第{i + 1}行{tb.tbMoralItem.MoralItemName}的分数必须在{tb.tbMoralItem.MinScore}-{tb.tbMoralItem.MaxScore}之间！");
                            }
                            tb.DataText = option.ConvertToDecimal();
                        }
                        listData.Add(tb);
                    }
                }
                if (!error.Any())
                {
                    var tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                    listData.ForEach(p =>
                    {
                        p.InputDate = DateTime.Now;
                        p.MoralDate = vm.MoralDate;
                        p.tbSysUser = tbSysUser;
                    });

                    var oldData = (from p in db.Table<Moral.Entity.tbMoralData>()
                                   where studentIds.Contains(p.tbStudent.Id) && moralItemIds.Contains(p.tbMoralItem.Id)
                                   && vm.MoralDate == p.MoralDate
                                   select p);
                    foreach (var item in oldData)
                    {
                        item.IsDeleted = true;
                    }

                    db.Set<Moral.Entity.tbMoralData>().AddRange(listData);

                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("录入了德育数据！");
                    }
                }

            }
            return Code.MvcHelper.Post(error, message: "保存成功！");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DayEdit(Models.MoralData.DayEdit vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("DayEdit", new
            {
                moralId = vm.MoralId,
                moralDate = vm.MoralDate
            }));
        }

        #endregion

        #region 多次模式
        public ActionResult Edit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralData.Edit();
                var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                if (moral == null)
                {
                    //moral = (from p in db.Table<Moral.Entity.tbMoral>() where DateTime.Now <= p.ToDate && DateTime.Now >= p.FromDate && p.IsOpen select p).FirstOrDefault();
                    moral = (from p in db.Table<Moral.Entity.tbMoral>() where p.IsOpen select p).FirstOrDefault();
                }
                if (moral == null)
                {
                    vm.DataIsNull = true;
                    return View(vm);
                    //return RedirectToAction("List", "Moral");
                }
                switch (moral.MoralType)
                {
                    case Code.EnumHelper.MoralType.Once:
                        return RedirectToAction("OnceEdit", new { MoralId = vm.MoralId });
                    case Code.EnumHelper.MoralType.Days:
                        return RedirectToAction("DayEdit", new { MoralId = vm.MoralId });
                }

                if (vm.MoralDate == Code.DateHelper.MinDate)
                {
                    vm.MoralDate = DateTime.Now.Date;
                }
                vm.MoralId = moral.Id;

                vm.MoralClassList = MoralClassController.SelectList(vm.MoralId);

                if (vm.ClassId == 0 && vm.MoralClassList != null && vm.MoralClassList.Any())
                {
                    vm.ClassId = vm.MoralClassList[0].ClassId;
                }

                if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator)
                {
                    //获取拥有评价权限的德育选项及班级列表
                    var classList = (from p in db.Table<Entity.tbMoralPowerClass>()
                                     where p.tbMoralPower.tbTeacher.tbSysUser.Id == Code.Common.UserId &&
                                        (p.tbMoralPower.MoralDate == vm.MoralDate || !p.tbMoralPower.MoralDate.HasValue) &&
                                        p.tbMoralPower.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.MoralId
                                     select new //Models.MoralPower.Info()
                                     {
                                         MoralItemId = p.tbMoralPower.tbMoralItem.Id,
                                         MoralItemName = p.tbMoralPower.tbMoralItem.MoralItemName,
                                         ClassId = p.tbClass.Id,
                                         ClassName = p.tbClass.ClassName
                                     }).ToList();

                    classList.RemoveAll(p => !vm.MoralClassList.Select(t => t.ClassId).ToList().Contains(p.ClassId));

                    vm.MoralPowerClass = classList.GroupBy(p => new { p.MoralItemId, p.MoralItemName }).Select(p => new Models.MoralPower.Info()
                    {
                        MoralItemId = p.Key.MoralItemId,
                        MoralItemName = p.Key.MoralItemName,
                        MoralClass = new List<Dto.MoralClass.Info>()
                    }).ToList();

                    vm.MoralPowerClass.ForEach(p =>
                    {
                        p.MoralClass = classList.Where(c => c.MoralItemId == p.MoralItemId).Select(c => new Dto.MoralClass.Info()
                        {
                            ClassId = c.ClassId,
                            ClassName = c.ClassName
                        }).ToList();
                    });
                }

                vm.MoralList = MoralController.SelectList();

                if (vm.MoralClassList == null || !vm.MoralClassList.Any())
                {
                    return View(vm);
                }


                vm.MoralType = moral.MoralType;
                vm.FromDate = moral.FromDate.ToString(Code.Common.StringToDate);
                vm.ToDate = moral.ToDate.ToString(Code.Common.StringToDate);

                vm.KindId = vm.KindId ?? (int)Code.EnumHelper.MoralItemKind.Class;
                if (typeof(Code.EnumHelper.MoralItemKind).IsEnumDefined(vm.KindId))
                {
                    vm.Kind = (Code.EnumHelper.MoralItemKind)System.Enum.Parse(typeof(Code.EnumHelper.MoralItemKind), vm.KindId.ToString());
                }

                //分组列表
                vm.MoralGroupList = MoralGroupController.GetMoralGroupInfoList(vm.MoralId, vm.Kind);

                foreach (var group in vm.MoralGroupList)
                {
                    //根据分组获取德育选项
                    vm.MoralItemList.AddRange(MoralItemController.SelectList(group.Id, vm.Kind));

                    ////根据分组及评价权限获取德育选项
                    //var tb=(from p in db.Table<tbMoralPowerClass>)
                }

                vm.MoralItemIsNull = vm.MoralItemList == null || vm.MoralItemList.Count == 0;
                if (vm.MoralItemIsNull)
                {
                    return View(vm);
                }


                var moralClassIds = vm.MoralClassList.Select(p => p.ClassId).ToList();

                vm.MoralDataList = (from p in db.Table<Moral.Entity.tbMoralData>()
                                    where
                                        p.CheckStatus == Code.EnumHelper.CheckStatus.Success &&
                                        p.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.MoralId &&
                                        p.MoralDate == vm.MoralDate &&
                                        p.MoralItemOperateType == Code.EnumHelper.MoralItemOperateType.Score
                                    select new Dto.MoralData.List()
                                    {
                                        Id = p.Id,
                                        StudentId = p.tbStudent != null ? p.tbStudent.Id : 0,
                                        StudentGroupId = p.tbClassGroup != null ? p.tbClassGroup.Id : 0,
                                        ClassId = p.tbClass != null ? p.tbClass.Id : 0,
                                        MoralItemId = p.tbMoralItem.Id,
                                        DataText = p.DataText
                                    }).ToList();

                if (vm.ClassId > 0)
                {
                    moralClassIds = new List<int> { vm.ClassId.Value };
                }

                switch (vm.Kind)
                {
                    case Code.EnumHelper.MoralItemKind.Student:
                        vm.StudentList = Student.Controllers.StudentController.GetStudentInfoListByClassIds(moralClassIds);
                        //vm.MoralClassListItem = MoralClassController.SelectItemList(vm.MoralId);
                        vm.MoralClassListItem = vm.MoralClassList.Select(p => new SelectListItem()
                        {
                            Text = p.ClassName,
                            Value = p.ClassId.ToString()
                        }).ToList();
                        break;
                    case Code.EnumHelper.MoralItemKind.Group:
                        //vm.MoralClassListItem = MoralClassController.SelectItemList(vm.MoralId);
                        vm.StudentGroupList = Basis.Controllers.ClassGroupController.SelectList(moralClassIds);
                        vm.MoralClassListItem = vm.MoralClassList.Select(p => new SelectListItem()
                        {
                            Text = p.ClassName,
                            Value = p.ClassId.ToString()
                        }).ToList();
                        break;
                    case Code.EnumHelper.MoralItemKind.Class:
                        break;
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.MoralData.Edit vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("Edit", new
            {
                MoralId = vm.MoralId,
                KindId = vm.KindId,
                MoralDate = vm.MoralDate,
                ClassId = vm.ClassId
            }));
        }


        public ActionResult StudentDetailList()
        {
            var vm = new Models.MoralData.StudentDetailList();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.IsPower = Code.Common.UserType == Code.EnumHelper.SysUserType.Administrator;

                if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator)
                {
                    var moralPowerList = (from p in db.Table<Moral.Entity.tbMoralPower>() where p.tbMoralItem.Id == vm.MoralItemId && p.tbTeacher.tbSysUser.Id == Code.Common.UserId select p).ToList();
                    if (moralPowerList.Exists(p => !p.MoralDate.HasValue) || moralPowerList.Exists(p => p.MoralDate.HasValue && p.MoralDate.Value == vm.MoralDate))
                    {
                        vm.IsPower = true;
                    }
                }

                //当前德育选项的操作方式是否为打分
                //vm.IsScoreOperate = (from p in db.Table<Entity.tbMoralItem>() where p.Id == vm.MoralItemId select p.MoralItemOperateType).FirstOrDefault() == Code.EnumHelper.MoralItemOperateType.Score;

                vm.IsScoreOperate = (Code.EnumHelper.MoralItemOperateType)Enum.Parse(typeof(Code.EnumHelper.MoralItemOperateType), vm.OperateType) == Code.EnumHelper.MoralItemOperateType.Score;

                var tb = (from p in db.Table<Entity.tbMoralData>()
                          join o in db.Table<Entity.tbMoralPhoto>() on p.Id equals o.tbMoralData.Id into photo
                          from ph in photo.DefaultIfEmpty()
                          where p.tbMoralItem.Id == vm.MoralItemId && p.MoralDate == vm.MoralDate
                          select new { moralData = p, photo = ph });

                if (vm.StudentId > 0)
                {
                    tb = tb.Where(p => p.moralData.tbStudent.Id == vm.StudentId);
                }
                if (vm.StudentGroupId > 0)
                {
                    tb = tb.Where(p => p.moralData.tbClassGroup.Id == vm.StudentGroupId);
                }
                if (vm.ClassId > 0)
                {
                    tb = tb.Where(p => p.moralData.tbClass.Id == vm.ClassId);
                }

                if (vm.IsScoreOperate)
                {
                    vm.DataList = (from p in tb
                                   select new
                                   {
                                       Id = p.moralData.Id,
                                       Score = p.moralData.DataText.Value,
                                       Date = p.moralData.MoralDate,
                                       MoralExpress = p.moralData.DataText.Value > 0 ? Code.EnumHelper.MoralExpress.Add : Code.EnumHelper.MoralExpress.Subtract,
                                       Reason = p.moralData.tbMoralDataReason.Reason,
                                       CheckStatus = p.moralData.CheckStatus,
                                       Photo = p.photo != null ? p.photo.Id : 0
                                   }).GroupBy(p => new { p.Id, p.Score, p.Date, p.MoralExpress, p.Reason, p.CheckStatus }).Select(p => new Dto.MoralData.StudentDetailList()
                                   {
                                       Id = p.Key.Id,
                                       Score = p.Key.Score,
                                       Date = p.Key.Date,
                                       MoralExpress = p.Key.MoralExpress,
                                       Reason = p.Key.Reason,
                                       CheckStatus = p.Key.CheckStatus,
                                       HasPhoto = p.Sum(d => d.Photo) > 0
                                   }).ToList();
                }
                else
                {
                    vm.DataList = (from p in tb
                                   select new Dto.MoralData.StudentDetailList()
                                   {
                                       Id = p.moralData.Id,
                                       Date = p.moralData.MoralDate,
                                       Comment = p.moralData.Comment,
                                       CheckStatus = p.moralData.CheckStatus
                                   }).ToList();
                }
            }
            return View(vm);
        }

        public ActionResult StudentDetailEdit(int id = 0)
        {
            var vm = new Models.MoralData.StudentDetailEdit();

            //当前德育选项的操作方式是否为打分
            //vm.IsScoreOperate = (from p in db.Table<Entity.tbMoralItem>() where p.Id == vm.MoralItemId select p.MoralItemOperateType).FirstOrDefault() == Code.EnumHelper.MoralItemOperateType.Score;

            vm.IsScoreOperate = (Code.EnumHelper.MoralItemOperateType)Enum.Parse(typeof(Code.EnumHelper.MoralItemOperateType), vm.OperateType) == Code.EnumHelper.MoralItemOperateType.Score;

            if (vm.IsScoreOperate)
            {
                vm.MoralDataReasonList = MoralDataReasonController.SelectList(vm.MoralItemId);
            }
            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var tb = (from p in db.Table<Moral.Entity.tbMoralData>()
                              where p.Id == id
                              select new Dto.MoralData.StudentDetailEdit()
                              {
                                  Id = id,
                                  MoralExpress = p.DataText.HasValue && p.DataText.Value > 0 ? Code.EnumHelper.MoralExpress.Add : Code.EnumHelper.MoralExpress.Subtract,
                                  Score = Math.Abs(p.DataText.Value),
                                  MoralDataReason = p.tbMoralDataReason.Reason,
                                  Comment = p.Comment
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.Edit = tb;
                    }
                }
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentDetailEdit(Models.MoralData.StudentDetailEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var moralItem = db.Set<Entity.tbMoralItem>().Find(vm.MoralItemId);

                var tbData = (from p in db.Table<Entity.tbMoralData>() where p.tbMoralItem.Id == vm.MoralItemId && p.MoralDate == vm.MoralDate select p);
                if (vm.ClassId > 0)
                {
                    tbData = tbData.Where(p => p.tbClass.Id == vm.ClassId);
                }
                if (vm.StudentGroupId > 0)
                {
                    tbData = tbData.Where(p => p.tbClassGroup.Id == vm.StudentGroupId);
                }
                if (vm.StudentId > 0)
                {
                    tbData = tbData.Where(p => p.tbStudent.Id == vm.StudentId);
                }
                //累计已评分数值
                var existsScore = tbData.Sum(p => p.DataText);

                //当前评分值
                var currentScore = vm.Edit.MoralExpress == Code.EnumHelper.MoralExpress.Add ? vm.Edit.Score : 0 - vm.Edit.Score;

                var score = moralItem.DefaultValue + (existsScore.HasValue ? existsScore.Value : 0) + currentScore;

                if (score > moralItem.MaxScore)
                {
                    return Code.MvcHelper.Post(new List<string>() { $"累计分数【{score}】已超过该评分项的最高分【{moralItem.MaxScore}】！" });
                }

                if (score < moralItem.MinScore)
                {
                    return Code.MvcHelper.Post(new List<string>() { $"累计分数【{score}】不能小于该评分项的最低分【{moralItem.MinScore}】！" });
                }



                if (vm.MoralDataId > 0)
                {
                    var tb = db.Set<Moral.Entity.tbMoralData>().Find(vm.MoralDataId);
                    if (tb != null)
                    {

                        tb.DataText = vm.Edit.MoralExpress == Code.EnumHelper.MoralExpress.Add ? vm.Edit.Score : 0 - vm.Edit.Score;
                        tb.Comment = vm.Edit.Comment ?? string.Empty;
                        if (vm.Edit.tbMoralDataReasonId.HasValue && vm.Edit.tbMoralDataReasonId.Value> 0)
                        {
                            tb.tbMoralDataReason = db.Set<Entity.tbMoralDataReason>().Find(vm.Edit.tbMoralDataReasonId);
                        }
                        tb.MoralItemOperateType = moralItem.MoralItemOperateType;
                        tb.CheckStatus = moralItem.AutoCheck ? Code.EnumHelper.CheckStatus.Success : Code.EnumHelper.CheckStatus.None;
                        if (!string.IsNullOrWhiteSpace(vm.Edit.MoralPhotos))
                        {
                            var tbMoralPhoto = (from p in db.Table<Entity.tbMoralPhoto>() where p.tbMoralData.Id == vm.MoralDataId select p);
                            foreach (var photo in tbMoralPhoto)
                            {
                                photo.IsDeleted = true;
                            }

                            var photoList = vm.Edit.MoralPhotos.Split('|').ToList();
                            photoList.RemoveAll(p => string.IsNullOrWhiteSpace(p));
                            db.Set<Entity.tbMoralPhoto>().AddRange(photoList.Select(p => new Entity.tbMoralPhoto()
                            {
                                FileName = p,
                                tbMoralData = tb
                            }).ToList());
                        }

                        if (db.SaveChanges() > 0)
                        {
                            Sys.Controllers.SysUserLogController.Insert("修改了德育评分！");
                        }
                    }
                    else
                    {
                        return Code.MvcHelper.Post(new List<string>() { Resources.LocalizedText.MsgNotFound });
                    }

                }
                else
                {
                    var tb = new Moral.Entity.tbMoralData()
                    {
                        DataText = vm.Edit.MoralExpress == Code.EnumHelper.MoralExpress.Add ? vm.Edit.Score : 0 - vm.Edit.Score,
                        InputDate = DateTime.Now.Date,
                        MoralDate = vm.MoralDate,
                        tbMoralItem = moralItem,
                        MoralItemOperateType = moralItem.MoralItemOperateType,
                        CheckStatus = moralItem.AutoCheck ? Code.EnumHelper.CheckStatus.Success : Code.EnumHelper.CheckStatus.None,
                        Comment = vm.Edit.Comment ?? string.Empty,
                        tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                    };

                    if (vm.Edit.tbMoralDataReasonId.HasValue && vm.Edit.tbMoralDataReasonId.Value > 0)
                    {
                        tb.tbMoralDataReason = db.Set<Entity.tbMoralDataReason>().Find(vm.Edit.tbMoralDataReasonId);
                    }

                    if (vm.StudentId > 0)
                    {
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                    }
                    if (vm.StudentGroupId > 0)
                    {
                        tb.tbClassGroup = db.Set<Basis.Entity.tbClassGroup>().Find(vm.StudentGroupId);
                    }
                    if (vm.ClassId > 0)
                    {
                        tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(vm.ClassId);
                    }

                    db.Set<Moral.Entity.tbMoralData>().Add(tb);

                    if (!string.IsNullOrWhiteSpace(vm.Edit.MoralPhotos))
                    {
                        var photoList = vm.Edit.MoralPhotos.Split('|').ToList();
                        photoList.RemoveAll(p => string.IsNullOrWhiteSpace(p));
                        db.Set<Entity.tbMoralPhoto>().AddRange(photoList.Select(p => new Entity.tbMoralPhoto()
                        {
                            FileName = p,
                            tbMoralData = tb
                        }).ToList());
                    }

                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("添加了德育评分！");
                    }
                }
            }
            return Code.MvcHelper.Post(message: "操作成功！", returnUrl: Url.Action("StudentDetailList", new
            {
                ItemId = vm.MoralItemId,
                StudentId = vm.StudentId,
                GroupId = vm.StudentGroupId,
                ClassId = vm.ClassId,
                Date = vm.MoralDate.Date,
                Op = vm.OperateType
            }));
        }

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            if (ids != null && ids.Any())
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var tbMoralData = (from p in db.Table<Moral.Entity.tbMoralData>() where ids.Contains(p.Id) select p);
                    foreach (var item in tbMoralData)
                    {
                        item.IsDeleted = true;
                    }
                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("删除了德育数据！");
                    }
                }
            }
            return Code.MvcHelper.Post();
        }


        [HttpPost]
        public JsonResult Uploader()
        {
            var file = Request.Files["Filedata"];
            var result = Code.Common.SaveFile(file, "~/Files/Moral/");
            return Json(result);
        }
        #endregion

    }
}