using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.OrgManager
{
    public class TeacherList
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 老师名称
        /// </summary>
        [Display(Name = "老师名称")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 老师ID
        /// </summary>
        [Display(Name = "老师ID")]
        public int TeacherId { get; set; }
        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgNames { get; set; }
    }
}