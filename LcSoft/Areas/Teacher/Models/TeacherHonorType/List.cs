using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.TeacherHonorType
{
    public class List
    {
        public List<Entity.tbTeacherHonorType> DataList { get; set; } = new List<Entity.tbTeacherHonorType>();

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}