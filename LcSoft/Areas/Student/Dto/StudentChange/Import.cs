﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentChange
{
    public class Import
    {
        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 学生调动类型
        /// </summary>
        [Display(Name = "学生调动类型")]
        public string ChangeTypeName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}