using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveInputByBase
{
    public class History
    {
        /// <summary>
        /// 课程列表
        /// </summary>
        public List<Dto.ElectiveOrg.List> ElectiveOrgList { get; set; } = new List<Dto.ElectiveOrg.List>();

        public XkSystem.Areas.Student.Dto.Student.SelectStudent Student { get; set; } = new XkSystem.Areas.Student.Dto.Student.SelectStudent();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public string ElectiveName { get; set; }

    }
}