using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveRule
{
    public class List
    {
        public List<Dto.ElectiveRule.List> ElectiveRuleList { get; set; } = new List<Dto.ElectiveRule.List>();

        public List<Course.Dto.Course.SimpleInfo> CourseList { get; set; } = new List<Course.Dto.Course.SimpleInfo>();

        public string ElectiveName { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();
    }
}
