using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Perform.Dto.PerformComment
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 所属学生
        /// </summary>
        [Display(Name = "所属学生")]
        public string StudentName { get; set; }

        /// <summary>
        /// 所属学年（取学期）
        /// </summary>
        [Display(Name = "所属学年")]
        public string YearName { get; set; }

        /// <summary>
        /// 学年Id
        /// </summary>
        [Display(Name = "学年Id")]
        public int YearId { get; set; }

        /// <summary>
        /// 评语内容
        /// </summary>
        [Display(Name = "评语内容")]
        public string Comment { get; set; }

        /// <summary>
        /// 录入日期
        /// </summary>
        [Display(Name = "录入日期")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 录入教师
        /// </summary>
        [Display(Name = "录入教师")]
        public string SysUserName { get; set; }

        /// <summary>
        /// 录入教师
        /// </summary>
        [Display(Name = "学生Id")]
        public int StudentId { get; set; }
    }
}
