using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveySubject
{
    public class Edit
    {
        public List<Dto.SurveySubject.Edit> AppraiseEditList { get; set; } = new List<Dto.SurveySubject.Edit>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int GroupId { get; set; }

        public int SurveyId { get; set; }

        public int SubjectId { get; set; }
    }
}