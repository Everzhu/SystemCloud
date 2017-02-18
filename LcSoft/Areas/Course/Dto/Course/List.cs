using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Course.Dto.Course
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程名称")]
        public string CourseName { get; set; }

        /// <summary>
        /// 课程编码 
        /// </summary>
        [Display(Name = "课程编码")]
        public string CourseCode { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        [Display(Name = "英文名")]
        public string CourseNameEn { get; set; }
       
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
        /// 等级
        /// </summary>
        [Display(Name = "是否等级录入")]
        public bool IsLevel { get; set; }
    }
}
