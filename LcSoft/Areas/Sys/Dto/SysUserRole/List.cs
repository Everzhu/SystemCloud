using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Sys.Dto.SysUserRole
{
    public class List
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户帐号
        /// </summary>
        [Display(Name = "用户帐号")]
        public string SysUserCode { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [Display(Name = "对应用户")]
        public string SysUserName { get; set; }
    }
}
