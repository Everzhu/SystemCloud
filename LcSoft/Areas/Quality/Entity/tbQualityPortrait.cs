using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Entity
{
    /// <summary>
    /// 自画像
    /// </summary>
    public class tbQualityPortrait : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学年
        /// </summary>
        [Required]
        [Display(Name = "学年")]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        [Required]
        [Display(Name = "学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 照片标题
        /// </summary>
        [Required]
        [Display(Name = "照片标题")]
        public string PhotoTitle { get; set; }

        /// <summary>
        /// 照片文件
        /// </summary>
        [Required]
        [Display(Name = "照片文件")]
        public string PhotoFile { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        [Required]
        [Display(Name = "上传时间")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        public string Remark { get; set; }
    }
}