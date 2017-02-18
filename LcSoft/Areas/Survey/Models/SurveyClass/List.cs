using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyClass
{
    public class List
    {
        public List<Dto.SurveyClass.List> AppraiseClassList { get; set; } = new List<Dto.SurveyClass.List>();

        public string SurveyName { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();

        public List<Basis.Entity.tbGrade> GradeList { get; set; } = new List<Basis.Entity.tbGrade>();

        public List<Basis.Entity.tbClass> ClassList { get; set; } = new List<Basis.Entity.tbClass>();

        public string ClassIds { get; set; }
    }
}