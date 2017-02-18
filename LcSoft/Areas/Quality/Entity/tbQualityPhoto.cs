using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Entity
{
    /// <summary>
    /// 相片
    /// </summary>
    public class tbQualityPhoto : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 照片标题
        /// </summary>
        [Required]
        [Display(Name = "照片标题")]
        public string PhotoTitle { get; set; }

        /// <summary>
        /// 所属学生
        /// </summary>
        [Required]
        [Display(Name = "所属学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

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
    }
}