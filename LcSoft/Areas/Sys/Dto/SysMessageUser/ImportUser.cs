using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Dto.SysMessageUser
{
    public class ImportUser
    {
        /// <summary>
        /// 用户帐号
        /// </summary>
        [Display(Name = "id")]
        public int Id { get; set; }
        /// <summary>
        /// 用户帐号
        /// </summary>
        [Display(Name = "用户帐号")]
        public string UserCode { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        [Display(Name = "用户姓名")]
        public string UserName { get; set; }
    }
}