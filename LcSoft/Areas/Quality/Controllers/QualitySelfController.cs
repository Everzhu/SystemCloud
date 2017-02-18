using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace XkSystem.Areas.Quality.Controllers
{
    public class QualitySelfController : Controller
    {
        public ActionResult List()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Teacher)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualitySelf.List();

                //获取学期信息
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Term);
                //获取当前激活学段
                var section = (from p in db.Table<Basis.Entity.tbYear>()
                               .Include(d => d.tbYearParent)
                               where p.IsDeleted == false
                               && p.IsDefault == true
                               && (vm.YearId == 0 ? true : p.tbYearParent.Id == vm.YearId)
                               select p).FirstOrDefault();
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.Where(d => (section != null ? d.Value == section.tbYearParent.Id.ToString() : true)).FirstOrDefault().Value.ConvertToInt();
                }
                //获取学年信息
                var yearId = (from p in db.Table<Basis.Entity.tbYear>()
                              where p.Id == vm.YearId
                              select p.tbYearParent.Id).FirstOrDefault();

                //获取教师所在行政班信息
                vm.ClassList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                where p.IsDeleted == false
                                && p.tbClass.IsDeleted == false
                                && p.tbTeacher.IsDeleted == false
                                && p.tbTeacher.tbSysUser.IsDeleted == false
                                && p.tbClass.tbGrade.IsDeleted == false
                                && p.tbClass.tbYear.IsDeleted == false
                                && p.tbClass.tbYear.Id == yearId
                                && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                select new System.Web.Mvc.SelectListItem()
                                {
                                    Text = p.tbClass.ClassName,
                                    Value = p.tbClass.Id.ToString(),
                                }).Distinct().ToList();
                //默认取第一个行政班Id去查询后面数据
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                //获取学生所在行政班信息
                var studentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                   .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                                  && p.tbClass.IsDeleted == false
                                  && p.tbStudent.IsDeleted == false
                                  && p.tbStudent.tbSysUser.IsDeleted == false
                                  && p.tbClass.Id == vm.ClassId
                                   select p).ToList();

                var studentIdList = studentList.Select(d => d.tbStudent.Id).ToList();
                //自评
                var selfList = (from p in db.Table<Quality.Entity.tbQualitySelf>()
                                .Include(d => d.tbStudent)
                                where p.IsDeleted == false
                            && p.tbYear.IsDeleted == false
                            && studentIdList.Contains(p.tbStudent.Id)
                            && p.tbYear.Id == vm.YearId
                                select new Dto.QualitySelf.Input
                                {
                                    Id = p.tbStudent.Id,
                                }).ToList();
                //学期期待
                var planList = (from p in db.Table<Quality.Entity.tbQualityPlan>()
                                .Include(d => d.tbStudent)
                                where p.IsDeleted == false
                            && p.tbYear.IsDeleted == false
                             && studentIdList.Contains(p.tbStudent.Id)
                            && p.tbYear.Id == vm.YearId
                                select new Dto.QualitySelf.Input
                                {
                                    Id = p.tbStudent.Id,
                                }).ToList();
                //学期总结
                var summaryList = (from p in db.Table<Quality.Entity.tbQualitySummary>()
                                   .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                                   && p.tbYear.IsDeleted == false
                                    && studentIdList.Contains(p.tbStudent.Id)
                                   && p.tbYear.Id == vm.YearId
                                   select new Dto.QualitySelf.Input
                                   {
                                       Id = p.tbStudent.Id,
                                   }).ToList();
                vm.QualitySelfList = (from p in studentList
                                      select new Dto.QualitySelf.List
                                      {
                                          Id = p.tbStudent.Id,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName,
                                          SelfName = selfList.Where(d => d.Id == p.tbStudent.Id).FirstOrDefault() != null ? "已评" : "未评",
                                          PlanName = planList.Where(d => d.Id == p.tbStudent.Id).FirstOrDefault() != null ? "已评" : "未评",
                                          SummaryName = summaryList.Where(d => d.Id == p.tbStudent.Id).FirstOrDefault() != null ? "已评" : "未评",
                                      }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.QualitySelf.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { yearId = vm.YearId, classId = vm.ClassId, }));
        }

        public ActionResult QualityView()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualitySelf.QualityView();

                //获取行政班信息
                var cla = (from p in db.Table<Basis.Entity.tbClass>()
                           where p.IsDeleted == false
                           && p.tbGrade.IsDeleted == false
                           && p.tbYear.IsDeleted == false
                           && p.Id == vm.ClassId
                           select p).FirstOrDefault();

                //获取学生信息
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && p.Id == vm.StudentId
                               select p).FirstOrDefault();

                if (cla != null && student != null)
                {
                    vm.StudentCode = student.StudentCode;
                    vm.StudentName = student.StudentName;
                    vm.ClassName = cla.ClassName;
                    //自评
                    var self = (from p in db.Table<Quality.Entity.tbQualitySelf>()
                                where p.IsDeleted == false
                                && p.tbYear.IsDeleted == false
                                && p.tbStudent.Id == student.Id
                                && p.tbYear.Id == vm.YearId
                                select new Dto.QualitySelf.QualityView
                                {
                                    Id = p.Id,
                                    Content = p.Content,
                                    Type = 1,
                                }).FirstOrDefault();
                    if (self != null)
                    {
                        vm.QualitySelfList.Add(self);
                    }
                    //学期期待
                    var plan = (from p in db.Table<Quality.Entity.tbQualityPlan>()
                                where p.IsDeleted == false
                                && p.tbYear.IsDeleted == false
                                && p.tbStudent.Id == student.Id
                                && p.tbYear.Id == vm.YearId
                                select new Dto.QualitySelf.QualityView
                                {
                                    Id = p.Id,
                                    Content = p.Content,
                                    Type = 2,
                                }).FirstOrDefault();
                    if (plan != null)
                    {
                        vm.QualitySelfList.Add(plan);
                    }
                    //学期总结
                    var summary = (from p in db.Table<Quality.Entity.tbQualitySummary>()
                                   where p.IsDeleted == false
                                   && p.tbYear.IsDeleted == false
                                   && p.tbStudent.Id == student.Id
                                   && p.tbYear.Id == vm.YearId
                                   select new Dto.QualitySelf.QualityView
                                   {
                                       Id = p.Id,
                                       Content = p.Content,
                                       Type = 3,
                                   }).FirstOrDefault();
                    if (summary != null)
                    {
                        vm.QualitySelfList.Add(summary);
                    }
                }
                return View(vm);
            }
        }

        public ActionResult Input()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Student)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualitySelf.Input();

                //获取学期
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Term);

                //获取当前激活学段
                var section = (from p in db.Table<Basis.Entity.tbYear>()
                               .Include(d => d.tbYearParent)
                               where p.IsDeleted == false
                               && p.IsDefault == true
                               && (vm.YearId == 0 ? true : p.tbYearParent.Id == vm.YearId)
                               select p).FirstOrDefault();
                if (vm.YearList.Count > 0 && vm.YearId == 0 && section != null && section.YearType == Code.EnumHelper.YearType.Section)
                {
                    vm.YearId = vm.YearList.Where(d => (section != null ? d.Value == section.tbYearParent.Id.ToString() : true)).FirstOrDefault().Value.ConvertToInt();
                }
                vm.YearDefault = section != null ? true : false;

                //获取学生信息
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && p.tbSysUser.Id == Code.Common.UserId
                               select p).FirstOrDefault();

                if (student != null)
                {
                    //自评
                    var self = (from p in db.Table<Quality.Entity.tbQualitySelf>()
                                where p.IsDeleted == false
                                && p.tbYear.IsDeleted == false
                                && p.tbStudent.Id == student.Id
                                && p.tbYear.Id == vm.YearId
                                select new Dto.QualitySelf.Input
                                {
                                    Id = p.Id,
                                    Content = p.Content,
                                    Type = 1,
                                }).FirstOrDefault();
                    if (self != null)
                    {
                        vm.QualitySelfList.Add(self);
                    }
                    //学期期待
                    var plan = (from p in db.Table<Quality.Entity.tbQualityPlan>()
                                where p.IsDeleted == false
                                && p.tbYear.IsDeleted == false
                                && p.tbStudent.Id == student.Id
                                && p.tbYear.Id == vm.YearId
                                select new Dto.QualitySelf.Input
                                {
                                    Id = p.Id,
                                    Content = p.Content,
                                    Type = 2,
                                }).FirstOrDefault();
                    if (plan != null)
                    {
                        vm.QualitySelfList.Add(plan);
                    }
                    //学期总结
                    var summary = (from p in db.Table<Quality.Entity.tbQualitySummary>()
                                   where p.IsDeleted == false
                                   && p.tbYear.IsDeleted == false
                                   && p.tbStudent.Id == student.Id
                                   && p.tbYear.Id == vm.YearId
                                   select new Dto.QualitySelf.Input
                                   {
                                       Id = p.Id,
                                       Content = p.Content,
                                       Type = 3,
                                   }).FirstOrDefault();
                    if (summary != null)
                    {
                        vm.QualitySelfList.Add(summary);
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Input(Models.QualitySelf.Input vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("Input", new { yearId = vm.YearId, }));
        }

        public ActionResult Edit(int id = 0, int type = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualitySelf.Edit();
                vm.Type = type;
                if (id != 0 && type == 1)//自评
                {
                    var tb = (from p in db.Table<Quality.Entity.tbQualitySelf>()
                              .Include(d => d.tbYear)
                              where p.Id == id
                              && p.tbStudent.IsDeleted == false
                              && p.tbStudent.tbSysUser.IsDeleted == false
                              && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                              select new Dto.QualitySelf.Edit
                              {
                                  Id = p.Id,
                                  Content = p.Content,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.QualitySelfEdit = tb;
                    }
                }
                else if (id != 0 && type == 2)//学期期待
                {
                    var tb = (from p in db.Table<Quality.Entity.tbQualityPlan>()
                              where p.Id == id
                              && p.tbStudent.IsDeleted == false
                              && p.tbStudent.tbSysUser.IsDeleted == false
                              && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                              select new Dto.QualitySelf.Edit
                              {
                                  Id = p.Id,
                                  Content = p.Content,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.QualitySelfEdit = tb;
                    }
                }
                else if (id != 0 && type == 3)//学期总结
                {
                    var tb = (from p in db.Table<Quality.Entity.tbQualitySummary>()
                              where p.Id == id
                              && p.tbStudent.IsDeleted == false
                              && p.tbStudent.tbSysUser.IsDeleted == false
                              && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                              select new Dto.QualitySelf.Edit
                              {
                                  Id = p.Id,
                                  Content = p.Content,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.QualitySelfEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Models.QualitySelf.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                int qualitySelfId = 0;

                if (error.Count == decimal.Zero)
                {
                    //获取学生信息
                    var student = (from p in db.Table<Student.Entity.tbStudent>()
                                   where p.IsDeleted == false
                                   && p.tbSysUser.IsDeleted == false
                                   && p.tbSysUser.Id == Code.Common.UserId
                                   select p).FirstOrDefault();
                    if (student != null)
                    {
                        //自评校验
                        if (vm.Type == 1 && vm.QualitySelfEdit.Content == null)
                        {
                            var errorMsg = new { Status = decimal.Zero, Message = "请填写完整的自评内容；" };
                            return Json(errorMsg);
                        }
                        if (vm.QualitySelfEdit.Id == 0)
                        {
                            if (vm.Type == 1)//自评
                            {
                                var tb = new Quality.Entity.tbQualitySelf();
                                tb.No = vm.QualitySelfEdit.No == null ? db.Table<Quality.Entity.tbQualitySelf>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualitySelfEdit.No;
                                tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                                tb.tbStudent = student;
                                tb.Content = vm.QualitySelfEdit.Content;
                                tb.InputDate = DateTime.Now;
                                db.Set<Quality.Entity.tbQualitySelf>().Add(tb);
                            }
                            else if (vm.Type == 2)//学期期待
                            {
                                var tb = new Quality.Entity.tbQualityPlan();
                                tb.No = vm.QualitySelfEdit.No == null ? db.Table<Quality.Entity.tbQualityPlan>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualitySelfEdit.No;
                                tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                                tb.tbStudent = student;
                                tb.Content = vm.QualitySelfEdit.Content;
                                tb.InputDate = DateTime.Now;
                                db.Set<Quality.Entity.tbQualityPlan>().Add(tb);
                            }
                            else if (vm.Type == 3)//学期总结
                            {
                                var tb = new Quality.Entity.tbQualitySummary();
                                tb.No = vm.QualitySelfEdit.No == null ? db.Table<Quality.Entity.tbQualitySummary>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualitySelfEdit.No;
                                tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                                tb.tbStudent = student;
                                tb.Content = vm.QualitySelfEdit.Content;
                                tb.InputDate = DateTime.Now;
                                db.Set<Quality.Entity.tbQualitySummary>().Add(tb);
                            }
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价");
                            }
                        }
                        else
                        {
                            if (vm.Type == 1)//自评
                            {
                                var tb = new Quality.Entity.tbQualitySelf();
                                tb = (from p in db.Table<Quality.Entity.tbQualitySelf>()
                                      where p.Id == vm.QualitySelfEdit.Id
                                      select p).FirstOrDefault();
                                if (tb != null)
                                {
                                    qualitySelfId = tb.Id;

                                    tb.No = vm.QualitySelfEdit.No == null ? db.Table<Quality.Entity.tbQualitySelf>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualitySelfEdit.No;
                                    tb.Content = vm.QualitySelfEdit.Content;
                                    //tb.InputDate = DateTime.Now;
                                    if (db.SaveChanges() > 0)
                                    {
                                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价");
                                    }
                                }
                            }
                            else if (vm.Type == 2)//学期期待
                            {
                                var tb = new Quality.Entity.tbQualityPlan();
                                tb = (from p in db.Table<Quality.Entity.tbQualityPlan>()
                                      where p.Id == vm.QualitySelfEdit.Id
                                      select p).FirstOrDefault();
                                if (tb != null)
                                {
                                    qualitySelfId = tb.Id;

                                    tb.No = vm.QualitySelfEdit.No == null ? db.Table<Quality.Entity.tbQualityPlan>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualitySelfEdit.No;
                                    tb.Content = vm.QualitySelfEdit.Content;
                                    //tb.InputDate = DateTime.Now;
                                    if (db.SaveChanges() > 0)
                                    {
                                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价");
                                    }
                                }
                            }
                            else if (vm.Type == 3)//学期总结
                            {
                                var tb = new Quality.Entity.tbQualitySummary();
                                tb = (from p in db.Table<Quality.Entity.tbQualitySummary>()
                                      where p.Id == vm.QualitySelfEdit.Id
                                      select p).FirstOrDefault();
                                if (tb != null)
                                {
                                    qualitySelfId = tb.Id;

                                    tb.No = vm.QualitySelfEdit.No == null ? db.Table<Quality.Entity.tbQualitySummary>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualitySelfEdit.No;
                                    tb.Content = vm.QualitySelfEdit.Content;
                                    //tb.InputDate = DateTime.Now;
                                    if (db.SaveChanges() > 0)
                                    {
                                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价");
                                    }
                                }
                            }
                        }
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }
    }
}