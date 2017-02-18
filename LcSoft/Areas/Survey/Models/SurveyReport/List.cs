using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class List
    {
        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SurveyGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SurveyCourseList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SurveyItemList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Areas.Survey.Dto.SurveyOption.Info> SurveyOptionList { get; set; } = new List<Dto.SurveyOption.Info>();

        public List<Areas.Teacher.Dto.Teacher.Info> SurveyTotalTeacherList { get; set; } = new List<Teacher.Dto.Teacher.Info>();
        public List<Dto.SurveyData.Info> SurveyTotalList { get; set; } = new List<Dto.SurveyData.Info>();

        public List<Teacher.Dto.Teacher.Info> TeacherList { get; set; } = new List<Teacher.Dto.Teacher.Info>();

        public List<Areas.Basis.Dto.Class.Info> ClassList { get; set; } = new List<Basis.Dto.Class.Info>();

        public List<Areas.Course.Dto.Org.Edit> OrgList { get; set; } = new List<Course.Dto.Org.Edit>();

        public int SurveyOptionAverageNumber { get; set; } = 5;

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();

        public int? SurveyGroupId { get; set; } = System.Web.HttpContext.Current.Request["SurveyGroupId"].ConvertToInt();

        public int? SurveyCourseId { get; set; } = System.Web.HttpContext.Current.Request["SurveyCourseId"].ConvertToInt();

        public int SurveyTeacherId { get; set; } = System.Web.HttpContext.Current.Request["SurveyTeacherId"].ConvertToInt();
    }
}