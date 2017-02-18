using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityComment
{
    public class ChildComment
    {
        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public Entity.tbQualitySelf QualitySelf { get; set; } = new Entity.tbQualitySelf();

        public Entity.tbQualityPlan QualityPlan { get; set; } = new Entity.tbQualityPlan();

        public Entity.tbQualitySummary QualitySummary { get; set; } = new Entity.tbQualitySummary();

        public Student.Entity.tbStudent Student { get; set; } = new Student.Entity.tbStudent();
    }
}