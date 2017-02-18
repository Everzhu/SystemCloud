using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamPower
{
    public class List
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 所属考试
        /// </summary>
        [Display(Name = "所属考试")]
        public string ExamName { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "录入人员")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 教职工号
        /// </summary>
        [Display(Name = "教职工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 录入课程
        /// </summary>
        [Display(Name = "录入课程")]
        public string CourseName { get; set; }

        [Display(Name = "科目")]
        public string SubjectName { get; set; }

        [Display(Name = "是否任课教师")]
        public bool IsOrgTeacher { get; set; }
        
        /// <summary>
        /// 录入状态
        /// </summary>
        [Display(Name = "录入时间")]
        public string InputDate { get; set; }
    }
}
