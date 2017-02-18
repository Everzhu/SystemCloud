using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Moral.Dto.MoralClass
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 参评班级 
        /// </summary>
        [Display(Name = "参评班级"), Required]
        public int ClassId { get; set; }

        /// <summary>
        /// 所属德育
        /// </summary>
        [Display(Name = "所属德育"), Required]
        public int ElectiveId { get; set; }
    }
}
