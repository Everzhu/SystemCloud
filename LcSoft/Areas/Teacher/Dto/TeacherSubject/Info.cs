using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Teacher.Dto.TeacherSubject
{
    public class Info
    {
        /// <summary>
        /// 科组
        /// </summary>
        [Display(Name = "科组")]
        public int SubjectId { get; set; }

        public int SubjectName { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public int GradeId { get; set; }
    }
}
