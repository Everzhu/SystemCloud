using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Survey.Dto.SurveyGroup
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
        /// 评价分组
        /// </summary>
        [Display(Name = "评价分组"), Required]
        public string SurveyGroupName { get; set; }

        /// <summary>
        /// 评教方式
        /// </summary>
        [Display(Name = "评教方式")]
        public bool IsOrg { get; set; }
    }
}
