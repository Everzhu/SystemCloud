using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveReport
{
    public class UnElectiveClassStudentList
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 评价教师
        /// </summary>
        [Display(Name = "评价教师")]
        public string TeacherName { get; set; }

        ///// <summary>
        ///// 对应教学班
        ///// </summary>
        //[Display(Name = "对应教学班")]
        //public string OrgName { get; set; }

        /// <summary>
        /// 对应行政班
        /// </summary>
        [Display(Name = "对应行政班")]
        public int ClassId { get; set; }

        /// <summary>
        /// 对应行政班
        /// </summary>
        [Display(Name = "对应行政班")]
        public string ClassName { get; set; }

        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 学生性别
        /// </summary>
        [Display(Name = "学生性别")]
        public string StudentSex { get; set; }
    }
}