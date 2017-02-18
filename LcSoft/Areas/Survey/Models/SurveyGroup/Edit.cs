using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyGroup
{
    public class Edit
    {
        public Entity.tbSurveyGroup SurveyGroupEdit { get; set; } = new Entity.tbSurveyGroup();

        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
    }
}