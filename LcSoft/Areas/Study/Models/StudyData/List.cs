using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyData
{
    public class List
    {
        public List<Dto.StudyData.List> StudyDataList { get; set; } = new List<Dto.StudyData.List>();
        public List<System.Web.Mvc.SelectListItem> StudyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> RoomOrClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int RoomOrClassId { get; set; } = System.Web.HttpContext.Current.Request["RoomOrClassId"].ConvertToInt();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
        public string DateSearch { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearch"]);
        /// <summary>
        /// 自习模式：True:班级模式 False:教室模式
        /// </summary>
        public bool IsRoomType { get; set; } = false;
    }
}