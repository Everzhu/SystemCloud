using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.Course
{
    public class SimpleInfo
    {
        public int Id { get; set; }

        /// <summary>
        /// 课程编码 
        /// </summary>
        [Display(Name = "课程编码")]
        public string CourseCode { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程名称")]
        public string CourseName { get; set; }

    }
}