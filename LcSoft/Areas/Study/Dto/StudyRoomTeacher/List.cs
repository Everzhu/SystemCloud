using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyRoomTeacher
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 教室Id
        /// </summary>
        [Display(Name = "教室Id")]
        public int RoomId { get; set; }
        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public string RoomName { get; set; }
        /// <summary>
        /// 主要责任人
        /// </summary>
        [Display(Name = "主要责任人")]
        public bool IsMaster { get; set; }
        /// <summary>
        /// 教师Id
        /// </summary>
        [Display(Name = "教师Id")]
        public int TeacherId { get; set; }

        /// <summary>
        /// 教师工号
        /// </summary>
        [Display(Name = "教师工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 教师姓名
        /// </summary>
        [Display(Name = "教师姓名")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        [Display(Name = "星期")]
        public int WeekId { get; set; }
    }
}