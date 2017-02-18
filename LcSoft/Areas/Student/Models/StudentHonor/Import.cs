using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentHonor
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public List<Dto.StudentHonor.Import> ImportList { get; set; } = new List<Dto.StudentHonor.Import>();

        /// <summary>
        /// 是否对已有数据进行更新操作
        /// </summary>
        [Display(Name = "是否对已有数据进行更新操作")]
        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        /// <summary>
        /// 是否自动新增获奖级别
        /// </summary>
        [Display(Name = "是否自动创建获奖级别")]
        public bool IsAddLevel { get; set; }

        /// <summary>
        /// 是否自动新增荣誉类型
        /// </summary>
        [Display(Name = "是否自动创建荣誉类型")]
        public bool IsAddType { get; set; }


        public bool Status { get; set; }
    }
}