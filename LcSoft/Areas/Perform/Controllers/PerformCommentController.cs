using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Perform.Controllers
{
    public class PerformCommentController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformComment.List();

                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);

                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                var teacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                   where p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                   && p.tbClass.IsDeleted == false
                                   && p.tbTeacher.IsDeleted == false
                                   select p.tbTeacher.Id).ToList();

                var teacherId = 0;

                if (teacherList.Count() > 0)
                {
                    teacherId = teacherList.FirstOrDefault();
                }

                vm.ClassList = Basis.Controllers.ClassController.SelectClassByTeacherList(vm.YearId, teacherId);

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

                vm.OrgSelectInfo = SelectOrgSelectInfo(db, vm.ClassList, vm.YearId);

                vm.ClassStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                       where p.tbStudent.IsDeleted == false
                                       && p.tbClass.Id == vm.ClassId
                                       && p.tbClass.IsDeleted == false
                                       orderby p.No
                                       select new Areas.Basis.Dto.ClassStudent.List
                                       {
                                           Id = p.Id,
                                           ClassId = p.tbClass.Id,
                                           ClassName = p.tbClass.ClassName,
                                           No = p.No,
                                           SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentId = p.tbStudent.Id
                                       }
                                       ).ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    vm.ClassStudentList = (from p in vm.ClassStudentList
                                           where (p.StudentCode.Contains(vm.SearchText) || p.StudentName.Contains(vm.SearchText))
                                           select p).ToList();
                }

                if (vm.StudentId == 0 && vm.ClassStudentList.Count > 0)
                {
                    vm.StudentId = vm.ClassStudentList.FirstOrDefault().StudentId;
                }

                if (vm.ClassStudentList.Count == 0)
                {
                    vm.StudentId = 0;
                }

                var tbFirst = from p in db.Table<Perform.Entity.tbPerformComment>()
                              .Include(d => d.tbStudent)
                              where p.tbYear.Id == vm.YearId
                              && p.tbStudent.IsDeleted == false
                              select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbFirst = tbFirst.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }

                vm.PerformCommentFirstList = (from p in tbFirst
                                              orderby p.tbYear.No
                                              select new Dto.PerformComment.List
                                              {
                                                  Id = p.Id,
                                                  StudentId = p.tbStudent.Id,
                                                  Comment = p.Comment,
                                                  InputDate = p.InputDate,
                                                  StudentName = p.tbStudent.StudentName,
                                                  SysUserName = p.tbSysUser.UserName,
                                                  YearName = p.tbYear.YearName,
                                                  YearId = p.tbYear.Id
                                              }).ToList();

                var tb = from p in db.Table<Perform.Entity.tbPerformComment>()
                         where p.tbYear.Id != vm.YearId
                            && p.tbStudent.Id == vm.StudentId
                            && p.tbStudent.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }

                vm.PerformCommentList = (from p in tb
                                         orderby p.tbYear.No
                                         select new Dto.PerformComment.List
                                         {
                                             Id = p.Id,
                                             Comment = p.Comment,
                                             InputDate = p.InputDate,
                                             StudentName = p.tbStudent.StudentName,
                                             SysUserName = p.tbSysUser.UserName,
                                             YearName = p.tbYear.YearName,
                                             YearId = p.tbYear.Id
                                         }).ToList();
                return View(vm);
            }
        }

        public static List<Dto.PerformData.OrgSelectInfo> SelectOrgSelectInfo(XkSystem.Models.DbContext db, List<System.Web.Mvc.SelectListItem> orgSelectList, int YearId)
        {
            var orgSelectInfoList = new List<Dto.PerformData.OrgSelectInfo>();
            if (YearId == 0)
            {
                return orgSelectInfoList;
            }

            var tbPerformComment = (from p in db.Table<Perform.Entity.tbPerformComment>()
                                    where p.tbYear.Id == YearId && p.tbYear.IsDeleted == false
                                    && p.tbStudent.IsDeleted == false
                                    select new
                                    {
                                        YearId = p.tbYear.Id,
                                        StudentId = p.tbStudent.Id
                                    }).Distinct().ToList();

            foreach (var org in orgSelectList)
            {
                var orgId = org.Value.ConvertToInt();
                var orgSelectInfo = new Dto.PerformData.OrgSelectInfo();
                var orgStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                      .Include(d => d.tbClass)
                                      where p.tbClass.Id == orgId && p.tbClass.IsDeleted == false
                                      select new
                                      {
                                          ClassId = p.tbClass.Id,
                                          StudentId = p.tbStudent.Id,
                                          YearId = p.tbClass.tbYear.Id
                                      }).ToList();
                if (orgStudentList != null)
                {
                    orgSelectInfo.OrgId = orgId;
                    orgSelectInfo.SumCount = orgStudentList.Distinct().Count();//赋值总人数
                    foreach (var student in orgStudentList)
                    {
                        if (tbPerformComment.Where(d => d.YearId == student.YearId && d.StudentId == student.StudentId).Count() > 0)
                        {
                            orgSelectInfo.Count += 1;
                        }
                    }
                    orgSelectInfoList.Add(orgSelectInfo);
                }
            }
            return orgSelectInfoList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.PerformComment.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, YearId = vm.YearId, ClassId = vm.ClassId, StudentId = vm.StudentId }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Perform.Entity.tbPerformComment>()
                          where ids.Contains(p.Id) && p.tbStudent.IsDeleted == false
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除学生评语");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformComment.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Perform.Entity.tbPerformComment>()
                              where p.Id == id && p.tbStudent.IsDeleted == false
                              select new Dto.PerformComment.Edit
                              {
                                  Id = p.Id,
                                  Comment = p.Comment,
                                  StudentId = p.tbStudent.Id,
                                  YearId = p.tbYear.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.PerformCommentEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.PerformComment.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.PerformCommentEdit.Id == 0)
                    {
                        var tb = new Perform.Entity.tbPerformComment();
                        tb.Comment = vm.PerformCommentEdit.Comment;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Perform.Entity.tbPerformComment>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生评语");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Perform.Entity.tbPerformComment>()
                                  where p.Id == vm.PerformCommentEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.Comment = vm.PerformCommentEdit.Comment;
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改学生评语");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Models.PerformComment.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var txtId = Request["txtId"].Split(',');
                var txtYearId = Request["txtYearId"].Split(',');
                var txtComment = Request["txtComment"].Split(',');

                var list = (from p in db.Table<Perform.Entity.tbPerformComment>()
                            where p.tbStudent.IsDeleted == false
                            select p).ToList();

                for (var i = 0; i < txtYearId.Count(); i++)
                {
                    if (string.IsNullOrEmpty(txtYearId[i]))
                    {
                        if (string.IsNullOrEmpty(txtId[i]) == false)
                        {
                            var tb = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                            tb.IsDeleted = true;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(txtId[i]) == false && txtId[i].ConvertToInt() != 0)
                        {
                            var tb = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                            tb.Comment = txtComment[i].ToString();
                            if (string.IsNullOrEmpty(txtComment[i].ToString()))
                            {
                                tb.IsDeleted = true;
                            }
                            else
                            {
                                tb.IsDeleted = false;
                            }
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        }
                        else
                        {
                            var tb = new Perform.Entity.tbPerformComment();
                            tb.InputDate = DateTime.Now;
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            tb.Comment = txtComment[i].ToString();
                            tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.YearId);
                            db.Set<Perform.Entity.tbPerformComment>().Add(tb);
                        }
                    }
                }

                db.SaveChanges();
                return Code.MvcHelper.Post(null, Url.Action("List", new { YearId = vm.YearId, ClassId = vm.ClassId, StudentId = vm.StudentId }), "提交成功!");
            }
        }

        public ActionResult Info(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformComment.Info();
                vm.PerformCommentInfo = (from p in db.Table<Perform.Entity.tbPerformComment>()
                                         where p.Id == id
                                         select new Dto.PerformComment.Info
                                         {
                                             Id = p.Id,
                                             StudentCode = p.tbStudent.StudentCode,
                                             StudentName = p.tbStudent.StudentName,
                                             Comment = p.Comment,
                                             TeacherName = p.tbSysUser.UserName,
                                             InputDate = p.InputDate
                                         }).FirstOrDefault();
                return View(vm);
            }
        }

        public ActionResult My()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformComment.My();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                vm.PerformCommentInfo = (from p in db.Table<Perform.Entity.tbPerformComment>()
                                         where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                           && p.tbYear.Id == vm.YearId
                                         select new Dto.PerformComment.Info
                                         {
                                             Id = p.Id,
                                             StudentCode = p.tbStudent.StudentCode,
                                             StudentName = p.tbStudent.StudentName,
                                             Comment = p.Comment,
                                             TeacherName = p.tbSysUser.UserName,
                                             InputDate = p.InputDate
                                         }).FirstOrDefault();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult My(Models.PerformComment.My vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("My", new { yearId = vm.YearId }));
        }

        [NonAction]
        public static List<Dto.PerformComment.Info> GetAllComment(int studentId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Perform.Entity.tbPerformComment>()
                          where p.tbStudent.Id == studentId
                            && p.tbYear.IsDeleted == false
                          orderby p.tbYear.No
                          select new Dto.PerformComment.Info
                          {
                              Id = p.Id,
                              StudentCode = p.tbStudent.StudentCode,
                              StudentName = p.tbStudent.StudentName,
                              Comment = p.Comment,
                              YearName = p.tbYear.YearName,
                              TeacherName = p.tbSysUser.UserName,
                              InputDate = p.InputDate
                          }).ToList();
                return tb;
            }
        }

        public ActionResult ReportAll()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformComment.ReportAll();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                vm.ClassList = Basis.Controllers.ClassController.SelectList(vm.YearId);
                vm.ReportAllList = (from p in db.Table<Basis.Entity.tbClass>()
                                    where p.tbYear.Id == vm.YearId
                                    select new Dto.PerformComment.ReportAll
                                    {
                                        Id = p.Id,
                                        ClassName = p.ClassName
                                    }).ToList();
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbClass.tbYear.Id == vm.YearId
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                    group p by p.tbClass.Id into g
                                    select new
                                    {
                                        ClassId = g.Key,
                                        Count = g.Count()
                                    }).ToList();
                var dataList = (from p in db.Table<Perform.Entity.tbPerformComment>()
                                join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals q.tbStudent.Id
                                where p.tbYear.Id == vm.YearId
                                    && q.tbClass.tbYear.Id == vm.YearId
                                group p by q.tbClass.Id into g
                                select new
                                {
                                    ClassId = g.Key,
                                    Count = g.Count()
                                }).ToList();
                foreach (var a in vm.ReportAllList)
                {
                    a.TotalCount = classStudent.Where(d => d.ClassId == a.Id).Select(d=>d.Count).FirstOrDefault();
                    a.UnCount = a.TotalCount - dataList.Where(d => d.ClassId == a.Id).Select(d=>d.Count).FirstOrDefault();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportAll(Models.PerformComment.ReportAll vm)
        {
            if (vm.ClassId == 0 || vm.ClassId == null)
            {
                return Code.MvcHelper.Post(null, Url.Action("ReportAll", new { yearId = vm.YearId }));
            }
            else
            {
                return Code.MvcHelper.Post(null, Url.Action("ReportClass", new { yearId = vm.YearId, classId = vm.ClassId }));
            }
        }

        public ActionResult ReportClass()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformComment.ReportClass();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                vm.ClassList = Basis.Controllers.ClassController.SelectList(vm.YearId);
                vm.ReportClassList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                      where p.tbClass.Id == vm.ClassId
                                          && p.tbStudent.IsDeleted == false
                                          && (p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText))
                                      orderby p.tbStudent.StudentCode
                                      select new Dto.PerformComment.ReportClass
                                      {
                                          Id = p.Id,
                                          StudentId = p.tbStudent.Id,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName
                                      }).ToList();
                var data = (from p in db.Table<Perform.Entity.tbPerformComment>()
                            join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals q.tbStudent.Id
                            where p.tbYear.Id == vm.YearId
                            select new
                            {
                                StudentId = p.tbStudent.Id,
                                CommentId = p.Id
                            }).ToList();
                foreach (var a in vm.ReportClassList)
                {
                    var exist = data.Where(d => d.StudentId == a.StudentId).FirstOrDefault();
                    if (exist != null)
                    {
                        a.Status = true;
                        a.CommentId = exist.CommentId;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportClass(Models.PerformComment.ReportClass vm)
        {
            if (vm.ClassId == 0 || vm.ClassId == null)
            {
                return Code.MvcHelper.Post(null, Url.Action("ReportAll", new { yearId = vm.YearId }));
            }
            else
            {
                return Code.MvcHelper.Post(null, Url.Action("ReportClass", new { yearId = vm.YearId, classId = vm.ClassId }));
            }
        }
    }
}