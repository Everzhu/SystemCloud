﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.OrgStudent
{
    public class ExportListByOrg
    {
        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public string ClassName { get; set; }

        /// <summary>
        /// 班主任
        /// </summary>
        [Display(Name = "班主任")]
        public string TeacherName { get; set; }

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
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexName { get; set; }

        /// <summary>
        /// 座位号
        /// </summary>
        [Display(Name = "座位号")]
        public int? No { get; set; }
    }
}