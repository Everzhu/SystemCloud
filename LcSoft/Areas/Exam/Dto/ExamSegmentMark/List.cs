using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamSegmentMark
{
    public class List
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
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        public string SegmentName { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "科目")]
        public string SubjectName { get; set; }

        /// <summary>
        /// 最小分数
        /// </summary>
        [Display(Name = "最低百分数")]
        public decimal MinMark { get; set; }

        /// <summary>
        /// 最大分数
        /// </summary>
        [Display(Name = "最高百分数")]
        public decimal MaxMark { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }

        [Display(Name = "是否优秀"), Required]
        public bool IsGood { get; set; }

        [Display(Name = "是否及格"), Required]
        public bool IsPass { get; set; }

        [Display(Name = "是否良好"), Required]
        public bool IsNormal { get; set; }

        [Display(Name = "是否总分段"), Required]
        public bool IsTotal { get; set; }

        [Display(Name = "划分方式"), Required]
        public bool IsGenerate { get; set; }

        /// <summary>
        /// 比例
        /// </summary>
        [Display(Name = "比例")]
        public decimal Rate { get; set; }

        /// <summary>
        /// 分数段分组
        /// </summary>
        [Display(Name = "分数段分组")]
        public string ExamSegmentGroupName { get; set; }
    }
}
