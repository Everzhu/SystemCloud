using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Basis.Dto.Year
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年")]
        public string YearName { get; set; }

        /// <summary>
        /// 状态（开启/禁用)
        /// </summary>
        [Display(Name = "状态")]
        public bool IsDisable { get; set; }

        /// <summary>
        /// 默认
        /// </summary>
        [Display(Name = "默认")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [Display(Name = "开始日期")]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        [Display(Name = "结束日期")]
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// 学年类型
        /// </summary>
        [Display(Name = "学年类型")]
        public Code.EnumHelper.YearType YearType { get; set; }


        /// <summary>
        /// 父级Id
        /// </summary>
        public int ParentId { get; set; }

        public int GrandFatherId { get; set; }


        public List<XkSystem.Areas.Basis.Dto.Year.List> ChildList { get; set; } = new List<List>();
    }
}
