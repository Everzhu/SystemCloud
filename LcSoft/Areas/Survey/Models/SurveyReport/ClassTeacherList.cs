using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class ClassTeacherList
    {
        public List<Dto.SurveyReport.ClassTeacherList> SurveyClassTeacherList { get; set; } = new List<Dto.SurveyReport.ClassTeacherList>();
        public List<Dto.SurveyReport.ClassTeacherList> SurveyClassTeacherTextList { get; set; } = new List<Dto.SurveyReport.ClassTeacherList>();
        public List<Dto.SurveyReport.ClassTeacherList> SurveyTeacherList { get; set; } = new List<Dto.SurveyReport.ClassTeacherList>();
        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyGradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<Dto.SurveyItem.Info> SurveyItemInfoList { get; set; } = new List<Dto.SurveyItem.Info>();
        public List<Dto.SurveyOption.Info> SurveyOptionInfoList { get; set; } = new List<Dto.SurveyOption.Info>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public int? SurveyClassId { get; set; } = System.Web.HttpContext.Current.Request["SurveyClassId"].ConvertToInt();
        public int? SurveyGradeId { get; set; } = System.Web.HttpContext.Current.Request["SurveyGradeId"].ConvertToInt();
    }
}