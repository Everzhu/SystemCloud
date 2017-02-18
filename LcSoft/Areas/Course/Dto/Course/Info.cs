using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Course.Dto.Course
{
    public class Info
    {
        public int Id { get; set; }

        /// <summary>
        /// 课程编码 
        /// </summary>
        [Display(Name = "课程编码")]
        public string CourseCode { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程名称")]
        public string CourseName { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        [Display(Name = "英文名")]
        public string CourseNameEn { get; set; }

        /// <summary>
        /// 课程领域
        /// </summary>
        [Display(Name = "课程领域")]
        public string CourseDomainName { get; set; }

        /// <summary>
        /// 课程分组
        /// </summary>
        [Display(Name = "课程分组")]
        public string CourseGroupName { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "科目")]
        public string SubjectName { get; set; }

        /// <summary>
        /// 课程类型
        /// </summary>
        [Display(Name = "课程类型")]
        public string CourseTypeName { get; set; }

        /// <summary>
        /// 学分
        /// </summary>
        [Display(Name = "学分")]
        public decimal Point { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        [Display(Name = "课时")]
        public int Hour { get; set; }

        /// <summary>
        /// 课程说明
        /// </summary>
        [Display(Name = "课程说明")]
        public string Remark { get; set; }
    }
}
