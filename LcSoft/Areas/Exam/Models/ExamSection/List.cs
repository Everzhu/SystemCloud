using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamSection
{
    public class List
    {
        public List<Dto.ExamSection.List> ExamSectionList { get; set; } = new List<Dto.ExamSection.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}