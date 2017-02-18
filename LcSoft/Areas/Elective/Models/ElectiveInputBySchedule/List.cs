using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Models.ElectiveInputBySchedule
{
    public class List
    {
      
        public List<Dto.ElectiveSection.Info> ElectiveSectionList { get; set; } = new List<Dto.ElectiveSection.Info>();

       
        public List<Dto.ElectiveGroup.Info> ElectiveGroupList { get; set; } = new List<Dto.ElectiveGroup.Info>();

        /// <summary>
        /// 课程列表
        /// </summary>
        public List<Dto.ElectiveInputBySchedule.List> ElectiveOrgList { get; set; } = new List<Dto.ElectiveInputBySchedule.List>();

        /// <summary>
        /// 当前登录学生
        /// </summary>
        public XkSystem.Areas.Student.Dto.Student.SelectStudent Student { get; set; } = new XkSystem.Areas.Student.Dto.Student.SelectStudent();


        public List<SelectListItem> WeekList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PeriodList { get; set; } = new List<SelectListItem>();

        public string ElectiveName { get; set; }

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public bool IsOpen { get; set; }

        public bool IsEnd { get; set; }

        public bool IsWeekPireod { get; set; }

    }


}