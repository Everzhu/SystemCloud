using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.DormStudent
{
    public class Edit
    {
        public Dto.DormStudent.Edit DormStudentEdit { get; set; } = new Dto.DormStudent.Edit();

        public List<System.Web.Mvc.SelectListItem> DormList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> BuildList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> RoomList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}