using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static XkSystem.Code.EntityHelper;

namespace XkSystem.Areas.Moral.Entity
{
    /// <summary>
    /// 德育每月之星
    /// </summary>
    public class tbMoralStar:EntityBase
    {

        /// <summary>
        /// 学生
        /// </summary>
        [Display(Name = "学生"),Required]
        public Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 所属德育
        /// </summary>
        [Display(Name = "德育"),Required]
        public tbMoral tbMoral { get; set; }


        /// <summary>
        /// 所属月份
        /// </summary>
        [Display(Name = "所属月份"),Required]
        public DateTime Date { get; set; }

        [Display(Name = "是否禁用"),Required]
        public bool IsDisabled { get; set; }

        [Display(Name ="提交日期"),Required]
        public DateTime InputDate { get; set; }

        [Display(Name ="提交人"),Required]
        public Sys.Entity.tbSysUser tbSysUser { get; set; }

    }
}