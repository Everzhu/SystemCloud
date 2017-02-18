using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralComment
{
    public class SuggestStudentView
    {
        public int Id { get; set; }

        public int SuggestId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Display(Name = "序号")]
        public int? No { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string Comment { get; set; }

        /// <summary>
        /// 回复人
        /// </summary>
        [Display(Name = "回复人")]
        public string UserName { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        [Display(Name = "回复内容")]
        public string ReplyComment { get; set; }

        /// <summary>
        /// 回复时间
        /// </summary>
        [Display(Name = "回复时间")]
        public DateTime ReplyDate { get; set; }
    }
}