using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.ElectiveOrgClass
{
    public class List
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称")]
        public string ClassName { get; set; }

        /// <summary>
        ///  年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }

        /// <summary>
        /// 班级类型
        /// </summary>
        [Display(Name = "班级类型")]
        public string ClassTypeName { get; set; }

        /// <summary>
        /// 班级名额
        /// </summary>
        [Display(Name = "班级名额")]
        public int MaxLimit { get; set; }

        /// <summary>
        /// 选中
        /// </summary>
        public bool IsChecked { get; set; }
    }
}
