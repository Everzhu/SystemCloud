using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.OrgManager
{
    public class EditOrg
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

        public int OrgId { get; set; }

        /// <summary>
        /// 老师名称
        /// </summary>
        [Display(Name = "老师名称")]
        public string TeacherName { get; set; }

        public int TeacherId { get; set; }
    }
}