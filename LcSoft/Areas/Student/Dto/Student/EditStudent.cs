using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.Student
{
    public class EditStudent
    {
        public int Id { get; set; }

        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号"), Required, RegularExpression(Code.Common.RegUserCode, ErrorMessage = "请输入正确的学生学号，不得输入特殊字符")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名"), Required, RegularExpression(Code.Common.RegUserName, ErrorMessage = "请输入正确的学生姓名，不得输入特殊字符")]
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
        [Display(Name = "英文名称"), RegularExpression(Code.Common.RegEnglishName, ErrorMessage = "请输入正确的英文名称，只允许输入字母和数字")]
        public string StudentNameEn { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别"), Required]
        public int? SexId { get; set; }

        /// <summary>
        /// 中考号（新增页面暂缺）
        /// </summary>
        [Display(Name = "中考号")]
        public string TicketNumber { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [Display(Name = "身份证号"), Required, RegularExpression(Code.Common.RegIdentityNumber, ErrorMessage = "身份证号格式不正确")]
        public string IdentityNumber { get; set; }

        /// <summary>
        /// 借阅证号（新增页面暂缺）
        /// </summary>
        [Display(Name = "借阅证号")]
        public string LibraryNo { get; set; }

        /// <summary>
        /// 一卡通（新增页面暂缺）
        /// </summary>
        [Display(Name = "一卡通号")]
        public string CardNo { get; set; }

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

        /// <summary>
        /// 姓名拼音
        /// </summary>
        [Display(Name = "姓名拼音"), RegularExpression(Code.Common.RegEnglishName2, ErrorMessage = "请输入正确的姓名拼音，只允许输入字母和数字")]
        public string PinYin { get; set; }

        /// <summary>
        /// CMIS账号
        /// </summary>
        [Display(Name = "中考号")]
        public string CMIS { get; set; }

        /// <summary>
        /// 入学成绩
        /// </summary>
        [Display(Name = "入学成绩"), RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "请输入正确入学成绩")]
        public decimal? EntranceScore { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        [Display(Name = "入学时间")]
        public DateTime? EntranceDate { get; set; }

        /// <summary>
        /// 毕业院校
        /// </summary>
        [Display(Name = "毕业院校"), Required]
        public string StudentSourceName { get; set; }

        /// <summary>
        /// 教育编号
        /// </summary>
        [Display(Name = "教育编号")]
        public string EduNo { get; set; }
    }
}