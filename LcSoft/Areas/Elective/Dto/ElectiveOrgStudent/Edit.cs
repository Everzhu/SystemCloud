using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.ElectiveOrgStudent
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 所属选课开班
        /// </summary>
        [Display(Name = "所属选课开班"), Required]
        public int ElectiveOrgId { get; set; }

        /// <summary>
        /// 对应班级
        /// </summary>
        [Display(Name = "对应班级"), Required]
        public int ClassId { get; set; }
    }
}
