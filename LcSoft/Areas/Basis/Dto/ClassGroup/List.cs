using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.ClassGroup
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 小组名称
        /// </summary>
        [Display(Name = "小组名称")]
        public string ClassGroupName { get; set; }

        /// <summary>
        /// 老师姓名
        /// </summary>
        [Display(Name = "老师姓名")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 老师工号
        /// </summary>
        [Display(Name = "老师工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        [Display(Name = "人数")]
        public int StudentCount { get; set; }

        ///// <summary>
        ///// 班级名称
        ///// </summary>
        //[Display(Name = "班级名称")]
        //public string ClassName { get; set; }
    }
}