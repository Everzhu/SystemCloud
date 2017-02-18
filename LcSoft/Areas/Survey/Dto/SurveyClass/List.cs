using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Survey.Dto.SurveyClass
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
        public string SurveyName { get; set; }

        /// <summary>
        /// 参评班级
        /// </summary>
        [Display(Name = "参评班级")]
        public string ClassName { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public Basis.Entity.tbClass tbClass { get; set; }
    }
}
