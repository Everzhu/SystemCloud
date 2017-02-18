using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralStatController : Controller
    {

        #region 单次模式(MoralType=MoralType.Days)统计及导出

        public ActionResult List()
        {
            var vm = new Models.MoralStat.List();
            using (var db = new XkSystem.Models.DbContext())
            {
                var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                if (moral == null)
                {
                    var endDate = DateTime.Now.Date;
                    //moral = (from p in db.Table<Moral.Entity.tbMoral>() where endDate <= p.ToDate && DateTime.Now >= p.FromDate && p.IsOpen select p).FirstOrDefault();
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
                    case Code.EnumHelper.MoralType.Many:
                        return RedirectToAction("StatByClass", new { moralId = moral.Id });
                    case Code.EnumHelper.MoralType.Days:
                        return RedirectToAction("StatByDay", new { moralId = moral.Id });
                }
                vm.MoralId = moral.Id;

                SetVmData(vm, db);

                //vm.MoralType = moral.MoralType;
                vm.MoralList = MoralController.SelectList();
            }

            return View(vm);
        }

        private void SetVmData(Models.MoralStat.List vm, XkSystem.Models.DbContext db)
        {
            vm.MoralClassList = MoralClassController.SelectItemList(vm.MoralId);
            vm.MoralClassList.Insert(0, new SelectListItem() { Text = "全部", Value = "0" });

            //学生列表          
            var classIds = new List<int>();
            if (vm.ClassId.HasValue && vm.ClassId.Value > 0)
            {
                classIds.Add(vm.ClassId.Value);
            }
            else
            {
                classIds = (from p in db.Table<Moral.Entity.tbMoralClass>() where p.tbMoral.Id == vm.MoralId select p.tbClass.Id).ToList();
            }
            vm.MoralStudentList = Student.Controllers.StudentController.GetStudentInfoListByClassIds(classIds);


            //MoralData
            var studentIds = vm.MoralStudentList.Select(p => p.Id).ToList();
            var tbMoralData = (from p in db.Table<Moral.Entity.tbMoralData>()
                               join item in db.Table<Moral.Entity.tbMoralItem>() on p.tbMoralItem.Id equals item.Id
                               join option in db.Table<Moral.Entity.tbMoralOption>() on p.tbMoralOption.Id equals option.Id into MoralOption
                               from op in MoralOption.DefaultIfEmpty()
                               where 
                                    p.CheckStatus== Code.EnumHelper.CheckStatus.Success &&
                                    p.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.MoralId && studentIds.Contains(p.tbStudent.Id)
                               select new
                               {
                                   StudentId = p.tbStudent.Id,
                                   MoralItemId = p.tbMoralItem.Id,
                                   MoralExpress = p.tbMoralItem.MoralExpress,
                                   AddScore = p.tbMoralItem.MoralExpress == Code.EnumHelper.MoralExpress.Add ? (p.DataText.HasValue ? p.DataText.Value : p.tbMoralOption.MoralOptionValue) : 0,
                                   SubScore = p.tbMoralItem.MoralExpress == Code.EnumHelper.MoralExpress.Subtract ? 0 - (p.DataText.HasValue ? p.DataText.Value : p.tbMoralOption.MoralOptionValue) : 0,
                               }).ToList();

            //德育选项
            vm.MoralItemList = MoralItemController.SelectListByMoralId(vm.MoralId);
            var statList = new List<Dto.MoralStat.List>();

            vm.MoralStudentList.ForEach(p =>
            {
                var entity = new Dto.MoralStat.List()
                {
                    StudentId = p.Id,
                    StudentName = p.StudentName,
                    MoralItemList = new List<Dto.MoralStat.MoralItemUnMany>()
                };
                vm.MoralItemList.ForEach(i =>
                {
                    var moralItem = new Dto.MoralStat.MoralItemUnMany()
                    {
                        MaxScore = i.MaxScore,
                        MoralItemId = i.Id,
                        MoralItemName = i.MoralItemName
                    };

                    var hasData = tbMoralData.Where(d => d.MoralItemId == i.Id && d.StudentId == entity.StudentId).ToList();
                    if (hasData != null && hasData.Any())
                    {
                        moralItem.RealScore = hasData.Sum(d => d.AddScore + d.SubScore);
                        moralItem.AddSocre = hasData.Sum(d => d.AddScore);
                        moralItem.SubScore = hasData.Sum(d => d.SubScore);
                    }
                    entity.MoralItemList.Add(moralItem);
                });
                entity.TotalScore = entity.MoralItemList.Sum(d => d.MaxScore);
                entity.TotalRealScore = entity.MoralItemList.Sum(d => d.RealScore);
                entity.TotalAddScore = entity.MoralItemList.Sum(d => d.AddSocre);
                entity.TotalSubScore = entity.MoralItemList.Sum(d => d.SubScore);
                statList.Add(entity);
            });

            statList.ForEach(p =>
            {
                p.Ranking = statList.Count(i => i.TotalRealScore > p.TotalRealScore) + 1;
            });

            vm.StatList = statList.OrderBy(p => p.Ranking).ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.MoralStat.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                MoralId = vm.MoralId,
                ClassId = vm.ClassId,
            }));
        }

        public ActionResult Export()
        {
            string file = System.IO.Path.GetTempFileName();

            if (string.IsNullOrEmpty(file) == false)
            {
                var vm = new Models.MoralStat.List();
                using (var db = new XkSystem.Models.DbContext())
                {
                    SetVmData(vm, db);
                }
                var dt = new System.Data.DataTable();
                dt.Columns.Add(new System.Data.DataColumn("学生姓名"));
                dt.Columns.AddRange(vm.MoralItemList.Select(p => new System.Data.DataColumn(p.MoralItemName)).ToArray());
                dt.Columns.AddRange(
                    new string[] { "总分", "排名" }.Select(p => new System.Data.DataColumn(p)).ToArray()
                    );

                foreach (var stat in vm.StatList)
                {
                    var dr = dt.NewRow();
                    dr["学生姓名"] = stat.StudentName;
                    foreach (var moralItem in vm.MoralItemList)
                    {
                        var item = stat.MoralItemList.Where(p => p.MoralItemId == moralItem.Id).FirstOrDefault();
                        dr[item.MoralItemName] = item.RealScore;
                    }
                    dr["总分"] = stat.TotalRealScore;
                    dr["排名"] = stat.Ranking;
                    dt.Rows.Add(dr);
                }
                Code.NpoiHelper.DataTableToExcel(file, dt);
                return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Export(Models.MoralStat.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("Export", new
            {
                MoralId = vm.MoralId,
                ClassId = vm.ClassId
            }));
        }

        #endregion

        #region 每天模式(MoralType=MoralType.Days)统计及导出

        public ActionResult StatByDay()
        {
            var vm = new Models.MoralStat.DayStat();
            using (var db = new XkSystem.Models.DbContext())
            {
                var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                if (moral == null)
                {   
                    var date = DateTime.Now.Date;
                    //moral = (from p in db.Table<Moral.Entity.tbMoral>() where date <= p.ToDate && date >= p.FromDate && p.IsOpen select p).FirstOrDefault();
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
                    case Code.EnumHelper.MoralType.Many:
                        return RedirectToAction("StatByClass", new { moralId = moral.Id });
                    case Code.EnumHelper.MoralType.Once:
                        return RedirectToAction("List", new { moralId = moral.Id });
                }
                vm.MoralId = moral.Id;
                SetVmDataForDay(vm, db,moral);
                vm.MoralList = MoralController.SelectList();
            }

            return View(vm);
        }

        private void SetVmDataForDay(Models.MoralStat.DayStat vm, XkSystem.Models.DbContext db, Moral.Entity.tbMoral moral)
        {
            vm.MoralClassList = MoralClassController.SelectItemList(vm.MoralId);
            vm.MoralClassList.Insert(0, new SelectListItem() { Text = "全部", Value = "0" });

            DateTime fromDate = DateTime.Now.Date.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));  //本周周一  
            DateTime toDate = fromDate.AddDays(7).AddSeconds(-1);  //本周周日 23:59:59

            if (string.IsNullOrWhiteSpace(vm.StatDate))
            {
                vm.StatDate = "w";
            }

            switch (vm.StatDate.ToLower())
            {
                case "w":
                    break;
                case "m":
                    fromDate = DateTime.Now.Date.AddDays(0 - DateTime.Now.Day);
                    toDate = fromDate.AddMonths(1).AddSeconds(-1);
                    break;
                case "d":
                    fromDate = DateTime.Now.Date;
                    toDate = fromDate.AddDays(1).AddSeconds(-1);
                    break;
                case "s":
                    fromDate = vm.FromDate.Date;
                    fromDate = fromDate == Code.DateHelper.MinDate ? DateTime.Now.Date : fromDate;
                    toDate = vm.ToDate.Date;
                    toDate = toDate == Code.DateHelper.MinDate ? fromDate.AddDays(1).AddSeconds(-1) : toDate;
                    break;
            }

            vm.FromDate = fromDate;
            vm.ToDate = toDate;

            if (vm.FromDate > moral.ToDate || vm.ToDate < moral.FromDate)
            {
                return;
            }

            //学生列表          
            var classIds = new List<int>();
            if (vm.ClassId.HasValue && vm.ClassId.Value > 0)
            {
                classIds.Add(vm.ClassId.Value);
            }
            else
            {
                classIds = (from p in db.Table<Moral.Entity.tbMoralClass>() where p.tbMoral.Id == vm.MoralId select p.tbClass.Id).ToList();
            }
            vm.MoralStudentList = Student.Controllers.StudentController.GetStudentInfoListByClassIds(classIds);


            //MoralData
            var studentIds = vm.MoralStudentList.Select(p => p.Id).ToList();
            var tbMoralData = (from p in db.Table<Moral.Entity.tbMoralData>()
                               join item in db.Table<Moral.Entity.tbMoralItem>() on p.tbMoralItem.Id equals item.Id
                               join option in db.Table<Moral.Entity.tbMoralOption>() on p.tbMoralOption.Id equals option.Id into MoralOption
                               from op in MoralOption.DefaultIfEmpty()
                               where
                                    p.CheckStatus == Code.EnumHelper.CheckStatus.Success &&
                                    p.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.MoralId && studentIds.Contains(p.tbStudent.Id)
                                    && p.MoralDate >= vm.FromDate && p.MoralDate <= vm.ToDate
                               select new
                               {
                                   StudentId = p.tbStudent.Id,
                                   MoralItemId = p.tbMoralItem.Id,
                                   MoralExpress = p.tbMoralItem.MoralExpress,
                                   AddScore = p.tbMoralItem.MoralExpress == Code.EnumHelper.MoralExpress.Add ? (p.DataText.HasValue ? p.DataText.Value : p.tbMoralOption.MoralOptionValue) : 0,
                                   SubScore = p.tbMoralItem.MoralExpress == Code.EnumHelper.MoralExpress.Subtract ? 0 - (p.DataText.HasValue ? p.DataText.Value : p.tbMoralOption.MoralOptionValue) : 0,
                               }).ToList();

            //德育选项
            vm.MoralItemList = MoralItemController.SelectListByMoralId(vm.MoralId);
            var statList = new List<Dto.MoralStat.List>();

            vm.MoralStudentList.ForEach(p =>
            {
                var entity = new Dto.MoralStat.List()
                {
                    StudentId = p.Id,
                    StudentName = p.StudentName,
                    MoralItemList = new List<Dto.MoralStat.MoralItemUnMany>()
                };
                vm.MoralItemList.ForEach(i =>
                {
                    var moralItem = new Dto.MoralStat.MoralItemUnMany()
                    {
                        MaxScore = i.MaxScore,
                        MoralItemId = i.Id,
                        MoralItemName = i.MoralItemName
                    };

                    var hasData = tbMoralData.Where(d => d.MoralItemId == i.Id && d.StudentId == entity.StudentId).ToList();
                    if (hasData != null && hasData.Any())
                    {
                        moralItem.RealScore = hasData.Sum(d => d.AddScore + d.SubScore);
                        moralItem.AddSocre = hasData.Sum(d => d.AddScore);
                        moralItem.SubScore = hasData.Sum(d => d.SubScore);
                    }
                    entity.MoralItemList.Add(moralItem);
                });
                entity.TotalScore = entity.MoralItemList.Sum(d => d.MaxScore);
                entity.TotalRealScore = entity.MoralItemList.Sum(d => d.RealScore);
                entity.TotalAddScore = entity.MoralItemList.Sum(d => d.AddSocre);
                entity.TotalSubScore = entity.MoralItemList.Sum(d => d.SubScore);
                statList.Add(entity);
            });

            statList.ForEach(p =>
            {
                p.Ranking = statList.Count(i => i.TotalRealScore > p.TotalRealScore) + 1;
            });

            vm.StatList = statList.OrderBy(p => p.Ranking).ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StatByDay(Models.MoralStat.DayStat vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StatByDay", new
            {
                Fd = vm.FromDate,
                Td = vm.ToDate,
                MoralId = vm.MoralId,
                ClassId = vm.ClassId,
                StatDate = vm.StatDate
            }));
        }

        public ActionResult ExportForDay()
        {
            string file = System.IO.Path.GetTempFileName();

            if (string.IsNullOrEmpty(file) == false)
            {
                var vm = new Models.MoralStat.DayStat();
                using (var db = new XkSystem.Models.DbContext())
                {
                    var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                    SetVmDataForDay(vm, db,moral);
                }

                var dt = new System.Data.DataTable();
                dt.Columns.Add(new System.Data.DataColumn("学生姓名"));
                dt.Columns.AddRange(vm.MoralItemList.Select(p => new System.Data.DataColumn(p.MoralItemName)).ToArray());
                dt.Columns.AddRange(
                    new string[] { "总分", "排名" }.Select(p => new System.Data.DataColumn(p)).ToArray()
                    );

                foreach (var stat in vm.StatList)
                {
                    var dr = dt.NewRow();
                    dr["学生姓名"] = stat.StudentName;
                    foreach (var moralItem in vm.MoralItemList)
                    {
                        var item = stat.MoralItemList.Where(p => p.MoralItemId == moralItem.Id).FirstOrDefault();
                        dr[item.MoralItemName] = item.RealScore;
                    }
                    dr["总分"] = stat.TotalRealScore;
                    dr["排名"] = stat.Ranking;
                    dt.Rows.Add(dr);
                }
                Code.NpoiHelper.DataTableToExcel(file, dt);
                return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportForDay(Models.MoralStat.DayStat vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ExportForDay", new
            {
                MoralId = vm.MoralId,
                ClassId = vm.ClassId,
                fd = vm.FromDate,
                td = vm.ToDate,
                statDate = vm.StatDate
            }));
        }

        #endregion


        #region 多次模式(MoralType=MoralType.Many)按小组统计及导出

        public ActionResult StatByGroup()
        {
            var vm = new Models.MoralStat.GroupStat();
            using (var db = new XkSystem.Models.DbContext())
            {
                var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                if (moral == null)
                {
                    var date = DateTime.Now.Date;
                    //moral = (from p in db.Table<Entity.tbMoral>() where date <= p.ToDate && date >= p.FromDate && p.IsOpen select p).FirstOrDefault();
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
                        return RedirectToAction("List", new { moralId = moral.Id });
                    case Code.EnumHelper.MoralType.Days:
                        return RedirectToAction("StatByDay", new { moralId = moral.Id });
                }
                vm.MoralId = moral.Id;
                vm.MoralClassList = MoralClassController.SelectItemList(vm.MoralId);
                SetVmDataForGroupStat(vm, db,moral);
            }
            return View(vm);
        }

        private void SetVmDataForGroupStat(Models.MoralStat.GroupStat vm, XkSystem.Models.DbContext db, Entity.tbMoral moral)
        {
            vm.MoralList = MoralController.SelectList();
            var classIds = new List<int>();
            if (vm.MoralClassId == 0)
            {
                vm.MoralClassId = vm.MoralClassList[0].Value.ConvertToInt();
            }
            classIds.Add(vm.MoralClassId);
            //else
            //{
            //    classIds = (from p in db.Table<Moral.Entity.tbMoralClass>() where p.tbMoral.Id == vm.MoralId select p.tbClass.Id).ToList();
            //}

            vm.MoralGroupList = Basis.Controllers.ClassGroupController.SelectList(classIds);
            vm.MoralItemList = MoralItemController.SelectListByMoralIdAndKind(vm.MoralId, Code.EnumHelper.MoralItemKind.Group);

            DateTime fromDate = DateTime.Now.Date.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));  //本周周一  
            DateTime toDate = fromDate.AddDays(7).AddSeconds(-1);  //本周周日 23:59:59

            if (string.IsNullOrWhiteSpace(vm.StatDate))
            {
                vm.StatDate = "w";
            }

            switch (vm.StatDate.ToLower())
            {
                case "w":
                    break;
                case "m":
                    fromDate = DateTime.Now.Date.AddDays(0 - DateTime.Now.Day);
                    toDate = fromDate.AddMonths(1).AddSeconds(-1);
                    break;
                case "d":
                    fromDate = DateTime.Now.Date;
                    toDate = fromDate.AddDays(1).AddSeconds(-1);
                    break;
                case "s":
                    fromDate = vm.FromDate.Date;
                    fromDate = fromDate == Code.DateHelper.MinDate ? DateTime.Now.Date : fromDate;
                    toDate = vm.ToDate.Date;
                    toDate = toDate == Code.DateHelper.MinDate ? fromDate.AddDays(1).AddSeconds(-1) : toDate;
                    break;
            }

            vm.FromDate = fromDate;
            vm.ToDate = toDate;

            if (vm.FromDate > moral.ToDate || vm.ToDate < moral.FromDate)
            {
                return;
            }

            var moralData = (from p in db.Table<Moral.Entity.tbMoralData>()
                             where
                                p.CheckStatus == Code.EnumHelper.CheckStatus.Success && p.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.MoralId && p.tbClassGroup != null
                                && p.MoralDate >= vm.FromDate && p.MoralDate <= vm.ToDate
                             select new
                             {
                                 MoralItemId = p.tbMoralItem.Id,
                                 GroupId = p.tbClassGroup.Id,
                                 AddScore = p.DataText.Value > 0 ? p.DataText.Value : 0,
                                 SubScore = p.DataText.Value < 0 ? (0 - p.DataText.Value) : 0,
                                 DefaultValue = p.tbMoralItem.DefaultValue,
                                 Score = p.DataText.Value
                             }).GroupBy(p => new { p.MoralItemId, p.GroupId, p.DefaultValue }).Select(p => new
                             {
                                 GroupId = p.Key.GroupId,
                                 MoralItemId = p.Key.MoralItemId,
                                 DefaultValue = p.Key.DefaultValue,
                                 AddScore = p.Sum(d => d.AddScore),
                                 SubScore = p.Sum(d => d.SubScore),
                                 Score = p.Sum(d => d.Score)
                             }).ToList();


            vm.MoralGroupList.ForEach(p =>
            {
                var entity = new Dto.MoralStat.GroupStat()
                {
                    GroupId = p.Id,
                    GroupName = p.ClassGroupName,
                    ClassId=p.ClassId,
                    ClassName=p.ClassName,
                    MoralItemList = new List<Dto.MoralStat.MoralItemList>()
                };
                vm.MoralItemList.ForEach(i =>
                {
                    var itemData = new Dto.MoralStat.MoralItemList()
                    {
                        Id = i.Id,
                        MoralItemName = i.MoralItemName,
                        DefaultValue = i.DefaultValue
                    };
                    var hasData = moralData.Where(d => d.GroupId == entity.GroupId && d.MoralItemId == i.Id).ToList();
                    if (hasData != null && hasData.Any())
                    {
                        itemData.DefaultValue = hasData.Select(d => d.DefaultValue).FirstOrDefault();
                        itemData.AddScore = hasData.Select(d => d.AddScore).FirstOrDefault();
                        itemData.SubScore = hasData.Select(d => d.SubScore).FirstOrDefault();
                        itemData.RealScore = hasData.Select(d => d.Score).FirstOrDefault();
                    }
                    entity.MoralItemList.Add(itemData);
                    entity.TotalScore = entity.MoralItemList.Sum(s => (s.DefaultValue + s.RealScore));
                    entity.TotalDefaultValue = entity.MoralItemList.Sum(s => s.DefaultValue);
                    entity.TotalAddScore = entity.MoralItemList.Sum(s => s.AddScore);
                    entity.TotalSubScore = entity.MoralItemList.Sum(s => s.SubScore);
                });
                vm.StatList.Add(entity);
            });

            vm.StatList.ForEach(p =>
            {
                p.Ranking = vm.StatList.Count(s =>s.ClassId==p.ClassId && s.TotalScore > p.TotalScore) + 1;
            });
            vm.StatList = vm.StatList.OrderBy(p=>p.ClassId).ThenBy(p => p.Ranking).ToList();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StatByGroup(Models.MoralStat.GroupStat vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("StatByGroup", new
            {
                moralId = vm.MoralId,
                statDate = vm.StatDate,
                classId=vm.MoralClassId,
                fd = vm.FromDate,
                td = vm.ToDate
            }));
        }

        public ActionResult ExportForGroup()
        {
            string file = System.IO.Path.GetTempFileName();

            if (string.IsNullOrEmpty(file) == false)
            {
                var vm = new Models.MoralStat.GroupStat();
                using (var db = new XkSystem.Models.DbContext())
                {
                    var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                    //vm.MoralClassList = MoralClassController.SelectItemList(vm.MoralId);
                    SetVmDataForGroupStat(vm, db,moral);
                }
                var dt = new System.Data.DataTable();
                dt.Columns.Add(new System.Data.DataColumn("班级"));
                dt.Columns.Add(new System.Data.DataColumn("小组"));
                dt.Columns.AddRange(vm.StatList[0].MoralItemList.Select(p => new System.Data.DataColumn(p.MoralItemName)).ToArray());
                dt.Columns.AddRange(
                    new string[] { "总分", "排名" }.Select(p => new System.Data.DataColumn(p)).ToArray()
                    );

                foreach (var item in vm.StatList)
                {
                    var dr = dt.NewRow();
                    dr["班级"] = item.ClassName;
                    dr["小组"] = item.GroupName;
                    foreach (var moralItem in item.MoralItemList)
                    {
                        dr[moralItem.MoralItemName] = moralItem.RealScore + moralItem.DefaultValue;
                    }
                    dr["总分"] = item.TotalScore;
                    dr["排名"] = item.Ranking;
                    dt.Rows.Add(dr);
                }
                Code.NpoiHelper.DataTableToExcel(file, dt);
                return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
            }
            else
            {
                return View();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportForGroup(Models.MoralStat.GroupStat vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("ExportForGroup", new
            {
                MoralId = vm.MoralId,
                FromDate = vm.FromDate,
                ToDate = vm.ToDate,
                ClassId = vm.MoralClassId
            }));
        }

        #endregion

        #region 多次模式(MoralType=MoralType.Many)按班级统计及导出

        public ActionResult StatByClass()
        {
            var vm = new Models.MoralStat.ClassStat();
            using (var db = new XkSystem.Models.DbContext())
            {

                var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                if (moral == null)
                {
                    var date = DateTime.Now.Date;
                    //moral = (from p in db.Table<Moral.Entity.tbMoral>() where date <= p.ToDate && date >= p.FromDate && p.IsOpen select p).FirstOrDefault();
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
                        return RedirectToAction("List", new { moralId = moral.Id });
                    case Code.EnumHelper.MoralType.Days:
                        return RedirectToAction("StatByDay", new { moralId = moral.Id });
                }
                vm.MoralId = moral.Id;
                SetVmDataForClassStat(vm, db,moral);
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StatByClass(Models.MoralStat.ClassStat vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("StatByClass", new
            {
                moralId = vm.MoralId,
                fd = vm.FromDate,
                td = vm.ToDate,
                statDate = vm.StatDate
            }));
        }

        public ActionResult ExportForClass()
        {
            string file = System.IO.Path.GetTempFileName();

            if (string.IsNullOrEmpty(file) == false)
            {
                var vm = new Models.MoralStat.ClassStat();

                using (var db = new XkSystem.Models.DbContext())
                {
                    var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                    SetVmDataForClassStat(vm, db,moral);
                }
                var dt = new System.Data.DataTable();
                dt.Columns.Add(new System.Data.DataColumn("班级名称"));
                dt.Columns.AddRange(vm.StatList[0].MoralItemList.Select(p => new System.Data.DataColumn(p.MoralItemName)).ToArray());
                dt.Columns.AddRange(
                    new string[] { "总分", "排名" }.Select(p => new System.Data.DataColumn(p)).ToArray()
                    );
                dt.Columns.AddRange(vm.StudentStatList[0].MoralItemList.Select(p => new System.Data.DataColumn(p.MoralItemName + "_个人")).ToArray());
                dt.Columns.AddRange(
                    new string[] { "总分_个人", "排名_个人" }.Select(p => new System.Data.DataColumn(p)).ToArray()
                    );

                var listHeaders = new List<string>();
                listHeaders.Add("班级名称");
                listHeaders.AddRange(vm.StatList[0].MoralItemList.Select(p => "班级分").ToArray());
                listHeaders.AddRange(new string[] { "", "" });
                listHeaders.AddRange(vm.StatList[0].MoralItemList.Select(p => "个人分").ToArray());
                listHeaders.AddRange(new string[] { "", "" });

                foreach (var item in vm.StatList)
                {
                    var dr = dt.NewRow();
                    dr["班级名称"] = item.ClassName;
                    foreach (var moralItem in item.MoralItemList)
                    {
                        dr[moralItem.MoralItemName] = moralItem.RealScore + moralItem.DefaultValue;
                    }
                    dr["总分"] = item.TotalScore;
                    dr["排名"] = item.Ranking;

                    var student = vm.StudentStatList.Where(p => p.ClassId == item.ClassId).FirstOrDefault();
                    foreach (var stuItem in student.MoralItemList)
                    {
                        dr[stuItem.MoralItemName+"_个人"] = stuItem.RealScore + stuItem.DefaultValue;
                    }
                    dr["总分_个人"] = student.TotalScore;
                    dr["排名_个人"] = student.Ranking;
                    dt.Rows.Add(dr);
                }
                DataTableToExcel(file, dt, listHeaders);
                return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
            }
            else
            {
                return View();
            }
        }

        public bool DataTableToExcel(string fileName, System.Data.DataTable data, List<string> listHeaders)
        {
            int i = 0;
            int j = 0;
            NPOI.SS.UserModel.ISheet sheet = null;
            var fs = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet();
                }
                else
                {
                    return false;
                }

                var rowIndex = 0;
                var cellStyle = workbook.CreateCellStyle();
                cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                cellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                cellStyle.ShrinkToFit = false;

                NPOI.SS.UserModel.IRow row = sheet.CreateRow(rowIndex);
                for (var k = 0; k < listHeaders.Count; k++)
                {
                    var cell = row.CreateCell(k);
                    cell.SetCellValue(listHeaders[k]);
                    cell.CellStyle = cellStyle;
                }
                rowIndex = rowIndex + 1;

                row = sheet.CreateRow(rowIndex);
                for (j = 0; j < data.Columns.Count; ++j)
                {
                    var cell = row.CreateCell(j);
                    cell.SetCellValue(data.Columns[j].ColumnName);
                    cell.CellStyle = cellStyle;
                }
                rowIndex = rowIndex + 1;

                //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, 0, 0));
                var startIndex = 1;
                var endIndex = (listHeaders.Count - 1) / 2;
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, startIndex, endIndex));
                startIndex = endIndex + 1;
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, startIndex, startIndex + endIndex));

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    row = sheet.CreateRow(rowIndex);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        var cell = row.CreateCell(j);
                        cell.SetCellValue(data.Rows[i][j].ToString());
                        cell.CellStyle = cellStyle;
                    }
                    rowIndex = rowIndex + 1;
                }

                for (j = 0; j < data.Columns.Count; ++j)
                {
                    sheet.AutoSizeColumn(j);
                }

                workbook.Write(fs);
                fs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportForClass(Models.MoralStat.ClassStat vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("ExportForClass", new
            {
                moralId = vm.MoralId,
                fd = vm.FromDate,
                td = vm.ToDate,
                statDate = vm.StatDate
            }));
        }

        private void SetVmDataForClassStat(Models.MoralStat.ClassStat vm, XkSystem.Models.DbContext db, Moral.Entity.tbMoral moral)
        {

            vm.MoralList = MoralController.SelectList();
            vm.MoralClassList = MoralClassController.SelectList(vm.MoralId);

            //vm.MoralItemList = MoralItemController.SelectListByMoralIdAndKind(vm.MoralId, Code.EnumHelper.MoralItemKind.Class);
            vm.MoralItemList = MoralItemController.SelectListByMoralId(vm.MoralId);

            DateTime fromDate = DateTime.Now.Date.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));  //本周周一  
            DateTime toDate = fromDate.AddDays(7).AddSeconds(-1);  //本周周日 23:59:59

            if (string.IsNullOrWhiteSpace(vm.StatDate))
            {
                vm.StatDate = "w";
            }

            switch (vm.StatDate.ToLower())
            {
                case "w":
                    break;
                case "m":
                    fromDate = DateTime.Now.Date.AddDays(0 - DateTime.Now.Day);
                    toDate = fromDate.AddMonths(1).AddSeconds(-1);
                    break;
                case "d":
                    fromDate = DateTime.Now.Date;
                    toDate = fromDate.AddDays(1).AddSeconds(-1);
                    break;
                case "s":
                    fromDate = vm.FromDate.Date;
                    fromDate = fromDate == Code.DateHelper.MinDate ? DateTime.Now.Date : fromDate;
                    toDate = vm.ToDate.Date;
                    toDate = toDate == Code.DateHelper.MinDate ? fromDate.AddDays(1).AddSeconds(-1) : toDate;
                    break;
            }
            vm.FromDate = fromDate;
            vm.ToDate = toDate;

            if (vm.FromDate > moral.ToDate || vm.ToDate < moral.FromDate)
            {
                return;
            }

            var moralData = (from p in db.Table<Moral.Entity.tbMoralData>()
                             join c in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals c.tbStudent.Id into tbClassStudent
                             from cs in tbClassStudent.DefaultIfEmpty()
                             where 
                                p.CheckStatus== Code.EnumHelper.CheckStatus.Success &&
                                p.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.MoralId && (p.tbStudent!=null || p.tbClass!=null)
                                && p.MoralDate >= vm.FromDate && p.MoralDate <= vm.ToDate
                             select new
                             {
                                 MoralItemId = p.tbMoralItem.Id,
                                 ClassId = p.tbClass != null ? p.tbClass.Id : cs.tbClass.Id,
                                 StudentId = cs != null ? cs.tbStudent.Id : 0,
                                 AddScore = p.DataText.Value > 0 ? p.DataText.Value : 0,
                                 SubScore = p.DataText.Value < 0 ? (0 - p.DataText.Value) : 0,
                                 DefaultValue = p.tbMoralItem.DefaultValue,
                                 Score = p.DataText.Value
                             }).ToList();
            var classData = moralData.Where(p => p.StudentId == 0).GroupBy(p => new { p.MoralItemId, p.ClassId, p.DefaultValue }).Select(p => new
            {
                ClassId = p.Key.ClassId,
                MoralItemId = p.Key.MoralItemId,
                DefaultValue = p.Key.DefaultValue,
                AddScore = p.Sum(d => d.AddScore),
                SubScore = p.Sum(d => d.SubScore),
                Score = p.Sum(d => d.Score)
            }).ToList();
            var studentData = moralData.Where(p => p.StudentId > 0).GroupBy(p => new { p.MoralItemId, p.ClassId, p.DefaultValue }).Select(p => new
            {
                ClassId = p.Key.ClassId,
                MoralItemId = p.Key.MoralItemId,
                DefaultValue = p.Key.DefaultValue,
                AddScore = p.Sum(d => d.AddScore),
                SubScore = p.Sum(d => d.SubScore),
                Score = p.Sum(d => d.Score)
            }).ToList();


            var moralItemForClassList = vm.MoralItemList.Where(p => p.MoralItemKind == Code.EnumHelper.MoralItemKind.Class).ToList();
            var moralItemForStudentList = vm.MoralItemList.Where(p => p.MoralItemKind == Code.EnumHelper.MoralItemKind.Student).ToList();

            vm.MoralClassList.ForEach(p =>
            {
                #region 班级分
                var entity = new Dto.MoralStat.ClassStat()
                {
                    ClassId = p.ClassId,
                    ClassName = p.ClassName,
                    MoralItemList = new List<Dto.MoralStat.MoralItemList>()
                };
                moralItemForClassList.ForEach(i =>
                {
                    var itemData = new Dto.MoralStat.MoralItemList()
                    {
                        Id = i.Id,
                        MoralItemName = i.MoralItemName,
                        DefaultValue = i.DefaultValue
                    };
                    var hasData = classData.Where(d => d.ClassId == entity.ClassId && d.MoralItemId == i.Id).ToList();
                    if (hasData != null && hasData.Any())
                    {
                        itemData.DefaultValue = hasData.Select(d => d.DefaultValue).FirstOrDefault();
                        itemData.AddScore = hasData.Select(d => d.AddScore).FirstOrDefault();
                        itemData.SubScore = hasData.Select(d => d.SubScore).FirstOrDefault();
                        itemData.RealScore = hasData.Select(d => d.Score).FirstOrDefault();
                    }
                    entity.MoralItemList.Add(itemData);
                    entity.TotalScore = entity.MoralItemList.Sum(s => (s.DefaultValue + s.RealScore));
                    entity.TotalDefaultValue = entity.MoralItemList.Sum(s => s.DefaultValue);
                    entity.TotalAddScore = entity.MoralItemList.Sum(s => s.AddScore);
                    entity.TotalSubScore = entity.MoralItemList.Sum(s => s.SubScore);
                });
                vm.StatList.Add(entity);
                #endregion

                #region 个人分
                var studentEntity = new Dto.MoralStat.ClassStat()
                {
                    ClassId = p.ClassId,
                    ClassName = p.ClassName,
                    //TotalScore = moralData.Where(d => d.ClassId== p.ClassId && d.StudentId>0).Sum(d => (d.DefaultValue+d.Score)),
                    MoralItemList = new List<Dto.MoralStat.MoralItemList>()
                };

                
                moralItemForStudentList.ForEach(i =>
                {
                    var itemData = new Dto.MoralStat.MoralItemList()
                    {
                        Id = i.Id,
                        MoralItemName = i.MoralItemName,
                        DefaultValue = i.DefaultValue
                    };
                    var hasData = studentData.Where(d => d.ClassId == entity.ClassId && d.MoralItemId == i.Id).ToList();
                    if (hasData != null && hasData.Any())
                    {
                        itemData.DefaultValue = hasData.Select(d => d.DefaultValue).FirstOrDefault();
                        itemData.AddScore = hasData.Select(d => d.AddScore).FirstOrDefault();
                        itemData.SubScore = hasData.Select(d => d.SubScore).FirstOrDefault();
                        itemData.RealScore = hasData.Select(d => d.Score).FirstOrDefault();
                    }
                    studentEntity.MoralItemList.Add(itemData);

                    studentEntity.TotalScore = studentEntity.MoralItemList.Sum(s => (s.DefaultValue + s.RealScore));
                    studentEntity.TotalDefaultValue = studentEntity.MoralItemList.Sum(s => s.DefaultValue);
                    studentEntity.TotalAddScore = studentEntity.MoralItemList.Sum(s => s.AddScore);
                    studentEntity.TotalSubScore = studentEntity.MoralItemList.Sum(s => s.SubScore);
                });
                vm.StudentStatList.Add(studentEntity);
                #endregion
            });


            vm.StudentStatList.ForEach(p =>
            {
                p.Ranking = vm.StudentStatList.Count(l => l.TotalScore > p.TotalScore) + 1;
            });
            vm.StudentStatList = vm.StudentStatList.OrderBy(p => p.Ranking).ToList();

            vm.StatList.ForEach(p =>
            {
                p.Ranking = vm.StatList.Count(l => l.TotalScore > p.TotalScore) + 1;
                p.Total = p.TotalScore + vm.StudentStatList.Where(s => s.ClassId == p.ClassId).Select(s => s.TotalScore).FirstOrDefault();
            });

            vm.StatList.ForEach(p =>
            {
                p.TotalRanking = vm.StatList.Count(l => l.Total > p.Total) + 1;
            });

            vm.StatList = vm.StatList.OrderBy(p => p.TotalRanking).ToList();


        }

        #endregion

        #region 多次模式(MoralType=MoralType.Many)按学生统计及导出

        public ActionResult StatByStudent()
        {
            var vm = new Models.MoralStat.StudentStat();
            using (var db = new XkSystem.Models.DbContext())
            {

                var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                if (moral == null)
                {
                    var date = DateTime.Now.Date;
                    //moral = (from p in db.Table<Moral.Entity.tbMoral>() where date <= p.ToDate && date >= p.FromDate select p).FirstOrDefault();
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
                        return RedirectToAction("List", new { moralId = moral.Id });
                    case Code.EnumHelper.MoralType.Days:
                        return RedirectToAction("StatByDay", new { moralId = moral.Id });
                }
                vm.MoralId = moral.Id;
                vm.MoralClassList = MoralClassController.SelectItemList(vm.MoralId);

                if (vm.MoralClassList == null || !vm.MoralClassList.Any())
                {
                    vm.DataIsNull = true;
                    return View(vm);
                }
                SetVmDataForStudentStat(vm, db,moral);
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StatByStudent(Models.MoralStat.StudentStat vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("StatByStudent", new
            {
                moralId = vm.MoralId,
                classId = vm.MoralClassId,
                fd = vm.FromDate,
                td = vm.ToDate,
                statDate = vm.StatDate
            }));
        }

        private void SetVmDataForStudentStat(Models.MoralStat.StudentStat vm, XkSystem.Models.DbContext db, Entity.tbMoral moral)
        {
            vm.MoralList = MoralController.SelectList();
            var classIds = new List<int>();
            if (vm.MoralClassId==0)
            {
                vm.MoralClassId = vm.MoralClassList[0].Value.ConvertToInt();
                classIds.Add(vm.MoralClassId);
            }
            else
            {
                classIds = (from p in db.Table<Moral.Entity.tbMoralClass>() where p.tbMoral.Id == vm.MoralId select p.tbClass.Id).ToList();
            }
            vm.MoralStudentList = Student.Controllers.StudentController.GetStudentInfoListByClassIds(classIds);
            vm.MoralItemList = MoralItemController.SelectListByMoralIdAndKind(vm.MoralId, Code.EnumHelper.MoralItemKind.Student);

            DateTime fromDate = DateTime.Now.Date.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));  //本周周一  
            DateTime toDate = fromDate.AddDays(7).AddSeconds(-1);  //本周周日 23:59:59

            if (string.IsNullOrWhiteSpace(vm.StatDate))
            {
                vm.StatDate = "w";
            }

            switch (vm.StatDate.ToLower())
            {
                case "w":
                    break;
                case "m":
                    fromDate = DateTime.Now.Date.AddDays(0 - DateTime.Now.Day);
                    toDate = fromDate.AddMonths(1).AddSeconds(-1);
                    break;
                case "d":
                    fromDate = DateTime.Now.Date;
                    toDate = fromDate.AddDays(1).AddSeconds(-1);
                    break;
                case "s":
                    fromDate = vm.FromDate.Date;
                    fromDate = fromDate == Code.DateHelper.MinDate ? DateTime.Now.Date : fromDate;
                    toDate = vm.ToDate.Date;
                    toDate = toDate == Code.DateHelper.MinDate ? fromDate.AddDays(1).AddSeconds(-1) : toDate;
                    break;
            }

            vm.FromDate = fromDate;
            vm.ToDate = toDate;

            if (vm.FromDate > moral.ToDate || vm.ToDate < moral.FromDate)
            {
                return;
            }

            var moralData = (from p in db.Table<Moral.Entity.tbMoralData>()
                            // join cs in db.Table< Basis.Entity.tbClassStudent >() on p.tbStudent.Id equals cs.tbStudent.Id
                             where
                                p.CheckStatus == Code.EnumHelper.CheckStatus.Success && 
                                p.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.MoralId && p.tbStudent != null && 
                                p.MoralDate >= vm.FromDate && p.MoralDate <= vm.ToDate
                             select new
                             {
                                 MoralItemId = p.tbMoralItem.Id,
                                 StudentId = p.tbStudent.Id,
                                 //ClassId=cs.tbClass.Id,
                                 AddScore = p.DataText.Value > 0 ? p.DataText.Value : 0,
                                 SubScore = p.DataText.Value < 0 ? (0 - p.DataText.Value) : 0,
                                 DefaultValue = p.tbMoralItem.DefaultValue,
                                 Score = p.DataText.Value
                             }).GroupBy(p => new { p.MoralItemId, p.StudentId, p.DefaultValue/*,p.ClassId*/}).Select(p => new
                             {
                                 StudentId = p.Key.StudentId,
                                 //ClassId=p.Key.ClassId,
                                 MoralItemId = p.Key.MoralItemId,
                                 DefaultValue = p.Key.DefaultValue,
                                 AddScore = p.Sum(d => d.AddScore),
                                 SubScore = p.Sum(d => d.SubScore),
                                 Score = p.Sum(d => d.Score)
                             }).ToList();


            vm.MoralStudentList.ForEach(p =>
            {
                var entity = new Dto.MoralStat.StudentStat()
                {
                    StudentId = p.Id,
                    ClassId=p.ClassId,
                    ClassName=p.ClassName,
                    StudentName = p.StudentName,
                    MoralItemList = new List<Dto.MoralStat.MoralItemList>()
                };
                vm.MoralItemList.ForEach(i =>
                {
                    var itemData = new Dto.MoralStat.MoralItemList()
                    {
                        Id = i.Id,
                        MoralItemName = i.MoralItemName,
                        DefaultValue = i.DefaultValue
                    };
                    var hasData = moralData.Where(d => d.StudentId == entity.StudentId && d.MoralItemId == i.Id).ToList();
                    if (hasData != null && hasData.Any())
                    {
                        itemData.DefaultValue = hasData.Select(d => d.DefaultValue).FirstOrDefault();
                        itemData.AddScore = hasData.Select(d => d.AddScore).FirstOrDefault();
                        itemData.SubScore = hasData.Select(d => d.SubScore).FirstOrDefault();
                        itemData.RealScore = hasData.Select(d => d.Score).FirstOrDefault();
                    }
                    entity.MoralItemList.Add(itemData);
                    entity.TotalScore = entity.MoralItemList.Sum(s => (s.DefaultValue + s.RealScore));
                    entity.TotalDefaultValue = entity.MoralItemList.Sum(s => s.DefaultValue);
                    entity.TotalAddScore = entity.MoralItemList.Sum(s => s.AddScore);
                    entity.TotalSubScore = entity.MoralItemList.Sum(s => s.SubScore);
                });
                vm.StatList.Add(entity);
            });

            vm.StatList.ForEach(p =>
            {
                p.Ranking = vm.StatList.Count(s =>s.ClassId==p.ClassId && s.TotalScore > p.TotalScore) + 1;
            });
            vm.StatList = vm.StatList.OrderBy(p=>p.ClassId).ThenBy(p=>p.Ranking).ToList();

        }

        public ActionResult ExportForStudent()
        {
            string file = System.IO.Path.GetTempFileName();

            if (string.IsNullOrEmpty(file) == false)
            {
                var vm = new Models.MoralStat.StudentStat();
                using (var db = new XkSystem.Models.DbContext())
                {
                    var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                    SetVmDataForStudentStat(vm, db,moral);
                }
                var dt = new System.Data.DataTable();
                dt.Columns.Add(new System.Data.DataColumn("班级"));
                dt.Columns.Add(new System.Data.DataColumn("学生"));
                dt.Columns.AddRange(vm.StatList[0].MoralItemList.Select(p => new System.Data.DataColumn(p.MoralItemName)).ToArray());
                dt.Columns.AddRange(
                    new string[] { "总分", "排名" }.Select(p => new System.Data.DataColumn(p)).ToArray()
                    );

                foreach (var item in vm.StatList)
                {
                    var dr = dt.NewRow();
                    dr["班级"] = item.ClassName;
                    dr["学生"] = item.StudentName;
                    foreach (var moralItem in item.MoralItemList)
                    {
                        dr[moralItem.MoralItemName] = moralItem.RealScore + moralItem.DefaultValue;
                    }
                    dr["总分"] = item.TotalScore;
                    dr["排名"] = item.Ranking;
                    dt.Rows.Add(dr);
                }
                Code.NpoiHelper.DataTableToExcel(file, dt);
                return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportForStudent(Models.MoralStat.StudentStat vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("ExportForStudent", new
            {
                moralId = vm.MoralId,
                classId = vm.MoralClassId,
                fd = vm.FromDate,
                td = vm.ToDate,
                statDate = vm.StatDate
            }));
        }

        #endregion


        #region 每月之星及导出
        public ActionResult Star()
        {
            var vm = new Models.MoralStat.Star();
            using (var db = new XkSystem.Models.DbContext())
            {

                var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                if (moral == null || moral.MoralType!= Code.EnumHelper.MoralType.Many)
                {
                    var date = DateTime.Now.Date;
                    moral = (from p in db.Table<Moral.Entity.tbMoral>() where date <= p.ToDate && date >= p.FromDate && p.MoralType== Code.EnumHelper.MoralType.Many select p).FirstOrDefault();
                }
                if (moral == null)
                {
                    vm.MoralIsNull = true;
                    //return RedirectToAction("List", "Moral");
                    return View(vm);
                }

                vm.MoralId = moral.Id;
                //vm.MoralClassList = MoralClassController.SelectItemList(vm.MoralId);
                SetVmDataForStar(vm, db, moral);
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Star(Models.MoralStat.Star vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("Star", new
            {
                moralId = vm.MoralId,
                date = vm.Date
            }));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetStar(int moralId,int studentId, DateTime date)
        {
            
            using (var db = new XkSystem.Models.DbContext())
            {
                var model = (from p in db.Table<Entity.tbMoralStar>() where p.tbMoral.Id==moralId && p.tbStudent.Id == studentId && p.Date == date select p).FirstOrDefault();
                if (model == null)
                {
                    db.Set<Entity.tbMoralStar>().Add(new Entity.tbMoralStar()
                    {
                        Date = date,
                        InputDate=DateTime.Now,
                        tbMoral=db.Set<Entity.tbMoral>().Find(moralId),
                        tbStudent = db.Set<Student.Entity.tbStudent>().Find(studentId),
                        tbSysUser=db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId)
                    });
                }
                else
                {
                    model.IsDisabled = !model.IsDisabled;
                    model.UpdateTime = DateTime.Now;
                }
                if (db.SaveChanges() > 0)
                {
                    Sys.Controllers.SysUserLogController.Insert("设置了每月之星！");
                }
            }
            return Code.MvcHelper.Post(message:"设置成功！");
        }

        private void SetVmDataForStar(Models.MoralStat.Star vm, XkSystem.Models.DbContext db, Moral.Entity.tbMoral moral)
        {
            vm.MoralList = MoralController.SelectList();
            var classIds = new List<int>();
            //管理员不限制班级
            if (Code.Common.UserType == Code.EnumHelper.SysUserType.Administrator)
            {
                classIds = (from p in db.Table<Entity.tbMoralClass>() where p.tbMoral.Id == vm.MoralId select p.tbClass.Id).ToList();
            }
            else
            {
                classIds = (
                                from p in db.Table<Entity.tbMoralClass>()
                                join ct in db.Table<Basis.Entity.tbClassTeacher>() on p.tbClass.Id equals ct.tbClass.Id
                                where
                                    p.tbMoral.Id == vm.MoralId && ct.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                select
                                    p.tbClass.Id
                                ).ToList();
            }
            if (classIds == null || !classIds.Any())
            {
                vm.DataIsNull = true;
                return;
            }            

            vm.MoralItemList = MoralItemController.SelectListByMoralIdAndKind(vm.MoralId, Code.EnumHelper.MoralItemKind.Student);

            var startDate = Code.Common.DateMonthFirst;
            if(!DateTime.TryParse(vm.Date, out startDate))
            {
                startDate = Code.Common.DateMonthFirst;
                vm.Date = startDate.ToString(Code.Common.StringToYearMonth);
            }
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

            //vm.MoralStudentList = Student.Controllers.StudentController.GetStudentInfoListByClassIds(classIds);
            vm.MoralStudentList = (from p in db.Table<Entity.tbMoralClass>() 
                                    join cs in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals cs.tbClass.Id
                                    join ms in db.Table<Entity.tbMoralStar>() on new { moralId=p.tbMoral.Id,studentId = cs.tbStudent.Id, date =startDate } equals new { moralId=ms.tbMoral.Id,studentId=ms.tbStudent.Id,date=ms.Date} into star
                                    from s in star.DefaultIfEmpty()
                                    where 
                                        p.tbMoral.Id==vm.MoralId && classIds.Contains(p.tbClass.Id)
                                    select new Dto.MoralStat.StudentInfo
                                    {
                                        Id = cs.tbStudent.Id,
                                        StudentCode = cs.tbStudent.StudentCode,
                                        StudentName = cs.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                        ClassName = p.tbClass.ClassName,
                                        IsStar=s!=null && !s.IsDisabled
                                    }).ToList();

            var moralData = (from p in db.Table<Entity.tbMoralData>()
                             where 
                                p.CheckStatus== Code.EnumHelper.CheckStatus.Success &&
                                p.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.MoralId && p.tbStudent != null && 
                                p.MoralDate >= startDate && p.MoralDate <= endDate
                             select new
                             {
                                 MoralItemId = p.tbMoralItem.Id,
                                 StudentId = p.tbStudent.Id,
                                 AddScore = p.DataText.Value > 0 ? p.DataText.Value : 0,
                                 SubScore = p.DataText.Value < 0 ? (0 - p.DataText.Value) : 0,
                                 DefaultValue = p.tbMoralItem.DefaultValue,
                                 Score = p.DataText.Value                               
                             }).GroupBy(p => new { p.MoralItemId, p.StudentId, p.DefaultValue}).Select(p => new
                             {
                                 StudentId = p.Key.StudentId,
                                 MoralItemId = p.Key.MoralItemId,
                                 DefaultValue = p.Key.DefaultValue,
                                 AddScore = p.Sum(d => d.AddScore),
                                 SubScore = p.Sum(d => d.SubScore),
                                 Score = p.Sum(d => d.Score)
                             }).ToList();


            vm.MoralStudentList.ForEach(p =>
            {
                var entity = new Dto.MoralStat.Star()
                {
                    StudentId = p.Id,
                    ClassId = p.ClassId,
                    ClassName = p.ClassName,
                    StudentName = p.StudentName,
                    IsStar=p.IsStar,
                    MoralItemList = new List<Dto.MoralStat.MoralItemList>()
                };
                vm.MoralItemList.ForEach(i =>
                {
                    var itemData = new Dto.MoralStat.MoralItemList()
                    {
                        Id = i.Id,
                        MoralItemName = i.MoralItemName,
                        DefaultValue = i.DefaultValue
                    };
                    var hasData = moralData.Where(d => d.StudentId == entity.StudentId && d.MoralItemId == i.Id).ToList();
                    if (hasData != null && hasData.Any())
                    {
                        itemData.DefaultValue = hasData.Select(d => d.DefaultValue).FirstOrDefault();
                        itemData.AddScore = hasData.Select(d => d.AddScore).FirstOrDefault();
                        itemData.SubScore = hasData.Select(d => d.SubScore).FirstOrDefault();
                        itemData.RealScore = hasData.Select(d => d.Score).FirstOrDefault();
                    }
                    entity.MoralItemList.Add(itemData);
                    entity.TotalScore = entity.MoralItemList.Sum(s => (s.DefaultValue + s.RealScore));
                    entity.TotalDefaultValue = entity.MoralItemList.Sum(s => s.DefaultValue);
                    entity.TotalAddScore = entity.MoralItemList.Sum(s => s.AddScore);
                    entity.TotalSubScore = entity.MoralItemList.Sum(s => s.SubScore);
                });
                vm.StatList.Add(entity);
            });

            vm.StatList.ForEach(p =>
            {
                p.Ranking = vm.StatList.Count(s => s.ClassId == p.ClassId && s.TotalScore > p.TotalScore) + 1;
            });
            vm.StatList = vm.StatList.OrderBy(p => p.ClassId).ThenBy(p => p.Ranking).ToList();

        }

        public ActionResult ExportForStar()
        {
            string file = System.IO.Path.GetTempFileName();

            if (string.IsNullOrEmpty(file) == false)
            {
                var vm = new Models.MoralStat.Star();
                using (var db = new XkSystem.Models.DbContext())
                {
                    var moral = db.Set<Entity.tbMoral>().Find(vm.MoralId);
                    SetVmDataForStar(vm, db, moral);
                }
                var dt = new System.Data.DataTable();
                dt.Columns.Add(new System.Data.DataColumn("班级"));
                dt.Columns.Add(new System.Data.DataColumn("学生"));
                dt.Columns.AddRange(vm.StatList[0].MoralItemList.Select(p => new System.Data.DataColumn(p.MoralItemName)).ToArray());
                dt.Columns.AddRange(
                    new string[] { "总分", "排名","每月之星" }.Select(p => new System.Data.DataColumn(p)).ToArray()
                    );

                foreach (var item in vm.StatList)
                {
                    var dr = dt.NewRow();
                    dr["班级"] = item.ClassName;
                    dr["学生"] = item.StudentName;
                    foreach (var moralItem in item.MoralItemList)
                    {
                        dr[moralItem.MoralItemName] = moralItem.RealScore + moralItem.DefaultValue;
                    }
                    dr["总分"] = item.TotalScore;
                    dr["排名"] = item.Ranking;
                    dr["每月之星"] = item.IsStar ? "是" : "否";
                    dt.Rows.Add(dr);
                }
                Code.NpoiHelper.DataTableToExcel(file, dt);
                return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportForStar(Models.MoralStat.Star vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("ExportForStar", new
            {
                moralId = vm.MoralId,
                date=vm.Date
            }));
        }

        #endregion


    }
}