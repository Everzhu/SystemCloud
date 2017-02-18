using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyOption
{
    public class List
    {
        public List<Entity.tbStudyOption> StudyOptionList { get; set; } = new List<Entity.tbStudyOption>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}