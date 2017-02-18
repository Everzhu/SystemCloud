using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class GradeTeacherRanking
    {
        public List<Dto.SurveyReport.GradeTeacherRanking> SurveyGradeTeacherRankingList { get; set; } = new List<Dto.SurveyReport.GradeTeacherRanking>();
        public List<Dto.SurveyReport.GradeTeacherRanking> SurveyGradeTeacherNoRankingList { get; set; } = new List<Dto.SurveyReport.GradeTeacherRanking>();
        public List<System.Web.Mvc.SelectListItem> SurveyGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyGradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public int? SurveyGroupId { get; set; } = System.Web.HttpContext.Current.Request["SurveyGroupId"].ConvertToInt();
        public int? SurveyGradeId { get; set; } = System.Web.HttpContext.Current.Request["SurveyGradeId"].ConvertToInt();
    }
}