using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.Class
{
    public class ImportTeacher
    {
        /// <summary>
        /// 班主任工号
        /// </summary>
        [Display(Name = "班主任工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 班主任姓名
        /// </summary>
        [Display(Name = "班主任姓名")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}