using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Entity
{
    /// <summary>
    /// 科目
    /// </summary>
    public class tbSubject : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 科目
        /// </summary>
        [Required]
        [Display(Name = "科目")]
        public string SubjectName { get; set; }

        /// <summary>
        /// 科目英文
        /// </summary>
        [Display(Name = "科目英文")]
        public string SubjectNameEn { get; set; }

        /// <summary>
        ///  必修学分
        /// </summary>
        [Display(Name = "必修学分")]
        public int RequirePoint { get; set; }

        /// <summary>
        /// 选修学分
        /// </summary>
        [Display(Name = "选修学分")]
        public int ElectivePoint { get; set; }
    }
}