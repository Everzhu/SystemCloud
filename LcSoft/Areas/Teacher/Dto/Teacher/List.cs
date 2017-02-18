using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Teacher.Dto.Teacher
{
    public class List
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
        /// 学历
        /// </summary>
        [Display(Name = "学历")]
        public string EducationName { get; set; }

        /// <summary>
        /// 教师类型
        /// </summary>
        [Display(Name = "教师类型")]
        public string TeacherTypeName { get; set; }

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

        /// <summary>
        /// 身份证号
        /// </summary>
        [Display(Name = "身份证号")]
        public string IdentityNumber { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [Display(Name = "QQ号码")]
        public string Qq { get; set; }

        /// <summary>
        /// SysUserId
        /// </summary>
        [Display(Name = "SysUserId")]
        public int SysUserId { get; set; }
    }
}
