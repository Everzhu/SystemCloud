using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Perform.Dto.PerformComment
{
    public class ReportAll
    {
        public int Id { get; set; }

        [Display(Name = "班级名称")]
        public string ClassName { get; set; }

        [Display(Name = "班级人数")]
        public int TotalCount { get; set; }

        [Display(Name = "未录人数")]
        public int UnCount { get; set; }
    }
}
