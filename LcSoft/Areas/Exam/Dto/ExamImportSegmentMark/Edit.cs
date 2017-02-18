using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamImportSegmentMark
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称"), Required]
        public string SegmentName { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级"), Required]
        public int GradeId { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        [Display(Name = "最低百分数"), Required, RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "请输入正确的分数，必须大于或等于零")]
        public decimal MinMark { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        [Display(Name = "最高百分数"), Required, RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "请输入正确的分数，必须大于或等于零")]
        public decimal MaxMark { get; set; }
    }
}
