using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XkSystem.Areas.Elective.Controllers;

namespace XkSystem.Areas.Elective.Models.ElectiveApply
{
    public class Edit
    {
        public Dto.ElectiveApply.Edit ElectiveApplyEdit { get; set; } = new Dto.ElectiveApply.Edit();

        public List<SelectListItem> CourseList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> CourseSubject { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> RoomList { get; set; } = new List<SelectListItem>();
        public Course.Dto.Course.Edit CourseEdit { get; set; } = new Course.Dto.Course.Edit();

        public List<SelectListItem> PeriodList { get; set; } =new List<SelectListItem>();
        public List<SelectListItem> WeekList { get; set; } = new List<SelectListItem>();

        public string SelectFiles { get; set; } = string.Empty;
        public List<SelectListItem> ElectiveList { get; set; } = new List<SelectListItem>();
        
        public int ElectiveId { get; set; } = HttpContext.Current.Request["ElectiveId"].ConvertToInt();
        
        public bool IsWeekPeriod { get; set; } = false;
    }
}