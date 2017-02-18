using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamCourse
{
    public class Edit
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 所属考试
        /// </summary>
        [Display(Name = "所属考试"), Required]
        public int ExamId { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Display(Name = "课程"), Required]
        public int CourseId { get; set; }

        /// <summary>
        /// 过程分折算
        /// </summary>
        [Display(Name = "过程分折算比例"), Required, RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "请输入正确的过程分折算比例，必须大于或等于零")]
        public decimal AppraiseRate { get; set; } = 100;

        /// <summary>
        /// 过程分满分值
        /// </summary>
        [Display(Name = "过程分满分"), RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "请输入正确的过程分满分，必须大于或等于零")]
        public decimal FullAppraiseMark { get; set; } = 100;

        /// <summary>
        /// 考试成绩满分值
        /// </summary>
        [Display(Name = "考试成绩折算比例"), RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "请输入正确的考试成绩折算比例，必须大于或等于零")]
        public decimal TotalRate { get; set; } = 100;

        /// <summary>
        /// 考试成绩满分值
        /// </summary>
        [Display(Name = "考试成绩满分"), RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "请输入正确的考试成绩满分，必须大于或等于零")]
        public decimal FullTotalMark { get; set; } = 100;

        /// <summary>
        /// 综合成绩满分值
        /// </summary>
        [Display(Name = "综合成绩满分"), RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "请输入正确的综合成绩满分，必须大于或等于零")]
        public decimal FullSegmentMark { get; set; } = 100;

        /// <summary>
        /// 考试等级组
        /// </summary>
        [Display(Name = "考试等级组"), Required]
        public int ExamLevelGroupId { get; set; }

        /// <summary>
        /// 学习时间
        /// </summary>
        [Display(Name = "学习时间")]
        public string StudyTime { get; set; }

        /// <summary>
        /// 录入状态
        /// </summary>
        [Display(Name = "录入状态"), Required]
        public bool IsInputState { get; set; }

        /// <summary>
        /// 录入模式
        /// </summary>
        [Display(Name = "录入模式"), Required]
        public bool IsInputType { get; set; }

        /// <summary>
        /// 学习时间
        /// </summary>
        [Display(Name = "学习时间"), Required]
        public int? ExamSectionId { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "指定人员")]
        public int? TeacherId { get; set; }

        /// <summary>
        /// 模块认定
        /// </summary>
        [Display(Name = "模块认定")]
        public bool Identified { get; set; }
    }
}
