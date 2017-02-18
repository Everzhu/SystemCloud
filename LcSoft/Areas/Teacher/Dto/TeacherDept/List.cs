using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Teacher.Dto.TeacherDept
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Display(Name = "部门")]
        public string TeacherDeptName { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>
        [Display(Name = "上级部门")]
        public string TeacherDeptParentName { get; set; }
    }
}
