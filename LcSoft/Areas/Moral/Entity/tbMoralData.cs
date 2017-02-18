using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    /// <summary>
    /// 德育记录
    /// </summary>
    public class tbMoralData : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学生
        /// </summary>
        [Display(Name = "学生")]

        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级")]
        public virtual Basis.Entity.tbClass tbClass { get; set; }

        /// <summary>
        /// 小组
        /// </summary>
        [Display(Name = "小组")]
        public virtual Basis.Entity.tbClassGroup tbClassGroup { get; set; }

        /// <summary>
        /// 评分日期
        /// </summary>
        [Required]
        [Display(Name = "评分日期")]
        public DateTime MoralDate { get; set; }

        /// <summary>
        /// 德育项目
        /// </summary>
        [Required]
        [Display(Name = "德育项目")]
        public virtual tbMoralItem tbMoralItem { get; set; }

        /// <summary>
        /// 德育选项
        /// </summary>
        [Display(Name = "德育选项")]
        public virtual tbMoralOption tbMoralOption { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public decimal? DataText { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        [Display(Name = "录入时间")]
        [Required]
        public DateTime InputDate { get; set; }


        /// <summary>
        /// 评分备注、评语
        /// </summary>
        [Required]
        [Display(Name = "评分备注")]
        public string Comment { get; set; }

        /// <summary>
        /// 操作方式 打分、评语
        /// </summary>
        [Required]
        [Display(Name = "操作方式")]
        public Code.EnumHelper.MoralItemOperateType MoralItemOperateType { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Required]
        [Display(Name = "录入人员")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }

        ///// <summary>
        ///// 备注说明
        ///// </summary>
        //[Display(Name = "备注说明")]
        //public string Remark { get; set; }

        [Display(Name ="评分原因")]
        public virtual tbMoralDataReason tbMoralDataReason { get; set; }


        [Display(Name = "审核状态")]
        public Code.EnumHelper.CheckStatus CheckStatus { get; set; } = Code.EnumHelper.CheckStatus.None;

    }
}