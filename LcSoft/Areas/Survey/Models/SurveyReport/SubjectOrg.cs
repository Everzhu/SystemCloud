using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class SubjectOrg
    {
        public List<Dto.SurveyReport.SubjectOrg> SurveySubjectOrgList { get; set; } = new List<Dto.SurveyReport.SubjectOrg>();
        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveySubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyCourseList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyItemList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyOptionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyTeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public int? SurveyGroupId { get; set; } = System.Web.HttpContext.Current.Request["SurveyGroupId"].ConvertToInt();
        public int? SurveySubjectId { get; set; } = System.Web.HttpContext.Current.Request["SurveySubjectId"].ConvertToInt();
        public int? SurveyCourseId { get; set; } = System.Web.HttpContext.Current.Request["SurveyCourseId"].ConvertToInt();
        public int? SurveyTeacherId { get; set; } = System.Web.HttpContext.Current.Request["SurveyTeacherId"].ConvertToInt();
    }
}