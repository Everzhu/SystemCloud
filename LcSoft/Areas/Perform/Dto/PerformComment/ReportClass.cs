using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Perform.Dto.PerformComment
{
    public class ReportClass
    {
        public int Id { get; set; }

        /// <summary>
        /// StudentId
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// CommentId
        /// </summary>
        public int CommentId { get; set; }

        /// <summary>
        /// 录入状态
        /// </summary>
        [Display(Name = "录入状态")]
        public bool Status { get; set; }
    }
}
