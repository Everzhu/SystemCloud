using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.OrgManager
{
    public class EditOrgList
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgName { get; set; }
    }
}