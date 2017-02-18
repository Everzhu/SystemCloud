using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Dto.PerformData
{
    public class Info
    {
        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgName { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        [Display(Name = "项目")]
        public int PerformItemId { get; set; }

        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public int OrgId { get; set; }

        /// <summary>
        /// 教师
        /// </summary>
        [Display(Name = "教师")]
        public string OrgTeacherName { get; set; }

        /// <summary>
        /// 比率
        /// </summary>
        [Display(Name = "比率")]
        public string ScoreRate { get; set; }
    }
}