using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveOrg
{
    public class TeacherList
    {
        public int Id { get; set; }

        /// <summary>
        /// 教职工号
        /// </summary>
        [Display(Name = "教职工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 教师姓名
        /// </summary>
        [Display(Name = "教师姓名")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 对应帐号
        /// </summary>
        [Display(Name = "对应帐号")]
        public string SysUserName { get; set; }

        /// <summary>
        /// 教师部门
        /// </summary>
        [Display(Name = "教师部门")]
        public string TeacherDeptName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexName { get; set; }
    }
}