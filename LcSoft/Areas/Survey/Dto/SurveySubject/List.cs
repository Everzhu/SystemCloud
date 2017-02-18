using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Survey.Dto.SurveySubject
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
        /// 所属评教
        /// </summary>
        [Display(Name = "所属评教")]
        public string SurveyGroupName { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "科目")]
        public string SubjectName { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public int? SubjectId { get; set; }

        /// <summary>
        /// 评教组ID
        /// </summary>
        public int SurveyGroupId { get; set; }
    }
}
