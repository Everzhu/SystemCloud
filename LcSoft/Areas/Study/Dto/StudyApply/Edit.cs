using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyApply
{
    public class Edit
    {
        public int Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }
        /// <summary>
        /// 申请备注
        /// </summary>
        [Display(Name = "申请备注"), Required]
        public string Remark { get; set; }
    }
}