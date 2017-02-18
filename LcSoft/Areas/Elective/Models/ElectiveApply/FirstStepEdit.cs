using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Models.ElectiveApply
{
    public class FirstStepEdit
    {
        
        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ElectiveId { get; set; } = HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public List<Areas.Course.Dto.Course.List> CourseList { get; set; } = new List<Course.Dto.Course.List>();

        public List<SelectListItem> ElectiveList { get; set; } = new List<SelectListItem>();
    }
}