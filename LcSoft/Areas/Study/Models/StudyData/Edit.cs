using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyData
{
    public class Edit
    {
        public Dto.StudyData.Edit StudyDataEdit { get; set; } = new Dto.StudyData.Edit();
        public List<System.Web.Mvc.SelectListItem> StudyOptionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();
        public string DateSearch { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearch"]);
    }
}