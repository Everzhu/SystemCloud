using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentHonorType
{
    public class List
    {
        public List<Entity.tbStudentHonorType> StudentHonorTypeList { get; set; } = new List<Entity.tbStudentHonorType>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}