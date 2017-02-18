using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Entity
{
    /// <summary>
    /// 评教数据
    /// </summary>
    public class tbSurveyData : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价教师
        /// </summary>
        [Display(Name = "评价教师"), Required]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 对应班级(教学班)
        /// </summary>
        [Display(Name = "对应班级")]
        public virtual Course.Entity.tbOrg tbOrg { get; set; }

        /// <summary>
        /// 对应班级(行政班)
        /// </summary>
        [Display(Name = "对应班级")]
        public virtual Basis.Entity.tbClass tbClass { get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        [Display(Name = "评价内容"), Required]
        public virtual tbSurveyItem tbSurveyItem { get; set; }

        /// <summary>
        /// 评价项
        /// </summary>
        [Display(Name = "评价项")]
        public virtual tbSurveyOption tbSurveyOption { get; set; }

        /// <summary>
        /// 回答
        /// </summary>
        [Display(Name = "回答")]
        public string SurveyText { get; set; }

        /// <summary>
        /// 评价学生
        /// </summary>
        [Display(Name = "评价学生"), Required]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }
    }
}