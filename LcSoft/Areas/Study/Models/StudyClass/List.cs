using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyClass
{
    public class List
    {
        public List<Dto.StudyClass.List> StudyClassList { get; set; } = new List<Dto.StudyClass.List>();
        public List<System.Web.Mvc.SelectListItem> RoomList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
    }
}