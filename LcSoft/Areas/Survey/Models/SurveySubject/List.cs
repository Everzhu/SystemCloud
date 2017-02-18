using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveySubject
{
    public class List
    {
        public List<Dto.SurveySubject.List> AppraiseSubjectList { get; set; } = new List<Dto.SurveySubject.List>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Entity.tbSurveyGroup> SurveyGroupList { get; set; } = new List<Entity.tbSurveyGroup>();

        public int SurveyGroupId { get; set; } = System.Web.HttpContext.Current.Request["SurveyGroupId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int SubjectId { get; set; }

        public int SubjectTypeId { get; set; }
    }
}