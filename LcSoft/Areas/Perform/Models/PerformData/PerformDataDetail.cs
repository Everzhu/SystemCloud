using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformData
{
    public class PerformDataDetail
    {
        public List<Dto.PerformData.Detail> PerformDataDetailList { get; set; } = new List<Dto.PerformData.Detail>();

        public List<System.Web.Mvc.SelectListItem> PerformItemList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Areas.Course.Dto.OrgStudent.List> OrgStudentList { get; set; } = new List<Areas.Course.Dto.OrgStudent.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int OrgId { get; set; } = System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();

        public int PerformId { get; set; } = System.Web.HttpContext.Current.Request["PerformId"].ConvertToInt();

        public int SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();
    }
}