using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.Exam
{
    /// <summary>
    /// 编辑考试
    /// </summary>
    public class Edit
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }
        /// <summary>
        /// 考试名称
        /// </summary>
        [Display(Name = "考试名称"), Required]
        public string ExamName { get; set; }
        /// <summary>
        /// 考试类型
        /// </summary>
        [Display(Name = "考试类型"), Required]
        public int ExamTypeId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态"), Required]
        public bool IsPublish { get; set; }
        /// <summary>
        /// 学年学段
        /// </summary>
        [Display(Name = "学段"), Required]
        public int YearId { get; set; }
        /// <summary>
        /// 总分等级组
        /// </summary>
        [Display(Name = "总分等级组")]
        public int? LevelGroupId { get; set; }
        /// <summary>
        /// 分数段分组
        /// </summary>
        [Display(Name = "分数段分组")]
        public int? SegmentGroupId { get; set; }
    }
}
