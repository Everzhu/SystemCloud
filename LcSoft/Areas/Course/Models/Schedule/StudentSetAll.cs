using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class StudentSetAll
    {
        /// <summary>
        /// 学年
        /// </summary>
        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        /// <summary>
        /// 查询关键字
        /// </summary>
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        /// <summary>
        /// 学年ID
        /// </summary>
        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        /// <summary>
        /// 行政班级
        /// </summary>
        public List<Areas.Basis.Dto.Class.Info> ClassInfoList { get; set; } = new List<Basis.Dto.Class.Info>();

        /// <summary>
        /// 行政班级Id
        /// </summary>
        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        /// <summary>
        /// 学生列表
        /// </summary>
        public List<Areas.Student.Dto.Student.Info> StudentInfoList { get; set; } = new List<Student.Dto.Student.Info>();
    }
}