using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.ElectiveOrgStudent
{
    public class List
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 所属选课开班
        /// </summary>
        [Display(Name = "所属选课开班")]
        public string ElectiveOrgName { get; set; }

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
        /// 允许学生修改
        /// </summary>
        [Display(Name = "允许学生修改")]
        public bool IsFixed { get; set; }

        /// <summary>
        /// 默认选中
        /// </summary>
        [Display(Name = "默认选中")]
        public bool IsChecked { get; set; }
    }
}
