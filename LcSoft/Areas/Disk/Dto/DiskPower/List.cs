using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Disk.Dto.DiskPower
{
    public class List
    {
        public int Id { get; set; }

        public int DiskFolderId { get; set; }

        [Display(Name = "管理权限")]
        public bool IsAdmin { get; set; }

        [Display(Name = "上传权限")]
        public bool IsInput { get; set; }

        [Display(Name = "查看权限")]
        public bool IsView { get; set; }

        public int UserId { get; set; }

        [Display(Name = "用户名")]
        public string UserCode { get; set; }

        [Display(Name = "姓名")]
        public string UserName { get; set; }

        [Display(Name = "用户类型")]
        public Code.EnumHelper.SysUserType UserType { get; set; }
    }
}