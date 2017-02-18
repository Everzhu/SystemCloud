using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamTeacher
{
    public class Edit
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 考场名称
        /// </summary>
        [Display(Name = "考场名称"), Required]
        public int ExamRoomId { get; set; } = System.Web.HttpContext.Current.Request["ExamRoomId"].ConvertToInt();

        /// <summary>
        /// 教师姓名
        /// </summary>
        [Display(Name = "教师姓名"), Required]
        public int TeacherId { get; set; }

        /// <summary>
        /// 主监考
        /// </summary>
        [Display(Name = "主监考")]
        public bool IsPrimary { get; set; }
    }
}
