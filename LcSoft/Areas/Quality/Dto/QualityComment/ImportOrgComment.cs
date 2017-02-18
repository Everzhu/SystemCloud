using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Dto.QualityComment
{
    public class ImportOrgComment
    {
        /// <summary>
        /// 教学班Id
        /// </summary>
        [Display(Name = "教学班Id")]
        public int OrgId { get; set; }

        /// <summary>
        /// 学生Id
        /// </summary>
        [Display(Name = "学生Id")]
        public int StudentId { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string Comment { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}