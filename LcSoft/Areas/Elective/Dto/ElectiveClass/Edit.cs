using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.ElectiveClass
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 参选班级 
        /// </summary>
        [Display(Name = "参选班级"), Required]
        public int ClassId { get; set; }

        /// <summary>
        /// 所属选课
        /// </summary>
        [Display(Name = "所属选课"), Required]
        public int ElectiveId { get; set; }
    }
}
