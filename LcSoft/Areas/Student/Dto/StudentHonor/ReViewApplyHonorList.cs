using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentHonor
{
    public class ReViewApplyHonorList
    {
        public int Id { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string Sex { get; set; }

        /// <summary>
        /// 荣誉名称
        /// </summary>
        [Display(Name = "荣誉名称")]
        public string HonorName { get; set; }

        /// <summary>
        /// 获奖级别
        /// </summary>
        [Display(Name = "获奖级别")]
        public string StudentHonorLevelName { get; set; }

        /// <summary>
        /// 荣誉类型
        /// </summary>
        [Display(Name = "荣誉类型")]
        public string StudentHonorTypeName { get; set; }

        /// <summary>
        /// 荣誉证书
        /// </summary>
        [Display(Name = "荣誉证书")]
        public string HonorFile { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        [Display(Name = "申请时间")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        [Display(Name = "审批意见")]
        public string CheckRemark { get; set; }
    }
}