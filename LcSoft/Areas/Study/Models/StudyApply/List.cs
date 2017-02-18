using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyApply
{
    public class List
    {
        public List<Dto.StudyApply.List> StudyApplyList { get; set; } = new List<Dto.StudyApply.List>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}