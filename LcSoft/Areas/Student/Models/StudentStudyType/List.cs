using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentStudyType
{
    public class List
    {
        public List<Entity.tbStudentStudyType> StudyTypeList { get; set; } = new List<Entity.tbStudentStudyType>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}