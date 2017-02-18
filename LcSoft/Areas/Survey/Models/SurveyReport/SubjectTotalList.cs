using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class SubjectTotalList
    {
        public List<Dto.SurveyReport.SubjectTotalList> SurveySubjectTotalList { get; set; } = new List<Dto.SurveyReport.SubjectTotalList>();
        public List<Dto.SurveyReport.SubjectTotalList> SurveySubjectTotalRankingList { get; set; } = new List<Dto.SurveyReport.SubjectTotalList>();
        public List<Dto.SurveyReport.SubjectTotalList> SurveySubjectTotalTextList { get; set; } = new List<Dto.SurveyReport.SubjectTotalList>();
        public List<Dto.SurveyReport.SubjectTotalList> SurveyTeacherList { get; set; } = new List<Dto.SurveyReport.SubjectTotalList>();
        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveySubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyCourseList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<Dto.SurveyItem.Info> SurveyItemInfoList { get; set; } = new List<Dto.SurveyItem.Info>();
        public List<Dto.SurveyOption.Info> SurveyOptionInfoList { get; set; } = new List<Dto.SurveyOption.Info>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public int? SurveySubjectId { get; set; } = System.Web.HttpContext.Current.Request["SurveySubjectId"].ConvertToInt();
    }
}