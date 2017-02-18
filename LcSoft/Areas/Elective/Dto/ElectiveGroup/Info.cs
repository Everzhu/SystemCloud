using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.ElectiveGroup
{
    public class Info
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        [Display(Name = "分组名称")]
        public string ElectiveGroupName { get; set; }

        /// <summary>
        /// 最大选课数
        /// </summary>
        [Display(Name = "最大选课数")]
        public int MaxElective { get; set; }

        /// <summary>
        /// 最小选课数
        /// </summary>
        [Display(Name = "最小选课数")]
        public int MinElective { get; set; }
    }
}
