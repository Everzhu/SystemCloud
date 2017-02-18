using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Moral.Dto.MoralClass
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 参评班级 
        /// </summary>
        [Display(Name = "参评班级")]
        public string ClassName { get; set; }

        /// <summary>
        /// 所属德育
        /// </summary>
        [Display(Name = "所属德育")]
        public string MoralName { get; set; }
    }
}
