using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentChange
{
    public class StudentOut
    {
        /// <summary>
        /// Id
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号"), Required]
        public string StudentCode { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 异动状态
        /// </summary>
        [Display(Name = "异动状态"), Required]
        public int StudentChangeTypeId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}