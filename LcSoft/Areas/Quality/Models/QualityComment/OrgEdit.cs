using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityComment
{
    public class OrgEdit
    {
        public Dto.QualityComment.OrgEdit RemarkEdit { get; set; } = new Dto.QualityComment.OrgEdit();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public int OrgId { get; set; } = System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();

        public string StudentCode = string.Empty;

        public string StudentName = string.Empty;
    }
}