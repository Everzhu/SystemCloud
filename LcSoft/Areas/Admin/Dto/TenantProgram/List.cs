using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Admin.Dto.TeanntProgram
{
    public class List
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Display(Name = "序号"), Required]
        public int? No { get; set; }

        /// <summary>
        /// 程序类型
        /// </summary>
        [Display(Name = "程序类型"), Required]
        public string TeanntProgramName { get; set; }
    }
}