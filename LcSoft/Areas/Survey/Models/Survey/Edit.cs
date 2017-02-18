using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.Survey
{
    public class Edit
    {
        public Dto.Survey.Edit SurveyEdit { get; set; } = new Dto.Survey.Edit();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["PerformId"].ConvertToInt();

        public int SurveySolutionId { get; set; }

        public int YearId { get; set; }

        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string CreateWay { get; set; }

        public int CopySurveyId { get; set; }
    }
}