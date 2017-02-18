using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Dto.DormStudent
{
    public class List
    {
        public int Id { get; set; }

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
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string Sex { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年")]
        public string YearName { get; set; }

        /// <summary>
        /// 宿舍名称
        /// </summary>
        [Display(Name = "宿舍名称")]
        public string RoomName { get; set; }

        /// <summary>
        /// 住宿楼
        /// </summary>
        [Display(Name = "住宿楼")]
        public string BuildName { get; set; }
    }
}