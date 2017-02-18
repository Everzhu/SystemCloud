using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Student.Dto.Student
{
    public class Info
    {
        public int Id { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生照片
        /// </summary>
        [Display(Name = "学生照片")]
        public string Photo { get; set; }


        /// <summary>
        /// 班级Id
        /// </summary>
        public int ClassId { get; internal set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }
    }
}