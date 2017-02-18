﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Dto.QualityComment
{
    public class FamilyEdit
    {
        public int Id { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Display(Name = "序号")]
        public int? No { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        [Required]
        public string Comment { get; set; }
    }
}