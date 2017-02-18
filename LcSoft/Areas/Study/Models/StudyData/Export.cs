using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyData
{
    public class Export
    {
        public List<Dto.StudyData.Export> ExportList { get; set; } = new List<Dto.StudyData.Export>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int RoomOrClassId { get; set; } = System.Web.HttpContext.Current.Request["RoomOrClassId"].ConvertToInt();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
        public string DateSearch { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearch"]);
    }
}