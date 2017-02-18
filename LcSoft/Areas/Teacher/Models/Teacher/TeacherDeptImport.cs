﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.Teacher
{
    public class TeacherDeptImport
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public List<Dto.Teacher.TeacherDeptImport> ImportList { get; set; } = new List<Dto.Teacher.TeacherDeptImport>();

        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        //[Display(Name = "对于系统中已存在的数据做更新操作")]
        //public bool IsAddTeacherDept { get; set; }

        public bool IsRemove { get; set; }

        public bool Status { get; set; }
    }
}