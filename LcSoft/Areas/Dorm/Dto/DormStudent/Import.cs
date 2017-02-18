using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Dto.DormStudent
{
    public class Import
    {
        /// <summary>
        /// 住宿名称
        /// </summary>
        [Display(Name = "住宿名称")]
        public string DormName { get; set; }

        /// <summary>
        /// 宿舍
        /// </summary>
        [Display(Name = "宿舍")]
        public string RoomName { get; set; }

        /// <summary>
        /// 宿舍楼
        /// </summary>
        [Display(Name = "宿舍楼")]
        public string BuildName { get; set; }

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
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}