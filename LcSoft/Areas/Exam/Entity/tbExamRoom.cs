using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考场
    /// </summary>
    public class tbExamRoom : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 考场名称
        /// </summary>
        [Required]
        [Display(Name = "考场名称")]
        public string ExamRoomName { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Required]
        [Display(Name = "考试课程")]
        public virtual tbExamCourse tbExamCourse { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Required]
        [Display(Name = "教室")]
        public virtual Basis.Entity.tbRoom tbRoom { get; set; }

        /// <summary>
        /// 每行座位
        /// </summary>
        [Required]
        [Display(Name = "每行座位")]
        public int RowSeat { get; set; }
    }
}