using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentHonor
{
    public class HonorList
    {
        public List<Dto.StudentHonor.HonorList> honorList { get; set; } = new List<Dto.StudentHonor.HonorList>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public List<System.Web.Mvc.SelectListItem> honorTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> honorLevelList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CheckStatusList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int? honorTypeId { get; set; } = System.Web.HttpContext.Current.Request["honorTypeId"].ConvertToInt();

        public int? honorLevelId { get; set; } = System.Web.HttpContext.Current.Request["honorLevelId"].ConvertToInt();

        public int? CheckStatusId { get; set; } = System.Web.HttpContext.Current.Request["CheckStatusId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}