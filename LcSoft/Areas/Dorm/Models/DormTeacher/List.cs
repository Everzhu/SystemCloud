using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.DormTeacher
{
    public class List
    {
        public List<Dto.DormTeacher.List> DormTeacherList { get; set; } = new List<Dto.DormTeacher.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? BuildId { get; set; } = System.Web.HttpContext.Current.Request["BuildId"].ConvertToInt();

        public List<System.Web.Mvc.SelectListItem> BuildList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}