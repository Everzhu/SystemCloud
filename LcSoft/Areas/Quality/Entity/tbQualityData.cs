using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Entity
{
    public class tbQualityData : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价人
        /// </summary>
        [Display(Name = "评价人")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        [Display(Name = "评价内容")]
        public virtual tbQualityItem tbQualityItem { get; set; }

        /// <summary>
        /// 评价选项
        /// </summary>
        [Display(Name = "评价选项")]
        public virtual tbQualityOption tbQualityOption { get; set; }

        /// <summary>
        /// 被评人
        /// </summary>
        [Display(Name = "被评人")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public virtual Course.Entity.tbOrg tbOrg { get; set; }

        /// <summary>
        /// 回答
        /// </summary>
        [Display(Name = "回答")]
        public string QualityText { get; set; }

        /// <summary>
        /// 评价人对象1、学生，2、班主任、3、任课教师，4、家长
        /// </summary>
        [Display(Name = "评价人对象")]
        public int IsUserType { get; set; }
    }
}