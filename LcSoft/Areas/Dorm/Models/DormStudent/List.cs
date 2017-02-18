using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.DormStudent
{
    public class List
    {
        public List<Dto.DormStudent.List> DormStudentList { get; set; } = new List<Dto.DormStudent.List>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? SexId { get; set; } = System.Web.HttpContext.Current.Request["SexId"].ConvertToInt();

        public int? BuildId { get; set; } = System.Web.HttpContext.Current.Request["BuildId"].ConvertToInt();

        public int? RoomId { get; set; } = System.Web.HttpContext.Current.Request["RoomId"].ConvertToInt();

        public List<System.Web.Mvc.SelectListItem> SexList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> BuildList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> RoomList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}