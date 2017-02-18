using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class UnSubjectFullList
    {
        public List<Dto.SurveyReport.UnSubjectFullList> SurveyUnSubjectFullList { get; set; } = new List<Dto.SurveyReport.UnSubjectFullList>();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public int SurveyCourseId { get; set; } = System.Web.HttpContext.Current.Request["SurveyCourseId"].ConvertToInt();
        public int SurveyTeacherId { get; set; } = System.Web.HttpContext.Current.Request["SurveyTeacherId"].ConvertToInt();
        public int SurveyOrgId { get; set; } = System.Web.HttpContext.Current.Request["SurveyOrgId"].ConvertToInt();
        public int OpenFlag { get; set; } = System.Web.HttpContext.Current.Request["OpenFlag"].ConvertToInt();
    }
}