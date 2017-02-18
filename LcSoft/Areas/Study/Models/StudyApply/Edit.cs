using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyApply
{
    public class Edit
    {
        public Dto.StudyApply.Edit StudyApplyEdit { get; set; } = new Dto.StudyApply.Edit();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
    }
}