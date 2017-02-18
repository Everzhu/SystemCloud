using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Course.Dto.OrgStudent
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 座位号
        /// </summary>
        [Display(Name = "座位号"), Required]
        public int? No { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        [Display(Name = "学生"), Required]
        public string StudentCode { get; set; }

        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班"), Required]
        public int OrgId { get; set; }
    }
}
