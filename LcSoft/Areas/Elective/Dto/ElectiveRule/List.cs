using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.ElectiveRule
{
    public class List
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }


        public int CourseId { get; set; }

        /// <summary>
        /// 课程名
        /// </summary>
        [Display(Name = "课程名")]
        public string CourseName { get; set; }

        /// <summary>
        /// 规则目标
        /// </summary>
        [Display(Name = "规则目标")]
        public string CourseTarget { get; set; }

        /// <summary>
        /// 课程关系
        /// </summary>
        [Display(Name = "课程关系")]
        public Code.EnumHelper.ElectiveRule ElectiveRule { get; set; }
    }
}
