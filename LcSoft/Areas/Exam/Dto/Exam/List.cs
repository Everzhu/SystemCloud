using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.Exam
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
        /// 考试名称
        /// </summary>
        [Display(Name = "考试名称")]
        public string ExamName { get; set; }

        /// <summary>
        /// 考试类型
        /// </summary>
        [Display(Name = "考试类型")]
        public string ExamTypeName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public bool IsPublish { get; set; }

        /// <summary>
        /// 学年学段
        /// </summary>
        [Display(Name = "学段")]
        public string YearName { get; set; }

        /// <summary>
        /// 总分等级组
        /// </summary>
        [Display(Name = "总分等级组")]
        public string ExamLevelGroupName { get; set; }

        /// <summary>
        /// 分数段组
        /// </summary>
        [Display(Name = "分数段分组")]
        public string ExamSegmentGroupName { get; set; }
    }
}
