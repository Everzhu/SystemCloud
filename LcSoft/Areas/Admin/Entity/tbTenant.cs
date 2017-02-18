using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Admin.Entity
{
    /// <summary>
    /// 学校
    /// </summary>
    public class tbTenant : Code.EntityHelper.EntityRoot
    {
        /// <summary>
        /// 学校名称
        /// </summary>
        [Display(Name = "学校名称"), Required]
        public string TenantName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        public string Title { get; set; }

        /// <summary>
        /// 上级学校（可用于集团）
        /// </summary>
        [Display(Name = "上级学校")]
        public virtual tbTenant tbTenantParent { get; set; }

        /// <summary>
        /// 主机头（访问时的二级域名）
        /// </summary>
        [Display(Name = "主机头"), Required]
        public string Host { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        [Display(Name = "是否默认"), Required]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 权限(直接就是各个Area的名称，以一个格式存放，模块的授权，同MVC Area的名称)
        /// </summary>
        [Display(Name = "权限")]
        public string Power { get; set; }

        /// <summary>
        /// 注册码
        /// </summary>
        [Display(Name = "注册码")]
        public string Cdkey { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        [Display(Name = "联系地址")]
        public string Address { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Display(Name = "联系电话")]
        public string Phone { get; set; }

        /// <summary>
        /// 是否VIP
        /// </summary>
        [Display(Name = "是否VIP"), Required]
        public decimal IsVip { get; set; }

        /// <summary>
        /// 使用期限
        /// </summary>
        [Display(Name = "使用期限"), Required]
        public DateTime Deadine { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "备注信息")]
        public string Remark { get; set; }

        /// <summary>
        /// Logo
        /// </summary>
        [Display(Name = "Logo")]
        public string Logo { get; set; }
    }
}