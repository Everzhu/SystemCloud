using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Teacher.Dto.TeacherSubject
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// SubjectId
        /// </summary>
        public int SubjectId { get; set; }

        /// <summary>
        /// 科组
        /// </summary>
        [Display(Name = "科组")]
        public string SubjectName { get; set; }

        /// <summary>
        /// TeacherId
        /// </summary>
        public int TeacherId { get; set; }

        /// <summary>
        /// 教职工号
        /// </summary>
        [Display(Name = "教职工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 科组长
        /// </summary>
        [Display(Name = "科组长")]
        public string TeacherName { get; set; }

        /// <summary>
        /// GradeId
        /// </summary>
        public int GradeId { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }
    }
}
