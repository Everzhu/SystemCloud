using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Dto.DormTeacher
{
    public class Import
    {
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public string No { get; set; }

        /// <summary>
        /// 宿舍名称
        /// </summary>
        [Display(Name = "宿舍名称")]
        public string RoomName { get; set; }

        /// <summary>
        /// 宿舍楼
        /// </summary>
        [Display(Name = "宿舍楼")]
        public string BuildName { get; set; }

        /// <summary>
        /// 容纳人数
        /// </summary>
        [Display(Name = "容纳人数")]
        public string MaxCount { get; set; }

        /// <summary>
        /// 宿管教职工号
        /// </summary>
        [Display(Name = "宿管教职工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 宿管姓名
        /// </summary>
        [Display(Name = "宿管姓名")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}