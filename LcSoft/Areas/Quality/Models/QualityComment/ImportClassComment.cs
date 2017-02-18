using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityComment
{
    public class ImportClassComment
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public Dto.QualityComment.ImportClassComment ImportComment { get; set; } = new Dto.QualityComment.ImportClassComment();

        public List<Dto.QualityComment.ImportClassComment> ImportClassCommentList { get; set; } = new List<Dto.QualityComment.ImportClassComment>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int? YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public bool Status { get; set; }
    }
}