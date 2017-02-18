using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.Class
{
    public class ImportClassAndStudent
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

        ///// <summary>
        ///// 座位号
        ///// </summary>
        //[Display(Name = "座位号")]
        //public string SeatNo { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public string No { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称")]
        public string ClassName { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }

        /// <summary>
        /// 班级类型
        /// </summary>
        [Display(Name = "班级类型")]
        public string ClassTypeName { get; set; }

        /// <summary>
        /// 班主任
        /// </summary>
        [Display(Name = "班主任")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 班级教室
        /// </summary>
        [Display(Name = "班级教室")]
        public string RoomName { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}