using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Entity
{
    /// <summary>
    /// 教师所在部门表
    /// </summary>
    public class tbTeacherWithDept : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教师
        /// </summary>
        [Display(Name = "教师"), Required]
        public virtual tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        [Display(Name = "部门"), Required]
        public virtual tbTeacherDept tbTeacherDept { get; set; }

        //选课新版数据库异常提示缺下面字段，手动补
        //public string Remark { get; set; }

        //public int tbKpi_Id { get; set; }
    }
}