using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Entity
{
    /// <summary>
    /// 选课模式
    /// </summary>
    public class tbElectiveType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 选课模式
        /// </summary>
        [Required]
        [Display(Name = "选课模式")]
        public string ElectiveTypeName { get; set; }

        /// <summary>
        /// 模式编码
        /// </summary>
        [Required]
        [Display(Name = "模式编码")]
        public Code.EnumHelper.ElectiveType ElectiveTypeCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Required]
        [Display(Name = "状态")]
        public bool IsDisable { get; set; }

        /// <summary>
        /// 模式说明
        /// </summary>
        [Display(Name = "模式说明")]
        public string Remark { get; set; }
    }
}