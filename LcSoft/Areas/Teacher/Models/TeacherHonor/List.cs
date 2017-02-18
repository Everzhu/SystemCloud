using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.TeacherHonor
{
    public class List
    {
        public List<Dto.TeacherHonor.List> DataList { get; set; } = new List<Dto.TeacherHonor.List>();

        //public int TeacherId { get; set; } = HttpContext.Current.Request["TeacherId"].ConvertToInt();

        //public string TeacherName { get; set; }

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();

        public List<System.Web.Mvc.SelectListItem> TeacherHonorLevelList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> TeacherHonorTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int? TeacherHonorLevelId { get; set; } = HttpContext.Current.Request["TeacherHonorLevelId"].ConvertToInt();

        public int? TeacherHonorTypeId { get; set; } = HttpContext.Current.Request["TeacherHonorTypeId"].ConvertToInt();
    }
}