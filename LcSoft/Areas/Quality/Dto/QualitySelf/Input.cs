using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Dto.QualitySelf
{
    public class Input
    {
        public int Id { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name ="学年")]
        public string YearName { get; set; }

        /// <summary>
        /// 学期
        /// </summary>
        [Display(Name = "学期")]
        public string YearTearmName { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Display(Name = "提交时间")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string Content { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Display(Name = "类型")]
        public int Type { get; set; }
    }
}