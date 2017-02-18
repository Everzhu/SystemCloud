using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformComment
{
    public class List
    {
        public List<Dto.PerformComment.List> PerformCommentList { get; set; } = new List<Dto.PerformComment.List>();

        public List<Dto.PerformData.OrgSelectInfo> OrgSelectInfo { get; set; } = new List<Dto.PerformData.OrgSelectInfo>();
        public List<Dto.PerformComment.List> PerformCommentFirstList { get; set; } = new List<Dto.PerformComment.List>();

        public Dto.PerformComment.List PerformComment { get; set; } = new Dto.PerformComment.List();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Areas.Basis.Dto.ClassStudent.List> ClassStudentList { get; set; } = new List<Areas.Basis.Dto.ClassStudent.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();
    }
}