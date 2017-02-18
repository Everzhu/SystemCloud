using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamTeacher
{
    public class List
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 考场名称
        /// </summary>
        [Display(Name = "考场名称")]
        public string ExamRoomName { get; set; }

        /// <summary>
        /// 教职工号
        /// </summary>
        [Display(Name = "教职工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 教师
        /// </summary>
        [Display(Name = "教师姓名")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 主监考
        /// </summary>
        [Display(Name = "主监考")]
        public bool IsPrimary { get; set; }
    }
}
