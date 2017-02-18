using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class StudentSetOrg
    {
        /// <summary>
        /// 教学班
        /// </summary>
        public List<Dto.Org.List> OrgList { get; set; } = new List<Dto.Org.List>();

        /// <summary>
        /// 学生ID
        /// </summary>
        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        /// <summary>
        /// 学年ID
        /// </summary>
        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        /// <summary>
        /// 星期ID
        /// </summary>
        public int WeekId { get; set; } = System.Web.HttpContext.Current.Request["WeekId"].ConvertToInt();

        /// <summary>
        /// 节次ID
        /// </summary>
        public int PeriodId { get; set; } = System.Web.HttpContext.Current.Request["PeriodId"].ConvertToInt();

        /// <summary>
        /// 关键字查询
        /// </summary>
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        /// <summary>
        /// 教学班ID
        /// </summary>
        public int OrgId { get; set; } = System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();

        /// <summary>
        /// 新选择的教学班ID
        /// </summary>
        public int SelectedOrgId { get; set; }

        /// <summary>
        /// 操作状态
        /// </summary>
        public bool Status { get; set; }
    }
}