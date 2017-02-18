using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class UnClassTeacherList
    {
        public List<Dto.SurveyReport.UnClassTeacherList> SurveyUnClassTeacherList { get; set; } = new List<Dto.SurveyReport.UnClassTeacherList>();
        public List<Dto.SurveyReport.UnClassTeacherList> SurveyUnClassTeacherTextList { get; set; } = new List<Dto.SurveyReport.UnClassTeacherList>();
        public List<Dto.SurveyReport.UnClassTeacherList> SurveyUnClassTeacherEachList { get; set; } = new List<Dto.SurveyReport.UnClassTeacherList>();
        public List<Dto.SurveyReport.UnClassTeacherList> SurveyClassTeacherList { get; set; } = new List<Dto.SurveyReport.UnClassTeacherList>();
        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyGradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyItemList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SurveyOptionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<Dto.SurveyItem.Info> SurveyItemInfoList { get; set; } = new List<Dto.SurveyItem.Info>();
        public List<Dto.SurveyOption.Info> SurveyOptionInfoList { get; set; } = new List<Dto.SurveyOption.Info>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public int? SurveyGroupId { get; set; } = System.Web.HttpContext.Current.Request["SurveyGroupId"].ConvertToInt();
        public int? SurveyClassId { get; set; } = System.Web.HttpContext.Current.Request["SurveyClassId"].ConvertToInt();
        public int? SurveyGradeId { get; set; } = System.Web.HttpContext.Current.Request["SurveyGradeId"].ConvertToInt();
        public int? SurveyTeacherId { get; set; } = System.Web.HttpContext.Current.Request["SurveyTeacherId"].ConvertToInt();
    }
}