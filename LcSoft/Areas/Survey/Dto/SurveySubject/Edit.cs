using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Survey.Dto.SurveySubject
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 所属评教
        /// </summary>
        [Display(Name = "所属评教"), Required]
        public string SurveyGroupName { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "科目"), Required]
        public string SubjectName { get; set; }
    }
}
