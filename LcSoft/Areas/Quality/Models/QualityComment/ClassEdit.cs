using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityComment
{
    public class ClassEdit
    {
        public Dto.QualityComment.ClassEdit CommentEdit { get; set; } = new Dto.QualityComment.ClassEdit();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public string StudentCode = string.Empty;

        public string StudentName = string.Empty;
    }
}