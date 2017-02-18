using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Teacher.Dto.TeacherDept
{
    /// <summary>
    /// 部门编辑
    /// </summary>
    public class Edit
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Display(Name = "部门名称"), Required]
        public string TeacherDeptName { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>
        [Display(Name = "上级部门")]
        public int? TeacherDeptParentId { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>
        [Display(Name = "上级部门")]
        public string TeacherDeptParentName { get; set; }
    }
}
