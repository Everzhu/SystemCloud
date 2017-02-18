using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Perform.Dto.PerformComment
{
    public class Info
    {
        public int Id { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 教师名称
        /// </summary>
        [Display(Name = "教师名称")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 所属学年（取学期）
        /// </summary>
        [Display(Name = "所属学年")]
        public int YearId { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年")]
        public string YearName { get; set; }

        /// <summary>
        /// 评语内容
        /// </summary>
        [Display(Name = "评语内容")]
        public string Comment { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        [Display(Name = "录入时间")]
        public DateTime InputDate { get; set; }
    }
}
