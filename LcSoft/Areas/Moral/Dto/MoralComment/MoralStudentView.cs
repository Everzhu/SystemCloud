using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralComment
{
    public class MoralStudentView
    {
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
    }
}