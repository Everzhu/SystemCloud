using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyApply
{
    public class CheckList
    {
        public List<Dto.StudyApply.CheckList> CheckStudyApplyList { get; set; } = new List<Dto.StudyApply.CheckList>();
        public List<System.Web.Mvc.SelectListItem> CheckStatusList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int CheckStatuId { get; set; } = System.Web.HttpContext.Current.Request["CheckStatuId"].ConvertToInt();
        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}