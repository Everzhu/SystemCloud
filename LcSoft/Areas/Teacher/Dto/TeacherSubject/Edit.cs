using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Teacher.Dto.TeacherSubject
{
    public class Edit
    {
        /// <summary>
        /// 科组
        /// </summary>
        [Display(Name = "科组")]
        public int SubjectId { get; set; }

        /// <summary>
        /// 科组长
        /// </summary>
        [Display(Name = "科组长"), Required]
        public int TeacherId { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public int GradeId { get; set; }
    }
}
