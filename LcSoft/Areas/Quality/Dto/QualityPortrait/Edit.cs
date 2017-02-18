using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace XkSystem.Areas.Quality.Dto.QualityPortrait
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 照片标题
        /// </summary>
        [Display(Name = "标题"), Required]
        public string PhotoTitle { get; set; }

        /// <summary>
        /// 照片文件
        /// </summary>
        [Display(Name = "照片"), Required]
        public string PhotoFile { get; set; }

        /// <summary>
        /// 学年Id
        /// </summary>
        [Required]
        public int YearId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述"), Required]
        public string Remark { get; set; }
    }
}