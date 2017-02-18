using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Perform.Dto.PerformData
{
    public class Edit
    {
        public int Id { get; set; }

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
        /// 评价项目
        /// </summary>
        [Display(Name = "评价项目"), Required]
        public int PerformItemId { get; set; }

        /// <summary>
        /// 评价分数
        /// </summary>
        [Display(Name = "评价分数"), Required]
        public decimal Score { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        [Display(Name = "录入时间"), Required]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "录入人员"), Required]
        public int SysUserId { get; set; }
    }
}
