using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 教学楼
    /// </summary>
    public class tbBuild : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教学楼
        /// </summary>
        [Required]
        [Display(Name = "教学楼")]
        public string BuildName { get; set; }

        /// <summary>
        /// 所属校区
        /// </summary>
        [Display(Name = "所属校区")]
        public virtual tbSchool tbSchool { get; set; }

        /// <summary>
        /// 建筑物用途
        /// </summary>
        [Display(Name = "建筑物用途")]
        public virtual tbBuildType tbBuildType { get; set; }
    }
}