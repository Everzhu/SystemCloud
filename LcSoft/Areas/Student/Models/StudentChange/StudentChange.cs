using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentChange
{
    public class StudentChange
    {
        /// <summary>
        /// 学生ID
        /// </summary>
        public string StudentCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 离校类型
        /// </summary>
        public string StudentChangeName { get; set; }
    }
}