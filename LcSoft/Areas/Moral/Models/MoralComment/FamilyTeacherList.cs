using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class FamilyTeacherList
    {
        public List<Dto.MoralComment.FamilyTeacherList> FTeacherList { get; set; } = new List<Dto.MoralComment.FamilyTeacherList>();

        public List<SelectListItem> YearList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> YearTermList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? YearTermId { get; set; } = System.Web.HttpContext.Current.Request["YearTermId"].ConvertToInt();

        public int? ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();
    }
}