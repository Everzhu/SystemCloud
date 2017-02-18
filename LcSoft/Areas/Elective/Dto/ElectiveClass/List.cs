using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Elective.Dto.ElectiveClass
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 参选班级 
        /// </summary>
        [Display(Name = "参选班级")]
        public string ClassName { get; set; }

        /// <summary>
        /// 所属选课
        /// </summary>
        [Display(Name = "所属选课")]
        public string ElectiveName { get; set; }
    }
}
