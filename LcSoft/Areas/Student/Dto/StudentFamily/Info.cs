using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentFamily
{
    public class Info
    {
        public int? Id { get; set; }

        /// <summary>
        /// 家长姓名
        /// </summary>
        [Display(Name = "成员姓名"), Required]
        public string FamilyName { get; set; }

        /// <summary>
        /// 家庭关系
        /// </summary>
        [Display(Name = "家庭关系"), Required]
        public string Relation { get; set; }

        /// <summary>
        /// 家庭关系
        /// </summary>
        [Display(Name = "家庭关系"), Required]
        public int? KinshipId { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [Display(Name = "单位名称")]
        public string UnitName { get; set; }

        /// <summary>
        /// 岗位职务
        /// </summary>
        [Display(Name = "岗位职务")]
        public string Job { get; set; }

        /// <summary>
        /// 联系手机
        /// </summary>
        [Display(Name = "联系手机")]
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Display(Name = "邮箱")]
        public string Email { get; set; }
    }
}