using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 教育程度
    /// </summary>
    public class tbDictEducation : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学历名称
        /// </summary>
        [Display(Name = "学历名称"), Required]
        public string EducationName { get; set; }

        /// <summary>
        /// 对应学位
        /// </summary>
        [Display(Name = "对应学位"), Required]
        public virtual tbDictDegree tbDictDegree { get; set; }
    }
}