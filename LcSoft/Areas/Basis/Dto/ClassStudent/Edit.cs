using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Basis.Dto.ClassStudent
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 座位号
        /// </summary>
        [Display(Name = "座位号"), Required]
        public int? No { get; set; }
    }
}
