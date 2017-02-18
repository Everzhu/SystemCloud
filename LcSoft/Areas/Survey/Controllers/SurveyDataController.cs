using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Survey.Controllers
{
    public class SurveyDataController : Controller
    {
        public ActionResult Input()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Student)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyData.Input();

                //评教下拉框数据源逻辑
                vm.SurveyList = (from p in db.Table<Entity.tbSurveyClass>()
                                 join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                 where q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                    && q.tbStudent.IsDeleted == false
                                    && p.tbSurvey.IsOpen == true
                                    && p.tbClass.IsDeleted == false
                                    && p.tbSurvey.IsDeleted == false
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Text = p.tbSurvey.SurveyName,
                                     Value = p.tbSurvey.Id.ToString()
                                 }).Distinct().ToList();
                //默认取第一个评教Id去查询后面数据
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                var survey = db.Set<Entity.tbSurvey>().Find(vm.SurveyId);
                if (survey != null)
                {
                    var surveyGroupList = db.Table<Entity.tbSurveyGroup>().Where(d => d.tbSurvey.Id == vm.SurveyId).ToList();
                    foreach (var group in surveyGroupList)
                    {
                        if (group.IsOrg)
                        {
                            var surveyCourseIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                                   where p.tbCourse.IsDeleted == false
                                                    && p.tbSurveyGroup.IsDeleted == false
                                                    && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                    && p.tbSurveyGroup.Id == @group.Id
                                                    && p.tbSurveyGroup.IsOrg
                                                   select p.tbCourse.Id).ToList();

                            var tbOrgTeacherList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                                    join t in db.Table<Course.Entity.tbOrgTeacher>() on p.tbOrg.Id equals t.tbOrg.Id
                                                    join s in db.Table<Entity.tbSurvey>() on p.tbOrg.tbYear.Id equals s.tbYear.Id
                                                    where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                       && s.Id == vm.SurveyId
                                                       && p.tbOrg.IsClass == false
                                                       && surveyCourseIds.Contains(p.tbOrg.tbCourse.Id)
                                                    select new Dto.SurveyData.Input.OrgTeacher
                                                    {
                                                        OrgId = p.tbOrg.Id,
                                                        SurveyId = s.Id,
                                                        TeacherId = t.tbTeacher.Id,
                                                        TeacherName = t.tbTeacher.TeacherName,
                                                        OrgName = p.tbOrg.OrgName,
                                                        IsChecked = t.tbTeacher.Id == vm.TeacherId && t.tbOrg.Id == vm.OrgId && vm.IsClass == false,
                                                        IsClass = false
                                                    }).ToList();

                            vm.OrgTeacherList = vm.OrgTeacherList.Union(tbOrgTeacherList).ToList();

                            // 学生所属行政教学班
                            int classId = (from c in db.Table<Basis.Entity.tbClassStudent>()
                                           where c.tbStudent.tbSysUser.Id == Code.Common.UserId
                                           select c.tbClass.Id).FirstOrDefault();

                            var classOrgScheduleList = (from t in db.Table<Course.Entity.tbOrgTeacher>()
                                                        join s in db.Table<Entity.tbSurvey>() on t.tbOrg.tbYear.Id equals s.tbYear.Id
                                                        where s.Id == vm.SurveyId
                                                              && t.tbOrg.IsClass == true
                                                              && t.tbOrg.tbClass.Id == classId
                                                              && surveyCourseIds.Contains(t.tbOrg.tbCourse.Id)
                                                        select new Dto.SurveyData.Input.OrgTeacher
                                                        {
                                                            OrgId = t.tbOrg.Id,
                                                            SurveyId = s.Id,
                                                            TeacherId = t.tbTeacher.Id,
                                                            TeacherName = t.tbTeacher.TeacherName,
                                                            OrgName = t.tbOrg.OrgName,
                                                            IsChecked = t.tbTeacher.Id == vm.TeacherId && t.tbOrg.Id == vm.OrgId && vm.IsClass == false,
                                                            IsClass = false
                                                        }).ToList();

                            vm.OrgTeacherList = vm.OrgTeacherList.Union(classOrgScheduleList).ToList();
                        }
                        else
                        {
                            // 学生所属行政班
                            var classTeacherList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                    join q in db.Table<Basis.Entity.tbClassTeacher>() on p.tbClass.Id equals q.tbClass.Id
                                                    join o in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals o.tbClass.Id
                                                    where o.tbSurvey.Id == vm.SurveyId
                                                        && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                        && p.tbStudent.IsDeleted == false
                                                        && q.tbTeacher.IsDeleted == false
                                                        && p.tbClass.IsDeleted == false
                                                    select new Dto.SurveyData.Input.OrgTeacher
                                                    {
                                                        OrgId = p.tbClass.Id,
                                                        OrgName = p.tbClass.ClassName,
                                                        TeacherId = q.tbTeacher.Id,
                                                        TeacherName = q.tbTeacher.TeacherName,
                                                        SurveyId = vm.SurveyId,
                                                        IsChecked = q.tbTeacher.Id == vm.TeacherId && p.tbClass.Id == vm.OrgId && vm.IsClass,
                                                        IsClass = true
                                                    }).ToList();

                            vm.OrgTeacherList = vm.OrgTeacherList.Union(classTeacherList).ToList();
                        }
                    }
                    var ranking = 1;
                    foreach (var a in vm.OrgTeacherList)
                    {
                        a.Ranking = ranking;
                        ranking++;
                    }

                    //表示初次进入页面,左侧列表要选中，右侧需要根据左侧的选中值查询
                    if (vm.OrgId == 0)
                    {
                        var teacher = vm.OrgTeacherList.FirstOrDefault();
                        if (teacher != null)
                        {
                            teacher.IsChecked = true;
                            vm.OrgId = teacher.OrgId;
                            vm.IsClass = teacher.IsClass;
                            vm.TeacherId = teacher.TeacherId;
                            vm.RankIng = teacher.Ranking;
                        }
                    }

                    if (vm.IsClass == false)
                    {
                        vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                             join s in db.Table<Entity.tbSurveyCourse>() on p.tbSurveyGroup.Id equals s.tbSurveyGroup.Id
                                             join t in db.Table<Course.Entity.tbOrgTeacher>() on s.tbCourse.Id equals t.tbOrg.tbCourse.Id
                                             where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                && p.tbSurveyGroup.IsOrg
                                                && p.tbSurveyGroup.tbSurvey.tbYear.Id == t.tbOrg.tbYear.Id
                                                && t.tbTeacher.Id == vm.TeacherId
                                                && t.tbOrg.Id == vm.OrgId
                                             select new Dto.SurveyItem.Info
                                             {
                                                 Id = p.Id,
                                                 No = p.No,
                                                 SurveyItemName = p.SurveyItemName,
                                                 IsVertical = p.IsVertical,
                                                 SurveyItemType = p.SurveyItemType,
                                                 TextMaxLength = p.TextMaxLength
                                             }).ToList();
                    }
                    else
                    {
                        vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                             join c in db.Table<Entity.tbSurveyClass>() on p.tbSurveyGroup.tbSurvey.Id equals c.tbSurvey.Id
                                             where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                 && p.tbSurveyGroup.IsOrg == false
                                                 && c.tbClass.Id == vm.OrgId // vm.OrgId为行政班级ID
                                             select new Dto.SurveyItem.Info
                                             {
                                                 Id = p.Id,
                                                 No = p.No,
                                                 SurveyItemName = p.SurveyItemName,
                                                 IsVertical = p.IsVertical,
                                                 SurveyItemType = p.SurveyItemType,
                                                 TextMaxLength = p.TextMaxLength
                                             }).ToList();
                    }

                    var itemIds = vm.SurveyItemList.Where(d => d.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox).Select(d => d.Id).ToList();
                    var itemIdTexts = vm.SurveyItemList.Where(d => d.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox).Select(d => d.Id).ToList();
                    vm.ItemIds = string.Join("|", itemIds);
                    vm.ItemTextIds = string.Join("|", itemIdTexts);
                    vm.SurveyOptionList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where itemIds.Contains(p.tbSurveyItem.Id)
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).ToList();

                    //需要选中的值
                    vm.SurveyDataList = (from p in db.Table<Entity.tbSurveyData>()
                                         where (p.tbOrg.Id == vm.OrgId || p.tbClass.Id == vm.OrgId)
                                         && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                         && p.tbTeacher.Id == vm.TeacherId
                                         && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                         && p.tbSurveyItem.IsDeleted == false
                                         && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                         && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                         select new Dto.SurveyData.Edit
                                         {
                                             ItemId = p.tbSurveyItem.Id,
                                             OptionId = p.tbSurveyOption.Id,
                                             SurveyText = p.SurveyText
                                         }).ToList();

                    //需要选中的值
                    vm.SurveyDataTextList = (from p in db.Table<Entity.tbSurveyData>()
                                             where (p.tbOrg.Id == vm.OrgId || p.tbClass.Id == vm.OrgId)
                                             && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                             && p.tbTeacher.Id == vm.TeacherId
                                             && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                             && p.tbSurveyItem.IsDeleted == false
                                             && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                             && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                             select new Dto.SurveyData.Edit
                                             {
                                                 ItemId = p.tbSurveyItem.Id,
                                                 SurveyText = p.SurveyText
                                             }).ToList();
                }

                #region 是否已评
                var myInputList = (from p in db.Table<Entity.tbSurveyData>()
                                   where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                   && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                   && p.tbStudent.IsDeleted == false
                                   && p.tbSurveyItem.IsDeleted == false
                                   && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                   && p.tbSurveyItem.tbSurveyGroup.tbSurvey.IsDeleted == false
                                   group p by new { teacherId = p.tbTeacher.Id, classId = p.tbClass == null ? 0 : p.tbClass.Id, orgId = p.tbOrg == null ? 0 : p.tbOrg.Id } into g
                                   select new
                                   {
                                       teacherId = g.Key.teacherId,
                                       classId = g.Key.classId,
                                       orgId = g.Key.orgId
                                   }).Distinct().ToList();

                foreach (var a in vm.OrgTeacherList)
                {
                    if (myInputList.Where(d => d.teacherId == a.TeacherId && d.classId == a.OrgId && a.IsClass).Count() > decimal.Zero)
                    {
                        a.IsHaveInput = true;
                    }
                    else if (myInputList.Where(d => d.teacherId == a.TeacherId && d.orgId == a.OrgId && a.IsClass == false).Count() > decimal.Zero)
                    {
                        a.IsHaveInput = true;
                    }
                    else
                    {
                        a.IsHaveInput = false;
                    }
                }
                #endregion

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Input(Models.SurveyData.Input vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                #region 单选结果
                var radiolcList = new List<string>();
                foreach (var key in Request.Form.AllKeys)
                {
                    if (key.StartsWith("radiolc"))
                    {
                        var radiolc = Request[key];
                        if (radiolc != null)
                        {
                            radiolcList.Add(radiolc);
                        }
                    }
                    else if (key.StartsWith("checkboxlc"))
                    {
                        var checkboxlc = Request[key];
                        if (checkboxlc != null)
                        {
                            radiolcList.Add(checkboxlc);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                #endregion

                #region 基础数据
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
            .Include(p => p.tbClass)
                                    where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                    select p).FirstOrDefault();

                var org = db.Set<Course.Entity.tbOrg>().Find(vm.OrgId);
                var teacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.TeacherId);
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.tbSysUser.Id == Code.Common.UserId
                               select p).FirstOrDefault();

                var survey = db.Set<Entity.tbSurvey>().Find(vm.SurveyId);
                #endregion

                //单选题目
                if (string.IsNullOrEmpty(vm.ItemIds) == false)
                {
                    var itemIds = vm.ItemIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                    var voteItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                        where itemIds.Contains(p.Id)
                                        select p).ToList();

                    foreach (var itemId in itemIds)
                    {
                        var item = voteItemList.Where(d => d.Id == itemId).FirstOrDefault();
                        if (item != null)
                        {
                            if (item.SurveyItemType == Code.EnumHelper.SurveyItemType.Radio)
                            {
                                if (Request["radiolc" + item.Id] == null)
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                            }
                            else if (item.SurveyItemType == Code.EnumHelper.SurveyItemType.CheckBox)
                            {
                                if (Request["checkboxlc" + item.Id] == null)
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            error.AddError("请填写完整!");
                            break;
                        }
                    }

                    if (error.Count > 0)
                    {
                        return Code.MvcHelper.Post(error);
                    }
                    #region 保存结果

                    var del = from p in db.Table<Entity.tbSurveyData>()
                              where itemIds.Contains(p.tbSurveyItem.Id)
                              && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                              && p.tbTeacher.Id == vm.TeacherId
                              select p;

                    if (org == null)
                    {
                        del = del.Where(d => d.tbOrg == null);
                    }
                    else
                    {
                        del = del.Where(d => d.tbOrg.Id == org.Id);
                    }

                    if (vm.IsClass == true && classStudent != null)
                    {
                        del = del.Where(d => d.tbClass.Id == classStudent.tbClass.Id);
                    }
                    else
                    {
                        del = del.Where(d => d.tbClass == null);
                    }

                    var delList = (from p in del
                                   select p).ToList();

                    foreach (var a in delList)
                    {
                        a.IsDeleted = true;
                    }

                    foreach (var item in radiolcList)
                    {
                        var radioResult = item.Split(',');
                        foreach (var option in radioResult)
                        {
                            int surveyOptionId = option.Split('|')[0].ConvertToInt();
                            int surveyItemId = option.Split('|')[1].ConvertToInt();

                            var tb = new Entity.tbSurveyData();
                            tb.tbOrg = org;
                            tb.tbStudent = student;
                            tb.tbSurveyOption = db.Set<Entity.tbSurveyOption>().Find(surveyOptionId);
                            tb.tbSurveyItem = db.Set<Entity.tbSurveyItem>().Find(surveyItemId);
                            tb.tbTeacher = teacher;

                            if (vm.IsClass && classStudent != null)
                            {
                                tb.tbClass = classStudent.tbClass;
                            }
                            db.Set<Entity.tbSurveyData>().Add(tb);
                        }
                    }
                    #endregion
                }
                if (string.IsNullOrEmpty(vm.ItemTextIds) == false)
                {
                    #region 问答题目
                    var ItemTextIds = vm.ItemTextIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();

                    var del = from p in db.Table<Entity.tbSurveyData>()
                              where ItemTextIds.Contains(p.tbSurveyItem.Id)
                              && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                              && p.tbTeacher.Id == vm.TeacherId
                              select p;

                    if (org == null)
                    {
                        del = del.Where(d => d.tbOrg == null);
                    }
                    else
                    {
                        del = del.Where(d => d.tbOrg.Id == org.Id);
                    }

                    if (vm.IsClass == true && classStudent != null)
                    {
                        del = del.Where(d => d.tbClass.Id == classStudent.tbClass.Id);
                    }
                    else
                    {
                        del = del.Where(d => d.tbClass == null);
                    }

                    var delList = (from p in del
                                   select p).ToList();

                    foreach (var a in delList)
                    {
                        a.IsDeleted = true;
                    }
                    var voteItemTextList = (from p in db.Table<Entity.tbSurveyItem>()
                                            where ItemTextIds.Contains(p.Id)
                                            select p).ToList();

                    foreach (var itemId in ItemTextIds)
                    {
                        var item = voteItemTextList.Where(d => d.Id == itemId).FirstOrDefault();
                        if (item != null)
                        {
                            if (item.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox)
                            {
                                if (Request["textareaText" + item.Id] != null)
                                {
                                    var tb = new Entity.tbSurveyData();
                                    tb.tbOrg = org;
                                    tb.tbStudent = student;
                                    tb.tbSurveyItem = db.Set<Entity.tbSurveyItem>().Find(itemId);
                                    tb.tbTeacher = teacher;
                                    tb.SurveyText = Request["textareaText" + item.Id].ToString();

                                    if (vm.IsClass && classStudent != null)
                                    {
                                        tb.tbClass = classStudent.tbClass;
                                    }
                                    db.Set<Entity.tbSurveyData>().Add(tb);
                                }
                                else
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                            }
                        }
                    }
                    #endregion

                    if (error.Count > 0)
                    {
                        return Code.MvcHelper.Post(error);
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("提交了教师评价数据");
                }

                #region 获取参数
                var surveyGroupList = db.Table<Entity.tbSurveyGroup>().Where(d => d.tbSurvey.Id == vm.SurveyId).ToList();
                foreach (var group in surveyGroupList)
                {
                    if (group.IsOrg)
                    {
                        var surveyCourseIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                               where p.tbCourse.IsDeleted == false
                                                && p.tbSurveyGroup.IsDeleted == false
                                                && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                && p.tbSurveyGroup.Id == @group.Id
                                                && p.tbSurveyGroup.IsOrg
                                               select p.tbCourse.Id).ToList();

                        var tbOrgTeacherList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                                join t in db.Table<Course.Entity.tbOrgTeacher>() on p.tbOrg.Id equals t.tbOrg.Id
                                                join s in db.Table<Entity.tbSurvey>() on p.tbOrg.tbYear.Id equals s.tbYear.Id
                                                where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                   && s.Id == vm.SurveyId
                                                   && p.tbOrg.IsClass == false
                                                   && surveyCourseIds.Contains(p.tbOrg.tbCourse.Id)
                                                select new Dto.SurveyData.Input.OrgTeacher
                                                {
                                                    OrgId = p.tbOrg.Id,
                                                    SurveyId = s.Id,
                                                    TeacherId = t.tbTeacher.Id,
                                                    TeacherName = t.tbTeacher.TeacherName,
                                                    OrgName = p.tbOrg.OrgName,
                                                    IsChecked = t.tbTeacher.Id == vm.TeacherId && t.tbOrg.Id == vm.OrgId && vm.IsClass == false,
                                                    IsClass = false
                                                }).ToList();

                        vm.OrgTeacherList = vm.OrgTeacherList.Union(tbOrgTeacherList).ToList();

                        // 学生所属行政教学班
                        int classId = (from c in db.Table<Basis.Entity.tbClassStudent>()
                                       where c.tbStudent.tbSysUser.Id == Code.Common.UserId
                                       select c.tbClass.Id).FirstOrDefault();

                        var classOrgScheduleList = (from t in db.Table<Course.Entity.tbOrgTeacher>()
                                                    join s in db.Table<Entity.tbSurvey>() on t.tbOrg.tbYear.Id equals s.tbYear.Id
                                                    where s.Id == vm.SurveyId
                                                          && t.tbOrg.IsClass == true
                                                          && t.tbOrg.tbClass.Id == classId
                                                          && surveyCourseIds.Contains(t.tbOrg.tbCourse.Id)
                                                    select new Dto.SurveyData.Input.OrgTeacher
                                                    {
                                                        OrgId = t.tbOrg.Id,
                                                        SurveyId = s.Id,
                                                        TeacherId = t.tbTeacher.Id,
                                                        TeacherName = t.tbTeacher.TeacherName,
                                                        OrgName = t.tbOrg.OrgName,
                                                        IsChecked = t.tbTeacher.Id == vm.TeacherId && t.tbOrg.Id == vm.OrgId && vm.IsClass == false,
                                                        IsClass = false
                                                    }).ToList();

                        vm.OrgTeacherList = vm.OrgTeacherList.Union(classOrgScheduleList).ToList();
                    }
                    else
                    {
                        // 学生所属行政班
                        var classTeacherList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                join q in db.Table<Basis.Entity.tbClassTeacher>() on p.tbClass.Id equals q.tbClass.Id
                                                join o in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals o.tbClass.Id
                                                where o.tbSurvey.Id == vm.SurveyId
                                                    && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                    && p.tbStudent.IsDeleted == false
                                                    && q.tbTeacher.IsDeleted == false
                                                    && p.tbClass.IsDeleted == false
                                                select new Dto.SurveyData.Input.OrgTeacher
                                                {
                                                    OrgId = p.tbClass.Id,
                                                    OrgName = p.tbClass.ClassName,
                                                    TeacherId = q.tbTeacher.Id,
                                                    TeacherName = q.tbTeacher.TeacherName,
                                                    SurveyId = vm.SurveyId,
                                                    IsChecked = q.tbTeacher.Id == vm.TeacherId && p.tbClass.Id == vm.OrgId && vm.IsClass,
                                                    IsClass = true
                                                }).ToList();

                        vm.OrgTeacherList = vm.OrgTeacherList.Union(classTeacherList).ToList();
                    }
                }
                var ranking = 1;
                foreach (var a in vm.OrgTeacherList)
                {
                    a.Ranking = ranking;
                    ranking++;
                }
                #endregion

                var orgTeacherLast = vm.OrgTeacherList.Where(d => d.Ranking == (vm.RankIng + 1)).FirstOrDefault();
                if (orgTeacherLast != null)
                {
                    vm.SurveyId = orgTeacherLast.SurveyId;
                    vm.OrgId = orgTeacherLast.OrgId;
                    vm.TeacherId = orgTeacherLast.TeacherId;
                    vm.IsClass = orgTeacherLast.IsClass;
                    vm.RankIng = orgTeacherLast.Ranking;
                }
                return Code.MvcHelper.Post(error, Url.Action("Input", "SurveyData", new { SurveyId = vm.SurveyId, OrgId = vm.OrgId, TeacherId = vm.TeacherId, IsClass = vm.IsClass, RankIng = vm.RankIng }), "评价提交成功!");
            }
        }

        [NonAction]
        public static List<Areas.Teacher.Dto.Teacher.Info> SelectTeacherInfoListBySurveyGroup(int surveyGroupId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSurveyData>()
                          where p.tbSurveyItem.IsDeleted == false
                            && p.tbSurveyItem.tbSurveyGroup.Id == surveyGroupId
                            && p.tbTeacher != null
                          orderby p.tbTeacher.TeacherName
                          select new Areas.Teacher.Dto.Teacher.Info
                          {
                              Id = p.tbTeacher.Id,
                              TeacherName = p.tbTeacher.TeacherName
                          }).Distinct().ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<Dto.SurveyData.Info> SelectInfoListBySurveyCourse(int surveyCourseId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var surveyTotalList = new List<Dto.SurveyData.Info>();

                if (surveyCourseId != 0)
                {
                    surveyTotalList = (from p in db.Table<Entity.tbSurveyData>()
                                       where p.tbSurveyItem.IsDeleted == false
                                        && p.tbSurveyOption.IsDeleted == false
                                        && p.tbOrg.tbCourse.Id == surveyCourseId
                                        && p.tbOrg.IsDeleted == false
                                       orderby p.tbOrg.No, p.tbTeacher.TeacherName
                                       group p by new { classId = p.tbClass == null ? 0 : p.tbClass.Id, orgId = p.tbOrg == null ? 0 : p.tbOrg.Id, surveyId = p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id, surveryItemId = p.tbSurveyItem.Id, surveryOptionId = p.tbSurveyOption == null ? 0 : p.tbSurveyOption.Id, teacherId = p.tbTeacher.Id } into g
                                       select new Areas.Survey.Dto.SurveyData.Info
                                       {
                                           TotalCount = g.Count(),
                                           SurveyId = g.Key.surveyId,
                                           TeacherId = g.Key.teacherId,
                                           OrgId = g.Key.orgId,
                                           ClassId = g.Key.classId,
                                           SurveyOptionId = g.Key.surveryOptionId,
                                           SurveyItemId = g.Key.surveryItemId
                                       }).ToList();
                }
                else
                {
                    surveyTotalList = (from p in db.Table<Entity.tbSurveyData>()
                                       where p.tbSurveyItem.IsDeleted == false
                                       orderby p.tbOrg.No, p.tbTeacher.TeacherName
                                       group p by new { classId = p.tbClass == null ? 0 : p.tbClass.Id, orgId = p.tbOrg == null ? 0 : p.tbOrg.Id, surveyId = p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id, surveryItemId = p.tbSurveyItem.Id, surveryOptionId = p.tbSurveyOption == null ? 0 : p.tbSurveyOption.Id, teacherId = p.tbTeacher.Id } into g
                                       select new Areas.Survey.Dto.SurveyData.Info
                                       {
                                           TotalCount = g.Count(),
                                           SurveyId = g.Key.surveyId,
                                           TeacherId = g.Key.teacherId,
                                           OrgId = g.Key.orgId,
                                           ClassId = g.Key.classId,
                                           SurveyOptionId = g.Key.surveryOptionId,
                                           SurveyItemId = g.Key.surveryItemId
                                       }).ToList();
                }

                return surveyTotalList;
            }
        }

        public ActionResult m_Input()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyData.Input();

                //评教下拉框数据源逻辑
                vm.SurveyList = (from p in db.Table<Entity.tbSurveyClass>()
                                 join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                 where q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                    && q.tbStudent.IsDeleted == false
                                    && p.tbSurvey.IsOpen == true
                                    && p.tbClass.IsDeleted == false
                                    && p.tbSurvey.IsDeleted == false
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Text = p.tbSurvey.SurveyName,
                                     Value = p.tbSurvey.Id.ToString()
                                 }).Distinct().ToList();
                //默认取第一个评教Id去查询后面数据
                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                var survey = db.Set<Entity.tbSurvey>().Find(vm.SurveyId);
                if (survey != null)
                {
                    var surveyGroupList = db.Table<Entity.tbSurveyGroup>().Where(d => d.tbSurvey.Id == vm.SurveyId).ToList();
                    foreach (var group in surveyGroupList)
                    {
                        if (group.IsOrg)
                        {
                            var surveyCourseIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                                   where p.tbCourse.IsDeleted == false
                                                    && p.tbSurveyGroup.IsDeleted == false
                                                    && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                    && p.tbSurveyGroup.Id == @group.Id
                                                    && p.tbSurveyGroup.IsOrg
                                                   select p.tbCourse.Id).ToList();

                            var tbOrgTeacherList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                                    join t in db.Table<Course.Entity.tbOrgTeacher>() on p.tbOrg.Id equals t.tbOrg.Id
                                                    join s in db.Table<Entity.tbSurvey>() on p.tbOrg.tbYear.Id equals s.tbYear.Id
                                                    where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                       && s.Id == vm.SurveyId
                                                       && p.tbOrg.IsClass == false
                                                       && surveyCourseIds.Contains(p.tbOrg.tbCourse.Id)
                                                    select new Dto.SurveyData.Input.OrgTeacher
                                                    {
                                                        OrgId = p.tbOrg.Id,
                                                        SurveyId = s.Id,
                                                        TeacherId = t.tbTeacher.Id,
                                                        TeacherName = t.tbTeacher.TeacherName,
                                                        OrgName = p.tbOrg.OrgName,
                                                        IsChecked = t.tbTeacher.Id == vm.TeacherId && t.tbOrg.Id == vm.OrgId && vm.IsClass == false,
                                                        IsClass = false
                                                    }).ToList();

                            vm.OrgTeacherList = vm.OrgTeacherList.Union(tbOrgTeacherList).ToList();

                            // 学生所属行政教学班
                            int classId = (from c in db.Table<Basis.Entity.tbClassStudent>()
                                           where c.tbStudent.tbSysUser.Id == Code.Common.UserId
                                           select c.tbClass.Id).FirstOrDefault();

                            var classOrgScheduleList = (from t in db.Table<Course.Entity.tbOrgTeacher>()
                                                        join s in db.Table<Entity.tbSurvey>() on t.tbOrg.tbYear.Id equals s.tbYear.Id
                                                        where s.Id == vm.SurveyId
                                                              && t.tbOrg.IsClass == true
                                                              && t.tbOrg.tbClass.Id == classId
                                                              && surveyCourseIds.Contains(t.tbOrg.tbCourse.Id)
                                                        select new Dto.SurveyData.Input.OrgTeacher
                                                        {
                                                            OrgId = t.tbOrg.Id,
                                                            SurveyId = s.Id,
                                                            TeacherId = t.tbTeacher.Id,
                                                            TeacherName = t.tbTeacher.TeacherName,
                                                            OrgName = t.tbOrg.OrgName,
                                                            IsChecked = t.tbTeacher.Id == vm.TeacherId && t.tbOrg.Id == vm.OrgId && vm.IsClass == false,
                                                            IsClass = false
                                                        }).ToList();

                            vm.OrgTeacherList = vm.OrgTeacherList.Union(classOrgScheduleList).ToList();
                        }
                        else
                        {
                            // 学生所属行政班
                            var classTeacherList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                    join q in db.Table<Basis.Entity.tbClassTeacher>() on p.tbClass.Id equals q.tbClass.Id
                                                    join o in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals o.tbClass.Id
                                                    where o.tbSurvey.Id == vm.SurveyId
                                                        && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                        && p.tbStudent.IsDeleted == false
                                                        && q.tbTeacher.IsDeleted == false
                                                        && p.tbClass.IsDeleted == false
                                                    select new Dto.SurveyData.Input.OrgTeacher
                                                    {
                                                        OrgId = p.tbClass.Id,
                                                        OrgName = p.tbClass.ClassName,
                                                        TeacherId = q.tbTeacher.Id,
                                                        TeacherName = q.tbTeacher.TeacherName,
                                                        SurveyId = vm.SurveyId,
                                                        IsChecked = q.tbTeacher.Id == vm.TeacherId && p.tbClass.Id == vm.OrgId && vm.IsClass,
                                                        IsClass = true
                                                    }).ToList();

                            vm.OrgTeacherList = vm.OrgTeacherList.Union(classTeacherList).ToList();
                        }
                    }

                    //表示初次进入页面,左侧列表要选中，右侧需要根据左侧的选中值查询
                    if (vm.OrgId == 0)
                    {
                        var teacher = vm.OrgTeacherList.FirstOrDefault();
                        if (teacher != null)
                        {
                            teacher.IsChecked = true;
                            vm.OrgId = teacher.OrgId;
                            vm.IsClass = teacher.IsClass;
                            vm.TeacherId = teacher.TeacherId;
                        }
                    }

                    if (vm.IsClass == false)
                    {
                        vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                             join s in db.Table<Entity.tbSurveyCourse>() on p.tbSurveyGroup.Id equals s.tbSurveyGroup.Id
                                             join t in db.Table<Course.Entity.tbOrgTeacher>() on s.tbCourse.Id equals t.tbOrg.tbCourse.Id
                                             where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                && p.tbSurveyGroup.IsOrg
                                                && p.tbSurveyGroup.tbSurvey.tbYear.Id == t.tbOrg.tbYear.Id
                                                && t.tbTeacher.Id == vm.TeacherId
                                                && t.tbOrg.Id == vm.OrgId
                                             select new Dto.SurveyItem.Info
                                             {
                                                 Id = p.Id,
                                                 No = p.No,
                                                 SurveyItemName = p.SurveyItemName,
                                                 IsVertical = p.IsVertical,
                                                 SurveyItemType = p.SurveyItemType
                                             }).ToList();
                    }
                    else
                    {
                        vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                             join c in db.Table<Entity.tbSurveyClass>() on p.tbSurveyGroup.tbSurvey.Id equals c.tbSurvey.Id
                                             where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                 && p.tbSurveyGroup.IsOrg == false
                                                 && c.tbClass.Id == vm.OrgId // vm.OrgId为行政班级ID
                                             select new Dto.SurveyItem.Info
                                             {
                                                 Id = p.Id,
                                                 No = p.No,
                                                 SurveyItemName = p.SurveyItemName,
                                                 IsVertical = p.IsVertical,
                                                 SurveyItemType = p.SurveyItemType
                                             }).ToList();
                    }

                    var itemIds = vm.SurveyItemList.Where(d => d.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox).Select(d => d.Id).ToList();
                    var itemIdTexts = vm.SurveyItemList.Where(d => d.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox).Select(d => d.Id).ToList();
                    vm.ItemIds = string.Join("|", itemIds);
                    vm.ItemTextIds = string.Join("|", itemIdTexts);
                    vm.SurveyOptionList = (from p in db.Table<Entity.tbSurveyOption>()
                                           where itemIds.Contains(p.tbSurveyItem.Id)
                                           select new Dto.SurveyOption.Info
                                           {
                                               Id = p.Id,
                                               No = p.No,
                                               OptionName = p.OptionName,
                                               SurveyItemId = p.tbSurveyItem.Id
                                           }).ToList();

                    //需要选中的值
                    vm.SurveyDataList = (from p in db.Table<Entity.tbSurveyData>()
                                         where (p.tbOrg.Id == vm.OrgId || p.tbClass.Id == vm.OrgId)
                                         && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                         && p.tbTeacher.Id == vm.TeacherId
                                         && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                         && p.tbSurveyItem.IsDeleted == false
                                         && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                         && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                         select new Dto.SurveyData.Edit
                                         {
                                             ItemId = p.tbSurveyItem.Id,
                                             OptionId = p.tbSurveyOption.Id,
                                             SurveyText = p.SurveyText
                                         }).ToList();

                    //需要选中的值
                    vm.SurveyDataTextList = (from p in db.Table<Entity.tbSurveyData>()
                                             where (p.tbOrg.Id == vm.OrgId || p.tbClass.Id == vm.OrgId)
                                             && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                             && p.tbTeacher.Id == vm.TeacherId
                                             && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                             && p.tbSurveyItem.IsDeleted == false
                                             && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                             && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                             select new Dto.SurveyData.Edit
                                             {
                                                 ItemId = p.tbSurveyItem.Id,
                                                 SurveyText = p.SurveyText
                                             }).ToList();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult m_Input(Models.SurveyData.Input vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                #region 单选结果
                var radiolcList = new List<string>();
                foreach (var key in Request.Form.AllKeys)
                {
                    if (key.StartsWith("radiolc"))
                    {
                        var radiolc = Request[key];
                        if (radiolc != null)
                        {
                            radiolcList.Add(radiolc);
                        }
                    }
                    else if (key.StartsWith("checkboxlc"))
                    {
                        var checkboxlc = Request[key];
                        if (checkboxlc != null)
                        {
                            radiolcList.Add(checkboxlc);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                #endregion

                #region 基础数据
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
            .Include(p => p.tbClass)
                                    where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                    select p).FirstOrDefault();

                var org = db.Set<Course.Entity.tbOrg>().Find(vm.OrgId);
                var teacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.TeacherId);
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.tbSysUser.Id == Code.Common.UserId
                               select p).FirstOrDefault();

                var survey = db.Set<Entity.tbSurvey>().Find(vm.SurveyId);
                #endregion

                //单选题目
                if (string.IsNullOrEmpty(vm.ItemIds) == false)
                {
                    var itemIds = vm.ItemIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                    #region 保存结果

                    var del = from p in db.Table<Entity.tbSurveyData>()
                              where itemIds.Contains(p.tbSurveyItem.Id)
                              && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                              && p.tbTeacher.Id == vm.TeacherId
                              select p;

                    if (org == null)
                    {
                        del = del.Where(d => d.tbOrg == null);
                    }
                    else
                    {
                        del = del.Where(d => d.tbOrg.Id == org.Id);
                    }

                    if (vm.IsClass == true && classStudent != null)
                    {
                        del = del.Where(d => d.tbClass.Id == classStudent.tbClass.Id);
                    }
                    else
                    {
                        del = del.Where(d => d.tbClass == null);
                    }

                    var delList = (from p in del
                                   select p).ToList();

                    foreach (var a in delList)
                    {
                        a.IsDeleted = true;
                    }

                    foreach (var item in radiolcList)
                    {
                        var radioResult = item.Split(',');
                        foreach (var option in radioResult)
                        {
                            int surveyOptionId = option.Split('|')[0].ConvertToInt();
                            int surveyItemId = option.Split('|')[1].ConvertToInt();

                            var tb = new Entity.tbSurveyData();
                            tb.tbOrg = org;
                            tb.tbStudent = student;
                            tb.tbSurveyOption = db.Set<Entity.tbSurveyOption>().Find(surveyOptionId);
                            tb.tbSurveyItem = db.Set<Entity.tbSurveyItem>().Find(surveyItemId);
                            tb.tbTeacher = teacher;

                            if (vm.IsClass && classStudent != null)
                            {
                                tb.tbClass = classStudent.tbClass;
                            }
                            db.Set<Entity.tbSurveyData>().Add(tb);
                        }
                    }
                    #endregion
                }
                if (string.IsNullOrEmpty(vm.ItemTextIds) == false)
                {
                    #region 问答题目
                    var ItemTextIds = vm.ItemTextIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();

                    var del = from p in db.Table<Entity.tbSurveyData>()
                              where ItemTextIds.Contains(p.tbSurveyItem.Id)
                              && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                              && p.tbTeacher.Id == vm.TeacherId
                              select p;

                    if (org == null)
                    {
                        del = del.Where(d => d.tbOrg == null);
                    }
                    else
                    {
                        del = del.Where(d => d.tbOrg.Id == org.Id);
                    }

                    if (vm.IsClass == true && classStudent != null)
                    {
                        del = del.Where(d => d.tbClass.Id == classStudent.tbClass.Id);
                    }
                    else
                    {
                        del = del.Where(d => d.tbClass == null);
                    }

                    var delList = (from p in del
                                   select p).ToList();

                    foreach (var a in delList)
                    {
                        a.IsDeleted = true;
                    }
                    var voteItemTextList = (from p in db.Table<Entity.tbSurveyItem>()
                                            where ItemTextIds.Contains(p.Id)
                                            select p).ToList();

                    foreach (var itemId in ItemTextIds)
                    {
                        var item = voteItemTextList.Where(d => d.Id == itemId).FirstOrDefault();
                        if (item != null)
                        {
                            if (item.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox)
                            {
                                if (Request["textareaText" + item.Id] != null)
                                {
                                    var tb = new Entity.tbSurveyData();
                                    tb.tbOrg = org;
                                    tb.tbStudent = student;
                                    tb.tbSurveyItem = db.Set<Entity.tbSurveyItem>().Find(itemId);
                                    tb.tbTeacher = teacher;
                                    tb.SurveyText = Request["textareaText" + item.Id].ToString();

                                    if (vm.IsClass && classStudent != null)
                                    {
                                        tb.tbClass = classStudent.tbClass;
                                    }
                                    db.Set<Entity.tbSurveyData>().Add(tb);
                                }
                            }
                        }
                    }
                    #endregion
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("提交了教师评价数据");
                }
                return Code.MvcHelper.Post(error, Url.Action("m_InputIndex", "SurveyData"), "提交成功");
            }
        }

        public ActionResult m_InputIndex()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyData.Input();
                vm.SurveyList = (from p in db.Table<Entity.tbSurveyClass>()
                                 join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                 where q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                    && q.tbStudent.IsDeleted == false
                                    && p.tbSurvey.IsOpen == true
                                    && p.tbClass.IsDeleted == false
                                    && p.tbSurvey.IsDeleted == false
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Text = p.tbSurvey.SurveyName,
                                     Value = p.tbSurvey.Id.ToString()
                                 }).Distinct().ToList();

                var muiSelect = (from p in vm.SurveyList
                                 select new Code.MuiJsonDataBind
                                 {
                                     text = p.Text,
                                     value = p.Value
                                 }).ToList();

                vm.SurveryJson = JsonConvert.SerializeObject(muiSelect);

                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                var survey = db.Set<Entity.tbSurvey>().Find(vm.SurveyId);
                if (survey != null)
                {
                    var surveyGroupList = db.Table<Entity.tbSurveyGroup>().Where(d => d.tbSurvey.Id == vm.SurveyId).ToList();
                    foreach (var group in surveyGroupList)
                    {
                        if (group.IsOrg)
                        {
                            var surveyCourseIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                                   where p.tbCourse.IsDeleted == false
                                                    && p.tbSurveyGroup.IsDeleted == false
                                                    && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                    && p.tbSurveyGroup.Id == @group.Id
                                                    && p.tbSurveyGroup.IsOrg
                                                   select p.tbCourse.Id).ToList();

                            var tbOrgTeacherList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                                    join t in db.Table<Course.Entity.tbOrgTeacher>() on p.tbOrg.Id equals t.tbOrg.Id
                                                    join s in db.Table<Entity.tbSurvey>() on p.tbOrg.tbYear.Id equals s.tbYear.Id
                                                    where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                       && s.Id == vm.SurveyId
                                                       && p.tbOrg.IsClass == false
                                                       && surveyCourseIds.Contains(p.tbOrg.tbCourse.Id)
                                                    select new Dto.SurveyData.Input.OrgTeacher
                                                    {
                                                        OrgId = p.tbOrg.Id,
                                                        SurveyId = s.Id,
                                                        TeacherId = t.tbTeacher.Id,
                                                        TeacherName = t.tbTeacher.TeacherName,
                                                        OrgName = p.tbOrg.OrgName,
                                                        IsChecked = t.tbTeacher.Id == vm.TeacherId && t.tbOrg.Id == vm.OrgId && vm.IsClass == false,
                                                        IsClass = false
                                                    }).ToList();

                            vm.OrgTeacherList = vm.OrgTeacherList.Union(tbOrgTeacherList).ToList();

                            // 学生所属行政教学班
                            int classId = (from c in db.Table<Basis.Entity.tbClassStudent>()
                                           where c.tbStudent.tbSysUser.Id == Code.Common.UserId
                                           select c.tbClass.Id).FirstOrDefault();

                            var classOrgScheduleList = (from t in db.Table<Course.Entity.tbOrgTeacher>()
                                                        join s in db.Table<Entity.tbSurvey>() on t.tbOrg.tbYear.Id equals s.tbYear.Id
                                                        where s.Id == vm.SurveyId
                                                              && t.tbOrg.IsClass == true
                                                              && t.tbOrg.tbClass.Id == classId
                                                              && surveyCourseIds.Contains(t.tbOrg.tbCourse.Id)
                                                        select new Dto.SurveyData.Input.OrgTeacher
                                                        {
                                                            OrgId = t.tbOrg.Id,
                                                            SurveyId = s.Id,
                                                            TeacherId = t.tbTeacher.Id,
                                                            TeacherName = t.tbTeacher.TeacherName,
                                                            OrgName = t.tbOrg.OrgName,
                                                            IsChecked = t.tbTeacher.Id == vm.TeacherId && t.tbOrg.Id == vm.OrgId && vm.IsClass == false,
                                                            IsClass = false
                                                        }).ToList();

                            vm.OrgTeacherList = vm.OrgTeacherList.Union(classOrgScheduleList).ToList();
                        }
                        else
                        {
                            // 学生所属行政班
                            var classTeacherList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                    join q in db.Table<Basis.Entity.tbClassTeacher>() on p.tbClass.Id equals q.tbClass.Id
                                                    join o in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals o.tbClass.Id
                                                    where o.tbSurvey.Id == vm.SurveyId
                                                        && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                        && p.tbStudent.IsDeleted == false
                                                        && q.tbTeacher.IsDeleted == false
                                                        && p.tbClass.IsDeleted == false
                                                    select new Dto.SurveyData.Input.OrgTeacher
                                                    {
                                                        OrgId = p.tbClass.Id,
                                                        OrgName = p.tbClass.ClassName,
                                                        TeacherId = q.tbTeacher.Id,
                                                        TeacherName = q.tbTeacher.TeacherName,
                                                        SurveyId = vm.SurveyId,
                                                        IsChecked = q.tbTeacher.Id == vm.TeacherId && p.tbClass.Id == vm.OrgId && vm.IsClass,
                                                        IsClass = true
                                                    }).ToList();

                            vm.OrgTeacherList = vm.OrgTeacherList.Union(classTeacherList).ToList();
                        }
                    }
                }

                #region 是否已评
                var myInputList = (from p in db.Table<Entity.tbSurveyData>()
                                   where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                   && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                   && p.tbStudent.IsDeleted == false
                                   && p.tbSurveyItem.IsDeleted == false
                                   && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                   && p.tbSurveyItem.tbSurveyGroup.tbSurvey.IsDeleted == false
                                   group p by new { teacherId = p.tbTeacher.Id, classId = p.tbClass == null ? 0 : p.tbClass.Id, orgId = p.tbOrg == null ? 0 : p.tbOrg.Id } into g
                                   select new
                                   {
                                       teacherId = g.Key.teacherId,
                                       classId = g.Key.classId,
                                       orgId = g.Key.orgId
                                   }).Distinct().ToList();

                foreach (var a in vm.OrgTeacherList)
                {
                    if (myInputList.Where(d => d.teacherId == a.TeacherId && d.classId == a.OrgId && a.IsClass).Count() > decimal.Zero)
                    {
                        a.IsHaveInput = true;
                    }
                    else if (myInputList.Where(d => d.teacherId == a.TeacherId && d.orgId == a.OrgId && a.IsClass == false).Count() > decimal.Zero)
                    {
                        a.IsHaveInput = true;
                    }
                    else
                    {
                        a.IsHaveInput = false;
                    }
                }
                #endregion
                if (vm.IsHaveInput != decimal.Zero)
                {
                    if (vm.IsHaveInput == decimal.One)
                    {
                        vm.OrgTeacherList = vm.OrgTeacherList.Where(d => d.IsHaveInput == true).ToList();
                    }
                    else
                    {
                        vm.OrgTeacherList = vm.OrgTeacherList.Where(d => d.IsHaveInput == false).ToList();
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult m_InputIndex(Models.SurveyData.Input vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("m_InputIndex", new
            {
                searchText = vm.SearchText,
                surveyId = vm.SurveyId,
                IsHaveInput = vm.IsHaveInput
            }));
        }

        public ActionResult InputHor()
        {
            #region 是否学生
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Student)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }
            #endregion

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyData.InputHor();

                #region 评教下拉
                vm.SurveyList = (from p in db.Table<Entity.tbSurveyClass>()
                                 join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbClass.Id equals q.tbClass.Id
                                 where q.tbStudent.tbSysUser.Id == Code.Common.UserId
                                    && q.tbStudent.IsDeleted == false
                                    && p.tbSurvey.IsOpen == true
                                    && p.tbClass.IsDeleted == false
                                    && p.tbSurvey.IsDeleted == false
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Text = p.tbSurvey.SurveyName,
                                     Value = p.tbSurvey.Id.ToString()
                                 }).Distinct().ToList();

                if (vm.SurveyId == 0 && vm.SurveyList.Count > 0)
                {
                    vm.SurveyId = vm.SurveyList.FirstOrDefault().Value.ConvertToInt();
                }

                var tbSurvey = (from p in db.Table<Entity.tbSurvey>()
                                where p.Id == vm.SurveyId
                                select p).FirstOrDefault();

                if (tbSurvey == null)
                {
                    return View(vm);
                }
                #endregion

                #region 评价教师
                var surveyGroupList = (from p in db.Table<Entity.tbSurveyGroup>()
                                       where p.tbSurvey.Id == vm.SurveyId
                                       && p.tbSurvey.IsDeleted == false
                                       orderby p.No
                                       select p).ToList();

                foreach (var surveyGroup in surveyGroupList)
                {
                    if (surveyGroup.IsOrg)
                    {
                        var surveyCourseIds = (from p in db.Table<Entity.tbSurveyCourse>()
                                               where p.tbCourse.IsDeleted == false
                                                && p.tbSurveyGroup.IsDeleted == false
                                                && p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                                && p.tbSurveyGroup.Id == surveyGroup.Id
                                                && p.tbSurveyGroup.IsOrg
                                               select p.tbCourse.Id).ToList();

                        var tbOrgTeacherList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                                join t in db.Table<Course.Entity.tbOrgTeacher>() on p.tbOrg.Id equals t.tbOrg.Id
                                                join s in db.Table<Entity.tbSurvey>() on p.tbOrg.tbYear.Id equals s.tbYear.Id
                                                where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                   && s.Id == vm.SurveyId
                                                   && p.tbOrg.IsClass == false
                                                   && surveyCourseIds.Contains(p.tbOrg.tbCourse.Id)
                                                select new Dto.SurveyData.Input.OrgTeacher
                                                {
                                                    OrgId = p.tbOrg.Id,
                                                    OrgName = p.tbOrg.OrgName,
                                                    SurveyId = s.Id,
                                                    SurveyGroupId = surveyGroup.Id,
                                                    SurveyGroupName = surveyGroup.SurveyGroupName,
                                                    TeacherId = t.tbTeacher.Id,
                                                    TeacherCode = t.tbTeacher.TeacherCode,
                                                    TeacherName = t.tbTeacher.TeacherName,
                                                    IsClass = false
                                                }).ToList();

                        vm.OrgTeacherList = vm.OrgTeacherList.Union(tbOrgTeacherList).ToList();

                        int classId = (from c in db.Table<Basis.Entity.tbClassStudent>()
                                       where c.tbStudent.tbSysUser.Id == Code.Common.UserId
                                       select c.tbClass.Id).FirstOrDefault();

                        var classOrgScheduleList = (from t in db.Table<Course.Entity.tbOrgTeacher>()
                                                    join s in db.Table<Entity.tbSurvey>() on t.tbOrg.tbYear.Id equals s.tbYear.Id
                                                    where s.Id == vm.SurveyId
                                                          && t.tbOrg.IsClass == true
                                                          && t.tbOrg.tbClass.Id == classId
                                                          && surveyCourseIds.Contains(t.tbOrg.tbCourse.Id)
                                                    select new Dto.SurveyData.Input.OrgTeacher
                                                    {
                                                        OrgId = t.tbOrg.Id,
                                                        OrgName = t.tbOrg.OrgName,
                                                        SurveyId = s.Id,
                                                        SurveyGroupId = surveyGroup.Id,
                                                        SurveyGroupName = surveyGroup.SurveyGroupName,
                                                        TeacherId = t.tbTeacher.Id,
                                                        TeacherCode = t.tbTeacher.TeacherCode,
                                                        TeacherName = t.tbTeacher.TeacherName,
                                                        IsClass = false
                                                    }).ToList();

                        vm.OrgTeacherList = vm.OrgTeacherList.Union(classOrgScheduleList).ToList();
                    }
                    else
                    {
                        var classTeacherList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                join q in db.Table<Basis.Entity.tbClassTeacher>() on p.tbClass.Id equals q.tbClass.Id
                                                join o in db.Table<Entity.tbSurveyClass>() on p.tbClass.Id equals o.tbClass.Id
                                                where o.tbSurvey.Id == vm.SurveyId
                                                    && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                    && p.tbStudent.IsDeleted == false
                                                    && q.tbTeacher.IsDeleted == false
                                                    && p.tbClass.IsDeleted == false
                                                select new Dto.SurveyData.Input.OrgTeacher
                                                {
                                                    OrgId = p.tbClass.Id,
                                                    OrgName = p.tbClass.ClassName,
                                                    SurveyId = vm.SurveyId,
                                                    SurveyGroupId = surveyGroup.Id,
                                                    SurveyGroupName = surveyGroup.SurveyGroupName,
                                                    TeacherId = q.tbTeacher.Id,
                                                    TeacherCode = q.tbTeacher.TeacherCode,
                                                    TeacherName = q.tbTeacher.TeacherName,
                                                    IsClass = true
                                                }).ToList();

                        vm.OrgTeacherList = vm.OrgTeacherList.Union(classTeacherList).ToList();
                    }
                }
                #endregion

                #region 评价项目
                var surveyGroupIds = vm.OrgTeacherList.Select(d => d.SurveyGroupId).Distinct().ToList();

                vm.SurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                     where surveyGroupIds.Contains(p.tbSurveyGroup.Id)
                                     && p.tbSurveyGroup.IsDeleted == false
                                     && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                                     select new Dto.SurveyItem.Info
                                     {
                                         Id = p.Id,
                                         No = p.No,
                                         SurveyGroupId = p.tbSurveyGroup.Id,
                                         SurveyItemName = p.SurveyItemName,
                                         IsVertical = p.IsVertical,
                                         SurveyItemType = p.SurveyItemType,
                                         TextMaxLength = p.TextMaxLength
                                     }).ToList();

                var itemIds = vm.SurveyItemList.Where(d => d.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox).Select(d => d.Id).ToList();

                var itemIdTexts = vm.SurveyItemList.Where(d => d.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox).Select(d => d.Id).ToList();

                vm.ItemIds = string.Join("|", itemIds);
                vm.ItemTextIds = string.Join("|", itemIdTexts);

                vm.SurveyOptionList = (from p in db.Table<Entity.tbSurveyOption>()
                                       where itemIds.Contains(p.tbSurveyItem.Id)
                                       select new Dto.SurveyOption.Info
                                       {
                                           Id = p.Id,
                                           No = p.No,
                                           OptionName = p.OptionName,
                                           SurveyItemId = p.tbSurveyItem.Id
                                       }).ToList();

                //需要选中的值
                vm.SurveyDataList = (from p in db.Table<Entity.tbSurveyData>()
                                     where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                     && itemIds.Contains(p.tbSurveyItem.Id)
                                     && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                     && p.tbSurveyItem.IsDeleted == false
                                     && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                     && p.tbSurveyItem.SurveyItemType != Code.EnumHelper.SurveyItemType.TextBox
                                     select new Dto.SurveyData.Edit
                                     {
                                         TeacherId = p.tbTeacher.Id,
                                         OrgId = p.tbOrg == null ? 0 : p.tbOrg.Id,
                                         ClassId = p.tbClass == null ? 0 : p.tbClass.Id,
                                         ItemId = p.tbSurveyItem.Id,
                                         OptionId = p.tbSurveyOption.Id,
                                         SurveyText = p.SurveyText
                                     }).ToList();

                //需要选中的值
                vm.SurveyDataTextList = (from p in db.Table<Entity.tbSurveyData>()
                                         where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                         && itemIdTexts.Contains(p.tbSurveyItem.Id)
                                         && p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                         && p.tbSurveyItem.IsDeleted == false
                                         && p.tbSurveyItem.tbSurveyGroup.IsDeleted == false
                                         && p.tbSurveyItem.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox
                                         select new Dto.SurveyData.Edit
                                         {
                                             OrgId = p.tbOrg == null ? 0 : p.tbOrg.Id,
                                             ClassId = p.tbClass == null ? 0 : p.tbClass.Id,
                                             TeacherId = p.tbTeacher.Id,
                                             ItemId = p.tbSurveyItem.Id,
                                             SurveyText = p.SurveyText
                                         }).ToList();
                #endregion
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InputHorSave(Models.SurveyData.InputHor vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var arryInt = new List<int>();
                    var arrySurveyGroupId = Request["hdfSurveyGroupId"] == null ? arryInt : Request["hdfSurveyGroupId"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                    var arrySurveyItemIdList = arryInt;
                    foreach (var surveyGroupId in arrySurveyGroupId)
                    {
                        var arrySurveyItemId = Request["hdfSurveyItemId" + surveyGroupId] == null ? arryInt : Request["hdfSurveyItemId" + surveyGroupId].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                        arrySurveyItemIdList.AddRange(arrySurveyItemId);
                    }

                    var dbSurveyItemList = (from p in db.Table<Entity.tbSurveyItem>()
                                            where p.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                            && arrySurveyItemIdList.Contains(p.Id)
                                            select p).ToList();

                    var tbClassStudent = (from p in db.Table<Basis.Entity.tbClassStudent>().Include(p => p.tbClass)
                                          where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                          select p).FirstOrDefault();

                    var tbStudent = (from p in db.Table<Student.Entity.tbStudent>()
                                     where p.tbSysUser.Id == Code.Common.UserId
                                     select p).FirstOrDefault();

                    foreach (var surveyGroupId in arrySurveyGroupId)
                    {
                        var arrySurveyItemId = Request["hdfSurveyItemId" + surveyGroupId] == null ? arryInt : Request["hdfSurveyItemId" + surveyGroupId].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                        var arryTeacherId = Request["hdfTeacherId" + surveyGroupId] == null ? arryInt : Request["hdfTeacherId" + surveyGroupId].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                        var arryOrgId = Request["hdfOrgId" + surveyGroupId] == null ? arryInt : Request["hdfOrgId" + surveyGroupId].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                        var arryIsClass = Request["hdfIsClass" + surveyGroupId] == null ? arryInt : Request["hdfIsClass" + surveyGroupId].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                        var i = 0;
                        foreach (var teacherId in arryTeacherId)
                        {
                            var OrgId = arryOrgId[i];
                            var IsClass = arryIsClass[i] == 1 ? true : false;

                            var addEntitytbSurveyDataList = new List<Entity.tbSurveyData>();
                            foreach (var itemId in arrySurveyItemId)
                            {
                                var item = dbSurveyItemList.Where(d => d.Id == itemId).FirstOrDefault();
                                if (item == null)
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    var _ControlId = "_" + surveyGroupId + "_" + teacherId + "_" + item.Id;

                                    if (item.SurveyItemType == Code.EnumHelper.SurveyItemType.Radio)
                                    {
                                        #region 单选题目
                                        if (Request["Radio" + _ControlId] == null)
                                        {
                                            error.AddError("请填写完整!");
                                            break;
                                        }
                                        else
                                        {
                                            var OptionId = Request["Radio" + _ControlId].ConvertToInt();
                                            var tb = new Entity.tbSurveyData();
                                            tb.tbSurveyItem = item;
                                            tb.tbSurveyOption = db.Set<Entity.tbSurveyOption>().Find(OptionId);
                                            tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(teacherId);
                                            tb.tbOrg = db.Set<Course.Entity.tbOrg>().Find(OrgId);
                                            if (IsClass && tbClassStudent != null)
                                            {
                                                tb.tbClass = tbClassStudent.tbClass;
                                            }
                                            addEntitytbSurveyDataList.Add(tb);
                                        }
                                        #endregion
                                    }
                                    else if (item.SurveyItemType == Code.EnumHelper.SurveyItemType.CheckBox)
                                    {
                                        #region 多选题目
                                        if (Request["Cbox" + _ControlId] == null)
                                        {
                                            error.AddError("请填写完整!");
                                            break;
                                        }
                                        else
                                        {
                                            var OptionList = Request["Cbox" + _ControlId].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                                            foreach (var OptionId in OptionList)
                                            {
                                                var tb = new Entity.tbSurveyData();
                                                tb.tbSurveyItem = item;
                                                tb.tbSurveyOption = db.Set<Entity.tbSurveyOption>().Find(OptionId);
                                                tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(teacherId);
                                                tb.tbOrg = db.Set<Course.Entity.tbOrg>().Find(OrgId);
                                                if (IsClass && tbClassStudent != null)
                                                {
                                                    tb.tbClass = tbClassStudent.tbClass;
                                                }
                                                addEntitytbSurveyDataList.Add(tb);
                                            }
                                        }
                                        #endregion
                                    }
                                    else if (item.SurveyItemType == Code.EnumHelper.SurveyItemType.TextBox)
                                    {
                                        #region 问答题目
                                        if (Request["Txt" + _ControlId] == null)
                                        {
                                            error.AddError("请填写完整!");
                                            break;
                                        }
                                        else
                                        {
                                            var SurveyText = Request["Txt" + _ControlId].ToString();
                                            var tb = new Entity.tbSurveyData();
                                            tb.SurveyText = SurveyText;
                                            tb.tbSurveyItem = item;
                                            tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(teacherId);
                                            tb.tbOrg = db.Set<Course.Entity.tbOrg>().Find(OrgId);
                                            if (IsClass && tbClassStudent != null)
                                            {
                                                tb.tbClass = tbClassStudent.tbClass;
                                            }
                                            addEntitytbSurveyDataList.Add(tb);
                                        }
                                        #endregion
                                    }
                                }
                            }
                            if (error.Count > 0)
                            {
                                return Code.MvcHelper.Post(error);
                            }

                            addEntitytbSurveyDataList.ForEach(p =>
                            {
                                p.tbStudent = tbStudent;
                                p.UpdateTime = DateTime.Now;
                            });

                            db.Set<Entity.tbSurveyData>().AddRange(addEntitytbSurveyDataList);
                            i++;
                        }
                    }

                    if (error.Count > 0)
                    {
                        return Code.MvcHelper.Post(error);
                    }
                    else
                    {
                        var delSurveyData = (from p in db.Table<Entity.tbSurveyData>()
                                             where p.tbSurveyItem.tbSurveyGroup.tbSurvey.Id == vm.SurveyId
                                             && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                             select p).ToList();

                        delSurveyData.ForEach(p =>
                            {
                                p.IsDeleted = true;
                                p.UpdateTime = DateTime.Now;
                            });

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("保存教师评价数据成功!");
                        }
                    }
                }
            }

            return Code.MvcHelper.Post(error, null, "提交成功!");
        }
    }
}