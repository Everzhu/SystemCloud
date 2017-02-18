using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Entity
{
    /// <summary>
    /// 选课数据
    /// </summary>
    public class tbElectiveData : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 选课学生
        /// </summary>
        [Required]
        [Display(Name = "选课学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 选课班级
        /// </summary>
        [Display(Name = "选课班级")]
        public virtual tbElectiveOrg tbElectiveOrg { get; set; }

        /// <summary>
        /// 选课对应的星期节次
        /// </summary>
        //public virtual tbElectiveOrgSchedule tbElectiveOrgSchedule { get; set; }

        /// <summary>
        /// 是否预选
        /// </summary>
        [Display(Name = "是否预选")]
        public bool IsPreElective { get; set; }

        /// <summary>
        /// 是否不可调整
        /// </summary>
        [Display(Name = "是否不可调整")]
        public bool IsFixed { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Required]
        [Display(Name = "提交时间")]
        public DateTime InputDate { get; set; }
    }
}