using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class SubjectTeacherList
    {
        public List<Dto.SurveyReport.SubjectTeacherList> SurveySubjectTeacherList { get; set; } = new List<Dto.SurveyReport.SubjectTeacherList>();
        public List<Dto.SurveyReport.SubjectTeacherList> SurveySubjectTeacherTextList { get; set; } = new List<Dto.SurveyReport.SubjectTeacherList>();
        public List<Dto.SurveyReport.SubjectTeacherList> SurveyCourseList { get; set; } = new List<Dto.SurveyReport.SubjectTeacherList>();
        public List<Dto.SurveyReport.ClassTeacherList> SurveyTeacherList { get; set; } = new List<Dto.SurveyReport.ClassTeacherList>();
        public List<Dto.SurveyReport.ClassTeacherList> SurveyClassTeacherList { get; set; } = new List<Dto.SurveyReport.ClassTeacherList>();
        public List<Dto.SurveyReport.ClassTeacherList> SurveyClassTeacherTextList { get; set; } = new List<Dto.SurveyReport.ClassTeacherList>();
        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<Dto.SurveyItem.Info> SurveyItemInfoList { get; set; } = new List<Dto.SurveyItem.Info>();
        public List<Dto.SurveyOption.Info> SurveyOptionInfoList { get; set; } = new List<Dto.SurveyOption.Info>();
        public List<Dto.SurveyItem.Info> SurveyItemInfoClassList { get; set; } = new List<Dto.SurveyItem.Info>();
        public List<Dto.SurveyOption.Info> SurveyOptionInfoClassList { get; set; } = new List<Dto.SurveyOption.Info>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
    }
}