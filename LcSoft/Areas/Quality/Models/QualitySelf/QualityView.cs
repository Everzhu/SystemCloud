using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualitySelf
{
    public class QualityView
    {
        public List<Dto.QualitySelf.QualityView> QualitySelfList { get; set; } = new List<Dto.QualitySelf.QualityView>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public string ClassName=string.Empty;

        public string StudentCode=string.Empty;

        public string StudentName=string.Empty;

        public bool YearDefault = false;
    }
}