using System.Collections.Generic;
using System.Web;

namespace XkSystem.Areas.Course.Models.Course
{
    public class List
    {
        public List<Dto.Course.List> CourseList { get; set; } = new List<Dto.Course.List>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CourseTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CourseDomainList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CourseGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Dto.Subject.Info> CourseSubjectList { get; set; } = new List<Dto.Subject.Info>();

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? SubjectId { get; set; } = HttpContext.Current.Request["SubjectId"].ConvertToInt();

        public int? CourseTypeId { get; set; } = HttpContext.Current.Request["CourseTypeId"].ConvertToInt();

        public int? CourseDomainTypeId { get; set; } = HttpContext.Current.Request["CourseDomainTypeId"].ConvertToInt();

        public int? CourseDomainId { get; set; } = HttpContext.Current.Request["CourseDomainId"].ConvertToInt();

        public int? CourseGroupId { get; set; } = HttpContext.Current.Request["CourseGroupId"].ConvertToInt();

        public string ShowModel { get; set; } = HttpContext.Current.Request["ShowModel"].ConvertToString();
    }
}