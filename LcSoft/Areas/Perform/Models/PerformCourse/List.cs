using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformCourse
{
    public class List
    {
        //public List<Dto.PerformCourse.List> PerformCourseList { get; set; } = new List<Dto.PerformCourse.List>();

        public List<Entity.tbPerformGroup> PerformGroupList { get; set; } = new List<Entity.tbPerformGroup>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int SubjectId { get; set; }

        public int CourseTypeId { get; set; }

        public int PerformGroupId { get; set; } = System.Web.HttpContext.Current.Request["PerformGroupId"].ConvertToInt();

        public int PerformId { get; set; } = System.Web.HttpContext.Current.Request["PerformId"].ConvertToInt();
    }
}