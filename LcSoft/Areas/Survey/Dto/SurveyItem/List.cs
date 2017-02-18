using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Survey.Dto.SurveyItem
{
    public class List
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
        /// 评教组
        /// </summary>
        [Display(Name = "评教组")]
        public int SurveyGroupId { get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        [Display(Name = "评价内容")]
        public string SurveyItemName { get; set; }

        /// <summary>
        /// 选项数
        /// </summary>
        [Display(Name = "选项数")]
        public int OptionCount { get; set; }

        /// <summary>
        /// 选项布局
        /// </summary>
        [Display(Name = "选项布局")]
        public bool IsVertical { get; set; }

        /// <summary>
        /// 试题类型
        /// </summary>
        [Display(Name = "试题类型")]
        public Code.EnumHelper.SurveyItemType SurveyItemType { get; set; }

        /// <summary>
        /// 字数限制
        /// </summary>
        [Display(Name = "字数限制")]
        public int TextMaxLength { get; set; }
    }
}
