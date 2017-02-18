using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Code
{
    public class EntityHelper
    {
        /// <summary>
        /// 公共类
        /// </summary>
        public class EntityBase : Code.EntityHelper.EntityRoot
        {
            /// <summary>
            /// 租户
            /// </summary>
            [Required]
            public virtual Areas.Admin.Entity.tbTenant tbTenant
            {
                get;
                set;
            }
        }

        public class EntityRoot
        {
            /// <summary>
            /// ID
            /// </summary>
            [Key]
            public int Id { get; set; }

            /// <summary>
            /// 排序
            /// </summary>
            [Display(Name = "序号")]
            public int? No { get; set; }

            /// <summary>
            /// 记录状态
            /// </summary>
            [Required]
            public bool IsDeleted { get; set; }

            public DateTime UpdateTime { get; set; } = DateTime.Now;
        }
    }
}