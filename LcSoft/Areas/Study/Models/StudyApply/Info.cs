using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyApply
{
    public class Info
    {
        public Dto.StudyApply.Info StudyApplyInfo { get; set; } = new Dto.StudyApply.Info();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
    }
}