using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Course.Dto.OrgTeacher
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班"), Required]
        public int OrgId { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Required]
        [Display(Name = "任课教师")]
        public int? TeacherId { get; set; }
    }
}
