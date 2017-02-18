using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyGroup
{
    public class List
    {
        public List<Entity.tbSurveyGroup> SurveyGroupList { get; set; } = new List<Entity.tbSurveyGroup>();

        public string SurveyName { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
    }
}