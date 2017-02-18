using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Basis.Dto.ClassTeacher
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班"), Required]
        public int ClassId { get; set; }

        /// <summary>
        /// 班主任
        /// </summary>
        [Display(Name = "班主任"), Required]
        public int TeacherId { get; set; }
    }
}
