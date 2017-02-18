using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamStudent
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
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名"), Required]
        public int StudentId { get; set; }
    }
}
