using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentHonor
{
    public class List
    {
        public List<Dto.StudentHonor.List> StudentHonorList { get; set; } = new List<Dto.StudentHonor.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int StudentId { get; set; }
    }
}