using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentFamily
{
    public class FamilyList
    {
        public List<Dto.StudentFamily.FamilyList> StudentFamilyList { get; set; } = new List<Dto.StudentFamily.FamilyList>();

        public string Step { get; set; } = System.Web.HttpContext.Current.Request["Step"].ConvertToString();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int StudentId { get; set; }
    }
}