using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Dto.AttendanceReport
{
    public class ClassList
    {
        public int Id { get; set; }
        /// <summary>
        /// 考勤日期
        /// </summary>
        [Display(Name = "考勤日期")]
        public DateTime AttendanceDate { get; set; }
        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "考勤类型")]
        public int AttendanceTypeId { get; set; }
        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "考勤类型")]
        public string AttendanceTypeName { get; set; }
        /// <summary>
        /// 考勤分值
        /// </summary>
        [Display(Name = "考勤分值")]
        public decimal AttendanceValue { get; set; }
        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public int GradeId { get; set; }
        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }
        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public int? GradeNo { get; set; }
        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public int ClassId { get; set; }
        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public string ClassName { get; set; }
        /// <summary>
        /// 行政班序号
        /// </summary>
        [Display(Name = "行政班序号")]
        public int? ClassNo { get; set; }
        /// <summary>
        /// 参评班级
        /// </summary>
        [Display(Name = "教学班")]
        public int OrgId { get; set; }
        /// <summary>
        /// 参评班级
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgName { get; set; }        
        /// <summary>
        /// 打分学生
        /// </summary>
        [Display(Name = "打分学生")]
        public int StudentId { get; set; }
        /// <summary>
        /// 打分学生
        /// </summary>
        [Display(Name = "打分学生")]
        public string StudentName { get; set; }
        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "节次")]
        public int PeriodId { get; set; }
        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "节次")]
        public string PeriodName { get; set; }
        /// <summary>
        /// 选择此项数量
        /// </summary>
        [Display(Name = "选择此项数量")]
        public int AttendanceTypeCount { get; set; }
        /// <summary>
        /// 选择此项总分
        /// </summary>
        [Display(Name = "选择此项总分")]
        public decimal AttendanceTypeSum { get; set; }
        /// <summary>
        /// 获取项平均分
        /// </summary>
        [Display(Name = "获取项平均分")]
        public decimal AttendanceTypeAvg { get; set; }
        /// <summary>
        /// 参评总人数
        /// </summary>
        [Display(Name = "参评总人数")]
        public int AttendanceTypeAllCount { get; set; }
        /// <summary>
        /// 项平均得分
        /// </summary>
        [Display(Name = "项平均得分")]
        public decimal AttendanceTypeAllAvg { get; set; }
    }
}