using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Models.ElectiveSubject
{
    public class List
    {
        public int ElectiveId { get; set; } = HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();

        public string ElectiveName { get; set; } = string.Empty;
        public List<Dto.ElectiveSubject.List> ElectiveSubjectList { get; set; } = new List<Dto.ElectiveSubject.List>();
        public List<Course.Dto.Subject.Info> CourseSubjectList { get; set; } = new List<Course.Dto.Subject.Info>();
    }
}