using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Teacher.Entity
{
    /// <summary>
    /// 荣誉级别
    /// </summary>
    public class tbTeacherHonorLevel : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 荣誉级别
        /// </summary>
        [Display(Name = "荣誉级别"), Required]
        public string TeacherHonorLevelName { get; set; }
    }
}
