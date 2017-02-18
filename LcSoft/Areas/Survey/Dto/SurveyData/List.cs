using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Dto.SurveyData
{
    public class Edit
    {
        public int ItemId { get; set; }
        public int OptionId { get; set; }
        public int TeacherId { get; set; }
        public int OrgId { get; set; }
        public int ClassId { get; set; }
        public string SurveyText { get; set; }
    }
}