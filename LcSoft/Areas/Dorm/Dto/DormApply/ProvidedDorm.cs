using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Dto.DormApply
{
    public class ProvidedDorm
    {
        public int Id { get; set; }

        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name ="学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 住宿
        /// </summary>
        [Display(Name ="住宿")]
        public int DormId { get; set; }

        /// <summary>
        /// 宿舍
        /// </summary>
        [Display(Name = "宿舍")]
        public int RoomId { get; set; }

        /// <summary>
        /// 宿舍楼
        /// </summary>
        [Display(Name = "宿舍楼")]
        public int BuildId { get; set; }
    }
}