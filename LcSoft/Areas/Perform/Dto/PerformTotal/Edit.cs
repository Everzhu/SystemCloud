using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Dto.PerformTotal
{
    public class Edit
    {
        public int Id { get; set; }
        /// <summary>
        /// 对应评价
        /// </summary>
        [Display(Name = "对应评价"), Required]
        public int PerformId { get; set; }

        /// <summary>
        /// 评价课程
        /// </summary>
        [Display(Name = "评价课程"), Required]
        public int CourseId { get; set; }

        /// <summary>
        /// 评价学生
        /// </summary>
        [Display(Name = "评价学生"), Required]
        public int StudentId { get; set; }
        /// <summary>
        /// 评价总分
        /// </summary>
        [Display(Name = "评价总分"), Required]
        public decimal TotalScore { get; set; }
    }
}