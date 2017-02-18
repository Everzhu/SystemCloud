using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.ClassManager
{
    public class EditClassList
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称")]
        public string ClassName { get; set; }

        public int ClassId { get; set; }

        /// <summary>
        /// 老师名称
        /// </summary>
        [Display(Name = "老师名称")]
        public string TeacherName { get; set; }

        public int TeacherId { get; set; }
    }
}