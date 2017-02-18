using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.ClassStudent
{
    public class ChangeClass
    {
        /// <summary>
        /// 学生Id
        /// </summary>
        [Display(Name = "学生")]
        public int StudentId { get; set; }

        /// <summary>
        /// 调前班级ID
        /// </summary>
        [Display(Name = "调前班级")]
        public int FromClassId { get; set; }

        /// <summary>
        /// 调后班级ID
        /// </summary>
        [Display(Name = "调后班级")]
        public int ToClassId { get; set; }

        /// <summary>
        /// 座位号
        /// </summary>
        [Display(Name = "座位号")]
        public int? No { get; set; }
    }
}