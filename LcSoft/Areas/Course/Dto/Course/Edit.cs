using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Course.Dto.Course
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 课程 
        /// </summary>
        [Display(Name = "课程名称"), Required]
        public string CourseName { get; set; }

        /// <summary>
        /// 课程编码 
        /// </summary>
        [Display(Name = "课程编码"), RegularExpression(Code.Common.RegUserName, ErrorMessage = "课程名称只允许输入中英文字符、数字和_或-，特殊字符都不能包含")]
        public string CourseCode { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        [Display(Name = "英文名"), RegularExpression(Code.Common.RegEnglishName, ErrorMessage = "英文名只允许输入英文和数字")]
        public string CourseNameEn { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "科目"), Required]
        public int SubjectId { get; set; }

        /// <summary>
        /// 课程类型
        /// </summary>
        [Display(Name = "课程类型"), Required]
        public int CourseTypeId { get; set; }

        /// <summary>
        /// 学分
        /// </summary>
        [Display(Name = "学分"), Required, RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "学分请输入非负数")]
        public decimal Point { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        [Display(Name = "课时"), Required, RegularExpression(Code.Common.RegPositiveIntZero, ErrorMessage = "课时请输入正整数")]
        public int Hour { get; set; }

        /// <summary>
        /// 课程说明
        /// </summary>
        [Display(Name = "课程说明")]
        public string Remark { get; set; }

        /// <summary>
        /// 课程领域
        /// </summary>
        [Display(Name = "课程领域")]
        public int? CourseDomainId { get; set; }

        /// <summary>
        /// 课程分组
        /// </summary>
        [Display(Name = "课程分组")]
        public int? CourseGroupId { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        [Display(Name = "是否等级录入")]
        public bool IsLevel { get; set; }
    }
}
