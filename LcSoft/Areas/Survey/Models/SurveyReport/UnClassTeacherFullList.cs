using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class UnClassTeacherFullList
    {
        public List<Dto.SurveyReport.UnClassTeacherFullList> SurveyUnClassTeacherFullList { get; set; } = new List<Dto.SurveyReport.UnClassTeacherFullList>();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public int SurveyTeacherId { get; set; } = System.Web.HttpContext.Current.Request["SurveyTeacherId"].ConvertToInt();
        public int SurveyClassId { get; set; } = System.Web.HttpContext.Current.Request["SurveyClassId"].ConvertToInt();
        public int OpenFlag { get; set; } = System.Web.HttpContext.Current.Request["OpenFlag"].ConvertToInt();
    }
}