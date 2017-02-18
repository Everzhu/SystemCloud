using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamImportSegmentMark
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
    }
}
