using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Dto.TeacherDept
{
    public class Import
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [Display(Name = "部门名称")]
        public string DeptName { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>
        [Display(Name = "上级部门")]
        public string ParentDeptName { get; set; }

        /// <summary>
        /// 错误提示
        /// </summary>
        [Display(Name = "错误提示")]
        public string Error { get; set; }
    }
}