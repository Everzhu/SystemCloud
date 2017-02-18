using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityData
{
    public class OrgInput
    {
        public List<System.Web.Mvc.SelectListItem> QualityList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> QualityItemGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Entity.tbQualityItem> QualityItemList { get; set; } = new List<Entity.tbQualityItem>();

        public List<Entity.tbQualityOption> QualityOptionList { get; set; } = new List<Entity.tbQualityOption>();

        public List<Student.Entity.tbStudent> StudentList { get; set; } = new List<Student.Entity.tbStudent>();

        public List<Dto.QualityData.Edit> QualityDataList { get; set; } = new List<Dto.QualityData.Edit>();

        public List<System.Web.Mvc.SelectListItem> OrgList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? QualityId { get; set; } = System.Web.HttpContext.Current.Request["QualityId"].ConvertToInt();

        public int QualityItemGroupId { get; set; } = System.Web.HttpContext.Current.Request["QualityItemGroupId"].ConvertToInt();

        public int QualityItemId { get; set; } = System.Web.HttpContext.Current.Request["QualityItemId"].ConvertToInt();

        public int UserId { get; set; } = System.Web.HttpContext.Current.Request["UserId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public string ItemIds { get; set; }

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public int? OrgId { get; set; } = System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();
    }
}