using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.Student
{
    public class Import
    {
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
        /// 姓名拼音
        /// </summary>
        [Display(Name = "姓名拼音")]
        public string PinYin { get; set; }
        /// <summary>
        /// 英文名
        /// </summary>
        [Display(Name = "英文名")]
        public string StudentNameEn { get; set; }

        /// <summary>
        /// CMIS账号
        /// </summary>
        [Display(Name = "中考号")]
        public string CMIS { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexName { get; set; }

        /// <summary>
        /// 学届
        /// </summary>
        [Display(Name = "学届")]
        public string StudentSessionName { get; set; }

        /// <summary>
        /// 学生类型
        /// </summary>
        [Display(Name = "学生类型")]
        public string StudentTypeName { get; set; }

        /// <summary>
        /// 就读方式
        /// </summary>
        [Display(Name = "就读方式")]
        public string StudyTypeName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        [Display(Name = "家庭住址")]
        public string Address { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [Display(Name = "身份证号")]
        public string IdentityNumber { get; set; }

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
        /// 血型
        /// </summary>
        [Display(Name = "血型")]
        public string BloodName { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [Display(Name = "民族")]
        public string NationName { get; set; }

        /// <summary>
        /// 党派(政治面貌)
        /// </summary>
        [Display(Name = "党派")]
        public string PartyName { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Display(Name = "出生日期")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 父亲姓名
        /// </summary>
        [Display(Name = "父亲姓名")]
        public string FatherName { get; set; }

        /// <summary>
        /// 父亲单位
        /// </summary>
        [Display(Name = "父亲单位")]
        public string FatherCompany { get; set; }

        /// <summary>
        /// 父亲职务
        /// </summary>
        [Display(Name = "父亲职务")]
        public string FatherJob { get; set; }

        /// <summary>
        /// 父亲电话号码
        /// </summary>
        [Display(Name = "父亲电话号码")]
        public string FatherPhone { get; set; }

        /// <summary>
        /// 父亲邮箱
        /// </summary>
        [Display(Name = "父亲邮箱")]
        public string FatherEmail { get; set; }

        /// <summary>
        /// 母亲姓名
        /// </summary>
        [Display(Name = "母亲姓名")]
        public string MotherName { get; set; }

        /// <summary>
        /// 母亲单位
        /// </summary>
        [Display(Name = "母亲单位")]
        public string MotherCompany { get; set; }

        /// <summary>
        /// 母亲职务
        /// </summary>
        [Display(Name = "母亲职务")]
        public string MotherJob { get; set; }

        /// <summary>
        /// 母亲电话号码
        /// </summary>
        [Display(Name = "母亲电话号码")]
        public string MotherPhone { get; set; }

        /// <summary>
        /// 母亲邮箱
        /// </summary>
        [Display(Name = "母亲邮箱")]
        public string MotherEmail { get; set; }

        /// <summary>
        /// 入学成绩
        /// </summary>
        [Display(Name = "入学成绩")]
        public decimal? EntranceScore { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        [Display(Name = "入学时间")]
        public DateTime? EntranceDate { get; set; }

        /// <summary>
        /// 毕业院校
        /// </summary>
        [Display(Name = "毕业院校")]
        public string StudentSourceName { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}