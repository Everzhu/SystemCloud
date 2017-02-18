using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Teacher.Dto.Teacher
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 教职工号
        /// </summary>
        [Display(Name = "教职工号"), Required, RegularExpression(Code.Common.RegUserCode, ErrorMessage = "教职工号：只允许输入中英文字符、数字和_或-，特殊字符都不能包含")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 教师姓名
        /// </summary>
        [Display(Name = "教师姓名"), Required, RegularExpression(Code.Common.RegUserName, ErrorMessage = "教职姓名：只允许输入中英文字符、数字和_或-，特殊字符都不能包含")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 教师类型
        /// </summary>
        [Display(Name = "教师类型")]
        public int? TeacherTypeId { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        [Display(Name = "学历")]
        public int? EducationId { get; set; }

        /// <summary>
        /// 教师部门
        /// </summary>
        [Display(Name = "教师部门")]
        public string TeacherDeptName { get; set; }

        /// <summary>
        /// 教师部门
        /// </summary>
        [Display(Name = "教师部门"), Required]
        public string TeacherDeptId { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [Display(Name = "身份证号"), RegularExpression(Code.Common.RegIdentityNumber, ErrorMessage = "身份证号格式不正确")]
        public string IdentityNumber { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码"), RegularExpression(Code.Common.RegMobil, ErrorMessage = "手机格式不正确")]
        public string Mobile { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "邮箱"), RegularExpression(Code.Common.RegEmail, ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [Display(Name = "QQ号码"), RegularExpression(Code.Common.RegQQNumber, ErrorMessage = "QQ号码格式不正确")]
        public string Qq { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public int? SexId { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        [Display(Name = "政治面貌")]
        public int? DictPartyId { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [Display(Name = "民族")]
        public int? DictNationId { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        [Display(Name = "婚姻状况")]
        public int? DictMarriageId { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        [Display(Name = "籍贯")]
        public int? DictRegionId { get; set; }

        /// <summary>
        /// 健康状况
        /// </summary>
        [Display(Name = "健康状况")]
        public int? DictHealthId { get; set; }

        /// <summary>
        /// 个人简介
        /// </summary>
        [Display(Name = "个人简介")]
        public string Profile { get; set; }
    }
}
