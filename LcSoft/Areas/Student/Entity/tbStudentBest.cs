using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Student.Entity
{
    /// <summary>
    /// 优秀学生
    /// </summary>
    public class tbStudentBest : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学生
        /// </summary>
        [Display(Name = "学生"), Required]
        public virtual tbStudent tbStudent { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        [Display(Name = "录入时间"), Required]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "录入人员"), Required]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }
    }
}
