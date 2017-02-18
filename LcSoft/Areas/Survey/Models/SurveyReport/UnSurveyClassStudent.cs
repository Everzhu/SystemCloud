using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class UnSurveyClassStudent
    {
        public List<Dto.SurveyReport.UnSurveyClassStudent> List { get; set; } = new List<Dto.SurveyReport.UnSurveyClassStudent>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ClassId { get; set; }
    }
}