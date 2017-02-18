using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyApply
{
    public class CheckEdit
    {
        public Dto.StudyApply.CheckEdit CheckStudyApplyEdit { get; set; } = new Dto.StudyApply.CheckEdit();
        public List<System.Web.Mvc.SelectListItem> CheckStatusList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
    }
}