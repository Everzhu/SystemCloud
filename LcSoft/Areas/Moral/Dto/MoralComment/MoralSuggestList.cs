using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralComment
{
    public class MoralSuggestList
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name ="内容")]
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
        /// 评价状态
        /// </summary>
        [Display(Name = "评价状态")]
        public int CommentType { get; set; }
    }
}