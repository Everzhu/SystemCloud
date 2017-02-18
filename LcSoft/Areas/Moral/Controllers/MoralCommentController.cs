using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralCommentController : Controller
    {
        #region 班主任评语
        // GET: Moral/MoralComment
        public ActionResult MoralClassList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralClassList();
                //获取学年
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
                //获取当前激活学段
                var section = (from p in db.Table<Basis.Entity.tbYear>()
                               .Include(d => d.tbYearParent.tbYearParent)
                               where p.IsDeleted == false
                               && p.YearType == Code.EnumHelper.YearType.Section
                               && p.IsDefault == true
                               select p).FirstOrDefault();
                if (vm.YearId == 0 && vm.YearList.Count() > 0 && section != null)
                {
                    vm.YearId = vm.YearList.Where(d => d.Value == section.tbYearParent.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }

                //获取行政班
                vm.ClassList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                where p.IsDeleted == false
                                && p.tbClass.IsDeleted == false
                                && p.tbTeacher.IsDeleted == false
                                && p.tbClass.tbYear.Id == vm.YearId
                                && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.tbClass.ClassName,
                                    Value = p.tbClass.Id.ToString(),
                                }).ToList();
                if (vm.ClassId == 0 && vm.ClassList.Count() > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.InputMonth == null || vm.InputMonth == "")
                {
                    vm.InputMonth = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString();
                }

                //获取学生所在行政班信息
                var studentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  .Include(d => d.tbStudent)
                                  .Include(d => d.tbClass)
                                   where p.IsDeleted == false
                                  && p.tbClass.IsDeleted == false
                                  && p.tbStudent.IsDeleted == false
                                  && p.tbStudent.tbSysUser.IsDeleted == false
                                  && p.tbClass.Id == vm.ClassId
                                   select p).ToList();

                var studentIdList = studentList.Select(d => d.tbStudent.Id).ToList();

                //班主任评语
                var commentList = (from p in db.Table<Moral.Entity.tbMoralComment>()
                                .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && studentIdList.Contains(p.tbStudent.Id)
                               && p.tbYear.Id == vm.YearId
                               && p.tbSysUser.Id == Code.Common.UserId
                               && p.InputMonth == vm.InputMonth
                                   select p).ToList();

                vm.MClassList = (from p in studentList
                                 select new Dto.MoralComment.MoralClassList
                                 {
                                     StudentId = p.tbStudent.Id,
                                     StudentCode = p.tbStudent.StudentCode,
                                     StudentName = p.tbStudent.StudentName,
                                     CommentType = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? 1 : 0),
                                     Comment = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault().Comment : ""),
                                 }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoralClassList(Models.MoralComment.MoralClassList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("MoralClassList", new { yearId = vm.YearId, classId = vm.ClassId, inputMonth = vm.InputMonth }));
        }

        public ActionResult MoralClassEdit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralClassEdit();
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && p.Id == vm.StudentId
                               select p).FirstOrDefault();
                if (student != null)
                {
                    vm.StudentCode = student.StudentCode;
                    vm.StudentName = student.StudentName;
                    //班主任评语
                    var tb = (from p in db.Table<Moral.Entity.tbMoralComment>()
                                .Include(d => d.tbStudent)
                              where p.IsDeleted == false
                              && p.tbYear.IsDeleted == false
                              && p.tbStudent.Id == student.Id
                              && p.tbYear.Id == vm.YearId
                              && p.tbSysUser.Id == Code.Common.UserId
                              && p.InputMonth == vm.InputMonth
                              select new Dto.MoralComment.MoralClassEdit
                              {
                                  Id = p.Id,
                                  Comment = p.Comment,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.MClassEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoralClassEdit(Models.MoralComment.MoralClassEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                if (error.Count == decimal.Zero)
                {
                    if (vm.MClassEdit.Id == 0)
                    {
                        var tb = new Moral.Entity.tbMoralComment();
                        tb.No = vm.MClassEdit.No == null ? db.Table<Moral.Entity.tbMoralComment>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MClassEdit.No;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                        tb.InputMonth = vm.InputMonth;
                        tb.Comment = vm.MClassEdit.Comment;
                        tb.InputDate = DateTime.Now;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Moral.Entity.tbMoralComment>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评语");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Moral.Entity.tbMoralComment>()
                                  where p.Id == vm.MClassEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.MClassEdit.No == null ? db.Table<Moral.Entity.tbMoralComment>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MClassEdit.No;
                            tb.Comment = vm.MClassEdit.Comment;
                            tb.InputMonth = vm.InputMonth;
                            tb.UpdateTime = DateTime.Now;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评语");
                            }
                        }
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }
        #endregion

        #region 我的心得
        public ActionResult MoralStudentList()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Student)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralStudentList();

                //获取当前激活学段
                var section = (from p in db.Table<Basis.Entity.tbYear>()
                               .Include(d => d.tbYearParent)
                               where p.IsDeleted == false
                               && p.IsDefault == true
                               && (vm.YearId == 0 ? true : p.tbYearParent.Id == vm.YearId)
                               select p).FirstOrDefault();
                if (section != null && section.YearType == Code.EnumHelper.YearType.Section)
                {
                    vm.YearId = section.tbYearParent.Id;
                }

                if (vm.Week == null || vm.Week == "")
                {
                    vm.Week = DateTime.Now.ToShortDateString();
                }
                var week = DateTime.Parse(vm.Week);
                DateTime startWeek = week.AddDays(1 - Convert.ToInt32(week.DayOfWeek.ToString("d")));  //本周周一
                DateTime endWeek = startWeek.AddDays(6);

                //根据家长获取学生信息
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               .Include(d => d.tbSysUser)
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && p.tbSysUser.Id == Code.Common.UserId
                               select p).FirstOrDefault();
                if (student != null)
                {
                    //我的心得
                    var commentList = (from p in db.Table<Moral.Entity.tbMoralSelf>()
                                    .Include(d => d.tbStudent)
                                       where p.IsDeleted == false
                                   && p.tbYear.IsDeleted == false
                                   && p.tbStudent.Id == student.Id
                                   && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                       select p).ToList();

                    commentList = (from p in commentList
                                   where (startWeek <= p.Week.ConvertToDateTime() && p.Week.ConvertToDateTime() <= endWeek)
                                   select p).ToList();

                    var qualityFamily = new Dto.MoralComment.MoralStudentList();
                    qualityFamily.StudentId = student.Id;
                    qualityFamily.StudentCode = student.StudentCode;
                    qualityFamily.StudentName = student.StudentName;
                    qualityFamily.CommentType = (commentList.Where(d => d.tbStudent.Id == student.Id).FirstOrDefault() != null ? 1 : 0);
                    qualityFamily.Comment = (commentList.Where(d => d.tbStudent.Id == student.Id).FirstOrDefault() != null ? commentList.Where(d => d.tbStudent.Id == student.Id).FirstOrDefault().Content : "");
                    vm.MStudentList.Add(qualityFamily);
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoralStudentList(Models.MoralComment.MoralStudentList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("MoralStudentList", new { week = vm.Week, }));
        }

        public ActionResult MoralStudentEdit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralStudentEdit();
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && p.Id == vm.StudentId
                               select p).FirstOrDefault();

                var week = DateTime.Parse(vm.Week);
                DateTime startWeek = DateTime.Parse(vm.Week).AddDays(1 - Convert.ToInt32(DateTime.Parse(vm.Week).DayOfWeek.ToString("d")));  //本周周一
                DateTime endWeek = startWeek.AddDays(6);

                if (student != null)
                {
                    vm.StudentCode = student.StudentCode;
                    vm.StudentName = student.StudentName;
                    //我的心得
                    var tb = (from p in db.Table<Moral.Entity.tbMoralSelf>()
                                .Include(d => d.tbStudent)
                              where p.IsDeleted == false
                              && p.tbYear.IsDeleted == false
                              && p.tbStudent.Id == student.Id
                              && p.tbYear.Id == vm.YearId
                              && p.tbStudent.Id == student.Id
                              && (startWeek <= week && week <= endWeek)
                              select new Dto.MoralComment.MoralStudentEdit
                              {
                                  Id = p.Id,
                                  Comment = p.Content,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.MStudentEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoralStudentEdit(Models.MoralComment.MoralStudentEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                if (error.Count == decimal.Zero)
                {
                    if (vm.MStudentEdit.Id == 0)
                    {
                        var tb = new Moral.Entity.tbMoralSelf();
                        tb.No = vm.MStudentEdit.No == null ? db.Table<Moral.Entity.tbMoralSelf>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MStudentEdit.No;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                        tb.Content = vm.MStudentEdit.Comment;
                        tb.Week = vm.Week;
                        tb.InputDate = DateTime.Now;
                        db.Set<Moral.Entity.tbMoralSelf>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加心得");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Moral.Entity.tbMoralSelf>()
                                  where p.Id == vm.MStudentEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.MStudentEdit.No == null ? db.Table<Moral.Entity.tbMoralSelf>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MStudentEdit.No;
                            tb.Content = vm.MStudentEdit.Comment;
                            tb.Week = vm.Week;
                            tb.UpdateTime = DateTime.Now;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改心得");
                            }
                        }
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }
        #endregion

        #region 学生心得
        public ActionResult MoralTeacherList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralTeacherList();

                //获取学年
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
                //获取当前激活学段
                var section = (from p in db.Table<Basis.Entity.tbYear>()
                               .Include(d => d.tbYearParent.tbYearParent)
                               where p.IsDeleted == false
                               && p.YearType == Code.EnumHelper.YearType.Section
                               && p.IsDefault == true
                               select p).FirstOrDefault();
                if (vm.YearId == 0 && vm.YearList.Count() > 0 && section != null)
                {
                    vm.YearId = vm.YearList.Where(d => d.Value == section.tbYearParent.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }

                //获取行政班
                vm.ClassList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                where p.IsDeleted == false
                                && p.tbClass.IsDeleted == false
                                && p.tbTeacher.IsDeleted == false
                                && p.tbClass.tbYear.Id == vm.YearId
                                && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.tbClass.ClassName,
                                    Value = p.tbClass.Id.ToString(),
                                }).ToList();
                if (vm.ClassId == 0 && vm.ClassList.Count() > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.Week == null || vm.Week == "")
                {
                    vm.Week = DateTime.Now.ToShortDateString();
                }
                var week = DateTime.Parse(vm.Week);
                DateTime startWeek = week.AddDays(1 - Convert.ToInt32(week.DayOfWeek.ToString("d")));  //本周周一
                DateTime endWeek = startWeek.AddDays(6);

                //获取学生所在行政班信息
                var studentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  .Include(d => d.tbStudent)
                                  .Include(d => d.tbClass)
                                   where p.IsDeleted == false
                                  && p.tbClass.IsDeleted == false
                                  && p.tbStudent.IsDeleted == false
                                  && p.tbStudent.tbSysUser.IsDeleted == false
                                  && p.tbClass.Id == vm.ClassId
                                   select p).ToList();

                var studentIdList = studentList.Select(d => d.tbStudent.Id).ToList();

                //学生心得
                var commentList = (from p in db.Table<Moral.Entity.tbMoralSelf>()
                                .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && studentIdList.Contains(p.tbStudent.Id)
                                   //&& p.tbYear.Id == vm.YearId
                                   select p).ToList();

                commentList = (from p in commentList
                               where (startWeek <= p.Week.ConvertToDateTime() && p.Week.ConvertToDateTime() <= endWeek)
                               select p).ToList();

                vm.MTeacherList = (from p in studentList
                                   select new Dto.MoralComment.MoralClassList
                                   {
                                       StudentId = p.tbStudent.Id,
                                       StudentCode = p.tbStudent.StudentCode,
                                       StudentName = p.tbStudent.StudentName,
                                       CommentType = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? 1 : 0),
                                       Comment = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault().Content : ""),
                                   }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoralTeacherList(Models.MoralComment.MoralTeacherList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("MoralTeacherList", new { yearId = vm.YearId, classId = vm.ClassId, week = vm.Week }));
        }

        public ActionResult MoralStudentView()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralStudentView();

                var week = DateTime.Parse(vm.Week);
                DateTime startWeek = week.AddDays(1 - Convert.ToInt32(week.DayOfWeek.ToString("d")));  //本周周一
                DateTime endWeek = startWeek.AddDays(6);

                var commentList = (from p in db.Table<Moral.Entity.tbMoralSelf>()
                               .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && p.tbStudent.Id == vm.StudentId
                                   //&& p.tbYear.Id == vm.YearId
                                   select p).ToList();

                vm.MStudentView = (from p in commentList
                                   where (startWeek <= p.Week.ConvertToDateTime() && p.Week.ConvertToDateTime() <= endWeek)
                                   select new Dto.MoralComment.MoralStudentView
                                   {
                                       Comment = p.Content,
                                       StudentName = p.tbStudent.StudentName,
                                   }).FirstOrDefault();

                return View(vm);
            }
        }
        #endregion

        #region 家长评语
        public ActionResult MoralFamilyList()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Family)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralFamilyList();

                //获取学期信息
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
                    vm.YearId = vm.YearList.Where(d => d.Value == section.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }

                vm.YearDefault = section != null ? true : false;

                //根据家长获取学生信息
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               .Include(d => d.tbSysUser)
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && (p.tbSysUser.Id == Code.Common.UserId || p.tbSysUserFamily.Id == Code.Common.UserId)
                               select p).FirstOrDefault();
                if (student != null)
                {
                    //家长评语
                    var commentList = (from p in db.Table<Entity.tbMoralFamily>()
                                    .Include(d => d.tbStudent)
                                       where p.IsDeleted == false
                                   && p.tbYear.IsDeleted == false
                                   && p.tbStudent.Id == student.Id
                                   && p.tbYear.Id == vm.YearId
                                   && p.tbSysUser.Id == Code.Common.UserId
                                       select p).ToList();

                    var moralFamily = new Dto.MoralComment.MoralFamilyList();
                    moralFamily.StudentId = student.Id;
                    moralFamily.StudentCode = student.StudentCode;
                    moralFamily.StudentName = student.StudentName;
                    moralFamily.CommentType = (commentList.Where(d => d.tbStudent.Id == student.Id).FirstOrDefault() != null ? 1 : 0);
                    moralFamily.Comment = (commentList.Where(d => d.tbStudent.Id == student.Id).FirstOrDefault() != null ? commentList.Where(d => d.tbStudent.Id == student.Id).FirstOrDefault().Content : "");
                    vm.MFamilyList.Add(moralFamily);
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoralFamilyList(Models.MoralComment.MoralFamilyList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("MoralFamilyList", new { yearId = vm.YearId, }));
        }

        public ActionResult MoralFamilyEdit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralFamilyEdit();
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && p.Id == vm.StudentId
                               select p).FirstOrDefault();
                if (student != null)
                {
                    vm.StudentCode = student.StudentCode;
                    vm.StudentName = student.StudentName;
                    //家长评语
                    var tb = (from p in db.Table<Entity.tbMoralFamily>()
                                .Include(d => d.tbStudent)
                              where p.IsDeleted == false
                              && p.tbYear.IsDeleted == false
                              && p.tbStudent.Id == student.Id
                              && p.tbYear.Id == vm.YearId
                              && p.tbSysUser.Id == Code.Common.UserId
                              select new Dto.MoralComment.MoralFamilyEdit
                              {
                                  Id = p.Id,
                                  Comment = p.Content,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.MFamilyEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoralFamilyEdit(Models.MoralComment.MoralFamilyEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                if (error.Count == decimal.Zero)
                {
                    if (vm.MFamilyEdit.Id == 0)
                    {
                        var tb = new Entity.tbMoralFamily();
                        tb.No = vm.MFamilyEdit.No == null ? db.Table<Entity.tbMoralFamily>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MFamilyEdit.No;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                        tb.Content = vm.MFamilyEdit.Comment;
                        tb.InputDate = DateTime.Now;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Entity.tbMoralFamily>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评语");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Entity.tbMoralFamily>()
                                  where p.Id == vm.MFamilyEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.MFamilyEdit.No == null ? db.Table<Entity.tbMoralFamily>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MFamilyEdit.No;
                            tb.Content = vm.MFamilyEdit.Comment;
                            tb.UpdateTime = DateTime.Now;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评语");
                            }
                        }
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }
        #endregion

        #region 学生家长评语
        public ActionResult FamilyTeacherList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.FamilyTeacherList();

                //获取学年
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
                //获取当前激活学段
                var section = (from p in db.Table<Basis.Entity.tbYear>()
                               .Include(d => d.tbYearParent.tbYearParent)
                               where p.IsDeleted == false
                               && p.YearType == Code.EnumHelper.YearType.Section
                               && p.IsDefault == true
                               select p).FirstOrDefault();
                if (vm.YearId == 0 && vm.YearList.Count() > 0 && section != null)
                {
                    vm.YearId = vm.YearList.Where(d => d.Value == section.tbYearParent.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }

                //获取学期
                vm.YearTermList = (from p in db.Table<Basis.Entity.tbYear>()
                                   where p.IsDeleted == false
                                   && p.YearType == Code.EnumHelper.YearType.Term
                                   && p.tbYearParent.Id == vm.YearId
                                   select new System.Web.Mvc.SelectListItem
                                   {
                                       Text = p.YearName,
                                       Value = p.Id.ToString(),
                                   }).ToList();
                if (vm.YearTermId == 0 && vm.YearTermList.Count() > 0 && section != null)
                {
                    vm.YearTermId = vm.YearTermList.Where(d => d.Value == section.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }

                //获取行政班
                vm.ClassList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                where p.IsDeleted == false
                                && p.tbClass.IsDeleted == false
                                && p.tbTeacher.IsDeleted == false
                                && p.tbClass.tbYear.Id == vm.YearId
                                && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.tbClass.ClassName,
                                    Value = p.tbClass.Id.ToString(),
                                }).ToList();
                if (vm.ClassId == 0 && vm.ClassList.Count() > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                //获取学生所在行政班信息
                var studentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  .Include(d => d.tbStudent)
                                  .Include(d => d.tbClass)
                                   where p.IsDeleted == false
                                  && p.tbClass.IsDeleted == false
                                  && p.tbStudent.IsDeleted == false
                                  && p.tbStudent.tbSysUser.IsDeleted == false
                                  && p.tbClass.Id == vm.ClassId
                                   select p).ToList();

                var studentIdList = studentList.Select(d => d.tbStudent.Id).ToList();

                //家长评语
                var commentList = (from p in db.Table<Moral.Entity.tbMoralFamily>()
                                .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && studentIdList.Contains(p.tbStudent.Id)
                               && p.tbYear.Id == vm.YearTermId
                                   select p).ToList();

                vm.FTeacherList = (from p in studentList
                                   select new Dto.MoralComment.FamilyTeacherList
                                   {
                                       StudentId = p.tbStudent.Id,
                                       StudentCode = p.tbStudent.StudentCode,
                                       StudentName = p.tbStudent.StudentName,
                                       CommentType = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? 1 : 0),
                                       Comment = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault().Content : ""),
                                   }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FamilyTeacherList(Models.MoralComment.FamilyTeacherList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("FamilyTeacherList", new { yearId = vm.YearId, yearTermId = vm.YearTermId, classId = vm.ClassId }));
        }

        public ActionResult FamilyStudentView()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.FamilyStudentView();

                var commentList = (from p in db.Table<Moral.Entity.tbMoralFamily>()
                               .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && p.tbStudent.Id == vm.StudentId
                              && p.tbYear.Id == vm.YearTermId
                                   select p).ToList();

                vm.FStudentView = (from p in commentList
                                   select new Dto.MoralComment.FamilyStudentView
                                   {
                                       Comment = p.Content,
                                       StudentName = p.tbStudent.StudentName,
                                   }).FirstOrDefault();

                return View(vm);
            }
        }
        #endregion

        #region 孩子家中情况
        public ActionResult MoralHappeningList()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Family)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralHappeningList();

                //获取学期信息
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
                    vm.YearId = vm.YearList.Where(d => d.Value == section.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.InputDate == null || vm.InputDate == "")
                {
                    vm.InputDate = DateTime.Now.ToShortDateString();
                }
                var inputDate = vm.InputDate.ConvertToDateTime();

                //根据家长获取学生信息
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               .Include(d => d.tbSysUser)
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && (p.tbSysUser.Id == Code.Common.UserId || p.tbSysUserFamily.Id == Code.Common.UserId)
                               select p).FirstOrDefault();
                if (student != null)
                {
                    //孩子家中情况
                    var comment = (from p in db.Table<Entity.tbMoralHappening>()
                                    .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && p.tbStudent.Id == student.Id
                               && p.tbYear.Id == vm.YearId
                               && p.tbSysUser.Id == Code.Common.UserId
                               && p.InputDate == inputDate
                                   select p).FirstOrDefault();

                    var moralFamily = new Dto.MoralComment.MoralHappeningList();
                    moralFamily.StudentId = student.Id;
                    moralFamily.StudentCode = student.StudentCode;
                    moralFamily.StudentName = student.StudentName;
                    moralFamily.Comment = (comment != null ? comment.Content : "");
                    vm.MHappeningList.Add(moralFamily);

                    if (comment != null)
                    {
                        vm.HappeningId = comment.Id;
                        vm.HStudentViewList = (from p in db.Table<Entity.tbMoralHappeningReply>()
                                               where p.IsDeleted == false
                                               && p.tbMoralHappening.IsDeleted == false
                                               && p.tbMoralHappening.Id == comment.Id
                                               orderby p.InputDate descending
                                               select new Dto.MoralComment.HappeningStudentView
                                               {
                                                   UserName = p.tbSysUser.UserName,
                                                   ReplyComment = p.Content,
                                                   ReplyDate = p.InputDate,
                                               }).ToList();
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoralHappeningList(Models.MoralComment.MoralHappeningList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("MoralHappeningList", new { yearId = vm.YearId, inputDate = vm.InputDate }));
        }

        public ActionResult MoralHappeningEdit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralHappeningEdit();
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && p.Id == vm.StudentId
                               select p).FirstOrDefault();
                if (student != null)
                {
                    vm.StudentCode = student.StudentCode;
                    vm.StudentName = student.StudentName;
                    var inputDate = vm.InputDate.ConvertToDateTime();
                    //孩子家中情况
                    var tb = (from p in db.Table<Entity.tbMoralHappening>()
                                .Include(d => d.tbStudent)
                              where p.IsDeleted == false
                              && p.tbYear.IsDeleted == false
                              && p.tbStudent.Id == student.Id
                              && p.tbYear.Id == vm.YearId
                              && p.tbSysUser.Id == Code.Common.UserId
                              && p.InputDate == inputDate
                              select new Dto.MoralComment.MoralHappeningEdit
                              {
                                  Id = p.Id,
                                  Comment = p.Content,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.MHappeningEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoralHappeningEdit(Models.MoralComment.MoralHappeningEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                if (error.Count == decimal.Zero)
                {
                    if (vm.MHappeningEdit.Id == 0)
                    {
                        var tb = new Entity.tbMoralHappening();
                        tb.No = vm.MHappeningEdit.No == null ? db.Table<Entity.tbMoralHappening>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MHappeningEdit.No;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                        tb.Content = vm.MHappeningEdit.Comment;
                        tb.InputDate = vm.InputDate.ConvertToDateTime();
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Entity.tbMoralHappening>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加家中情况");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Entity.tbMoralHappening>()
                                  where p.Id == vm.MHappeningEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.MHappeningEdit.No == null ? db.Table<Entity.tbMoralHappening>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MHappeningEdit.No;
                            tb.Content = vm.MHappeningEdit.Comment;
                            tb.UpdateTime = DateTime.Now;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改家中情况");
                            }
                        }
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HappeningReplySave(Models.MoralComment.MoralHappeningList vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                var resultArea = new List<int>();
                if (vm.HappeningId > 0)
                {
                    if (Request["textareaText"] == null || Request["textareaText"] == "")
                    {
                        error.AddError("请填写回复内容!");
                    }
                }
                else
                {
                    error.AddError("请填写回复内容!");
                }
                #region 保存结果
                var tb = new Entity.tbMoralHappeningReply();
                tb.tbMoralHappening = db.Set<Entity.tbMoralHappening>().Find(vm.HappeningId);
                tb.Content = Request["textareaText"];
                tb.InputDate = DateTime.Now;
                tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                db.Set<Entity.tbMoralHappeningReply>().Add(tb);
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加回复");
                }
                #endregion
                return Code.MvcHelper.Post(error, null, "提交成功!");
            }
        }
        #endregion

        #region 学生家中情况
        public ActionResult HappeningTeacherList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralHappeningList();

                //获取学年
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
                //获取当前激活学段
                var section = (from p in db.Table<Basis.Entity.tbYear>()
                               .Include(d => d.tbYearParent.tbYearParent)
                               where p.IsDeleted == false
                               && p.YearType == Code.EnumHelper.YearType.Section
                               && p.IsDefault == true
                               select p).FirstOrDefault();
                if (vm.YearId == 0 && vm.YearList.Count() > 0 && section != null)
                {
                    vm.YearId = vm.YearList.Where(d => d.Value == section.tbYearParent.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }

                //获取行政班
                vm.ClassList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                where p.IsDeleted == false
                                && p.tbClass.IsDeleted == false
                                && p.tbTeacher.IsDeleted == false
                                && p.tbClass.tbYear.Id == vm.YearId
                                && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.tbClass.ClassName,
                                    Value = p.tbClass.Id.ToString(),
                                }).ToList();
                if (vm.ClassId == 0 && vm.ClassList.Count() > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.InputDate == null || vm.InputDate == "")
                {
                    vm.InputDate = DateTime.Now.ToShortDateString();
                }
                var inputDate = vm.InputDate.ConvertToDateTime();

                //获取学生所在行政班信息
                var studentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  .Include(d => d.tbStudent)
                                  .Include(d => d.tbClass)
                                   where p.IsDeleted == false
                                  && p.tbClass.IsDeleted == false
                                  && p.tbStudent.IsDeleted == false
                                  && p.tbStudent.tbSysUser.IsDeleted == false
                                  && p.tbClass.Id == vm.ClassId
                                   select p).ToList();

                var studentIdList = studentList.Select(d => d.tbStudent.Id).ToList();

                //家长评语
                var commentList = (from p in db.Table<Moral.Entity.tbMoralHappening>()
                                .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && studentIdList.Contains(p.tbStudent.Id)
                                && p.InputDate == inputDate
                                   select p).ToList();

                vm.MHappeningList = (from p in studentList
                                     select new Dto.MoralComment.MoralHappeningList
                                     {
                                         StudentId = p.tbStudent.Id,
                                         StudentCode = p.tbStudent.StudentCode,
                                         StudentName = p.tbStudent.StudentName,
                                         CommentType = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? 1 : 0),
                                         Comment = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault().Content : ""),
                                     }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HappeningTeacherList(Models.MoralComment.MoralHappeningList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("HappeningTeacherList", new { yearId = vm.YearId, classId = vm.ClassId, inputDate = vm.InputDate }));
        }

        public ActionResult HappeningStudentView()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.HappeningStudentView();
                var inputDate = vm.InputDate.ConvertToDateTime();
                var commentList = (from p in db.Table<Moral.Entity.tbMoralHappening>()
                               .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && p.tbStudent.Id == vm.StudentId
                              && p.InputDate == inputDate
                                   select p).ToList();

                vm.HStudentView = (from p in commentList
                                   select new Dto.MoralComment.HappeningStudentView
                                   {
                                       HappeningId = p.Id,
                                       Comment = p.Content,
                                       StudentName = p.tbStudent.StudentName,
                                   }).FirstOrDefault();

                if (vm.HStudentView != null)
                {
                    vm.HStudentViewList = (from p in db.Table<Entity.tbMoralHappeningReply>()
                                           where p.IsDeleted == false
                                           && p.tbMoralHappening.IsDeleted == false
                                           && p.tbMoralHappening.Id == vm.HStudentView.HappeningId
                                           orderby p.InputDate descending
                                           select new Dto.MoralComment.HappeningStudentView
                                           {
                                               UserName = p.tbSysUser.UserName,
                                               ReplyComment = p.Content,
                                               ReplyDate = p.InputDate,
                                           }).ToList();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HappeningStudentView(Models.MoralComment.HappeningStudentView vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                var resultArea = new List<int>();
                if (vm.HStudentView.HappeningId > 0)
                {
                    if (Request["textareaText"] == null || Request["textareaText"] == "")
                    {
                        error.AddError("请填写回复内容!");
                    }
                }
                else
                {
                    error.AddError("请填写回复内容!");
                }
                var tb = new Entity.tbMoralHappeningReply();
                tb.No = vm.HStudentView.No == null ? db.Table<Entity.tbMoralHappeningReply>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.HStudentView.No;
                tb.tbMoralHappening = db.Set<Entity.tbMoralHappening>().Find(vm.HStudentView.HappeningId);
                tb.Content = Request["textareaText"];
                tb.InputDate = DateTime.Now;
                tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                db.Set<Entity.tbMoralHappeningReply>().Add(tb);
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加回复");
                }
                return Code.MvcHelper.Post(error, "HappeningStudentView?studentId=" + vm.StudentId + "&yearId=" + vm.YearId + "&inputDate=" + vm.InputDate, "提交成功!");
            }
        }
        #endregion

        #region 家长意见与建议
        public ActionResult MoralSuggestList()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Family)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralSuggestList();

                //获取学期信息
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
                    vm.YearId = vm.YearList.Where(d => d.Value == section.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.InputDate == null || vm.InputDate == "")
                {
                    vm.InputDate = DateTime.Now.ToShortDateString();
                }
                var inputDate = vm.InputDate.ConvertToDateTime();

                //根据家长获取学生信息
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               .Include(d => d.tbSysUser)
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && (p.tbSysUser.Id == Code.Common.UserId || p.tbSysUserFamily.Id == Code.Common.UserId)
                               select p).FirstOrDefault();
                if (student != null)
                {
                    //家长意见与建议
                    var comment = (from p in db.Table<Entity.tbMoralSuggest>()
                                    .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && p.tbStudent.Id == student.Id
                               && p.tbYear.Id == vm.YearId
                               && p.tbSysUser.Id == Code.Common.UserId
                               && p.InputDate == inputDate
                                   select p).FirstOrDefault();

                    var moralFamily = new Dto.MoralComment.MoralSuggestList();
                    moralFamily.StudentId = student.Id;
                    moralFamily.StudentCode = student.StudentCode;
                    moralFamily.StudentName = student.StudentName;
                    moralFamily.Comment = comment != null ? comment.Content : "";
                    vm.MSuggestList.Add(moralFamily);

                    if (comment != null)
                    {
                        vm.SuggestId = comment.Id;
                        vm.SStudentViewList = (from p in db.Table<Entity.tbMoralSuggestReply>()
                                               where p.IsDeleted == false
                                               && p.tbMoralSuggest.IsDeleted == false
                                               && p.tbMoralSuggest.Id == comment.Id
                                               orderby p.InputDate descending
                                               select new Dto.MoralComment.SuggestStudentView
                                               {
                                                   UserName = p.tbSysUser.UserName,
                                                   ReplyComment = p.Content,
                                                   ReplyDate = p.InputDate,
                                               }).ToList();
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoralSuggestList(Models.MoralComment.MoralSuggestList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("MoralSuggestList", new { yearId = vm.YearId, inputDate = vm.InputDate }));
        }

        public ActionResult MoralSuggestEdit()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralSuggestEdit();
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && p.Id == vm.StudentId
                               select p).FirstOrDefault();
                if (student != null)
                {
                    vm.StudentCode = student.StudentCode;
                    vm.StudentName = student.StudentName;
                    var inputDate = vm.InputDate.ConvertToDateTime();
                    //孩子家中情况
                    var tb = (from p in db.Table<Entity.tbMoralSuggest>()
                                .Include(d => d.tbStudent)
                              where p.IsDeleted == false
                              && p.tbYear.IsDeleted == false
                              && p.tbStudent.Id == student.Id
                              && p.tbYear.Id == vm.YearId
                              && p.tbSysUser.Id == Code.Common.UserId
                              && p.InputDate == inputDate
                              select new Dto.MoralComment.MoralSuggestEdit
                              {
                                  Id = p.Id,
                                  Comment = p.Content,
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.MSuggestEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoralSuggestEdit(Models.MoralComment.MoralSuggestEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                if (error.Count == decimal.Zero)
                {
                    if (vm.MSuggestEdit.Id == 0)
                    {
                        var tb = new Entity.tbMoralSuggest();
                        tb.No = vm.MSuggestEdit.No == null ? db.Table<Entity.tbMoralSuggest>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MSuggestEdit.No;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                        tb.Content = vm.MSuggestEdit.Comment;
                        tb.InputDate = vm.InputDate.ConvertToDateTime();
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Entity.tbMoralSuggest>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加家中情况");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Entity.tbMoralSuggest>()
                                  where p.Id == vm.MSuggestEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.MSuggestEdit.No == null ? db.Table<Entity.tbMoralHappening>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.MSuggestEdit.No;
                            tb.Content = vm.MSuggestEdit.Comment;
                            tb.UpdateTime = DateTime.Now;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改家中情况");
                            }
                        }
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuggestReplySave(Models.MoralComment.MoralSuggestList vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                var resultArea = new List<int>();
                if (vm.SuggestId > 0)
                {
                    if (Request["textareaText"] == null || Request["textareaText"] == "")
                    {
                        error.AddError("请填写回复内容!");
                    }
                }
                else
                {
                    error.AddError("请填写回复内容!");
                }
                #region 保存结果
                var tb = new Entity.tbMoralSuggestReply();
                tb.tbMoralSuggest = db.Set<Entity.tbMoralSuggest>().Find(vm.SuggestId);
                tb.Content = Request["textareaText"];
                tb.InputDate = DateTime.Now;
                tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                db.Set<Entity.tbMoralSuggestReply>().Add(tb);
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加回复");
                }
                #endregion
                return Code.MvcHelper.Post(error, null, "提交成功!");
            }
        }
        #endregion

        #region 学生家长意见与建议
        public ActionResult SuggestTeacherList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.MoralSuggestList();

                //获取学年
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
                //获取当前激活学段
                var section = (from p in db.Table<Basis.Entity.tbYear>()
                               .Include(d => d.tbYearParent.tbYearParent)
                               where p.IsDeleted == false
                               && p.YearType == Code.EnumHelper.YearType.Section
                               && p.IsDefault == true
                               select p).FirstOrDefault();
                if (vm.YearId == 0 && vm.YearList.Count() > 0 && section != null)
                {
                    vm.YearId = vm.YearList.Where(d => d.Value == section.tbYearParent.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }

                //获取行政班
                vm.ClassList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                where p.IsDeleted == false
                                && p.tbClass.IsDeleted == false
                                && p.tbTeacher.IsDeleted == false
                                && p.tbClass.tbYear.Id == vm.YearId
                                && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.tbClass.ClassName,
                                    Value = p.tbClass.Id.ToString(),
                                }).ToList();
                if (vm.ClassId == 0 && vm.ClassList.Count() > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.InputDate == null || vm.InputDate == "")
                {
                    vm.InputDate = DateTime.Now.ToShortDateString();
                }
                var inputDate = vm.InputDate.ConvertToDateTime();

                //获取学生所在行政班信息
                var studentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  .Include(d => d.tbStudent)
                                  .Include(d => d.tbClass)
                                   where p.IsDeleted == false
                                  && p.tbClass.IsDeleted == false
                                  && p.tbStudent.IsDeleted == false
                                  && p.tbStudent.tbSysUser.IsDeleted == false
                                  && p.tbClass.Id == vm.ClassId
                                   select p).ToList();

                var studentIdList = studentList.Select(d => d.tbStudent.Id).ToList();

                //家长意见与建议
                var commentList = (from p in db.Table<Moral.Entity.tbMoralSuggest>()
                                .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && studentIdList.Contains(p.tbStudent.Id)
                               && p.InputDate == inputDate
                                   select p).ToList();

                vm.MSuggestList = (from p in studentList
                                   select new Dto.MoralComment.MoralSuggestList
                                   {
                                       StudentId = p.tbStudent.Id,
                                       StudentCode = p.tbStudent.StudentCode,
                                       StudentName = p.tbStudent.StudentName,
                                       CommentType = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? 1 : 0),
                                       Comment = (commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault() != null ? commentList.Where(d => d.tbStudent.Id == p.tbStudent.Id).FirstOrDefault().Content : ""),
                                   }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuggestTeacherList(Models.MoralComment.MoralSuggestList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SuggestTeacherList", new { yearId = vm.YearId, classId = vm.ClassId, inputDate = vm.InputDate }));
        }

        public ActionResult SuggestStudentView()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralComment.SuggestStudentView();

                var inputDate = vm.InputDate.ConvertToDateTime();
                var commentList = (from p in db.Table<Moral.Entity.tbMoralSuggest>()
                               .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                               && p.tbYear.IsDeleted == false
                               && p.tbStudent.Id == vm.StudentId
                              && p.InputDate == inputDate
                                   select p).ToList();

                vm.SStudentView = (from p in commentList
                                   select new Dto.MoralComment.SuggestStudentView
                                   {
                                       SuggestId = p.Id,
                                       Comment = p.Content,
                                       StudentName = p.tbStudent.StudentName,
                                   }).FirstOrDefault();

                if (vm.SStudentView != null)
                {
                    vm.SStudentViewList = (from p in db.Table<Entity.tbMoralSuggestReply>()
                                           where p.IsDeleted == false
                                           && p.tbMoralSuggest.IsDeleted == false
                                           && p.tbMoralSuggest.Id == vm.SStudentView.SuggestId
                                           orderby p.InputDate descending
                                           select new Dto.MoralComment.HappeningStudentView
                                           {
                                               UserName = p.tbSysUser.UserName,
                                               ReplyComment = p.Content,
                                               ReplyDate = p.InputDate,
                                           }).ToList();
                }


                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuggestStudentView(Models.MoralComment.SuggestStudentView vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                var resultArea = new List<int>();
                if (vm.SStudentView.SuggestId > 0)
                {
                    if (Request["textareaText"] == null || Request["textareaText"] == "")
                    {
                        error.AddError("请填写回复内容!");
                    }
                }
                else
                {
                    error.AddError("请填写回复内容!");
                }
                var tb = new Entity.tbMoralSuggestReply();
                tb.No = vm.SStudentView.No == null ? db.Table<Entity.tbMoralSuggestReply>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SStudentView.No;
                tb.tbMoralSuggest = db.Set<Entity.tbMoralSuggest>().Find(vm.SStudentView.SuggestId);
                tb.Content = Request["textareaText"];
                tb.InputDate = DateTime.Now;
                tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                db.Set<Entity.tbMoralSuggestReply>().Add(tb);
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加回复");
                }
                return Code.MvcHelper.Post(error, "SuggestStudentView?studentId=" + vm.StudentId + "&yearId=" + vm.YearId + "&inputDate=" + vm.InputDate, "提交成功!");
            }
        }
        #endregion
    }
}