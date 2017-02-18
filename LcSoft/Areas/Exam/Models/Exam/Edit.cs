using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.Exam
{
    public class Edit
    {
        public Dto.Exam.Edit ExamEdit { get; set; } = new Dto.Exam.Edit();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ExamTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> LevelGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SegmentGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}