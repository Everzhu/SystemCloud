using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveRuleController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveRule.List();
                vm.ElectiveName = db.Table<Entity.tbElective>().FirstOrDefault(d => d.Id == vm.ElectiveId).ElectiveName;

                var tb = from p in db.Table<Entity.tbElectiveRule>()
                         where p.tbElective.Id == vm.ElectiveId
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbCourse.CourseName.Contains(vm.SearchText));
                }

                vm.ElectiveRuleList = (from p in tb
                                       orderby p.No
                                       select new Dto.ElectiveRule.List
                                       {
                                           Id = p.Id,
                                           No = p.No,
                                           CourseId = p.tbCourse.Id,
                                           CourseName = p.tbCourse.CourseName,
                                           CourseTarget = p.tbCourseTarget.CourseName,
                                           ElectiveRule = p.ElectiveRule
                                       }).ToList();

                vm.CourseList = (from p in db.Table<Entity.tbElectiveOrg>()
                                 where p.tbElective.Id == vm.ElectiveId
                                 group p by new { p.tbCourse.Id, p.tbCourse.CourseCode, p.tbCourse.CourseName } into g
                                 select new Course.Dto.Course.SimpleInfo()
                                 {
                                     Id = g.Key.Id,
                                     CourseCode = g.Key.CourseCode,
                                     CourseName = g.Key.CourseName
                                 }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveRule.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, electiveId = vm.ElectiveId }));
        }

        public JsonResult Delete(List<int> ids, int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var count = db.Table<Entity.tbElectiveRule>().Count(d => d.tbElective.Id == electiveId);
                if (count == decimal.One)
                {
                    error.Add("至少应该保留一个分段数据");
                }

                var tb = (from p in db.Table<Entity.tbElectiveRule>()
                          where ids.Contains(p.Id)
                          select p).FirstOrDefault();
                if (tb != null)
                {
                    tb.IsDeleted = true;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除选课分段");
                    }
                }

                return Code.MvcHelper.Post(error, Url.Action("List", new { electiveId = electiveId }));
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveRule.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbElectiveRule>()
                              where p.Id == id
                              select new Dto.ElectiveRule.Edit
                              {
                                  CourseId = p.tbCourse.Id,
                                  CourseName = p.tbCourse.CourseName,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ElectiveRuleEdit = tb;
                    }
                }

                //已经设置为规则的课程
                var ruleCourseList = (from p in db.Table<Entity.tbElectiveRule>()
                                      where p.tbElective.Id == vm.ElectiveId && p.tbCourse.Id == vm.CourseId
                                      select new
                                      {
                                          CourseId = p.tbCourse.Id,
                                          CourseTargetId = p.tbCourseTarget.Id,
                                          ElectiveRule = p.ElectiveRule
                                      }).ToList();

                //当前选课规则已设置的课程Id 
                vm.RuleCourseIds = ruleCourseList.Where(p => p.CourseId == vm.CourseId && (int)p.ElectiveRule == vm.RuleId).Select(p => p.CourseTargetId).ToList();

                //当前课程下其他选课规则已设置的课程id
                var otherCourseIds = ruleCourseList.Where(p => p.CourseId == vm.CourseId && (int)p.ElectiveRule != vm.RuleId).Select(p => p.CourseTargetId).ToList();

                var ruleClassIds = vm.RuleCourseIds;

                //自身互斥
                otherCourseIds.Add(vm.CourseId);

                vm.CourseList = (from p in db.Table<Entity.tbElectiveOrg>()
                                 where p.tbElective.Id == vm.ElectiveId && !otherCourseIds.Contains(p.tbCourse.Id)
                                 select new Course.Dto.Course.Info()
                                 {
                                     Id = p.tbCourse.Id,
                                     Hour = p.tbCourse.Hour,
                                     CourseCode = p.tbCourse.CourseCode,
                                     CourseName = p.tbCourse.CourseName,
                                     CourseNameEn = p.tbCourse.CourseNameEn,
                                     SubjectName = p.tbCourse.tbSubject.SubjectName,
                                     CourseTypeName = p.tbCourse.tbCourseType.CourseTypeName
                                 }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ElectiveRule.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                //课程关系规则
                var electiveRule = (Code.EnumHelper.ElectiveRule)Enum.Parse(typeof(Code.EnumHelper.ElectiveRule), vm.RuleId.ToString());

                var courseIds = Request.Form["CboxId"].ConvertToString().Split(',').Select(p => p.ConvertToInt()).ToList();
                courseIds.RemoveAll(p => p == 0);
                var tb = (from p in db.Table<Entity.tbElectiveRule>() where p.tbCourse.Id == vm.CourseId && p.ElectiveRule == electiveRule select p);
                foreach (var courseRule in tb)
                {
                    courseRule.IsDeleted = true;
                }

                if (courseIds != null && courseIds.Any())
                {
                    courseIds.Add(vm.CourseId);
                    //选择的课程列表
                    var courseTargetList = (from p in db.Table<Course.Entity.tbCourse>() where courseIds.Contains(p.Id) select p).ToList();
                    var electiveRuleList = new List<Entity.tbElectiveRule>();

                    var tbElective = db.Set<Entity.tbElective>().Find(vm.ElectiveId);
                    //var tbCourse = db.Set<Course.Entity.tbCourse>().Find(vm.ElectiveRuleEdit.CourseId);

                    var tbCourse = courseTargetList.FirstOrDefault(p => p.Id == vm.CourseId);
                    courseTargetList.Remove(tbCourse);

                    foreach (var courseTarget in courseTargetList)
                    {
                        electiveRuleList.Add(new Entity.tbElectiveRule()
                        {
                            tbElective = tbElective,
                            tbCourse = tbCourse,
                            tbCourseTarget = courseTarget,
                            ElectiveRule = electiveRule
                        });
                    }
                    db.Set<Entity.tbElectiveRule>().AddRange(electiveRuleList);
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了课程关系");
                }
                return Code.MvcHelper.Post();
            }
        }
    }
}