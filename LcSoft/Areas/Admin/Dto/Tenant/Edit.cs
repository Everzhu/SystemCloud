using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Admin.Dto.Tenant
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 租户名称
        /// </summary>
        [Display(Name = "租户名称"), Required]
        public string TenantName { get; set; }

        /// <summary>
        /// 系统标题
        /// </summary>
        [Display(Name = "系统标题")]
        public string Title { get; set; }

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
        public string Power { get; set; }

        /// <summary>
        /// 注册码
        /// </summary>
        public string Cdkey { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 是否VIP
        /// </summary>
        [Required]
        public decimal IsVip { get; set; }

        /// <summary>
        /// 最后期限
        /// </summary>
        [Display(Name = "最后期限"), Required]
        public DateTime Deadine { get; set; } = new DateTime(2099, 12, 30);

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 管理员帐号
        /// </summary>
        [Display(Name = "管理员帐号"), Required]
        public string AdminLoginCode { get; set; }

        /// <summary>
        /// 管理员密码
        /// </summary>
        [Display(Name = "管理员密码"), Required]
        public string AdminPassword { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        [Display(Name = "确认密码"), Required, Compare("AdminPassword")]
        public string PasswordConfirm { get; set; }

        /// <summary>
        /// Logo
        /// </summary>
        [Display(Name = "Logo")]
        public string Logo { get; set; }
    }
}
