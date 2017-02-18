using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyItem
{
    public class Edit
    {
        public Dto.SurveyItem.Edit SurveyItemEdit { get; set; } = new Dto.SurveyItem.Edit();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public List<System.Web.Mvc.SelectListItem> SurveyGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<Dto.SurveyOption.List> SurveyOptionList { get; set; } = new List<Dto.SurveyOption.List>();
        public List<System.Web.Mvc.SelectListItem> SurveyItemTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}