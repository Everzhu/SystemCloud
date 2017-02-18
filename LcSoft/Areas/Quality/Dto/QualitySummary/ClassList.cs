using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Dto.QualitySummary
{
    public class ClassList
    {
        public int StudentId { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 自我
        /// </summary>
        [Display(Name = "自我")]
        public string StudentSelf { get; set; }

        /// <summary>
        /// 班主任
        /// </summary>
        [Display(Name = "班主任")]
        public string StudentTeacher{ get; set; }

        /// <summary>
        /// 同学
        /// </summary>
        [Display(Name = "同学")]
        public string StudentQuality { get; set; }
    }
}