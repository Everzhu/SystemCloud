using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyData
{
    public class Input
    {
        public List<Dto.SurveyData.Input.OrgTeacher> OrgTeacherList { get; set; } = new List<Dto.SurveyData.Input.OrgTeacher>();

        public List<Dto.SurveyItem.Info> SurveyItemList { get; set; } = new List<Dto.SurveyItem.Info>();

        public List<Dto.SurveyOption.Info> SurveyOptionList { get; set; } = new List<Dto.SurveyOption.Info>();

        public List<Dto.SurveyData.Edit> SurveyDataList { get; set; } = new List<Dto.SurveyData.Edit>();
        public List<Dto.SurveyData.Edit> SurveyDataTextList { get; set; } = new List<Dto.SurveyData.Edit>();

        public List<Areas.Perform.Dto.PerformData.OrgSelectInfo> OrgSelectInfo { get; set; } = new List<Areas.Perform.Dto.PerformData.OrgSelectInfo>();

        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();

        public int OrgId { get; set; } = System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();

        public int TeacherId { get; set; } = System.Web.HttpContext.Current.Request["TeacherId"].ConvertToInt();
        public int RankIng { get; set; } = System.Web.HttpContext.Current.Request["RankIng"].ConvertToInt();
        public string ItemIds { get; set; }
        public string ItemTextIds { get; set; }

        public string SurveryJson { get; set; }

        public bool IsClass { get; set; } = System.Web.HttpContext.Current.Request["IsClass"].ToBoolean();

        public int IsHaveInput { get; set; } = System.Web.HttpContext.Current.Request["IsHaveInput"].ConvertToInt();
    }
}