using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class FullList
    {
        public List<Dto.SurveyReport.FullList> SurveyFullList { get; set; } = new List<Dto.SurveyReport.FullList>();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public int IsOrgOrClass { get; set; } = System.Web.HttpContext.Current.Request["IsOrgOrClass"].ConvertToInt();
        public int TeacherId { get; set; } = System.Web.HttpContext.Current.Request["TeacherId"].ConvertToInt();
        public int IsSelected { get; set; } = System.Web.HttpContext.Current.Request["IsSelected"].ConvertToInt();
        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();
    }
}