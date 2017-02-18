using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyItem
{
    public class List
    {
        public List<Dto.SurveyItem.List> SurveyItemList { get; set; } = new List<Dto.SurveyItem.List>();

        public List<System.Web.Mvc.SelectListItem> SurveyGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SurveyName { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
    }
}