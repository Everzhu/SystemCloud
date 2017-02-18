using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Dto.SysUser
{
    public class Info
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string Portrait { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string UserTypeName { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 班主任
        /// </summary>
        public string TeacherName { get; set; }

        public bool NeedAlert { get; set; } = false;
    }
}