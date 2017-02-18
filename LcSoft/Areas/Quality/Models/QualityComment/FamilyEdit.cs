using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityComment
{
    public class FamilyEdit
    {
        public Dto.QualityComment.FamilyEdit QualityFamilyEdit { get; set; } = new Dto.QualityComment.FamilyEdit();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public string StudentCode = string.Empty;

        public string StudentName = string.Empty;
    }
}