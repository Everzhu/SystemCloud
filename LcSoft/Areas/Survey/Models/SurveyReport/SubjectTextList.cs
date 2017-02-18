using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class SubjectTextList
    {
        public List<Dto.SurveyReport.SubjectTextList> SurveySubjectTextList { get; set; } = new List<Dto.SurveyReport.SubjectTextList>();
        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public int SurveyCourseId { get; set; } = System.Web.HttpContext.Current.Request["SurveyCourseId"].ConvertToInt();
        public int SurveyTeacherId { get; set; } = System.Web.HttpContext.Current.Request["SurveyTeacherId"].ConvertToInt();
        public int SurveyItemId { get; set; } = System.Web.HttpContext.Current.Request["SurveyItemId"].ConvertToInt();
        //打开方式
        public int OpenFlag { get; set; } = System.Web.HttpContext.Current.Request["OpenFlag"].ConvertToInt();
    }
}