using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class UnSubjectList
    {
        public List<Dto.SurveyReport.UnSubjectList> SurveyUnSubjectList { get; set; } = new List<Dto.SurveyReport.UnSubjectList>();
        public List<Dto.SurveyReport.UnSubjectList> SurveyUnSubjectTextList { get; set; } = new List<Dto.SurveyReport.UnSubjectList>();
        public List<Dto.SurveyReport.UnSubjectList> SurveyUnSubjectEachList { get; set; } = new List<Dto.SurveyReport.UnSubjectList>();
        public List<Dto.SurveyReport.UnSubjectList> SurveyTeacherList { get; set; } = new List<Dto.SurveyReport.UnSubjectList>();
        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyGradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyOrgList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveySubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyCourseList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyItemList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyOptionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<Dto.SurveyItem.Info> SurveyItemInfoList { get; set; } = new List<Dto.SurveyItem.Info>();
        public List<Dto.SurveyOption.Info> SurveyOptionInfoList { get; set; } = new List<Dto.SurveyOption.Info>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public int? SurveyGroupId { get; set; } = System.Web.HttpContext.Current.Request["SurveyGroupId"].ConvertToInt();
        public int? SurveyOrgId { get; set; } = System.Web.HttpContext.Current.Request["SurveyOrgId"].ConvertToInt();
        public int? SurveyClassId { get; set; } = System.Web.HttpContext.Current.Request["SurveyClassId"].ConvertToInt();
        public int? SurveyGradeId { get; set; } = System.Web.HttpContext.Current.Request["SurveyGradeId"].ConvertToInt();
        public int? SurveySubjectId { get; set; } = System.Web.HttpContext.Current.Request["SurveySubjectId"].ConvertToInt();
        public int? SurveyCourseId { get; set; } = System.Web.HttpContext.Current.Request["SurveyCourseId"].ConvertToInt();
        public int? SurveyTeacherId { get; set; } = System.Web.HttpContext.Current.Request["SurveyTeacherId"].ConvertToInt();
    }
}