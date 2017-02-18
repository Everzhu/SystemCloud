using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.Student
{
    public class InfoList
    {
        public int Id { get; set; }

        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级")]
        public string ClassName { get; set; }

        /// <summary>
        /// 学生类型
        /// </summary>
        [Display(Name = "学生类型")]
        public string StudentTypeName { get; set; }

        /// <summary>
        /// 就读方式
        /// </summary>
        [Display(Name = "就读方式")]
        public string StudentStudyTypeName { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        [Display(Name = "英文名称")]
        public string StudentNameEn { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexName { get; set; }

        /// <summary>
        /// 中考号
        /// </summary>
        [Display(Name = "中考号")]
        public string TicketNumber { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [Display(Name = "身份证号")]
        public string IdentityNumber { get; set; }

        /// <summary>
        /// 借阅证号
        /// </summary>
        [Display(Name = "借阅证号")]
        public string LibraryNo { get; set; }

        /// <summary>
        /// 一卡通
        /// </summary>
        [Display(Name = "一卡通号")]
        public string CardNo { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [Display(Name = "QQ")]
        public string Qq { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Display(Name = "出生日期")]
        public string Birthday { get; set; }

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
        /// 血型
        /// </summary>
        [Display(Name = "血型")]
        public string BloodTypeName { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        [Display(Name = "家庭住址")]
        public string Address { get; set; }

        /// <summary>
        /// 个人简介
        /// </summary>
        [Display(Name = "个人简介")]
        public string Profile { get; set; }

        /// <summary>
        /// 学生照片
        /// </summary>
        [Display(Name = "学生照片")]
        public string Photo { get; set; }

        public string EntranceDate { get; set; }
        public string PrintDate { get; set; }
    }
}