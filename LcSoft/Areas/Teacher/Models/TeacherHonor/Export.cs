using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.TeacherHonor
{
    public class Export
    {
        public List<Dto.TeacherHonor.Export> DataList { get; set; } = new List<Dto.TeacherHonor.Export>();
        
        public string searchText { get; set; } = HttpContext.Current.Request["searchText"].ConvertToString();

        public int TeacherHonorTypeId { get; set; } = HttpContext.Current.Request["TeacherHonorTypeId"].ConvertToInt();

        public int TeacherHonorLevelId { get; set; } = HttpContext.Current.Request["TeacherHonorLevelId"].ConvertToInt();
    }
}