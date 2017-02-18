using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Perform.Dto.PerformComment
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 所属学生
        /// </summary>
        [Display(Name = "所属学生"), Required]
        public int StudentId { get; set; }

        /// <summary>
        /// 所属学年（取学期）
        /// </summary>
        [Display(Name = "所属学年"), Required]
        public int YearId { get; set; }

        /// <summary>
        /// 评语内容
        /// </summary>
        [Display(Name = "评语内容")]
        public string Comment { get; set; }
    }
}
