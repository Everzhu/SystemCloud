using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyClassTeacher
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 班级Id
        /// </summary>
        [Display(Name = "班级Id")]
        public int ClassId { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级")]
        public string ClassName { get; set; }
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