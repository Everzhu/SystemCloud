using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Teacher.Entity
{
    /// <summary>
    /// 荣誉类型
    /// </summary>
    public class tbTeacherHonorType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 荣誉类型
        /// </summary>
        [Display(Name = "荣誉类型"), Required]
        public string TeacherHonorTypeName { get; set; }
    }
}
