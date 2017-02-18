using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static XkSystem.Code.PageHelper;

namespace XkSystem.Areas.Elective.Models.ElectiveReport
{
    public class SolutionList
    {
        public List<System.Web.Mvc.SelectListItem> ElectiveList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public List<Solution> MySolutionList = new List<Solution>();


        public class mySolution
        {
            public int StudentId { get; set; }

            public int OrgId { get; set; }
        }

        public class MyStudentGroup
        {
            public int StudentId { get; set; }

            public string StrValue { get; set; }
        }

        public class Solution
        {
            public int StudentId { get; set; }

            public int StudentCount { get; set; }

            public string StrValue { get; set; }
        }
    }
}