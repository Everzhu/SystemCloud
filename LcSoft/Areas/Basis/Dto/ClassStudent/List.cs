using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Basis.Dto.ClassStudent
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        [Display(Name = "班级ID")]
        public int? ClassId { get; set; }

        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public string ClassName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生Id
        /// </summary>
        [Display(Name = "学生Id")]
        public int StudentId { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexName { get; set; }

        /// <summary>
        /// 座位号
        /// </summary>
        [Display(Name = "座位号")]
        public int? No { get; set; }

        /// <summary>
        /// 行政班小组
        /// </summary>
        [Display(Name = "行政班小组")]
        public string ClassGroupName { get; set; }
    }
}
