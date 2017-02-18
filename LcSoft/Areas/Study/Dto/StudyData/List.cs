using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyData
{
    public class List
    {
        public int Id { get; set; }
        /// <summary>
        /// 晚自习Id
        /// </summary>
        [Display(Name = "晚自习Id")]
        public int StudyId { get; set; }
        /// <summary>
        /// 晚自习Id
        /// </summary>
        [Display(Name = "晚自习")]
        public string StudyName { get; set; }
        /// <summary>
        /// 班级Id
        /// </summary>
        [Display(Name = "班级Id")]
        public int? ClassId { get; set; }
        /// <summary>
        /// 教室Id
        /// </summary>
        [Display(Name = "教室Id")]
        public int? RoomId { get; set; }
        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public string ClassName { get; set; }
        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public string RoomName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }
        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }
        /// <summary>
        /// 学生Id
        /// </summary>
        [Display(Name = "学生Id")]
        public int StudentId { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexName { get; set; }
        /// <summary>
        /// 自习日期
        /// </summary>
        [Display(Name = "自习日期")]
        public DateTime InputDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 表现名称
        /// </summary>
        [Display(Name = "表现名称")]
        public string StudyOptionName { get; set; }
        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "录入人员")]
        public string SysUserName { get; set; }
    }
}