using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    public class tbMoralPower : Code.EntityHelper.EntityBase
    {

        /// <summary>
        /// 德育选项
        /// </summary>
        [Required]
        [Display(Name = "德育选项")]
        public virtual tbMoralItem tbMoralItem { get; set; }

        /// <summary>
        /// 评分教师
        /// </summary>
        [Required]
        [Display(Name = "评分教师")]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 评分日期，有值时评对应日期，为空时不限制
        /// </summary>
        [Display(Name = "评分日期")]
        public DateTime? MoralDate { get; set; }
    }
}