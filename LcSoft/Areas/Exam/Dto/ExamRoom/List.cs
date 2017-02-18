using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamRoom
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
        /// 考试课程
        /// </summary>
        [Display(Name = "考试课程")]
        public string ExamCourseName { get; set; }

        /// <summary>
        /// 考试教室
        /// </summary>
        [Display(Name = "考试教室")]
        public string RoomName { get; set; }

        /// <summary>
        /// 每行座位
        /// </summary>
        [Display(Name = "每行座位")]
        public int RowSeat { get; set; }

        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public string TeacherCode { get; set; }
        public string TeacherName { get; set; }
    }
}
