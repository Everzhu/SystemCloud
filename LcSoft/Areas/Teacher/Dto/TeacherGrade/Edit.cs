using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Teacher.Dto.TeacherGrade
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public int GradeId { get; set; }

        /// <summary>
        /// 年级组长
        /// </summary>
        [Display(Name = "年级组长")]
        public int TeacherId { get; set; }
    }
}
