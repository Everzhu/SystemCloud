using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Student.Dto.Student
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号"), Required]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名"), Required]
        public string StudentName { get; set; }

        /// <summary>
        /// 学生类型
        /// </summary>
        [Display(Name = "学生类型")]
        public int? StudentTypeId { get; set; }

        /// <summary>
        /// 就读方式
        /// </summary>
        [Display(Name = "就读方式")]
        public int? StudentStudyTypeId { get; set; }

        /// <summary>
        /// 学届
        /// </summary>
        [Display(Name = "学届")]
        public int? StudentSessionId { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        [Display(Name = "英文名称")]
        public string StudentNameEn { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public int? SexId { get; set; }

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
        [RegularExpression(@"\d{11}", ErrorMessage = "手机格式不正确")]
        public string Mobile { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "Email")]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$", ErrorMessage = "邮箱格式不正确")]
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
        public DateTime? Birthday { get; set; }

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
        /// 血型
        /// </summary>
        [Display(Name = "血型")]
        public int? BloodTypeId { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        [Display(Name = "家庭住址")]
        public string Address { get; set; }

        /// <summary>
        /// 简介特长
        /// </summary>
        [Display(Name = "简介特长")]
        public string Profile { get; set; }

        /// <summary>
        /// 学生照片
        /// </summary>
        [Display(Name = "学生照片")]
        public string Photo { get; set; }

        /// <summary>
        /// 姓名拼音
        /// </summary>
        [Display(Name = "姓名拼音")]
        public string PinYin { get; set; }

        /// <summary>
        /// CMIS账号
        /// </summary>
        [Display(Name = "中考号")]
        public string CMIS { get; set; }

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
        /// 邮政编码
        /// </summary>
        [Display(Name = "邮政编码"), RegularExpression(Code.Common.RegPostalCode, ErrorMessage = "邮政编码格式不正确")]
        public string PostCode { get; set; }

        /// <summary>
        /// 毕业院校
        /// </summary>
        [Display(Name = "毕业院校")]
        public string StudentSourceName { get; set; }
    }
}
