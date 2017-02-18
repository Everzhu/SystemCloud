using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Course
{
    public class Export
    {
        public List<Dto.Course.Export> ExportList { get; set; } = new List<Dto.Course.Export>();

        public int? SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();

        public int? CourseTypeId { get; set; } = System.Web.HttpContext.Current.Request["CourseTypeId"].ConvertToInt();
    }
}