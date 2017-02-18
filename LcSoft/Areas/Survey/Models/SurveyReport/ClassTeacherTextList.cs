using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class ClassTeacherTextList
    {
        public List<Dto.SurveyReport.ClassTeacherTextList> SurveyClassTeacherTextList { get; set; } = new List<Dto.SurveyReport.ClassTeacherTextList>();
        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public int SurveyClassId { get; set; } = System.Web.HttpContext.Current.Request["SurveyClassId"].ConvertToInt();
        public int SurveyTeacherId { get; set; } = System.Web.HttpContext.Current.Request["SurveyTeacherId"].ConvertToInt();
        public int SurveyItemId { get; set; } = System.Web.HttpContext.Current.Request["SurveyItemId"].ConvertToInt();
    }
}