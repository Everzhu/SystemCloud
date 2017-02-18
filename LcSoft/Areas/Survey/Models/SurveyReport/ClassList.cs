using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class ClassList
    {
        public List<Dto.SurveyReport.ClassList> SurveyReportClassList { get; set; } = new List<Dto.SurveyReport.ClassList>();

        public List<Dto.SurveyReport.ClassList> SurveyReportOrgList { get; set; } = new List<Dto.SurveyReport.ClassList>();

        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Dto.SurveyGroup.Info> SurveyGroupList { get; set; } = new List<Dto.SurveyGroup.Info>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
    }
}