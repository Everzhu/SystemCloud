using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 教室类型
    /// </summary>
    public class tbRoomType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教室类型
        /// </summary>
        [Required]
        [Display(Name = "教室类型")]
        public string RoomTypeName { get; set; }
    }
}