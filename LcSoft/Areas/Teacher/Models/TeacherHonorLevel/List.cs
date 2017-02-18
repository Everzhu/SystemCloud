using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.TeacherHonorLevel
{
    public class List
    {
        public List<Entity.tbTeacherHonorLevel> DataList { get; set; } = new List<Entity.tbTeacherHonorLevel>();
        
        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}