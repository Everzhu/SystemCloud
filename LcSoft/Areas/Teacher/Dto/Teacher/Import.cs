using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Dto.Teacher
{
    public class Import
    {
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
        /// 部门
        /// </summary>
        [Display(Name = "部门")]
        public string[] TeacherDeptName { get; set; }

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
        /// 邮箱
        /// </summary>
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [Display(Name = "QQ")]
        public string Qq { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        [Display(Name = "政治面貌")]
        public string DictPartyName { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [Display(Name = "民族")]
        public string DictNationName { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        [Display(Name = "婚姻状况")]
        public string DictMarriageName { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        [Display(Name = "籍贯")]
        public string DictRegionName { get; set; }

        /// <summary>
        /// 健康状况
        /// </summary>
        [Display(Name = "健康状况")]
        public string DictHealthName { get; set; }

        /// <summary>
        /// 个人简介
        /// </summary>
        [Display(Name = "个人简介")]
        public string Profile { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}