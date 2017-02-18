using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveInputByBase
{
    public class List
    {
        /// <summary>
        /// 分段列表
        /// </summary>
        public List<Dto.ElectiveSection.Info> ElectiveSectionList { get; set; } = new List<Dto.ElectiveSection.Info>();

        /// <summary>
        /// 分组列表
        /// </summary>
        public List<Dto.ElectiveGroup.Info> ElectiveGroupList { get; set; } = new List<Dto.ElectiveGroup.Info>();

        /// <summary>
        /// 课程列表
        /// </summary>
        public List<Dto.ElectiveOrg.List> ElectiveOrgList { get; set; } = new List<Dto.ElectiveOrg.List>();

        /// <summary>
        /// 当前登录学生
        /// </summary>
        public XkSystem.Areas.Student.Dto.Student.SelectStudent Student { get; set; } = new XkSystem.Areas.Student.Dto.Student.SelectStudent();

        public string ElectiveName { get; set; }

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public bool IsOpen { get; set; }

        public bool IsEnd { get; set; }

        public bool IsWeekPeriod { get; set; }

        public bool IsHiddenSection { get; set; }

        public bool IsHiddenGroup { get; set; }
    }
}