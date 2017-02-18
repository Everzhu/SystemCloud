using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralRedFlagController : Controller
    {
        // GET: Moral/MoralRedFlag
        public ActionResult List()
        {
            var vm = new Models.MoralRedFlag.List();
            using (var db = new XkSystem.Models.DbContext())
            {

                var moral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                if (moral == null || moral.MoralType != Code.EnumHelper.MoralType.Many)
                {
                    var date = DateTime.Now.Date;
                    moral = (from p in db.Table<Entity.tbMoral>() where p.MoralType == Code.EnumHelper.MoralType.Many && p.IsOpen select p).FirstOrDefault();
                    //moral = (from p in db.Table<Entity.tbMoral>() where date <= p.ToDate && date >= p.FromDate && p.MoralType == Code.EnumHelper.MoralType.Many select p).FirstOrDefault();
                }
                if (moral == null)
                {
                    vm.DataIsNull = true;
                    //return RedirectToAction("List", "Moral");
                    return View(vm);
                }

                vm.MoralId = moral.Id;
                vm.WeekList = Code.DateHelper.GetWeekDateList(DateTime.Now.Year);
                //vm.MoralClassList = MoralClassController.SelectItemList(vm.MoralId);
                SetVmDataForStar(vm, db, moral);
            }
            return View(vm);
        }

        private void SetVmDataForStar(Models.MoralRedFlag.List vm, XkSystem.Models.DbContext db, Entity.tbMoral moral)
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

            //vm.MoralItemList = MoralItemController.SelectListByMoralIdAndKind(vm.MoralId, Code.EnumHelper.MoralItemKind.Class);

            vm.MoralItemList=(from p in db.Table<Moral.Entity.tbMoralItem>()
                              where p.tbMoralGroup.tbMoral.Id == vm.MoralId && (p.MoralItemKind ==  Code.EnumHelper.MoralItemKind.Class || p.MoralItemKind== Code.EnumHelper.MoralItemKind.Student)
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

            //var weekNum = Code.DateHelper.GetWeekNumOfYearByDate(DateTime.Now);
            //var startDate = Code.DateHelper.GetDateTimeByWeekNumOfYear(DateTime.Now.Year, weekNum);
            //var endDate = startDate.AddDays(8).AddSeconds(-1);

            DateTime startDate;
            DateTime endDate;

            var weekNum = vm.WeekNum.HasValue ? vm.WeekNum.Value : Code.DateHelper.GetWeekNumOfYearByDate(DateTime.Now);

            if (!vm.WeekNum.HasValue)
            {
                vm.WeekNum = weekNum;
            }
            Code.DateHelper.GetWeekDate(DateTime.Now.Year, weekNum, out startDate, out endDate);

            //vm.MoralStudentList = Student.Controllers.StudentController.GetStudentInfoListByClassIds(classIds);
            vm.MoralClassInfo = (from p in db.Table<Entity.tbMoralClass>()
                                   join mr in db.Table<Entity.tbMoralRedFlag>() on new { moralId=p.tbMoral.Id,classId = p.tbClass.Id, weekNum = vm.WeekNum.Value } equals new { moralId=mr.tbMoral.Id,classId = mr.tbClass.Id, weekNum =mr.WeekNum  } into redFlag
                                   from r in redFlag.DefaultIfEmpty()
                                   where 
                                        p.tbMoral.Id==vm.MoralId && classIds.Contains(p.tbClass.Id)
                                   select new Dto.MoralRedFlag.ClassInfo
                                   {
                                       Id = p.tbClass.Id,
                                       ClassName = p.tbClass.ClassName,
                                       IsRedFlag=r!=null && !r.IsDisabled
                                   }).ToList();

            //流动红旗和每周之星不一样，流动红旗也累加班级及班级下面学生/*、小组*/的分数
            var moralData = (from p in db.Table<Entity.tbMoralData>()
                             //join c in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals c.tbStudent.Id into tbClassStudent
                             //from cs in tbClassStudent.DefaultIfEmpty()
                             where
                                p.CheckStatus == Code.EnumHelper.CheckStatus.Success &&
                                p.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.MoralId && (p.tbStudent != null || p.tbClass != null /*|| p.tbClassGroup!=null*/)
                                && p.MoralDate >=startDate && p.MoralDate <= endDate
                             select new
                             {
                                 MoralItemId = p.tbMoralItem.Id,
                                 ClassId = p.tbClass != null ? p.tbClass.Id : 0,
                                 StudentId = p.tbStudent != null ? p.tbStudent.Id : 0,
                                 //AddScore = p.DataText.Value > 0 ? p.DataText.Value : 0,
                                 //SubScore = p.DataText.Value < 0 ? (0 - p.DataText.Value) : 0,
                                 DefaultValue = p.tbMoralItem.DefaultValue,
                                 Score = p.DataText.Value
                             }).ToList();

            var classData = moralData.Where(p => p.StudentId == 0).GroupBy(p => new { p.MoralItemId, p.ClassId, p.DefaultValue }).Select(p => new
            {
                ClassId = p.Key.ClassId,
                MoralItemId = p.Key.MoralItemId,
                DefaultValue = p.Key.DefaultValue,
                //AddScore = p.Sum(d => d.AddScore),
                //SubScore = p.Sum(d => d.SubScore),
                Score = p.Sum(d => d.Score)
            }).ToList();
            var studentData = moralData.Where(p => p.StudentId > 0).GroupBy(p => new { p.MoralItemId, p.ClassId, p.DefaultValue }).Select(p => new
            {
                ClassId = p.Key.ClassId,
                MoralItemId = p.Key.MoralItemId,
                DefaultValue = p.Key.DefaultValue,
                //AddScore = p.Sum(d => d.AddScore),
                //SubScore = p.Sum(d => d.SubScore),
                Score = p.Sum(d => d.Score)
            }).ToList();

            classData.AddRange(studentData);
            dynamic data;
            if (classData != null && classData.Any())
            {
                data = classData.GroupBy(p => new
                {
                    p.ClassId,
                    p.MoralItemId,
                    p.DefaultValue,
                    p.Score
                }).Select(p => new
                {
                    ClassId=p.Key.ClassId,
                    MoralItemId=p.Key.MoralItemId,
                    DefaultValue=p.Sum(d=>d.DefaultValue),
                    Score=p.Sum(d=>d.Score)
                }).ToList();
            }
            
            vm.MoralClassInfo.ForEach(p =>
            {
                var entity = new Dto.MoralStat.RedFlag()
                {   
                    ClassId = p.Id,
                    ClassName = p.ClassName,
                    IsRedFlag=p.IsRedFlag,
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
                    var hasData = classData != null && classData.Any() ? classData.Where(d => d.ClassId == entity.ClassId && d.MoralItemId == i.Id).ToList() : null;
                    if (hasData != null && hasData.Any())
                    {
                        itemData.DefaultValue = hasData.Select(d => d.DefaultValue).FirstOrDefault();
                        //itemData.AddScore = hasData.Select(d => d.AddScore).FirstOrDefault();
                        //itemData.SubScore = hasData.Select(d => d.SubScore).FirstOrDefault();
                        itemData.RealScore = hasData.Select(d => d.Score).FirstOrDefault();
                    }
                    entity.MoralItemList.Add(itemData);
                    entity.TotalScore = entity.MoralItemList.Sum(s => (s.DefaultValue + s.RealScore));
                    entity.TotalDefaultValue = entity.MoralItemList.Sum(s => s.DefaultValue);
                    //entity.TotalAddScore = entity.MoralItemList.Sum(s => s.AddScore);
                    //entity.TotalSubScore = entity.MoralItemList.Sum(s => s.SubScore);
                });
                vm.StatList.Add(entity);
            });

            vm.StatList.ForEach(p =>
            {
                p.Ranking = vm.StatList.Count(s =>s.TotalScore > p.TotalScore) + 1;
            });
            vm.StatList = vm.StatList.OrderBy(p => p.Ranking).ToList();
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.MoralRedFlag.List vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("List", new
            {
                MoralId=vm.MoralId,
                WeekNum=vm.WeekNum
            }));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetRedFlag(int moralId,int classId, int weekNum)
        {

            using (var db = new XkSystem.Models.DbContext())
            {
                var model = (from p in db.Table<Entity.tbMoralRedFlag>() where p.tbMoral.Id==moralId && p.tbClass.Id == classId && p.WeekNum == weekNum select p).FirstOrDefault();
                if (model == null)
                {
                    DateTime startDate;
                    DateTime endDate;
                    Code.DateHelper.GetWeekDate(DateTime.Now.Year, weekNum, out startDate, out endDate);
                    db.Set<Entity.tbMoralRedFlag>().Add(new Entity.tbMoralRedFlag()
                    {
                        WeekNum=weekNum,
                        StartDate=startDate,
                        EndDate=endDate,
                        tbClass = db.Set<Basis.Entity.tbClass>().Find(classId),
                        tbMoral=db.Set<Entity.tbMoral>().Find(moralId),
                        InputDate=DateTime.Now
                    });
                }
                else
                {
                    model.IsDisabled = !model.IsDisabled;
                    model.UpdateTime = DateTime.Now;
                }
                if (db.SaveChanges() > 0)
                {
                    Sys.Controllers.SysUserLogController.Insert("设置了流动红旗！");
                }
            }
            return Code.MvcHelper.Post(message: "设置成功！");
        }


    }
}