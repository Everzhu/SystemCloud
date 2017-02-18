using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveRule
{
    public class Edit
    {
        public Dto.ElectiveRule.Edit ElectiveRuleEdit { get; set; } = new Dto.ElectiveRule.Edit();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public int CourseId { get; set; } = HttpContext.Current.Request["CourseId"].ConvertToInt();

        public int RuleId { get; set; } = HttpContext.Current.Request["RuleId"].ConvertToInt();

        public List<Course.Dto.Course.Info> CourseList { get; set; } = new List<Course.Dto.Course.Info>();

        public List<int> RuleCourseIds { get; set; } = new List<int>();

    }
}