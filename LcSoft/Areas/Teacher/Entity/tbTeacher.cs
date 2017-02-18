using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Entity
{
    /// <summary>
    /// 教师信息
    /// </summary>
    public class tbTeacher : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教职工号
        /// </summary>
        [Display(Name = "教职工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 教师姓名
        /// </summary>
        [Display(Name = "教师姓名"), Required]
        public string TeacherName { get; set; }

        /// <summary>
        /// 对应帐号
        /// </summary>
        [Display(Name = "对应帐号"), Required]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 所属校区
        /// </summary>
        [Display(Name = "所属校区")]
        public virtual Basis.Entity.tbSchool tbSchool { get; set; }

        ///// <summary>
        ///// 教师部门
        ///// </summary>
        //[Display(Name = "教师部门")]
        //public virtual tbTeacherDept tbTeacherDept { get; set; }

        /// <summary>
        /// 教师类型
        /// </summary>
        [Display(Name = "教师类型")]
        public virtual tbTeacherType tbTeacherType { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        [Display(Name = "学历")]
        public virtual Dict.Entity.tbDictEducation tbDictEducation { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        [Display(Name = "政治面貌")]
        public virtual Dict.Entity.tbDictParty tbDictParty { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [Display(Name = "民族")]
        public virtual Dict.Entity.tbDictNation tbDictNation { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        [Display(Name = "婚姻状况")]
        public virtual Dict.Entity.tbDictMarriage tbDictMarriage { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        [Display(Name = "籍贯")]
        public virtual Dict.Entity.tbDictRegion tbDictRegion { get; set; }

        /// <summary>
        /// 健康状况
        /// </summary>
        [Display(Name = "健康状况")]
        public virtual Dict.Entity.tbDictHealth tbDictHealth { get; set; }

        /// <summary>
        /// 个人简介
        /// </summary>
        [Display(Name = "个人简介")]
        public string Profile { get; set; }
    }
}