using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 纸张大小
    /// </summary>
    public class tbDictPaperSize : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 纸张大小
        /// </summary>
        [Display(Name = "纸张大小"), Required]
        public string PaperSizeName { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        [Display(Name = "高度"), Required]
        public int Height { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        [Display(Name = "宽度"), Required]
        public int Width { get; set; }
    }
}