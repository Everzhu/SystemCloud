using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace XkSystem.Areas.Quality.Controllers
{
    public class QualitySummaryController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Quality.Models.QualitySummary.List();
                var yearList = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDeleted == false
                                && p.IsDisable == false
                                select p).ToList();
                vm.YearList = (from p in yearList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.YearName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                //绑定开启和激活学年
                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = yearList.Where(d => d.IsDefault == true).FirstOrDefault() != null ? yearList.Where(d => d.IsDefault == true).FirstOrDefault().Id : 0;
                }

                //获取教师所在班级
                var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                        .Include(d => d.tbClass)
                                        .Include(d => d.tbTeacher)
                                        .Include(d=>d.tbTeacher.tbSysUser)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbTeacher.IsDeleted == false
                                        && p.tbClass.tbYear.Id == vm.YearId
                                        select p).ToList();

                //获取学生所在班级
                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                        .Include(d => d.tbClass)
                                        .Include(d => d.tbStudent)
                                        .Include(d => d.tbStudent.tbSysUser)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbClass.tbYear.Id == vm.YearId
                                        select p).ToList();

                var qualitySummaryList = (from p in db.Table<Basis.Entity.tbClass>()
                                 .Include(d => d.tbGrade)
                                          where p.IsDeleted == false
                                          && p.tbGrade.IsDeleted == false
                                          && p.tbYear.IsDeleted == false
                                          && p.tbYear.Id == vm.YearId
                                          select new Dto.QualitySummary.List
                                          {
                                              ClassId = p.Id,
                                              GradeName = p.tbGrade.GradeName,
                                              ClassName = p.ClassName,
                                          }).ToList();
                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    qualitySummaryList = qualitySummaryList.Where(d => d.ClassName.Contains(vm.SearchText)).ToList();
                }

                //获取自评信息
                var selfList = (from p in db.Table<Quality.Entity.tbQualitySelf>()
                                where p.IsDeleted == false
                                && p.tbStudent.IsDeleted == false
                                && p.tbYear.IsDeleted == false
                                && p.tbYear.Id == vm.YearId
                                select p).ToList();

                //获取学期期待信息
                var planList = (from p in db.Table<Quality.Entity.tbQualityPlan>()
                                where p.IsDeleted == false
                                && p.tbStudent.IsDeleted == false
                                && p.tbYear.IsDeleted == false
                                && p.tbYear.Id == vm.YearId
                                select p).ToList();

                //获取学期总结信息
                var summaryList = (from p in db.Table<Quality.Entity.tbQualitySummary>()
                                   where p.IsDeleted == false
                                   && p.tbStudent.IsDeleted == false
                                   && p.tbYear.IsDeleted == false
                                   && p.tbYear.Id == vm.YearId
                                   select p).ToList();

                //获取当前激活评价结果信息信息
                var qualityList = (from p in db.Table<Quality.Entity.tbQualityData>()
                                   .Include(d=>d.tbSysUser)
                                   .Include(d=>d.tbStudent)
                                   where p.IsDeleted == false
                                   && p.tbSysUser.IsDeleted==false
                                   && p.tbStudent.IsDeleted==false
                                   && p.tbQualityItem.IsDeleted==false
                                   && p.tbQualityItem.tbQualityItemGroup.IsDeleted==false
                                   && p.tbQualityItem.tbQualityItemGroup.tbQuality.IsDeleted==false
                                   && p.tbQualityItem.tbQualityItemGroup.tbQuality.IsOpen==true
                                   && p.tbQualityItem.tbQualityItemGroup.tbQuality.tbYear.Id == vm.YearId
                                   select p).ToList();

                //班主任评语
                var commentList = (from p in db.Table<Quality.Entity.tbQualityComment>()
                            .Include(d => d.tbStudent)
                            .Include(d=>d.tbSysUser)
                          where p.IsDeleted == false
                          && p.tbYear.IsDeleted == false
                          && p.tbSysUser.IsDeleted==false
                          && p.tbStudent.IsDeleted==false
                          && p.tbYear.Id == vm.YearId
                          select p).ToList();

                foreach (var qualitySummary in qualitySummaryList)
                {
                    var model = new Dto.QualitySummary.List();
                    model.ClassId = qualitySummary.ClassId;
                    model.GradeName = qualitySummary.GradeName;
                    model.ClassName = qualitySummary.ClassName;
                    var studentList = classStudentList.Where(d => d.tbClass.Id == qualitySummary.ClassId).ToList();
                    //获取被评价学生ID
                    var studentIdList = studentList.Select(d => d.tbStudent.Id).ToList();
                    //获取评价学生ID
                    var studentUserIdList = studentList.Select(d => d.tbStudent.tbSysUser.Id).ToList();
                    var classTeacher = classTeacherList.Where(d => d.tbClass.Id == qualitySummary.ClassId).FirstOrDefault();
                    if (classTeacher != null)
                    {
                        model.ClassTeacherName = classTeacher.tbTeacher.TeacherName;
                        model.TeacherId = classTeacher.tbTeacher.Id;

                        //班主任评价
                        var qualityClassList = qualityList.Where(d => d.tbSysUser.Id == classTeacher.tbTeacher.tbSysUser.Id && d.IsUserType == 2 && studentIdList.Contains(d.tbStudent.Id)).Select(d => d.tbStudent.Id).Distinct().ToList();
                        if ((studentList.Count - qualityClassList.Count) == 0)
                        {
                            model.QualityComment = "<span class='label label-primary'>已完成</span>|";
                        }
                        else
                        {
                            model.QualityComment = "<span class='label label-info'>未完成</span>|";
                        }
                        //班主任评语
                        var commentStudentList = commentList.Where(d => d.tbSysUser.Id == classTeacher.tbTeacher.tbSysUser.Id && studentIdList.Contains(d.tbStudent.Id)).Select(d => d.tbStudent.Id).Distinct().ToList();
                        if ((studentList.Count - commentStudentList.Count) == 0)
                        {
                            model.QualityComment += "<span class='label label-primary'>已完成</span>";
                        }
                        else
                        {
                            model.QualityComment += "<span class='label label-info'>未完成</span>";
                        }
                    }
                    else
                    {
                        model.ClassTeacherName = string.Empty;
                    }
                   
                    if (studentList.Count > 0)
                    {
                        model.ClassStudentCount = classStudentList.Where(d => d.tbClass.Id == qualitySummary.ClassId).ToList().Count;
                    }
                    else
                    {
                        model.ClassStudentCount = 0;
                    }
                    //自评
                    var selfStudentList = selfList.Where(d => studentIdList.Contains(d.tbStudent.Id)).ToList();
                    model.QualitySelf = selfStudentList.Count.ToString() + "|" + (studentList.Count - selfStudentList.Count).ToString();

                    //学期期待
                    var planStudentList = planList.Where(d => studentIdList.Contains(d.tbStudent.Id)).ToList();
                    model.QualityPlan = planStudentList.Count.ToString() + "|" + (studentList.Count - planStudentList.Count).ToString();

                    //学期总结
                    var summaryStudentList = summaryList.Where(d => studentIdList.Contains(d.tbStudent.Id)).ToList();
                    model.QualitySummary = summaryStudentList.Count.ToString() + "|" + (studentList.Count - summaryStudentList.Count).ToString();

                    //同学评价
                    var qualityStudentList= qualityList.Where(d => studentUserIdList.Contains(d.tbSysUser.Id) && d.IsUserType==1).Select(d => d.tbSysUser.Id).Distinct().ToList();
                    if (qualityStudentList.Count>0)
                    {
                        model.QualityStudent = qualityStudentList.Count + "|"+ (studentList.Count - qualityStudentList.Count);
                    }
                    else
                    {
                        model.QualityStudent = "0|"+ studentList.Count;
                    }

                    vm.QualitySummaryList.Add(model);
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.QualitySummary.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { yearId = vm.YearId, searchText = vm.SearchText }));
        }

        public ActionResult ClassList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Quality.Models.QualitySummary.ClassList();
                var yearList = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDeleted == false
                                && p.IsDisable == false
                                select p).ToList();
                vm.YearList = (from p in yearList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.YearName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                //绑定开启和激活学年
                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = yearList.Where(d => d.IsDefault == true).FirstOrDefault() != null ? yearList.Where(d => d.IsDefault == true).FirstOrDefault().Id : 0;
                }

                //获取行政班
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted==false
                                 && p.tbYear.IsDeleted==false
                                 && p.tbYear.Id==vm.YearId
                                 select p).ToList();
                vm.ClaList = (from p in classList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ClassName,
                                    Value = p.Id.ToString(),
                                }).ToList();
                if (vm.ClassId == 0 && vm.ClaList.Count > 0)
                {
                    vm.ClassId = vm.ClaList.FirstOrDefault().Value.ConvertToInt();
                }

                //获取学生所在班级
                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                        .Include(d => d.tbClass)
                                        .Include(d => d.tbStudent)
                                        .Include(d=>d.tbStudent.tbSysUser)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbClass.Id==vm.ClassId
                                        select p).ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    classStudentList = classStudentList.Where(d => d.tbStudent.StudentName.Contains(vm.SearchText)).ToList();
                }

                var studentIdList = classStudentList.Select(d => d.tbStudent.Id).ToList();
                //获取教师所在班级
                var classTeacher= (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                        .Include(d => d.tbClass)
                                        .Include(d => d.tbTeacher)
                                        .Include(d => d.tbTeacher.tbSysUser)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbTeacher.IsDeleted == false
                                        && p.tbClass.tbYear.Id == vm.YearId
                                        && p.tbClass.Id==vm.ClassId
                                        select p).FirstOrDefault();

                //获取自评信息
                var selfList = (from p in db.Table<Quality.Entity.tbQualitySelf>()
                                   .Include(d => d.tbStudent)
                                where p.IsDeleted == false
                                && p.tbStudent.IsDeleted == false
                                && p.tbYear.IsDeleted == false
                                && p.tbYear.Id == vm.YearId
                                && studentIdList.Contains(p.tbStudent.Id)
                                select p).ToList();

                //获取学期期待信息
                var planList = (from p in db.Table<Quality.Entity.tbQualityPlan>()
                                   .Include(d => d.tbStudent)
                                where p.IsDeleted == false
                                && p.tbStudent.IsDeleted == false
                                && p.tbYear.IsDeleted == false
                                && p.tbYear.Id == vm.YearId
                                && studentIdList.Contains(p.tbStudent.Id)
                                select p).ToList();

                //获取学期总结信息
                var summaryList = (from p in db.Table<Quality.Entity.tbQualitySummary>()
                                   .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                                   && p.tbStudent.IsDeleted == false
                                   && p.tbYear.IsDeleted == false
                                   && p.tbYear.Id == vm.YearId
                                   && studentIdList.Contains(p.tbStudent.Id)
                                   select p).ToList();

                //获取当前激活评价结果信息信息
                var qualityList = (from p in db.Table<Quality.Entity.tbQualityData>()
                                   .Include(d => d.tbSysUser)
                                   .Include(d => d.tbStudent)
                                   where p.IsDeleted == false
                                   && p.tbSysUser.IsDeleted == false
                                   && p.tbStudent.IsDeleted == false
                                   && p.tbQualityItem.IsDeleted == false
                                   && p.tbQualityItem.tbQualityItemGroup.IsDeleted == false
                                   && p.tbQualityItem.tbQualityItemGroup.tbQuality.IsDeleted == false
                                   && p.tbQualityItem.tbQualityItemGroup.tbQuality.IsOpen == true
                                   && p.tbQualityItem.tbQualityItemGroup.tbQuality.tbYear.Id == vm.YearId
                                   && studentIdList.Contains(p.tbStudent.Id)
                                   select p).ToList();

                //班主任评语
                var commentList = (from p in db.Table<Quality.Entity.tbQualityComment>()
                            .Include(d => d.tbStudent)
                            .Include(d => d.tbSysUser)
                                   where p.IsDeleted == false
                                   && p.tbYear.IsDeleted == false
                                   && p.tbSysUser.IsDeleted == false
                                   && p.tbStudent.IsDeleted == false
                                   && p.tbYear.Id == vm.YearId
                                   && studentIdList.Contains(p.tbStudent.Id)
                                   select p).ToList();

                foreach (var classStudent in classStudentList)
                {
                    var model = new Dto.QualitySummary.ClassList();
                    model.StudentCode = classStudent.tbStudent.StudentCode;
                    model.StudentName = classStudent.tbStudent.StudentName;

                    #region 自我
                    //自评
                    var self = selfList.Where(d => d.tbStudent.Id == classStudent.tbStudent.Id).FirstOrDefault();
                    if (self != null)
                    {
                        model.StudentSelf = "<span class='label label-primary'>已完成</span>|";
                    }
                    else
                    {
                        model.StudentSelf = "<span class='label label-info'>未完成</span>|";
                    }
                    //学期期待
                    var plan= planList.Where(d => d.tbStudent.Id == classStudent.tbStudent.Id).FirstOrDefault();
                    if (plan != null)
                    {
                        model.StudentSelf += "<span class='label label-primary'>已完成</span>|";
                    }
                    else
                    {
                        model.StudentSelf += "<span class='label label-info'>未完成</span>|";
                    }
                    //学期总结
                    var summary = summaryList.Where(d => d.tbStudent.Id == classStudent.tbStudent.Id).FirstOrDefault();
                    if (summary != null)
                    {
                        model.StudentSelf += "<span class='label label-primary'>已完成</span>|";
                    }
                    else
                    {
                        model.StudentSelf += "<span class='label label-info'>未完成</span>|";
                    }
                    //评价
                    var quality = qualityList.Where(d => d.tbStudent.Id == classStudent.tbStudent.Id && d.tbSysUser.Id==classStudent.tbStudent.tbSysUser.Id && d.IsUserType==1).FirstOrDefault();
                    if (quality != null)
                    {
                        model.StudentSelf += "<span class='label label-primary'>已完成</span>";
                    }
                    else
                    {
                        model.StudentSelf += "<span class='label label-info'>未完成</span>";
                    }
                    #endregion

                    //班主任
                    if (classTeacher != null)
                    {
                        //评语
                        var teacherComment = commentList.Where(d => d.tbStudent.Id == classStudent.tbStudent.Id && d.tbSysUser.Id==classTeacher.tbTeacher.tbSysUser.Id).FirstOrDefault();
                        if (teacherComment != null)
                        {
                            model.StudentTeacher += "<span class='label label-primary'>已完成</span>|";
                        }
                        else
                        {
                            model.StudentTeacher += "<span class='label label-info'>未完成</span>|";
                        }
                        //评价
                        var teacherQuality = qualityList.Where(d => d.tbStudent.Id==classStudent.tbStudent.Id && d.tbSysUser.Id == classTeacher.tbTeacher.tbSysUser.Id && d.IsUserType == 1).FirstOrDefault();
                        if (teacherQuality != null)
                        {
                            model.StudentTeacher += "<span class='label label-primary'>已完成</span>";
                        }
                        else
                        {
                            model.StudentTeacher += "<span class='label label-info'>未完成</span>";
                        }
                    }

                    //同学
                    //评价
                    var studentQualityList = qualityList.Where(d => studentIdList.Contains(d.tbStudent.Id) && d.tbSysUser.Id == classStudent.tbStudent.tbSysUser.Id && d.IsUserType == 1).Select(d => d.tbSysUser.Id).Distinct().ToList();
                    if (studentQualityList.Count > 0)
                    {
                        if ((classStudentList.Count - studentQualityList.Count) <= 0)
                        {
                            model.StudentQuality += "<span class='label label-primary'>已完成</span>";
                        }
                        else if ((classStudentList.Count - studentQualityList.Count) > 0)
                        {
                            model.StudentQuality += "<span class='label label-info'>未完成</span>";
                        }
                    }
                    else
                    {
                        model.StudentQuality += "<span class='label label-info'>未完成</span>";
                    }
                    vm.QualityClassList.Add(model);

                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassList(Models.QualitySummary.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassList", new { yearId = vm.YearId, classId = vm.ClassId, searchText = vm.SearchText }));
        }
    }
}