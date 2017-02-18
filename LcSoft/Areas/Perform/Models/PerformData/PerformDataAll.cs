using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformData
{
    public class PerformDataAll
    {
        public Dto.PerformData.Info PerformDataInfo { get; set; } = new Dto.PerformData.Info();

        public List<Dto.PerformData.Info> PerformDataInfoList { get; set; } = new List<Dto.PerformData.Info>();

        public List<System.Web.Mvc.SelectListItem> PerformList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> PerformItemList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Areas.Course.Dto.OrgTeacher.List> OrgTeacherList { get; set; } = new List<Areas.Course.Dto.OrgTeacher.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int PerformId { get; set; } = System.Web.HttpContext.Current.Request["PerformId"].ConvertToInt();

        public int SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();
    }
}