using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyData
{
    public class Edit
    {
        public int Id { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 表现
        /// </summary>
        [Display(Name = "表现"), Required]
        public int StudyOptionId { get; set; }
    }
}