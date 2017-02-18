using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyRoomTeacher
{
    public class Edit
    {
        [Display(Name = "教师"), Required]
        public int TeacherId { get; set; }
        public Dto.StudyRoomTeacher.Edit StudyRoomTeacherEdit { get; set; } = new Dto.StudyRoomTeacher.Edit();
        public List<int> WeekIdList = new List<int>();
        public List<System.Web.Mvc.SelectListItem> TeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> WeekList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int RoomId { get; set; } = System.Web.HttpContext.Current.Request["RoomId"].ConvertToInt();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
    }
}