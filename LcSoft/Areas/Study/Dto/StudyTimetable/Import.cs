using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyTimetable
{
    public class Import
    {
        public int Id { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级")]
        public string ClassName { get; set; }
        /// <summary>
        /// 教师
        /// </summary>
        [Display(Name = "教师")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 职工号
        /// </summary>
        [Display(Name = "职工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}