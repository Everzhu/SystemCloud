using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentFamily
{
    public class List
    {
        public List<Dto.StudentFamily.List> StudentFamilyList { get; set; } = new List<Dto.StudentFamily.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int StudentId { get; set; }
    }
}