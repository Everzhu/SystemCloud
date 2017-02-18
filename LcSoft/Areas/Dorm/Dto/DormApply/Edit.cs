using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Dto.DormApply
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 住宿
        /// </summary>
        [Display(Name = "住宿")]
        public int DormId { get; set; }

        ///// <summary>
        ///// 宿舍
        ///// </summary>
        //[Display(Name = "宿舍")]
        //public int RoomId { get; set; }

        ///// <summary>
        ///// 宿舍楼
        ///// </summary>
        //[Display(Name = "宿舍楼")]
        //public int BuildId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}