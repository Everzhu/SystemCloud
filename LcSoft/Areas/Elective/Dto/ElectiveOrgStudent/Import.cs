using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveOrgStudent
{
    public class Import
    {

        
        public int ClassId { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称")]
        public string OrgName { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 总名额
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 学生可修改
        /// </summary>
        [Display(Name = "学生可修改")]
        public string IsFixed { get; set; }

        /// <summary>
        /// 默认选中
        /// </summary>
        [Display(Name = "默认选中")]
        public string IsChecked { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}