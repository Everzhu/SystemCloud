using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class Export
    {
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();

        public int SurveyGroupId { get; set; } = System.Web.HttpContext.Current.Request["SurveyGroupId"].ConvertToInt();

        public int SurveyCourseId { get; set; } = System.Web.HttpContext.Current.Request["SurveyCourseId"].ConvertToInt();
        public List<Dto.SurveyData.Info> SurveyTotalList { get; set; } = new List<Dto.SurveyData.Info>();
    }
}