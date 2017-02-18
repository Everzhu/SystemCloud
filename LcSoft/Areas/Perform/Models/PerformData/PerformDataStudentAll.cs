using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformData
{
    public class PerformDataStudentAll
    {
        public Dto.PerformData.StudentAll PerformDataStudent { get; set; } = new Dto.PerformData.StudentAll();
        public List<Dto.PerformData.StudentAll> PerformDataStudentAllList { get; set; } = new List<Dto.PerformData.StudentAll>();
        public List<Dto.PerformData.List> PerformDataList { get; set; } = new List<Dto.PerformData.List>();
        public List<Areas.Course.Dto.OrgStudent.List> OrgStudentList { get; set; } = new List<Areas.Course.Dto.OrgStudent.List>();
        public List<System.Web.Mvc.SelectListItem> PerformList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> PerformItemList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int PerformId { get; set; } = System.Web.HttpContext.Current.Request["PerformId"].ConvertToInt();
        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();
        public int SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();
    }
}