using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Perform.Controllers
{
    public class PerformDataDayController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformDataDay.List();

                vm.PerformList = Perform.Controllers.PerformController.SelectList();

                if (vm.PerformId == 0 && vm.PerformList.Count > 0)
                {
                    vm.PerformId = vm.PerformList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.ClassList = PerformDataController.SelectOrgList(vm.PerformId);

                if (vm.ClassList.Count > 0 && vm.ClassId > 0)
                {
                    if (vm.ClassList.Where(d => d.Value.ConvertToInt() == vm.ClassId).Count() == decimal.Zero)
                    {
                        vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                    }
                }

                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.ClassList.Count() == decimal.Zero)
                {
                    vm.ClassId = 0;
                }

                vm.OrgSelectInfo = PerformChangeController.SelectOrgSelectInfo(db, vm.ClassList, vm.PerformId);

                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                            .Include(d => d.tbClass)
                            .Include(d => d.tbYear)
                            .Include(d => d.tbCourse)
                             where p.Id == vm.ClassId
                             select p).FirstOrDefault();

                if (tbOrg == null)
                {
                    return View(vm);
                }

                var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                         where p.tbCourse.Id == tbOrg.tbCourse.Id
                                         && p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                         && p.tbPerformGroup.IsDeleted == false
                                         && p.tbCourse.IsDeleted == false
                                         select p.tbPerformGroup.Id).ToList();


                vm.PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                      where tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                      && p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                      && p.tbPerformGroup.IsDeleted == false
                                      && p.IsMany
                                      orderby p.No
                                      select new Dto.PerformItem.List
                                      {
                                          Id = p.Id,
                                          Rate = p.Rate,
                                          ScoreMax = p.ScoreMax,
                                          PerformItemName = p.PerformItemName
                                      }
                                    ).ToList();

                vm.PerformDataDayAllList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                            where p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                                            && p.tbPerformItem.IsDeleted == false
                                            && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                            && p.tbStudent.IsDeleted == false
                                            && p.tbCourse.IsDeleted == false
                                            && p.tbPerformItem.IsMany
                                            select new Dto.PerformDataDay.List
                                            {
                                                PerformItemId = p.tbPerformItem.Id,
                                                Score = p.Score,
                                                StudentId = p.tbStudent.Id,
                                                CourseId = p.tbCourse.Id
                                            }).ToList();

                vm.PerformTotalList = (from p in db.Table<Perform.Entity.tbPerformTotal>()
                                       where p.tbPerform.Id == vm.PerformId
                                       && p.tbCourse.Id == tbOrg.tbCourse.Id
                                       && p.tbStudent.IsDeleted == false
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbPerform.IsDeleted == false
                                       select new Dto.PerformTotal.List
                                       {
                                           Id = p.Id,
                                           PerformId = p.tbPerform.Id,
                                           CourseId = p.tbCourse.Id,
                                           PerformName = p.tbPerform.PerformName,
                                           CourseName = p.tbCourse.CourseName,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           TotalScore = p.TotalScore
                                       }).ToList();

                if (tbOrg != null)
                {
                    var orgStudentList = new List<Dto.PerformDataDay.List>();
                    if (tbOrg.IsClass)
                    {
                        if (tbOrg.tbClass != null)
                        {
                            orgStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                              where p.tbClass.Id == tbOrg.tbClass.Id
                                              && p.tbStudent.IsDeleted == false
                                              && p.tbClass.IsDeleted == false
                                              orderby p.No, p.tbStudent.StudentCode
                                              select new Dto.PerformDataDay.List
                                              {
                                                  StudentNo = p.No.ToString(),
                                                  StudentId = p.tbStudent.Id,
                                                  StudentCode = p.tbStudent.StudentCode,
                                                  StudentName = p.tbStudent.StudentName,
                                                  CourseId = tbOrg.tbCourse.Id,
                                              }).ToList();
                        }
                    }
                    else
                    {
                        orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                          where p.tbOrg.Id == vm.ClassId
                                          && p.tbStudent.IsDeleted == false
                                          && p.tbOrg.IsDeleted == false
                                          orderby p.No, p.tbStudent.StudentCode
                                          select new Dto.PerformDataDay.List
                                          {
                                              StudentNo = p.No.ToString(),
                                              StudentId = p.tbStudent.Id,
                                              StudentCode = p.tbStudent.StudentCode,
                                              StudentName = p.tbStudent.StudentName,
                                              CourseId = tbOrg.tbCourse.Id
                                          }).ToList();
                    }

                    if (string.IsNullOrEmpty(vm.SearchText) == false)
                    {
                        orgStudentList = (from p in orgStudentList
                                          where p.StudentCode.Contains(vm.SearchText) || p.StudentName.Contains(vm.SearchText)
                                          select p).ToList();
                    }

                    vm.PerformDataDayList = (from p in orgStudentList
                                             select new Dto.PerformDataDay.List
                                             {
                                                 StudentNo = p.StudentNo == null ? "" : p.StudentNo.ToString(),
                                                 StudentCode = p.StudentCode.ToString(),
                                                 StudentName = p.StudentName.ToString(),
                                                 StudentId = p.StudentId,
                                                 CourseId = p.CourseId
                                             }).ToList();
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.PerformData.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                performId = vm.PerformId,
                classId = vm.ClassId
            }));
        }

        public ActionResult InputMultipleList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformDataDay.InputMultipleList();

                vm.PerformInputMultipleList = (from p in db.Table<Entity.tbPerformData>()
                                               where p.tbPerformItem.Id == vm.PerformItemId
                                               && p.tbStudent.Id == vm.StudentId
                                               && p.tbPerformItem.IsDeleted == false
                                               && p.tbCourse.IsDeleted == false
                                               select new Dto.PerformDataDay.InputMultipleList
                                               {
                                                   Id = p.Id,
                                                   PerformItemId = p.tbPerformItem.Id,
                                                   PerformItemName = p.tbPerformItem.PerformItemName,
                                                   PerformOptionId = p.tbPerformOption == null ? 0 : p.tbPerformOption.Id,
                                                   PerformOptionValue = p.tbPerformOption == null ? 0 : p.tbPerformOption.OptionValue,
                                                   StudentName = p.tbStudent.StudentName,
                                                   StudentCode = p.tbStudent.StudentCode,
                                                   StudentId = p.tbStudent.Id,
                                                   Score = p.Score,
                                                   InputDate = p.InputDate,
                                                   SysUserName = p.tbSysUser.UserName
                                               }).ToList();

                var tbPerformItem = (from p in db.Table<Entity.tbPerformItem>()
                                     where p.Id == vm.PerformItemId
                                     && p.IsSelect && p.IsMany
                                     select p).FirstOrDefault();

                if (tbPerformItem == null)
                {

                }
                else
                {
                    vm.PerformOptionList = PerformOptionController.SelectList(vm.PerformItemId);
                }
                return View(vm);
            }
        }

        public ActionResult InputMultipleEdit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformDataDay.InputMultipleEdit();
                var tbPerformItem = (from p in db.Table<Entity.tbPerformItem>()
                                     where p.Id == vm.PerformItemId
                                     && p.IsSelect && p.IsMany
                                     select p).FirstOrDefault();

                if (tbPerformItem == null)
                {

                }
                else
                {
                    vm.PerformOptionList = PerformOptionController.SelectList(vm.PerformItemId);
                }
                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbPerformData>()
                              where p.Id == id
                              select new Dto.PerformDataDay.InputMultipleEdit
                              {
                                  Id = p.Id,
                                  PerformItemName = p.tbPerformItem.PerformItemName,
                                  StudentCode = p.tbStudent.StudentCode,
                                  StudentName = p.tbStudent.StudentName,
                                  Score = p.Score,
                                  PerformOptionId = p.tbPerformOption == null ? 0 : p.tbPerformOption.Id
                              }).FirstOrDefault();

                    if (tb != null)
                    {
                        vm.PerformInputMultipleEdit = tb;
                    }
                }
                else
                {
                    vm.PerformInputMultipleEdit.PerformItemName = db.Set<Entity.tbPerformItem>().Find(vm.PerformItemId).PerformItemName;
                    vm.PerformInputMultipleEdit.StudentName = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId).StudentName;
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InputMultipleEdit(Models.PerformDataDay.InputMultipleEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var tbPerformItemFirst = (from p in db.Table<Entity.tbPerformItem>()
                                              where p.Id == vm.PerformItemId
                                              select p).FirstOrDefault();

                    if (tbPerformItemFirst == null)
                    {
                        return Code.MvcHelper.Post(new List<string>() { $"【评价项目】不存在，请重试;" });
                    }
                    else
                    {
                        var tbOldSum = (from p in db.Table<Entity.tbPerformData>()
                                        where p.tbCourse.Id == vm.PerformCourseId
                                        && p.tbPerformItem.Id == vm.PerformItemId
                                        && p.tbStudent.Id == vm.StudentId
                                        select p.Score).ToList().Sum();

                        if (vm.PerformInputMultipleEdit.Id == 0)
                        {
                            var tb = new Entity.tbPerformData();
                            tb.InputDate = DateTime.Now;
                            tb.Score = vm.PerformInputMultipleEdit.Score;
                            tb.tbPerformItem = db.Set<Entity.tbPerformItem>().Find(vm.PerformItemId);
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                            tb.tbCourse = db.Set<Course.Entity.tbCourse>().Find(vm.PerformCourseId);
                            tb.tbPerformOption = db.Set<Entity.tbPerformOption>().Find(vm.PerformInputMultipleEdit.PerformOptionId);
                            if (tb.tbPerformOption != null)
                            {
                                tb.Score = tb.tbPerformOption.OptionValue;
                            }
                            var sumScore = tb.Score + tbOldSum;
                            if (sumScore > tbPerformItemFirst.ScoreMax.ConvertToDecimal())
                            {
                                return Code.MvcHelper.Post(new List<string>() { $"项目总分:{sumScore}累计达到上限{tbPerformItemFirst.ScoreMax},请重新输入分数;" });
                            }
                            db.Set<Entity.tbPerformData>().Add(tb);
                            if (db.SaveChanges() > decimal.Zero)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增学生多次表现分");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Entity.tbPerformData>()
                                      where p.Id == vm.PerformInputMultipleEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.Score = vm.PerformInputMultipleEdit.Score;
                                tb.tbPerformItem = db.Set<Entity.tbPerformItem>().Find(vm.PerformItemId);
                                tb.tbCourse = db.Set<Course.Entity.tbCourse>().Find(vm.PerformCourseId);
                                tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                                tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                                tb.tbPerformOption = db.Set<Entity.tbPerformOption>().Find(vm.PerformInputMultipleEdit.PerformOptionId);
                                if (tb.tbPerformOption != null)
                                {
                                    tb.Score = tb.tbPerformOption.OptionValue;
                                }
                                var sumScore = tb.Score + tbOldSum;
                                if (sumScore > tbPerformItemFirst.ScoreMax.ConvertToDecimal())
                                {
                                    return Code.MvcHelper.Post(new List<string>() { $"项目总分:{sumScore}累计达到上限{tbPerformItemFirst.ScoreMax},请重新输入分数;" });
                                }
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改学生多次表现分");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }

                        #region 保存总分
                        var oldTotalList = (from p in db.Table<Perform.Entity.tbPerformTotal>()
                                            where p.tbPerform.Id == vm.PerformId
                                               && p.tbCourse.Id == vm.PerformCourseId
                                               && p.tbStudent.Id == vm.StudentId
                                               && p.tbStudent.IsDeleted == false
                                               && p.tbCourse.IsDeleted == false
                                               && p.tbPerform.IsDeleted == false
                                            select p).ToList();

                        foreach (var a in oldTotalList)
                        {
                            a.IsDeleted = true;
                            a.UpdateTime = DateTime.Now;
                        }

                        var studentSumScoreList = (from p in db.Table<Entity.tbPerformData>()
                                                   where p.tbCourse.Id == vm.PerformCourseId
                                                   && p.tbStudent.Id == vm.StudentId
                                                   && p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                                                   && p.tbPerformItem.IsMany == false
                                                   && p.tbPerformItem.IsSelect == false
                                                   select new
                                                   {
                                                       score = p.Score,
                                                       rate = p.tbPerformItem.Rate
                                                   }).ToList();

                        var studentSumScore = 0m;
                        if(studentSumScoreList.Count>0)
                        {
                            studentSumScore = studentSumScoreList.Select(d => d.score * d.rate / 100).Sum();
                        }

                        var studentSumDayScore = (from p in db.Table<Entity.tbPerformData>()
                                                  where p.tbCourse.Id == vm.PerformCourseId
                                                  && p.tbStudent.Id == vm.StudentId
                                                  && p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                                                  && ((p.tbPerformItem.IsMany && p.tbPerformItem.IsSelect) || (p.tbPerformItem.IsMany && p.tbPerformItem.IsSelect == false))
                                                  select p.Score).Sum();

                        var tfTotal = new Perform.Entity.tbPerformTotal();
                        tfTotal.TotalScore = studentSumScore + studentSumDayScore;
                        tfTotal.tbPerform = db.Set<Perform.Entity.tbPerform>().Find(vm.PerformId);
                        tfTotal.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                        tfTotal.tbCourse = db.Set<Course.Entity.tbCourse>().Find(vm.PerformCourseId);
                        db.Set<Perform.Entity.tbPerformTotal>().Add(tfTotal);
                        if (db.SaveChanges() > decimal.Zero)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增学习表现总分");
                        }
                        #endregion
                    }
                }
                return Code.MvcHelper.Post(null, returnUrl: Url.Action("InputMultipleList", new
                {
                    PerformId = vm.PerformId,
                    StudentId = vm.StudentId,
                    PerformItemId = vm.PerformItemId,
                    PerformCourseId = vm.PerformCourseId
                }));
            }
        }

        [HttpPost]
        public ActionResult InputMultipleDelete(List<int> ids)
        {
            if (ids != null && ids.Any())
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var tbRemove = (from p in db.Table<Entity.tbPerformData>()
                                    .Include(d => d.tbCourse)
                                    .Include(d => d.tbStudent)
                                    .Include(d => d.tbPerformItem.tbPerformGroup.tbPerform)
                                    where ids.Contains(p.Id)
                                    select p).ToList();

                    foreach (var item in tbRemove)
                    {
                        item.IsDeleted = true;
                        item.UpdateTime = DateTime.Now;
                    }
                    if (db.SaveChanges() > decimal.Zero)
                    {
                        Sys.Controllers.SysUserLogController.Insert("删除了学生表现多次分数！");
                    }

                    var tb = tbRemove.FirstOrDefault();
                    if (tb == null)
                    {

                    }
                    else
                    {                        
                        #region 保存总分
                        var oldTotalList = (from p in db.Table<Perform.Entity.tbPerformTotal>()
                                            where p.tbPerform.Id == tb.tbPerformItem.tbPerformGroup.tbPerform.Id
                                               && p.tbCourse.Id == tb.tbCourse.Id
                                               && p.tbStudent.Id == tb.tbStudent.Id
                                               && p.tbStudent.IsDeleted == false
                                               && p.tbCourse.IsDeleted == false
                                               && p.tbPerform.IsDeleted == false
                                            select p).ToList();

                        foreach (var a in oldTotalList)
                        {
                            a.IsDeleted = true;
                            a.UpdateTime = DateTime.Now;
                        }

                        var studentSumScoreList = (from p in db.Table<Entity.tbPerformData>()
                                                   where p.tbCourse.Id == tb.tbCourse.Id
                                                   && p.tbStudent.Id == tb.tbStudent.Id
                                                   && p.tbPerformItem.tbPerformGroup.tbPerform.Id == tb.tbPerformItem.tbPerformGroup.tbPerform.Id
                                                   && p.tbPerformItem.IsMany == false
                                                   && p.tbPerformItem.IsSelect == false
                                                   select new
                                                   {
                                                       score = p.Score,
                                                       rate = p.tbPerformItem.Rate
                                                   }).ToList();

                        var studentSumScore = 0m;
                        if (studentSumScoreList.Count > 0)
                        {
                            studentSumScore = studentSumScoreList.Select(d => d.score * d.rate / 100).Sum();
                        }

                        var studentSumDayScore = (from p in db.Table<Entity.tbPerformData>()
                                                  where p.tbCourse.Id == tb.tbCourse.Id
                                                   && p.tbStudent.Id == tb.tbStudent.Id
                                                   && p.tbPerformItem.tbPerformGroup.tbPerform.Id == tb.tbPerformItem.tbPerformGroup.tbPerform.Id
                                                  && ((p.tbPerformItem.IsMany && p.tbPerformItem.IsSelect) || (p.tbPerformItem.IsMany && p.tbPerformItem.IsSelect == false))
                                                  select p.Score).DefaultIfEmpty(0).Sum();

                        var tfTotal = new Perform.Entity.tbPerformTotal();
                        tfTotal.TotalScore = studentSumScore + studentSumDayScore;
                        tfTotal.tbPerform = db.Set<Perform.Entity.tbPerform>().Find(tb.tbPerformItem.tbPerformGroup.tbPerform.Id);
                        tfTotal.tbStudent = db.Set<Student.Entity.tbStudent>().Find(tb.tbStudent.Id);
                        tfTotal.tbCourse = db.Set<Course.Entity.tbCourse>().Find(tb.tbCourse.Id);
                        db.Set<Perform.Entity.tbPerformTotal>().Add(tfTotal);

                        if (db.SaveChanges() > decimal.Zero)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("更新学习表现总分数据");
                        }
                        #endregion
                    }
                }
            }
            return Code.MvcHelper.Post();
        }
    }
}