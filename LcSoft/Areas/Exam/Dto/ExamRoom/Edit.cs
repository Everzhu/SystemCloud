using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamRoom
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
        public string ExamRoomName { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Display(Name = "考试课程"), Required]
        public int ExamCourseId { get; set; } = System.Web.HttpContext.Current.Request["ExamCourseId"].ConvertToInt();

        /// <summary>
        /// 考试教室
        /// </summary>
        [Display(Name = "考试教室"), Required]
        public int RoomId { get; set; }

        /// <summary>
        /// 每行座位
        /// </summary>
        [Display(Name = "每行座位"), Required]
        public int RowSeat { get; set; } = 5;
    }
}
