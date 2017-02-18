﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysMessageUser
{
    public class ImportMessage
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public List<Dto.SysMessageUser.ImportMessage> ImportMsgList { get; set; }

        public bool Status { get; set; } = false;
    }
}