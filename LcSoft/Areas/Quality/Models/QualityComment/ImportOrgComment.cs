using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityComment
{
    public class ImportOrgComment
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public Dto.QualityComment.ImportClassComment ImportComment { get; set; } = new Dto.QualityComment.ImportClassComment();

        public List<Dto.QualityComment.ImportOrgComment> ImportOrgCommentList { get; set; } = new List<Dto.QualityComment.ImportOrgComment>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int? YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? OrgId { get; set; } = System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();

        public bool Status { get; set; }
    }
}