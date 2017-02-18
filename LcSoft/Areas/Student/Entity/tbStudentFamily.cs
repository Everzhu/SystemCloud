using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Entity
{
    /// <summary>
    /// 学生家庭成员
    /// </summary>
    public class tbStudentFamily : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 家长姓名
        /// </summary>
        [Display(Name = "家长姓名"), Required]
        public string FamilyName { get; set; }

        /// <summary>
        /// 所属学生
        /// </summary>
        [Display(Name = "所属学生"), Required]
        public virtual tbStudent tbStudent { get; set; }

        /// <summary>
        /// 家庭关系
        /// </summary>
        [Display(Name = "家庭关系")]
        public virtual Dict.Entity.tbDictKinship tbDictKinship { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        [Display(Name = "学历")]
        public virtual Dict.Entity.tbDictEducation tbDictEducation { get; set; }

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

        /// <summary>
        /// 身份证号
        /// </summary>
        [Display(Name = "身份证号")]
        public string IdentityNumber { get; set; }
    }
}