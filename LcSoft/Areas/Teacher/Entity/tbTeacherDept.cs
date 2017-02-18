using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Teacher.Entity
{
    /// <summary>
    /// 部门
    /// </summary>
    public class tbTeacherDept : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [Display(Name = "部门名称"), Required]
        public string TeacherDeptName { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>
        [Display(Name = "上级部门")]
        public virtual tbTeacherDept tbTeacherDeptParent { get; set; }

        /// <summary>
        /// 所属校区
        /// </summary>
        [Display(Name = "所属校区")]
        public virtual Basis.Entity.tbSchool tbSchool { get; set; }
    }
}
