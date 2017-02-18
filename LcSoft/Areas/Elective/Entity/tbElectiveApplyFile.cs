using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Entity
{
    /// <summary>
    /// 选课申报附件
    /// </summary>
    public class tbElectiveApplyFile: Code.EntityHelper.EntityBase
    {
        [Display(Name ="所属选课申报"),Required]
        public virtual tbElectiveApply tbElectiveApply { get; set; }


        [Display(Name ="文件名称"),Required]
        public string FileName { get; set; }

        [Display(Name ="文件标题"),Required]
        public string FileTitle { get; set; }
    }
}